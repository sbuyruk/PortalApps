using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.Logging;

namespace DAO.Repositories.Portal
{
    public sealed class PortalToplantiOutboxItem
    {
        public int Id { get; internal set; }
        public string Payload { get; internal set; }
        public string DeliveryPlan { get; internal set; }
        public DateTime CreatedUtc { get; internal set; }
        public Guid Lease { get; internal set; }
        public PortalLogContext Context { get; internal set; }
    }

    public sealed class PortalToplantiOutboxRepository
    {
        private readonly string connectionString;
        public PortalToplantiOutboxRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Bağlantı bilgisi gereklidir.", "connectionString");
            this.connectionString = connectionString;
        }

        // Enqueue participates in the meeting transaction; it never opens another connection.
        public static void Enqueue(int meetingId, string payload, PortalLogContext context, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            var query = new SqlQuery(@"INSERT INTO dbo.PortalToplantiOutbox_Table
                (MeetingId,Payload,CreatedUtc,NextAttemptUtc,Status,AttemptCount,UserLogin,WebPart,PagePath,OperationId)
                VALUES (@MeetingId,@Payload,SYSUTCDATETIME(),SYSUTCDATETIME(),'Pending',0,@User,@WebPart,@Page,@Operation)");
            query.AddParameter("@MeetingId", meetingId); query.AddParameter("@Payload", payload);
            query.AddParameter("@User", context.UserLogin); query.AddParameter("@WebPart", context.WebPart);
            query.AddParameter("@Page", context.PagePath); query.AddParameter("@Operation", context.OperationId);
            if (new PortalSqlExecutor(context, "PortalToplantiOutboxRepository").Insert(query, transaction) <= 0)
                throw new InvalidOperationException("Toplantı bildirimi kuyruğa alınamadı.");
        }

        public PortalToplantiOutboxItem Claim()
        {
            PortalToplantiOutboxItem item = null;
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                var query = new SqlQuery(@";WITH candidate AS
                    (SELECT TOP (1) Q.* FROM dbo.PortalToplantiOutbox_Table Q WITH (UPDLOCK,HOLDLOCK)
                     WHERE ((Q.Status='Pending' AND Q.NextAttemptUtc<=SYSUTCDATETIME())
                        OR (Q.Status='Processing' AND Q.LeaseUntilUtc<SYSUTCDATETIME()))
                     AND NOT EXISTS (SELECT 1 FROM dbo.PortalToplantiOutbox_Table P
                         WHERE P.MeetingId=Q.MeetingId AND P.Id<Q.Id AND P.Status IN ('Pending','Processing'))
                     ORDER BY Q.Id)
                    UPDATE candidate SET Status='Processing',LeaseId=@Lease,
                        LeaseUntilUtc=DATEADD(minute,5,SYSUTCDATETIME()),AttemptCount=AttemptCount+1
                    OUTPUT inserted.*;");
                Guid lease = Guid.NewGuid(); query.AddParameter("@Lease", lease);
                DataTable rows = transaction.Select(query);
                if (rows != null)
                {
                    DataRow row = rows.Rows[0];
                    item = new PortalToplantiOutboxItem { Id = Convert.ToInt32(row["Id"]), Payload = Convert.ToString(row["Payload"]),
                        DeliveryPlan = row.IsNull("DeliveryPlan") ? null : Convert.ToString(row["DeliveryPlan"]),
                        CreatedUtc = DateTime.SpecifyKind((DateTime)row["CreatedUtc"], DateTimeKind.Utc), Lease = lease,
                        Context = new PortalLogContext("Portal", Convert.ToString(row["WebPart"]), Convert.ToString(row["UserLogin"]),
                            Convert.ToString(row["PagePath"]), (Guid)row["OperationId"]) };
                }
                transaction.Complete();
            }
            return item;
        }

        public HashSet<string> Delivered(int id)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                var query = new SqlQuery("SELECT Recipient FROM dbo.PortalToplantiDelivery_Table WHERE OutboxId=@Id");
                query.AddParameter("@Id", id);
                DataTable rows = transaction.Select(query);
                if (rows != null) foreach (DataRow row in rows.Rows) result.Add(Convert.ToString(row["Recipient"]));
                transaction.Complete();
            }
            return result;
        }

        public void Renew(PortalToplantiOutboxItem item)
        {
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                Renew(item, transaction);
                transaction.Complete();
            }
        }

        private static void Renew(PortalToplantiOutboxItem item, SqlTransactionContext transaction)
        {
            var query = new SqlQuery(@"UPDATE dbo.PortalToplantiOutbox_Table SET LeaseUntilUtc=DATEADD(minute,5,SYSUTCDATETIME())
                WHERE Id=@Id AND LeaseId=@Lease AND Status='Processing' AND LeaseUntilUtc>SYSUTCDATETIME()");
            query.AddParameter("@Id", item.Id); query.AddParameter("@Lease", item.Lease);
            if (transaction.ExecuteAffectedRows(query) != 1) throw new InvalidOperationException("Bildirim işleme hakkı sona erdi; gönderim durduruldu.");
        }

        public void MarkDelivered(PortalToplantiOutboxItem item, string recipient)
        {
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                Renew(item, transaction);
                var query = new SqlQuery(@"IF NOT EXISTS (SELECT 1 FROM dbo.PortalToplantiDelivery_Table WHERE OutboxId=@Id AND Recipient=@Recipient)
                    INSERT INTO dbo.PortalToplantiDelivery_Table (OutboxId,Recipient,SentUtc) VALUES (@Id,@Recipient,SYSUTCDATETIME())");
                query.AddParameter("@Id", item.Id); query.AddParameter("@Recipient", recipient);
                transaction.Update(query);
                transaction.Complete();
            }
        }

        public void SavePlan(PortalToplantiOutboxItem item, string plan)
        {
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                Renew(item, transaction);
                var query = new SqlQuery("UPDATE dbo.PortalToplantiOutbox_Table SET DeliveryPlan=@Plan WHERE Id=@Id AND LeaseId=@Lease AND DeliveryPlan IS NULL");
                query.AddParameter("@Id", item.Id); query.AddParameter("@Lease", item.Lease); query.AddParameter("@Plan", plan);
                if (transaction.ExecuteAffectedRows(query) != 1) throw new InvalidOperationException("Bildirim alıcı planı kaydedilemedi.");
                transaction.Complete();
            }
        }

        public void Finish(PortalToplantiOutboxItem item, bool succeeded, string error)
        {
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                Renew(item, transaction);
                var query = new SqlQuery(@"UPDATE dbo.PortalToplantiOutbox_Table SET
                    Status=CASE WHEN @Success=1 THEN 'Sent' WHEN AttemptCount>=8 THEN 'Failed' ELSE 'Pending' END,
                    NextAttemptUtc=DATEADD(minute,CASE WHEN AttemptCount>=6 THEN 60 ELSE POWER(2,AttemptCount) END,SYSUTCDATETIME()),
                    SentUtc=CASE WHEN @Success=1 THEN SYSUTCDATETIME() ELSE NULL END,LastError=@Error,LeaseId=NULL,LeaseUntilUtc=NULL
                    WHERE Id=@Id AND LeaseId=@Lease AND Status='Processing'");
                query.AddParameter("@Id", item.Id); query.AddParameter("@Lease", item.Lease);
                query.AddParameter("@Success", succeeded); query.AddParameter("@Error", succeeded ? (object)DBNull.Value : error);
                if (transaction.ExecuteAffectedRows(query) != 1) throw new InvalidOperationException("Bildirim sonucu kaydedilemedi.");
                transaction.Complete();
            }
        }

        public string Parameter(string key)
        {
            var query = new SqlQuery("SELECT TOP (1) Deger FROM OrtakParametre_Table WHERE Anahtar=@Key ORDER BY Sira,Deger");
            query.AddParameter("@Key", key);
            return ScalarText(query, "Deger");
        }

        public string Address(int personId)
        {
            var query = new SqlQuery("SELECT TOP (1) IntranetEPosta FROM IletisimBilgileri_Table WHERE PersonelId=@Id ORDER BY Adres");
            query.AddParameter("@Id", personId);
            return ScalarText(query, "IntranetEPosta");
        }

        private string ScalarText(SqlQuery query, string field)
        {
            string value;
            using (var transaction = new SqlTransactionContext(connectionString))
            {
                DataTable rows = transaction.Select(query);
                value = rows == null || rows.Rows[0].IsNull(field) ? null : Convert.ToString(rows.Rows[0][field]);
                transaction.Complete();
            }
            return value;
        }
    }
}

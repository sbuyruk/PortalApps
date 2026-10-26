using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.Logging;
using Utility.ProjeGlobal;

namespace DAO.Repositories.Portal
{
    public sealed class PortalToplantiWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly PortalSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();
        public PortalToplantiWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new PortalSqlExecutor(context, "PortalToplantiWriteRepository");
        }

        // One lock order for meeting, participant and parameter writers. No SMTP under this lock.
        public void Lock(SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=N'PortalApps.Portal.Toplanti',
                    @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
                SELECT @result Result;");
            DataTable rows = sql.Select(query, transaction);
            if (rows == null || Convert.ToInt32(rows.Rows[0]["Result"]) < 0)
                throw new InvalidOperationException("Toplantı işlem kilidi alınamadı.");
        }

        public DataTable Read(string table, int id, SqlTransactionContext transaction)
        {
            Require(transaction); Fields(table);
            var query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable Participants(int id, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT * FROM ToplantiKatilim_Table WITH (UPDLOCK,HOLDLOCK) WHERE ToplantiId=@Id ORDER BY Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable CurrentPerson(SqlTransactionContext transaction)
        {
            Require(transaction);
            string login = context.UserLogin ?? string.Empty;
            login = login.Substring(login.LastIndexOf('\\') + 1);
            if (string.IsNullOrWhiteSpace(login)) return null;
            // Return up to two people so the service can reject ambiguous account bindings.
            // Multiple job-history rows must not turn one person into multiple candidates.
            var query = new SqlQuery(@"SELECT TOP (2) P.Id,I.BirimId
                FROM Personel_Table P WITH (HOLDLOCK)
                OUTER APPLY
                (
                    SELECT TOP (1) B.BirimId FROM IsBilgileri_Table B WITH (HOLDLOCK)
                    WHERE B.PersonelId=P.Id ORDER BY B.Id DESC
                ) I
                WHERE P.KullaniciAdi=@Login ORDER BY P.Id");
            query.AddParameter("@Login", login);
            return sql.Select(query, transaction);
        }

        public bool SameUnit(int ownerId, int unitId, SqlTransactionContext transaction)
        {
            Require(transaction);
            if (unitId <= 0) return false;
            var query = new SqlQuery(@"SELECT TOP (1) BirimId FROM IsBilgileri_Table WITH (HOLDLOCK)
                WHERE PersonelId=@Owner ORDER BY Id DESC");
            query.AddParameter("@Owner", ownerId);
            DataTable rows = sql.Select(query, transaction);
            return rows != null && !rows.Rows[0].IsNull("BirimId")
                && Convert.ToInt32(rows.Rows[0]["BirimId"]) == unitId;
        }

        public bool PersonExists(int id, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT Id FROM Personel_Table WITH (HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction) != null;
        }

        public bool Overlaps(int id, int room, DateTime start, DateTime end, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery(@"SELECT TOP (1) Id FROM Toplanti_Table WITH (UPDLOCK,HOLDLOCK)
                WHERE Id<>@Id AND ToplantiYeri=@Room AND BaslangicTarihi<=@End AND BitisTarihi>=@Start");
            query.AddParameter("@Id", id); query.AddParameter("@Room", room);
            query.AddParameter("@Start", start); query.AddParameter("@End", end);
            return room != ProjeConstants.PARAM_DIGER_INT && sql.Select(query, transaction) != null;
        }

        public bool ParameterInUse(int id, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT TOP (1) Id FROM Toplanti_Table WITH (UPDLOCK,HOLDLOCK) WHERE ToplantiYeri=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction) != null;
        }

        public bool DuplicateParameter(int id, string group, string value, SqlTransactionContext transaction)
        {
            Require(transaction);
            var query = new SqlQuery("SELECT Id FROM ToplantiParametre_Table WITH (UPDLOCK,HOLDLOCK) WHERE Id<>@Id AND Grup=@Group AND Deger=@Value");
            query.AddParameter("@Id", id); query.AddParameter("@Group", group); query.AddParameter("@Value", value);
            return sql.Select(query, transaction) != null;
        }

        public int Save<T>(string table, T item, int id, SqlTransactionContext transaction)
        {
            Require(transaction); Fields(table);
            DataTable before = id > 0 ? Read(table, id, transaction) : null;
            if (id > 0 && before == null) throw new InvalidOperationException("Güncellenecek kayıt bulunamadı.");
            if (id > 0)
            {
                if (!sql.Write(builder.BuildUpdate(item, table, new[] { "BaslangicTarihi", "BitisTarihi" }), "UPDATE", transaction))
                    throw new InvalidOperationException("Toplantı kaydı güncellenemedi.");
            }
            else
            {
                id = sql.Insert(builder.BuildInsert(item, table, new[] { "BaslangicTarihi", "BitisTarihi" }), transaction);
                if (id <= 0) throw new InvalidOperationException("Toplantı kaydı oluşturulamadı.");
            }
            DataTable after = Read(table, id, transaction);
            if (after == null) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            Audit(table, id, before, after, transaction);
            return id;
        }

        public void Delete(string table, int id, SqlTransactionContext transaction)
        {
            DataTable before = Read(table, id, transaction);
            if (before == null) throw new InvalidOperationException("Silinecek kayıt bulunamadı.");
            if (!sql.Write(builder.BuildDelete(table, id), "DELETE", transaction))
                throw new InvalidOperationException("Toplantı kaydı silinemedi.");
            Audit(table, id, before, null, transaction);
        }

        private void Audit(string table, int id, DataTable before, DataTable after, SqlTransactionContext transaction)
        {
            var log = new PortalLogRepository();
            int audit = log.RecordAudit(context, before == null ? "INSERT" : after == null ? "DELETE" : "UPDATE",
                table.Replace("_Table", ""), id.ToString(CultureInfo.InvariantCulture), transaction);
            if (audit <= 0) return;
            foreach (string field in Fields(table))
            {
                string oldValue = Value(before, field), newValue = Value(after, field);
                if (oldValue != newValue) log.RecordAuditChange(audit, field, oldValue, newValue, transaction);
            }
        }
        private static string[] Fields(string table)
        {
            // Do not log subjects, free text, names, addresses or participant identities.
            switch (table)
            {
                case "Toplanti_Table": return new[] { "ToplantiYeri", "BaslangicTarihi", "BitisTarihi", "CevrimIci", "IkramOnayi" };
                case "ToplantiKatilim_Table": return new[] { "ToplantiId", "Bilgi" };
                case "ToplantiParametre_Table": return new[] { "Grup", "Sira" };
                default: throw new ArgumentException("Geçersiz toplantı kayıt türü.", "table");
            }
        }
        private static string Value(DataTable table, string field)
        {
            if (table == null || table.Rows[0].IsNull(field)) return null;
            object value = table.Rows[0][field];
            return value is DateTime ? ((DateTime)value).ToString("o", CultureInfo.InvariantCulture) : Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static void Require(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }
    }
}

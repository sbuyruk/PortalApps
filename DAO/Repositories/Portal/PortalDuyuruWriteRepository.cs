using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.Logging;

namespace DAO.Repositories.Portal
{
    public sealed class PortalDuyuruWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly PortalSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();

        public PortalDuyuruWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new PortalSqlExecutor(context, "PortalDuyuruWriteRepository");
        }

        public void LockAnnouncement(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"DECLARE @result int;
                EXEC @result=sp_getapplock @Resource=@Resource,
                    @LockMode=N'Exclusive', @LockOwner=N'Transaction', @LockTimeout=15000;
                SELECT @result Result;");
            query.AddParameter("@Resource", "PortalApps.Portal.Duyuru." + id.ToString(CultureInfo.InvariantCulture));
            DataTable rows = sql.Select(query, transaction);
            if (rows == null || Convert.ToInt32(rows.Rows[0]["Result"]) < 0)
                throw new InvalidOperationException("Duyuru işlem kilidi alınamadı.");
        }

        public DataTable Read(string table, int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Fields(table);
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable CurrentPerson(string login, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            string user = login == null ? string.Empty : login.Substring(login.LastIndexOf('\\') + 1);
            SqlQuery query = new SqlQuery("SELECT TOP (1) Id FROM Personel_Table WITH (HOLDLOCK) WHERE KullaniciAdi=@User ORDER BY Id");
            query.AddParameter("@User", user);
            return sql.Select(query, transaction);
        }

        public DataTable LatestVersion(int announcementId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT TOP (1) * FROM DuyuruGosterim_Table WITH (UPDLOCK,HOLDLOCK) WHERE DuyuruId=@Id ORDER BY Id DESC");
            query.AddParameter("@Id", announcementId);
            return sql.Select(query, transaction);
        }

        public bool AlreadyRead(int versionId, int personId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT Id FROM DuyuruOkuma_Table WITH (UPDLOCK,HOLDLOCK) WHERE DuyuruGosterimId=@Version AND PersonelId=@Person");
            query.AddParameter("@Version", versionId);
            query.AddParameter("@Person", personId);
            return sql.Select(query, transaction) != null;
        }

        public DataTable VersionOwner(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT Id,DuyuruId FROM DuyuruGosterim_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public int Save<T>(string table, T item, int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Fields(table);
            DataTable previous = id > 0 ? Read(table, id, transaction) : null;
            if (id > 0 && previous == null) throw new InvalidOperationException("Güncellenecek duyuru kaydı bulunamadı.");
            if (id > 0)
            {
                if (!sql.Write(builder.BuildUpdate(item, table, new[] { "YayinBasTar", "YayinBitTar" }), "UPDATE", transaction))
                    throw new InvalidOperationException("Duyuru kaydı güncellenemedi.");
            }
            else
            {
                id = sql.Insert(builder.BuildInsert(item, table, new[] { "YayinBasTar", "YayinBitTar" }), transaction);
                if (id <= 0) throw new InvalidOperationException("Duyuru kaydı oluşturulamadı.");
            }
            Audit(table, id, previous, Read(table, id, transaction), transaction);
            return id;
        }

        public void Delete(int id, SqlTransactionContext transaction)
        {
            DataTable previous = Read("Duyuru_Table", id, transaction);
            if (previous == null) throw new InvalidOperationException("Silinecek duyuru bulunamadı.");
            SqlQuery query = new SqlQuery("DELETE FROM Duyuru_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            if (!sql.Write(query, "DELETE", transaction)) throw new InvalidOperationException("Duyuru silinemedi.");
            Audit("Duyuru_Table", id, previous, null, transaction);
        }

        private void Audit(string table, int id, DataTable before, DataTable after, SqlTransactionContext transaction)
        {
            PortalLogRepository log = new PortalLogRepository();
            string operation = before == null ? "INSERT" : after == null ? "DELETE" : "UPDATE";
            if (operation != "DELETE" && after == null) throw new InvalidOperationException("Kaydedilen duyuru kaydı okunamadı.");
            int auditId = log.RecordAudit(context, operation, table.Replace("_Table", string.Empty), id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in Fields(table))
            {
                string oldValue = Value(before, field), newValue = Value(after, field);
                if (oldValue != newValue) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string[] Fields(string table)
        {
            // Body, recipient lists, image names and personnel details are not audit fields.
            switch (table)
            {
                case "Duyuru_Table": return new[] { "YayinBasTar", "YayinBitTar", "Tekrar", "Popup", "Aktif" };
                case "DuyuruGosterim_Table": return new[] { "DuyuruId", "GosterildigiTarih", "YayinBasTar", "YayinBitTar", "Tekrar", "Popup", "Aktif" };
                case "DuyuruOkuma_Table": return new[] { "DuyuruId", "DuyuruGosterimId", "OkumaTarihi" };
                default: throw new ArgumentException("Geçersiz duyuru kayıt türü.", "table");
            }
        }

        private static string Value(DataTable table, string field)
        {
            if (table == null || table.Rows[0].IsNull(field)) return null;
            object value = table.Rows[0][field];
            return value is DateTime ? ((DateTime)value).ToString("o", CultureInfo.InvariantCulture) : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Duyuru transaction bağlamı gereklidir.");
        }
    }
}

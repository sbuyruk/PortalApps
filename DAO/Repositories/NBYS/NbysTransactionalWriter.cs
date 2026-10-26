using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    // Only explicitly migrated callers use this path. Audit never opens a separate business connection.
    internal sealed class NbysTransactionalWriter
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;
        private readonly string table;
        private readonly string[] fields;

        internal NbysTransactionalWriter(PortalLogContext context, string table)
        {
            this.context = context;
            this.table = table;
            sql = new NbysSqlExecutor(context, "NbysTransactionalWriter");
            switch (table)
            {
                case "EkstreAktarma_Table": fields = new[] { "NakitBagisciId", "NakitBagisHareketId", "AktarildiMi", "BagisTarihi", "Tutar", "DovizCinsi", "DovizTutari", "DovizKuru", "KurTarihi", "ElleKayit", "TuzelKisi", "BelgeIstemiyor" }; break;
                case "NakitBagisci_Table": fields = NbysAuditPolicy.DonorFields; break;
                case "NakitBagisHareket_Table": fields = new[] { "BagisciId", "ArmaganId", "EkstreAktarmaId", "BankaId", "BagisTarihi", "BagisMiktari", "DovizCinsi", "DovizTutari", "DovizKuru", "KurTarihi" }; break;
                case "Armagan_Table": fields = NbysAuditPolicy.GiftFields; break;
                case "Bagis_Table": fields = new[] { "ArmaganDurumu", "ArmaganTarihi" }; break;
                case "SMSAylikBagis_Table": fields = new[] { "Yil", "Ay", "TurkcellSMSAdedi", "VodafoneSMSAdedi", "TurkTelekomSMSAdedi", "SMSTutari", "DovizCinsi", "IslemTarihi" }; break;
                case "BagisaVesileOlanTesekkur_Table": fields = new[] { "BelgeTarihi" }; break;
                case "DuzenliNakitBagisci_Table": fields = new[] { "BagisciId", "BagisAdedi", "BagisToplami", "BaslamaTarihi", "BitisTarihi", "Tutar", "Aktif", "ArmaganId", "NakitBagisHareketId" }; break;
                case "FTKKisi_Table": fields = new[] { "Ili", "Ilcesi", "Vali", "Kaymakam", "FTKGorevi", "UyelikDurumu" }; break;
                case "FTKIslem_Table": fields = new[] { "Ili", "Ilcesi", "BolgeId", "KurulusTarihi", "GuncellemeTarihi" }; break;
                case "FTK_Table": fields = new[] { "FTKIslemId", "Ili", "Ilcesi", "BolgeId", "KurulusTarihi", "GuncellemeTarihi", "Sayac", "KisiId", "FTKGorevi" }; break;
                default: throw new ArgumentException("Geçersiz kayıt türü.", "table");
            }
        }

        internal int Insert(SqlQuery query, SqlTransactionContext transaction)
        {
            int id = sql.Insert(query, transaction);
            if (id <= 0) throw new InvalidOperationException("Kayıt oluşturulamadı.");
            RecordInserted(id, transaction);
            return id;
        }

        internal void RecordInserted(int id, SqlTransactionContext transaction)
        {
            if (id <= 0) throw new InvalidOperationException("Kayıt oluşturulamadı.");
            Audit("INSERT", id, null, Snapshot(id, transaction), transaction);
        }

        internal bool Update(SqlQuery query, int id, SqlTransactionContext transaction)
        {
            DataTable previous = Snapshot(id, transaction);
            if (previous == null) throw new InvalidOperationException("Güncellenecek kayıt bulunamadı.");
            if (!sql.Write(query, "UPDATE", transaction)) throw new InvalidOperationException("Kayıt güncellenemedi.");
            Audit("UPDATE", id, previous, Snapshot(id, transaction), transaction);
            return true;
        }

        internal bool Delete(int id, SqlTransactionContext transaction)
        {
            DataTable previous = Snapshot(id, transaction);
            if (previous == null) return false;
            SqlQuery query = new SqlQuery("DELETE FROM " + table + " WHERE Id=@Id");
            query.AddParameter("@Id", id);
            if (!sql.Write(query, "DELETE", transaction)) throw new InvalidOperationException("Kayıt silinemedi.");
            Audit("DELETE", id, previous, null, transaction);
            return true;
        }

        internal static int Id<T>(T item)
        {
            return Convert.ToInt32(item.GetType().GetProperty("Id").GetValue(item, null));
        }

        private DataTable Snapshot(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        private void Audit(string operation, int id, DataTable previous, DataTable saved, SqlTransactionContext transaction)
        {
            if (operation != "DELETE" && saved == null) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, table.Substring(0, table.Length - 6), id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            NbysAuditPolicy.RecordChanges(log, auditId, table.Substring(0, table.Length - 6), previous, saved, fields, transaction);
        }
    }
}

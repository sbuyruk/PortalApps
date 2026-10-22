using DAO.Ortak;
using DAO.Repositories.Ortak;
using System;
using System.Data;
using System.Globalization;
using Utility.Logging;

namespace DAO.Repositories.NBYS
{
    public sealed class NbysBagisciWriteRepository
    {
        private readonly PortalLogContext context;
        private readonly NbysSqlExecutor sql;
        private readonly CrudQueryBuilder builder = new CrudQueryBuilder();

        public NbysBagisciWriteRepository(PortalLogContext context)
        {
            this.context = context;
            sql = new NbysSqlExecutor(context, "NbysBagisciWriteRepository");
        }

        public DataTable SelectDonor(int id, SqlTransactionContext transaction)
        {
            return ReadStoredRecord("NakitBagisci_Table", id, transaction);
        }

        public DataTable SelectUserRegion(string login, SqlTransactionContext transaction = null)
        {
            string userName = login == null ? string.Empty : login.Substring(login.LastIndexOf('\\') + 1);
            // Preserve Personel ORDER BY Id and IsBilgileri ORDER BY UnvanId first-row selection.
            SqlQuery query = new SqlQuery(@"SELECT * FROM Bolge_Table WITH (HOLDLOCK) WHERE Id=(
                SELECT BolgeId FROM BirimTanim_Table WITH (HOLDLOCK) WHERE Id=(
                    SELECT TOP (1) BirimId FROM IsBilgileri_Table WITH (HOLDLOCK) WHERE PersonelId=(
                        SELECT TOP (1) Id FROM Personel_Table WITH (HOLDLOCK) WHERE KullaniciAdi=@KullaniciAdi ORDER BY Id)
                    ORDER BY UnvanId))");
            query.AddParameter("@KullaniciAdi", userName);
            if (transaction == null) return sql.Select(query);
            return sql.Select(query, transaction);
        }

        public DataTable SelectDonorRegion(int donorId)
        {
            SqlQuery query = new SqlQuery(@"SELECT A.* FROM Bolge_Table A
                INNER JOIN Il_Table C ON C.BolgeId=A.Id
                INNER JOIN NakitBagisci_Table B ON B.Ili=C.Id WHERE B.Id=@BagisciId");
            query.AddParameter("@BagisciId", donorId);
            return sql.Select(query);
        }

        public DataTable SelectProvinceRegion(int provinceId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT BolgeId FROM Il_Table WITH (HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", provinceId);
            return sql.Select(query, transaction);
        }

        public DataTable SelectGifts(int donorId, string status, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@BagisciId AND Durum=@Durum");
            query.AddParameter("@BagisciId", donorId);
            query.AddParameter("@Durum", status);
            return sql.Select(query, transaction);
        }

        public DataTable SelectPeriodGifts(int donorId, DateTime start, DateTime end,
            int? definitionId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId AND Tarih BETWEEN @Baslangic AND @Bitis"
                + (definitionId.HasValue ? " AND ArmaganTanimId=@ArmaganTanimId" : ""));
            query.AddParameter("@BagisciId", donorId);
            query.AddParameter("@Baslangic", start);
            query.AddParameter("@Bitis", end);
            if (definitionId.HasValue) query.AddParameter("@ArmaganTanimId", definitionId.Value);
            return sql.Select(query, transaction);
        }

        public DataTable SelectPeriodDonations(int donorId, DateTime start, DateTime end,
            SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM NakitBagisHareket_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@BagisciId AND BagisTarihi BETWEEN @Baslangic AND @Bitis");
            query.AddParameter("@BagisciId", donorId);
            query.AddParameter("@Baslangic", start);
            query.AddParameter("@Bitis", end);
            return sql.Select(query, transaction);
        }

        public DataTable SelectGiftDefinition(decimal amount, bool corporate, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            string limits = corporate
                ? "TuzelKisiAltLimit<=@Amount AND TuzelKisiUstLimit>=@Amount"
                : "OzelKisiAltLimit<=@Amount AND OzelKisiUstLimit>=@Amount";
            SqlQuery query = new SqlQuery("SELECT * FROM ArmaganTanim_Table WITH (HOLDLOCK) WHERE Aktif=1 AND " + limits);
            query.AddParameter("@Amount", amount);
            return sql.Select(query, transaction);
        }

        public int SaveDonor<T>(T item, int id, SqlTransactionContext transaction)
        {
            return Save(item, id, "NakitBagisci_Table", "NakitBagisci",
                new[] { "Ili", "Ilcesi", "TuzelKisi", "Sag", "Ulasilamiyor", "BelgeIstemiyor", "DergiGonderilmesin" }, transaction, "SaveDonor");
        }

        public int SaveGift<T>(T item, int id, SqlTransactionContext transaction)
        {
            return Save(item, id, "Armagan_Table", "Armagan",
                new[] { "BagisciId", "ArmaganTanimId", "Tarih", "Durum", "BagisMiktari", "DovizCinsi", "CokluBagis" }, transaction, "SaveGift");
        }

        public void LinkDonation(int donationId, int giftId, string user, DateTime modifiedAt,
            SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            DataTable previous = ReadStoredRecord("NakitBagisHareket_Table", donationId, transaction);
            if (previous == null) throw new InvalidOperationException("Bağış kaydı artık bulunamıyor.");
            if (!previous.Rows[0].IsNull("ArmaganId") && Convert.ToInt32(previous.Rows[0]["ArmaganId"]) == giftId) return;
            SqlQuery query = new SqlQuery(@"UPDATE NakitBagisHareket_Table SET ArmaganId=@ArmaganId,
                Degistiren=@Degistiren, DegistirmeTarihi=@DegistirmeTarihi WHERE Id=@Id");
            query.AddParameter("@Id", donationId);
            query.AddParameter("@ArmaganId", giftId);
            query.AddParameter("@Degistiren", user);
            query.AddParameter("@DegistirmeTarihi", modifiedAt);
            if (!sql.Write(query, "UPDATE", transaction)) throw new InvalidOperationException("Bağışın armağan bağlantısı güncellenemedi.");
            DataTable saved = ReadStoredRecord("NakitBagisHareket_Table", donationId, transaction);
            RecordAudit("UPDATE", "NakitBagisHareket", donationId, previous, saved, new[] { "ArmaganId" }, transaction);
        }

        private int Save<T>(T item, int id, string table, string entity, string[] fields,
            SqlTransactionContext transaction, string method)
        {
            RequireTransaction(transaction);
            DataTable previous = id > 0 ? ReadStoredRecord(table, id, transaction, method) : null;
            if (id > 0 && previous == null) throw new InvalidOperationException("Güncellenecek kayıt artık bulunamıyor.");
            string operation = id > 0 ? "UPDATE" : "INSERT";
            if (id > 0)
            {
                if (!sql.Write(builder.BuildUpdate(item, table), operation, transaction, method))
                    throw new InvalidOperationException("Kayıt güncellenemedi.");
            }
            else id = sql.Insert(builder.BuildInsert(item, table), transaction, method);
            if (id <= 0) throw new InvalidOperationException("Kayıt oluşturulamadı.");
            DataTable saved = ReadStoredRecord(table, id, transaction, method);
            RecordAudit(operation, entity, id, previous, saved, fields, transaction);
            return id;
        }

        private DataTable ReadStoredRecord(string table, int id, SqlTransactionContext transaction,
            [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            RequireTransaction(transaction);
            if (table != "NakitBagisci_Table" && table != "Armagan_Table" && table != "NakitBagisHareket_Table")
                throw new ArgumentException("Geçersiz kayıt türü.", "table");
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction, method);
        }

        private void RecordAudit(string operation, string entity, int id, DataTable previous, DataTable saved,
            string[] fields, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (saved == null || saved.Rows.Count == 0) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entity, id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in fields)
            {
                string oldValue = Value(previous, field);
                string newValue = Value(saved, field);
                if (oldValue != newValue) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string Value(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0].IsNull(field)) return null;
            object value = table.Rows[0][field];
            return value is DateTime ? ((DateTime)value).ToString("o", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }
    }
}

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

        public DataTable SelectPeriodGifts(int donorId, DateTime start, DateTime endExclusive,
            int? definitionId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BelgeGecersizMi!=1 AND BagisciId=@BagisciId AND Tarih>=@Baslangic AND Tarih<@BitisHaric"
                + (definitionId.HasValue ? " AND ArmaganTanimId=@ArmaganTanimId" : ""));
            query.AddParameter("@BagisciId", donorId);
            query.AddParameter("@Baslangic", start);
            query.AddParameter("@BitisHaric", endExclusive);
            if (definitionId.HasValue) query.AddParameter("@ArmaganTanimId", definitionId.Value);
            return sql.Select(query, transaction);
        }

        public DataTable SelectPeriodDonations(int donorId, DateTime start, DateTime endExclusive,
            SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT * FROM NakitBagisHareket_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE BagisciId=@BagisciId AND BagisTarihi>=@Baslangic AND BagisTarihi<@BitisHaric");
            query.AddParameter("@BagisciId", donorId);
            query.AddParameter("@Baslangic", start);
            query.AddParameter("@BitisHaric", endExclusive);
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
                NbysAuditPolicy.DonorFields, transaction, "SaveDonor");
        }

        public int SaveGift<T>(T item, int id, SqlTransactionContext transaction)
        {
            return Save(item, id, "Armagan_Table", "Armagan",
                NbysAuditPolicy.GiftFields, transaction, "SaveGift");
        }

        public DataTable SelectDonationOwner(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT BagisciId FROM NakitBagisHareket_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable SelectDonation(int id, SqlTransactionContext transaction)
        {
            return ReadStoredRecord("NakitBagisHareket_Table", id, transaction);
        }

        public DataTable SelectGift(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery("SELECT * FROM Armagan_Table WITH (UPDLOCK, HOLDLOCK) WHERE BelgeGecersizMi!=1 AND Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction);
        }

        public DataTable SelectDonorRows(string table, int donorId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            string field = DonorField(table);
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE " + field + "=@Id ORDER BY Id");
            query.AddParameter("@Id", donorId);
            return sql.Select(query, transaction);
        }

        public bool HasGiftReferences(int giftId, int excludedDonationId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            SqlQuery query = new SqlQuery(@"SELECT Id FROM NakitBagisHareket_Table WITH (UPDLOCK, HOLDLOCK)
                WHERE ArmaganId=@ArmaganId AND Id<>@ExcludedId
                UNION ALL SELECT Id FROM DuzenliNakitBagisci_Table WITH (UPDLOCK, HOLDLOCK) WHERE ArmaganId=@ArmaganId");
            query.AddParameter("@ArmaganId", giftId);
            query.AddParameter("@ExcludedId", excludedDonationId);
            DataTable rows = sql.Select(query, transaction);
            return rows != null && rows.Rows.Count > 0;
        }

        public int SaveDonation<T>(T item, int id, SqlTransactionContext transaction)
        {
            return Save(item, id, "NakitBagisHareket_Table", "NakitBagisHareket",
                new[] { "BagisciId", "ArmaganId", "BagisTarihi", "BagisMiktari", "IadeEdildiMi", "IadeMiktari", "IadeTarihi" },
                transaction, "SaveDonation");
        }

        public int SaveRefundGift<T>(T item, int id, SqlTransactionContext transaction)
        {
            return Save(item, id, "Armagan_Table", "Armagan",
                NbysAuditPolicy.GiftFields,
                transaction, "SaveRefundGift");
        }

        public int SaveArchive<T>(T item, SqlTransactionContext transaction)
        {
            return Save(item, 0, "SilinenKayit_Table", "SilinenKayit", new[] { "TabloAdi" }, transaction, "SaveArchive");
        }

        public void MoveDonorReference(string table, int rowId, int targetId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            string field = DonorField(table);
            DataTable previous = ReadStoredRecord(table, rowId, transaction);
            if (previous == null) throw new InvalidOperationException("Birleştirilecek bağlantı kaydı bulunamadı.");
            SqlQuery query = new SqlQuery("UPDATE " + table + " SET " + field + "=@DonorId, Degistiren=@User, DegistirmeTarihi=@ModifiedAt WHERE Id=@Id");
            query.AddParameter("@DonorId", targetId);
            query.AddParameter("@Id", rowId);
            query.AddParameter("@User", context.UserLogin);
            query.AddParameter("@ModifiedAt", DateTime.Now);
            if (!sql.Write(query, "UPDATE", transaction)) throw new InvalidOperationException("Bağışçı bağlantısı güncellenemedi.");
            DataTable saved = ReadStoredRecord(table, rowId, transaction);
            RecordAudit("UPDATE", table.Substring(0, table.Length - "_Table".Length), rowId, previous, saved, new[] { field }, transaction);
        }

        public DataTable SelectDonationReferences(string table, int donationId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (table != "EkstreAktarma_Table" && table != "DuzenliNakitBagisci_Table")
                throw new ArgumentException("Geçersiz bağış bağlantısı.", "table");
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE NakitBagisHareketId=@Id ORDER BY Id");
            query.AddParameter("@Id", donationId);
            return sql.Select(query, transaction);
        }

        public void ClearDonationReference(string table, int rowId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (table != "EkstreAktarma_Table" && table != "DuzenliNakitBagisci_Table")
                throw new ArgumentException("Geçersiz bağış bağlantısı.", "table");
            DataTable previous = ReadStoredRecord(table, rowId, transaction);
            if (previous == null) throw new InvalidOperationException("Bağış bağlantı kaydı bulunamadı.");
            SqlQuery query = new SqlQuery("UPDATE " + table + " SET NakitBagisHareketId=0, Degistiren=@User, DegistirmeTarihi=@ModifiedAt WHERE Id=@Id");
            query.AddParameter("@Id", rowId);
            query.AddParameter("@User", context.UserLogin);
            query.AddParameter("@ModifiedAt", DateTime.Now);
            if (!sql.Write(query, "UPDATE", transaction)) throw new InvalidOperationException("Bağış bağlantısı kaldırılamadı.");
            DataTable saved = ReadStoredRecord(table, rowId, transaction);
            RecordAudit("UPDATE", table.Substring(0, table.Length - "_Table".Length), rowId, previous, saved, new[] { "NakitBagisHareketId" }, transaction);
        }

        public void DeleteRow(string table, int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (table != "NakitBagisci_Table" && table != "NakitBagisHareket_Table" && table != "Armagan_Table")
                throw new ArgumentException("Geçersiz silme türü.", "table");
            DataTable previous = ReadStoredRecord(table, id, transaction);
            if (previous == null) throw new InvalidOperationException("Silinecek kayıt bulunamadı.");
            if (!sql.Write(builder.BuildDelete(table, id), "DELETE", transaction)) throw new InvalidOperationException("Kayıt silinemedi.");
            string[] fields = table == "NakitBagisci_Table" ? NbysAuditPolicy.DonorFields
                : table == "Armagan_Table" ? NbysAuditPolicy.GiftFields
                : new[] { "BagisciId", "ArmaganId", "BagisTarihi", "BagisMiktari", "IadeEdildiMi", "IadeMiktari" };
            RecordAudit("DELETE", table.Substring(0, table.Length - "_Table".Length), id, previous, null, fields, transaction);
        }

        private static string DonorField(string table)
        {
            if (table == "EkstreAktarma_Table") return "NakitBagisciId";
            if (table == "NakitBagisHareket_Table" || table == "Armagan_Table" || table == "DuzenliNakitBagisci_Table") return "BagisciId";
            throw new ArgumentException("Geçersiz bağışçı bağlantısı.", "table");
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
            if (table != "NakitBagisci_Table" && table != "Armagan_Table" && table != "NakitBagisHareket_Table"
                && table != "SilinenKayit_Table" && table != "EkstreAktarma_Table" && table != "DuzenliNakitBagisci_Table")
                throw new ArgumentException("Geçersiz kayıt türü.", "table");
            SqlQuery query = new SqlQuery("SELECT * FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return sql.Select(query, transaction, method);
        }

        private void RecordAudit(string operation, string entity, int id, DataTable previous, DataTable saved,
            string[] fields, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (operation != "DELETE" && (saved == null || saved.Rows.Count == 0)) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entity, id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            NbysAuditPolicy.RecordChanges(log, auditId, entity, previous, saved, fields, transaction);
        }

        private static void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "İşlem bağlamı gereklidir.");
        }
    }
}

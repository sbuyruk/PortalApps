using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using System.Globalization;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class TasinmazTaahhutService
    {
        private readonly TasinmazTaahhutRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazBagisciRepository donorRepository;
        private readonly TasinmazRepository propertyRepository;

        public TasinmazTaahhutService() : this(new TasinmazTaahhutRepository()) { }

        public TasinmazTaahhutService(PortalLogContext context) : this(new TasinmazTaahhutRepository(context))
        {
            this.context = context;
            donorRepository = new TasinmazBagisciRepository(context);
            propertyRepository = new TasinmazRepository(context);
        }

        public TasinmazTaahhutService(TasinmazTaahhutRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public TasinmazTaahhut GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<TasinmazTaahhut> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public List<TasinmazTaahhut> GetByBagisciId(int bagisciId)
        {
            return ToList(repository.SelectByBagisciId(bagisciId));
        }

        public TasinmazTaahhut GetByTcKimlikNo(long tcKimlikNo)
        {
            return Map(repository.SelectByTcKimlikNo(tcKimlikNo));
        }

        public List<TasinmazTaahhut> GetByFilters(bool isSagVefat, bool isTCKimlikNoFull, bool isDogumTarihiFull, int bolgeId)
        {
            return ToList(repository.SelectByFilters(
                isSagVefat,
                isTCKimlikNoFull,
                isDogumTarihiFull,
                bolgeId,
                ProjeConstants.BAGISCI_SAG,
                ProjeConstants.BOLGE_HEPSI_INT,
                ProjeConstants.BOLGE_GENELMUDURLUK_INT));
        }

        public List<TasinmazTaahhut> GetByIlAdi(string ilAdi)
        {
            return ToList(repository.SelectByIlAdi(ilAdi));
        }

        public string GetAllCountDonationAsJson()
        {
            return new TasinmazTaahhut().ToJSON(repository.SelectAllCountDonationAsJson());
        }

        public DataTable GetAllCountDonation(bool excludeDeceased, bool excludeHidden)
        {
            return repository.SelectAllCountDonation(excludeDeceased, excludeHidden, ProjeConstants.BAGISCI_SAG);
        }

        public int Save(TasinmazTaahhut item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool Update(TasinmazTaahhut item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Delete(TasinmazTaahhut item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public bool AddDonorToPledgeList(int donorId, int propertyId)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool added = AddDonorToPledgeList(donorId, propertyId, transaction);
                if (added) transaction.Complete();
                return added;
            }
        }

        public bool AddDonorToPledgeList(int donorId, int propertyId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            TasinmazBagisci donor = new TasinmazBagisci().ToList<TasinmazBagisci>(
                donorRepository.SelectById(donorId, transaction, context)).FirstOrDefault();
            if (donor == null) throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
            if (donor.TCKimlikNo > 0 && repository.SelectByTcKimlikNo(donor.TCKimlikNo, transaction, context) != null) return false;
            DataTable district = repository.SelectDonorDistrict(donor.Ili, donor.Ilcesi, transaction, context);
            TasinmazTaahhut item = new TasinmazTaahhut
            {
                BagisciId = donor.Id, TasinmazId = propertyId,
                Adi = donor.Adi, Soyadi = donor.Soyadi, Adres = donor.Adres,
                DogumTarihi = donor.DogumTarihi, TCKimlikNo = donor.TCKimlikNo,
                Telefon = donor.Telefon1, Sag_vefat = donor.Sag_vefat, VefatTarihi = donor.VefatTarihi,
                TaahhutAciklama = "Bağışçıya taahhüt verilmiştir."
            };
            if (district != null)
            {
                item.Ilcesi = Convert.ToInt32(district.Rows[0]["Id"]);
                item.Ili = Convert.ToInt32(district.Rows[0]["IlId"]);
            }
            Save(item, transaction);
            return true;
        }

        public int Save(TasinmazTaahhut item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek kayıt gereklidir.");
            EnsureParents(item, transaction);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Taahhüt kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(TasinmazTaahhut item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            EnsureParents(item, transaction);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            TasinmazTaahhut old = Map(oldValues);
            if (old == null) return false;
            if (old.BagisciId != item.BagisciId)
                throw new InvalidOperationException("Kaydın bağlı bağışçısı değişmiş. Sayfayı yenileyip tekrar deneyiniz.");
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen kayıt okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(TasinmazTaahhut item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            TasinmazTaahhut old = Map(oldValues);
            if (old == null || old.BagisciId != item.BagisciId || old.TasinmazId != item.TasinmazId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private void EnsureParents(TasinmazTaahhut item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item.BagisciId < 0)
                throw new InvalidOperationException("Bağışçı seçimi geçersiz.");
            if (item.BagisciId > 0 && donorRepository.SelectById(item.BagisciId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
            if (item.TasinmazId < 0) throw new InvalidOperationException("Taşınmaz seçimi geçersiz.");
            if (item.TasinmazId > 0 && propertyRepository.SelectById(item.TasinmazId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, int id, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "TasinmazTaahhut", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            foreach (string field in new[] { "BagisciId", "TasinmazId", "Ili", "Ilcesi", "EvrakTarihi", "Sag_vefat" })
            {
                string oldValue = AuditValue(oldValues, field);
                string newValue = AuditValue(current, field);
                if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                    log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0][field] == DBNull.Value) return null;
            object value = table.Rows[0][field];
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static List<TasinmazTaahhut> ToList(DataTable table)
        {
            return new TasinmazTaahhut().ToList<TasinmazTaahhut>(table);
        }

        private static TasinmazTaahhut Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

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
    public class BagisService
    {
        private readonly BagisRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazRepository propertyRepository;
        private readonly TasinmazBagisciRepository donorRepository;

        public BagisService() : this(new BagisRepository()) { }
        public BagisService(PortalLogContext context) : this(new BagisRepository(context))
        {
            this.context = context;
            propertyRepository = new TasinmazRepository(context);
            donorRepository = new TasinmazBagisciRepository(context);
        }
        public BagisService(BagisRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }

        public Bagis GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Bagis> GetAll() { return ToList(repository.SelectAll()); }
        public List<Bagis> GetByBagisciId(int bagisciId) { return ToList(repository.SelectByBagisciId(bagisciId)); }
        public Bagis GetByTasinmazId(int tasinmazId) { return Map(repository.SelectByTasinmazId(tasinmazId)); }
        public DataTable GetByBagisciIdGroupByKullanimSekli(int bagisciId) { return repository.SelectByBagisciIdGroupByKullanimSekli(bagisciId); }
        public string GetByBagisciIdAsJson(int bagisciId) { return new Bagis().ToJSON(repository.SelectByBagisciIdAsJson(bagisciId)); }
        public DataTable GetTasinmazByBagisciId(int bagisciId) { return repository.SelectTasinmazByBagisciId(bagisciId, null); }
        public string GetTasinmazByBagisciIdAsJson(int bagisciId) { return new Bagis().ToJSON(GetTasinmazByBagisciId(bagisciId)); }
        public DataTable GetSatisVsDahilTasinmazByBagisciId(int bagisciId)
        {
            return repository.SelectTasinmazByBagisciId(bagisciId, Tasinmaz.SatisVsDahilEnvanterdenCikmaSebepleri);
        }
        public decimal GetSumTahminiRayicByBagisciId(int bagisciId)
        {
            DataTable table = repository.SelectSumTahminiRayicByBagisciId(bagisciId);
            return table != null && table.Rows.Count > 0 ? table.Rows[0]["Toplam"].ToString().ConvertToDecimal() : 0;
        }
        public int Save(Bagis item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }
        public bool Update(Bagis item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }
        public bool Delete(Bagis item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }
        public Bagis AssignToProperty(int propertyId, int donorId)
        {
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                Bagis item = AssignToProperty(propertyId, donorId, transaction);
                transaction.Complete();
                return item;
            }
        }

        public Bagis AssignToProperty(int propertyId, int donorId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            Tasinmaz property = new Tasinmaz().ToList<Tasinmaz>(propertyRepository.SelectById(propertyId, transaction, context)).FirstOrDefault();
            if (property == null) throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            if (donorRepository.SelectById(donorId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
            Bagis item = Map(repository.SelectByTasinmazId(propertyId, transaction, context));
            if (item == null)
            {
                item = new Bagis { TasinmazId = propertyId, BagisciId = donorId,
                    BagisTarihi = property.EnvantereGirisTarihi, BagisYili = property.EnvantereGirisTarihi.Year, Envanterde = true };
                Save(item, transaction);
            }
            else
            {
                item.BagisciId = donorId;
                if (!Update(item, transaction)) throw new InvalidOperationException("Bağış ilişkisi güncellenemedi.");
            }
            return item;
        }

        public int Save(Bagis item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek bağış gereklidir.");
            EnsureParents(item, transaction);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Bağış kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen bağış okunamadı.");
            RecordDonationAudit("INSERT", item.Id, null, saved, transaction);
            SynchronizeProperty(item.TasinmazId, transaction);
            return item.Id;
        }

        public bool Update(Bagis item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            EnsureParents(item, transaction);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Bagis old = Map(oldValues);
            if (old == null) return false;
            if (old.TasinmazId != item.TasinmazId)
                throw new InvalidOperationException("Bağışın bağlı taşınmazı değişmiş. Sayfayı yenileyip tekrar deneyiniz.");
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen bağış okunamadı.");
            RecordDonationAudit("UPDATE", item.Id, oldValues, saved, transaction);
            SynchronizeProperty(item.TasinmazId, transaction);
            return true;
        }

        public bool Delete(Bagis item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            // Lock the property before its donation set, consistently with assignment.
            DataTable property = propertyRepository.SelectById(item.TasinmazId, transaction, context);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            Bagis old = Map(oldValues);
            if (old == null || old.TasinmazId != item.TasinmazId || old.BagisciId != item.BagisciId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordDonationAudit("DELETE", item.Id, oldValues, null, transaction);
            if (property != null) SynchronizeProperty(item.TasinmazId, transaction);
            return true;
        }

        private void EnsureParents(Bagis item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item.TasinmazId <= 0 || propertyRepository.SelectById(item.TasinmazId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            if (item.BagisciId <= 0 || donorRepository.SelectById(item.BagisciId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
        }

        private void SynchronizeProperty(int propertyId, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            DataTable oldValues = propertyRepository.SelectById(propertyId, transaction, context);
            if (oldValues == null) throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
            Bagis donation = Map(repository.SelectByTasinmazId(propertyId, transaction, context));
            int donorId = 0;
            string name = string.Empty;
            if (donation != null)
            {
                TasinmazBagisci donor = new TasinmazBagisci().ToList<TasinmazBagisci>(
                    donorRepository.SelectById(donation.BagisciId, transaction, context)).FirstOrDefault();
                if (donor == null) throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
                donorId = donor.Id;
                name = donor.Adi + ' ' + donor.Soyadi;
            }
            object oldId = oldValues.Rows[0]["BagisciId"];
            if (oldId != DBNull.Value && Convert.ToInt32(oldId) == donorId &&
                string.Equals(Convert.ToString(oldValues.Rows[0]["Bagisci"]), name, StringComparison.Ordinal)) return;
            propertyRepository.UpdateDonor(propertyId, donorId, name, DateTime.Now, UtilityHelper.GetCurrentUserName(), transaction, context);
            DataTable saved = propertyRepository.SelectById(propertyId, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen taşınmaz okunamadı.");
            RecordEntityAudit("UPDATE", "Tasinmaz", propertyId, oldValues, saved, new[] { "BagisciId" }, transaction);
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordDonationAudit(string operation, int id, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RecordEntityAudit(operation, "Bagis", id, oldValues, current,
                new[] { "BagisciId", "TasinmazId", "BagisTarihi", "BagisYili", "Envanterde", "ArmaganDurumu", "ArmaganTarihi" }, transaction);
        }

        private void RecordEntityAudit(string operation, string entityName, int id, DataTable oldValues,
            DataTable current, string[] fields, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, entityName, id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            foreach (string field in fields)
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
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static List<Bagis> ToList(DataTable table) { return new Bagis().ToList<Bagis>(table); }
        private static Bagis Map(DataTable table) { return ToList(table).FirstOrDefault(); }
    }
}

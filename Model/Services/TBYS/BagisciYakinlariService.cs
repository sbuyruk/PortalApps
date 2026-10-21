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
using Utility.ProjeGlobal;

namespace Model.Services.TBYS
{
    public class BagisciYakinlariService
    {
        private readonly BagisciYakinlariRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazBagisciRepository donorRepository;
        public BagisciYakinlariService() : this(new BagisciYakinlariRepository()) { }
        public BagisciYakinlariService(PortalLogContext context) : this(new BagisciYakinlariRepository(context))
        {
            this.context = context;
            donorRepository = new TasinmazBagisciRepository(context);
        }

        public BagisciYakinlariService(BagisciYakinlariRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagisciYakinlari GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BagisciYakinlari> GetAll() { return ToList(repository.SelectAll()); }
        public int Save(BagisciYakinlari item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    int id = Save(item, transaction);
                    transaction.Complete();
                    return id;
                }
            }
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
            return item.Id;
        }

        public bool Update(BagisciYakinlari item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    bool updated = Update(item, transaction);
                    if (updated) transaction.Complete();
                    return updated;
                }
            }
            if (item == null) throw new ArgumentNullException("item");
            BagisciYakinlari old = GetById(item.Id);
            bool ok = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                ok = repository.Update(item);
            }
            if (ok && ProjeConstants.TBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
            return ok;
        }

        public bool Delete(BagisciYakinlari item)
        {
            if (context != null)
            {
                using (SqlTransactionContext transaction = new SqlTransactionContext())
                {
                    bool deleted = Delete(item, transaction);
                    if (deleted) transaction.Complete();
                    return deleted;
                }
            }
            if (item == null || item.Id == 0) return false;
            BagisciYakinlari old = GetById(item.Id);
            if (old == null) return false;
            bool ok = repository.Delete(item.Id);
            if (ok && ProjeConstants.TBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI);
            return ok;
        }

        public List<BagisciYakinlari> GetByBagisciId(int bagisciId) { return ToList(repository.SelectByBagisciId(bagisciId)); }
        public int Save(BagisciYakinlari item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek kayıt gereklidir.");
            EnsureParents(item, transaction);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Yakın kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen kayıt okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(BagisciYakinlari item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            EnsureParents(item, transaction);
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            BagisciYakinlari old = Map(oldValues);
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

        public bool Delete(BagisciYakinlari item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            BagisciYakinlari old = Map(oldValues);
            if (old == null || old.BagisciId != item.BagisciId) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private void EnsureParents(BagisciYakinlari item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item.BagisciId <= 0)
                throw new InvalidOperationException("Bağışçı seçimi geçersiz.");
            if (item.BagisciId > 0 && donorRepository.SelectById(item.BagisciId, transaction, context) == null)
                throw new InvalidOperationException("İlişkili bağışçı kaydı bulunamadı.");
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
            int auditId = log.RecordAudit(context, operation, "BagisciYakinlari", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId <= 0) return;
            foreach (string field in new[] { "BagisciId" })
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

        private static BagisciYakinlari Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<BagisciYakinlari> ToList(DataTable table) { return new BagisciYakinlari().ToList<BagisciYakinlari>(table); }
    }
}

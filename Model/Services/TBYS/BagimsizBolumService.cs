using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Globalization;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.TBYS
{
    public class BagimsizBolumService
    {
        private readonly BagimsizBolumRepository repository;
        private readonly PortalLogContext context;
        private readonly TasinmazRepository propertyRepository;
        private readonly SozlesmeTasinmazRepository contractRepository;
        public BagimsizBolumService() : this(new BagimsizBolumRepository()) { }
        public BagimsizBolumService(PortalLogContext context) : this(new BagimsizBolumRepository(context))
        {
            this.context = context;
            propertyRepository = new TasinmazRepository(context);
            contractRepository = new SozlesmeTasinmazRepository(context);
        }
        public BagimsizBolumService(BagimsizBolumRepository repository) { if (repository == null) throw new ArgumentNullException("repository"); this.repository = repository; }
        public BagimsizBolum GetById(int id) { return Map(repository.SelectById(id)); }
        public List<BagimsizBolum> GetAll() { return ToList(repository.SelectAll()); }
        public List<BagimsizBolum> GetByTasinmazId(int id) { return ToList(repository.SelectByTasinmazId(id)); }
        public List<BagimsizBolum> GetByBolumNo(string bolumNo) { return ToList(repository.SelectByBolumNo(bolumNo)); }
        public BagimsizBolum GetByBolumId(int id) { return Map(repository.SelectByBolumId(id)); }
        public int Save(BagimsizBolum item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool Update(BagimsizBolum item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Delete(BagimsizBolum item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public int Save(BagimsizBolum item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek bağımsız bölüm gereklidir.");
            EnsureParent(item.TasinmazId, transaction);
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Bağımsız bölüm kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen bağımsız bölüm okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(BagimsizBolum item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            BagimsizBolum old = Map(oldValues);
            if (old == null) return false;
            EnsureParent(item.TasinmazId, transaction);
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen bağımsız bölüm okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(BagimsizBolum item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null) return false;
            // Recheck at deletion time, not only while deciding whether to show the button.
            if (contractRepository.SelectByBolumId(item.Id, transaction, context) != null) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        private void EnsureParent(int id, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (id <= 0 || propertyRepository.SelectById(id, transaction, context) == null)
                throw new InvalidOperationException("İlişkili taşınmaz kaydı bulunamadı.");
        }

        private void RequireTransaction(SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
        }

        private void RecordAudit(string operation, int entityId, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "BagimsizBolum", entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            if (auditId <= 0) return;
            foreach (string field in new[] { "TasinmazId", "BolumNo", "BBBrutAlan", "BBNetAlan", "KullanimAmaci",
                "MuhasebeyeKayitliDeger", "TahminiRayicDegeri", "EmlakBeyanDegeri", "YaklasikPiyasaDegeri" })
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
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        private static List<BagimsizBolum> ToList(DataTable t) { return new BagimsizBolum().ToList<BagimsizBolum>(t); }
        private static BagimsizBolum Map(DataTable t) { return ToList(t).FirstOrDefault(); }
    }
}

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
    public class GecikmeZammiService
    {
        private readonly GecikmeZammiRepository repository;
        private readonly PortalLogContext context;

        public GecikmeZammiService() : this(new GecikmeZammiRepository()) { }
        public GecikmeZammiService(PortalLogContext context) : this(new GecikmeZammiRepository(context)) { this.context = context; }

        public GecikmeZammiService(GecikmeZammiRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public GecikmeZammi GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<GecikmeZammi> GetAll()
        {
            return new GecikmeZammi().ToList<GecikmeZammi>(repository.SelectAll());
        }

        public List<GecikmeZammi> GetChangedBetween(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            return ToList(repository.SelectChangedBetween(ilkOdemeTarihi, sonOdemeTarihi));
        }

        public List<GecikmeZammi> GetByStartDate(DateTime baslangicTarihi)
        {
            return ToList(repository.SelectByStartDate(baslangicTarihi));
        }

        public List<GecikmeZammi> GetByDateRange(DateTime vadeBaslangicTarihi, DateTime vadeBitisTarihi)
        {
            return ToList(repository.SelectByDateRange(vadeBaslangicTarihi, vadeBitisTarihi));
        }

        public GecikmeZammi GetLatestByDate(DateTime sonOdemeTarihi)
        {
            return Map(repository.SelectLatestByDate(sonOdemeTarihi));
        }

        public GecikmeZammi GetPrevious(DateTime tarih)
        {
            return Map(repository.SelectPrevious(tarih));
        }

        public GecikmeZammi GetNext(DateTime tarih)
        {
            return Map(repository.SelectNext(tarih));
        }

        public List<GecikmeZammi> GetByDate(DateTime ilkOdemeTarihi, DateTime sonOdemeTarihi)
        {
            List<GecikmeZammi> changedList = GetChangedBetween(ilkOdemeTarihi, sonOdemeTarihi);
            List<GecikmeZammi> result = new List<GecikmeZammi>();

            if (changedList.Count < 1)
            {
                GecikmeZammi previous = GetLatestByDate(sonOdemeTarihi);
                if (previous != null)
                    result.Add(previous);
            }

            result.AddRange(changedList);
            return result;
        }

        public GecikmeZammi GetLatest()
        {
            return Map(repository.SelectLatest());
        }

        public int Save(GecikmeZammi item)
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
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
            }

            return item.Id;
        }

        public bool Update(GecikmeZammi item)
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

            GecikmeZammi oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
            }

            return isSuccess;
        }

        public bool Delete(GecikmeZammi item)
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

            GecikmeZammi oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_GECIKMEZAMMI);
            }

            return isDeleted;
        }

        public int Save(GecikmeZammi item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null) throw new ArgumentNullException("item", "Kaydedilecek kayıt gereklidir.");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id <= 0) throw new InvalidOperationException("Gecikme zammı kaydı oluşturulamadı.");
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Kaydedilen gecikme zammı kaydı okunamadı.");
            RecordAudit("INSERT", item.Id, null, saved, transaction);
            return item.Id;
        }

        public bool Update(GecikmeZammi item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            GecikmeZammi old = Map(oldValues);
            if (old == null) return false;
            item.OlusturmaTarihi = old.OlusturmaTarihi;
            item.Olusturan = old.Olusturan;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            if (!repository.Update(item, transaction, context)) return false;
            DataTable saved = repository.SelectById(item.Id, transaction, context);
            if (saved == null) throw new InvalidOperationException("Güncellenen gecikme zammı kaydı okunamadı.");
            RecordAudit("UPDATE", item.Id, oldValues, saved, transaction);
            return true;
        }

        public bool Delete(GecikmeZammi item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            if (item == null || item.Id <= 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null) return false;
            if (!repository.Delete(item.Id, transaction, context)) return false;
            RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return true;
        }

        public int SavePeriod(GecikmeZammi item)
        {
            RequirePeriod(item);
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                SetPeriodBoundaries(item, transaction);
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public bool UpdatePeriod(GecikmeZammi item)
        {
            RequirePeriod(item);
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                GecikmeZammi current = Map(repository.SelectById(item.Id, transaction, context));
                if (current == null) return false;
                item.BitisTarihi = current.BitisTarihi;
                SetPeriodBoundaries(item, transaction);
                if (!Update(item, transaction)) throw new InvalidOperationException("Gecikme zammı kaydı güncellenemedi.");
                transaction.Complete();
                return true;
            }
        }

        public bool DeletePeriod(int id)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                GecikmeZammi item = Map(repository.SelectById(id, transaction, context));
                if (item == null) return false;
                GecikmeZammi previous = Map(repository.SelectPrevious(item.BaslangicTarihi, item.Id, transaction, context));
                if (previous != null && previous.BitisTarihi != item.BitisTarihi)
                {
                    previous.BitisTarihi = item.BitisTarihi;
                    if (!Update(previous, transaction)) throw new InvalidOperationException("Önceki dönemin bitiş tarihi güncellenemedi.");
                }
                if (!Delete(item, transaction)) throw new InvalidOperationException("Gecikme zammı kaydı silinemedi.");
                transaction.Complete();
                return true;
            }
        }

        private void SetPeriodBoundaries(GecikmeZammi item, SqlTransactionContext transaction)
        {
            RequireTransaction(transaction);
            GecikmeZammi previous = Map(repository.SelectPrevious(item.BaslangicTarihi, item.Id, transaction, context));
            GecikmeZammi next = Map(repository.SelectNext(item.BaslangicTarihi, item.Id, transaction, context));
            if (previous != null && previous.BitisTarihi != item.BaslangicTarihi.AddDays(-1))
            {
                previous.BitisTarihi = item.BaslangicTarihi.AddDays(-1);
                if (!Update(previous, transaction)) throw new InvalidOperationException("Önceki dönemin bitiş tarihi güncellenemedi.");
            }
            if (next != null) item.BitisTarihi = next.BaslangicTarihi.AddDays(-1);
        }

        private void RequirePeriod(GecikmeZammi item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            if (item == null) throw new ArgumentNullException("item", "Dönem kaydı gereklidir.");
            if (item.BaslangicTarihi < new DateTime(1901, 1, 1)) throw new ArgumentException("Geçerli bir başlangıç tarihi giriniz.", "item");
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
            int id = log.RecordAudit(context, operation, "GecikmeZammi", entityId.ToString(CultureInfo.InvariantCulture), transaction);
            if (id <= 0) return; // Logging may be disabled; do not use the legacy audit writer.
            foreach (string field in new[] { "BaslangicTarihi", "BitisTarihi", "ZamOrani" })
                AddChange(log, id, field, AuditValue(oldValues, field), AuditValue(current, field), transaction);
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || table.Rows[0][field] == DBNull.Value) return null;
            object value = table.Rows[0][field];
            if (value is DateTime) return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
            return value is decimal ? ((decimal)value).ToString("G29", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static void AddChange(PortalLogRepository log, int auditId, string field, string oldValue,
            string newValue, SqlTransactionContext transaction)
        {
            if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
                log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
        }

        private static List<GecikmeZammi> ToList(DataTable table)
        {
            return new GecikmeZammi().ToList<GecikmeZammi>(table);
        }

        private static GecikmeZammi Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

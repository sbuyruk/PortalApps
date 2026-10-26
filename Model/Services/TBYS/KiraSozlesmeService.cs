using DAO.Repositories.TBYS;
using DAO.Ortak;
using DAO.Repositories.Ortak;
using Utility.Logging;
using Model.Ortak;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using System.Globalization;

namespace Model.Services.TBYS
{
    public class KiraSozlesmeService
    {
        private readonly KiraSozlesmeRepository repository;
        private readonly PortalLogContext context;

        public KiraSozlesmeService() : this(new KiraSozlesmeRepository()) { }

        public KiraSozlesmeService(KiraSozlesmeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public KiraSozlesmeService(PortalLogContext context) : this(new KiraSozlesmeRepository(context))
        {
            this.context = context;
        }

        public KiraSozlesme GetById(int id, PortalLogContext context)
        {
            return Map(new KiraSozlesmeRepository(context).SelectById(id));
        }

        public KiraSozlesme GetById(int id)
        {
            return Map(repository.SelectById(id));
        }
        public KiraSozlesme GetById(int id, SqlTransactionContext transaction) { return Map(repository.SelectById(id, transaction, context)); }
        public int Save(KiraSozlesme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                int id = Save(item, transaction);
                transaction.Complete();
                return id;
            }
        }

        public int Save(KiraSozlesme item, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            if (item == null) throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = context.UserLogin;
            item.Id = repository.Insert(item, transaction, context);
            if (item.Id > 0)
                RecordAudit("INSERT", item.Id, null, repository.SelectById(item.Id, transaction, context), transaction);
            return item.Id;
        }
        public bool Update(KiraSozlesme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = Update(item, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool Update(KiraSozlesme item, SqlTransactionContext transaction)
        {
            if (item == null || item.Id == 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null || oldValues.Rows.Count == 0) return false;
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = repository.Update(item, transaction, context);
            if (updated)
                RecordAudit("UPDATE", item.Id, oldValues, repository.SelectById(item.Id, transaction, context), transaction);
            return updated;
        }
        public bool Delete(KiraSozlesme item)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool deleted = Delete(item, transaction);
                if (deleted) transaction.Complete();
                return deleted;
            }
        }

        public bool Delete(KiraSozlesme item, SqlTransactionContext transaction)
        {
            if (item == null || item.Id == 0) return false;
            DataTable oldValues = repository.SelectById(item.Id, transaction, context);
            if (oldValues == null || oldValues.Rows.Count == 0) return false;
            bool deleted = repository.Delete(item.Id, transaction, context);
            if (deleted) RecordAudit("DELETE", item.Id, oldValues, null, transaction);
            return deleted;
        }

        private void RecordAudit(string operation, int id, DataTable oldValues, DataTable current, SqlTransactionContext transaction)
        {
            if (!PortalLogSettings.IsEnabled(context)) return;
            PortalLogRepository log = new PortalLogRepository();
            int auditId = log.RecordAudit(context, operation, "KiraSozlesme", id.ToString(CultureInfo.InvariantCulture), transaction);
            if (auditId == 0) return;
            string[] fields = { "KiraciId", "SozBasTar", "SozBitTar", "KiraBedeli", "OdemeSekli", "TaksitSayisi", "TeminatTutari", "Aktif", "SozlesmeDurumu", "DurumDegismeTar", "DosyaNo", "DevirAnaPara", "DevirFaizTutari", "DevirFaizliBakiye", "OdenenTeminatTutari", "IadeTeminatTutari", "KalanTeminatTutari", "BolgeId" };
            foreach (string field in fields)
            {
                string oldValue = AuditValue(oldValues, field);
                string newValue = AuditValue(current, field);
                if (oldValue != newValue) log.RecordAuditChange(auditId, field, oldValue, newValue, transaction);
            }
        }

        private static string AuditValue(DataTable table, string field)
        {
            if (table == null || table.Rows.Count == 0 || !table.Columns.Contains(field) || table.Rows[0][field] == DBNull.Value) return null;
            return Convert.ToString(table.Rows[0][field], CultureInfo.InvariantCulture);
        }
        public bool UpdateByKiraciId(int bolgeId, int kiraciId)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                foreach (KiraSozlesme item in GetAllByKiraciId(kiraciId, transaction))
                {
                    if (item.BolgeId == bolgeId) continue;
                    item.BolgeId = bolgeId;
                    if (!Update(item, transaction)) return false;
                }
                transaction.Complete();
            }
            return true;
        }
        public bool UpdateActiveStatus(KiraSozlesme item, string durum, string degistirmeTar, bool aktif)
        {
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            using (SqlTransactionContext transaction = new SqlTransactionContext())
            {
                bool updated = UpdateActiveStatus(item, durum, degistirmeTar, aktif, transaction);
                if (updated) transaction.Complete();
                return updated;
            }
        }

        public bool UpdateActiveStatus(KiraSozlesme item, string durum, string degistirmeTar, bool aktif, SqlTransactionContext transaction)
        {
            DateTime date = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(degistirmeTar)
                && !DateTime.TryParse(degistirmeTar, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out date))
                return false;
            return UpdateActiveStatus(item, durum, date, aktif, transaction);
        }

        public bool UpdateActiveStatus(KiraSozlesme item, string durum, DateTime date, bool aktif, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction", "Veritabanı işlem bağlamı gereklidir.");
            if (context == null) throw new InvalidOperationException("Bu işlem için TBYS log bağlamı gereklidir.");
            if (item == null || item.Id <= 0 || date < new DateTime(1753, 1, 1)) return false;
            DataTable previous = repository.SelectById(item.Id, transaction, context);
            if (previous == null) return false;
            bool updated = repository.UpdateActiveStatus(item.Id, aktif, durum, date,
                context.UserLogin, DateTime.Now, transaction, context);
            if (updated)
                RecordAudit("UPDATE", item.Id, previous, repository.SelectById(item.Id, transaction, context), transaction);
            return updated;
        }

        public List<KiraSozlesme> GetAll()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAll());
        }

        public List<KiraSozlesme> GetAllActive()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAllActive());
        }
        public DataTable GetListReturnDataTable(int kiraciId, int aktif, int bolgeId)
        {
            return repository.SelectListReturnDataTable(kiraciId, aktif, bolgeId);
        }
        public List<KiraSozlesme> GetList(int kiraciId, int aktif, int bolgeId)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectList(kiraciId, aktif, bolgeId));
        }

        public List<KiraSozlesme> GetList(int kiraciId, int aktif, int bolgeId, SqlTransactionContext transaction)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectList(kiraciId, aktif, bolgeId, transaction, context));
        }
        public DataTable GetRentIncreaseDue(int bolgeId, DateTime tarih)
        {
            return repository.SelectRentIncreaseDue(bolgeId, tarih.ReturnTRDateFormat());
        }
        public DataTable GetRealizedRentIncreases(int bolgeId)
        {
            DateTime baslangic = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime bitis = new DateTime(DateTime.Today.AddYears(1).Year, 12, 31);
            return repository.SelectRealizedRentIncreases(bolgeId, baslangic.ReturnTRDateFormat(), bitis.ReturnTRDateFormat());
        }
        public DataTable GetListByYear(int yil) { return repository.SelectListByYear(yil); }
        public DataTable GetCompletedListByYear(int yil) { return repository.SelectCompletedListByYear(yil); }
        public KiraSozlesme GetActiveByKiraciId(int kiraciId) { return Map(repository.SelectActiveByKiraciId(kiraciId)); }
        public KiraSozlesme GetByKiraciId(int kiraciId) { return Map(repository.SelectByKiraciId(kiraciId)); }
        public List<KiraSozlesme> GetAllByKiraciId(int kiraciId) { return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAllByKiraciId(kiraciId)); }
        public List<KiraSozlesme> GetAllByKiraciId(int kiraciId, SqlTransactionContext transaction) { return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAllByKiraciId(kiraciId, transaction, context)); }
        public KiraSozlesme GetByKiraciIdAndDate(int kiraciId, DateTime tarih) { return Map(repository.SelectByKiraciIdAndDate(kiraciId, tarih.ReturnTRDateFormat())); }
        public KiraSozlesme GetCompletedByKiraciId(int kiraciId) { return Map(repository.SelectCompletedByKiraciId(kiraciId)); }
        public KiraSozlesme GetNearestByKiraciIdAndDate(int kiraciId, DateTime tarih) { return Map(repository.SelectNearestByKiraciIdAndDate(kiraciId, tarih.ReturnTRDateFormat())); }
        public List<KiraSozlesme> GetByTasinmazId(int tasinmazId) { return ToList(repository.SelectByTasinmazId(tasinmazId)); }
        public DataTable GetAddressById(int sozlesmeId) { return repository.SelectAddressById(sozlesmeId); }
        public DataTable GetSecurityDepositSummaryByRegionAndPurpose(int bolgeId, string kiralamaAmaci) { return repository.SelectSecurityDepositSummaryByRegionAndPurpose(bolgeId, kiralamaAmaci); }
        public DataTable GetTenantCountAndRentTotal(int bolgeId, int ay, int yil) { return repository.SelectTenantCountAndRentTotal(bolgeId, ay, yil); }
        public DataTable GetTenantCountByRegionAndDate(int bolgeId, int ay, int yil) { return repository.SelectTenantCountByRegionAndDate(bolgeId, ay, yil); }
        public List<KiraSozlesme> GetByKiraciAdi(string adi) { return ToList(repository.SelectByKiraciAdi(adi)); }
        public DataTable GetTransferRequiredActiveContracts() { return repository.SelectTransferRequiredActiveContracts(); }
        public KiraSozlesme GetNext(int id, int dosyaNo) { return Map(repository.SelectNext(id, dosyaNo)) ?? GetMin(); }
        public KiraSozlesme GetPreviousByTenant(int kiraciId, DateTime sozBasTar) { return Map(repository.SelectPreviousByTenant(kiraciId, sozBasTar.ReturnTRDateFormat())); }
        public KiraSozlesme GetPreviousByTenant(int kiraciId, DateTime sozBasTar, SqlTransactionContext transaction) { return Map(repository.SelectPreviousByTenant(kiraciId, sozBasTar, transaction, context)); }
        public KiraSozlesme GetPrevious(int id, int dosyaNo) { return Map(repository.SelectPrevious(id, dosyaNo)) ?? GetMax(); }
        public KiraSozlesme GetMax() { return Map(repository.SelectMax()); }
        public KiraSozlesme GetMin() { return Map(repository.SelectMin()); }
        public KiraSozlesme GetNextCompleted(int id, int dosyaNo) { return Map(repository.SelectNextCompleted(id, dosyaNo)) ?? GetMinCompleted(); }
        public KiraSozlesme GetPreviousCompleted(int id, int dosyaNo) { return Map(repository.SelectPreviousCompleted(id, dosyaNo)) ?? GetMaxCompleted(); }
        public KiraSozlesme GetMaxCompleted() { return Map(repository.SelectMaxCompleted()); }
        public KiraSozlesme GetMinCompleted() { return Map(repository.SelectMinCompleted()); }

        private static List<KiraSozlesme> ToList(DataTable table)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(table);
        }
        private static KiraSozlesme Map(DataTable table)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(table).FirstOrDefault();
        }
    }
}

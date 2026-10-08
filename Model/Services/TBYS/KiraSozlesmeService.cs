using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.HelperClasses;

namespace Model.Services.TBYS
{
    public class KiraSozlesmeService
    {
        private readonly KiraSozlesmeRepository repository;

        public KiraSozlesmeService() : this(new KiraSozlesmeRepository()) { }

        public KiraSozlesmeService(KiraSozlesmeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public KiraSozlesme GetById(int id)
        {
            return Map(repository.SelectById(id));
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
        public KiraSozlesme GetByKiraciIdAndDate(int kiraciId, DateTime tarih) { return Map(repository.SelectByKiraciIdAndDate(kiraciId, tarih.ReturnTRDateFormat())); }
        public KiraSozlesme GetCompletedByKiraciId(int kiraciId) { return Map(repository.SelectCompletedByKiraciId(kiraciId)); }
        public KiraSozlesme GetNearestByKiraciIdAndDate(int kiraciId, DateTime tarih) { return Map(repository.SelectNearestByKiraciIdAndDate(kiraciId, tarih.ReturnTRDateFormat())); }
        public List<KiraSozlesme> GetByTasinmazId(int tasinmazId) { return ToList(repository.SelectByTasinmazId(tasinmazId)); }
        public DataTable GetAddressById(int sozlesmeId) { return repository.SelectAddressById(sozlesmeId); }
        public DataTable GetSecurityDepositSummaryByRegionAndPurpose(int bolgeId, string kiralamaAmaci) { return repository.SelectSecurityDepositSummaryByRegionAndPurpose(bolgeId, kiralamaAmaci); }
        public DataTable GetTenantCountAndRentTotal(int bolgeId, int ay, int yil) { return repository.SelectTenantCountAndRentTotal(bolgeId, ay, yil); }
        public KiraSozlesme GetNext(int id, int dosyaNo) { return Map(repository.SelectNext(id, dosyaNo)) ?? GetMin(); }
        public KiraSozlesme GetPreviousByTenant(int kiraciId, DateTime sozBasTar) { return Map(repository.SelectPreviousByTenant(kiraciId, sozBasTar.ReturnTRDateFormat())); }
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

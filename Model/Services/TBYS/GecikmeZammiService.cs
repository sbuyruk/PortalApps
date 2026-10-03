using DAO.Repositories.TBYS;
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
    public class GecikmeZammiService
    {
        private readonly GecikmeZammiRepository repository;

        public GecikmeZammiService() : this(new GecikmeZammiRepository()) { }

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

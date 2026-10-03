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
    public class HukukiTakipService
    {
        private readonly HukukiTakipRepository repository;

        public HukukiTakipService() : this(new HukukiTakipRepository()) { }

        public HukukiTakipService(HukukiTakipRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public HukukiTakip GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<HukukiTakip> GetAll()
        {
            return new HukukiTakip().ToList<HukukiTakip>(repository.SelectAll());
        }

        public HukukiTakip GetBySozlesmeId(int sozlesmeId)
        {
            return new HukukiTakip().ToList<HukukiTakip>(repository.SelectBySozlesmeId(sozlesmeId)).FirstOrDefault();
        }

        public string GetActiveListAsJson()
        {
            return new HukukiTakip().ToJSON(repository.SelectActiveList());
        }

        public DataTable GetActiveList()
        {
            return repository.SelectActiveListForDataTable();
        }

        public int Save(HukukiTakip item)
        {
            if (item == null) throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
            }

            return item.Id;
        }

        public bool Update(HukukiTakip item)
        {
            if (item == null) throw new ArgumentNullException("item");

            HukukiTakip oldItem = GetById(item.Id);
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
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
            }

            return isSuccess;
        }

        public bool Delete(HukukiTakip item)
        {
            if (item == null || item.Id == 0) return false;

            HukukiTakip oldItem = GetById(item.Id);
            if (oldItem == null) return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_HUKUKITAKIP);
            }

            return isDeleted;
        }

        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            return repository.DeleteBySozlesmeId(sozlesmeId);
        }

        private static HukukiTakip Map(DataTable table)
        {
            return new HukukiTakip().ToList<HukukiTakip>(table).FirstOrDefault();
        }
    }
}

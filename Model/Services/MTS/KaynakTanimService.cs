using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class KaynakTanimService
    {
        private readonly KaynakTanimRepository repository;

        public KaynakTanimService() : this(new KaynakTanimRepository())
        {
        }

        public KaynakTanimService(KaynakTanimRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public KaynakTanim GetById(int id)
        {
            return MapSingle(repository.SelectById(id));
        }

        public List<KaynakTanim> GetAll()
        {
            return new KaynakTanim().ToList<KaynakTanim>(repository.SelectAll());
        }

        public int Save(KaynakTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);

            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KAYNAKTANIM);
            }

            return item.Id;
        }

        public bool Update(KaynakTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            KaynakTanim oldItem = GetById(item.Id);
            bool isSuccess = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                isSuccess = repository.Update(item);
            }

            if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.GuncellemeOlayKaydet(item, oldItem, ProjeConstants.MTS, ProjeConstants.MTS_KAYNAKTANIM);
            }

            return isSuccess;
        }

        public bool Delete(KaynakTanim item)
        {
            if (item == null || item.Id == 0)
                return false;

            KaynakTanim oldItem = GetById(item.Id);
            if (oldItem == null)
                return false;

            bool isDeleted = repository.Delete(item.Id);
            if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(oldItem, ProjeConstants.MTS, ProjeConstants.MTS_KAYNAKTANIM);
            }

            return isDeleted;
        }

        private static KaynakTanim MapSingle(DataTable dataTable)
        {
            return new KaynakTanim().ToList<KaynakTanim>(dataTable).FirstOrDefault();
        }
    }
}

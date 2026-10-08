using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class DepoStokService
    {
        private readonly DepoStokRepository repository;

        public DepoStokService() : this(new DepoStokRepository())
        {
        }

        public DepoStokService(DepoStokRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public DepoStok GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<DepoStok> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public int Save(DepoStok item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return item.Id;
        }

        public bool Update(DepoStok item)
        {
            DepoStok old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return updated;
        }

        public bool Delete(DepoStok item)
        {
            DepoStok old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return deleted;
        }

        public DepoStok GetByDepoIdAniObjesiId(int depoId, int aniObjesiId, string stokluMu)
        {
            return ToList(repository.SelectDepoStok(depoId, aniObjesiId, stokluMu)).FirstOrDefault();
        }

        private static List<DepoStok> ToList(System.Data.DataTable table)
        {
            return new DepoStok().ToList<DepoStok>(table);
        }

        private static DepoStok Map(System.Data.DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

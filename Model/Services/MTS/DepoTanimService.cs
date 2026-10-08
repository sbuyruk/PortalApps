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
    public class DepoTanimService
    {
        private readonly DepoTanimRepository repository;
        private readonly MtsLookupRepository lookupRepository;

        public DepoTanimService() : this(new DepoTanimRepository(), new MtsLookupRepository())
        {
        }

        public DepoTanimService(DepoTanimRepository repository, MtsLookupRepository lookupRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
            this.lookupRepository = lookupRepository ?? throw new ArgumentNullException("lookupRepository");
        }

        public DepoTanimService(MtsLookupRepository lookupRepository)
            : this(new DepoTanimRepository(), lookupRepository)
        {
        }

        public DepoTanim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<DepoTanim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public int Save(DepoTanim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return item.Id;
        }

        public bool Update(DepoTanim item)
        {
            DepoTanim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return updated;
        }

        public bool Delete(DepoTanim item)
        {
            DepoTanim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return deleted;
        }

        public DataTable GetStokluAniObjesiList(int aniObjesiId)
        {
            return lookupRepository.SelectStokluAniObjesiDepolari(ProjeConstants.MTS_ANIOBJESISTOKLU, aniObjesiId);
        }

        private static List<DepoTanim> ToList(DataTable table)
        {
            return new DepoTanim().ToList<DepoTanim>(table);
        }

        private static DepoTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

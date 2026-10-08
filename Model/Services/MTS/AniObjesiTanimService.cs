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
    public class AniObjesiTanimService
    {
        private readonly AniObjesiTanimRepository repository;
        private readonly MtsLookupRepository lookupRepository;

        public AniObjesiTanimService() : this(new AniObjesiTanimRepository(), new MtsLookupRepository())
        {
        }

        public AniObjesiTanimService(AniObjesiTanimRepository repository, MtsLookupRepository lookupRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
            this.lookupRepository = lookupRepository ?? throw new ArgumentNullException("lookupRepository");
        }

        public AniObjesiTanimService(MtsLookupRepository lookupRepository)
            : this(new AniObjesiTanimRepository(), lookupRepository)
        {
        }

        public AniObjesiTanim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<AniObjesiTanim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public int Save(AniObjesiTanim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return item.Id;
        }

        public bool Update(AniObjesiTanim item)
        {
            AniObjesiTanim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return updated;
        }

        public bool Delete(AniObjesiTanim item)
        {
            AniObjesiTanim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_ANIOBJESITANIM);
            return deleted;
        }

        public DataTable GetStokluAniObjesiList()
        {
            return lookupRepository.SelectStokluAniObjeleri(ProjeConstants.MTS_ANIOBJESISTOKLU);
        }

        private static List<AniObjesiTanim> ToList(DataTable table)
        {
            return new AniObjesiTanim().ToList<AniObjesiTanim>(table);
        }

        private static AniObjesiTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

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
        private const string TableName = "DepoTanim_Table";
        private readonly MtsLookupRepository repository;

        public DepoTanimService() : this(new MtsLookupRepository())
        {
        }

        public DepoTanimService(MtsLookupRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public DepoTanim GetById(int id)
        {
            return Map(repository.SelectById(TableName, id));
        }

        public List<DepoTanim> GetAll()
        {
            return ToList(repository.SelectAll(TableName));
        }

        public int Save(DepoTanim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(TableName, item);
            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return item.Id;
        }

        public bool Update(DepoTanim item)
        {
            DepoTanim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(TableName, item);
            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return updated;
        }

        public bool Delete(DepoTanim item)
        {
            DepoTanim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(TableName, item.Id);
            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_DEPOTANIM);
            return deleted;
        }

        public DataTable GetStokluAniObjesiList(int aniObjesiId)
        {
            return repository.SelectStokluAniObjesiDepolari(ProjeConstants.MTS_ANIOBJESISTOKLU, aniObjesiId);
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

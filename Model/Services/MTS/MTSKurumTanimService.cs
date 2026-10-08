using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class MTSKurumTanimService
    {
        private const string TableName = "MTSKurumTanim_Table";
        private readonly MtsLookupRepository repository;

        public MTSKurumTanimService()
            : this(new MtsLookupRepository())
        {
        }

        public MTSKurumTanimService(MtsLookupRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public MTSKurumTanim GetById(int id)
        {
            return Map(repository.SelectById(TableName, id));
        }

        public List<MTSKurumTanim> GetAll()
        {
            return ToList(repository.SelectAll(TableName));
        }

        public int Save(MTSKurumTanim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(TableName, item);

            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);

            return item.Id;
        }

        public bool Update(MTSKurumTanim item)
        {
            MTSKurumTanim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(TableName, item);

            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);

            return updated;
        }

        public bool Delete(MTSKurumTanim item)
        {
            MTSKurumTanim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(TableName, item.Id);

            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_KURUMTANIM);

            return deleted;
        }

        private static List<MTSKurumTanim> ToList(DataTable table)
        {
            return new MTSKurumTanim().ToList<MTSKurumTanim>(table);
        }

        private static MTSKurumTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

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
    public class MTSGorevTanimService
    {
        private const string TableName = "MTSGorevTanim_Table";
        private readonly MtsLookupRepository repository;

        public MTSGorevTanimService()
            : this(new MtsLookupRepository())
        {
        }

        public MTSGorevTanimService(MtsLookupRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public MTSGorevTanim GetById(int id)
        {
            return Map(repository.SelectById(TableName, id));
        }

        public List<MTSGorevTanim> GetAll()
        {
            return ToList(repository.SelectAll(TableName));
        }

        public int Save(MTSGorevTanim item)
        {
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(TableName, item);

            if (item.Id > 0 && ProjeConstants.MTS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM);

            return item.Id;
        }

        public bool Update(MTSGorevTanim item)
        {
            MTSGorevTanim old = GetById(item.Id);
            item.DegistirmeTarihi = DateTime.Now;
            item.Degistiren = UtilityHelper.GetCurrentUserName();
            bool updated = item.Id != 0 && repository.Update(TableName, item);

            if (updated && ProjeConstants.MTS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM);

            return updated;
        }

        public bool Delete(MTSGorevTanim item)
        {
            MTSGorevTanim old = GetById(item.Id);
            bool deleted = item.Id != 0 && old != null && repository.Delete(TableName, item.Id);

            if (deleted && ProjeConstants.MTS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.MTS, ProjeConstants.MTS_GOREVTANIM);

            return deleted;
        }

        private static List<MTSGorevTanim> ToList(DataTable table)
        {
            return new MTSGorevTanim().ToList<MTSGorevTanim>(table);
        }

        private static MTSGorevTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

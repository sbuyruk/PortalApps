using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.NBYS
{
    public class BagisaVesileOlanTesekkurService
    {
        private readonly BagisaVesileOlanTesekkurRepository repository;

        public BagisaVesileOlanTesekkurService()
            : this(new BagisaVesileOlanTesekkurRepository())
        {
        }

        public BagisaVesileOlanTesekkurService(BagisaVesileOlanTesekkurRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public BagisaVesileOlanTesekkur GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public DataTable GetListTable()
        {
            return repository.SelectForList();
        }

        public string GetListJson()
        {
            return new BagisaVesileOlanTesekkur().ToJSON(repository.SelectForList());
        }

        public int Save(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(
                    item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return item.Id;
        }

        public bool Update(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            BagisaVesileOlanTesekkur previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(
                    item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return updated;
        }

        public bool Delete(BagisaVesileOlanTesekkur item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;
            BagisaVesileOlanTesekkur previous = GetById(item.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
            return deleted;
        }

        private static List<BagisaVesileOlanTesekkur> Map(DataTable table)
        {
            return new BagisaVesileOlanTesekkur().ToList<BagisaVesileOlanTesekkur>(table);
        }
    }
}

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
    public class ArmaganTanimService
    {
        private readonly ArmaganTanimRepository repository;

        public ArmaganTanimService() : this(new ArmaganTanimRepository())
        {
        }

        public ArmaganTanimService(ArmaganTanimRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public ArmaganTanim GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<ArmaganTanim> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public List<ArmaganTanim> GetActive()
        {
            return Map(repository.SelectActive());
        }

        public ArmaganTanim GetByAmount(decimal amount, bool isCorporate)
        {
            return Map(repository.SelectByAmount(amount, isCorporate)).FirstOrDefault();
        }

        public int Save(ArmaganTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    item, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
            }
            return item.Id;
        }

        public bool Update(ArmaganTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ArmaganTanim previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
            {
                new OlayKayit().GuncellemeOlayKaydet(
                    item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
            }
            return updated;
        }

        public bool Delete(ArmaganTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;

            ArmaganTanim previous = GetById(item.Id);
            if (previous == null)
                return false;

            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGANTANIM);
            }
            return deleted;
        }

        private static List<ArmaganTanim> Map(DataTable table)
        {
            return new ArmaganTanim().ToList<ArmaganTanim>(table);
        }
    }
}

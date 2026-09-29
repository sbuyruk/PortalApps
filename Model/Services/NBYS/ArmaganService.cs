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
    public class ArmaganService
    {
        private readonly ArmaganRepository repository;

        public ArmaganService() : this(new ArmaganRepository())
        {
        }

        public ArmaganService(ArmaganRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Armagan GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<Armagan> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public int Save(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");

            armagan.OlusturmaTarihi = DateTime.Now;
            armagan.Olusturan = UtilityHelper.GetCurrentUserName();
            armagan.Id = repository.Insert(armagan);

            if (armagan.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
            {
                new OlayKayit().GirisOlayKaydet(
                    armagan, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return armagan.Id;
        }

        public bool Update(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");

            Armagan previous = GetById(armagan.Id);
            bool updated = false;
            if (armagan.Id != 0)
            {
                armagan.DegistirmeTarihi = DateTime.Now;
                armagan.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(armagan);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
            {
                new OlayKayit().GuncellemeOlayKaydet(
                    armagan, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return updated;
        }

        public bool Delete(Armagan armagan)
        {
            if (armagan == null)
                throw new ArgumentNullException("armagan");
            if (armagan.Id == 0)
                return false;

            Armagan previous = GetById(armagan.Id);
            if (previous == null)
                return false;

            bool deleted = repository.Delete(armagan.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
            {
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_ARMAGAN);
            }
            return deleted;
        }

        private static List<Armagan> Map(DataTable table)
        {
            return new Armagan().ToList<Armagan>(table);
        }
    }
}

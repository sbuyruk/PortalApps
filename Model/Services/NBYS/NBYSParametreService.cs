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
    public class NBYSParametreService
    {
        private readonly NBYSParametreRepository repository;

        public NBYSParametreService() : this(new NBYSParametreRepository())
        {
        }

        public NBYSParametreService(NBYSParametreRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public NBYSParametre GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<NBYSParametre> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public List<NBYSParametre> GetByGroup(string group)
        {
            return Map(repository.SelectByGroup(group));
        }

        public NBYSParametre GetByGroupAndKey(string group, string key)
        {
            return Map(repository.SelectByGroupAndKey(group, key)).FirstOrDefault();
        }

        public NBYSParametre GetByGroupKeyAndValue(string group, string key, string value)
        {
            return Map(repository.SelectByGroupKeyAndValue(group, key, value)).FirstOrDefault();
        }

        public int Save(NBYSParametre item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(
                    item, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
            return item.Id;
        }

        public bool Update(NBYSParametre item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            NBYSParametre previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(
                    item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
            return updated;
        }

        public bool Delete(NBYSParametre item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;
            NBYSParametre previous = GetById(item.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
            return deleted;
        }

        private static List<NBYSParametre> Map(DataTable table)
        {
            return new NBYSParametre().ToList<NBYSParametre>(table);
        }
    }
}

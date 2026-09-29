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
    public class BankaTanimService
    {
        private readonly BankaTanimRepository repository;

        public BankaTanimService() : this(new BankaTanimRepository())
        {
        }

        public BankaTanimService(BankaTanimRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public BankaTanim GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<BankaTanim> GetAll()
        {
            return Map(repository.SelectAll());
        }

        public BankaTanim GetByName(string name)
        {
            return Map(repository.SelectByName(name)).FirstOrDefault();
        }

        public List<string> GetGroups()
        {
            DataTable table = repository.SelectGroups();
            return table.AsEnumerable()
                .Select(row => row.Field<string>("BankaGrup"))
                .ToList();
        }

        public int Save(BankaTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            item.OlusturmaTarihi = DateTime.Now;
            item.Olusturan = UtilityHelper.GetCurrentUserName();
            item.Id = repository.Insert(item);
            if (item.Id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                new OlayKayit().GirisOlayKaydet(
                    item, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
            return item.Id;
        }

        public bool Update(BankaTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            BankaTanim previous = GetById(item.Id);
            bool updated = false;
            if (item.Id != 0)
            {
                item.DegistirmeTarihi = DateTime.Now;
                item.Degistiren = UtilityHelper.GetCurrentUserName();
                updated = repository.Update(item);
            }
            if (updated && ProjeConstants.NBYS_UPDATE_LOG)
                new OlayKayit().GuncellemeOlayKaydet(
                    item, previous, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
            return updated;
        }

        public bool Delete(BankaTanim item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Id == 0)
                return false;
            BankaTanim previous = GetById(item.Id);
            if (previous == null)
                return false;
            bool deleted = repository.Delete(item.Id);
            if (deleted && ProjeConstants.NBYS_DELETE_LOG)
                new OlayKayit().SilmeOlayKaydet(
                    previous, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
            return deleted;
        }

        private static List<BankaTanim> Map(DataTable table)
        {
            return new BankaTanim().ToList<BankaTanim>(table);
        }
    }
}

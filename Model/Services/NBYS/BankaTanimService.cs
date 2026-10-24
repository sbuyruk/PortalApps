using DAO.Repositories.NBYS;
using DAO.Ortak;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.NBYS
{
    public class BankaTanimService
    {
        private readonly BankaTanimRepository repository;

        public BankaTanimService(PortalLogContext context) : this(new BankaTanimRepository(context)) { }

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

        public BankaTanim GetByName(string name, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectByName(name, transaction)).FirstOrDefault();
        }

        public List<string> GetGroups()
        {
            DataTable table = repository.SelectGroups();
            return table.AsEnumerable()
                .Select(row => row.Field<string>("BankaGrup"))
                .ToList();
        }

        public List<string> GetGroups(PortalLogContext context)
        {
            DataTable table = repository.SelectGroups(context);
            if (table == null) return new List<string>();
            return table.AsEnumerable()
                .Select(row => row.Field<string>("BankaGrup"))
                .Where(group => !string.IsNullOrWhiteSpace(group))
                .ToList();
        }

        public List<string> GetGroups2()
        {
            DataTable table = repository.SelectGroups2();
            return table.AsEnumerable()
                .Select(row => row.Field<string>("BankaGrup2"))
                .ToList();
        }

        private static List<BankaTanim> Map(DataTable table)
        {
            return new BankaTanim().ToList<BankaTanim>(table);
        }
    }
}

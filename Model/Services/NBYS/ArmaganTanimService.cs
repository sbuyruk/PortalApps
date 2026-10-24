using Utility.Logging;
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

namespace Model.Services.NBYS
{
    public class ArmaganTanimService
    {
        private readonly ArmaganTanimRepository repository;

        public ArmaganTanimService(PortalLogContext context) : this(new ArmaganTanimRepository(context)) { }

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

        public ArmaganTanim GetById(int id, PortalLogContext context)
        {
            return Map(repository.SelectById(id, context)).FirstOrDefault();
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

        public ArmaganTanim GetByAmount(
            decimal amount,
            bool isCorporate,
            SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectByAmount(amount, isCorporate, transaction)).FirstOrDefault();
        }

        private static List<ArmaganTanim> Map(DataTable table)
        {
            return new ArmaganTanim().ToList<ArmaganTanim>(table);
        }
    }
}

using Utility.Logging;
using DAO.Repositories.Ortak;
using DAO.Ortak;
using Model.Ortak;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Model.Services.Ortak
{
    public class IlService
    {
        private readonly IlRepository repository;

        public IlService() : this(new IlRepository())
        {
        }

        public IlService(IlRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Il GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public Il GetById(int id, PortalLogContext context)
        {
            return Map(repository.SelectById(id, context));
        }

        public Il GetById(int id, SqlTransactionContext transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            return Map(repository.SelectById(id, transaction));
        }

        public List<Il> GetAll()
        {
            return new Il().ToList<Il>(repository.SelectAll());
        }

        public List<Il> GetAll(PortalLogContext context)
        {
            return new Il().ToList<Il>(repository.SelectAll(context));
        }

        public List<Il> GetAllOrderByRegion()
        {
            return new Il().ToList<Il>(repository.SelectAllOrderByRegion());
        }

        public Il GetByName(string name)
        {
            return Map(repository.SelectByName(name));
        }

        public Il GetByName(string name, SqlTransactionContext transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            return Map(repository.SelectByName(name, transaction));
        }

        public Il GetByEnglishName(string name)
        {
            return Map(repository.SelectByEnglishName(name));
        }

        public Il GetByEnglishName(string name, PortalLogContext context)
        {
            return Map(repository.SelectByEnglishName(name, context));
        }

        public List<Il> GetByRegion(int regionId, bool orderByName)
        {
            return new Il().ToList<Il>(repository.SelectByRegion(regionId, orderByName));
        }

        public int CountByRegion(int regionId)
        {
            DataTable table = repository.SelectCountByRegion(regionId);
            return table == null || table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0]["Adet"]);
        }

        public List<Il> GetWithFTK()
        {
            return new Il().ToList<Il>(repository.SelectWithFTK());
        }

        public DataTable GetFTKRegions()
        {
            return repository.SelectFTKRegions();
        }

        private static Il Map(DataTable table)
        {
            return new Il().ToList<Il>(table).FirstOrDefault();
        }
    }
}

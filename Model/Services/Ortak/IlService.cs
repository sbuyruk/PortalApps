using DAO.Repositories.Ortak;
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

        public List<Il> GetAll()
        {
            return new Il().ToList<Il>(repository.SelectAll());
        }

        public Il GetByName(string name)
        {
            return Map(repository.SelectByName(name));
        }

        public Il GetByEnglishName(string name)
        {
            return Map(repository.SelectByEnglishName(name));
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

        private static Il Map(DataTable table)
        {
            return new Il().ToList<Il>(table).FirstOrDefault();
        }
    }
}

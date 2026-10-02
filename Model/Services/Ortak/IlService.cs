using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Data;
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

        public Il GetByName(string name)
        {
            return Map(repository.SelectByName(name));
        }

        private static Il Map(DataTable table)
        {
            return new Il().ToList<Il>(table).FirstOrDefault();
        }
    }
}

using DAO.Repositories.IKYS;
using Model.IKYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.IKYS
{
    public class IzinTalepService
    {
        private readonly IzinTalepRepository repository;

        public IzinTalepService() : this(new IzinTalepRepository()) { }

        public IzinTalepService(IzinTalepRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public IzinTalep GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<IzinTalep> GetAll()
        {
            return new IzinTalep().ToList<IzinTalep>(repository.SelectAll());
        }

        private static IzinTalep Map(DataTable table)
        {
            return new IzinTalep().ToList<IzinTalep>(table).FirstOrDefault();
        }
    }
}

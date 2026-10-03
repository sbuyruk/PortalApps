using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.TBYS
{
    public class KiraciService
    {
        private readonly KiraciRepository repository;

        public KiraciService() : this(new KiraciRepository()) { }

        public KiraciService(KiraciRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Kiraci GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Kiraci> GetAll()
        {
            return new Kiraci().ToList<Kiraci>(repository.SelectAll());
        }

        public List<Kiraci> GetActiveTenants()
        {
            return new Kiraci().ToList<Kiraci>(repository.SelectActiveTenants());
        }

        private static Kiraci Map(DataTable table)
        {
            return new Kiraci().ToList<Kiraci>(table).FirstOrDefault();
        }
    }
}

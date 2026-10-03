using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.TBYS
{
    public class KiraSozlesmeService
    {
        private readonly KiraSozlesmeRepository repository;

        public KiraSozlesmeService() : this(new KiraSozlesmeRepository()) { }

        public KiraSozlesmeService(KiraSozlesmeRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public KiraSozlesme GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<KiraSozlesme> GetAll()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAll());
        }

        public List<KiraSozlesme> GetAllActive()
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(repository.SelectAllActive());
        }

        private static KiraSozlesme Map(DataTable table)
        {
            return new KiraSozlesme().ToList<KiraSozlesme>(table).FirstOrDefault();
        }
    }
}

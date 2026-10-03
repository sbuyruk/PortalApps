using DAO.Repositories.TBYS;
using Model.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.TBYS
{
    public class SigortaService
    {
        private readonly SigortaRepository repository;

        public SigortaService() : this(new SigortaRepository()) { }

        public SigortaService(SigortaRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public Sigorta GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<Sigorta> GetAll()
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectAll());
        }

        public List<Sigorta> GetByTasinmazId(int tasinmazId)
        {
            return new Sigorta().ToList<Sigorta>(repository.SelectByTasinmazId(tasinmazId));
        }

        public Sigorta GetLatestByTasinmazId(int tasinmazId)
        {
            return GetByTasinmazId(tasinmazId).FirstOrDefault();
        }

        private static Sigorta Map(DataTable table)
        {
            return new Sigorta().ToList<Sigorta>(table).FirstOrDefault();
        }
    }
}

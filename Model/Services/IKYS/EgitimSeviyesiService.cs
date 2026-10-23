using DAO.Repositories.IKYS;
using Model.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.Logging;

namespace Model.Services.IKYS
{
    public class EgitimSeviyesiService
    {
        private readonly EgitimSeviyesiRepository repository;
        public EgitimSeviyesiService() : this(new EgitimSeviyesiRepository()) { }
        public EgitimSeviyesiService(EgitimSeviyesiRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public EgitimSeviyesi GetById(int id) { return Map(repository.SelectById(id)); }
        public List<EgitimSeviyesi> GetAll() { return ToList(repository.SelectAll()); }
        public List<EgitimSeviyesi> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }


        public List<EgitimSeviyesi> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static EgitimSeviyesi Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<EgitimSeviyesi> ToList(DataTable table) { return new EgitimSeviyesi().ToList<EgitimSeviyesi>(table); }

        public EgitimSeviyesi GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<EgitimSeviyesi> GetByPersonelId(int personelId, PortalLogContext context)
        { return ToList(repository.SelectByPersonelId(personelId, context)); }
    }
}

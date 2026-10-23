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
    public class MeslekService
    {
        private readonly MeslekRepository repository;
        public MeslekService() : this(new MeslekRepository()) { }
        public MeslekService(MeslekRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            this.repository = repository;
        }
        public Meslek GetById(int id) { return Map(repository.SelectById(id)); }
        public List<Meslek> GetAll() { return ToList(repository.SelectAll()); }
        public List<Meslek> GetAll(PortalLogContext context)
        { return ToList(repository.SelectAll(context)); }


        public List<Meslek> GetByPersonelId(int personelId) { return ToList(repository.SelectByPersonelId(personelId)); }
        private static Meslek Map(DataTable table) { return ToList(table).FirstOrDefault(); }
        private static List<Meslek> ToList(DataTable table) { return new Meslek().ToList<Meslek>(table); }

        public Meslek GetById(int id, PortalLogContext context)
        { return Map(repository.SelectById(id, context)); }

        public List<Meslek> GetByPersonelId(int personelId, PortalLogContext context)
        { return ToList(repository.SelectByPersonelId(personelId, context)); }
    }
}

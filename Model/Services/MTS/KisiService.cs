using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class KisiService
    {
        private readonly KisiRepository repository;

        public KisiService()
            : this(new KisiRepository())
        {
        }

        public KisiService(Utility.Logging.PortalLogContext context) : this(new KisiRepository(context)) { }

        public KisiService(KisiRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public Kisi GetById(int id)
        {
            return Map(repository.SelectById(id));
        }
        public Kisi GetById(int id, Utility.Logging.PortalLogContext context) { return Map(repository.SelectById(id, context)); }

        public List<Kisi> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetAllData()
        {
            return repository.SelectAllData();
        }

        public List<Kisi> GetBirthdayCelebrations()
        {
            return ToList(repository.SelectBirthdayCelebrations());
        }

        public string GetAllJson()
        {
            return new Kisi().ToJSON(repository.SelectAllJsonData());
        }

        public DataTable GetUnselectedParticipants(int faaliyetId)
        {
            return repository.SelectUnselectedParticipants(faaliyetId);
        }

        public Kisi GetByName(string adi, string soyadi)
        {
            return Map(repository.SelectByName(adi, soyadi));
        }

        public Kisi GetByTcKimlikNo(string tcKimlikNo)
        {
            return Map(repository.SelectByTcKimlikNo(tcKimlikNo));
        }

        private static List<Kisi> ToList(DataTable table)
        {
            return new Kisi().ToList<Kisi>(table);
        }

        private static Kisi Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

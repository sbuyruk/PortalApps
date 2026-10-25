using DAO.Repositories.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Ortak
{
    public class OrtakParametreService
    {
        private readonly OrtakParametreRepository repository;

        public OrtakParametreService()
            : this(new OrtakParametreRepository())
        {
        }

        public OrtakParametreService(OrtakParametreRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public OrtakParametre GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<OrtakParametre> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public OrtakParametre GetByKey(string key)
        {
            return Map(repository.SelectByKey(key));
        }
        public OrtakParametre GetByKey(string key, Utility.Logging.PortalLogContext context)
        {
            return Map(repository.SelectByKey(key, context));
        }


        public List<OrtakParametre> GetByGroup(string group)
        {
            return ToList(repository.SelectByGroup(group));
        }

        public DataTable GetAllData()
        {
            return repository.SelectAllData();
        }

        public string GetAllJson()
        {
            return new OrtakParametre().ToJSON(repository.SelectAllData());
        }

        public List<OrtakParametre> GetByGroupValue(string group, string value)
        {
            return ToList(repository.SelectByGroupValue(group, value));
        }

        private static List<OrtakParametre> ToList(DataTable table)
        {
            return new OrtakParametre().ToList<OrtakParametre>(table);
        }

        private static OrtakParametre Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

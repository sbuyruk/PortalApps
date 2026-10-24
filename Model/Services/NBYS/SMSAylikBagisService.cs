using DAO.Repositories.NBYS;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;
using Utility.Logging;

namespace Model.Services.NBYS
{
    public class SMSAylikBagisService
    {
        private readonly SMSAylikBagisRepository repository;

        public SMSAylikBagisService(PortalLogContext context) : this(new SMSAylikBagisRepository(context)) { }

        public SMSAylikBagisService() : this(new SMSAylikBagisRepository())
        {
        }

        public SMSAylikBagisService(SMSAylikBagisRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            this.repository = repository;
        }

        public SMSAylikBagis GetById(int id)
        {
            return Map(repository.SelectById(id)).FirstOrDefault();
        }

        public List<SMSAylikBagis> GetByYear(int year)
        {
            return Map(repository.SelectByYear(year));
        }

        private static List<SMSAylikBagis> Map(DataTable table)
        {
            return new SMSAylikBagis().ToList<SMSAylikBagis>(table);
        }
    }
}

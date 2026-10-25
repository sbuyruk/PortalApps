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
    public class MTSUnvanTanimService
    {
        private readonly MTSUnvanTanimRepository repository;

        public MTSUnvanTanimService()
            : this(new MTSUnvanTanimRepository())
        {
        }

        public MTSUnvanTanimService(Utility.Logging.PortalLogContext context) : this(new MTSUnvanTanimRepository(context)) { }

        public MTSUnvanTanimService(MTSUnvanTanimRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.repository = repository;
        }

        public MTSUnvanTanim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<MTSUnvanTanim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        private static List<MTSUnvanTanim> ToList(DataTable table)
        {
            return new MTSUnvanTanim().ToList<MTSUnvanTanim>(table);
        }

        private static MTSUnvanTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

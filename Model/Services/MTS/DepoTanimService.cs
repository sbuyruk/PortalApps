using DAO.Repositories.MTS;
using Model.MTS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Services.MTS
{
    public class DepoTanimService
    {
        private readonly DepoTanimRepository repository;

        public DepoTanimService() : this(new DepoTanimRepository())
        {
        }

        public DepoTanimService(Utility.Logging.PortalLogContext context) : this(new DepoTanimRepository(context)) { }

        public DepoTanimService(DepoTanimRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public DepoTanim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<DepoTanim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetStokluAniObjesiList(int aniObjesiId)
        {
            return repository.SelectStokluAniObjesiDepolari(ProjeConstants.MTS_ANIOBJESISTOKLU, aniObjesiId);
        }

        private static List<DepoTanim> ToList(DataTable table)
        {
            return new DepoTanim().ToList<DepoTanim>(table);
        }

        private static DepoTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

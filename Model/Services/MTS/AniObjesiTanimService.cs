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
    public class AniObjesiTanimService
    {
        private readonly AniObjesiTanimRepository repository;

        public AniObjesiTanimService() : this(new AniObjesiTanimRepository())
        {
        }

        public AniObjesiTanimService(Utility.Logging.PortalLogContext context) : this(new AniObjesiTanimRepository(context)) { }

        public AniObjesiTanimService(AniObjesiTanimRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public AniObjesiTanim GetById(int id)
        {
            return Map(repository.SelectById(id));
        }

        public List<AniObjesiTanim> GetAll()
        {
            return ToList(repository.SelectAll());
        }

        public DataTable GetStokluAniObjesiList()
        {
            return repository.SelectStokluAniObjeleri(ProjeConstants.MTS_ANIOBJESISTOKLU);
        }

        private static List<AniObjesiTanim> ToList(DataTable table)
        {
            return new AniObjesiTanim().ToList<AniObjesiTanim>(table);
        }

        private static AniObjesiTanim Map(DataTable table)
        {
            return ToList(table).FirstOrDefault();
        }
    }
}

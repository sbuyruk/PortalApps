using DAO.Repositories.Portal;
using Utility.Logging;
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Portal
{
    public class DuyuruOkumaService
    {
        private readonly DuyuruOkumaRepository r;

        public DuyuruOkumaService(PortalLogContext context) : this(new DuyuruOkumaRepository(context)) { }

        public DuyuruOkumaService()
            : this(new DuyuruOkumaRepository())
        {
        }

        public DuyuruOkumaService(DuyuruOkumaRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public DuyuruOkuma GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<DuyuruOkuma> GetAll()
        {
            return List(r.SelectAll());
        }

        public string GetByDuyuruId(int id)
        {
            return new DuyuruOkuma().ToJSON(r.SelectByDuyuruId(id));
        }

        private static List<DuyuruOkuma> List(DataTable t)
        {
            return new DuyuruOkuma().ToList<DuyuruOkuma>(t);
        }

        private static DuyuruOkuma Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}

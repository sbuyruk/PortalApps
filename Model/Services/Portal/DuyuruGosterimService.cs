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
    public class DuyuruGosterimService
    {
        private readonly DuyuruGosterimRepository r;

        public DuyuruGosterimService(PortalLogContext context) : this(new DuyuruGosterimRepository(context)) { }

        public DuyuruGosterimService()
            : this(new DuyuruGosterimRepository())
        {
        }

        public DuyuruGosterimService(DuyuruGosterimRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public DuyuruGosterim GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<DuyuruGosterim> GetAll()
        {
            return List(r.SelectAll());
        }

        public DuyuruGosterim GetByDuyuruId(int id)
        {
            return Map(r.SelectByDuyuruId(id));
        }

        private static List<DuyuruGosterim> List(DataTable t)
        {
            return new DuyuruGosterim().ToList<DuyuruGosterim>(t);
        }

        private static DuyuruGosterim Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}

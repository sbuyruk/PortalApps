using Utility.Logging;
using DAO.Repositories.Portal;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Portal
{
    public class ToplantiParametreService
    {
        public ToplantiParametreService(PortalLogContext context) : this(new ToplantiParametreRepository(context)) { }
        private readonly ToplantiParametreRepository r;

        public ToplantiParametreService()
            : this(new ToplantiParametreRepository())
        {
        }

        public ToplantiParametreService(ToplantiParametreRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public ToplantiParametre GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public ToplantiParametre GetById(int id, PortalLogContext context) { return Map(r.SelectById(id, context)); }

        public List<ToplantiParametre> GetByGroup(string g)
        {
            return List(r.SelectByGroup(g));
        }

        private static List<ToplantiParametre> List(DataTable t)
        {
            return new ToplantiParametre().ToList<ToplantiParametre>(t);
        }

        private static ToplantiParametre Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}

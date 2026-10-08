using DAO.Repositories.Portal;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.Services.Portal
{
    public class ProgramYetkiService
    {
        private readonly ProgramYetkiRepository r;

        public ProgramYetkiService()
            : this(new ProgramYetkiRepository())
        {
        }

        public ProgramYetkiService(ProgramYetkiRepository r)
        {
            if (r == null)
                throw new ArgumentNullException("r");

            this.r = r;
        }

        public ProgramYetki GetById(int id)
        {
            return Map(r.SelectById(id));
        }

        public List<ProgramYetki> GetAll()
        {
            return List(r.SelectAll());
        }

        public List<ProgramYetki> GetByProgram(string p)
        {
            return List(r.SelectByProgram(p));
        }

        public List<ProgramYetki> GetByProgramModul(string p, string m, string b)
        {
            return List(r.SelectByProgramModul(p, m, b));
        }

        public int Save(ProgramYetki x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            x.OlusturmaTarihi = DateTime.Now;
            x.Id = r.Insert(x);
            return x.Id;
        }

        public bool Update(ProgramYetki x)
        {
            if (x == null || x.Id == 0)
                return false;

            x.DegistirmeTarihi = DateTime.Now;
            return r.Update(x);
        }

        public bool Delete(ProgramYetki x)
        {
            return x != null && x.Id != 0 && r.Delete(x.Id);
        }

        private static List<ProgramYetki> List(DataTable t)
        {
            return new ProgramYetki().ToList<ProgramYetki>(t);
        }

        private static ProgramYetki Map(DataTable t)
        {
            return List(t).FirstOrDefault();
        }
    }
}
































n

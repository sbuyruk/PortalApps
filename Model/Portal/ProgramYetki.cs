using Model.Ortak;
using Model.Services.Portal;
using System;
using System.Collections.Generic;

namespace Model.Portal
{
    public class ProgramYetki : EntityBase
    {
        public int BirimId { get; set; }
        public string Program { get; set; }
        public string Modul { get; set; }
        public string Kosul { get; set; }
        public bool Deger { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new ProgramYetkiService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new ProgramYetkiService().GetAll(), typeof(List<T>));
        }

        public int Save()
        {
            return new ProgramYetkiService().Save(this);
        }

        public bool Update()
        {
            return new ProgramYetkiService().Update(this);
        }

        public bool Delete()
        {
            return new ProgramYetkiService().Delete(this);
        }

        public List<ProgramYetki> SelectByProgram(string p)
        {
            return new ProgramYetkiService().GetByProgram(p);
        }

        public List<ProgramYetki> SelectByProgramModul(string p, string m, string b)
        {
            return new ProgramYetkiService().GetByProgramModul(p, m, b);
        }
    }
}



































using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class BulunmamaSebebi : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new BulunmamaSebebiService().GetById(id), typeof(T)); }
        public BulunmamaSebebi Select(int id) { return new BulunmamaSebebiService().GetById(id); }
        public override int Save() { return new BulunmamaSebebiService().Save(this); }
        public override bool Update() { return new BulunmamaSebebiService().Update(this); }
        public override bool Delete() { return new BulunmamaSebebiService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new BulunmamaSebebiService().GetAll(), typeof(List<T>)); }
    }
}

using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class UnvanTanim : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new UnvanTanimService().GetById(id), typeof(T)); }
        public override int Save() { return new UnvanTanimService().Save(this); }
        public override bool Update() { return new UnvanTanimService().Update(this); }
        public override bool Delete() { return new UnvanTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new UnvanTanimService().GetAll(), typeof(List<T>)); }
    }
}

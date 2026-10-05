using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class IzinTanim : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new IzinTanimService().GetById(id), typeof(T)); }
        public override int Save() { return new IzinTanimService().Save(this); }
        public override bool Update() { return new IzinTanimService().Update(this); }
        public override bool Delete() { return new IzinTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IzinTanimService().GetAll(), typeof(List<T>)); }
    }
}

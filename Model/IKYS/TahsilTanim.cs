using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class TahsilTanim : ParentClass
    {
        public string TahsilDurumu { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new TahsilTanimService().GetById(id), typeof(T)); }
        public TahsilTanim Select(int id) { return new TahsilTanimService().GetById(id); }
        public override int Save() { return new TahsilTanimService().Save(this); }
        public override bool Update() { return new TahsilTanimService().Update(this); }
        public override bool Delete() { return new TahsilTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new TahsilTanimService().GetAll(), typeof(List<T>)); }
    }
}

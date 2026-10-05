using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class Meslek : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new MeslekService().GetById(id), typeof(T)); }
        public override int Save() { return new MeslekService().Save(this); }
        public override bool Update() { return new MeslekService().Update(this); }
        public override bool Delete() { return new MeslekService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new MeslekService().GetAll(), typeof(List<T>)); }
        public List<Meslek> SelectByPersonelId(int personelId) { return new MeslekService().GetByPersonelId(personelId); }
    }
}

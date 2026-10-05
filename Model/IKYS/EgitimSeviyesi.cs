using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class EgitimSeviyesi : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new EgitimSeviyesiService().GetById(id), typeof(T)); }
        public override int Save() { return new EgitimSeviyesiService().Save(this); }
        public override bool Update() { return new EgitimSeviyesiService().Update(this); }
        public override bool Delete() { return new EgitimSeviyesiService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new EgitimSeviyesiService().GetAll(), typeof(List<T>)); }
        public List<EgitimSeviyesi> SelectByPersonelId(int personelId) { return new EgitimSeviyesiService().GetByPersonelId(personelId); }
    }
}

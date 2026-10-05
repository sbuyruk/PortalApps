using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class Egitim : ParentClass
    {
        public int PersonelId { get; set; }
        public string Seviye { get; set; }
        public string Okul { get; set; }
        public DateTime MezuniyetTar { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new EgitimService().GetById(id), typeof(T)); }
        public override int Save() { return new EgitimService().Save(this); }
        public override bool Update() { return new EgitimService().Update(this); }
        public override bool Delete() { return new EgitimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new EgitimService().GetAll(), typeof(List<T>)); }
        public List<Egitim> SelectByPersonelId(int personelId) { return new EgitimService().GetByPersonelId(personelId); }
    }
}

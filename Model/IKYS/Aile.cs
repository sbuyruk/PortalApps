using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class Aile : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TcKimlikNo { get; set; }
        public int YakinlikDerecesi { get; set; }
        public DateTime DogumTar { get; set; }
        public string Tahsil { get; set; }
        public string Okul { get; set; }
        public string Telefon { get; set; }
        public int Meslek { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new AileService().GetById(id), typeof(T)); }
        public override int Save() { return new AileService().Save(this); }
        public override bool Update() { return new AileService().Update(this); }
        public override bool Delete() { return new AileService().Delete(this); }
        public Aile Select(int id) { return new AileService().GetById(id); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new AileService().GetAll(), typeof(List<T>)); }
        public List<Aile> SelectByPersonelId(int personelId) { return new AileService().GetByPersonelId(personelId); }
        public Aile SelectEnGencCocukByPersonelId(int personelId) { return new AileService().GetEnGencCocukByPersonelId(personelId); }
    }
}

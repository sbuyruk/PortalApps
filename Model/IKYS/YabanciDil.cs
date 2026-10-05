using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class YabanciDil : ParentClass
    {
        public int PersonelId { get; set; }
        public string Dil { get; set; }
        public string SinavAdi { get; set; }
        public string SinavNotu { get; set; }
        public DateTime SinavTarihi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new YabanciDilService().GetById(id), typeof(T)); }
        public override int Save() { return new YabanciDilService().Save(this); }
        public override bool Update() { return new YabanciDilService().Update(this); }
        public override bool Delete() { return new YabanciDilService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new YabanciDilService().GetAll(), typeof(List<T>)); }
        public List<YabanciDil> SelectByPersonelId(int personelId) { return new YabanciDilService().GetByPersonelId(personelId); }
    }
}

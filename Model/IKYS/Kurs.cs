using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class Kurs : ParentClass
    {
        public int PersonelId { get; set; }
        public string KursAdi { get; set; }
        public string VerenKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string Sure { get; set; }
        public string Adres { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new KursService().GetById(id), typeof(T)); }
        public override int Save() { return new KursService().Save(this); }
        public override bool Update() { return new KursService().Update(this); }
        public override bool Delete() { return new KursService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new KursService().GetAll(), typeof(List<T>)); }
        public List<Kurs> SelectByPersonelId(int personelId) { return new KursService().GetByPersonelId(personelId); }
    }
}

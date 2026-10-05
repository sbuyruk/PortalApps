using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class Mahsup : ParentClass
    {
        public int PersonelId { get; set; }
        public int IzinHareketId { get; set; }
        public int IzinTipi { get; set; }
        public int KullanildigiDonemId { get; set; }
        public int MahsupDonemId { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new MahsupService().GetById(id), typeof(T)); }
        public override int Save() { return new MahsupService().Save(this); }
        public override bool Update() { return new MahsupService().Update(this); }
        public override bool Delete() { return new MahsupService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new MahsupService().GetAll(), typeof(List<T>)); }
        public List<Mahsup> SelectByDonemId(int donemId) { return new MahsupService().GetByDonemId(donemId); }
        public List<Mahsup> SelectByPersonelId(int personelId, int izinTipi) { return new MahsupService().GetByPersonelId(personelId, izinTipi); }
    }
}

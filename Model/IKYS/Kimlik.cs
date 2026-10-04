using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class Kimlik : ParentClass
    {
        public int PersonelId { get; set; }
        public string TCKimlikNo { get; set; }
        public string AnneAdi { get; set; }
        public string BabaAdi { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTar { get; set; }
        public string MedeniHali { get; set; }
        public DateTime EvlilikTar { get; set; }
        public string Cinsiyet { get; set; }
        public string EskiSoyadi { get; set; }
        public string KanGrubu { get; set; }
        public bool DogumGunuKutlama { get; set; }
        public bool EvlilikKutlama { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new KimlikService().GetById(id), typeof(T)); }
        public override int Save() { return new KimlikService().Save(this); }
        public override bool Update() { return new KimlikService().Update(this); }
        public override bool Delete() { return new KimlikService().Delete(this); }
        public Kimlik Select(int id) { Id = id; return new KimlikService().GetById(id); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new KimlikService().GetAll(), typeof(List<T>)); }
        public Kimlik SelectByPersonelId(int personelId) { return new KimlikService().GetByPersonelId(personelId); }
        public Kimlik SelectByTCKimlikNo(string kimlikNo) { return new KimlikService().GetByTcKimlikNo(kimlikNo); }
        public DataTable SelectAllFromKIMLIK() { return new KimlikService().GetAllFromKimlik(); }
    }
}

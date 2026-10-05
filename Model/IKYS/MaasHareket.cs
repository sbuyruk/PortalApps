using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;

namespace Model.IKYS
{
    public class MaasHareket : ParentClass
    {
        public int PersonelId { get; set; }
        public DateTime Tarih { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvan { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public DateTime DereceKademeIlerlemeTarihi { get; set; }
        public decimal Ucret { get; set; }
        public decimal Ikramiye { get; set; }
        public decimal Agi { get; set; }
        public decimal ToplamUcret { get; set; }
        public int GrupId { get; set; }
        public int ProtokolSiraNo { get; set; }

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new MaasHareketService().GetById(id), typeof(T)); }
        public MaasHareket Select(int id) { Id = id; return new MaasHareketService().GetById(id); }
        public override int Save() { return new MaasHareketService().Save(this); }
        public override bool Update() { return new MaasHareketService().Update(this); }
        public override bool Delete() { return new MaasHareketService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new MaasHareketService().GetAll(), typeof(List<T>)); }
        public MaasHareket SelectByTarih(DateTime tarih) { return new MaasHareketService().GetByTarih(tarih); }
        public List<MaasHareket> SelectMaasListesiByTarih(DateTime tarih) { return new MaasHareketService().GetMaasListesiByTarih(tarih); }
        public bool DeleteByGrupId(int grupId) { return new MaasHareketService().DeleteByGrupId(this, grupId); }
    }
}

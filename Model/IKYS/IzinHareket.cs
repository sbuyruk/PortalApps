using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class IzinHareket : ParentClass
    {
        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public int IzinTalepId { get; set; }
        public int IzinDonemId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Birim { get; set; }
        public string Adres { get; set; }
        public int VekilImza { get; set; }
        public int AmirImza { get; set; }
        public int OnayImza { get; set; }
        public bool Mahsup { get; set; }
        public string Aciklama { get; set; }
        public string OncekiIzinStr { get; set; }
        public string KullanilanIzinStr { get; set; }
        public string KalanIzinStr { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new IzinHareketService().GetById(id), typeof(T)); }
        public override int Save() { return new IzinHareketService().Save(this); }
        public override bool Update() { return new IzinHareketService().Update(this); }
        public override bool Delete() { return new IzinHareketService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IzinHareketService().GetAll(), typeof(List<T>)); }
        public List<IzinHareket> SelectByTarihReturnList(DateTime tarih) { return new IzinHareketService().GetByTarih(tarih); }
        public IzinHareket SelectByIzinTalepId(int izinTalepId) { return new IzinHareketService().GetByIzinTalepId(izinTalepId); }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar) { return new IzinHareketService().GetByTarihReturnDataTable(bastar, bittar); }
        public IzinHareket SelectByPersonelTarih(int personelId, DateTime bastar, DateTime bittar) { return new IzinHareketService().GetByPersonelTarih(personelId, bastar, bittar); }
        public DataTable SelectByIzinTipiTarihReturnDataTable(int izinTipi, DateTime ilkTarih, DateTime bittar) { return new IzinHareketService().GetByIzinTipiTarihReturnDataTable(izinTipi, ilkTarih, bittar); }
        public DataTable SelectByIzinDonemiReturnDataTable(int personelId, int izinTipi, DateTime izinDonemiBasi, DateTime izinDonemiSonu) { return new IzinHareketService().GetByIzinDonemiReturnDataTable(personelId, izinTipi, izinDonemiBasi, izinDonemiSonu); }
        public DataTable SelectByIzinDonemiReturnDataTable(int izinDonemId, int personelId, int izinTipi) { return new IzinHareketService().GetByIzinDonemiReturnDataTable(izinDonemId, personelId, izinTipi); }
        public string SelectByPersonelIdDonemIdReturnJson(int personelId, int donemId, int izinTanimId) { return new IzinHareketService().GetByPersonelIdDonemIdReturnJson(personelId, donemId, izinTanimId); }
        public List<IzinHareket> SelectDigerIzinlerByPersonelIdReturnJson(int personelId) { return new IzinHareketService().GetDigerByPersonelId(personelId); }
        public string ConvertDataTabletoString(DataTable dt) { return new IzinHareketService().ConvertDataTableToString(dt); }
    }
}

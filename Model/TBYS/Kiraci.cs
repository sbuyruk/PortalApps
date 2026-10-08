using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class Kiraci : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TCKimlikNo { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public string Aciklama { get; set; }
        public string KiralamaAmaci { get; set; }

        public T Select<T>(int id) { return (T)Convert.ChangeType(new KiraciService().GetById(id), typeof(T)); }
        public Kiraci Select(int id) { Id = id; return new KiraciService().GetById(id); }
        public int Save() { return new KiraciService().Save(this); }
        public bool Update() { return new KiraciService().Update(this); }
        public bool Delete() { return new KiraciService().Delete(this); }
        public List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new KiraciService().GetAll(), typeof(List<T>)); }
        public List<Kiraci> SelectAktifKiracilar() { return new KiraciService().GetActiveTenants(); }
        public string SelectAllReturnJson() { return new KiraciService().GetAllReturnJson(); }
        public DataTable SelectAllReturnDT(string secim, int bolgeId) { return new KiraciService().GetAllReturnDataTable(secim, bolgeId); }
        public DataTable SelectByByBolgeReturnDT(string aktif, string bolge) { return new KiraciService().GetByBolge(aktif, bolge); }
        public DataTable SelectAktifSozlesmesiOlmayanKiracilarReturnDT(int bolgeId) { return new KiraciService().GetWithoutActiveContract(bolgeId); }
        public DataTable SelectByFilterReturnDataTable(string filter) { return new KiraciService().GetByFilter(filter); }
        public string SelectByFilter(string filter) { return new KiraciService().GetByFilterJson(filter); }
        public Kiraci SelectNext() { return new KiraciService().GetNext(Id); }
        public Kiraci SelectPrev() { return new KiraciService().GetPrevious(Id); }
        public Kiraci SelectMax() { return new KiraciService().GetMax(); }
        public Kiraci SelectMin() { return new KiraciService().GetMin(); }
        public List<Kiraci> SelectByAdi(string adi, string soyadi = "") { return new KiraciService().GetByName(adi, soyadi); }
    }
}

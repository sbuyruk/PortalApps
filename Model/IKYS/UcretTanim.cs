using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class UcretTanim : ParentClass
    {
        public int GrupId { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Unvan { get; set; }
        public decimal AltUcret { get; set; }
        public decimal UstUcret { get; set; }
        public decimal AskerUcret { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Agi { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new UcretTanimService().GetById(id), typeof(T)); }
        public UcretTanim Select(int id) { Id = id; return new UcretTanimService().GetById(id); }
        public override int Save() { return new UcretTanimService().Save(this); }
        public override bool Update() { return new UcretTanimService().Update(this); }
        public override bool Delete() { return new UcretTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new UcretTanimService().GetAll(), typeof(List<T>)); }
        public List<UcretTanim> SelectByKademe(int grupId, int kademe) { return new UcretTanimService().GetByKademe(grupId, kademe); }
        public List<UcretTanim> SelectByMaxGrupId() { return new UcretTanimService().GetByMaxGrupId(); }
        public DataTable SelectByGrup() { return new UcretTanimService().GetByGrup(); }
        public DataTable SelectDerece() { return new UcretTanimService().GetDerece(); }
        public DataTable SelectKademe(int derece, int grupId) { return new UcretTanimService().GetKademe(derece, grupId); }
        public DataTable SelectMaasListesi(int grupId, DateTime tarih) { return new UcretTanimService().GetMaasListesi(grupId, tarih); }
        public DateTime SelectMaxBitisTarihi() { return new UcretTanimService().GetMaxBitisTarihi(); }
        public decimal SelectAgi(DateTime tarih) { return new UcretTanimService().GetAgi(tarih); }
        public int SelectGrupIdByTarih(DateTime tarih) { return new UcretTanimService().GetGrupIdByTarih(tarih); }
        public bool DeleteByGrupId(int grupId) { return new UcretTanimService().DeleteByGrupId(grupId, this); }
        public decimal SelectUcretByGrupDereceKademe(Personel personel, int grupId, int derece, int kademe) { return new UcretTanimService().GetUcretByGrupDereceKademe(personel, grupId, derece, kademe); }
    }
}

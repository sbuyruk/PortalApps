using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class IzinDonem : ParentClass
    {
        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string IzinHakki { get; set; }
        public string KullanilanIzin { get; set; }
        public string KalanIzin { get; set; }
        public string Birim { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new IzinDonemService().GetById(id), typeof(T)); }
        public override int Save() { return new IzinDonemService().Save(this); }
        public override bool Update() { return new IzinDonemService().Update(this); }
        public override bool Delete() { return new IzinDonemService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IzinDonemService().GetAll(), typeof(List<T>)); }
        public IzinDonem SelectByIzinTarihi(int personelId, int izinTipi, DateTime tarih) { return new IzinDonemService().GetByIzinTarihi(personelId, izinTipi, tarih); }
        public List<IzinDonem> SelectOncekiYillaraAitIzinDonemleri(int personelId) { return new IzinDonemService().GetOncekiYillaraAit(personelId); }
        public List<IzinDonem> SelectByPersonelId(int personelId, int izinTipi) { return new IzinDonemService().GetByPersonelId(personelId, izinTipi); }
        public DataTable SelectSUMKalanIzinByPersonelId(int personelId, bool sadeceEskiDonemler) { return new IzinDonemService().GetSumKalanIzinByPersonelId(personelId, sadeceEskiDonemler); }
        public string SelectByPersonelIdReturnJSon(int personelId, int izinTipi) { return new IzinDonemService().GetByPersonelIdReturnJson(personelId, izinTipi); }
        public DataTable SelectByPersonelIdReturnDT(int personelId, int izinTipi) { return new IzinDonemService().GetByPersonelIdReturnDataTable(personelId, izinTipi); }
        public IzinDonem IzinDonemiOlustur(Personel personel, int izinTipi, DateTime tarih, string currentUserName) { return new IzinDonemService().CreateForPersonel(personel, izinTipi, tarih, currentUserName); }
        public IzinDonem IzinDonemiGuncelle(Personel personel, int izinTipi, DateTime tarih, string currentUserName) { return new IzinDonemService().UpdateForPersonel(personel, izinTipi, tarih, currentUserName); }
        public bool KullanilanIzinGuncelle(IzinDonem izinDonemi, string sure, bool ekle, string currentUserName) { return new IzinDonemService().UpdateUsedLeave(izinDonemi, sure, ekle, currentUserName); }
    }
}

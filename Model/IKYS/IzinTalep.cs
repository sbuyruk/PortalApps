using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class IzinTalep : ParentClass
    {
        public IzinTalep() { Aktif = true; }
        public int PersonelId { get; set; }
        public int IzinTipi { get; set; }
        public int IzinDonemId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Birim { get; set; }
        public int VekilImza { get; set; }
        public int AmirImza { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public int OnayImza { get; set; }
        public int OnayDurumu { get; set; }
        public bool Aktif { get; set; }
        public bool EPostaGonder { get; set; }

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new IzinTalepService().GetById(id), typeof(T)); }
        public override int Save() { return new IzinTalepService().Save(this); }
        public override bool Update() { return new IzinTalepService().Update(this); }
        public override bool Delete() { return new IzinTalepService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new IzinTalepService().GetAll(), typeof(List<T>)); }
        public string SelectIzinTalepleriReturnJson(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri) { return new IzinTalepService().GetIzinTalepleriReturnJson(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri); }
        public DataTable SelectIzinTalepleriReturnDT(int personelId, int izinTipi, bool mazeretHaric, bool sadeceGecerliDonemTalepleri) { return new IzinTalepService().GetIzinTalepleriReturnDataTable(personelId, izinTipi, mazeretHaric, sadeceGecerliDonemTalepleri); }
        public IzinTalep SelectByPersonelIdBasBitTar(int personelId, DateTime basTar, DateTime bitTar) { return new IzinTalepService().GetByPersonelIdBasBitTar(personelId, basTar, bitTar); }
        public IzinTalep SelectIslemiDevamEdenIzinTalebiVarMi(int personelId, int izinTipi) { return new IzinTalepService().GetIslemiDevamEden(personelId, izinTipi); }
        public IzinTalep SelectSonIzinTalebiByPersonel(int izinTipi, int personelId) { return new IzinTalepService().GetSonByPersonel(izinTipi, personelId); }
    }
}

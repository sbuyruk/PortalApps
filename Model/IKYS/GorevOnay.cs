using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class GorevOnay : ParentClass
    {
        public enum AmirOnayDurumu
        {
            [System.ComponentModel.DataAnnotations.Display(Name = "Amirin Onayı Gerekli")] OnayBekliyor = 0,
            [System.ComponentModel.DataAnnotations.Display(Name = "Onaylandı")] Onaylandi = 1,
            [System.ComponentModel.DataAnnotations.Display(Name = "Reddedildi")] Reddedildi = 2,
            [System.ComponentModel.DataAnnotations.Display(Name = "Onay Gerekmiyor")] OnayGerekmez = 3,
            [System.ComponentModel.DataAnnotations.Display(Name = "Diğer")] Diger = 4
        }

        public int PersonelId { get; set; }
        public string GorevinSebebi { get; set; }
        public string GorevinYeri { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Sure { get; set; }
        public string Avans { get; set; }
        public string Yevmiye { get; set; }
        public string GunlukYevmiye { get; set; }
        public string ParaBirimi { get; set; }
        public string UlasimAraci { get; set; }
        public bool AracTahsisi { get; set; }
        public string AracPlakasi { get; set; }
        public int PerSubeImza { get; set; }
        public bool PerSubeVekil { get; set; }
        public int OnayImza { get; set; }
        public int OnayMakam { get; set; }
        public bool OnayMakamVekil { get; set; }
        public int GMImza { get; set; }
        public bool GMVekil { get; set; }
        public string Aciklama { get; set; }
        public bool Secildi { get; set; }
        public bool Odendi { get; set; }
        public int AmirOnayi { get; set; }
        public string Transfer { get; set; }
        public string Konaklama { get; set; }
        public string OnayRedAciklama { get; set; }
        public int OncekiId { get; set; } = 0;

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new GorevOnayService().GetById(id), typeof(T)); }
        public GorevOnay Select(int id) { Id = id; return new GorevOnayService().GetById(id); }
        public override int Save() { return new GorevOnayService().Save(this); }
        public override bool Update() { return new GorevOnayService().Update(this); }
        public override bool Delete() { return new GorevOnayService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new GorevOnayService().GetAll(), typeof(List<T>)); }
        public bool UpdateAllSecildiToFalse() { return new GorevOnayService().UpdateAllSecildiToFalse(); }
        public bool UpdateAllSecildiToTrue(string idString) { return new GorevOnayService().UpdateAllSecildiToTrue(idString); }
        public bool UpdateAllOdendiToTrue(string idString) { return new GorevOnayService().UpdateAllOdendiToTrue(idString); }
        public DataTable SelectAllReturnDT(int personelId, DateTime since) { return new GorevOnayService().GetAllReturnDataTable(personelId, since); }
        public DataTable SelectAllByPersonelReturnDT(int personelId) { return new GorevOnayService().GetAllByPersonelReturnDataTable(personelId); }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar) { return new GorevOnayService().GetByTarihReturnDataTable(bastar, bittar); }
        public GorevOnay SelectByPersonelTarih(int personelId, DateTime bastar, DateTime bittar) { return new GorevOnayService().GetByPersonelTarih(personelId, bastar, bittar); }
        public List<GorevOnay> SelectAllBySecildi(bool secildi) { return new GorevOnayService().GetAllBySecildi(secildi); }
        public DataTable SelectBekleyenAmirOnayiByBirimIdsReturnDataTable(string birimIdListStr) { return new GorevOnayService().GetBekleyenAmirOnayiByBirimIds(birimIdListStr); }
        public bool GorevOnayVarMi(int personelId, DateTime basTarih, DateTime bitTarih, int gorevOnayId) { return new GorevOnayService().HasDateConflict(personelId, basTarih, bitTarih, gorevOnayId); }
    }
}

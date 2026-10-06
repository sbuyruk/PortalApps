using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class GorevOnay : EntityBase
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

    }
}

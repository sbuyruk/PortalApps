using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IzinTalep : EntityBase
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

    }
}

using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IzinHareket : EntityBase
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
    }
}

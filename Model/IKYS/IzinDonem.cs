using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IzinDonem : EntityBase
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
    }
}

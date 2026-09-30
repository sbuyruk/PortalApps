using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class EkstreAktarma : EntityBase
    {
        public long TCKimlikNo { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public DateTime BagisTarihi { get; set; }
        public string BankaAdi { get; set; }
        public string DovizCinsi { get; set; }
        public decimal Tutar { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        public string DosyaYolu { get; set; }
        public bool AktarildiMi { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public DateTime IslemTarihi { get; set; }
        public bool TuzelKisi { get; set; }
        public bool ElleKayit { get; set; }
        public int NakitBagisHareketId { get; set; }
        public int NakitBagisciId { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public string FisNo { get; set; }
        public string BagisTipi { get; set; }
    }
}

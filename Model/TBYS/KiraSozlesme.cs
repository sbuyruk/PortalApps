using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class KiraSozlesme : EntityBase
    {
        public int KiraciId { get; set; }
        public DateTime IlkSozlesmeTar { get; set; }
        public DateTime SozBasTar { get; set; }
        public DateTime SozBitTar { get; set; }
        public decimal KiraBedeli { get; set; }
        public string OdemeSekli { get; set; }
        public int TaksitSayisi { get; set; }
        public string KefilAdiSoyadi { get; set; }
        public string KefilTCKimlikNo { get; set; }
        public string KefilAdresi { get; set; }
        public string KefilTel { get; set; }
        public string TeminatCinsi { get; set; }
        public decimal TeminatTutari { get; set; }
        public decimal OdenenTeminatTutari { get; set; }
        public decimal IadeTeminatTutari { get; set; }
        public decimal KalanTeminatTutari { get; set; }
        public DateTime TeminatOdemeTarihi { get; set; }
        public string TeminatAciklama { get; set; }
        public int DosyaNo { get; set; }
        public decimal DevirAnaPara { get; set; }
        public decimal DevirFaizTutari { get; set; }
        public decimal DevirFaizliBakiye { get; set; }
        public bool Aktif { get; set; }
        public string SozlesmeDurumu { get; set; }
        public DateTime DurumDegismeTar { get; set; }
        public string Aciklama { get; set; }
        public string SozlesmePDFDosyasi { get; set; }
        public string ArtisAyi { get; set; }
        public string Bolge { get; set; }
        public int BolgeId { get; set; }
        public string GecikmeZammiTipi { get; set; }
    }
}

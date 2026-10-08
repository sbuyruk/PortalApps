using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class TasinmazBagisci : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string EPosta { get; set; }
        public string Meslegi { get; set; }
        public string SosyalGuvence { get; set; }
        public string Foto { get; set; }
        public string Sag_vefat { get; set; }
        public DateTime VefatTarihi { get; set; }
        public string DefinYeri { get; set; }
        public string DefinIli { get; set; }
        public string DefinIlcesi { get; set; }
        public string DefinAciklama { get; set; }
        public string Aciklama { get; set; }
        public bool Gizli { get; set; }
        public string Tahsil { get; set; }
    }
}

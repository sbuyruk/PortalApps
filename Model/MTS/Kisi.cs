using Model.Ortak;
using System;

namespace Model.MTS
{
    public class Kisi : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }

        public int MTSUnvanTanimId { get; set; }
        public string Kurumu { get; set; }
        public string Unvani { get; set; }
        public string Gorevi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Telefon3 { get; set; }
        public string TelAciklama1 { get; set; }
        public string TelAciklama2 { get; set; }
        public string TelAciklama3 { get; set; }
        public string EPosta { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Aciklama { get; set; }
        public string Dahili1 { get; set; }
        public string Dahili2 { get; set; }
        public string Dahili3 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Kutlama { get; set; }
        public bool RandevuKisiti { get; set; }

    }
}

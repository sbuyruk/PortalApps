using Model.Ortak;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class Katilimci : EntityBase
    {
        [Required]
        [DisplayName("Katilimci Tipi")]
        public int KatilimciTipi { get; set; }

        [Required]
        [DisplayName("Adi")]
        public string Adi { get; set; }
        [Required]
        [DisplayName("Soyadi")]
        public string Soyadi { get; set; }
        [DisplayName("TC Kimlik No")]
        public long TCKimlikNo { get; set; }
        [DisplayName("Kurumu")]
        public string Kurumu { get; set; }
        [DisplayName("Ünvani")]
        public string Unvani { get; set; }
        [DisplayName("Görevi")]
        public string Gorevi { get; set; }
        [DisplayName("Telefon 1")]
        public string Telefon1 { get; set; }
        [DisplayName("Telefon 2")]
        public string Telefon2 { get; set; }
        
        [DisplayName("Telefon 3")]
        public string Telefon3 { get; set; }
        
        [DisplayName("Açiklama 1")]
        public string TelAciklama1 { get; set; }
        
        [DisplayName("Açiklama 2")]
        public string TelAciklama2 { get; set; }
        
        [DisplayName("Açiklama 3")]
        public string TelAciklama3 { get; set; }
        
        [DisplayName("Adres")]
        public string Adres { get; set; }
            
        [DisplayName("EPosta")]
        public string EPosta { get; set; }
        
        [DisplayName("Il")]
        public int Ili { get; set; }
        
        [DisplayName("Ilçe")]
        public int Ilcesi { get; set; }
        
        [DisplayName("Dahili Telefon 1")]
        public string Dahili1 { get; set; }
        
        [DisplayName("Dahili Telefon 2")]
        public string Dahili2 { get; set; }
        
        [DisplayName("Dahili Telefon 3")]
        public string Dahili3 { get; set; }
        
        [DisplayName("Dogum Tarihi")]
        public DateTime DogumTarihi { get; set; }
        
        [DisplayName("Kutlama")]
        public bool Kutlama { get; set; } = false;
        [DisplayName("Randevu Kisiti")]
        public bool RandevuKisiti { get; set; } = false;

    }
}

using Model.Ortak;
using System;
using System.ComponentModel.DataAnnotations;

namespace Model.IKYS
{
    [Serializable]
    public class Personel : EntityBase
    {
        public enum PersonelTipi { [Display(Name = "Tüm Personel")] Tumu = 0, [Display(Name = "Kadrolu Personel")] Kadrolu = 1, [Display(Name = "Kadro Harici Personel")] Kadrosuz = 2 }
        public int PerId { get; set; }
        public int SicilNo { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public int Tahsili { get; set; }
        public string KullaniciAdi { get; set; }
        public int Asker_sivil { get; set; }
        public int Tipi { get; set; } = (int)PersonelTipi.Kadrolu;
    }
}

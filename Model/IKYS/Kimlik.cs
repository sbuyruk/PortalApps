using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Kimlik : EntityBase
    {
        public int PersonelId { get; set; }
        public string TCKimlikNo { get; set; }
        public string AnneAdi { get; set; }
        public string BabaAdi { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTar { get; set; }
        public string MedeniHali { get; set; }
        public DateTime EvlilikTar { get; set; }
        public string Cinsiyet { get; set; }
        public string EskiSoyadi { get; set; }
        public string KanGrubu { get; set; }
        public bool DogumGunuKutlama { get; set; }
        public bool EvlilikKutlama { get; set; }
    }
}

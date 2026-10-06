using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Aile : EntityBase
    {
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TcKimlikNo { get; set; }
        public int YakinlikDerecesi { get; set; }
        public DateTime DogumTar { get; set; }
        public string Tahsil { get; set; }
        public string Okul { get; set; }
        public string Telefon { get; set; }
        public int Meslek { get; set; }

    }
}

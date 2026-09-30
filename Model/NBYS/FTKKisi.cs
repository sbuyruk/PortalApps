using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class FTKKisi : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Unvani { get; set; }
        public bool Vali { get; set; }
        public bool Kaymakam { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Aciklama { get; set; }
        public string KartNo { get; set; }
        public int FTKGorevi { get; set; }
        public string UyelikDurumu { get; set; }
    }
}

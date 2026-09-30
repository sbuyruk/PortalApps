using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class FTK : EntityBase
    {
        public int FTKIslemId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public int BolgeId { get; set; }
        public DateTime KurulusTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string FTKGorevi { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvani { get; set; }
        public string Telefon { get; set; }
        public string KartNo { get; set; }
        public int Sayac { get; set; }
        public int KisiId { get; set; }
        public string Aciklama { get; set; }
    }
}

using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class FTKIslem : EntityBase
    {
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public int BolgeId { get; set; }
        public DateTime KurulusTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string Aciklama { get; set; }
    }
}

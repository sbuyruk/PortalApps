using Model.Ortak;
using System;

namespace Model.MTS
{
    public class AniObjesiDagitim : EntityBase
    {
        public int AniObjesiId { get; set; }
        public int Adet { get; set; }
        public int KatilimciId { get; set; }
        public int FaaliyetId { get; set; }
        public int VerilenAlinan { get; set; }
        public int DagitimYeriTanimId { get; set; } = 0;
        public int CikisDepoId { get; set; } = 0;
        public string GetirilenAniObjesi { get; set; }
        public string Aciklama { get; set; }
        public DateTime VerilisTarihi { get; set; }

    }
}

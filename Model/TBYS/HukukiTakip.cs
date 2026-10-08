using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class HukukiTakip : EntityBase
    {
        public int SozlesmeId { get; set; }
        public int KiraciId { get; set; }
        public decimal BorcAnaPara { get; set; }
        public decimal BorcFaiz { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
    }
}

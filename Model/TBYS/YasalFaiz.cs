using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class YasalFaiz : EntityBase
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal Tufe { get; set; }
        public decimal Ufe { get; set; }
        public string Aciklama { get; set; }
    }
}

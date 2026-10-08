using Model.Ortak;
using System;

namespace Model.MTS
{
    public class AramaGorusme : EntityBase
    {
        public int ArayanId { get; set; }
        public int FaaliyetId { get; set; }
        public DateTime Tarih { get; set; }
        public string GorusmeSekli { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public bool GorusmeSaglandi { get; set; }
        public bool RandevuIstendi { get; set; }

    }
}

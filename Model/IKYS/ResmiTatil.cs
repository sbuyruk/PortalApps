using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class ResmiTatil : EntityBase
    {
        public int Gun { get; set; }
        public int Ay { get; set; }
        public int Yil { get; set; }
        public string Tatil { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime IlanTarihi { get; set; }
        public DateTime IptalTarihi { get; set; }
    }
}

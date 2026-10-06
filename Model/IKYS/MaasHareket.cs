using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class MaasHareket : EntityBase
    {
        public int PersonelId { get; set; }
        public DateTime Tarih { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvan { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public DateTime DereceKademeIlerlemeTarihi { get; set; }
        public decimal Ucret { get; set; }
        public decimal Ikramiye { get; set; }
        public decimal Agi { get; set; }
        public decimal ToplamUcret { get; set; }
        public int GrupId { get; set; }
        public int ProtokolSiraNo { get; set; }

    }
}

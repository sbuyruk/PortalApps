using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class UcretTanim : EntityBase
    {
        public int GrupId { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Unvan { get; set; }
        public decimal AltUcret { get; set; }
        public decimal UstUcret { get; set; }
        public decimal AskerUcret { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Agi { get; set; }
    }
}

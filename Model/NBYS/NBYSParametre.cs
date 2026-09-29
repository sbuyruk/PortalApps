using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class NBYSParametre : EntityBase
    {
        public string Grup { get; set; }
        public string Anahtar { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
    }
}

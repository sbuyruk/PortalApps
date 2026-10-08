using System;

namespace Model.Ortak
{
    [Serializable]
    public class OrtakParametre : EntityBase
    {
        public string Grup { get; set; }
        public string Anahtar { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
    }
}

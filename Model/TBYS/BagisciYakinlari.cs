using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciYakinlari : EntityBase
    {
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string YakinlikDerecesi { get; set; }
        public int BagisciId { get; set; }
    }
}

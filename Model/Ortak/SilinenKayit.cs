using System;

namespace Model.Ortak
{
    public class SilinenKayit : EntityBase
    {
        public string TabloAdi { get; set; }
        public string SilinenKayitBilgisi { get; set; }
        public string Silen { get; set; }
        public DateTime SilinmeTarihi { get; set; }
        public string SilinmeSebebi { get; set; }
    }
}

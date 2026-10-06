using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Kurs : EntityBase
    {
        public int PersonelId { get; set; }
        public string KursAdi { get; set; }
        public string VerenKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string Sure { get; set; }
        public string Adres { get; set; }

    }
}

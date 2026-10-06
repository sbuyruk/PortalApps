using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class YabanciDil : EntityBase
    {
        public int PersonelId { get; set; }
        public string Dil { get; set; }
        public string SinavAdi { get; set; }
        public string SinavNotu { get; set; }
        public DateTime SinavTarihi { get; set; }
        public string Aciklama { get; set; }

    }
}

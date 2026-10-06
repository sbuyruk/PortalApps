using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Egitim : EntityBase
    {
        public int PersonelId { get; set; }
        public string Seviye { get; set; }
        public string Okul { get; set; }
        public DateTime MezuniyetTar { get; set; }
        public string Aciklama { get; set; }

    }
}

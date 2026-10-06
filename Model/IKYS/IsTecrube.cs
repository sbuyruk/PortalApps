using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IsTecrube : EntityBase
    {
        public int PersonelId { get; set; }
        public string Isyeri { get; set; }
        public string Gorevi { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
        public string Adres { get; set; }

    }
}

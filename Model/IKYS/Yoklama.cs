using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Yoklama : EntityBase
    {
        public int PersonelId { get; set; }
        public int BulunmamaSebebi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Aciklama { get; set; }
        public string Adres { get; set; }

    }
}

using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class DereceKademeDegisim : EntityBase
    {
        public int PersonelId { get; set; }
        public string Degisim { get; set; }
        public DateTime DegisimTarihi { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Aciklama { get; set; }

    }
}

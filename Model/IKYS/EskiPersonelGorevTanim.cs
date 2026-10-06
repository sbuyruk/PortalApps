using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class EskiPersonelGorevTanim : EntityBase
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }

    }
}

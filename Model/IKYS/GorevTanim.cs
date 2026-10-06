using Model.Ortak;
using System;
using static Model.IKYS.Personel;

namespace Model.IKYS
{
    public class GorevTanim : EntityBase
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }
        public int HarcirahGrupId { get; set; }

    }
}

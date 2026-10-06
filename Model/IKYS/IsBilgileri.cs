using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class IsBilgileri : EntityBase
    {
        public int PersonelId { get; set; }
        public int UnvanId { get; set; }
        public int GorevId { get; set; }
        public int BirimId { get; set; }
        public DateTime BaslamaTar { get; set; }
        public DateTime IzinDonemiBasTar { get; set; }
        public int CalismaDurumu { get; set; }
        public DateTime AyrilmaTar { get; set; }
        public string AyrilmaSebebi { get; set; }
        public int ProtokolSiraNo { get; set; }
        public string SGKSicilNo { get; set; }
        public DateTime SGKBasTar { get; set; }
        public int VakifOncesiPrimGunSayisi { get; set; }
        public DateTime EmeklilikTarihi { get; set; }
        public string Aciklama { get; set; }

    }
}

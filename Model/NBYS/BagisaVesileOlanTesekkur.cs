using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class BagisaVesileOlanTesekkur : EntityBase
    {

        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvan { get; set; }
        public long TCKimlikNo { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public DateTime BelgeTarihi { get; set; }
        public string VerilmeSebebi { get; set; }
        public string BelgeMetni1 { get; set; }
        public string BelgeMetni2 { get; set; }
        public string ImzalayanAdiSoyadi { get; set; }
        public string ImzalayanUnvan { get; set; }
        public string ImzalayanMakam { get; set; }       
        public string Aciklama { get; set; }
    }
}

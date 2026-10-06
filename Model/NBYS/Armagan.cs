using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class Armagan : EntityBase
    {
        public int BagisciId { get; set; }
        public int ArmaganTanimId { get; set; }
        public DateTime Tarih { get; set; }
        public string Durum { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public string BelgedeYazanIsim { get; set; }
        public int BelgeGecersizMi { get; set; }
        public int GecersizNBHareketId { get; set; }
        public string GecersizYapan { get; set; }
        public DateTime GecersizYapmaTarihi { get; set; }
        public decimal ArmaganBagisMiktari { get; set; }
        public decimal IadeMiktari { get; set; }
        public bool BagisMiktariYazmasin { get; set; }
        public bool CokluBagis { get; set; }
        public bool DuzenliBagis { get; set; }= false;
        // Düzenli bağışta hak kazanılan 12 bağışlık yıl: 12 => 1, 24 => 2, 36 => 3.
        public int KacinciBelge { get; set; }= 0;
    }
}

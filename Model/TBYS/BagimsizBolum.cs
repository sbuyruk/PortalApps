using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class BagimsizBolum : EntityBase
    {
        public int TasinmazId { get; set; }
        public string BolumNo { get; set; }
        public string Nitelik { get; set; }
        public decimal BBBrutAlan { get; set; }
        public decimal BBNetAlan { get; set; }
        public string KullanimAmaci { get; set; }
        public decimal MuhasebeyeKayitliDeger { get; set; }
        public decimal TahminiRayicDegeri { get; set; }
        public decimal EmlakBeyanDegeri { get; set; }
        public decimal YaklasikPiyasaDegeri { get; set; }
        public string Aciklama { get; set; }
    }
}

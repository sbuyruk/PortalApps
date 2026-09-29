using Model.Ortak;

namespace Model.NBYS
{
    public class ArmaganTanim : EntityBase
    {
        public string Armagan { get; set; }
        public decimal OzelKisiAltLimit { get; set; }
        public decimal OzelKisiUstLimit { get; set; }
        public decimal TuzelKisiAltLimit { get; set; }
        public decimal TuzelKisiUstLimit { get; set; }
        public string ImzaGorevi { get; set; }
        public string ImzaAdi { get; set; }
    }
}

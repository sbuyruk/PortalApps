using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciTalepleri : EntityBase
    {
        public string Talep { get; set; }
        public string Irtibat { get; set; }
        public string Tarih { get; set; }
        public string Aciklama { get; set; }
        public int BagisciId { get; set; }
    }
}

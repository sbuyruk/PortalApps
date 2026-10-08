using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class Odeme : EntityBase
    {
        public int SozlesmeId { get; set; }
        public int KiraciId { get; set; }
        public int OdemePlaniId { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public decimal OdenenTutar { get; set; }
        public string Aciklama { get; set; }
    }
}

using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciBagis : EntityBase
    {
        public int BagisciId { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }
    }
}

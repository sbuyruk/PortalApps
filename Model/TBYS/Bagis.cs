using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class Bagis : EntityBase
    {
        public int BagisciId { get; set; }
        public int TasinmazId { get; set; }
        public DateTime BagisTarihi { get; set; }
        public int BagisYili { get; set; }
        public bool Envanterde { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }
    }
}

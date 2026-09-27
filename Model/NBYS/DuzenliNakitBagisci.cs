using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class DuzenliNakitBagisci : EntityBase
    {

        public int BagisciId { get; set; }
        public long TCKimlikNo { get; set; }
        public string BagisciAdi { get; set; }
        public int BagisAdedi { get; set; }
        public decimal BagisToplami { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Tutar { get; set; }
        public bool Aktif { get; set; }
        public int ArmaganId { get; set; }
        public int NakitBagisHareketId { get; set; }
        public string Telefon { get; set; }
        public string EPosta { get; set; }
        public string EslesmeBilgisi { get; set; }
        public string Aciklama { get; set; }
    }
}

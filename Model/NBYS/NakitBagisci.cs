using Model.Ortak;
using System;

namespace Model.NBYS
{
    [Serializable]
    public class NakitBagisci : EntityBase
    {
        private string _soyadi;

        public string Adi { get; set; }
        public string Soyadi
        {
            get { return _soyadi ?? string.Empty; }
            set { _soyadi = value; }
        }
        public long TCKimlikNo { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public bool TuzelKisi { get; set; }
        public bool Sag { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Meslek { get; set; }
        public string Aciklama { get; set; }
        public bool Ulasilamiyor { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public bool DergiGonderilmesin { get; set; }
    }
}

using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class Kiraci : EntityBase
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TCKimlikNo { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public int IlId { get; set; }
        public int IlceId { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Eposta { get; set; }
        public string Aciklama { get; set; }
        public string KiralamaAmaci { get; set; }

    }
}

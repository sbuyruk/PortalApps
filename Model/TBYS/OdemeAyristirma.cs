using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeAyristirma : EntityBase
    {
        public int KiraEkstreAktarmaId { get; set; }
        public int KiraciId { get; set; }
        public int SozlesmeId { get; set; }
        public int TeminatIslemId { get; set; }
        public int OdemeId { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public string OdemeSaati { get; set; }
        public int OdemeSebebiId { get; set; }
        public decimal Tutar { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
    }
}

using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class TeminatIslem : EntityBase
    {
        public int KiraciId { get; set; }
        public int DosyaNo { get; set; }
        public string IslemTipi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public decimal IslemTutari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public int OdemeId { get; set; }


    }
}

using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class KiraBorcuTakip : EntityBase
    {
        public int KiraciId { get; set; }
        public int KiraSozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public decimal KiraBedeli { get; set; }
        public decimal ToplamBorcu { get; set; }
        public int KiraBorcuAySayisi { get; set; }
        public string TakipIslemi { get; set; }
        public int IslemAyi { get; set; }
        public int IslemYili { get; set; }
        public string IslemYapan { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string TebligEdilenKisi { get; set; }
        public DateTime TebligTarihi { get; set; }
        public string Bolge { get; set; }
        public string Aciklama { get; set; }
    }
}

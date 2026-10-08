using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeAyrinti : EntityBase
    {
        public int KiraciId { get; set; }
        public int SozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public int OdemeId { get; set; }
        public int GecikmeZammiId { get; set; }
        public int OdemePlaniSirasi { get; set; }
        public DateTime SonOdemeTarihi { get; set; }
        public DateTime GecikmeZammiDegisimTar { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public DateTime IlkTarih { get; set; }
        public DateTime SonTarih { get; set; }
        public int AySayisi { get; set; }
        public int GunSayisi { get; set; }
        public decimal GecikmeZammiOrani { get; set; }
        public decimal AnaPara { get; set; }
        public decimal OdenenTutar { get; set; }
        public decimal KalanAnaPara { get; set; }
        public decimal GecikmeZammiTutari { get; set; }
        public string Aciklama { get; set; }
    }
}

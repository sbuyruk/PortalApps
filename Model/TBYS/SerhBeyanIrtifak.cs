using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class SerhBeyanIrtifak : EntityBase
    {
        public int TasinmazId { get; set; }
        public string SBI { get; set; }
        public string MalikLehtar { get; set; }
        public string TesisKurum { get; set; }
        public DateTime Tarih { get; set; }
        public string TerkinSebebi { get; set; }
        public string Yevmiye { get; set; }
        public string Aciklama { get; set; }

    }
}

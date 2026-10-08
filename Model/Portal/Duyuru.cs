using Model.Ortak;
using System;

namespace Model.Portal
{
    [Serializable]
    public class Duyuru : EntityBase
    {
        public string Baslik { get; set; }
        public string Metin { get; set; }
        public DateTime YayinBasTar { get; set; }
        public DateTime YayinBitTar { get; set; }
        public string Tekrar { get; set; }
        public string DuyuruAlicilari { get; set; }
        public string Resim { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
        public bool Popup { get; set; }
    }
}

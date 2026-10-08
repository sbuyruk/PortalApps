using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class GecikmeZammi : EntityBase
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ZamOrani { get; set; }
        public string Aciklama { get; set; }
    }
}

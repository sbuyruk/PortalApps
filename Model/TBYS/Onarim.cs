using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class Onarim : EntityBase
    {
        public int TasinmazId { get; set; }
        public string YapilanIs { get; set; }
        public string HarcamaUsulu { get; set; }
        public DateTime OnayTarihi { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }

    }
}

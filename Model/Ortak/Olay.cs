using System;

namespace Model.Ortak
{
    [Serializable]
    public class Olay : EntityBase
    {
        public string Program { get; set; }
        public string IslemTipi { get; set; }
        public string IslemKonusu { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string IslemYapan { get; set; }
        public string Aciklama { get; set; }
    }
}

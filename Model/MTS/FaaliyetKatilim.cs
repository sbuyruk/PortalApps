using Model.Ortak;
using System;

namespace Model.MTS
{
    public class FaaliyetKatilim : EntityBase
    {
        public int FaaliyetId { get; set; }
        public int KatilimciId { get; set; }
        public int KatilimciTipi { get; set; } = 2;
        public string KurumGorev { get; set; }
        public string TakvimDaveti { get; set; }
        public string Aciklama { get; set; }

    }
}

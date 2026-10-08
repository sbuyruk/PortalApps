using Model.Ortak;
using System;

namespace Model.Portal
{
    public class Toplanti : EntityBase
    {
        public Guid UniqueId { get; set; }
        public string ToplantiKonusu { get; set; }
        public int ToplantiYeri { get; set; }
        public string ToplantiYeriDiger { get; set; }
        public int ToplantiYetkilisi { get; set; }
        public int Koordinator { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public string DisKatilimcilar { get; set; }
        public bool CevrimIci { get; set; }
        public bool IkramOnayi { get; set; }
        public string IkramMalzemesi { get; set; }
        public string Aciklama { get; set; }

    }
}


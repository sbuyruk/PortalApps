using Model.Ortak;

namespace Model.IKYS
{
    public class Mahsup : EntityBase
    {
        public int PersonelId { get; set; }
        public int IzinHareketId { get; set; }
        public int IzinTipi { get; set; }
        public int KullanildigiDonemId { get; set; }
        public int MahsupDonemId { get; set; }
        public string Aciklama { get; set; }

    }
}

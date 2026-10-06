using Model.Ortak;
using System;

namespace Model.IKYS
{
    public class Harcirah : EntityBase
    {
        public int KadroGrupId { get; set; }
        public int SeriId { get; set; }
        public int Sira { get; set; }
        public string KadroGrubu { get; set; }
        public string Ulke { get; set; }
        public decimal Miktar { get; set; }
        public string ParaBirimi { get; set; }
        public string Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
    }
}

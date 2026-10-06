using Model.Ortak;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class BirimTanim : EntityBase
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public int ParentId { get; set; }
        public int AmirId { get; set; }
        public int Sira { get; set; }
        public bool Aktif { get; set; }
        public bool BirimKaldirildi { get; set; } = false;
        public int BolgeId { get; set; } = ProjeConstants.BOLGE_GENELMUDURLUK_INT;
    }
}

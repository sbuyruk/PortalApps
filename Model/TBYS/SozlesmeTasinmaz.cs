using Model.Ortak;
using System;

namespace Model.TBYS
{
    [Serializable]
    public class SozlesmeTasinmaz : EntityBase
    {
        public int SozlesmeId { get; set; }
        public int TasinmazId { get; set; }
        public int BolumId { get; set; }

    }
}

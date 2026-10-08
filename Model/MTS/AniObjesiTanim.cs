using Model.Ortak;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class AniObjesiTanim : EntityBase
    {
        [Required]
        [DisplayName("Ani Objesinin Adi")]
        public string Adi { get; set; }
        public string StokluMu { get; set; }
        public int Sira { get; set; }
        [Required]
        public int KaynakId { get; set; }
        public string Aciklama { get; set; }

    }
}

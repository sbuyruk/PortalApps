using Model.Ortak;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class DepoTanim : EntityBase
    {
        [Required]
        [DisplayName("Depo Adi")]
        public string Adi { get; set; }
        public string Aciklama { get; set; }

    }
}

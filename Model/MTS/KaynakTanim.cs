using Model.Ortak;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class KaynakTanim : EntityBase
    {
        [Required]
        [DisplayName("Ani Objesi Kaynagi")]
        public string Adi { get; set; }

    }
}

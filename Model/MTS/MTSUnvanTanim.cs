using Model.Ortak;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class MTSUnvanTanim : EntityBase
    {
        [Required]
        [DisplayName("Kurum Adi")]
        public string Adi { get; set; }

        [DisplayName("Kisa Adi")]
        public string KisaAdi { get; set; }

    }
}

using Model.Ortak;
using System;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class DepoStok : EntityBase
    {
        public int AniObjesiId { get; set; }
        public int DepoId { get; set; }

        [Required]
        public int SonAdet { get; set; } = 0;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SonIslemTarihi { get; set; } = DateTime.Now;
        public string SonIslemYapan { get; set; }
        public string Aciklama { get; set; }

    }
}

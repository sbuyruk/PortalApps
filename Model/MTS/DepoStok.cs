using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
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

        public int Save()
        {
            return new DepoStokService().Save(this);
        }

        public bool Update()
        {
            return new DepoStokService().Update(this);
        }

        public bool Delete()
        {
            return new DepoStokService().Delete(this);
        }

        public DepoStok Select(int id)
        {
            Id = id;
            return new DepoStokService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new DepoStokService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new DepoStokService().GetAll(), typeof(List<T>));
        }

        public DepoStok SelectByDepoIdAniObjesiId(int depoId, int aniObjesiId, string stokluMu)
        {
            return new DepoStokService().GetByDepoIdAniObjesiId(depoId, aniObjesiId, stokluMu);
        }
    }
}

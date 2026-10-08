using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

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

        public int Save()
        {
            return new AniObjesiTanimService().Save(this);
        }

        public bool Update()
        {
            return new AniObjesiTanimService().Update(this);
        }

        public bool Delete()
        {
            return new AniObjesiTanimService().Delete(this);
        }

        public AniObjesiTanim Select(int id)
        {
            Id = id;
            return new AniObjesiTanimService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new AniObjesiTanimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new AniObjesiTanimService().GetAll(), typeof(List<T>));
        }

        public DataTable SelectStokluAniObjesiList()
        {
            return new AniObjesiTanimService().GetStokluAniObjesiList();
        }
    }
}

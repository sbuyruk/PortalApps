using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Model.MTS
{
    public class DepoTanim : EntityBase
    {
        [Required]
        [DisplayName("Depo Adi")]
        public string Adi { get; set; }
        public string Aciklama { get; set; }

        public int Save()
        {
            return new DepoTanimService().Save(this);
        }

        public bool Update()
        {
            return new DepoTanimService().Update(this);
        }

        public bool Delete()
        {
            return new DepoTanimService().Delete(this);
        }

        public DepoTanim Select(int id)
        {
            Id = id;
            return new DepoTanimService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new DepoTanimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new DepoTanimService().GetAll(), typeof(List<T>));
        }

        public DataTable SelectStokluAniObjesiList(int secilenAniObjesiId)
        {
            return new DepoTanimService().GetStokluAniObjesiList(secilenAniObjesiId);
        }
    }
}

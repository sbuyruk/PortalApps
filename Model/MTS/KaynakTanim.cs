using Model.Ortak;
using Model.Services.MTS;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class KaynakTanim : EntityBase
    {
        [Required]
        [DisplayName("Ani Objesi Kaynagi")]
        public string Adi { get; set; }

        public int Save()
        {
            return new KaynakTanimService().Save(this);
        }
        public bool Update()
        {
            return new KaynakTanimService().Update(this);
        }
        public bool Delete()
        {
            return new KaynakTanimService().Delete(this);
        }
        public KaynakTanim Select(int id)
        {
            Id = id;
            return new KaynakTanimService().GetById(id);
        }
        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(Select(id), typeof(T));
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KaynakTanimService().GetAll(), typeof(List<T>));
        }
    }
}

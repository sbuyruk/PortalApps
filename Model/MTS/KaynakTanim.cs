using Model.Ortak;
using Model.Services.MTS;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class KaynakTanim : ParentClass
    {
        [Required]
        [DisplayName("Ani Objesi Kaynagi")]
        public string Adi { get; set; }

        public override int Save()
        {
            return new KaynakTanimService().Save(this);
        }
        public override bool Update()
        {
            return new KaynakTanimService().Update(this);
        }
        public override bool Delete()
        {
            return new KaynakTanimService().Delete(this);
        }
        public KaynakTanim Select(int id)
        {
            Id = id;
            return new KaynakTanimService().GetById(id);
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(Select(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new KaynakTanimService().GetAll(), typeof(List<T>));
        }
    }
}

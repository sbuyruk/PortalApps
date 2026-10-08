using Model.Ortak;
using Model.Services.MTS;
using System.Collections.Generic;
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

        public int Save()
        {
            return new MTSUnvanTanimService().Save(this);
        }

        public bool Update()
        {
            return new MTSUnvanTanimService().Update(this);
        }

        public bool Delete()
        {
            return new MTSUnvanTanimService().Delete(this);
        }

        public MTSUnvanTanim Select(int id)
        {
            Id = id;
            return new MTSUnvanTanimService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)System.Convert.ChangeType(new MTSUnvanTanimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)System.Convert.ChangeType(new MTSUnvanTanimService().GetAll(), typeof(List<T>));
        }
    }
}

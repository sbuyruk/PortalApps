using Model.Ortak;
using Model.Services.MTS;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class MTSGorevTanim : EntityBase
    {
        [Required]
        [DisplayName("Kurum Adi")]
        public string Adi { get; set; }

        [DisplayName("Kisa Adi")]
        public string KisaAdi { get; set; }

        public int Save()
        {
            return new MTSGorevTanimService().Save(this);
        }

        public bool Update()
        {
            return new MTSGorevTanimService().Update(this);
        }

        public bool Delete()
        {
            return new MTSGorevTanimService().Delete(this);
        }

        public MTSGorevTanim Select(int id)
        {
            Id = id;
            return new MTSGorevTanimService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)System.Convert.ChangeType(new MTSGorevTanimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)System.Convert.ChangeType(new MTSGorevTanimService().GetAll(), typeof(List<T>));
        }
    }
}

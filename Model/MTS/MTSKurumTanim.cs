using Model.Ortak;
using Model.Services.MTS;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model.MTS
{
    public class MTSKurumTanim : EntityBase
    {
        [Required]
        [DisplayName("Kurum Adi")]
        public string Adi { get; set; }

        [DisplayName("Kisa Adi")]
        public string KisaAdi { get; set; }

        public int Save()
        {
            return new MTSKurumTanimService().Save(this);
        }

        public bool Update()
        {
            return new MTSKurumTanimService().Update(this);
        }

        public bool Delete()
        {
            return new MTSKurumTanimService().Delete(this);
        }

        public MTSKurumTanim Select(int id)
        {
            Id = id;
            return new MTSKurumTanimService().GetById(id);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)System.Convert.ChangeType(new MTSKurumTanimService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)System.Convert.ChangeType(new MTSKurumTanimService().GetAll(), typeof(List<T>));
        }
    }
}

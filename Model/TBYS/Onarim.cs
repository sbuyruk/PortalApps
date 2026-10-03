using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class Onarim : ParentClass
    {
        public int TasinmazId { get; set; }
        public string YapilanIs { get; set; }
        public string HarcamaUsulu { get; set; }
        public DateTime OnayTarihi { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new OnarimService().GetById(id), typeof(T));
        }

        public override int Save()
        {
            return new OnarimService().Save(this);
        }

        public override bool Update()
        {
            return new OnarimService().Update(this);
        }

        public override bool Delete()
        {
            return new OnarimService().Delete(this);
        }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(
                new OnarimService().GetAll(),
                typeof(List<T>));
        }

        public List<Onarim> SelectByTasinmazId(int tasinmazId)
        {
            return new OnarimService().GetByTasinmazId(tasinmazId);
        }

        public DataTable SelectAllReturnDataTable()
        {
            return new OnarimService().GetAllForDataTable();
        }

        public List<Onarim> SelectByOnarimId(int onarimId)
        {
            return new OnarimService().GetByOnarimId(onarimId);
        }

        public Onarim SelectNext(int onarimId)
        {
            return new OnarimService().GetNext(onarimId);
        }

        public Onarim SelectPrev(int onarimId)
        {
            return new OnarimService().GetPrevious(onarimId);
        }

        public Onarim SelectMax()
        {
            return new OnarimService().GetMax();
        }

        public List<Onarim> SelectOnarimByTasinmazId(int tasinmazId)
        {
            return new OnarimService().GetByTasinmazIdWithAddress(tasinmazId);
        }

        public Onarim SelectMin()
        {
            return new OnarimService().GetMin();
        }
    }
}

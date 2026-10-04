using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeSebebiTanim : ParentClass
    {
        public string OdemeSebebi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new OdemeSebebiTanimService().GetById(id), typeof(T));
        }

        public OdemeSebebiTanim Select(int id)
        {
            return new OdemeSebebiTanimService().GetById(id);
        }

        public override int Save() { return new OdemeSebebiTanimService().Save(this); }
        public override bool Update() { return new OdemeSebebiTanimService().Update(this); }
        public override bool Delete() { return new OdemeSebebiTanimService().Delete(this); }

        public override List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new OdemeSebebiTanimService().GetAll(), typeof(List<T>));
        }
    }
}

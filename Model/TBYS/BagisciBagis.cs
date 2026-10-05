using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciBagis : ParentClass
    {
        public int BagisciId { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new BagisciBagisService().GetById(id), typeof(T)); }
        public BagisciBagis Select(int id) { return new BagisciBagisService().GetById(id); }
        public override int Save() { return new BagisciBagisService().Save(this); }
        public override bool Update() { return new BagisciBagisService().Update(this); }
        public override bool Delete() { return new BagisciBagisService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new BagisciBagisService().GetAll(), typeof(List<T>)); }
        public List<BagisciBagis> SelectByBagisId(int bagisId) { return new BagisciBagisService().GetByBagisId(bagisId); }
    }
}

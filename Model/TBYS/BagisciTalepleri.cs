using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciTalepleri : ParentClass
    {
        public string Talep { get; set; }
        public string Irtibat { get; set; }
        public string Tarih { get; set; }
        public string Aciklama { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new BagisciTalepleriService().GetById(id), typeof(T)); }
        public override int Save() { return new BagisciTalepleriService().Save(this); }
        public override bool Update() { return new BagisciTalepleriService().Update(this); }
        public override bool Delete() { return new BagisciTalepleriService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new BagisciTalepleriService().GetAll(), typeof(List<T>)); }
        public List<BagisciTalepleri> SelectByBagisciId(int bagisciId) { return new BagisciTalepleriService().GetByBagisciId(bagisciId); }
    }
}

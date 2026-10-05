using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;

namespace Model.TBYS
{
    [Serializable]
    public class BagisciYakinlari : ParentClass
    {
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string YakinlikDerecesi { get; set; }
        public int BagisciId { get; set; }
        public override T Select<T>(int id) { return (T)Convert.ChangeType(new BagisciYakinlariService().GetById(id), typeof(T)); }
        public override int Save() { return new BagisciYakinlariService().Save(this); }
        public override bool Update() { return new BagisciYakinlariService().Update(this); }
        public override bool Delete() { return new BagisciYakinlariService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new BagisciYakinlariService().GetAll(), typeof(List<T>)); }
        public List<BagisciYakinlari> SelectByBagisciId(int bagisciId) { return new BagisciYakinlariService().GetByBagisciId(bagisciId); }
    }
}

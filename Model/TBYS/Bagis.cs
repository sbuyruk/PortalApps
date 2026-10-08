using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class Bagis : EntityBase
    {
        public int BagisciId { get; set; }
        public int TasinmazId { get; set; }
        public DateTime BagisTarihi { get; set; }
        public int BagisYili { get; set; }
        public bool Envanterde { get; set; }
        public string ArmaganId { get; set; }
        public string ArmaganDurumu { get; set; }
        public DateTime ArmaganTarihi { get; set; }
        public string ArmaganAciklama { get; set; }

        private static BagisService Service { get { return new BagisService(); } }

        public T Select<T>(int id) { return (T)Convert.ChangeType(Service.GetById(id), typeof(T)); }
        public Bagis Select(int id) { return Service.GetById(id); }
        public int Save() { return Service.Save(this); }
        public bool Update() { return Service.Update(this); }
        public bool Delete() { return Service.Delete(this); }
        public List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(Service.GetAll(), typeof(List<T>)); }
        public List<Bagis> SelectByBagisciId(int bagisciId) { return Service.GetByBagisciId(bagisciId); }
        public DataTable SelectByBagisciIdGroupByKullanimSekli(int bagisciId) { return Service.GetByBagisciIdGroupByKullanimSekli(bagisciId); }
        public string SelectByBagisciIdReturnJson(int bagisciId) { return Service.GetByBagisciIdAsJson(bagisciId); }
        public Bagis SelectByTasinmazId(int tasinmazId) { return Service.GetByTasinmazId(tasinmazId); }
        public string SelectTasinmazByBagisciIdReturnJson(int bagisciId) { return Service.GetTasinmazByBagisciIdAsJson(bagisciId); }
        public DataTable SelectTasinmazByBagisciIdReturnDT(int bagisciId) { return Service.GetTasinmazByBagisciId(bagisciId); }
        public DataTable SelectSatisVsDahilTasinmazByBagisciIdReturnDT(int bagisciId) { return Service.GetSatisVsDahilTasinmazByBagisciId(bagisciId); }
        public decimal SelectSumTahminiRayicByBagisciId(int bagisciId) { return Service.GetSumTahminiRayicByBagisciId(bagisciId); }
    }
}

using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class Yoklama : ParentClass
    {
        public int PersonelId { get; set; }
        public int BulunmamaSebebi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Aciklama { get; set; }
        public string Adres { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new YoklamaService().GetById(id), typeof(T)); }
        public Yoklama Select(int id) { return new YoklamaService().GetById(id); }
        public override int Save() { return new YoklamaService().Save(this); }
        public override bool Update() { return new YoklamaService().Update(this); }
        public override bool Delete() { return new YoklamaService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new YoklamaService().GetAll(), typeof(List<T>)); }
        public string SelectAllReturnJson(int personelId) { return new YoklamaService().GetAllByPersonelIdAsJson(personelId); }
        public DataTable SelectAllReturnDataTable(int personelId) { return new YoklamaService().GetAllByPersonelId(personelId); }
        public DataTable SelectByTarihReturnDataTable(DateTime bastar, DateTime bittar) { return new YoklamaService().GetByTarih(bastar, bittar); }
        public DataTable SelectByTarihReturnDataTable(string bulunmamaSbebiIds, DateTime bastar, DateTime bittar) { return new YoklamaService().GetByTarihAndReasons(bulunmamaSbebiIds, bastar, bittar); }
        public List<Yoklama> SelectByPersonelIdTarih(int personelId, DateTime bastar, DateTime bittar) { return new YoklamaService().GetByPersonelIdAndDate(personelId, bastar, bittar); }
    }
}

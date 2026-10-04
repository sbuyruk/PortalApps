using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class Harcirah : ParentClass
    {
        public int KadroGrupId { get; set; }
        public int SeriId { get; set; }
        public int Sira { get; set; }
        public string KadroGrubu { get; set; }
        public string Ulke { get; set; }
        public decimal Miktar { get; set; }
        public string ParaBirimi { get; set; }
        public string Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new HarcirahService().GetById(id), typeof(T)); }
        public Harcirah Select(int id) { Id = id; return new HarcirahService().GetById(id); }
        public override int Save() { return new HarcirahService().Save(this); }
        public override bool Update() { return new HarcirahService().Update(this); }
        public override bool Delete() { return new HarcirahService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new HarcirahService().GetAll(), typeof(List<T>)); }
        public List<Harcirah> SelectByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih) { return new HarcirahService().GetByKadroUlkeTarih(kadroGrupId, ulke, tarih); }
        public Harcirah SelectByKadroGrupId(int kadroGrupId) { return new HarcirahService().GetByKadroGrupId(kadroGrupId); }
        public Harcirah SelectByParaBirimi(string paraBirimi) { return new HarcirahService().GetByParaBirimi(paraBirimi); }
        public DataTable SelectAllReturnDataTable() { return new HarcirahService().GetAllReturnDataTable(); }
        public List<Harcirah> SelectAll() { return new HarcirahService().GetAll(); }
    }
}

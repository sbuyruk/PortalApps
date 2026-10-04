using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class BirimTanim : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public int ParentId { get; set; }
        public int AmirId { get; set; }
        public int Sira { get; set; }
        public bool Aktif { get; set; }
        public bool BirimKaldirildi { get; set; } = false;
        public int BolgeId { get; set; } = ProjeConstants.BOLGE_GENELMUDURLUK_INT;
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new BirimTanimService().GetById(id), typeof(T)); }
        public BirimTanim Select(int id) { Id = id; return new BirimTanimService().GetById(id); }
        public override int Save() { return new BirimTanimService().Save(this); }
        public override bool Update() { return new BirimTanimService().Update(this); }
        public override bool Delete() { return new BirimTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new BirimTanimService().GetAll(), typeof(List<T>)); }
        public List<BirimTanim> SelectByAmirId(int amirId) { return new BirimTanimService().GetByAmirId(amirId); }
        public List<BirimTanim> SelectByBirimKaldirildi(bool birimKaldirildiMi) { return new BirimTanimService().GetByBirimKaldirildi(birimKaldirildiMi); }
        public DataTable SelectAllReturnDataTable() { return new BirimTanimService().GetAllReturnDataTable(); }
        public List<BirimTanim> SelectByParentId(int parentId) { return new BirimTanimService().GetByParentId(parentId); }
        public BirimTanim SelectRoot() { return new BirimTanimService().GetRoot(); }
    }
}

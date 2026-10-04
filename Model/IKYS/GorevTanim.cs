using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using static Model.IKYS.Personel;

namespace Model.IKYS
{
    public class GorevTanim : ParentClass
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }
        public int HarcirahGrupId { get; set; }

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new GorevTanimService().GetById(id), typeof(T)); }
        public GorevTanim Select(int id) { Id = id; return new GorevTanimService().GetById(id); }
        public override int Save() { return new GorevTanimService().Save(this); }
        public override bool Update() { return new GorevTanimService().Update(this); }
        public override bool Delete() { return new GorevTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new GorevTanimService().GetAll(), typeof(List<T>)); }
        public GorevTanim SelectByPersonelId(int personelId) { return new GorevTanimService().GetByPersonelId(personelId); }
        public List<GorevTanim> SelectByBirimId(int birimId) { return new GorevTanimService().GetByBirimId(birimId); }
        public GorevTanim SelectByGorevId(int gorevId) { return new GorevTanimService().GetByGorevId(gorevId); }
        public DataTable SelectAllReturnDataTable(PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new GorevTanimService().GetAllReturnDataTable(personelTipi); }
    }
}

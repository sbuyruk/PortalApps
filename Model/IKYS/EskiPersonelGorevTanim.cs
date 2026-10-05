using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using static Model.IKYS.Personel;

namespace Model.IKYS
{
    public class EskiPersonelGorevTanim : ParentClass
    {
        public int BirimId { get; set; }
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string KisaAdi { get; set; }
        public bool Vekil { get; set; }
        public bool Aktif { get; set; }

        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new EskiPersonelGorevTanimService().GetById(id), typeof(T)); }
        public EskiPersonelGorevTanim Select(int id) { Id = id; return new EskiPersonelGorevTanimService().GetById(id); }
        public override int Save() { return new EskiPersonelGorevTanimService().Save(this); }
        public override bool Update() { return new EskiPersonelGorevTanimService().Update(this); }
        public override bool Delete() { return new EskiPersonelGorevTanimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new EskiPersonelGorevTanimService().GetAll(), typeof(List<T>)); }
        public EskiPersonelGorevTanim SelectByPersonelId(int personelId) { return new EskiPersonelGorevTanimService().GetByPersonelId(personelId); }
        public List<EskiPersonelGorevTanim> SelectByBirimId(int birimId) { return new EskiPersonelGorevTanimService().GetByBirimId(birimId); }
        public EskiPersonelGorevTanim SelectByGorevId(int gorevId) { return new EskiPersonelGorevTanimService().GetByGorevId(gorevId); }
        public DataTable SelectAllReturnDataTable(PersonelTipi personelTipi = PersonelTipi.Kadrolu) { return new EskiPersonelGorevTanimService().GetAllReturnDataTable(personelTipi); }
    }
}

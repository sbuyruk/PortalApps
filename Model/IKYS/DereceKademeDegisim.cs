using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class DereceKademeDegisim : ParentClass
    {
        public int PersonelId { get; set; }
        public string Degisim { get; set; }
        public DateTime DegisimTarihi { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new DereceKademeDegisimService().GetById(id), typeof(T)); }
        public DereceKademeDegisim Select(int id) { return new DereceKademeDegisimService().GetById(id); }
        public override int Save() { return new DereceKademeDegisimService().Save(this); }
        public override bool Update() { return new DereceKademeDegisimService().Update(this); }
        public override bool Delete() { return new DereceKademeDegisimService().Delete(this); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new DereceKademeDegisimService().GetAll(), typeof(List<T>)); }
        public DataTable SelectAllByPersonelIdReturnDT(int personelId) { return new DereceKademeDegisimService().GetAllByPersonelIdReturnDataTable(personelId); }
        public DereceKademeDegisim SelectByPersonelId(int personelId) { return new DereceKademeDegisimService().GetByPersonelId(personelId); }
    }
}

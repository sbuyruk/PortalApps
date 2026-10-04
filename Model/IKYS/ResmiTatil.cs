using Model.Services.IKYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.IKYS
{
    public class ResmiTatil : ParentClass
    {
        public int Gun { get; set; }
        public int Ay { get; set; }
        public int Yil { get; set; }
        public string Tatil { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime IlanTarihi { get; set; }
        public DateTime IptalTarihi { get; set; }
        public override T Select<T>(int id) { Id = id; return (T)Convert.ChangeType(new ResmiTatilService().GetById(id), typeof(T)); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new ResmiTatilService().GetAll(), typeof(List<T>)); }
        public DataTable SelectAllReturnDataTable() { return new ResmiTatilService().GetAllReturnDataTable(); }
        public override int Save() { return new ResmiTatilService().Save(this); }
        public override bool Update() { return new ResmiTatilService().Update(this); }
        public override bool Delete() { return new ResmiTatilService().Delete(this); }
        public List<ResmiTatil> SelectByTarih(DateTime basTar, DateTime bitTar) { return new ResmiTatilService().GetByTarih(basTar, bitTar); }
        public List<ResmiTatil> SelectBySonIkiYil() { return new ResmiTatilService().GetBySonIkiYil(); }
        public List<ResmiTatil> SelectByYil(int yil) { return new ResmiTatilService().GetByYil(yil); }
        public bool ResmiTatilMi(DateTime tarih) { return new ResmiTatilService().IsResmiTatil(tarih); }
        public string SelectAllReturnJson(DateTime basTar, DateTime bitTar) { return new ResmiTatilService().GetAllReturnJson(basTar, bitTar); }
    }
}

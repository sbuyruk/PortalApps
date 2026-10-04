using DAO.Ortak;
using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class TeminatIslem : ParentClass
    {
        public int KiraciId { get; set; }
        public int DosyaNo { get; set; }
        public string IslemTipi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public decimal IslemTutari { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public int OdemeId { get; set; }

        public override T Select<T>(int id) { return (T)Convert.ChangeType(new TeminatIslemService().GetById(id), typeof(T)); }
        public TeminatIslem Select(int id) { return new TeminatIslemService().GetById(id); }
        public override int Save() { return new TeminatIslemService().Save(this); }
        public override bool Update() { return new TeminatIslemService().Update(this); }
        public override bool Delete() { return new TeminatIslemService().Delete(this); }
        public bool DeleteBySozlesmeId(int id) { return new TeminatIslemService().DeleteBySozlesmeId(id); }
        public override List<T> SelectAll<T>() { return (List<T>)Convert.ChangeType(new TeminatIslemService().GetAll(), typeof(List<T>)); }
        public List<TeminatIslem> SelectBySozlesmeId(int id) { return new TeminatIslemService().GetBySozlesmeId(id); }
        public decimal SelectSumOdenenTutarByKiraciId(int id) { return new TeminatIslemService().GetSumPaidByKiraciId(id); }
        public DataTable SelectSumIslemTutariByKiraciIdGroupByIslemTipi(int id) { return new TeminatIslemService().GetSumByKiraciIdGroupByType(id); }
        public List<TeminatIslem> SelectByKiraciId(int id) { return new TeminatIslemService().GetByKiraciId(id); }
        public List<TeminatIslem> SelectByOdemeId(int id) { return new TeminatIslemService().GetByOdemeId(id); }

        // Transaction compatibility: these public SQL-producing methods are still consumed by OdemeAyristirmaWP.
        public string GetInsertSQL(string extId)
        {
            GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_INSERT);
            OlusturmaTarihi = DateTime.Now;
            return genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";
        }

        public string GetUpdateSQL(string extId)
        {
            GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_UPDATE);
            OlusturmaTarihi = DateTime.Now;
            return genericEntity.GetQuery(this, extId);
        }

        public string GetDeleteSQL(string extId)
        {
            GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_DELETE);
            OlusturmaTarihi = DateTime.Now;
            return genericEntity.GetQuery(this, extId);
        }
    }
}

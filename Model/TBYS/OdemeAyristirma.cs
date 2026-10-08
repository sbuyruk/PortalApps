using DocumentFormat.OpenXml.Office.CustomUI;
using Model.NBYS;
using Model.Ortak;
using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeAyristirma : EntityBase
    {
        public int KiraEkstreAktarmaId { get; set; }
        public int KiraciId { get; set; }
        public int SozlesmeId { get; set; }
        public int TeminatIslemId { get; set; }
        public int OdemeId { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public string OdemeSaati { get; set; }
        public int OdemeSebebiId { get; set; }
        public decimal Tutar { get; set; }
        public string DovizCinsi { get; set; }
        public string Aciklama { get; set; }
        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new OdemeAyristirmaService().GetById(id), typeof(T));
        }
        public OdemeAyristirma Select(int id)
        {
            return new OdemeAyristirmaService().GetById(id);
        }
        public int Save()
        {
            return new OdemeAyristirmaService().Save(this);
        }
        public bool Update()
        {
            return new OdemeAyristirmaService().Update(this);
        }
        public bool Delete()
        {
            return new OdemeAyristirmaService().Delete(this);
        }
        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new OdemeAyristirmaService().GetAll(), typeof(List<T>));
        }
        public  DataTable SelectByKiraEkstreAktarmaId(int kiraEkstreAktarmaId)
        {
            return new OdemeAyristirmaService().GetByKiraEkstreAktarmaId(kiraEkstreAktarmaId);
        }
    }
}

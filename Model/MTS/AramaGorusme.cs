using Model.Ortak;
using Model.Services.MTS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.MTS
{
    public class AramaGorusme : EntityBase
    {
        public int ArayanId { get; set; }
        public int FaaliyetId { get; set; }
        public DateTime Tarih { get; set; }
        public string GorusmeSekli { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public bool GorusmeSaglandi { get; set; }
        public bool RandevuIstendi { get; set; }

        public int Save()
        {
            return new AramaGorusmeService().Save(this);
        }

        public bool Update()
        {
            return new AramaGorusmeService().Update(this);
        }

        public bool Delete()
        {
            return new AramaGorusmeService().Delete(this);
        }

        public AramaGorusme Select(int id)
        {
            Id = id;
            return new AramaGorusmeService().GetById(id);
        }

        public AramaGorusme SelectByFaaliyetId(int faaliyetId)
        {
            return new AramaGorusmeService().GetByFaaliyetId(faaliyetId);
        }

        public T Select<T>(int id)
        {
            Id = id;
            return (T)Convert.ChangeType(new AramaGorusmeService().GetById(id), typeof(T));
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new AramaGorusmeService().GetAll(), typeof(List<T>));
        }

        public List<AramaGorusme> SelectAllByArayanIdReturnList(int arayanId)
        {
            return new AramaGorusmeService().GetByArayanId(arayanId);
        }

        public DataTable SelectAllReturnDT(int arayanId, string gorusmeSekli, DateTime basTar, DateTime bitTar)
        {
            return new AramaGorusmeService().GetByFilter(arayanId, gorusmeSekli, basTar, bitTar);
        }
    }
}

using Model.Ortak;
using Model.Services.Portal;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.Portal
{
    public class ToplantiParametre : EntityBase
    {
        public string Grup { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }

        public T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new ToplantiParametreService().GetById(id), typeof(T));
        }

        public ToplantiParametre Select(int id)
        {
            return new ToplantiParametreService().GetById(id);
        }

        public int Save()
        {
            return new ToplantiParametreService().Save(this);
        }

        public bool Update()
        {
            return new ToplantiParametreService().Update(this);
        }

        public bool Delete()
        {
            return new ToplantiParametreService().Delete(this);
        }

        public List<T> SelectAll<T>()
        {
            return (List<T>)Convert.ChangeType(new ToplantiParametreService().GetAll(), typeof(List<T>));
        }

        public List<ToplantiParametre> SelectByGrupReturnList(string g)
        {
            return new ToplantiParametreService().GetByGroup(g);
        }

        public List<ToplantiParametre> SelectSecilmemisByGrupReturnList(string g)
        {
            return new ToplantiParametreService().GetUnselected(g);
        }

        public List<ToplantiParametre> SelectSecilmisByGrupReturnList(string g)
        {
            return new ToplantiParametreService().GetSelected(g);
        }

        public DataTable SelectAllReturnDT()
        {
            return new ToplantiParametreService().GetAllData();
        }

        public string SelectAllReturnJson()
        {
            return new ToplantiParametre().ToJSON(new ToplantiParametreService().GetAllData());
        }

        public List<ToplantiParametre> SelectByGrupDeger(string g, string d)
        {
            return new ToplantiParametreService().GetByGroupValue(g, d);
        }
    }
}

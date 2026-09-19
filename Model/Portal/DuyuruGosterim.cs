using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    public class DuyuruGosterim : ParentClass
    {
        public int DuyuruId { get; set; }
        public DateTime GosterildigiTarih { get; set; }
        public string Baslik { get; set; }
        public string Metin { get; set; }
        public DateTime YayinBasTar { get; set; }
        public DateTime YayinBitTar { get; set; }
        public string Tekrar { get; set; }
        public string DuyuruAlicilari { get; set; }
        public string Resim { get; set; }
        public string Aciklama { get; set; }
        public bool Aktif { get; set; }
        public bool Popup { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM DuyuruGosterim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruGosterim> list = ToList<DuyuruGosterim>(dataTable);
            DuyuruGosterim duyuruGosterim = new DuyuruGosterim();
            duyuruGosterim = list.FirstOrDefault();
            return (T)Convert.ChangeType(duyuruGosterim, typeof(T));

        }
        public DuyuruGosterim Select(int id)
        {
            GenericEntity<DuyuruGosterim> genericEntity = new GenericEntity<DuyuruGosterim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruGosterim> list = ToList<DuyuruGosterim>(dataTable);
            DuyuruGosterim duyuruGosterim = new DuyuruGosterim();
            duyuruGosterim = list.FirstOrDefault();
            return duyuruGosterim;
        }

        public override int Save()
        {
            try
            {
                GenericEntity<DuyuruGosterim> genericEntity = new GenericEntity<DuyuruGosterim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<DuyuruGosterim> genericEntity = new GenericEntity<DuyuruGosterim>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    isSuccess = dao.Update2Db(query);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            GenericEntity<DuyuruGosterim> genericEntity = new GenericEntity<DuyuruGosterim>(ProjeConstants.SQL_DELETE);
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            bool isSuccess = dao.DeleteFromDb(query, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM DuyuruGosterim_Table
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruGosterim> list = ToList<DuyuruGosterim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DuyuruGosterim SelectByDuyuruId(int duyuruId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM DuyuruGosterim_Table
                WHERE DuyuruId={0}
                ORDER BY Id DESC
                ", duyuruId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruGosterim> list = ToList<DuyuruGosterim>(dataTable);
            DuyuruGosterim dg = list.FirstOrDefault();
            return dg;
        }

        public void SaveDuyuru(Duyuru duyuru)
        {
            DuyuruId = duyuru.Id;
            GosterildigiTarih = DateTime.Now;

            Baslik = duyuru.Baslik;
            Metin = duyuru.Metin;
            YayinBasTar = duyuru.YayinBasTar;
            YayinBitTar = duyuru.YayinBitTar;
            DuyuruAlicilari = duyuru.DuyuruAlicilari;
            Resim = duyuru.Resim;
            Tekrar = duyuru.Tekrar;
            Popup = duyuru.Popup;
            Aktif = duyuru.Aktif;
            Aciklama = duyuru.Aciklama;
            Olusturan = string.IsNullOrEmpty(duyuru.Degistiren) ? duyuru.Olusturan : duyuru.Degistiren;
            Save();
        }
    }
}

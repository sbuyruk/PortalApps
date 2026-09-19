using DAO.Ortak;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.Ortak;
using System.Data;
using TSKGV_Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class Duyuru : ParentClass
    {
        public string Baslik { get; set; }
        public string Metin { get; set; }
        public DateTime YayinBasTar { get; set; }
        public DateTime YayinBitTar { get; set; }
        public string Tekrar { get; set; }
        public string DuyuruAlicilari { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Duyuru_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);
            Duyuru duyuru = new Duyuru();
            duyuru = list.FirstOrDefault();
            return (T)Convert.ChangeType(duyuru, typeof(T));

        }
        public Duyuru Select (int id)
        {
            GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);
            Duyuru duyuru = new Duyuru();
            duyuru = list.FirstOrDefault();
            return duyuru;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_INSERT);
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
                    GenericEntity<Duyuru> genericEntity = new GenericEntity<Duyuru>(ProjeConstants.SQL_UPDATE);
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
            GenericEntity<Duyuru> genericEntity =
                new GenericEntity<Duyuru>(ProjeConstants.SQL_DELETE);
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            bool isSuccess = dao.DeleteFromDb(query, "");

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Duyuru_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Duyuru> list = ToList<Duyuru>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}

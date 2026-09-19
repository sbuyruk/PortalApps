using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    public class DuyuruOkuma : ParentClass
    {
        public int DuyuruId { get; set; }
        public int DuyuruGosterimId { get; set; }
        public int PersonelId { get; set; }
        public DateTime OkumaTarihi { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM DuyuruOkuma_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruOkuma> list = ToList<DuyuruOkuma>(dataTable);
            DuyuruOkuma duyuruGosterim = new DuyuruOkuma();
            duyuruGosterim = list.FirstOrDefault();
            return (T)Convert.ChangeType(duyuruGosterim, typeof(T));

        }
        public DuyuruOkuma Select(int id)
        {
            GenericEntity<DuyuruOkuma> genericEntity = new GenericEntity<DuyuruOkuma>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruOkuma> list = ToList<DuyuruOkuma>(dataTable);
            DuyuruOkuma duyuruGosterim = new DuyuruOkuma();
            duyuruGosterim = list.FirstOrDefault();
            return duyuruGosterim;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<DuyuruOkuma> genericEntity = new GenericEntity<DuyuruOkuma>(ProjeConstants.SQL_INSERT);
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
                    GenericEntity<DuyuruOkuma> genericEntity = new GenericEntity<DuyuruOkuma>(ProjeConstants.SQL_UPDATE);
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
            GenericEntity<DuyuruOkuma> genericEntity = new GenericEntity<DuyuruOkuma>(ProjeConstants.SQL_DELETE);
            SqlQuery query = genericEntity.GetQueryParametreli(this);

            bool isSuccess = dao.DeleteFromDb(query, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM DuyuruOkuma_Table
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<DuyuruOkuma> list = ToList<DuyuruOkuma>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public string SelectByDuyuruId(int duyuruId)
        {
            string json = string.Empty;
            string sqlString = string.Format(@"
                SELECT A.Id, B.Adi, B.Soyadi, A.OkumaTarihi
                FROM DuyuruOkuma_Table A
				INNER JOIN Personel_Table B ON B.Id=A.PersonelId
                WHERE DuyuruId={0}
                ORDER BY OkumaTarihi DESC
                ", duyuruId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            json = ToJSON(dataTable);
            return json;
        }
    }
}

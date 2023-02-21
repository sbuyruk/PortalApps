using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
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
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TeminatIslem_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);
            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = list.FirstOrDefault();
            return (T)Convert.ChangeType(teminatIslem, typeof(T));

        }
        public TeminatIslem Select(int id)
        {
            GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);
            TeminatIslem teminatIslem = new TeminatIslem();
            teminatIslem = list.FirstOrDefault();
            return teminatIslem;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
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
                    GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
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
            string sqlString = string.Format(@"DELETE 
                               FROM TeminatIslem_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<TeminatIslem> genericEntity = new GenericEntity<TeminatIslem>(ProjeConstants.SQL_DELETE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                DELETE TeminatIslem_Table 
                WHERE SozlesmeId={0}", sozlesmeId);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TeminatIslem_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<TeminatIslem> SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM TeminatIslem_Table
                WHERE SozlesmeId={0}
                ORDER BY TeminatIslemTarihi ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);
            return list;

        }
        public decimal SelectSumOdenenTutarByKiraciId(int kiraciId)
        {
            decimal toplam = 0;
            string sqlString = string.Format(@"
                SELECT SUM(IslemTutari) Toplam 
                FROM TeminatIslem_Table
                WHERE IslemTipi='Teminat Ödemesi'
	                AND KiraciId={0} ", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    toplam = row["Toplam"].ToString().ConvertToDecimal();
                }
            }
            return toplam;
        }
        public DataTable SelectSumIslemTutariByKiraciIdGroupByIslemTipi(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT SUM(IslemTutari) IslemToplami, IslemTipi 
                FROM TeminatIslem_Table
                WHERE KiraciId={0} 
                GROUP BY IslemTipi", kiraciId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public List<TeminatIslem> SelectByKiraciId(int kiracitId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM TeminatIslem_Table
                WHERE KiraciId={0}
                ORDER BY IslemTarihi DESC", kiracitId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);
            return list;

        }
        public List<TeminatIslem> SelectByOdemeId(int odemeId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM TeminatIslem_Table
                WHERE OdemeId={0}
                ORDER BY IslemTarihi DESC", odemeId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TeminatIslem> list = ToList<TeminatIslem>(dataTable);
            return list;

        }
    }
}

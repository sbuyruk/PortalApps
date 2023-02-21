using DocumentFormat.OpenXml.Office.CustomUI;
using Model.NBYS;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeAyristirma : ParentClass
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
        public override T Select<T>(int id)
        {
            //string sqlString = string.Format(@"SELECT *
            //                   FROM OdemeAyristirma_Table 
            //                   WHERE  Id={0}", id);
            GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeAyristirma> list = ToList<OdemeAyristirma>(dataTable);
            OdemeAyristirma teminatIslem = new OdemeAyristirma();
            teminatIslem = list.FirstOrDefault();
            return (T)Convert.ChangeType(teminatIslem, typeof(T));

        }
        public OdemeAyristirma Select(int id)
        {
            GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeAyristirma> list = ToList<OdemeAyristirma>(dataTable);
            OdemeAyristirma teminatIslem = new OdemeAyristirma();
            teminatIslem = list.FirstOrDefault();
            return teminatIslem;
        }
        public  int SaveAll(List<OdemeAyristirma> list)
        {
            int count = 0;
            try
            {
                foreach (var item in list)
                {

                    GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_INSERT);
                    OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(item);
                    int id = dao.Insert(sqlString);

                    item.Id = id;
                    count++;
                }
                return count;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_INSERT);
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
                    GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_UPDATE);
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
                               FROM OdemeAyristirma_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeAyristirma_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeAyristirma> list = ToList<OdemeAyristirma>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public  DataTable SelectByKiraEkstreAktarmaId(int kiraEkstreAktarmaId)
        {
            string sqlString = string.Format(
                @"
                SELECT A.KiraEkstreAktarmaId,A.KiraciId, A.OdemeTarihi,C.OdemeSebebi,A.Tutar,B.Adi+' '+B.Soyadi AdiSoyadi
                FROM OdemeAyristirma_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                INNER JOIN OdemeSebebiTanim_Table C ON C.Id=A.OdemeSebebiId
                WHERE KiraEkstreAktarmaId={0}
            ", kiraEkstreAktarmaId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
           

            return dataTable;
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_INSERT);
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
                GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_UPDATE);
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
                GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_DELETE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

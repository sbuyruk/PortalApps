using DAO.Ortak;
using System;
using System.Collections.Generic;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class SilinenKayit : EntityBase
    {
        public string TabloAdi { get; set; }
        public string SilinenKayitBilgisi { get; set; }
        public string Silen { get; set; }
        public DateTime SilinmeTarihi { get; set; }
        public string SilinmeSebebi { get; set; }
        public int Save()
        {
            try
            {
                GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_INSERT);
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

            //string sqlString = string.Format(@"INSERT INTO SilinenKayit_Table (TabloAdi,SilinenKayitBilgisi,Silen,SilinmeTarihi,SilinmeSebebi )
            //                   VALUES ({0},{1},{2},{3},{4})",
            //                    TabloAdi.ReturnQuotedValue(), SilinenKayitBilgisi.ReturnQuotedValue(), Silen.ReturnQuotedValue(), SilinmeTarihi.ReturnQuotedValue(),
            //                    SilinmeSebebi.ReturnQuotedValue());


            //int id = dao.Insert(sqlString);

            //this.Id = id;
            //return id;
        }
        public bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_UPDATE);
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
        public bool Delete()
        {
            throw new NotImplementedException();
        }
        public T Select<T>(int id)
        {
            throw new NotImplementedException();
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_DELETE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<T> SelectAll<T>()
        {
            throw new NotImplementedException();
        }


    }
}

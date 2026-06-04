using System;
using System.Collections.Generic;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class SilinenKayit : ParentClass
    {
        public string TabloAdi { get; set; }
        public string SilinenKayitBilgisi { get; set; }
        public string Silen { get; set; }
        public string SilinmeTarihi { get; set; }
        public string SilinmeSebebi { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

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
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<SilinenKayit> genericEntity = new GenericEntity<SilinenKayit>(ProjeConstants.SQL_UPDATE);
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
            throw new NotImplementedException();
        }
        public override T Select<T>(int id)
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
        public override List<T> SelectAll<T>()
        {
            throw new NotImplementedException();
        }


    }
}

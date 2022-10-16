using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class SMSAylikBagis : ParentClass
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int TurkcellSMSAdedi { get; set; }
        public int VodafoneSMSAdedi { get; set; }
        public int TurkTelekomSMSAdedi { get; set; }
        public decimal SMSTutari { get; set; }
        public string DovizCinsi { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }
        //Methods
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM SMSAylikBagis_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_UPDATE);
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
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SMSAylikBagis_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);
            SMSAylikBagis sMSBagis = new SMSAylikBagis();
            sMSBagis = list.FirstOrDefault();
            return (T)Convert.ChangeType(sMSBagis, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SMSAylikBagis_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_INSERT);
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
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_UPDATE);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<SMSAylikBagis> SelectByYilReturnList(int yil)
        {
            List<SMSAylikBagis> liste = new List<SMSAylikBagis>();
            string sqlString = string.Format(@"
                SELECT *
                FROM SMSAylikBagis_Table
                WHERE 
                    Yil = {0}
                ORDER BY Ay", yil);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);
            return list;
        }

    }
}

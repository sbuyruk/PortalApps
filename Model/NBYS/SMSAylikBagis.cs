using DAO.Ortak;
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
        public override int Save()
        {
            try
            {
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
                }
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
                if (this != null)
                {
                    SMSAylikBagis item = Select<SMSAylikBagis>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
                    }
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
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    SMSAylikBagis item = Select<SMSAylikBagis>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_SMSAYLIKBAGIS);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SMSAylikBagis_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);
            SMSAylikBagis sMSBagis = new SMSAylikBagis();
            sMSBagis = list.FirstOrDefault();
            return (T)Convert.ChangeType(sMSBagis, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SMSAylikBagis_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
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
                GenericEntity<SMSAylikBagis> genericEntity = new GenericEntity<SMSAylikBagis>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SMSAylikBagis> list = ToList<SMSAylikBagis>(dataTable);
            return list;
        }

    }
}

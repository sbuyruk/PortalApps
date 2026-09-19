using DAO.Ortak;
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
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OdemeAyristirma> list = ToList<OdemeAyristirma>(dataTable);
            OdemeAyristirma teminatIslem = new OdemeAyristirma();
            teminatIslem = list.FirstOrDefault();
            return teminatIslem;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
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
                    OdemeAyristirma item = Select<OdemeAyristirma>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
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
                    GenericEntity<OdemeAyristirma> genericEntity = new GenericEntity<OdemeAyristirma>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    OdemeAyristirma item = Select<OdemeAyristirma>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeAyristirma_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
           

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

                throw;
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

                throw;
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

                throw;
            }
        }
    }
}

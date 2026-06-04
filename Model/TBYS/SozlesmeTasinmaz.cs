using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class SozlesmeTasinmaz : ParentClass
    {
        public int SozlesmeId { get; set; }
        public int TasinmazId { get; set; }
        public int BolumId { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SozlesmeTasinmaz_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            SozlesmeTasinmaz sozlesmeTasinmaz = new SozlesmeTasinmaz();
            sozlesmeTasinmaz = list.FirstOrDefault();
            return (T)Convert.ChangeType(sozlesmeTasinmaz, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<SozlesmeTasinmaz> genericEntity = new GenericEntity<SozlesmeTasinmaz>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
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
                    SozlesmeTasinmaz item = Select<SozlesmeTasinmaz>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<SozlesmeTasinmaz> genericEntity = new GenericEntity<SozlesmeTasinmaz>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
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
                    GenericEntity<SozlesmeTasinmaz> genericEntity = new GenericEntity<SozlesmeTasinmaz>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    SozlesmeTasinmaz item = Select<SozlesmeTasinmaz>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"DELETE 
                               FROM SozlesmeTasinmaz_Table
                               WHERE SozlesmeId={0}", sozlesmeId);

            bool isDeleted = dao.DeleteFromDb(sqlString, this);
            if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_SOZLESMETASINMAZ);
            }
            return isDeleted;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SozlesmeTasinmaz_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public string SelectBySozlesmeIdReturnJson(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY B.Id) AS Sirano, B.Id, B.Id TasinmazId,
                    B.Adres,B.Adres +' '+ ISNULL(C.BolumNo,'')+' '+ B.Ilcesi+'/'+B.Ili AdresBolumNoIliIlcesi,
                    C.BolumNo,C.Id BolumId
                FROM SozlesmeTasinmaz_Table A 
				    LEFT JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    LEFT JOIN BagimsizBolum_Table C ON C.TasinmazId=B.Id AND C.Id=A.BolumId
                WHERE --B.EnvanterdeMi=1 AND 
                    A.SozlesmeId={0}", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectBySozlesmeIdReturnDataTable(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT ROW_NUMBER() OVER(ORDER BY B.Id) AS Sirano, B.Id, B.Id TasinmazId,
                    B.Adres,B.Adres +' '+ ISNULL(C.BolumNo,'')+' '+ B.Ilcesi+'/'+B.Ili AdresBolumNoIliIlcesi,
                    C.BolumNo,C.Id BolumId
                FROM SozlesmeTasinmaz_Table A 
				    LEFT JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    LEFT JOIN BagimsizBolum_Table C ON C.TasinmazId=B.Id AND C.Id=A.BolumId
                WHERE --B.EnvanterdeMi=1 AND 
                    A.SozlesmeId={0}", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }

            return dataTable;
        }

        public List<SozlesmeTasinmaz> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"SELECT * FROM SozlesmeTasinmaz_Table
                              WHERE TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
        public List<SozlesmeTasinmaz> SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"SELECT * FROM SozlesmeTasinmaz_Table
                              WHERE SozlesmeId={0}", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }

        public decimal SelectSumMetrekareBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT 
                    A.SozlesmeId, 
                    SUM(
                        CASE 
                            WHEN B.AltBolum = 1 THEN C.Metrekare 
                            WHEN B.AltBolum = 0 THEN B.Metrekare 
                            --WHEN B.KatMulkiyeti = 0 THEN C.Metrekare 
                            --WHEN B.KatMulkiyeti = 1 THEN B.Metrekare 
                            ELSE 0
                        END
                    ) AS Metrekare
                FROM SozlesmeTasinmaz_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                LEFT JOIN BagimsizBolum_Table C ON C.Id = A.BolumId
                WHERE A.SozlesmeId = {0}
                GROUP BY A.SozlesmeId", sozlesmeId.ReturnQuotedValue());
            //string sqlString = string.Format(
            //    @"SELECT SozlesmeId, SUM(Metrekare) Metrekare
            //        FROM SozlesmeTasinmaz_Table A
            //        INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
            //    WHERE SozlesmeId={0}
            //    GROUP BY SozlesmeId ", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            decimal metrekare = 0;
            if (dataTable != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    metrekare = row["Metrekare"].ToString().ConvertToDecimal();
                }
            }
            
            return metrekare;
        }

        public DataTable SelectBySozlesmeIdReturnDT(int sozlesmeId)
        {
            string sqlString = string.Format(@"
                SELECT A.SozlesmeId,A.TasinmazId,A.BolumId, B.Adres, C.BolumNo, '@'+ B.Adres+' '+ISNULL(C.BolumNo,'')+' '+ B.Ili+'/'+B.Ilcesi TasinmazAdresi
                FROM SozlesmeTasinmaz_Table A
                    INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                    LEFT JOIN BagimsizBolum_Table C ON C.Id=A.BolumId
                WHERE SozlesmeId={0}
                ORDER BY A.SozlesmeId,A.TasinmazId,A.BolumId", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public List<SozlesmeTasinmaz> SelectBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM SozlesmeTasinmaz_Table
                WHERE SozlesmeId={0} AND TasinmazId={1} AND BolumId={2} ", sozlesmeId.ReturnQuotedValue(), tasinmazId.ReturnQuotedValue(), bolumId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
        public List<SozlesmeTasinmaz> SelectByBolumId(int bolumId)
        {
            string sqlString = string.Format(@"SELECT * FROM SozlesmeTasinmaz_Table
                              WHERE SozlesmeId={0}", bolumId.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
    }
}

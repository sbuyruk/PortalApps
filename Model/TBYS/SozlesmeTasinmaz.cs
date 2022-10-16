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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }

            //string sqlString = string.Format(@"
            //                                INSERT INTO SozlesmeTasinmaz_Table 
            //                                    (SozlesmeId, TasinmazId, BolumId,Olusturan, OlusturmaTarihi)
            //                                VALUES ({0},{1},{2},{3},{4})",
            //                                SozlesmeId.ReturnQuotedValue(), TasinmazId.ReturnQuotedValue(), BolumId.ReturnQuotedValue(),
            //                                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());


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
                    GenericEntity<SozlesmeTasinmaz> genericEntity = new GenericEntity<SozlesmeTasinmaz>(ProjeConstants.SQL_UPDATE);
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
            //bool isSuccess = false;
            //if (Id != 0)
            //{
            //    string sqlString = string.Format(@"
            //                            UPDATE SozlesmeTasinmaz_Table 
            //                            SET SozlesmeId={0}, TasinmazId={1}, BolumId={2},Degistiren={3},DegistirmeTarihi={4}
            //                            WHERE Id={5}",
            //                                SozlesmeId.ReturnQuotedValue(), TasinmazId.ReturnQuotedValue(), BolumId.ReturnQuotedValue(),
            //                                Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

            //    isSuccess = dao.Update2Db(sqlString);
            //}
            //return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM SozlesmeTasinmaz_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public bool DeleteBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"DELETE 
                               FROM SozlesmeTasinmaz_Table
                               WHERE SozlesmeId={0}", sozlesmeId);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM SozlesmeTasinmaz_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
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
                dataTable = dao.selectFromDb(sqlString, "");
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
                dataTable = dao.selectFromDb(sqlString, "");
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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
        public List<SozlesmeTasinmaz> SelectBySozlesmeId(int sozlesmeId)
        {
            string sqlString = string.Format(@"SELECT * FROM SozlesmeTasinmaz_Table
                              WHERE SozlesmeId={0}", sozlesmeId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
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
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return dataTable;
        }
        public List<SozlesmeTasinmaz> SelectBySozlesmeIdTasinmazId(int sozlesmeId, int tasinmazId, int bolumId)
        {
            string sqlString = string.Format(@"
                SELECT * FROM SozlesmeTasinmaz_Table
                WHERE SozlesmeId={0} AND TasinmazId={1} AND BolumId={2} ", sozlesmeId.ReturnQuotedValue(), tasinmazId.ReturnQuotedValue(), bolumId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
        public List<SozlesmeTasinmaz> SelectByBolumId(int bolumId)
        {
            string sqlString = string.Format(@"SELECT * FROM SozlesmeTasinmaz_Table
                              WHERE SozlesmeId={0}", bolumId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<SozlesmeTasinmaz> list = ToList<SozlesmeTasinmaz>(dataTable);
            return list;
        }
    }
}

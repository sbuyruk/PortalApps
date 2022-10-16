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
    public class Onarim : ParentClass
    {
        public int TasinmazId { get; set; }
        public string YapilanIs { get; set; }
        public string HarcamaUsulu { get; set; }
        public DateTime OnayTarihi { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Onarim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            Onarim onarim = new Onarim();
            onarim = list.FirstOrDefault();
            return (T)Convert.ChangeType(onarim, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<Onarim> genericEntity = new GenericEntity<Onarim>(ProjeConstants.SQL_INSERT);
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
            //string sqlString = string.Format(@"
            //                                INSERT INTO Onarim_Table 
            //                                    (TasinmazId,YapilanIs,HarcamaUsulu,OnayTarihi,Tutar,Aciklama,Olusturan, OlusturmaTarihi)
            //                                VALUES ({0},{1},{2},{3},{4},{5},{6},{7})",
            //                                TasinmazId.ReturnQuotedValue(), YapilanIs.ReturnQuotedValue(), HarcamaUsulu.ReturnQuotedValue(),
            //                                OnayTarihi.ReturnTRDateFormat(), Tutar.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
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
                    GenericEntity<Onarim> genericEntity = new GenericEntity<Onarim>(ProjeConstants.SQL_UPDATE);
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
            //                            UPDATE Onarim_Table 
            //                            SET TasinmazId={0},YapilanIs={1},HarcamaUsulu={2},OnayTarihi={3},Tutar={4},Aciklama={5},Degistiren={6},DegistirmeTarihi={7}
            //                            WHERE Id={8}",
            //                                TasinmazId.ReturnQuotedValue(), YapilanIs.ReturnQuotedValue(), HarcamaUsulu.ReturnQuotedValue(),
            //                                OnayTarihi.ReturnTRDateFormat(), Tutar.ReturnQuotedValue(), Aciklama.ReturnQuotedValue(),
            //                                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

            //    isSuccess = dao.Update2Db(sqlString);
            //}
            //return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM Onarim_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Onarim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Onarim> SelectByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT *
                    FROM Onarim_Table A
                WHERE TasinmazId={0}
                ORDER BY TasinmazId", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                return list;
            }
            else
            {
                return null;
            }

        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = SelectAllString();
            DataTable dataTable = null;
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
        private string SelectAllString()
        {
            string sqlString = string.Format(@"
                SELECT A.Id OnarimId, A.TasinmazId,A.YapilanIs,A.HarcamaUsulu,A.OnayTarihi,
                    Convert(nvarchar,replace (A.Tutar,'.',',')) as Tutar, A.Aciklama,
                    B.Adres, B.Ili,B.Ilcesi, B.Ili+' '+B.Ilcesi IliIlcesi
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id=A.TasinmazId
                ORDER BY OnayTarihi DESC  
                            ");
            return sqlString;
        }
        public List<Onarim> SelectByOnarimId(int onarimId)
        {
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                              WHERE Id={0}", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            return list;
        }
        public Onarim SelectNext(int onarimId)
        {
            Onarim onarim = new Onarim();
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                                            WHERE Id > {0}
                                            ORDER BY Id ", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                onarim = list.FirstOrDefault();
            }
            else
            {
                onarim = SelectMin();

            }
            return onarim;
        }
        public Onarim SelectPrev(int onarimId)
        {
            Onarim onarim = new Onarim();
            string sqlString = string.Format(@"SELECT * FROM Onarim_Table
                                            WHERE Id < {0}
                                            ORDER BY Id ", onarimId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<Onarim> list = ToList<Onarim>(dataTable);
                onarim = list.FirstOrDefault();
            }
            else
            {
                onarim = SelectMax();

            }
            return onarim;
        }
        public Onarim SelectMax()
        {
            string sqlString = string.Format(@"SELECT MAX(Id) Id  FROM Onarim_Table ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int onarimId = row["Id"].ConvertToInt();
                Onarim onarim = new Onarim();
                onarim = onarim.Select<Onarim>(onarimId);
                return onarim;
            }
            else
            {
                return null;
            }
        }

        public List<Onarim> SelectOnarimByTasinmazId(int tasinmazId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id, A.TasinmazId, A.YapilanIs, convert(decimal(10, 2), A.Tutar) Tutar,
                    A.OnayTarihi,  A.Aciklama,A.HarcamaUsulu, B.Ili, B.Ilcesi,B.Adres
                FROM Onarim_Table A
                INNER JOIN Tasinmaz_Table B ON B.Id = A.TasinmazId
                WHERE A.TasinmazId={0}", tasinmazId.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Onarim> list = ToList<Onarim>(dataTable);
            return list;
        }

        public Onarim SelectMin()
        {
            string sqlString = string.Format(@"SELECT MIN(Id) Id  FROM Onarim_Table ");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int onarimId = row["Id"].ConvertToInt();
                Onarim onarim = new Onarim();
                onarim = onarim.Select<Onarim>(onarimId);
                return onarim;
            }
            else
            {
                return null;
            }
        }
    }
}

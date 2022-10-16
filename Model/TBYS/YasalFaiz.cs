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
    public class YasalFaiz : ParentClass
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public decimal FaizOrani { get; set; }
        public decimal Tufe { get; set; }
        public decimal Ufe { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM YasalFaiz_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            YasalFaiz yasalFaiz = new YasalFaiz();
            yasalFaiz = list.FirstOrDefault();
            return (T)Convert.ChangeType(yasalFaiz, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<YasalFaiz> genericEntity = new GenericEntity<YasalFaiz>(ProjeConstants.SQL_INSERT);
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
            //                                INSERT INTO YasalFaiz_Table 
            //                                    (Yil,Ay,AyAdi,FaizOrani,Aciklama,Olusturan, OlusturmaTarihi)
            //                                VALUES ({0},{1},{2},{3},{4},{5},{6})",
            //                                Yil.ReturnQuotedValue(), Ay.ReturnQuotedValue(), AyAdi.ReturnQuotedValue(),
            //                                FaizOrani.ConvertDecimalToString(), Aciklama.ReturnQuotedValue(),
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
                    GenericEntity<YasalFaiz> genericEntity = new GenericEntity<YasalFaiz>(ProjeConstants.SQL_UPDATE);
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
            //                            UPDATE YasalFaiz_Table 
            //                            SET Yil={0},Ay={1},AyAdi={2},FaizOrani={3},Aciklama={4},Degistiren={5},DegistirmeTarihi={6}
            //                            WHERE Id={7}",
            //                                Yil.ReturnQuotedValue(), Ay.ReturnQuotedValue(), AyAdi.ReturnQuotedValue(),
            //                                FaizOrani.ConvertDecimalToString(), Aciklama.ReturnQuotedValue(),
            //                                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

            //    isSuccess = dao.Update2Db(sqlString);
            //}
            //return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM YasalFaiz_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM YasalFaiz_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<YasalFaiz> SelectByYil(int yil)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table 
                WHERE Yil={0}", yil.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            return list;
        }
        public YasalFaiz SelectByYilAy(int yil, int ay)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table 
                WHERE Yil={0} AND Ay={1}", yil.ReturnQuotedValue(), ay.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
            YasalFaiz yasalFaiz = new YasalFaiz();
            yasalFaiz = list.FirstOrDefault();
            return yasalFaiz;
        }
        public decimal SelectSonFaizOrani()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  FaizOrani IS NOT Null AND FaizOrani > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.FaizOrani;
            }

            return value;
        }
        public decimal SelectSonTufe()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  Tufe IS NOT Null AND Tufe > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.Tufe;
            }

            return value;
        }
        public decimal SelectSonUfe()
        {
            decimal value = 0;
            string sqlString = string.Format(@"
                SELECT *
                FROM YasalFaiz_Table
                WHERE  Ufe IS NOT Null AND Ufe > 0 
                ORDER BY Yil DESC, Ay DESC");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<YasalFaiz> list = ToList<YasalFaiz>(dataTable);
                YasalFaiz yasalFaiz = new YasalFaiz();
                yasalFaiz = list.FirstOrDefault();
                value = yasalFaiz.Ufe;
            }

            return value;
        }
    }
}
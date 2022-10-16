using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.TBYS
{
    [Serializable]
    public class TestDb : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TestDb_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TestDb> list = ToList<TestDb>(dataTable);
            TestDb testDb = new TestDb();
            testDb = list.FirstOrDefault();
            return (T)Convert.ChangeType(testDb, typeof(T));

        }
        public override int Save()
        {

            GenericEntity<TestDb> genericEntity = new GenericEntity<TestDb>(3);
            string sqlString = genericEntity.GetQuery(this);
            //string sqlString = string.Format(@"
            //                                INSERT INTO TestDb_Table 
            //                                    (Adi, Soyadi,
            //                                    Olusturan, OlusturmaTarihi)
            //                                VALUES ({0},{1},{2},{3})",
            //                                Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(),
            //                                Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());

            int id = dao.Insert(sqlString);

            this.Id = id;
            return id;

        }
        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = string.Format(@"
                    UPDATE TestDb_Table 
                    SET Adi={0}, Soyadi={1},
                        Degistiren={2},DegistirmeTarihi={3}
                    WHERE Id={4}",
                    Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(),
                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM TestDb_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TestDb_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TestDb> list = ToList<TestDb>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        //public List<TestDb> SelectBySozlesmeIdTestDbPlaniIdAyYil(int sozlesmeId, int testDbPlaniId,int ay,int yil)
        //{
        //    string sqlString = string.Format(@"
        //        SELECT * FROM TestDb_Table
        //        WHERE SozlesmeId={0} AND TestDbPlaniId={1} AND MONTH(TestDbTarihi)={2} AND YEAR(TestDbTarihi)={3}
        //        ORDER BY TestDbTarihi ", sozlesmeId.ReturnQuotedValue(), testDbPlaniId,ay,yil);
        //    DataTable dataTable = dao.selectFromDb(sqlString, "");
        //    List<TestDb> list = ToList<TestDb>(dataTable);
        //    return list;

        //}
        //public decimal SelectSumBySozlesmeIdTestDbPlaniId(int sozlesmeId, int testDbPlaniId)
        //{
        //    decimal toplam = 0;
        //    string sqlString = string.Format(@"
        //        SELECT SUM(OdenenTutar) Toplam FROM TestDb_Table
        //        WHERE SozlesmeId={0} 
        //            AND TestDbPlaniId={1} 
        //        ", sozlesmeId.ReturnQuotedValue(), testDbPlaniId.ReturnQuotedValue());
        //    DataTable dataTable = dao.selectFromDb(sqlString, "");
        //    if (dataTable != null)
        //    {
        //        if (dataTable.Rows.Count > 0)
        //        {
        //            DataRow row = dataTable.Rows[0];
        //            toplam = row["Toplam"].ToString().ConvertToDecimal();
        //        }
        //    }

        //    return toplam;

        //}
        //public TestDb TestDbyiKaydetTestDbPlaniniGuncelle(KiraSozlesme kiraSozlesme, TestDbPlani testDbPlani, DateTime testDbTarihi, decimal odenenTutar, string aciklama,string kullanici)
        //{
        //    bool kaydedildiMi = false;

        //    TestDb testDb = null;

        //    try
        //    {
        //        dao.StartTransaction();
        //        //testDbyi yap
        //        testDb = new TestDb();
        //        testDb.SozlesmeId = kiraSozlesme.Id;
        //        testDb.KiraciId = kiraSozlesme.KiraciId;
        //        testDb.TestDbPlaniId = testDbPlani.Id;
        //        testDb.TestDbTarihi = testDbTarihi;
        //        testDb.OdenenTutar = odenenTutar;
        //        testDb.Aciklama = aciklama;
        //        testDb.Id = testDb.Save();
        //        if (testDb.Id > 0)
        //        {
        //            // toplamı bul
        //            TestDb testDbDao = new TestDb();
        //            decimal toplamOdenen = testDbDao.SelectSumBySozlesmeIdTestDbPlaniId(kiraSozlesme.Id, testDbPlani.Id);
        //            testDbPlani.OdenenTutar = toplamOdenen;
        //            testDbPlani.Aciklama += aciklama + System.Environment.NewLine;
        //            testDbPlani.Degistiren = kullanici;
        //            kaydedildiMi = testDbPlani.Update();
        //        }
        //        dao.EndTransaction();
        //    }
        //    catch (Exception exception1)
        //    {
        //        ExceptionHelper exHelper = new ExceptionHelper();
        //        Exception exception2 = new Exception("Ödeme Kaydedilemedi");
        //        exHelper.Exceptions.Add(exception2);
        //        exHelper.Exceptions.Add(exception1);
        //        exHelper.PublishException();
        //    }
        //    return testDb;
        //}
        //public bool TestDbyiSilTestDbPlaniniGuncelle(int testDbId, string aciklama, int testDbPlaniId,string kullanici)
        //{
        //    bool silindiMi = false;
        //    try
        //    {
        //        IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
        //        TestDb testDb = new TestDb();
        //        testDb = testDb.Select<TestDb>(testDbId);
        //        if (testDb != null)
        //        {
        //            silindiMi = testDb.Delete();
        //        }
        //        if (silindiMi)
        //        {
        //            TestDbPlani testDbPlani = new TestDbPlani();
        //            testDbPlani = testDbPlani.Select<TestDbPlani>(testDbPlaniId);
        //            TestDb testDbDao = new TestDb();
        //            decimal toplamOdenen = testDbDao.SelectSumBySozlesmeIdTestDbPlaniId(testDbPlani.SozlesmeId, testDbPlani.Id);
        //            testDbPlani.OdenenTutar = toplamOdenen;
        //            testDbPlani.Aciklama += aciklama + System.Environment.NewLine +
        //                " *" + testDb.TestDbTarihi.ConvertToDatetimeEmptyIfNull() + " tarihli " + testDb.OdenenTutar.ToString("N", culturInfo) + " ödeme silindi." + System.Environment.NewLine;
        //            testDbPlani.Degistiren = kullanici;
        //            testDbPlani.Update();
        //        }

        //    }
        //    catch (Exception exception1)
        //    {

        //        ExceptionHelper exHelper = new ExceptionHelper();
        //        Exception exception2 = new Exception("Ödeme Silinemedi");
        //        exHelper.Exceptions.Add(exception2);
        //        exHelper.Exceptions.Add(exception1);
        //        exHelper.PublishException();
        //    }
        //    return silindiMi;
        //}
        //public TestDb TestDbyiVeTestDbPlaniniGuncelle(int testDbId, DateTime testDbTarihi, decimal odenenTutar, string aciklama, int testDbPlaniId, string kullanici)
        //{
        //    TestDb testDb = null;
        //    try
        //    {
        //        testDb = new TestDb();
        //        bool guncellendiMi = false;

        //        testDb = testDb.Select<TestDb>(testDbId.ConvertToInt());
        //        if (testDb != null)
        //        {
        //            testDb.TestDbTarihi = testDbTarihi;
        //            testDb.OdenenTutar = odenenTutar;
        //            testDb.Aciklama = aciklama;
        //            testDb.Degistiren = kullanici; ;
        //            guncellendiMi = testDb.Update();
        //        }
        //        if (guncellendiMi)
        //        {
        //            TestDbPlani testDbPlani = new TestDbPlani();
        //            testDbPlani = testDbPlani.Select<TestDbPlani>(testDbPlaniId);
        //            TestDb testDbDao = new TestDb();
        //            decimal toplamOdenen = testDbDao.SelectSumBySozlesmeIdTestDbPlaniId(testDbPlani.SozlesmeId, testDbPlani.Id);
        //            testDbPlani.OdenenTutar = toplamOdenen;
        //            testDbPlani.Aciklama += aciklama + System.Environment.NewLine;
        //            testDbPlani.Degistiren = kullanici;
        //            testDbPlani.Update();
        //        }

        //    }
        //    catch (Exception exception1)
        //    {

        //        ExceptionHelper exHelper = new ExceptionHelper();
        //        Exception exception2 = new Exception("Ödeme Kaydedilemedi");
        //        exHelper.Exceptions.Add(exception2);
        //        exHelper.Exceptions.Add(exception1);
        //        exHelper.PublishException();
        //    }
        //    return testDb;
        //}
    }
}

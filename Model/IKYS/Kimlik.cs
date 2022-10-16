
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Kimlik : ParentClass
    {
        public int PersonelId { get; set; }
        public string TCKimlikNo { get; set; }
        public string AnneAdi { get; set; }
        public string BabaAdi { get; set; }
        public string DogumYeri { get; set; }
        public DateTime DogumTar { get; set; }
        public string MedeniHali { get; set; }
        public DateTime EvlilikTar { get; set; }
        public string Cinsiyet { get; set; }
        public string EskiSoyadi { get; set; }
        public string KanGrubu { get; set; }
        public bool DogumGunuKutlama { get; set; }
        public bool EvlilikKutlama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Kimlik> list = ToList<Kimlik>(dataTable);
            Kimlik kimlik = new Kimlik();
            kimlik = list.FirstOrDefault();
            return (T)Convert.ChangeType(kimlik, typeof(T));
        }
        public override int Save()
        {
            try
            {
                GenericEntity<Kimlik> genericEntity = new GenericEntity<Kimlik>(ProjeConstants.SQL_INSERT);
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

            //string sqlString = saveSQL();
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
                    GenericEntity<Kimlik> genericEntity = new GenericEntity<Kimlik>(ProjeConstants.SQL_UPDATE);
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
            //    string sqlString = UpdateSQL();
            //    isSuccess = dao.Update2Db(sqlString);
            //}
            //return isSuccess;
        }
        public override bool Delete()
        {
            string sqlString = DeleteSQL();

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public Kimlik Select(int id)
        {
            GenericEntity<Kimlik> genericEntity = new GenericEntity<Kimlik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Kimlik> list = ToList<Kimlik>(dataTable);
            Kimlik kimlik = new Kimlik();
            kimlik = list.FirstOrDefault();
            return kimlik;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Kimlik_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Kimlik> list = ToList<Kimlik>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {

            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Kimlik_Table 
                                        (PersonelId, TCKimlikNo, BabaAdi, AnneAdi, DogumYeri, DogumTar, 
                                        MedeniHali, EvlilikTar, Cinsiyet,EskiSoyadi,KanGrubu,
                                        Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}) ",
                                        PersonelId, TCKimlikNo.ReturnQuotedValue(), BabaAdi.ReturnQuotedValue(), AnneAdi.ReturnQuotedValue(),
                                        DogumYeri.ReturnQuotedValue(), DogumTar.ReturnTRDateFormat(), MedeniHali.ReturnQuotedValue(),
                                        EvlilikTar.ReturnTRDateFormat(), Cinsiyet.ReturnQuotedValue(), EskiSoyadi.ReturnQuotedValue(), KanGrubu.ReturnQuotedValue(),
                                        Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Kimlik_Table 
                                    SET PersonelId = {0}, TCKimlikNo = {1}, BabaAdi = {2}, AnneAdi = {3}, DogumYeri = {4}, DogumTar = {5}, 
                                        MedeniHali = {6},EvlilikTar = {7},Cinsiyet = {8}, EskiSoyadi = {9}, KanGrubu = {10},
                                        Degistiren={11},DegistirmeTarihi={12}
                                    WHERE Id= {13}",
                                    PersonelId, TCKimlikNo.ReturnQuotedValue(), BabaAdi.ReturnQuotedValue(), AnneAdi.ReturnQuotedValue(),
                                    DogumYeri.ReturnQuotedValue(), DogumTar.ReturnTRDateFormat(), MedeniHali.ReturnQuotedValue(),
                                    EvlilikTar.ReturnTRDateFormat(), Cinsiyet.ReturnQuotedValue(), EskiSoyadi.ReturnQuotedValue(), KanGrubu.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        public Kimlik SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Kimlik> list = ToList<Kimlik>(dataTable);
            Kimlik kimlik = list.FirstOrDefault();
            return (kimlik);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Kimlik_Table  
                    WHERE PersonelId={0}
                    ORDER BY Id", pId);
            return sqlstr;
        }
        public Kimlik SelectByTCKimlikNo(string kimlikNo)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Kimlik_Table  
                    WHERE TCKimlikNo={0}
                    ORDER BY Id", kimlikNo.ReturnQuotedValue());
            DataTable dataTable = dao.selectFromDb(sqlstr, "");
            List<Kimlik> list = ToList<Kimlik>(dataTable);
            Kimlik kimlik = list.FirstOrDefault();
            return (kimlik);
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Kimlik_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Kimlik_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public DataTable SelectAllFromKIMLIK()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KIMLIK_BILGILERI");
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            return (dataTable);
        }
    }
}

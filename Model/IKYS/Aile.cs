
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Aile : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TcKimlikNo { get; set; }
        public int YakinlikDerecesi { get; set; }
        public DateTime DogumTar { get; set; }
        public string Tahsil { get; set; }
        public string Okul { get; set; }
        public string Telefon { get; set; }
        public int Meslek { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile aile = new Aile();
            aile = list.FirstOrDefault();
            return (T)Convert.ChangeType(aile, typeof(T));
        }

        public override int Save()
        {
            string sqlString = saveSQL();
            int id = dao.Insert(sqlString);
            this.Id = id;
            return id;

        }

        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = UpdateSQL();
                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }

        public override bool Delete()
        {
            string sqlString = DeleteSQL();
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
        public Aile Select(int id)
        {
            GenericEntity<Aile> genericEntity = new GenericEntity<Aile>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile kimlik = new Aile();
            kimlik = list.FirstOrDefault();
            return kimlik;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Aile_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectAllFromAILE_BILGILERI()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM AILE_BILGILERI");

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return (dataTable);
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO Aile_Table 
                                        (PersonelId, Adi, Soyadi, TcKimlikNo,YakinlikDerecesi,DogumTar,Tahsil,
                                         Okul,Telefon,Meslek,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}) ",
                                    PersonelId, Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(), TcKimlikNo.ReturnQuotedValue(),
                                    YakinlikDerecesi.ReturnQuotedValue(), DogumTar.ReturnTRDateFormat(), Tahsil.ReturnQuotedValue(),
                                    Okul.ReturnQuotedValue(), Telefon.ReturnQuotedValue(), Meslek.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());

            return InsertSQL;
        }
        private string UpdateSQL()
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE Aile_Table 
                                    SET PersonelId = {0},Adi={1}, Soyadi={2}, TcKimlikNo={3}, YakinlikDerecesi={4},
                                        DogumTar={5}, Tahsil={6}, Okul={7}, Telefon={8},Meslek={9},Degistiren={10},DegistirmeTarihi={11}
                                        WHERE Id= {12}",
                                        PersonelId.ReturnQuotedValue(), Adi.ReturnQuotedValue(), Soyadi.ReturnQuotedValue(),
                                        TcKimlikNo.ReturnQuotedValue(), YakinlikDerecesi.ReturnQuotedValue(), DogumTar.ReturnTRDateFormat(),
                                        Tahsil.ReturnQuotedValue(), Okul.ReturnQuotedValue(), Telefon.ReturnQuotedValue(),
                                        Meslek.ReturnQuotedValue(), Degistiren.ReturnQuotedValue(), DateTime.Now.ConvertToDatetime(culture).ReturnTRDateFormat(), Id);

            return sqlSQL;
        }
        public List<Aile> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);

            return (list);
        }
        public Aile SelectEnGencCocukByPersonelId(int personelId)
        {
            string sqlString = string.Format(@" 
                    SELECT * FROM Aile_Table  
                    WHERE PersonelId={0} AND YakinlikDerecesi={1}
                    ORDER BY DogumTar Desc", personelId, ProjeConstants.PER_YAKINLIKDERECESI_COCUK_INT);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Aile> list = ToList<Aile>(dataTable);
            Aile aile = list.FirstOrDefault();
            return aile;
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM Aile_Table  
                    WHERE PersonelId={0}
                    ORDER BY YakinlikDerecesi", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM Aile_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM Aile_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
    }
}

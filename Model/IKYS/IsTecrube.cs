
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.IKYS
{
    public class IsTecrube : ParentClass
    {
        public int PersonelId { get; set; }
        public string Isyeri { get; set; }
        public string Gorevi { get; set; }
        public DateTime BasTar { get; set; }
        public DateTime BitTar { get; set; }
        public string Adres { get; set; }


        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsTecrube> list = ToList<IsTecrube>(dataTable);
            IsTecrube isyeri = new IsTecrube();
            isyeri = list.FirstOrDefault();
            return (T)Convert.ChangeType(isyeri, typeof(T));
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

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM IsTecrube_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsTecrube> list = ToList<IsTecrube>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IsTecrube_Table 
                                        (PersonelId, Isyeri,Gorevi, Adres,BasTar,BitTar, Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7}) ",
                                        PersonelId.ReturnQuotedValue(), Isyeri.ReturnQuotedValue(), Gorevi.ReturnQuotedValue(),
                                        Adres.ReturnQuotedValue(), BasTar.ReturnTRDateFormat(), BitTar.ReturnTRDateFormat(), Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE IsTecrube_Table 
                                    SET PersonelId = {0},Isyeri={2}, Gorevi={3}, Adres={4}, BasTar={5},BitTar={1}, 
                                        Degistiren={6},DegistirmeTarihi={7}
                                        WHERE Id= {8}", PersonelId.ReturnQuotedValue(), Isyeri.ReturnQuotedValue(),
                                            Gorevi.ReturnQuotedValue(), Adres.ReturnQuotedValue(), BasTar.ReturnTRDateFormat(), BitTar.ReturnTRDateFormat(),
                                            Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IsTecrube_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IsTecrube_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        public List<IsTecrube> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IsTecrube> list = ToList<IsTecrube>(dataTable);

            return (list);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IsTecrube_Table  
                    WHERE PersonelId={0}
                    ORDER BY BasTar DESC", pId);
            return sqlstr;
        }

    }
}

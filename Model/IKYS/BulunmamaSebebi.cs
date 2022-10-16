using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.IKYS
{
    public class BulunmamaSebebi : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BulunmamaSebebi> list = ToList<BulunmamaSebebi>(dataTable);
            BulunmamaSebebi bulunmamaSebebi = new BulunmamaSebebi();
            bulunmamaSebebi = list.FirstOrDefault();
            return (T)Convert.ChangeType(bulunmamaSebebi, typeof(T));
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
                               FROM BulunmamaSebebi_Table ORDER BY Id");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BulunmamaSebebi> list = ToList<BulunmamaSebebi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO BulunmamaSebebi_Table 
                                        (Adi,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2}) ",
                                    Adi.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE BulunmamaSebebi_Table 
                                    SET Adi = {0},Degistiren={1}, DegistirmeTarihi={2}
                                    WHERE Id= {3}", Adi.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM BulunmamaSebebi_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM BulunmamaSebebi_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
    }
}

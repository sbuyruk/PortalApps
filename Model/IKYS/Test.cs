using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.IKYS
{
    public class Test : ParentClass
    {
        public string Adi { get; set; }

        public override bool Delete()
        {
            string sqlString = string.Format(@"DELETE 
                               FROM Test_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }

        public override int Save()
        {
            string sqlString = string.Format(@"INSERT INTO Test_Table
                              (Adi,Olusturan,OlusturmaTarihi)
                               VALUES ({0},{1},{2})",
                           Adi.ReturnQuotedValue(), Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());


            int id = dao.Insert(sqlString);

            return id;
        }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Test_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Test> list = ToList<Test>(dataTable);
            Test test = new Test();
            test = list.FirstOrDefault();
            return (T)Convert.ChangeType(test, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Test_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<Test> list = ToList<Test>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }



        public override bool Update()
        {
            bool isSuccess = false;
            if (Id != 0)
            {
                string sqlString = string.Format(@"UPDATE Test_Table 
                              SET Adi={0},Degistiren={1},DegistirmeTarihi={2}
                               WHERE Id={3}",
                              Adi.ReturnQuotedValue(), Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);

                isSuccess = dao.Update2Db(sqlString);
            }
            return isSuccess;
        }


    }
}

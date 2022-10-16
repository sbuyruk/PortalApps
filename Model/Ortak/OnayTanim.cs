using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.Ortak
{
    public class OnayTanim : ParentClass
    {
        public string Adi { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OnayTanim> list = ToList<OnayTanim>(dataTable);
            OnayTanim onayTanim = new OnayTanim();
            onayTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(onayTanim, typeof(T));
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
                               FROM OnayTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OnayTanim> list = ToList<OnayTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO OnayTanim_Table 
                                        (Adi,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1}) ",
                                    Adi.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE OnayTanim_Table 
                                    SET PersonelId = {0},Adi={1},  Degistiren={3},DegistirmeTarihi={4}
                                    WHERE Id= {5}", Adi.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM OnayTanim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM OnayTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
    }
}

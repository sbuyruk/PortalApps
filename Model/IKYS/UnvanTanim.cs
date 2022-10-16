
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.IKYS
{
    public class UnvanTanim : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }


        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<UnvanTanim> list = ToList<UnvanTanim>(dataTable);
            UnvanTanim unvanTanim = new UnvanTanim();
            unvanTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(unvanTanim, typeof(T));
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
                               FROM UnvanTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<UnvanTanim> list = ToList<UnvanTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO UnvanTanim_Table 
                                        (Adi, KisaAdi,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1}) ",
                                    Adi.ReturnQuotedValue(), KisaAdi.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE UnvanTanim_Table 
                                    SET PersonelId = {0},Adi={1}, KisaAdi={2}, Degistiren={3},DegistirmeTarihi={4}
                                    WHERE Id= {5}", Adi.ReturnQuotedValue(), KisaAdi.ReturnQuotedValue(),
                                    Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }

        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM UnvanTanim_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM UnvanTanim_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }
        
    }
}

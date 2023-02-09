using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Model.NBYS
{
    public class BankaTanim : ParentClass
    {
        public string Banka { get; set; }
        public string BankaGrup { get; set; }
        public string BankaGrup2 { get; set; }
        public override bool Delete()
        {
            throw new NotImplementedException();
        }

        public override int Save()
        {
            throw new NotImplementedException();
        }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BankaTanim_Table
                               WHERE Id={0}", id);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BankaTanim> list = ToList<BankaTanim>(dataTable);
            BankaTanim bankaTanim = new BankaTanim();
            bankaTanim = list.FirstOrDefault();

            return (T)Convert.ChangeType(bankaTanim, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT * 
                               FROM BankaTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BankaTanim> list = ToList<BankaTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
        public BankaTanim SelectByBankaName(string BankaName)
        {
            string sqlString = string.Format(@"SELECT *
                                               FROM BankaTanim_Table
                                               WHERE Banka = '{0}'", BankaName);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<BankaTanim> list = ToList<BankaTanim>(dataTable);
            BankaTanim bankaTanim = new BankaTanim();
            bankaTanim = list.FirstOrDefault();
            return bankaTanim;
        }
        public List<string> SelectByBankaGrup()
        {
            string sqlString = string.Format(@"
                SELECT BankaGrup
                FROM BankaTanim_Table
                GROUP BY BankaGrup");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<string> list = dataTable.AsEnumerable()
                           .Select(r => r.Field<string>("BankaGrup"))
                           .ToList();
            return list;
        }
        public List<string> SelectByBankaGrup2()
        {
            string sqlString = string.Format(@"
                SELECT BankaGrup2
                FROM BankaTanim_Table
                GROUP BY BankaGrup2");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<string> list = dataTable.AsEnumerable()
                           .Select(r => r.Field<string>("BankaGrup2"))
                           .ToList();
            return list;
        }
    }
}

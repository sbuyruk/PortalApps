using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class BankaTanim : ParentClass
    {
        public string Banka { get; set; }
        public string BankaGrup { get; set; }
        public string BankaGrup2 { get; set; }
        public string HesapKodu { get; set; }
        public string HesapAdi { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<BankaTanim> genericEntity = new GenericEntity<BankaTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    BankaTanim item = Select<BankaTanim>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BankaTanim> genericEntity = new GenericEntity<BankaTanim>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public override bool Delete()
        {
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<BankaTanim> genericEntity = new GenericEntity<BankaTanim>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    BankaTanim item = Select<BankaTanim>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_BANKATANIM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BankaTanim_Table
                               WHERE Id={0}", id);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BankaTanim> list = ToList<BankaTanim>(dataTable);
            BankaTanim bankaTanim = new BankaTanim();
            bankaTanim = list.FirstOrDefault();

            return (T)Convert.ChangeType(bankaTanim, typeof(T));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(
                @"
                SELECT * 
                FROM BankaTanim_Table
                ORDER BY Banka");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BankaTanim> list = ToList<BankaTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public BankaTanim SelectByBankaName(string BankaName)
        {
            string sqlString = string.Format(@"SELECT *
                                               FROM BankaTanim_Table
                                               WHERE Banka = '{0}'", BankaName);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<string> list = dataTable.AsEnumerable()
                           .Select(r => r.Field<string>("BankaGrup2"))
                           .ToList();
            return list;
        }
    }
}


using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;

namespace Model.IKYS
{
    public class IletisimBilgileri : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adres { get; set; }
        public string Semt { get; set; }
        public string Ili { get; set; }
        public int Ilcesi { get; set; }
        public string PostaKodu { get; set; }
        public string DahiliTelefonu { get; set; }
        public string EvTelefonu { get; set; }
        public string CepTelefonu { get; set; }
        public string CepTelefonu2 { get; set; }
        public string IntranetEPosta { get; set; }
        public string InternetEPosta { get; set; }
        public string OzelEPosta { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);
            IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
            iletisimBilgileri = list.FirstOrDefault();
            return (T)Convert.ChangeType(iletisimBilgileri, typeof(T));
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
                               FROM IletisimBilgileri_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        private string saveSQL()
        {
            //Insert  SQL
            string InsertSQL = string.Format(@" 
                                    INSERT INTO IletisimBilgileri_Table 
                                        (PersonelId,Adres,Semt,Ilcesi,PostaKodu,DahiliTelefonu,EvTelefonu,CepTelefonu,CepTelefonu2,IntranetEPosta,
                                         InternetEPosta,OzelEPosta,Olusturan,OlusturmaTarihi)
                                    VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}) ",
                                    PersonelId.ReturnZeroIfNull(), Adres.ReturnQuotedValue(), Semt.ReturnQuotedValue(),
                                    Ilcesi.ReturnQuotedValue(), PostaKodu.ReturnQuotedValue(), DahiliTelefonu.ReturnQuotedValue(),
                                    EvTelefonu.ReturnQuotedValue(), CepTelefonu.ReturnQuotedValue(), CepTelefonu2.ReturnQuotedValue(),
                                    IntranetEPosta.ReturnQuotedValue(), InternetEPosta.ReturnQuotedValue(), OzelEPosta.ReturnQuotedValue(),
                                    Olusturan.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat());
            return InsertSQL;
        }
        private string UpdateSQL()
        {
            //Insert  SQL
            string sqlSQL = string.Format(@"
                                    UPDATE IletisimBilgileri_Table 
                                    SET PersonelId = {0},Adres={1}, Semt={2},Ilcesi={3}, PostaKodu={4},DahiliTelefonu={5}, 
                                        EvTelefonu={6}, CepTelefonu={7}, CepTelefonu2={8},IntranetEPosta={9},InternetEPosta={10},
                                        OzelEPosta={11},Degistiren={12},DegistirmeTarihi={13}
                                        WHERE Id= {14}",
                                        PersonelId.ReturnQuotedValue(), Adres.ReturnQuotedValue(), Semt.ReturnQuotedValue(),
                                        Ilcesi.ReturnQuotedValue(), PostaKodu.ReturnQuotedValue(), DahiliTelefonu.ReturnQuotedValue(),
                                        EvTelefonu.ReturnQuotedValue(), CepTelefonu.ReturnQuotedValue(), CepTelefonu2.ReturnQuotedValue(),
                                        IntranetEPosta.ReturnQuotedValue(), InternetEPosta.ReturnQuotedValue(), OzelEPosta.ReturnQuotedValue(),
                                        Degistiren.ReturnQuotedValue(), DateTime.Now.ReturnTRDateFormat(), Id);
            return sqlSQL;
        }
        public IletisimBilgileri SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);
            IletisimBilgileri ib = list.FirstOrDefault();
            return (ib);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IletisimBilgileri_Table  
                    WHERE PersonelId={0}
                    ORDER BY Adres", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IletisimBilgileri_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IletisimBilgileri_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

    }
}

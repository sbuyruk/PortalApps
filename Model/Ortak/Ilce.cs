using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{

    public class Ilce : ParentClass
    {
        public int IlId { get; set; }
        public string IlAdi { get; set; }
        public int IlceId { get; set; }
        public string IlceAdi { get; set; }
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
                               FROM Ilce_Table 
                               WHERE Id={0}
                               ORDER BY IlceAdi ", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            Ilce ilce = new Ilce();
            ilce = list.FirstOrDefault();
            return (T)Convert.ChangeType(ilce, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Ilce_Table ORDER BY IlceAdi");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
        public Ilce()
        {

        }
        public Ilce(int ilceId)
        {
            Ilce ilce = new Ilce();
            ilce = Select<Ilce>(ilceId);
            if (ilce != null)
            {
                this.IlAdi = ilce.IlAdi;
                this.IlceId = ilce.IlceId;
                this.IlceAdi = ilce.IlceAdi;
            }

        }
        public Ilce SelectByIlceId(int ilceId)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Ilce_Table
                                                WHERE Id={0} ORDER BY IlceAdi", ilceId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            Ilce ilce = new Ilce();
            ilce = list.FirstOrDefault();
            return ilce;
        }

        public DataTable SelectIlceAdiFromILCELER(int ilceId)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM ILCELER
                                                WHERE ILCE_ID={0} ORDER BY IlceAdi", ilceId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }

        public Ilce SelectByIlAndIlceAdi(string ilceAdi, string ilAdi)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Ilce_Table
                                                WHERE IlAdi='{0}'
                                                AND IlceAdi LIKE '%{1}%' ORDER BY IlceAdi", ilAdi, ilceAdi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            Ilce ilce = new Ilce();
            ilce = list.FirstOrDefault();
            return ilce;
        }


        public List<Ilce> SelectByIlAdi(string pIlAdi)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Ilce_Table
                                                WHERE IlAdi='{0}' ORDER BY IlceAdi", pIlAdi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            return list;
        }

        
        public int SelectCountIlceByBolgeId(int bolgeId)
        {
            string sqlString = string.Format(@"
                SELECT COUNT(A.Id) Adet
                FROM Ilce_Table A
	                INNER JOIN Il_Table B ON B.Id=A.IlId
                WHERE B.Id BETWEEN 1 AND 81 AND B.BolgeId={0} AND IlceAdi!={1} ", bolgeId, ProjeConstants.ILCE_MERKEZ.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            DataRow row = dataTable.Rows[0];
            int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
            return adet;
        }
        public List<Ilce> SelectByIlId(int pIlId)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Ilce_Table
                                                WHERE IlId={0} ORDER BY IlceAdi", pIlId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);

            return list;
        }
        public Ilce SelectByIlNameAndIlceName(string ilName, string ilceName)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Ilce_Table
                                                WHERE LOWER(IlceAdi)=LOWER('{0}') AND LOWER(IlAdi)=LOWER('{1}') ORDER BY IlceAdi", ilceName, ilName);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            Ilce ilce = new Ilce();
            ilce = list.FirstOrDefault();
            return ilce;
        }
        public List<Ilce> SelectFTKKuruluOlanIlceler(int ilId)
        {
            string iliStr = ilId < 1 ? string.Empty : string.Format(" AND B.Id={0}", ilId);
            string sqlString = string.Format(@"
                SELECT A.* 
                FROM Ilce_Table A 
	                INNER JOIN Il_Table B ON B.Id= A.IlId
                WHERE A.IlceAdi!= {0} 
                    AND (B.Id BETWEEN 0 AND 81 AND B.IlAdi != '') 
                    AND  A.Id IN (SELECT Ilcesi FROM FTK_Table WHERE Ilcesi > 0) 
                    {1}                    
                ORDER BY A.IlceAdi     
            ", ProjeConstants.ILCE_MERKEZ.ReturnQuotedValue(), iliStr);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Ilce> list = ToList<Ilce>(dataTable);
            return list;
        }

    }
}


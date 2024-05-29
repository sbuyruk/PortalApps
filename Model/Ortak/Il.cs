using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class Il : ParentClass
    {
        public string IlAdi { get; set; }
        public int PlakaKodu { get; set; }
        public string IngIlAdi { get; set; }
        public string Bolge { get; set; }
        public int BolgeId { get; set; }

        public override int Save()
        {
            throw new NotImplementedException();
        }
        public override bool Update()
        {
            throw new NotImplementedException();
        }
        public override bool Delete()
        {
            throw new NotImplementedException();
        }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Il_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            Il il = new Il();
            il = list.FirstOrDefault();
            return (T)Convert.ChangeType(il, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Il_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Il> SelectAllOrderByBolge()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Il_Table
                ORDER BY Bolge, IlAdi");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);

            return (list);
        }
        public Il SelectByIngAdi(string ingIlAdi)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Il_Table
                                                WHERE LOWER(IngIlAdi)=LOWER('{0}')", ingIlAdi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            Il il = new Il();
            il = list.FirstOrDefault();
            return il;
        }

        public int SelectCountIlByBolgeId(int bolgeId)
        {
            string sqlString = string.Format(@"
                SELECT COUNT(Id) Adet
                FROM Il_Table
                WHERE Id BETWEEN 1 AND 81 AND BolgeId={0}", bolgeId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                DataRow row = dataTable.Rows[0];
                int adet = row["Adet"].ReturnZeroIfNull().ConvertToInt();
                return adet;
            }
            return 0;
        }
        public List<Il> SelectByBolge(string bolge)
        {
            string bolgeStr = string.IsNullOrEmpty(bolge)||bolge.Equals(ProjeConstants.HEPSI)?string.Empty: string.Format(" AND Bolge={0} ",bolge.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT *
                FROM Il_Table
                WHERE (Id BETWEEN 1 AND 81)
                {0}", bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            return list;
        }
        public List<Il> SelectByBolgeId(int bolgeId)
        {
            string bolgeStr = bolgeId==ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND BolgeId={0} ", bolgeId);
            string sqlString = string.Format(@"
                SELECT *
                FROM Il_Table
                WHERE (Id BETWEEN 1 AND 81)
                {0}", bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            return list;
        }

        public Il SelectByIlAdi(string ilAdi)
        {
            string sqlString = string.Format(@"SELECT *
                                                FROM Il_Table
                                                WHERE LOWER(IlAdi)=LOWER('{0}')", ilAdi);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            Il il = new Il();
            il = list.FirstOrDefault();
            return il;
        }
        /// <summary>
        /// Valilikte veya en az bir ilcede ftk kurulu olan iller
        /// </summary>
        /// <param name="ilId"></param>
        /// <returns></returns>
        public List<Il> SelectFTKKuruluOlanIller()
        {
            //string iliStr = ilId < 1 ? string.Empty : string.Format(" AND A.Id={0}", ilId);
            string sqlString = string.Format(@"
                SELECT A.* 
                FROM Il_Table A 
                WHERE (A.Id BETWEEN 0 AND 81 AND A.IlAdi != '') 
                    AND  A.Id IN (SELECT Ili FROM FTK_Table) 
                ORDER BY A.Bolge, A.Id    
            ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Il> list = ToList<Il>(dataTable);
            return list;
        }
        public DataTable SelectFTKKuruluOlanBolgeler()
        {
            string sqlString = string.Format(@"
                SELECT A.Bolge 
                FROM Il_Table A 
                WHERE (A.Id BETWEEN 0 AND 81 AND A.IlAdi != '') 
                    AND  A.Id IN (SELECT Ili FROM FTK_Table) 
                GROUP BY A.Bolge
				ORDER BY A.Bolge   
            ");
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
    }
}

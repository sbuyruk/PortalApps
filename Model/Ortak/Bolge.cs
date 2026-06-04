using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class Bolge : ParentClass
    {
        public string Adi { get; set; }
        public string KisaAdi { get; set; }

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
                               FROM Bolge_Table 
                               WHERE Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bolge> list = ToList<Bolge>(dataTable);
            Bolge bolge = new Bolge();
            bolge = list.FirstOrDefault();
            return (T)Convert.ChangeType(bolge, typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM Bolge_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bolge> list = ToList<Bolge>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Bolge> SelectAktifBolgeler(int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND Id={0} ", bolgeId);
            string sqlString = string.Format(@"
                SELECT *
                FROM Bolge_Table
                WHERE Aktif=1 
                {0}",bolgeStr
            );

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bolge> list = ToList<Bolge>(dataTable);

            return list;
        }
        public Bolge SelectByBagisciId( int nakitBagisciId)
        {
            string sqlString = string.Format(@"
                SELECT A.* 
                FROM Bolge_Table A
                Inner JOIN Il_Table C ON C.BolgeId=A.Id
                Inner Join NakitBagisci_Table B On B.Ili=C.Id
                WHERE B.Id={0}
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bolge> list = ToList<Bolge>(dataTable);
            Bolge bolge = new Bolge();
            bolge = list.FirstOrDefault();
            return bolge;
        }
        public Bolge Select(int bolgeId)
        {
            string bolgeStr = bolgeId > 0 ? string.Format(" WHERE Id={0}", bolgeId):string.Empty;
            string sqlString = string.Format(@"SELECT *
                                                FROM Bolge_Table
                                                {0}", bolgeStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Bolge> list = ToList<Bolge>(dataTable);
            Bolge bolge = new Bolge();
            bolge = list.FirstOrDefault();
            return bolge;
        }
    }
}

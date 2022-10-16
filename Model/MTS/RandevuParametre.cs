using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    [Serializable]
    public class RandevuParametre : ParentClass
    {
        public string Grup { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<RandevuParametre> genericEntity = new GenericEntity<RandevuParametre>(ProjeConstants.SQL_DELETE);
                    OlusturmaTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    bool isDeleted = dao.DeleteFromDb(sqlString, "");
                    return isDeleted;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override int Save()
        {
            try
            {
                GenericEntity<RandevuParametre> genericEntity = new GenericEntity<RandevuParametre>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public RandevuParametre Select(int id)
        {
            GenericEntity<RandevuParametre> genericEntity = new GenericEntity<RandevuParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);
            _ = new RandevuParametre();
            RandevuParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<RandevuParametre> genericEntity = new GenericEntity<RandevuParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);
            RandevuParametre item = new RandevuParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM RandevuParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<RandevuParametre> genericEntity = new GenericEntity<RandevuParametre>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isSuccess;
        }
        public List<RandevuParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM RandevuParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ",parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);

            return (list);
        }
        public List<RandevuParametre> SelectSecilmemisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM RandevuParametre_Table A
	                LEFT JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id
                WHERE Grup={0} AND B.AniObjesiId IS NULL
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);

            return (list);
        }
        public List<RandevuParametre> SelectSecilmisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM RandevuParametre_Table A
	                INNER JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id
                WHERE Grup={0} 
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);

            return (list);
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT * FROM RandevuParametre_Table 
                ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM RandevuParametre_Table 
                ORDER BY Sira, Deger");
            DataTable dataTable;
            try
            {
                dataTable = dao.selectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }

        public List<RandevuParametre> SelectByGrupDeger(string grup, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM RandevuParametre_Table 
                WHERE Grup={0} AND Deger={1}
                ORDER BY Sira, Deger
                ", grup.ReturnQuotedValue(),deger.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<RandevuParametre> list = ToList<RandevuParametre>(dataTable);

            return list;
        }
    }
}

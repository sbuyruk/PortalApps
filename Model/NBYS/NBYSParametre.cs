using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class NBYSParametre : ParentClass
    {
        public string Grup { get; set; }
        public string Anahtar { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_DELETE);
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
                GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_INSERT);
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
        public NBYSParametre Select(int id)
        {
            GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);
            _ = new NBYSParametre();
            NBYSParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);
            NBYSParametre item = new NBYSParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM NBYSParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_UPDATE);
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
        public List<NBYSParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM NBYSParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (list);
        }
        public List<NBYSParametre> SelectSecilmemisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table A	                
                WHERE Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (list);
        }
        public List<NBYSParametre> SelectSecilmisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table A
	                INNER JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id
                WHERE Grup={0} 
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (list);
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT * FROM NBYSParametre_Table 
                ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");

            return dataTable;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM NBYSParametre_Table 
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

        public NBYSParametre SelectByGrupAnahtar(string grup, string anahtar)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table 
                WHERE Grup={0} AND Anahtar={1} 
                ORDER BY Sira, Anahtar
                ", grup.ReturnQuotedValue(), anahtar.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return list.FirstOrDefault<NBYSParametre>();
        }
        public NBYSParametre SelectByGrupAnahtar(string grup, string anahtar, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table 
                WHERE Grup={0} AND Anahtar={1} AND Deger={2}
                ORDER BY Sira, Anahtar
                ", grup.ReturnQuotedValue(), anahtar.ReturnQuotedValue(), deger.ReturnQuotedValue());

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return list.FirstOrDefault<NBYSParametre>();
        }
    }
}

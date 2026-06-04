using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Ortak
{
    [Serializable]
    public class OrtakParametre : ParentClass
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
                    GenericEntity<OrtakParametre> genericEntity = new GenericEntity<OrtakParametre>(ProjeConstants.SQL_DELETE);
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

                throw;
            }
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OrtakParametre> genericEntity = new GenericEntity<OrtakParametre>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public OrtakParametre Select(int id)
        {
            GenericEntity<OrtakParametre> genericEntity = new GenericEntity<OrtakParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);
            _ = new OrtakParametre();
            OrtakParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<OrtakParametre> genericEntity = new GenericEntity<OrtakParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);
            OrtakParametre item = new OrtakParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM OrtakParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<OrtakParametre> genericEntity = new GenericEntity<OrtakParametre>(ProjeConstants.SQL_UPDATE);
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
        public OrtakParametre SelectByAnahtar(string anahtar)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM OrtakParametre_Table 
                WHERE Anahtar={0}
                ORDER BY Sira, Deger
                ", anahtar.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);
            _ = new OrtakParametre();
            OrtakParametre item = list.FirstOrDefault();
            return item;
        }
        public List<OrtakParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM OrtakParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ",parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);

            return (list);
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT * FROM OrtakParametre_Table 
                ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM OrtakParametre_Table 
                ORDER BY Sira, Deger");
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }

        public List<OrtakParametre> SelectByGrupDeger(string grup, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM OrtakParametre_Table 
                WHERE Grup={0} AND Deger={1}
                ORDER BY Sira, Deger
                ", grup.ReturnQuotedValue(),deger.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<OrtakParametre> list = ToList<OrtakParametre>(dataTable);

            return list;
        }
    }
}


using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class YabanciDil : ParentClass
    {
        public int PersonelId { get; set; }
        public string Dil { get; set; }
        public string SinavAdi { get; set; }
        public string SinavNotu { get; set; }
        public DateTime SinavTarihi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            GenericEntity<YabanciDil> genericEntity = new GenericEntity<YabanciDil>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YabanciDil> list = ToList<YabanciDil>(dataTable);
            YabanciDil retval = new YabanciDil();
            retval = list.FirstOrDefault();
            return (T)Convert.ChangeType(retval, typeof(T));
        }

        public override int Save()
        {
            try
            {
                GenericEntity<YabanciDil> genericEntity = new GenericEntity<YabanciDil>(ProjeConstants.SQL_INSERT);
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

        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<YabanciDil> genericEntity = new GenericEntity<YabanciDil>(ProjeConstants.SQL_UPDATE);
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

        public override bool Delete()
        {
            string sqlString = string.Format(@"
                Delete FROM YabanciDil_Table 
                WHERE Id ={0}
            ", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM YabanciDil_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YabanciDil> list = ToList<YabanciDil>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
       
        public List<YabanciDil> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<YabanciDil> list = ToList<YabanciDil>(dataTable);

            return (list);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM YabanciDil_Table  
                    WHERE PersonelId={0}
                    ORDER BY SinavTarihi DESC", pId);
            return sqlstr;
        }

    }
}

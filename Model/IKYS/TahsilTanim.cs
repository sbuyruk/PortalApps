
using Model.Ortak;
using Model.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class TahsilTanim : ParentClass
    {
        public string TahsilDurumu { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TahsilTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);
            TahsilTanim tahsilTanim = new TahsilTanim();
            tahsilTanim = list.FirstOrDefault();
            return (T)Convert.ChangeType(tahsilTanim, typeof(T));
        }

        public TahsilTanim Select(int id)
        {
            GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);
            TahsilTanim tahsilTanim = new TahsilTanim();
            tahsilTanim = list.FirstOrDefault();
            return tahsilTanim;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_INSERT);
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
                    GenericEntity<TahsilTanim> genericEntity = new GenericEntity<TahsilTanim>(ProjeConstants.SQL_UPDATE);
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
            string sqlString = string.Format(@"DELETE 
                               FROM TahsilTanim_Table
                               WHERE Id={0}", Id);
            bool isSuccess = dao.DeleteFromDb(sqlString, this);
            return isSuccess;
        }
       
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TahsilTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TahsilTanim> list = ToList<TahsilTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

       
    }
}

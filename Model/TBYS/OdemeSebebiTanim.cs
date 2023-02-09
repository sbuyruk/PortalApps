using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class OdemeSebebiTanim : ParentClass
    {
        public string OdemeSebebi { get; set; }
        public string Aciklama { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeSebebiTanim_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);
            OdemeSebebiTanim odeme = new OdemeSebebiTanim();
            odeme = list.FirstOrDefault();
            return (T)Convert.ChangeType(odeme, typeof(T));

        }
        public OdemeSebebiTanim Select(int id)
        {
            GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);
            OdemeSebebiTanim odeme = new OdemeSebebiTanim();
            odeme = list.FirstOrDefault();
            return odeme;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                return id;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<OdemeSebebiTanim> genericEntity = new GenericEntity<OdemeSebebiTanim>(ProjeConstants.SQL_UPDATE);
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
                               FROM OdemeSebebiTanim_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM OdemeSebebiTanim_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<OdemeSebebiTanim> list = ToList<OdemeSebebiTanim>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}

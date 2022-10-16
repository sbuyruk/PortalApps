using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class TasinmazBagis : ParentClass
    {
        public DateTime BagisTarihi { get; set; }
        public int HissePay { get; set; }
        public int HissePayda { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazBagis_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TasinmazBagis> list = ToList<TasinmazBagis>(dataTable);
            TasinmazBagis bagis = new TasinmazBagis();
            bagis = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagis, typeof(T));

        }
        public TasinmazBagis Select(int id)
        {
            GenericEntity<TasinmazBagis> genericEntity = new GenericEntity<TasinmazBagis>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TasinmazBagis> list = ToList<TasinmazBagis>(dataTable);
            TasinmazBagis bagis = new TasinmazBagis();
            bagis = list.FirstOrDefault();
            return bagis;
        }
        public override int Save()
        {
            try
            {
                GenericEntity<TasinmazBagis> genericEntity = new GenericEntity<TasinmazBagis>(ProjeConstants.SQL_INSERT);
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
                    GenericEntity<TasinmazBagis> genericEntity = new GenericEntity<TasinmazBagis>(ProjeConstants.SQL_UPDATE);
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
                               FROM TasinmazBagis_Table
                               WHERE Id={0}", Id);

            bool isSuccess = dao.DeleteFromDb(sqlString, this);

            return isSuccess;
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM TasinmazBagis_Table");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<TasinmazBagis> list = ToList<TasinmazBagis>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
    }
}

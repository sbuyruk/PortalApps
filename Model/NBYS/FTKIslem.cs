using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class FTKIslem : ParentClass
    {
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string SorumluBolge { get; set; }     
        public DateTime KurulusTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public override bool Delete()
        {
            try
            {
                if (Id != 0)
                {
                    GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_DELETE);
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
                GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_INSERT);
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
        public FTKIslem Select(int id)
        {
            GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);
            FTKIslem item = new FTKIslem();
            item = list.FirstOrDefault();
            return item;
        }
        public FTKIslem SelectByIliIlcesi(int ili,int ilcesi)
        {

            string sqlString = string.Format(@"
                SELECT * FROM FTKIslem_Table
                WHERE Ili={0} AND Ilcesi={1}        
            
            ",ili,ilcesi);

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);
            FTKIslem item = new FTKIslem();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);
            FTKIslem item = new FTKIslem();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FTK_Table ORDER BY Ili
                ");

            DataTable dataTable = dao.selectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (Id != 0)
                {
                    GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_UPDATE);
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
    }
}

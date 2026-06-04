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
        public int BolgeId { get; set; }
        //public string SorumluBolge { get; set; }     
        public DateTime KurulusTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override bool Update()
        {
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    FTKIslem item = Select<FTKIslem>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
                    }
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
            try
            {
                bool isDeleted = false;
                if (Id != 0)
                {
                    GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    FTKIslem item = Select<FTKIslem>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_FTKISLEM);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public FTKIslem Select(int id)
        {
            GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);
            FTKIslem item = new FTKIslem();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FTKIslem> genericEntity = new GenericEntity<FTKIslem>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FTKIslem> list = ToList<FTKIslem>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

    }
}

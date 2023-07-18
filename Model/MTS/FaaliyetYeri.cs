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
    public class FaaliyetYeri : ParentClass
    {
        public string Adi { get; set; }
        public int Sira { get; set; }
        public string Aciklama { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<FaaliyetYeri> genericEntity = new GenericEntity<FaaliyetYeri>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUPARAMETRE);
                }
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
                if (this != null)
                {
                    FaaliyetYeri item = Select<FaaliyetYeri>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FaaliyetYeri> genericEntity = new GenericEntity<FaaliyetYeri>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUPARAMETRE);
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
                    GenericEntity<FaaliyetYeri> genericEntity = new GenericEntity<FaaliyetYeri>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    FaaliyetYeri item = Select<FaaliyetYeri>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_RANDEVUPARAMETRE);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FaaliyetYeri Select(int id)
        {
            GenericEntity<FaaliyetYeri> genericEntity = new GenericEntity<FaaliyetYeri>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetYeri> list = ToList<FaaliyetYeri>(dataTable);
            _ = new FaaliyetYeri();
            FaaliyetYeri item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FaaliyetYeri> genericEntity = new GenericEntity<FaaliyetYeri>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetYeri> list = ToList<FaaliyetYeri>(dataTable);
            FaaliyetYeri item = new FaaliyetYeri();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FaaliyetYeri_Table ORDER BY Sira,Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetYeri> list = ToList<FaaliyetYeri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
       
    }
}

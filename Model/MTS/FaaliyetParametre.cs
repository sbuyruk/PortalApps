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
    public class FaaliyetParametre : ParentClass
    {
        public string Grup { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<FaaliyetParametre> genericEntity = new GenericEntity<FaaliyetParametre>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETPARAMETRE);
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
                    FaaliyetParametre item = Select<FaaliyetParametre>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<FaaliyetParametre> genericEntity = new GenericEntity<FaaliyetParametre>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETPARAMETRE);
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
                    GenericEntity<FaaliyetParametre> genericEntity = new GenericEntity<FaaliyetParametre>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    FaaliyetParametre item = Select<FaaliyetParametre>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_FAALIYETPARAMETRE);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FaaliyetParametre Select(int id)
        {
            GenericEntity<FaaliyetParametre> genericEntity = new GenericEntity<FaaliyetParametre>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetParametre> list = ToList<FaaliyetParametre>(dataTable);
            _ = new FaaliyetParametre();
            FaaliyetParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<FaaliyetParametre> genericEntity = new GenericEntity<FaaliyetParametre>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetParametre> list = ToList<FaaliyetParametre>(dataTable);
            FaaliyetParametre item = new FaaliyetParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FaaliyetParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetParametre> list = ToList<FaaliyetParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<FaaliyetParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM FaaliyetParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ",parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetParametre> list = ToList<FaaliyetParametre>(dataTable);

            return (list);
        }
     
        public List<FaaliyetParametre> SelectByGrupDeger(string grup, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM FaaliyetParametre_Table 
                WHERE Grup={0} AND Deger={1}
                ORDER BY Sira, Deger
                ", grup.ReturnQuotedValue(),deger.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<FaaliyetParametre> list = ToList<FaaliyetParametre>(dataTable);

            return list;
        }
    }
}

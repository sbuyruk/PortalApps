using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.Portal
{
    [Serializable]
    public class ToplantiParametre : ParentClass
    {
        public string Grup { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<ToplantiParametre> genericEntity = new GenericEntity<ToplantiParametre>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.PORTAL_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIPARAMETRE);
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
                    ToplantiParametre item = Select<ToplantiParametre>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<ToplantiParametre> genericEntity = new GenericEntity<ToplantiParametre>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.PORTAL_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIPARAMETRE);
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
                    GenericEntity<ToplantiParametre> genericEntity = new GenericEntity<ToplantiParametre>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    ToplantiParametre item = Select<ToplantiParametre>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.PORTAL_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.PORTAL, ProjeConstants.PORTAL_TOPLANTIPARAMETRE);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ToplantiParametre Select(int id)
        {
            GenericEntity<ToplantiParametre> genericEntity = new GenericEntity<ToplantiParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);
            _ = new ToplantiParametre();
            ToplantiParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<ToplantiParametre> genericEntity = new GenericEntity<ToplantiParametre>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);
            ToplantiParametre item = new ToplantiParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM ToplantiParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<ToplantiParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM ToplantiParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);

            return (list);
        }
        public List<ToplantiParametre> SelectSecilmemisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM ToplantiParametre_Table A	                
                WHERE Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);

            return (list);
        }
        public List<ToplantiParametre> SelectSecilmisByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM ToplantiParametre_Table A
	                INNER JOIN AniObjesiDagitim_Table B ON B.AniObjesiId=A.Id
                WHERE Grup={0} 
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);

            return (list);
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT * FROM ToplantiParametre_Table 
                ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM ToplantiParametre_Table 
                ORDER BY Sira, Deger");
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw;
            }
            string json = ToJSON(dataTable);
            return json;
        }

        public List<ToplantiParametre> SelectByGrupDeger(string grup, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM ToplantiParametre_Table 
                WHERE Grup={0} AND Deger={1}
                ORDER BY Sira, Deger
                ", grup.ReturnQuotedValue(), deger.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<ToplantiParametre> list = ToList<ToplantiParametre>(dataTable);

            return list;
        }
    }
}

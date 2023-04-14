using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    [Serializable]
    public class NBYSParametre : ParentClass
    {
        public string Grup { get; set; }
        public string Anahtar { get; set; }
        public string Deger { get; set; }
        public int Sira { get; set; }
        public override int Save()
        {
            try
            {
                GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
                }
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
                    NBYSParametre item = Select<NBYSParametre>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
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
                    GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    NBYSParametre item = Select<NBYSParametre>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NBYSPARAMETRE);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NBYSParametre Select(int id)
        {
            GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);
            _ = new NBYSParametre();
            NBYSParametre item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<NBYSParametre> genericEntity = new GenericEntity<NBYSParametre>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);
            NBYSParametre item = new NBYSParametre();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM NBYSParametre_Table ORDER BY Sira,Deger
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<NBYSParametre> SelectByGrupReturnList(string parametreGrubu)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM NBYSParametre_Table 
                WHERE Grup={0}
                ORDER BY Sira, Deger
                ", parametreGrubu.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return (list);
        }
        public NBYSParametre SelectByGrupAnahtar(string grup, string anahtar)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table 
                WHERE Grup={0} AND Anahtar={1} 
                ORDER BY Sira, Anahtar
                ", grup.ReturnQuotedValue(), anahtar.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return list.FirstOrDefault<NBYSParametre>();
        }
        public NBYSParametre SelectByGrupAnahtar(string grup, string anahtar, string deger)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM NBYSParametre_Table 
                WHERE Grup={0} AND Anahtar={1} AND Deger={2}
                ORDER BY Sira, Anahtar
                ", grup.ReturnQuotedValue(), anahtar.ReturnQuotedValue(), deger.ReturnQuotedValue());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NBYSParametre> list = ToList<NBYSParametre>(dataTable);

            return list.FirstOrDefault<NBYSParametre>();
        }
    }
}

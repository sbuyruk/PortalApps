
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
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL);
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
                YabanciDil item = Select<YabanciDil>(Id);
                if (Id != 0)
                {
                    GenericEntity<YabanciDil> genericEntity = new GenericEntity<YabanciDil>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL);
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
                bool isDeleted;
                if (Id != 0)
                {
                    GenericEntity<YabanciDil> genericEntity = new GenericEntity<YabanciDil>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    YabanciDil item = Select<YabanciDil>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_YABANCIDIL);
                    }
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
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM YabanciDil_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<YabanciDil> list = ToList<YabanciDil>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
       
        public List<YabanciDil> SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

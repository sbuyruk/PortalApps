
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class IletisimBilgileri : ParentClass
    {
        public int PersonelId { get; set; }
        public string Adres { get; set; }
        public string Semt { get; set; }
        public string Ili { get; set; }
        public int Ilcesi { get; set; }
        public string PostaKodu { get; set; }
        public string DahiliTelefonu { get; set; }
        public string EvTelefonu { get; set; }
        public string CepTelefonu { get; set; }
        public string CepTelefonu2 { get; set; }
        public string IntranetEPosta { get; set; }
        public string InternetEPosta { get; set; }
        public string OzelEPosta { get; set; }
        public string Plaka { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = SelectSQL(id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);
            IletisimBilgileri iletisimBilgileri = new IletisimBilgileri();
            iletisimBilgileri = list.FirstOrDefault();
            return (T)Convert.ChangeType(iletisimBilgileri, typeof(T));
        }

        public override int Save()
        {
            try
            {
                GenericEntity<IletisimBilgileri> genericEntity = new GenericEntity<IletisimBilgileri>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI);
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
                IletisimBilgileri item = Select<IletisimBilgileri>(Id);
                if (Id != 0)
                {
                    GenericEntity<IletisimBilgileri> genericEntity = new GenericEntity<IletisimBilgileri>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI);
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
                    GenericEntity<IletisimBilgileri> genericEntity = new GenericEntity<IletisimBilgileri>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    IletisimBilgileri item = Select<IletisimBilgileri>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_ILETISIMBILGILERI);
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
                               FROM IletisimBilgileri_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        
        public IletisimBilgileri SelectByPersonelId(int personelId)
        {
            string sqlString = SelectByPersonelIdSQL(personelId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<IletisimBilgileri> list = ToList<IletisimBilgileri>(dataTable);
            IletisimBilgileri ib = list.FirstOrDefault();
            return (ib);
        }
        private string SelectByPersonelIdSQL(int pId)
        {
            string sqlstr = string.Format(@" 
                    SELECT * FROM IletisimBilgileri_Table  
                    WHERE PersonelId={0}
                    ORDER BY Adres", pId);
            return sqlstr;
        }
        private string SelectSQL(int id)
        {
            string sqlstr = string.Format(@"SELECT *
                               FROM IletisimBilgileri_Table 
                               WHERE  Id={0}", id);
            return sqlstr;
        }
        private string DeleteSQL()
        {
            string sqlString = string.Format(@"
                            DELETE 
                            FROM IletisimBilgileri_Table
                            WHERE Id={0}", Id);
            return sqlString;
        }

    }
}

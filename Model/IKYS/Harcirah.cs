
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class Harcirah : ParentClass
    {
        public int KadroGrupId { get; set; }
        public string KadroGrubu { get; set; }
        public string Ulke { get; set; }
        public decimal Miktar { get; set; }
        public string ParaBirimi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            GenericEntity<Harcirah> genericEntity = new GenericEntity<Harcirah>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);
            Harcirah obj = new Harcirah();
            obj = list.FirstOrDefault();
            return (T)Convert.ChangeType(obj, typeof(T));
        }
        public Harcirah Select(int id)
        {
            GenericEntity<Harcirah> genericEntity = new GenericEntity<Harcirah>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);
            Harcirah obj = new Harcirah();
            obj = list.FirstOrDefault();
            return obj;
        }
        public override int Save()
        {
            try
            {               
                GenericEntity<Harcirah> genericEntity = new GenericEntity<Harcirah>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH);
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
                Harcirah item = Select<Harcirah>(Id);
                if (Id != 0)
                {
                    GenericEntity<Harcirah> genericEntity = new GenericEntity<Harcirah>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH);
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
                    GenericEntity<Harcirah> genericEntity = new GenericEntity<Harcirah>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    Harcirah item = Select<Harcirah>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_HARCIRAH);
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
                               FROM Harcirah_Table ORDER BY KadroGrupId, Sira");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<Harcirah> SelectByKadroGrupId(int kadroGrupId)
        {
            string birimKaldirildiMiStr= string.Format(" WHERE KadroGrupId={0}", kadroGrupId ); 
            string sqlString = string.Format(@"SELECT *
                               FROM Harcirah_Table 
                               {0}
                               ORDER BY KadroGrupId, Sira", birimKaldirildiMiStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return list;
        }
        public DataTable SelectAllReturnDataTable()
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM Harcirah_Table A

                ORDER BY KadroGrupId, Sira
                                    ");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataTable;
        }

    }
}

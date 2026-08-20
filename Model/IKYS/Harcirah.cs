
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
        public int SeriId { get; set; }
        public int Sira { get; set; }
        public string KadroGrubu { get; set; }
        public string Ulke { get; set; }
        public decimal Miktar { get; set; }
        public string ParaBirimi { get; set; }
        public string Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

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

                throw;
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

                throw;
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
        public List<Harcirah> SelectByKadroUlkeTarih(int kadroGrupId, string ulke, DateTime tarih)
        {
            string kadroStr = string.Format(" WHERE KadroGrupId={0}", kadroGrupId);
            if (tarih != null)
            {
                kadroStr += string.Format(" AND BaslangicTarihi<={0} --AND BitisTarihi is NULL", tarih.ReturnTRDateFormat());
            }
            string ulkestr = string.Empty;
            if (!string.IsNullOrEmpty(ulke))
            {
                ulkestr = string.Format(" AND Ulke={0}", ulke.ReturnQuotedValue());
            }
            string sqlString = string.Format(@"SELECT *
                               FROM Harcirah_Table 
                               {0}
                                {1}
                               ORDER BY KadroGrupId, Sira", kadroStr, ulkestr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return list;
        }
        public Harcirah SelectByKadroGrupId(int kadroGrupId)
        {
            string kadroStr = string.Format(" WHERE KadroGrupId={0}", kadroGrupId);
            
            string sqlString = string.Format(@"SELECT *
                               FROM Harcirah_Table 
                               {0}
                               ORDER BY KadroGrupId, Sira", kadroStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return list.FirstOrDefault();
        }
        public Harcirah SelectByParaBirimi(string paraBirimi)
        {
            string paraBirimiStr = string.Format(" WHERE ParaBirimi={0}", paraBirimi.ReturnQuotedValue());

            string sqlString = string.Format(@"SELECT *
                               FROM Harcirah_Table 
                               {0}
                               ", paraBirimiStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return list.FirstOrDefault();
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
                throw;
            }
            return dataTable;
        }
        public List<Harcirah> SelectAll()
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
                throw;
            }
            List<Harcirah> list = ToList<Harcirah>(dataTable);

            return list;
        }

    }
}

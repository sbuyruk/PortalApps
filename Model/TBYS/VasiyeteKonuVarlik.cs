using DAO.Ortak;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    public class VasiyeteKonuVarlik : ParentClass
    {
        public int VasiyetciId { get; set; }
        public string Konusu { get; set; }
        public string Cinsi { get; set; }
        public string AdetMiktar { get; set; }
        public decimal TahminiRayic { get; set; }
        public string Aciklama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
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
                    VasiyeteKonuVarlik item = Select<VasiyeteKonuVarlik>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
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
                    GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    VasiyeteKonuVarlik item = Select<VasiyeteKonuVarlik>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_VASIYETEKONUVARLIK);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public VasiyeteKonuVarlik Select(int id)
        {
            GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);
            VasiyeteKonuVarlik item = new VasiyeteKonuVarlik();
            item = list.FirstOrDefault();
            return item;
        }

        public override T Select<T>(int id)
        {
            GenericEntity<VasiyeteKonuVarlik> genericEntity = new GenericEntity<VasiyeteKonuVarlik>(ProjeConstants.SQL_SELECT);
            OlusturmaTarihi = DateTime.Now;
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);
            VasiyeteKonuVarlik item = new VasiyeteKonuVarlik();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }

        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table ORDER BY VasiyetciId
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public List<VasiyeteKonuVarlik> SelectByVasiyetciId(int vasiyetciId)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM VasiyeteKonuVarlik_Table 
                WHERE VasiyetciId={0}
                ORDER BY VasiyetciId
                ", vasiyetciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<VasiyeteKonuVarlik> list = ToList<VasiyeteKonuVarlik>(dataTable);

            return list;
        }

    }
}
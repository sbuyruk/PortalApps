using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.TBYS
{
    [Serializable]
    public class KiraBorcuTakip : ParentClass
    {
        public int KiraciId { get; set; }
        public int KiraSozlesmeId { get; set; }
        public int OdemePlaniId { get; set; }
        public decimal KiraBedeli { get; set; }
        public decimal ToplamBorcu { get; set; }
        public int KiraBorcuAySayisi { get; set; }
        public string TakipIslemi { get; set; }
        public int IslemAyi { get; set; }
        public int IslemYili { get; set; }
        public string IslemYapan{ get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Aciklama { get; set; }

        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraBorcuTakip_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraBorcuTakip> list = ToList<KiraBorcuTakip>(dataTable);
            KiraBorcuTakip kiraBorcuTakip = new KiraBorcuTakip();
            kiraBorcuTakip = list.FirstOrDefault();
            return (T)Convert.ChangeType(kiraBorcuTakip, typeof(T));

        }
        public KiraBorcuTakip Select(int kiraBorcuTakipId)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM KiraBorcuTakip_Table 
                               WHERE  Id={0}", kiraBorcuTakipId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraBorcuTakip> list = ToList<KiraBorcuTakip>(dataTable);
            KiraBorcuTakip kiraBorcuTakip = new KiraBorcuTakip();
            kiraBorcuTakip = list.FirstOrDefault();
            return kiraBorcuTakip;

        }
        
        public override int Save()
        {
            try
            {
                GenericEntity<KiraBorcuTakip> genericEntity = new GenericEntity<KiraBorcuTakip>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.TBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
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
                    KiraBorcuTakip item = Select<KiraBorcuTakip>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<KiraBorcuTakip> genericEntity = new GenericEntity<KiraBorcuTakip>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.TBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
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
                    GenericEntity<KiraBorcuTakip> genericEntity = new GenericEntity<KiraBorcuTakip>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    KiraBorcuTakip item = Select<KiraBorcuTakip>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.TBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM KiraBorcuTakip_Table
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<KiraBorcuTakip> list = ToList<KiraBorcuTakip>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        
        public KiraBorcuTakip SelectByKiraciIdAyYil(int kiraciId)
        {
            string sqlString = string.Format(@"
                SELECT  *
                FROM KiraBorcuTakip_Table
                WHERE KiraciId={0}
                    AND IslemAyi={1}
                    AND IslemYili={2}
                ", kiraciId.ReturnQuotedValue(), DateTime.Today.Month, DateTime.Today.Year);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                List<KiraBorcuTakip> list = ToList<KiraBorcuTakip>(dataTable);
                KiraBorcuTakip kiraBorcuTakip = list.FirstOrDefault();
                return kiraBorcuTakip;
            }
            else
            {
                return null;
            }
        }
    }
}

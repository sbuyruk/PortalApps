using DocumentFormat.OpenXml.Office2010.Excel;
using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.IKYS
{
    public class MaasHareket : ParentClass
    {

        public int PersonelId { get; set; }
        public DateTime Tarih{ get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvan { get; set; }
        public int Derece { get; set; }
        public int Kademe { get; set; }
        public DateTime DereceKademeIlerlemeTarihi { get; set; }
        public decimal Ucret { get; set; }
        public decimal Ikramiye { get; set; }
        public decimal Agi { get; set; }
        public decimal ToplamUcret { get; set; }
        public int GrupId { get; set; }
        public int ProtokolSiraNo { get; set; }
        public override T Select<T>(int id)
        {
            GenericEntity<MaasHareket> genericEntity = new GenericEntity<MaasHareket>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MaasHareket> list = ToList<MaasHareket>(dataTable);
            MaasHareket maasHareket = new MaasHareket();
            maasHareket = list.FirstOrDefault();
            return (T)Convert.ChangeType(maasHareket, typeof(T));
        }
       
        public MaasHareket Select(int id)
        {
            GenericEntity<MaasHareket> genericEntity = new GenericEntity<MaasHareket>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MaasHareket> list = ToList<MaasHareket>(dataTable);
            MaasHareket maasHareket = new MaasHareket();
            maasHareket = list.FirstOrDefault();
            return maasHareket;
        }
        public override int Save()
        {
            try
            {

                GenericEntity<MaasHareket> genericEntity = new GenericEntity<MaasHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.IKYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
                MaasHareket item = Select<MaasHareket>(Id);
                if (Id != 0)
                {
                    GenericEntity<MaasHareket> genericEntity = new GenericEntity<MaasHareket>(ProjeConstants.SQL_UPDATE);
                    DegistirmeTarihi = DateTime.Now;
                    Degistiren = UtilityHelper.GetCurrentUserName();
                    string sqlString = genericEntity.GetQuery(this);
                    isSuccess = dao.Update2Db(sqlString);
                }
                if (isSuccess && ProjeConstants.IKYS_UPDATE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
                    GenericEntity<MaasHareket> genericEntity = new GenericEntity<MaasHareket>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);

                    MaasHareket item = Select<MaasHareket>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
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
            string sqlString = string.Format(@"
                SELECT *
                FROM MaasHareket_Table 
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MaasHareket> list = ToList<MaasHareket>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public MaasHareket SelectByTarih(DateTime tarih)
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM MaasHareket_Table 
                WHERE Tarih={0}
                ORDER BY Derece,Kademe
                ",tarih.ReturnTRDateFormat());

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MaasHareket> list = ToList<MaasHareket>(dataTable);
            MaasHareket maasHareket = new MaasHareket();
            maasHareket = list.FirstOrDefault();
            return maasHareket;
        }

        public List<MaasHareket> SelectMaasListesiByTarih(DateTime tarih)
        {
            // MaasHareket_Table'dan Tarih'e göre maas listesini seçen SQL sorgusu
            string sqlString = string.Format(@"
                SELECT *
                FROM MaasHareket_Table 
                WHERE Tarih={0}
                ORDER BY Derece,Kademe
                ", tarih.ReturnTRDateFormat());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<MaasHareket> list = ToList<MaasHareket>(dataTable);
            return list;
        }

        public bool DeleteByGrupId(int grupId)
        {
            // MaasHareket_Table'dan GrupId'ye göre silen SQL sorgusu
            string sqlString = string.Format(@"
                DELETE FROM MaasHareket_Table 
                WHERE GrupId={0}
                ", grupId);
            bool isDeleted = dao.DeleteFromDb(sqlString, "");
            if (isDeleted && ProjeConstants.IKYS_DELETE_LOG)
            {
                OlayKayit olayKayit = new OlayKayit();
                olayKayit.SilmeOlayKaydet(this, ProjeConstants.IKYS, ProjeConstants.IKYS_GOREVONAY);
            }
            return isDeleted;
        }
    }
}

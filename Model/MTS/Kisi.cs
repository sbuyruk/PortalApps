using Model.Ortak;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.MTS
{
    public class Kisi : ParentClass
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public string Kurumu { get; set; }
        public string Unvani { get; set; }
        public string Gorevi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string Telefon3 { get; set; }
        public string TelAciklama1 { get; set; }
        public string TelAciklama2 { get; set; }
        public string TelAciklama3 { get; set; }
        public string Adres { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Aciklama { get; set; }
        public string Dahili1 { get; set; }
        public string Dahili2 { get; set; }
        public string Dahili3 { get; set; }
        public DateTime DogumTarihi { get; set; }
        public bool Kutlama { get; set; }

        public override int Save()
        {
            try
            {
                GenericEntity<Kisi> genericEntity = new GenericEntity<Kisi>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);
                if (id > 0 && ProjeConstants.MTS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.MTS, ProjeConstants.MTS_KISI);
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
            bool updateLog = ProjeConstants.MTS_UPDATE_LOG;
            bool isSuccess = false;
            try
            {
                if (this != null)
                {
                    Kisi item = Select<Kisi>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<Kisi> genericEntity = new GenericEntity<Kisi>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.MTS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.MTS, ProjeConstants.MTS_KISI);
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
                    GenericEntity<Kisi> genericEntity = new GenericEntity<Kisi>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    Kisi item = Select<Kisi>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.MTS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.MTS, ProjeConstants.MTS_KISI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Kisi Select(int id)
        {
            GenericEntity<Kisi> genericEntity = new GenericEntity<Kisi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);
            Kisi item = new Kisi();
            item = list.FirstOrDefault();
            return item;
        }
        public override T Select<T>(int id)
        {
            GenericEntity<Kisi> genericEntity = new GenericEntity<Kisi>(ProjeConstants.SQL_SELECT);
            Id = id;
            string sqlString = genericEntity.GetQuery(this);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);
            Kisi item = new Kisi();
            item = list.FirstOrDefault();
            return ((T)Convert.ChangeType(item, typeof(T)));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"
                SELECT *
                FROM Kisi_Table ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public DataTable SelectAllReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT A.*, B.IlAdi, C.IlceAdi FROM Kisi_Table A
                LEFT JOIN Il_Table B on A.Ili = B.Id
                LEFT JOIN Ilce_Table C on A.Ilcesi = C.Id
                ORDER BY Adi
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public List<Kisi> SelectByDogumGunuKutlamaReturnDT()
        {
            string sqlString = string.Format(@"
                SELECT * FROM Kisi_Table A
                WHERE Kutlama=1 AND DogumTarihi IS NOT NULL AND DogumTarihi > '01.01.1900'
                ");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);
            return list;
        }
        public string SelectAllReturnJson()
        {
            string sqlString = string.Format(@"
                SELECT A.Id KisiId, A.Adi,A.Soyadi
                FROM Kisi_Table A");
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectSecilmemisDisKatilimcilarByRandevuIdReturnDT(int randevuId)
        {
            string randevuIdStr = randevuId > 0 ? 
                string.Format(" WHERE A.Id NOT IN (select KatilimciId FROM RandevuKatilim_Table WHERE KatilimciTipi={0} AND RandevuId={1})", ProjeConstants.RANDEVU_KATILIMCI_DIS_INT, randevuId) : 
                string.Empty;
            string sqlString = string.Format(@"
                SELECT A.Id KatilimciId, A.Adi, A.Soyadi, {0} KatilimciTipi
                FROM Kisi_Table A
	               {1}
                ORDER BY A.Adi", ProjeConstants.RANDEVU_KATILIMCI_DIS_INT, randevuIdStr);
            DataTable dataTable;
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

        public Kisi SelectByAdiSoyadi(string adi, string soyadi)
        {
            adi = string.IsNullOrEmpty(adi) ? "#${}?" : adi;
            soyadi = string.IsNullOrEmpty(soyadi) ? "#${}?" : soyadi;
            string sqlString = string.Format(@"
                SELECT * 
                FROM Kisi_Table
                WHERE Adi ={0} AND Soyadi ={1}
                ", adi.Trim().ReturnQuotedValue(), soyadi.Trim().ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);
            Kisi kisi = new Kisi();
            kisi = list.FirstOrDefault();
            return kisi;
        }
        public Kisi SelectByTCKimlikNo(string tckimlik)
        {
            string sqlString = string.Format(@"
                SELECT * 
                FROM Kisi_Table
                WHERE TCKimlikNo={0}
                ", tckimlik);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<Kisi> list = ToList<Kisi>(dataTable);
            Kisi kisi = new Kisi();
            kisi = list.FirstOrDefault();
            return kisi;
        }
    }
}

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
    public class BagisaVesileOlanTesekkur : ParentClass
    {

        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Unvan { get; set; }
        public long TCKimlikNo { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public DateTime BelgeTarihi { get; set; }
        public string VerilmeSebebi { get; set; }
        public string BelgeMetni1 { get; set; }
        public string BelgeMetni2 { get; set; }
        public string ImzalayanAdiSoyadi { get; set; }
        public string ImzalayanUnvan { get; set; }
        public string ImzalayanMakam { get; set; }       
        public string Aciklama { get; set; }
        
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisaVesileOlanTesekkur_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisaVesileOlanTesekkur> list = ToList<BagisaVesileOlanTesekkur>(dataTable);
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur();
            bagisaVesileOlanTesekkur = list.FirstOrDefault();
            return (T)Convert.ChangeType(bagisaVesileOlanTesekkur, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<BagisaVesileOlanTesekkur> genericEntity = new GenericEntity<BagisaVesileOlanTesekkur>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this);
                int id = dao.Insert(sqlString);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
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
                    BagisaVesileOlanTesekkur item = Select<BagisaVesileOlanTesekkur>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<BagisaVesileOlanTesekkur> genericEntity = new GenericEntity<BagisaVesileOlanTesekkur>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        string sqlString = genericEntity.GetQuery(this);
                        isSuccess = dao.Update2Db(sqlString);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
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
                    GenericEntity<BagisaVesileOlanTesekkur> genericEntity = new GenericEntity<BagisaVesileOlanTesekkur>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    BagisaVesileOlanTesekkur item = Select<BagisaVesileOlanTesekkur>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(sqlString, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISCI);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisaVesileOlanTesekkur_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisaVesileOlanTesekkur> list = ToList<BagisaVesileOlanTesekkur>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public string SelectReturnJson()
        {
            string sqlString = string.Format(@"SELECT *, CONVERT(varchar,FORMAT(BelgeTarihi,'dd.MM.yyyy')) BelgeTarihiDDMMYYYY
                               FROM BagisaVesileOlanTesekkur_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectReturnDataTable()
        {
            string sqlString = string.Format(@"SELECT *, CONVERT(varchar,FORMAT(BelgeTarihi,'dd.MM.yyyy')) BelgeTarihiDDMMYYYY
                               FROM BagisaVesileOlanTesekkur_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public BagisaVesileOlanTesekkur SelectByTcKimlikno(long tcKimlikno)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisaVesileOlanTesekkur_Table 
                               WHERE  TCKimlikNo!=0 AND TCKimlikNo={0}", tcKimlikno);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisaVesileOlanTesekkur> list = ToList<BagisaVesileOlanTesekkur>(dataTable);
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur();
            bagisaVesileOlanTesekkur = list.FirstOrDefault();
            return bagisaVesileOlanTesekkur;

        }
       
        public BagisaVesileOlanTesekkur SelectByAdAndTelefon(string adi, string telefon)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisaVesileOlanTesekkur_Table 
                               WHERE  Adi={0} AND (Telefon1={1} OR Telefon2={1})", adi.ReturnQuotedValue(), telefon.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<BagisaVesileOlanTesekkur> list = ToList<BagisaVesileOlanTesekkur>(dataTable);
            BagisaVesileOlanTesekkur bagisaVesileOlanTesekkur = new BagisaVesileOlanTesekkur();
            bagisaVesileOlanTesekkur = list.FirstOrDefault();
            return bagisaVesileOlanTesekkur;

        }
        public DataTable SelectByAd(string adi)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM BagisaVesileOlanTesekkur_Table 
                               WHERE  Adi LIKE '%{0}%' ", adi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;

        }
        
        public DataTable SelectByFilterReturnDataTable(string filter, int eksiId)
        {
            string sqlString = string.Format(@"
                SELECT distinct(N.Id) BagisaVesileOlanTesekkurId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM BagisaVesileOlanTesekkur_Table N 
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili 
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                    ---INNER JOIN NakitBagisHareket_Table C on C.BagisciId=N.Id
                WHERE N.Id!= {0} AND N.Adi like '%{1}%'
	                OR N.TCKimlikNo like '%{1}%'
	                OR N.Telefon1 like '%{1}%'
	                OR N.Adres like '%{1}%'
                ORDER BY BagisaVesileOlanTesekkurId ", eksiId, filter);
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
        public string SelectByFilter(string filter, int eksiId)
        {
            string ilStr = string.Empty;

            string sqlString = string.Format(@"
                SELECT distinct(N.Id) BagisaVesileOlanTesekkurId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM BagisaVesileOlanTesekkur_Table N 
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili 
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                    ---INNER JOIN NakitBagisHareket_Table C on C.BagisciId=N.Id
                WHERE N.Id!= {0} AND N.Adi like '%{1}%'
	                OR N.TCKimlikNo like '%{1}%'
	                OR N.Telefon1 like '%{1}%'
	                OR N.Adres like '%{1}%'
	                --OR C.Adresi like '%{1}%'
	                --OR C.BagisTarihi like '%{1}%'
	                --OR C.BagisMiktari like '%{1}%'
	                --OR A.IlAdi like '%{1}%'
	                --OR B.IlceAdi like '%{1}%'
                ORDER BY BagisaVesileOlanTesekkurId ", eksiId, filter);
            DataTable dataTable = null;
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
       

    }
}

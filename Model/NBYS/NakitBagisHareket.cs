using DAO.Ortak;
using DocumentFormat.OpenXml.Bibliography;
using Model.Ortak;
using Model.Services.NBYS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace Model.NBYS
{
    public class NakitBagisHareket : ParentClass
    {
        public DateTime BagisTarihi { get; set; }
        public int BagisciId { get; set; }
        public decimal BagisMiktari { get; set; }
        public string DovizCinsi { get; set; }
        public int BankaId { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adresi { get; set; }
        public string Telefon { get; set; }
        public string Aciklama { get; set; }
        public int ArmaganId { get; set; }
        public bool IadeEdildiMi { get; set; }
        public decimal IadeMiktari { get; set; }
        public DateTime IadeTarihi { get; set; }
        public string IadeSebebi { get; set; }
        public string IadeEden { get; set; }
        public decimal DovizTutari { get; set; }
        public decimal DovizKuru { get; set; }
        public DateTime KurTarihi { get; set; }
        public int EkstreAktarmaId { get; set; }
        public string BagisTipi { get; set; }
        //Methods
        public override int Save()
        {
            try
            {
                DateTime minTarih = new DateTime(1987, 9, 1);
                if (BagisTarihi == default(DateTime) || BagisTarihi < minTarih)
                {
                    throw new ArgumentException("BagisTarihi boş olamaz ve 01.09.1987 tarihinden önce olamaz.");
                }
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

                this.Id = id;
                if (id > 0 && ProjeConstants.NBYS_SAVE_LOG)
                {
                    OlayKayit olayKayit = new OlayKayit();
                    olayKayit.GirisOlayKaydet(this, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    NakitBagisHareket item = new NakitBagisHareketService().GetById(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
                    }
                    if (isSuccess && ProjeConstants.NBYS_UPDATE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.GuncellemeOlayKaydet(this, item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
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
                    GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    NakitBagisHareket item = new NakitBagisHareketService().GetById(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
                    }
                    else isDeleted = false;
                    if (isDeleted && ProjeConstants.NBYS_DELETE_LOG)
                    {
                        OlayKayit olayKayit = new OlayKayit();
                        olayKayit.SilmeOlayKaydet(item, ProjeConstants.NBYS, ProjeConstants.NBYS_NAKITBAGISHAREKET);
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public override T Select<T>(int id)
        {
            return (T)Convert.ChangeType(new NakitBagisHareketService().GetById(id), typeof(T));
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this, extId) + " ;SELECT SCOPE_IDENTITY() ";

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetUpdateSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
                string sqlString = string.IsNullOrEmpty(extId)?genericEntity.GetQuery(this): genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetDeleteSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisHareket> genericEntity = new GenericEntity<NakitBagisHareket>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<NakitBagisHareket> SelectByArmaganId(int armaganId)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisHareket_Table 
                               WHERE ArmaganId= {0}", armaganId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisHareket> list = ToList<NakitBagisHareket>(dataTable);

            return (list);
        }
        public DataTable SelectByArmaganIdReturnDataTable(int armaganId)
        {
            string sqlString = string.Format(@"
                SELECT A.*, A.BagisMiktari BagisTutari, A.DovizCinsi,
                    B.BagisMiktari ArmaganTutari, B.DovizCinsi, B.Durum,
                    C.Armagan,
                    D.Banka
                FROM NakitBagisHareket_Table A
                INNER JOIN Armagan_Table B ON B.Id=A.ArmaganId
                INNER JOIN ArmaganTanim_Table C ON C.Id=B.ArmaganTanimId
                LEFT JOIN BankaTanim_Table D ON D.Id=A.BankaId
                WHERE A.ArmaganId= {0}
                ORDER BY A.BagisTarihi DESC
                ", armaganId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            return dataTable;
        }
        public string SelectByBagisciIdReturnJSon(string nakitBagisciId, ref int rowCount)
        {
            return new NakitBagisHareketService().GetDonorDetailJson(
                nakitBagisciId.ConvertToInt(), ref rowCount);
        }
        public List<NakitBagisHareket> SelectArmaganiOlmayanBagislarByBagisciId(int bagisciId)
        {
            return new NakitBagisHareketService().GetArmaganiOlmayanByBagisciId(bagisciId);
        }
        public List<NakitBagisHareket> SelectByBagisciId(int bagisciId)
        {
            return new NakitBagisHareketService().GetByBagisciId(bagisciId);
        }
        public NakitBagisHareket SelectByEkstreAktarmaId(int ekstreAktarmaId)
        {
            return new NakitBagisHareketService().GetByEkstreAktarmaId(ekstreAktarmaId);
        }
        public List<NakitBagisHareket> SelectByBagisciIdTarih(int nakitBagisciId, DateTime bastar,DateTime bittar)
        {
            return new NakitBagisHareketService().GetByBagisciIdTarih(nakitBagisciId, bastar, bittar);
        }
        public NakitBagisHareket SelectBagisByBagisciIdTarih(int nakitBagisciId, DateTime bastar)
        {
            return new NakitBagisHareketService().GetLastInYearByBagisciId(nakitBagisciId, bastar);
        }
        /**
         * returns Json
         * ***/
        public string SelectByBagisciId(string nakitBagisciId, ref int rowCount)
        {
            string sqlString = string.Format(@"
                    SELECT NakitBagisHareket_Table.BagisciId
				         ,BagisTarihi as BagisTarihi
                         ,FORMAT(BagisTarihi,'dd.MM.yyyy') BagisTarihiDDMMYY
						 ,Convert(nvarchar,replace (NakitBagisHareket_Table.BagisMiktari,'.',',')) as BagisMiktari
                         ,NakitBagisHareket_Table.DovizCinsi as DovizCinsi
						 ,ArmaganId
						 ,ArmaganTanim_Table.Armagan
                         ,BankaTanim_Table.Banka
						 ,Armagan_Table.Durum
						 ,Armagan_Table.Aciklama
                    FROM NakitBagisHareket_Table 
                    LEFT OUTER JOIN BankaTanim_Table ON BankaTanim_Table.Id= NakitBagisHareket_Table.BankaId
                    LEFT OUTER JOIN Armagan_Table ON Armagan_Table.Id= NakitBagisHareket_Table.ArmaganId 
                    LEFT OUTER JOIN ArmaganTanim_Table ON ArmaganTanim_Table.Id= Armagan_Table.ArmaganTanimId
                    WHERE NakitBagisHareket_Table.BagisciId={0}
					Order BY BagisTarihi DESC, BagisciId 
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }

            string json = ToJSON(dataTable);
            return json;
        }
        /**
         * returns Datatable **/
        public DataTable SelectByBagisciIdReturnDataTable(int nakitBagisciId)
        {
            string sqlString = string.Format(@"
                SELECT NakitBagisHareket_Table.BagisciId
                    ,FORMAT(BagisTarihi,'dd.MM.yyyy') BagisTarihi
					,Convert(nvarchar,replace (NakitBagisHareket_Table.BagisMiktari,'.',',')) as BagisMiktari
                    ,NakitBagisHareket_Table.DovizCinsi as DovizCinsi
					,ArmaganTanim_Table.Armagan
                    ,BankaTanim_Table.Banka
					,Armagan_Table.Aciklama
                FROM NakitBagisHareket_Table 
                    LEFT OUTER JOIN BankaTanim_Table ON BankaTanim_Table.Id= NakitBagisHareket_Table.BankaId
                    LEFT OUTER JOIN Armagan_Table ON Armagan_Table.Id= NakitBagisHareket_Table.ArmaganId 
                    LEFT OUTER JOIN ArmaganTanim_Table ON ArmaganTanim_Table.Id= Armagan_Table.ArmaganTanimId
                WHERE NakitBagisHareket_Table.BagisciId={0}
				ORDER BY NakitBagisHareket_Table.BagisTarihi DESC, BagisciId 
            ", nakitBagisciId);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;
        }
        public decimal GetSumBagisMiktariByNakitBagisciIdBetweenBasTarBitTar(DateTime basTar, DateTime bitTar, int nakitBagisciId)
        {
            return new NakitBagisHareketService().GetTotalByBagisciIdDateRange(basTar, bitTar, nakitBagisciId);
        }
        public List<NakitBagisHareket> SelectNakitBagisHareketByNakitBagisciIdBetweenBasTarBitTar(DateTime basTar, DateTime bitTar, int nakitBagisciId)
        {
            return new NakitBagisHareketService().GetByBagisciIdDateRange(basTar, bitTar, nakitBagisciId);
        }
        public DataTable SelectByFilter(string filter, DateTime? bagisTarihi)
        {
            return new NakitBagisHareketService().Search(filter, bagisTarihi);
        }
        public string SelectByDurumTarihReturnJson(string ay, string yil, int ilId)
        {
            return new NakitBagisHareketService().ListByDurumTarihJson(ay, yil, ilId);
        }

        public DataTable SelectByDurumTarihReturnDataTable(string ay, string yil, int ilId)
        {
            return new NakitBagisHareketService().ListByDurumTarih(ay, yil, ilId);
        }
        public DataTable SelectByIliAndYil(int ilId, DateTime tarih)
        {
            return new NakitBagisHareketService().GetProvinceYearSummary(ilId, tarih);
        }
        public DataTable SelectByBolgeTarih(int bolgeId, DateTime ilkTarih, DateTime sonTarih)
        {
            return new NakitBagisHareketService().GetByRegionDate(bolgeId, ilkTarih, sonTarih);
        }
        public DataTable SelectByNakitBagisciId(int nakitBagisciId)
        {
            return new NakitBagisHareketService().GetDonorDonationDetails(nakitBagisciId);
        }
        public DataTable SelectByBagisTarihiBankaId(DateTime bagisTarihi, string bankaGrup, string dovizCinsi = "")
        {
            return new NakitBagisHareketService().GetDailyTotalsByBank(bagisTarihi, bankaGrup, dovizCinsi);
        }
        public decimal SelectSumByBagisTarihi(DateTime bagisTarihi,string bankaGrup, string dovizCinsi)
        {
            return new NakitBagisHareketService().GetDailyTotal(bagisTarihi, bankaGrup, dovizCinsi);
        }
        public DataTable SelectTlBagisByTarihBankaGrup2(DateTime bagisTarihi, string bankaGrup2)
        {
            return new NakitBagisHareketService().GetDailyTlTotalByBankGroup2(bagisTarihi, bankaGrup2);
        }
        public DataTable SelectDovizBagisByTarihBankaGrup2(DateTime bastar,DateTime bittar, string bankaGrup2,string dovizCinsi)
        {
            return new NakitBagisHareketService().GetCurrencyDonationsByDateBankGroup2(
                bastar, bittar, bankaGrup2, dovizCinsi);
        }
        public List<string> SelectBankaGrup2ByTarihDovizCinsi(DateTime bastar, DateTime bittar, string dovizCinsi)
        {
            return new NakitBagisHareketService().GetBankGroup2ByDateCurrency(bastar, bittar, dovizCinsi);
        }
        public DataTable SelectDovizleBagisByTarihBankaId(DateTime bastar, DateTime bittar, string bankaGrup)
        {
            return new NakitBagisHareketService().GetCurrencyTotalsByDateBank(bastar, bittar, bankaGrup);
        }

        public string SelectIadeEdilenBagislarReturnJson(string ay, string yil)
        {
            return new NakitBagisHareketService().ListIadeEdilenBagislarJson(ay, yil);
        }

        public DataTable SelectIadeEdilenBagislarReturnDataTable(string ay, string yil)
        {
            return new NakitBagisHareketService().ListIadeEdilenBagislar(ay, yil);
        }

        public DataTable SelectNakitBagisRaporu(
            DateTime? bagisBasTarihi, DateTime? bagisBitTarihi,
            decimal? minBagisMiktari, decimal? maxBagisMiktari,
            int armaganId, DateTime? SonBagisTarihi,
            int ilId, int ilceId,
            bool? sag, bool? belgeIstemiyor, bool? ulasilamiyor, bool? tuzelKisi)
        {
            return new NakitBagisHareketService().GetDonationReport(
                bagisBasTarihi, bagisBitTarihi,
                minBagisMiktari, maxBagisMiktari,
                armaganId, SonBagisTarihi,
                ilId, ilceId,
                sag, belgeIstemiyor, ulasilamiyor, tuzelKisi);
        }
    }

}

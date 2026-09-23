using DAO.Ortak;
using DAO.Repositories.NBYS;
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
    public class NakitBagisci : ParentClass
    {

        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public long TCKimlikNo { get; set; }
        public int Ili { get; set; }
        public int Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public bool TuzelKisi { get; set; }
        public bool Sag { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Meslek { get; set; }
        public string Aciklama { get; set; }
        public bool Ulasilamiyor { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public bool DergiGonderilmesin { get; set; }
        public override T Select<T>(int id)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectById(id);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return (T)Convert.ChangeType(nakitBagisci, typeof(T));

        }
        public override int Save()
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi = DateTime.Now;
                Olusturan = UtilityHelper.GetCurrentUserName();
                SqlQuery query = genericEntity.GetQueryParametreli(this);
                int id = dao.Insert(query);

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
                    NakitBagisci item = Select<NakitBagisci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_UPDATE);
                        DegistirmeTarihi = DateTime.Now;
                        Degistiren = UtilityHelper.GetCurrentUserName();
                        SqlQuery query = genericEntity.GetQueryParametreli(this);
                        isSuccess = dao.Update2Db(query);
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
                    GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_DELETE);
                    SqlQuery query = genericEntity.GetQueryParametreli(this);
                    NakitBagisci item = Select<NakitBagisci>(Id);
                    if (item != null)
                    {
                        isDeleted = dao.DeleteFromDb(query, "");
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
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectAll();
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);

            return (List<T>)Convert.ChangeType(list, typeof(List<T>));
        }

        public string GetSelectSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public string GetInsertSQL(string extId)
        {
            try
            {
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_INSERT);
                OlusturmaTarihi=DateTime.Now;
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
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_UPDATE);
                DegistirmeTarihi = DateTime.Now;
                Degistiren = UtilityHelper.GetCurrentUserName();
                string sqlString = genericEntity.GetQuery(this, extId);

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
                GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_DELETE);
                string sqlString = genericEntity.GetQuery(this, extId);

                return sqlString;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public DataTable SelectBagisciGroupByBagisAdediReturnDataTable(decimal bronzMadalyaMiktari)
        {
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectBagisciGroupByBagisAdedi(
                bronzMadalyaMiktari,
                ProjeConstants.NAKITBAGISCI_BILINMEYEN,
                ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ReturnQuotedValue().ToString(),
                ProjeConstants.COKBAGISYAPAN_SONBAGISI_KAC_AY_ONCE_YAPTI);
        }
        public NakitBagisci SelectByTcKimlikno(long tcKimlikno)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByTcKimlikno(tcKimlikno);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        /// <summary>
        /// NakitBagisHareket_Table'da Bagisi olmayan Bagisçiyi bulur
        /// </summary>
        /// <param name="bagisciId"></param>
        /// <returns></returns>
        public NakitBagisci SelectBagisiOlmayanBagisciById(int bagisciId)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectBagisiOlmayanBagisciById(bagisciId);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByAdAndTelefon(string adi, string telefon)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByAdAndTelefon(adi, telefon);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByTelefon(string telefon)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByTelefon(telefon);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByEposta(string eposta)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByEposta(eposta);
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public DataTable SelectByAd(string adi)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            return repository.SelectByAd(adi);

        }
        public string SelectByIl(int pIlId, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIl(ilId);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihi(int pIlId, string bTar, string sTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIlBagisTarihi(ilId, bTar, sTar);

            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihiYeni(int pIlId, string basTar, string sonTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            int? ilId = pIlId > ProjeConstants.IL_HEPSI ? (int?)pIlId : null;
            NakitBagisciRepository repository = new NakitBagisciRepository();
            DataTable dataTable = repository.SelectByIlBagisTarihiYeni(ilId, basTar, sonTar);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectByBagisTarihiBagisSayisi(DateTime basTar, DateTime bitTar, int bagisciSayisi, ref int rowCount, bool belgeIsitemeyenlerHaric,
            bool adresiBosOlanlarHaric, bool postadanIadelerHaric, bool dergiGonderilmesinlerHaric, bool ulasilamayanlarHaric, bool sadeceYeniBagiscilar)
        {
            string TLBasTar = ProjeConstants.TL_GECIS_TARIHI.ReturnTRDateFormat();
            string basTarEksi1GunStr = basTar.AddDays(-1).ReturnTRDateFormat();
            //string basBitTarStr = string.Format("AND B.Tarih BETWEEN {0} AND {1} ", basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat());
            string sadeceYeniBagiscilarstr = string.Format(sadeceYeniBagiscilar ? " AND C.Tarih < {0}" : "", basTar.ReturnTRDateFormat());

            string adresiBosOlanlarStr = string.Empty;
            if (adresiBosOlanlarHaric)
            {
                adresiBosOlanlarStr = " AND ISNULL(LTRIM(RTRIM(Adres)), '') != '' ";
            }
            string belgeIstemeyenlerHaricStr = string.Empty;
            string belgeIstemeyenlerHaricAStr = string.Empty;
            if (belgeIsitemeyenlerHaric)
            {
                belgeIstemeyenlerHaricStr = " AND Z.BelgeIstemiyor=0";
                belgeIstemeyenlerHaricAStr = " AND A.BelgeIstemiyor=0";
            }
            string postadanIadelerHaricBStr = string.Empty;
            string postadanIadelerHaricFStr = string.Empty;
            if (postadanIadelerHaric)
            {
                postadanIadelerHaricBStr = " AND B.Durum!=" + ProjeConstants.DURUM_PARAIADE.ReturnQuotedValue() +
                    " AND B.Durum!=" + ProjeConstants.DURUM_DAHAONCEIADE.ReturnQuotedValue();
                postadanIadelerHaricFStr = " AND F.Durum!=" + ProjeConstants.DURUM_PARAIADE.ReturnQuotedValue() +
                    " AND F.Durum!=" + ProjeConstants.DURUM_DAHAONCEIADE.ReturnQuotedValue();
            }
            string dergiGonderilmesinlerHaricStr = string.Empty;
            string dergiGonderilmesinlerHaricAStr = string.Empty;
            if (dergiGonderilmesinlerHaric)
            {
                dergiGonderilmesinlerHaricStr = " AND Z.DergiGonderilmesin=0 ";
                dergiGonderilmesinlerHaricAStr = " AND A.DergiGonderilmesin=0 ";
            }
            string ulasilamayanlarHaricStr = string.Empty;
            string ulasilamayanlarHaricAStr = string.Empty;
            if (ulasilamayanlarHaric)
            {
                ulasilamayanlarHaricStr = " AND Z.Ulasilamiyor=0 ";
                ulasilamayanlarHaricAStr = " AND A.Ulasilamiyor=0 ";
            }
            string sqlString = string.Empty;
            if (sadeceYeniBagiscilar)
            {
                sqlString = string.Format(@"
                SELECT TOP {0} SUM(Y.BagisMiktari) BagisMiktariDecimal, CONVERT(nvarchar, REPLACE(SUM(Y.BagisMiktari),'.',',')) BagisMiktari,
	                NakitBagisciId, Adi,Adres,Telefon1,Telefon2, Ilcesi,  Ili,DergiGonderilmesin, TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                FROM
                (
                SELECT  
	                A.Id NakitBagisciId, A.Adi,A.Adres,A.Telefon1,A.Telefon2,G.IlceAdi Ilcesi, F.IlAdi Ili, A.DergiGonderilmesin, A.TuzelKisi , BelgeIstemiyor ,Ulasilamiyor
                FROM 
                    NakitBagisci_Table A
	                    INNER JOIN Armagan_Table B ON B.BagisciId = A.Id
                        LEFT JOIN Il_Table F ON F.Id=A.Ili
                        LEFT JOIN Ilce_Table G ON G.Id=A.Ilcesi AND G.IlId=F.Id
                WHERE 
                    B.Tarih BETWEEN 
                        {1} AND {2}
                    {7} --Armagan tablosunda Durum=  daha önce iade edildi olanlar haric
                GROUP BY A.Id,A.Adi,A.Adres,A.Ili,F.IlAdi,G.IlceAdi,A.Telefon1,A.Telefon2, A.DergiGonderilmesin,A.TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                EXCEPT				
                SELECT	E.Id NakitBagisciId, E.Adi,E.Adres,E.Telefon1,E.Telefon2,J.IlceAdi Ilcesi, H.IlAdi Ili,E.DergiGonderilmesin, E.TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                FROM NakitBagisci_Table E
	                INNER JOIN Armagan_Table F ON F.BagisciId = E.Id
	                LEFT JOIN Armagan_Table G ON F.BagisciId=G.BagisciId 
	                LEFT JOIN Il_Table H ON H.Id=e.Ili
	                LEFT JOIN Ilce_Table J ON J.Id=E.Ilcesi AND J.IlId=H.Id
                WHERE 
                    F.Tarih BETWEEN 
                        {1} AND {2} 
	                AND G.Tarih BETWEEN 
                        {3} AND {4}
                    {8} --Armagan tablosunda Durum=  daha önce iade edildi olanlar haric
	                ) Z
	                INNER JOIN Armagan_Table Y ON Z.NakitBagisciId=Y.BagisciId
                WHERE Y.Tarih > {4}
                    {5} --belge istemiyor
                    {6} --adresi bos olanlar
                    {9} --dergi gonderilmesinler
                    {10} --ulasilamiyor olanlar haric
                GROUP BY NakitBagisciId,Adi,Adres,Ili,Ilcesi,Telefon1,Telefon2, DergiGonderilmesin,TuzelKisi,BelgeIstemiyor,Ulasilamiyor
                ORDER BY BagisMiktariDecimal DESC, Z.NakitBagisciId DESC
                ", bagisciSayisi, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat(), TLBasTar, basTarEksi1GunStr,
                belgeIstemeyenlerHaricStr, adresiBosOlanlarStr, postadanIadelerHaricBStr, postadanIadelerHaricFStr,
                dergiGonderilmesinlerHaricStr, ulasilamayanlarHaricStr);
            }
            else
            {
                sqlString = string.Format(@"
                    SELECT TOP {0} SUM(B.BagisMiktari)BagisMiktariDecimal, CONVERT(nvarchar, REPLACE(SUM(B.BagisMiktari), '.', ',')) BagisMiktari,
                        A.Id NakitBagisciId, A.Adi, A.Adres, A.Telefon1, A.Telefon2, G.IlceAdi Ilcesi, F.IlAdi Ili, A.DergiGonderilmesin, A.TuzelKisi, BelgeIstemiyor, Ulasilamiyor
                    FROM
                        NakitBagisci_Table A
                            INNER JOIN Armagan_Table B ON B.BagisciId = A.Id
                            LEFT JOIN Il_Table F ON F.Id = A.Ili
                            LEFT JOIN Ilce_Table G ON G.Id = A.Ilcesi AND G.IlId=F.Id
                    WHERE
                        B.Tarih BETWEEN
                            {1} AND {2}
                         {3} --durum
                         {4}--belge istemiyor
                         {5}--adresi bos olanlar
                         {6}--dergi gonderilmesinler
                         {7}--ulasilamayanlar haric
                    GROUP BY A.Id, A.Adi, A.Adres, A.Ili, F.IlAdi, G.IlceAdi, A.Telefon1, A.Telefon2, A.DergiGonderilmesin, A.TuzelKisi, BelgeIstemiyor, Ulasilamiyor
                    ORDER BY BagisMiktariDecimal DESC, A.Id DESC
                    ", bagisciSayisi, basTar.ReturnTRDateFormat(), bitTar.ReturnTRDateFormat(), postadanIadelerHaricBStr,
                belgeIstemeyenlerHaricAStr, adresiBosOlanlarStr,
                dergiGonderilmesinlerHaricAStr, ulasilamayanlarHaricAStr);
            }
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {

                throw;
            }
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            return dataTable;
        }
        public DataTable SelectByFilterReturnDataTable(string filter, int eksiId)
        {
            NakitBagisciRepository repository = new NakitBagisciRepository();
            return repository.SelectByFilter(filter, eksiId);
        }
        public string SelectByFilter(string filter, int eksiId)
        {
            DataTable dataTable = SelectByFilterReturnDataTable(filter, eksiId);
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectSecilmemisKatilimcilarByFaaliyetIdReturnDT()
        {
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectSecilmemisKatilimcilar(
                DateTime.Today.AddYears(-2),
                ProjeConstants.NAKITBAGISCI_SORGUBAGISTUTARI);
        }
        public DataTable SelectDuzenliBagisci(DateTime bastar,DateTime bittar, string durum)
        {
            bool sadeceBelgeOlusturulmadi = durum.Equals(ProjeConstants.DURUM_BELGEOLUSTURULMADI);
            bool durumFiltrele = !sadeceBelgeOlusturulmadi && !durum.Equals(ProjeConstants.HEPSI);
            NakitBagisciReportRepository repository = new NakitBagisciReportRepository();
            return repository.SelectDuzenliBagisci(
                bastar,
                bittar,
                sadeceBelgeOlusturulmadi,
                durumFiltrele,
                durum);
        }
    }
}

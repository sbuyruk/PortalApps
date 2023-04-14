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
        public string Ili { get; set; }
        public string Ilcesi { get; set; }
        public string Adres { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public bool TuzelKisi { get; set; }
        public bool Sag { get; set; }
        public string Eposta { get; set; }
        public string PostaKodu { get; set; }
        public string Aciklama { get; set; }
        public bool Ulasilamiyor { get; set; }
        public bool BelgeIstemiyor { get; set; }
        public bool DergiGonderilmesin { get; set; }
        public override T Select<T>(int id)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisci_Table 
                               WHERE  Id={0}", id);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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
                    NakitBagisci item = Select<NakitBagisci>(Id);
                    if (Id != 0)
                    {
                        GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_UPDATE);
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
                    GenericEntity<NakitBagisci> genericEntity = new GenericEntity<NakitBagisci>(ProjeConstants.SQL_DELETE);
                    string sqlString = genericEntity.GetQuery(this);
                    NakitBagisci item = Select<NakitBagisci>(Id);
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
                throw ex;
            }
        }
        public override List<T> SelectAll<T>()
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisci_Table");

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
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

                throw ex;
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

                throw ex;
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

                throw ex;
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

                throw ex;
            }
        }
        public DataTable SelectBagisciGroupByBagisAdediReturnDataTable(decimal bronzMadalyaMiktari)
        {
            string sqlString = string.Format(@"
                SELECT A.BagisciId NakitBagisciId
                    ,COUNT(A.Id) Adet, SUM(A.BagisMiktari) Toplam, MAX(A.BagisTarihi) SonBagisTarihi
                    ,B.Adi, B.Soyadi, B.TuzelKisi
	            FROM NakitBagisHareket_Table A
	                LEFT JOIN NakitBagisci_Table B ON B.Id=A.BagisciId
                    LEFT JOIN Armagan_Table C ON C.Id=A.ArmaganId AND C.ArmaganTanimId IN ({0})
                WHERE B.Id IS NOT NULL AND C.Id is NULL AND B.Adi!={1}
                    AND A.BagisTarihi > {2}
                GROUP BY A.BagisciId,B.Adi, B.Soyadi, B.TuzelKisi
                HAVING SUM(A.BagisMiktari) >= {3} AND COUNT(A.Id) >= 5 AND MAX(A.BagisTarihi) > DATEADD(MONTH, -6, GETDATE())
                ORDER BY COUNT(A.Id) Desc, SUM(A.BagisMiktari)", ProjeConstants.ARMAGAN_ALTINID +","+ ProjeConstants.ARMAGAN_GUMUSID + "," + ProjeConstants.ARMAGAN_BRONZID, 
                ProjeConstants.NAKITBAGISCI_BILINMEYEN.ReturnQuotedValue(),
                ProjeConstants.COKBAGISYAPAN_BASLAMATARIHI.ReturnQuotedValue(), bronzMadalyaMiktari.ToString().Replace(",",".").ReturnQuotedValue());
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
        public NakitBagisci SelectByTcKimlikno(long tcKimlikno)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisci_Table 
                               WHERE  TCKimlikNo!=0 AND TCKimlikNo={0}", tcKimlikno);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        /// <summary>
        /// NakitBagisHareket_Table'da Bağışı olmayan Bağışçıyı bulur
        /// </summary>
        /// <param name="bagisciId"></param>
        /// <returns></returns>
        public NakitBagisci SelectBagisiOlmayanBagisciById(int bagisciId)
        {
            string sqlString = string.Format(@"
                SELECT A.Id, A.Adi, B.BagisTarihi,B.BagisMiktari
                FROM NakitBagisci_Table A
                LEFT JOIN NakitBagisHareket_Table B ON B.BagisciId=A.Id
                WHERE A.Id={0}
                    AND  B.BagisciId IS NULL 
                ORDER BY BagisTarihi DESC ", bagisciId);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public NakitBagisci SelectByAdAndTelefon(string adi, string telefon)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisci_Table 
                               WHERE  Adi={0} AND (Telefon1={1} OR Telefon2={1})", adi.ReturnQuotedValue(), telefon.ReturnQuotedValue());
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            NakitBagisci nakitBagisci = new NakitBagisci();
            nakitBagisci = list.FirstOrDefault();
            return nakitBagisci;

        }
        public DataTable SelectByAd(string adi)
        {
            string sqlString = string.Format(@"SELECT *
                               FROM NakitBagisci_Table 
                               WHERE  Adi LIKE '%{0}%' ", adi);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");

            return dataTable;

        }
        public List<NakitBagisci> SelectBagisciByEkstreAktarmaId(int ekstreAktarmaId, string telefon1, string telefon2)
        {
            string telefon1Str = string.IsNullOrEmpty(telefon1) ? "" : string.Format(" AND (A.Telefon1 !={0} AND A.Telefon2 !={0})", telefon1.ReturnQuotedValue());
            string telefon2Str = string.IsNullOrEmpty(telefon2) ? "" : string.Format(" AND (A.Telefon1 !={0} AND A.Telefon2 !={0})", telefon2.ReturnQuotedValue());
            string sqlString = string.Format(@"
                SELECT * from NakitBagisci_Table A
                INNER JOIN EkstreAktarma_Table B ON LTRIM(RTRIM(UPPER(B.Adi))) =LTRIM(RTRIM(UPPER(A.Adi)))
                WHERE B.Id= {0} AND A.TCKimlikNo=0 
                    {1}
                    {2} "
                , ekstreAktarmaId.ReturnQuotedValue(), telefon1Str, telefon2Str);
            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);

            return list;

        }
        public string SelectByIl(int pIlId, ref List<NakitBagisci> list, ref int rowCount)
        {
            string ilStr = string.Empty;
            if (pIlId < ProjeConstants.IL_HEPSI)
            {
                ilStr = string.Format(" WHERE Ili='{0}'", pIlId);
            }

            //string sqlString = string.Format(@"SELECT  *
            //                                  FROM NakitBagisci_Table {0}", ilStr);

            string sqlString = string.Format(@"
                SELECT A.Id NakitBagisciId
	                ,ROW_NUMBER() OVER(ORDER BY A.Id) AS Sirano
                    ,Adi
                    ,Soyadi
                    ,TCKimlikNo
                    ,B.IlAdi Ili
                    ,C.IlceAdi Ilcesi
                    ,Adres
                    ,Telefon1
                    ,Telefon2
                    ,TuzelKisi
                    ,OlusturmaTarihi
                    ,Olusturan
                    ,DegistirmeTarihi
                    ,Degistiren
                    ,Sag
                    ,Eposta
                    ,PostaKodu
                    ,Aciklama
                    ,Ulasilamiyor,BelgeIstemiyor
                FROM NakitBagisci_Table A 
	                LEFT OUTER JOIN Il_Table B ON B.Id= A.Ili 
	                LEFT OUTER JOIN Ilce_Table C ON C.Id= A.Ilcesi AND C.IlId=B.Id
                {0}", ilStr);

            DataTable dataTable = dao.SelectFromDb(sqlString, "");
            list = ToList<NakitBagisci>(dataTable);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihi(int pIlId, string bTar, string sTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            string ilStr = string.Empty;
            if (pIlId < ProjeConstants.IL_HEPSI)
            {
                ilStr = string.Format(" AND A.Ili='{0}'", pIlId);
            }
            string sqlString = string.Format(@"
                SELECT DISTINCT(A.Id) NakitBagisciId
                    ,Adi,Soyadi,TCKimlikNo,Adres,Telefon1,Telefon2,TuzelKisi
                    ,Ulasilamiyor,BelgeIstemiyor,Sag,Eposta,PostaKodu,A.Aciklama
                    ,B.IlAdi Ili
                    ,C.IlceAdi Ilcesi
                FROM NakitBagisci_Table A 
                    LEFT JOIN Il_Table B ON B.Id= A.Ili 
                    LEFT JOIN Ilce_Table C ON C.Id= A.Ilcesi AND C.IlId=B.Id
                    INNER JOIN NakitBagisHareket_Table D on D.BagisciId=A.Id
                WHERE D.BagisTarihi BETWEEN {0} AND {1}
                {2}
                ORDER BY NakitBagisciId ", bTar, sTar, ilStr);
            DataTable dataTable;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {

                throw e;
            }
            //List<NakitBagisci> 
            list = ToList<NakitBagisci>(dataTable);
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            string json = ToJSON(dataTable);
            return json;
        }
        public string SelectByIlBagisTarihiYeni(int pIlId, string basTar, string sonTar, ref List<NakitBagisci> list, ref int rowCount)
        {
            string ilStr = string.Empty;
            if (pIlId < ProjeConstants.IL_HEPSI)
            {
                ilStr = string.Format(" AND N.Ili='{0}'", pIlId);
            }

            //string sqlString = string.Format(@"select * from NakitBagisci_Table
            //                                    WHERE Id  IN (SELECT BagisciId from NakitBagisHareket_Table WHERE BagisTarihi BETWEEN {0} and {1})
            //                                    AND Id NOT IN (SELECT BagisciId from NakitBagisHareket_Table WHERE BagisTarihi <{0})
            //                                    {2}
            //                                ORDER BY Id ", basTar,sonTar, ilStr);
            string sqlString = string.Format(@"
                SELECT distinct(N.Id) NakitBagisciId
                    ,Adi
                    ,Soyadi
                    ,TCKimlikNo
                    ,Il_Table.IlAdi Ili
                    ,Ilce_Table.IlceAdi Ilcesi
                    ,Adres
                    ,Telefon1
                    ,Telefon2
                    ,TuzelKisi
                    ,N.OlusturmaTarihi
                    ,N.Olusturan
                    ,N.DegistirmeTarihi
                    ,N.Degistiren
                    ,Sag
                    ,Eposta
                    ,PostaKodu
                    ,N.Aciklama
                    ,Ulasilamiyor,BelgeIstemiyor
                FROM NakitBagisci_Table N
	                LEFT OUTER JOIN Il_Table ON Il_Table.Id= N.Ili 
	                LEFT OUTER JOIN Ilce_Table ON Ilce_Table.Id= N.Ilcesi AND Ilce_Table.IlId= Il_Table.Id
                WHERE N.Id  IN (SELECT BagisciId from NakitBagisHareket_Table WHERE BagisTarihi BETWEEN {0} AND {1}) 
                AND N.Id NOT IN 
                (SELECT A.BagisciId
                    FROM NakitBagisHareket_Table B,NakitBagisHareket_Table A 
                    WHERE A.[BagisTarihi] BETWEEN {0} AND {1} AND B.[BagisTarihi] < {0} AND A.BagisciId=B.BagisciId 
                )
                {2}
                ORDER BY N.Id ", basTar, sonTar, ilStr);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {

                throw e;
            }
            //List<NakitBagisci> 
            list = ToList<NakitBagisci>(dataTable);
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
                postadanIadelerHaricBStr = " AND B.Durum!=" + ProjeConstants.DURUM_IADE.ReturnQuotedValue() +
                    " AND B.Durum!=" + ProjeConstants.DURUM_DAHAONCEIADE.ReturnQuotedValue();
                postadanIadelerHaricFStr = " AND F.Durum!=" + ProjeConstants.DURUM_IADE.ReturnQuotedValue() +
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
                    {6} --adresi boş olanlar
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
                         {5}--adresi boş olanlar
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

                throw e;
            }
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            return dataTable;
        }
        public DataTable SelectByFilterReturnDataTable(string filter, int eksiId)
        {
            string sqlString = string.Format(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM NakitBagisci_Table N 
                    LEFT OUTER JOIN Il_Table A ON A.Id= N.Ili 
                    LEFT OUTER JOIN Ilce_Table B ON B.Id= N.Ilcesi AND B.IlId=A.Id
                    ---INNER JOIN NakitBagisHareket_Table C on C.BagisciId=N.Id
                WHERE N.Id!= {0} AND N.Adi like '%{1}%'
	                OR N.TCKimlikNo like '%{1}%'
	                OR N.Telefon1 like '%{1}%'
	                OR N.Adres like '%{1}%'
                ORDER BY NakitBagisciId ", eksiId, filter);
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
        public string SelectByFilter(string filter, int eksiId)
        {
            string ilStr = string.Empty;

            string sqlString = string.Format(@"
                SELECT distinct(N.Id) NakitBagisciId, N.Adi,Soyadi, TCKimlikNo, A.IlAdi Ili ,B.IlceAdi Ilcesi,Adres,
	                Telefon1,Telefon2, Telefon1 + IIF(ISNULL(Telefon1,'')!='' AND ISNULL(Telefon2,'')!='',' - ','') + Telefon2 Telefon, 
                    N.OlusturmaTarihi  ,N.DegistirmeTarihi,N.Degistiren,Sag,Eposta ,PostaKodu, Ulasilamiyor, BelgeIstemiyor                                            
                FROM NakitBagisci_Table N 
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
                ORDER BY NakitBagisciId ", eksiId, filter);
            DataTable dataTable = null;
            try
            {
                dataTable = dao.SelectFromDb(sqlString, "");
            }
            catch (Exception e)
            {
                throw e;
            }

            //if (dataTable != null)
            //{
            //    List<NakitBagisci> list = ToList<NakitBagisci>(dataTable);
            //}
            string json = ToJSON(dataTable);
            return json;
        }
        public DataTable SelectSecilmemisKatilimcilarByRandevuIdReturnDT(int randevuId)
        {
            string randevuIdStr = randevuId > 0 ? string.Format(@" 
                AND A.Id Not in (SELECT KatilimciId FROM RandevuKatilim_Table WHERE KatilimciTipi={0} AND RandevuId={1})", ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT, randevuId) : string.Empty;
            string tarihStr = DateTime.Today.AddYears(-2).ReturnTRDateFormat();
            string sqlString = string.Format(@"              
                SELECT DISTINCT(A.Id) KatilimciId, A.Adi, A.Soyadi, {0} KatilimciTipi,
					A.Adres,A.Telefon1 Telefon,A.Sag,D.IlceAdi Ilce,C.IlAdi Il,MAX(B.BagisMiktari)
                FROM NakitBagisci_Table A
					INNER JOIN Armagan_Table B ON B.BagisciId =A.Id 
						AND Tarih>{1} AND BagisMiktari >= {2}
                    LEFT JOIN Il_Table C ON C.Id=A.Ili 
					LEFT JOIN Ilce_Table D ON D.Id=A.Ilcesi AND D.IlId=A.Ili 
				WHERE A.Sag=1 AND A.TuzelKisi=0 AND A.Adi IS NOT NULL AND A.Adi!='' AND A.Adi NOT Like '%BİLİNMEYEN%'
                    {3}
				GROUP BY  A.Id , A.Adi, A.Soyadi,
					A.Adres,A.Telefon1,A.Sag,D.IlceAdi,C.IlAdi
                ORDER BY A.Id
            ", ProjeConstants.RANDEVU_KATILIMCI_NAKITBAGISCI_INT, tarihStr, ProjeConstants.NAKITBAGISCI_SORGUBAGISTUTARI, randevuIdStr);
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

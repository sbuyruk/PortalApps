using DAO.Ortak;
using System;
using System.Data;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace DAO.Repositories.TBYS
{
    public class KiraSozlesmeRepository
    {
        private readonly DbClass db;

        public KiraSozlesmeRepository() : this(new DbClass()) { }

        public KiraSozlesmeRepository(DbClass db)
        {
            if (db == null) throw new ArgumentNullException("db");
            this.db = db;
        }

        public DataTable SelectById(int id)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE Id=@Id");
            query.AddParameter("@Id", id);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectAll()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }

        public DataTable SelectAllActive()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE Aktif=1
                ORDER BY CASE WHEN DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(DosyaNo,999999)"), "");
        }
        public DataTable SelectListReturnDataTable(int kiraciId, int aktif, int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND S.BolgeId={0} ", bolgeId);
            string aktifStr = aktif == ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT ? "" : " AND S.Aktif = " + aktif;
            string kiraciStr = kiraciId < 1 ? "" : " AND S.KiraciId=" + kiraciId;
            string sql = string.Format(@"
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, H.KisaAdi Bolge,S.BolgeId, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,ArtisAyi,
                    IlkSozlesmeTar,SozBasTar, SozBitTar,DATEDIFF(dd,S.SozBasTar,GETDATE()) SozlesmeBasladi,
                    S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi,
                    S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi,
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
                    S.TeminatAciklama, S.TeminatOdemeTarihi,
                    K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar, S.Aktif, S.SozlesmePDFDosyasi,K.KiralamaAmaci
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    INNER JOIN Bolge_Table H ON H.Id=S.BolgeId
                WHERE 1 > 0
                {0} {1} {2}
                ORDER BY DosyaNo, S.Id", aktifStr, kiraciStr, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectList(int kiraciId, int aktif, int bolgeId)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND S.BolgeId={0} ", bolgeId);
            string aktifStr = aktif == ProjeConstants.KIRASOZLESME_AKTIF_HEPSI_INT ? "" : " AND S.Aktif = " + aktif;
            string kiraciStr = kiraciId < 1 ? "" : " AND S.KiraciId=" + kiraciId;
            string sql = string.Format(@"
                SELECT S.*
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    INNER JOIN Bolge_Table H ON H.Id=S.BolgeId
                WHERE 1 > 0
                {0} {1} {2}
                ORDER BY DosyaNo, S.Id", aktifStr, kiraciStr, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectRentIncreaseDue(int bolgeId, string tarih)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND S.BolgeId={0} ", bolgeId);
            string sql = string.Format(@"
                SELECT S.Id KiraSozlesmeId, K.Adres, K.Ili,K.Ilcesi,K.Semt,H.KisaAdi Bolge,S.BolgeId,
                    IlkSozlesmeTar,SozBasTar,SozBitTar,S.ArtisAyi,
                    S.KiraBedeli KiraBedeli, S.Aktif, S.OdemeSekli,
                    S.KiraciId, K.Adi KiraciAdi, K.Soyadi KiraciSoyadi,K.KiralamaAmaci
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    INNER JOIN Bolge_Table H ON H.Id=S.BolgeId
                WHERE 1>0 AND S.Aktif = 1
                    {0}
                    AND (CONVERT(int,ArtisAyi) = DATEPART(MM,{1}) AND YEAR(SozBitTar)=DATEPART(YYYY,{1}))
                ORDER BY S.BolgeId, SozBitTar", bolgeStr, tarih);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectRealizedRentIncreases(int bolgeId, string baslangic, string bitis)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND A.BolgeId={0} ", bolgeId);
            string sql = string.Format(@"
                SELECT A.Id KiraSozlesmeId, C.Adres, C.Ili,C.Ilcesi,C.Semt,E.KisaAdi Bolge,A.BolgeId,
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar,
                    B.SozBasTar OncekiSozBasTar, B.SozBitTar OncekiSozBitTar,
                    A.ArtisAyi, A.KiraBedeli, B.KiraBedeli OncekiKiraBedeli,
                    A.Aktif, A.OdemeSekli, A.KiraciId, C.Adi KiraciAdi, C.Soyadi KiraciSoyadi,C.KiralamaAmaci,
                    A.OdemeSekli
                FROM KiraSozlesme_Table A
                    LEFT JOIN KiraSozlesme_Table B ON A.KiraciId=B.KiraciId AND B.SozBitTar=A.SozBasTar AND B.Aktif=0
                    LEFT JOIN Kiraci_Table C on C.Id= A.KiraciId
                    LEFT JOIN Bolge_Table E ON E.Id=A.BolgeId
                WHERE A.Aktif=1 AND A.SozBasTar >={0} AND A.SozBasTar<{1}
                    {2}
                ORDER BY A.BolgeId, SozBitTar DESC", baslangic, bitis, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectListByYear(int yil)
        {
            string sql = string.Format(@"
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, S.Bolge, S.BolgeId, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,
                    FORMAT(S.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
                    FORMAT(S.SozBasTar, 'dd/MM/yyyy') SozBasTar,
                    FORMAT(S.SozBitTar, 'dd/MM/yyyy') SozBitTar,
                    S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi,
                    S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi,
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
                    S.TeminatAciklama, S.TeminatOdemeTarihi,
                    K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE YEAR(S.IlkSozlesmeTar)={0}
                ORDER BY DosyaNo", yil);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectCompletedListByYear(int yil)
        {
            string sql = string.Format(@"
                SELECT S.Id KiraSozlesmeId, S.DosyaNo, S.Bolge, S.BolgeId, S.KiraciId, S.Aktif, F.BolumNo, D.Adres, D.Ili,D.Ilcesi,
                    FORMAT(S.IlkSozlesmeTar,'dd/MM/yyyy') IlkSozlesmeTar,
                    FORMAT(S.SozBasTar, 'dd/MM/yyyy') SozBasTar,
                    FORMAT(S.SozBitTar, 'dd/MM/yyyy') SozBitTar,
                    S.KiraBedeli KiraBedeli,S.OdemeSekli,S.TaksitSayisi, S.KefilAdiSoyadi,
                    S.KefilTCKimlikNo, S.KefilAdresi, S.KefilTel,S.TeminatCinsi,
                    S.TeminatTutari, S.OdenenTeminatTutari, S.IadeTeminatTutari, S.KalanTeminatTutari,
                    S.TeminatAciklama, S.TeminatOdemeTarihi,
                    K.Adi KiraciAdi, K.Soyadi KiraciSoyadi, S.SozlesmeDurumu, S.DurumDegismeTar
                FROM KiraSozlesme_Table S
                    LEFT JOIN Kiraci_Table K on K.Id= S.KiraciId
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table C On C.SozlesmeId=S.Id
                    LEFT OUTER JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                    LEFT OUTER JOIN BagimsizBolum_Table F ON F.TasinmazId=D.Id AND F.Id=C.BolumId
                    LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                WHERE YEAR(S.DurumDegismeTar)={0}
                    AND S.SozlesmeDurumu in ({1},{2})
                ORDER BY DosyaNo", yil, ProjeConstants.KIRASOZLESME_DURUMU_BITTI.ReturnQuotedValue(), ProjeConstants.KIRASOZLESME_DURUMU_FESIH.ReturnQuotedValue());
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectActiveByKiraciId(int kiraciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE Aktif=1 AND KiraciId=@KiraciId ORDER BY SozBastar DESC");
            query.AddParameter("@KiraciId", kiraciId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByKiraciId(int kiraciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE KiraciId=@KiraciId");
            query.AddParameter("@KiraciId", kiraciId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectAllByKiraciId(int kiraciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE KiraciId=@KiraciId ORDER BY SozBasTar DESC, DosyaNo");
            query.AddParameter("@KiraciId", kiraciId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectByKiraciIdAndDate(int kiraciId, string tarih)
        {
            string sql = string.Format("SELECT * FROM KiraSozlesme_Table WHERE KiraciId={0} AND ({1} >= SozBasTar AND {1} < SozBitTar)", kiraciId.ReturnQuotedValue(), tarih);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectCompletedByKiraciId(int kiraciId)
        {
            SqlQuery query = new SqlQuery("SELECT * FROM KiraSozlesme_Table WHERE Aktif=0 AND KiraciId=@KiraciId ORDER BY SozBastar DESC");
            query.AddParameter("@KiraciId", kiraciId);
            return db.SelectFromDb(query, "");
        }
        public DataTable SelectNearestByKiraciIdAndDate(int kiraciId, string tarih)
        {
            string sql = string.Format("SELECT * FROM KiraSozlesme_Table WHERE KiraciId={0} AND SozBitTar >= {1} ORDER BY SozBasTar", kiraciId.ReturnQuotedValue(), tarih);
            DataTable table = db.SelectFromDb(sql, "");
            if (table != null) return table;
            sql = string.Format("SELECT * FROM KiraSozlesme_Table WHERE KiraciId={0} AND SozBasTar < {1} ORDER BY SozBasTar DESC", kiraciId.ReturnQuotedValue(), tarih);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectByTasinmazId(int tasinmazId)
        {
            string sql = string.Format(@"
                SELECT A.*
                FROM KiraSozlesme_Table A
                    LEFT OUTER JOIN SozlesmeTasinmaz_Table B ON B.SozlesmeId=A.Id
                    LEFT OUTER JOIN Bagis_Table C ON C.TasinmazId=B.TasinmazId
                WHERE A.Aktif=1 AND B.TasinmazId={0}
                ORDER BY B.TasinmazId", tasinmazId.ReturnQuotedValue());
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectAddressById(int sozlesmeId)
        {
            string sql = string.Format(@"
                SELECT A.Id,C.Adres,C.Ili,C.Ilcesi
                FROM KiraSozlesme_Table A
                    LEFT JOIN SozlesmeTasinmaz_Table B On B.SozlesmeId=A.Id
                    LEFT JOIN Tasinmaz_Table C On C.Id=B.TasinmazId
                WHERE A.Id= {0}", sozlesmeId.ReturnQuotedValue());
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectSecurityDepositSummaryByRegionAndPurpose(int bolgeId, string kiralamaAmaci)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND A.BolgeId={0} ", bolgeId);
            string purposeStr = string.IsNullOrEmpty(kiralamaAmaci) || kiralamaAmaci.Equals(ProjeConstants.HEPSI) ? "" : " AND KiralamaAmaci=" + kiralamaAmaci.ReturnQuotedValue();
            string sql = string.Format(@"
                SELECT BolgeId, B.KiralamaAmaci, COUNT(A.Id) Adet, SUM(TeminatTutari) TeminatTutari, SUM(KalanTeminatTutari) KalanTeminatTutari
                FROM KiraSozlesme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                WHERE A.Aktif=1 {0} {1}
                GROUP BY A.BolgeId, B.KiralamaAmaci
                ORDER BY BolgeId", bolgeStr, purposeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectTenantCountAndRentTotal(int bolgeId, int ay, int yil)
        {
            string bolgeStr = bolgeId == ProjeConstants.HEPSI_INT || bolgeId == ProjeConstants.BOLGE_GENELMUDURLUK_INT ? string.Empty : string.Format(" AND C.BolgeId={0} ", bolgeId);
            string sql = string.Format(@"
                SELECT H.KisaAdi Bolge,B.KiralamaAmaci,
                    SUM(D.KiraBedeli) + (SUM(D.FaizTutari)*-1) Tahakkuk,
                    SUM(A.OdenenTutar) Tahsil,
                    COUNT(DISTINCT(C.ID)) KiraciSayisi,
                    COUNT(DISTINCT(A.KiraciId)) OdeyenKiraci
                FROM Odeme_Table A
                    INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                    INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
                    INNER JOIN OdemePlani_Table D ON D.Id=A.OdemePlaniId
                    LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
                    LEFT JOIN Bolge_Table H ON H.Id=C.BolgeId
                WHERE YEAR(A.OdemeTarihi) ={0} AND MONTH(A.OdemeTarihi) ={1} {2}
                GROUP BY H.KisaAdi,B.KiralamaAmaci", yil, ay, bolgeStr);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectNext(int id, int dosyaNo)
        {
            string sql = dosyaNo > 0
                ? string.Format("SELECT * FROM KiraSozlesme_Table WHERE Aktif=1 AND DosyaNo > {0} ORDER BY DosyaNo,Id", dosyaNo.ReturnQuotedValue())
                : string.Format("SELECT * FROM KiraSozlesme_Table WHERE Aktif=1 AND DosyaNo IS NULL AND Id > {0} ORDER BY DosyaNo,Id", id.ReturnQuotedValue());
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectPreviousByTenant(int kiraciId, string sozBasTar)
        {
            string sql = string.Format("SELECT * FROM KiraSozlesme_Table WHERE KiraciId={0} AND SozBastar < {1} ORDER BY SozBasTar DESC", kiraciId.ReturnQuotedValue(), sozBasTar);
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectPrevious(int id, int dosyaNo)
        {
            string sql = dosyaNo > 0
                ? string.Format("SELECT * FROM KiraSozlesme_Table WHERE Aktif=1 AND DosyaNo < {0} ORDER BY DosyaNo DESC,Id DESC", dosyaNo.ReturnQuotedValue())
                : string.Format("SELECT * FROM KiraSozlesme_Table WHERE Aktif=1 AND DosyaNo IS NULL AND Id < {0} ORDER BY DosyaNo DESC,Id DESC", id.ReturnQuotedValue());
            return db.SelectFromDb(sql, "");
        }
        public DataTable SelectMax()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(SELECT MAX(DosyaNo) FROM KiraSozlesme_Table WHERE Aktif=1)"), "");
        }
        public DataTable SelectMin()
        {
            return db.SelectFromDb(new SqlQuery(@"
                SELECT * FROM KiraSozlesme_Table
                WHERE DosyaNo=(SELECT MIN(DosyaNo) FROM KiraSozlesme_Table WHERE Aktif=1 AND DosyaNo>0)"), "");
        }
    }
}

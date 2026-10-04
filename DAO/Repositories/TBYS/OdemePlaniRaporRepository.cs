using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.TBYS
{
    public class OdemePlaniRaporRepository
    {
        private readonly DbClass db;
        public OdemePlaniRaporRepository() : this(new DbClass()) { }
        public OdemePlaniRaporRepository(DbClass db) { if (db == null) throw new ArgumentNullException("db"); this.db = db; }

        public DataTable SelectBorcluByBolgeTarih(int bolgeId, DateTime ilkTarih, DateTime sonTarih, int ayBas, int ayBit, int allRegionId, int headquartersRegionId)
        {
            string region = bolgeId == allRegionId || bolgeId == headquartersRegionId ? string.Empty : " AND A.BolgeId=@BolgeId";
            SqlQuery q = new SqlQuery(@"
                SELECT A.Id KiraSozlesmeId, H.KisaAdi Bolge, A.DosyaNo, B.Adi+' '+B.Soyadi Kiraci, A.KiraciId,
                    A.IlkSozlesmeTar, A.SozBasTar, A.SozBitTar, A.ArtisAyi, A.OdemeSekli,
                    A.KiraBedeli, C.AnaPara AnaPara,C.FaizTutari, C.FaizliBakiye, C.VadeBasTar, C.VadeBitTar, C.Id OdemePlaniId,
                    FORMAT(C.FaizliBakiye,'###.00') FaizliBakiyeFormat,
                    B.Adres, B.Ili,B.Ilcesi,B.Semt,A.TeminatOdemeTarihi,A.TeminatTutari,
                    ABS(FaizliBakiye/A.KiraBedeli)*C.Sira AySayisi, A.TaksitSayisi
                FROM KiraSozlesme_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                INNER JOIN Bolge_Table H ON H.Id=A.BolgeId
                LEFT JOIN OdemePlani_Table C ON C.SozlesmeId=A.Id
                WHERE 1>0" + region + @"
                    AND C.FaizliBakiye < 0 AND C.VadeBitTar BETWEEN @IlkTarih AND @SonTarih
                    AND (((A.TaksitSayisi>1 AND ((ABS(C.FaizliBakiye)-ABS(A.KiraBedeli))/A.KiraBedeli) BETWEEN @AyBas AND @AyBit)
                        OR (A.TaksitSayisi<2 AND (ABS(FaizliBakiye/A.KiraBedeli)*C.Sira BETWEEN @AyBas AND @AyBit)))
                    AND A.SozBasTar<@SonTarih AND A.SozBitTar>=@IlkTarih
                    AND (A.SozlesmeDurumu='Devam Ediyor'
                        OR (A.SozlesmeDurumu!='Devam Ediyor' AND A.SozlesmeDurumu!='Yenilendi' AND A.DurumDegismeTar BETWEEN @IlkTarih AND @SonTarih)
                        OR (A.SozlesmeDurumu='Yenilendi' AND A.SozBitTar>@SonTarih))
                ORDER BY A.BolgeId,A.DosyaNo,B.Adi");
            AddDebtParameters(q, bolgeId, region, ilkTarih, sonTarih, ayBas, ayBit);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectCurrentByDate(DateTime ilkTarih, DateTime sonTarih, string bolge, string allRegion)
        {
            string region = string.IsNullOrEmpty(bolge) || bolge.Equals(allRegion) ? string.Empty : " AND E.Bolge=@Bolge";
            SqlQuery q = new SqlQuery(@"
                SELECT A.Id KiraSozlesmeId,E.Bolge,A.DosyaNo,G.Adi+' '+G.Soyadi Kiraci,D.Adres+' '+ISNULL(F.BolumNo,'') Adres,
                    A.IlkSozlesmeTar,A.SozBasTar,A.SozBitTar,A.ArtisAyi,A.OdemeSekli,A.KiraBedeli,
                    B.AnaPara AnaPara,B.FaizTutari,B.FaizliBakiye,B.VadeBasTar,D.KullanimSekli,D.Ili,D.Ilcesi,
                    A.TeminatOdemeTarihi,A.TeminatTutari
                FROM KiraSozlesme_Table A
                LEFT JOIN OdemePlani_Table B ON B.SozlesmeId=A.Id AND B.Id IN (SELECT Id FROM OdemePlani_Table WHERE VadeBasTar BETWEEN @IlkTarih AND @SonTarih)
                LEFT JOIN SozlesmeTasinmaz_Table C ON C.SozlesmeId=A.Id AND C.Id=(SELECT TOP 1 Id FROM SozlesmeTasinmaz_Table WHERE SozlesmeId=A.Id)
                LEFT JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                LEFT JOIN BagimsizBolum_Table F ON F.Id=C.BolumId
                LEFT JOIN Kiraci_Table G ON G.Id=A.KiraciId
                WHERE A.Aktif=1" + region + @"
                ORDER BY CASE WHEN A.DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(A.DosyaNo,999999),A.Id");
            q.AddParameter("@IlkTarih", ilkTarih); q.AddParameter("@SonTarih", sonTarih); if (!string.IsNullOrEmpty(region)) q.AddParameter("@Bolge", bolge);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectIncomeByRegionMonth(int bolgeId, int ay, int yil, int allRegionId, int headquartersRegionId)
        {
            string region = bolgeId == allRegionId || bolgeId == headquartersRegionId ? string.Empty : " AND C.BolgeId=@BolgeId";
            SqlQuery q = new SqlQuery(@"
                SELECT H.KisaAdi Bolge,B.KiralamaAmaci,SUM(A.OdenenTutar) ToplamOdemeTutari,COUNT(DISTINCT(C.Id)) ToplamKiraciSayisi
                FROM Odeme_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                INNER JOIN KiraSozlesme_Table C ON C.Id=A.SozlesmeId
                INNER JOIN OdemePlani_Table D ON D.Id=A.OdemePlaniId
                LEFT JOIN TeminatIslem_Table E ON E.OdemeId=A.Id
                LEFT JOIN Bolge_Table H ON H.Id=C.BolgeId
                WHERE YEAR(A.OdemeTarihi)=@Yil AND MONTH(A.OdemeTarihi)=@Ay" + region + @"
                GROUP BY H.KisaAdi,B.KiralamaAmaci ORDER BY H.KisaAdi,B.KiralamaAmaci");
            q.AddParameter("@Yil", yil); q.AddParameter("@Ay", ay); if (!string.IsNullOrEmpty(region)) q.AddParameter("@BolgeId", bolgeId);
            return db.SelectFromDb(q, "");
        }

        public DataTable SelectListByDate(DateTime tarih)
        {
            SqlQuery q = new SqlQuery(@"
                SELECT A.Id SozlesmeId,A.DosyaNo,B.Adi,B.Soyadi,B.Adi+' '+B.Soyadi KiraciAdi,D.Adres,G.BolumNo,
                    F.OdemeBasTar,F.OdemeBitTar,F.KiraBedeli,F.OdenenTutar,F.FaizliBakiye
                FROM KiraSozlesme_Table A
                INNER JOIN Kiraci_Table B ON B.Id=A.KiraciId
                LEFT JOIN SozlesmeTasinmaz_Table C ON C.Id=(SELECT TOP 1 Id FROM SozlesmeTasinmaz_Table WHERE SozlesmeId=A.Id)
                LEFT JOIN Tasinmaz_Table D ON D.Id=C.TasinmazId
                LEFT JOIN BagimsizBolum_Table G ON G.Id=C.BolumId
                LEFT JOIN Il_Table E ON E.IlAdi=D.Ili
                LEFT JOIN OdemePlani_Table F ON F.Id=(SELECT MAX(Id) FROM OdemePlani_Table WHERE SozlesmeId=A.Id AND VadeBasTar<@Tarih)
                WHERE Aktif=1
                ORDER BY CASE WHEN A.DosyaNo=0 THEN 2 ELSE 1 END,ISNULL(A.DosyaNo,999999),ISNULL(A.BolgeId,0),A.Id");
            q.AddParameter("@Tarih", tarih); return db.SelectFromDb(q, "");
        }

        private static void AddDebtParameters(SqlQuery q, int bolgeId, string region, DateTime ilkTarih, DateTime sonTarih, int ayBas, int ayBit)
        {
            if (!string.IsNullOrEmpty(region)) q.AddParameter("@BolgeId", bolgeId);
            q.AddParameter("@IlkTarih", ilkTarih); q.AddParameter("@SonTarih", sonTarih); q.AddParameter("@AyBas", ayBas - 0.5); q.AddParameter("@AyBit", ayBit + 0.5);
        }
    }
}

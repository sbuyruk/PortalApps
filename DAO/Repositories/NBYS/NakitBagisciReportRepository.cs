using DAO.Ortak;
using System;
using System.Data;

namespace DAO.Repositories.NBYS
{
    public class NakitBagisciAdresRaporKriteri
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime TLGecisTarihi { get; set; }
        public int BagisciSayisi { get; set; }
        public bool BelgeIstemeyenlerHaric { get; set; }
        public bool AdresiBosOlanlarHaric { get; set; }
        public bool PostadanIadelerHaric { get; set; }
        public bool DergiGonderilmesinlerHaric { get; set; }
        public bool UlasilamayanlarHaric { get; set; }
        public bool SadeceYeniBagiscilar { get; set; }
        public string ParaIadeDurumu { get; set; }
        public string DahaOnceIadeDurumu { get; set; }
    }

    public class NakitBagisciReportRepository
    {
        private readonly DbClass db;

        public NakitBagisciReportRepository()
            : this(new DbClass())
        {
        }

        public NakitBagisciReportRepository(DbClass db)
        {
            this.db = db;
        }

        public DataTable SelectBagisciGroupByBagisAdedi(
            decimal minimumBagisMiktari,
            string bilinmeyenBagisciAdi,
            string baslangicTarihi,
            int sonBagisAyFarki)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT
                    A.BagisciId AS NakitBagisciId,
                    COUNT(A.Id) AS Adet,
                    SUM(A.BagisMiktari) AS Toplam,
                    MAX(A.BagisTarihi) AS SonBagisTarihi,
                    B.Adi,
                    B.Soyadi
                FROM NakitBagisHareket_Table A
                    LEFT JOIN NakitBagisci_Table B ON B.Id = A.BagisciId
                WHERE B.Adi NOT LIKE @BilinmeyenBagisciAdi
                    AND A.BagisTarihi > @BaslangicTarihi
                GROUP BY A.BagisciId, B.Adi, B.Soyadi
                HAVING SUM(A.BagisMiktari) >= @MinimumBagisMiktari
                    AND COUNT(A.Id) > 1
                    AND MAX(A.BagisTarihi) BETWEEN DATEADD(MONTH, @SonBagisAyFarki, GETDATE())
                        AND EOMONTH(GETDATE(), -1)
                    AND NOT EXISTS (
                        SELECT 1
                        FROM Armagan_Table C
                        WHERE C.BagisciId = A.BagisciId
                            AND C.ArmaganTanimId IN (2, 3, 4)
                            AND C.BagisciId IS NOT NULL
                    )
                ORDER BY Toplam DESC, Adet DESC, B.Adi");
            query.AddParameter("@BilinmeyenBagisciAdi", "%" + bilinmeyenBagisciAdi + "%");
            query.AddParameter("@BaslangicTarihi", NormalizeLegacyDateParameter(baslangicTarihi));
            query.AddParameter("@MinimumBagisMiktari", minimumBagisMiktari);
            query.AddParameter("@SonBagisAyFarki", sonBagisAyFarki);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectSecilmemisKatilimcilar(DateTime minimumBagisTarihi, decimal minimumBagisMiktari)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT DISTINCT(A.Id) KatilimciId, A.Adi, A.Soyadi,
                    A.Adres,A.Telefon1 Telefon,A.Sag,D.IlceAdi Ilce,C.IlAdi Il,MAX(B.BagisMiktari)
                FROM NakitBagisci_Table A
                    INNER JOIN Armagan_Table B ON B.BagisciId = A.Id
                        AND Tarih > @MinimumBagisTarihi AND BagisMiktari >= @MinimumBagisMiktari
                    LEFT JOIN Il_Table C ON C.Id = A.Ili
                    LEFT JOIN Ilce_Table D ON D.Id = A.Ilcesi AND D.IlId = A.Ili
                WHERE A.Sag = 1 AND A.TuzelKisi = 0 AND A.Adi IS NOT NULL
                    AND A.Adi != '' AND A.Adi NOT LIKE '%BILINMEYEN%'
                GROUP BY A.Id, A.Adi, A.Soyadi, A.Adres, A.Telefon1, A.Sag, D.IlceAdi, C.IlAdi
                ORDER BY A.Id");
            query.AddParameter("@MinimumBagisTarihi", minimumBagisTarihi);
            query.AddParameter("@MinimumBagisMiktari", minimumBagisMiktari);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectDuzenliBagisci(
            DateTime baslangicTarihi,
            DateTime bitisTarihi,
            bool sadeceBelgeOlusturulmadi,
            bool durumFiltrele,
            string durum)
        {
            SqlQuery query = new SqlQuery(@"
                SELECT A.BagisciId aBagisciId,A.BagisciAdi aBagisciAdi
                    ,A.Id aDuzenliBagisciId
                    ,A.BaslamaTarihi aBaslamaTarihi
                    ,A.Tutar aTutar,A.BagisAdedi aBagisAdedi,A.BagisToplami aBagisToplami
                    ,A.Aktif aAktif
                    ,A.ArmaganId aArmaganId, A.NakitBagisHareketId aNakitBagisHareketId
                    ,A.Telefon aTelefon,A.Eposta aEposta
                    ,A.EslesmeBilgisi aEslesmeBilgisi,A.Aciklama aAciklama
                    ,ISNULL(B.Adi, 'BAGISÇI BULUNAMADI') AS bAdi
                    ,B.TCKimlikNo bTCKimlikNo
                    ,Adres bAdres
                    ,Telefon1 bTelefon1
                    ,Telefon2 bTelefon2
                    ,TuzelKisi bTuzelKisi
                    ,Sag bSag
                    ,B.Eposta bEposta
                    ,PostaKodu bPostaKodu
                    ,B.Aciklama bAciklama
                    ,C.IlAdi bIl
                    ,D.IlceAdi bIlce
                    ,Ulasilamiyor bUlasilamiyor
                    ,BelgeIstemiyor bBelgeIstemiyor
                    ,E.Durum bDurum,E.KacinciBelge
                    ,F.KisaAdi bBolgeKisaAdi
                FROM DuzenliNakitBagisci_Table A
                    LEFT JOIN NakitBagisci_Table B ON A.BagisciId = B.Id
                    LEFT OUTER JOIN Il_Table C ON C.Id = B.Ili
                    LEFT OUTER JOIN Ilce_Table D ON D.Id = B.Ilcesi AND D.IlId = C.Id
                    LEFT JOIN Armagan_Table E ON E.Id = A.ArmaganId
                    LEFT JOIN Bolge_Table F ON F.Id = C.BolgeId
                WHERE A.Aktif = 1
                    AND BaslamaTarihi >= @BaslangicTarihi
                    AND BaslamaTarihi < @BitisTarihi
                    AND (@SadeceBelgeOlusturulmadi = 0 OR A.ArmaganId = 0)
                    AND (@DurumFiltrele = 0 OR E.Durum = @Durum)");
            query.AddParameter("@BaslangicTarihi", baslangicTarihi);
            query.AddParameter("@BitisTarihi", bitisTarihi);
            query.AddParameter("@SadeceBelgeOlusturulmadi", sadeceBelgeOlusturulmadi);
            query.AddParameter("@DurumFiltrele", durumFiltrele);
            query.AddParameter("@Durum", durum ?? string.Empty);
            return db.SelectFromDb(query, "");
        }

        public DataTable SelectByBagisTarihiBagisSayisi(NakitBagisciAdresRaporKriteri kriter)
        {
            SqlQuery query = kriter.SadeceYeniBagiscilar
                ? CreateYeniBagisciAdresRaporQuery()
                : CreateBagisciAdresRaporQuery();

            AddAdresRaporParameters(query, kriter);
            return db.SelectFromDb(query, "");
        }

        private static SqlQuery CreateYeniBagisciAdresRaporQuery()
        {
            return new SqlQuery(@"
                SELECT TOP (@BagisciSayisi)
                    SUM(Y.BagisMiktari) BagisMiktariDecimal,
                    CONVERT(nvarchar, REPLACE(SUM(Y.BagisMiktari),'.',',')) BagisMiktari,
                    NakitBagisciId, Adi,Adres,Telefon1,Telefon2, Ilcesi, Ili,
                    DergiGonderilmesin, TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                FROM
                (
                    SELECT A.Id NakitBagisciId, A.Adi,A.Adres,A.Telefon1,A.Telefon2,
                        G.IlceAdi Ilcesi, F.IlAdi Ili, A.DergiGonderilmesin,
                        A.TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                    FROM NakitBagisci_Table A
                        INNER JOIN Armagan_Table B ON B.BagisciId = A.Id
                        LEFT JOIN Il_Table F ON F.Id = A.Ili
                        LEFT JOIN Ilce_Table G ON G.Id = A.Ilcesi AND G.IlId = F.Id
                    WHERE B.Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi
                        AND (@PostadanIadelerHaric = 0 OR
                            (B.Durum != @ParaIadeDurumu AND B.Durum != @DahaOnceIadeDurumu))
                    GROUP BY A.Id,A.Adi,A.Adres,A.Ili,F.IlAdi,G.IlceAdi,A.Telefon1,A.Telefon2,
                        A.DergiGonderilmesin,A.TuzelKisi,BelgeIstemiyor,Ulasilamiyor

                    EXCEPT

                    SELECT E.Id NakitBagisciId, E.Adi,E.Adres,E.Telefon1,E.Telefon2,
                        J.IlceAdi Ilcesi, H.IlAdi Ili,E.DergiGonderilmesin,
                        E.TuzelKisi, BelgeIstemiyor,Ulasilamiyor
                    FROM NakitBagisci_Table E
                        INNER JOIN Armagan_Table F ON F.BagisciId = E.Id
                        LEFT JOIN Armagan_Table G ON F.BagisciId = G.BagisciId
                        LEFT JOIN Il_Table H ON H.Id = E.Ili
                        LEFT JOIN Ilce_Table J ON J.Id = E.Ilcesi AND J.IlId = H.Id
                    WHERE F.Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi
                        AND G.Tarih BETWEEN @TLGecisTarihi AND @BaslangicTarihiEksiBirGun
                        AND (@PostadanIadelerHaric = 0 OR
                            (F.Durum != @ParaIadeDurumu AND F.Durum != @DahaOnceIadeDurumu))
                ) Z
                    INNER JOIN Armagan_Table Y ON Z.NakitBagisciId = Y.BagisciId
                WHERE Y.Tarih > @BaslangicTarihiEksiBirGun
                    AND (@BelgeIstemeyenlerHaric = 0 OR Z.BelgeIstemiyor = 0)
                    AND (@AdresiBosOlanlarHaric = 0 OR ISNULL(LTRIM(RTRIM(Adres)), '') != '')
                    AND (@DergiGonderilmesinlerHaric = 0 OR Z.DergiGonderilmesin = 0)
                    AND (@UlasilamayanlarHaric = 0 OR Z.Ulasilamiyor = 0)
                GROUP BY NakitBagisciId,Adi,Adres,Ili,Ilcesi,Telefon1,Telefon2,
                    DergiGonderilmesin,TuzelKisi,BelgeIstemiyor,Ulasilamiyor
                ORDER BY BagisMiktariDecimal DESC, Z.NakitBagisciId DESC");
        }

        private static SqlQuery CreateBagisciAdresRaporQuery()
        {
            return new SqlQuery(@"
                SELECT TOP (@BagisciSayisi)
                    SUM(B.BagisMiktari) BagisMiktariDecimal,
                    CONVERT(nvarchar, REPLACE(SUM(B.BagisMiktari), '.', ',')) BagisMiktari,
                    A.Id NakitBagisciId, A.Adi, A.Adres, A.Telefon1, A.Telefon2,
                    G.IlceAdi Ilcesi, F.IlAdi Ili, A.DergiGonderilmesin,
                    A.TuzelKisi, BelgeIstemiyor, Ulasilamiyor
                FROM NakitBagisci_Table A
                    INNER JOIN Armagan_Table B ON B.BagisciId = A.Id
                    LEFT JOIN Il_Table F ON F.Id = A.Ili
                    LEFT JOIN Ilce_Table G ON G.Id = A.Ilcesi AND G.IlId = F.Id
                WHERE B.Tarih BETWEEN @BaslangicTarihi AND @BitisTarihi
                    AND (@PostadanIadelerHaric = 0 OR
                        (B.Durum != @ParaIadeDurumu AND B.Durum != @DahaOnceIadeDurumu))
                    AND (@BelgeIstemeyenlerHaric = 0 OR A.BelgeIstemiyor = 0)
                    AND (@AdresiBosOlanlarHaric = 0 OR ISNULL(LTRIM(RTRIM(Adres)), '') != '')
                    AND (@DergiGonderilmesinlerHaric = 0 OR A.DergiGonderilmesin = 0)
                    AND (@UlasilamayanlarHaric = 0 OR A.Ulasilamiyor = 0)
                GROUP BY A.Id, A.Adi, A.Adres, A.Ili, F.IlAdi, G.IlceAdi,
                    A.Telefon1, A.Telefon2, A.DergiGonderilmesin, A.TuzelKisi,
                    BelgeIstemiyor, Ulasilamiyor
                ORDER BY BagisMiktariDecimal DESC, A.Id DESC");
        }

        private static void AddAdresRaporParameters(SqlQuery query, NakitBagisciAdresRaporKriteri kriter)
        {
            query.AddParameter("@BagisciSayisi", kriter.BagisciSayisi);
            query.AddParameter("@BaslangicTarihi", kriter.BaslangicTarihi);
            query.AddParameter("@BitisTarihi", kriter.BitisTarihi);
            query.AddParameter("@TLGecisTarihi", kriter.TLGecisTarihi);
            query.AddParameter("@BaslangicTarihiEksiBirGun", kriter.BaslangicTarihi.AddDays(-1));
            query.AddParameter("@BelgeIstemeyenlerHaric", kriter.BelgeIstemeyenlerHaric);
            query.AddParameter("@AdresiBosOlanlarHaric", kriter.AdresiBosOlanlarHaric);
            query.AddParameter("@PostadanIadelerHaric", kriter.PostadanIadelerHaric);
            query.AddParameter("@DergiGonderilmesinlerHaric", kriter.DergiGonderilmesinlerHaric);
            query.AddParameter("@UlasilamayanlarHaric", kriter.UlasilamayanlarHaric);
            query.AddParameter("@ParaIadeDurumu", kriter.ParaIadeDurumu ?? string.Empty);
            query.AddParameter("@DahaOnceIadeDurumu", kriter.DahaOnceIadeDurumu ?? string.Empty);
        }

        private static object NormalizeLegacyDateParameter(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            if (value.Length >= 2 && value[0] == '\'' && value[value.Length - 1] == '\'')
                return value.Substring(1, value.Length - 2);

            return value;
        }
    }
}

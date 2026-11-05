-- PortalApps veritabanında çalıştırılır. Yalnızca okur; kayıt değiştirmez.
-- Geçmiş mükerrer kayıtlar bağlantıları incelenmeden silinmemelidir.
SET NOCOUNT ON;

-- Birden fazla aktif sözleşmesi bulunan kiracılar.
SELECT KiraciId, COUNT_BIG(*) AS AktifSozlesmeSayisi
FROM dbo.KiraSozlesme_Table
WHERE Aktif=1
GROUP BY KiraciId
HAVING COUNT_BIG(*)>1;

SELECT S.Id AS SozlesmeId, S.KiraciId, S.SozBasTar, S.SozBitTar,
       S.DosyaNo, S.OlusturmaTarihi
FROM dbo.KiraSozlesme_Table S
WHERE S.Aktif=1 AND S.KiraciId IN (
    SELECT KiraciId FROM dbo.KiraSozlesme_Table WHERE Aktif=1
    GROUP BY KiraciId HAVING COUNT_BIG(*)>1
)
ORDER BY S.KiraciId,S.Id;

-- Uygulamanın mevcut işlem numarası kuralına göre yinelenen banka hareketleri.
SELECT IslemNo, COUNT_BIG(*) AS KayitSayisi
FROM dbo.KiraEkstreAktarma_Table
WHERE NULLIF(LTRIM(RTRIM(IslemNo)),N'') IS NOT NULL
GROUP BY IslemNo
HAVING COUNT_BIG(*)>1;

SELECT E.Id, E.IslemNo, E.BankaAdi, E.OdemeTarihi, E.Tutar,
       E.AktarildiMi, E.KiraciId, E.OdemeId
FROM dbo.KiraEkstreAktarma_Table E
WHERE E.IslemNo IN (
    SELECT IslemNo FROM dbo.KiraEkstreAktarma_Table
    WHERE NULLIF(LTRIM(RTRIM(IslemNo)),N'') IS NOT NULL
    GROUP BY IslemNo HAVING COUNT_BIG(*)>1
)
ORDER BY E.IslemNo,E.Id;

-- Güvenilir mükerrer kontrolü yapılamayan boş işlem numaraları.
SELECT Id, BankaAdi, OdemeTarihi, Tutar, AktarildiMi
FROM dbo.KiraEkstreAktarma_Table
WHERE NULLIF(LTRIM(RTRIM(IslemNo)),N'') IS NULL;

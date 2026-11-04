-- Hedef PortalApps veritabanında çalıştırılır. Yalnızca okur; hiçbir kaydı düzeltmez.
-- Sonuçlar inceleme adaylarıdır. Geçmiş kayıtlar otomatik olarak değiştirilmemelidir.
SET NOCOUNT ON;

-- 1. Bağlı ödemesi/planı bulunamayan veya kiracı, tutar, tarih bağlantısı uyuşmayan mahsuplar.
SELECT T.Id AS TeminatIslemId, T.KiraciId, T.IslemTarihi, T.IslemTutari,
       T.OdemeId, O.KiraciId AS OdemeKiraciId, O.SozlesmeId, O.OdemePlaniId,
       O.OdemeTarihi, O.OdenenTutar, P.SozlesmeId AS PlanSozlesmeId
FROM dbo.TeminatIslem_Table T
LEFT JOIN dbo.Odeme_Table O ON O.Id = T.OdemeId
LEFT JOIN dbo.OdemePlani_Table P ON P.Id = O.OdemePlaniId
LEFT JOIN dbo.KiraSozlesme_Table S ON S.Id = O.SozlesmeId
WHERE T.IslemTipi = N'Kiraya Mahsup'
  AND (O.Id IS NULL OR P.Id IS NULL OR S.Id IS NULL
       OR ISNULL(O.KiraciId, 0) <> ISNULL(T.KiraciId, 0)
       OR ISNULL(S.KiraciId, 0) <> ISNULL(T.KiraciId, 0)
       OR ISNULL(O.SozlesmeId, 0) <> ISNULL(P.SozlesmeId, 0)
       OR ISNULL(O.OdenenTutar, 0) <> ISNULL(T.IslemTutari, 0)
       OR O.OdemeTarihi IS NULL OR T.IslemTarihi IS NULL
       OR O.OdemeTarihi <> T.IslemTarihi)
ORDER BY T.KiraciId, T.Id;

-- 2. Birden çok teminat hareketine bağlanmış kira ödemeleri.
SELECT T.OdemeId, COUNT_BIG(*) AS TeminatHareketSayisi
FROM dbo.TeminatIslem_Table T
WHERE T.OdemeId > 0
GROUP BY T.OdemeId
HAVING COUNT_BIG(*) > 1;

-- 3. Ödeme hareketleri toplamı ile planın saklanan ödenen tutarı arasındaki farklar.
-- Silinmiş mahsuplar artık ilişkilendirilemeyeceğinden tüm planlar taranır.
-- Farkın eski mahsup hatasından kaynaklandığı kayıt bazında ayrıca doğrulanmalıdır.
SELECT P.Id AS OdemePlaniId, P.SozlesmeId, P.OdenenTutar AS PlandakiOdenen,
       ISNULL(O.Toplam, 0) AS OdemeHareketleriToplami,
       ISNULL(P.OdenenTutar, 0) - ISNULL(O.Toplam, 0) AS Fark
FROM dbo.OdemePlani_Table P
OUTER APPLY (
    SELECT SUM(O.OdenenTutar) AS Toplam
    FROM dbo.Odeme_Table O
    WHERE O.OdemePlaniId = P.Id AND O.SozlesmeId = P.SozlesmeId
) O
WHERE ISNULL(P.OdenenTutar, 0) <> ISNULL(O.Toplam, 0)
ORDER BY P.SozlesmeId, P.Id;

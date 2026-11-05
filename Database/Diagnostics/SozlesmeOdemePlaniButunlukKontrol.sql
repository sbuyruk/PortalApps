-- PortalApps veritabanında çalıştırılır. Yalnızca okur; kayıt değiştirmez.
-- Sonuçlar inceleme adaylarıdır; otomatik eşleştirme veya silme yapılmamalıdır.
SET NOCOUNT ON;

-- Eski plan silmelerinden kalmış olabilecek kopuk veya yanlış ödeme bağlantıları.
SELECT O.Id AS OdemeId, O.KiraciId, O.SozlesmeId, O.OdemePlaniId,
       O.OdemeTarihi, O.OdenenTutar, P.SozlesmeId AS PlanSozlesmeId
FROM dbo.Odeme_Table O
LEFT JOIN dbo.KiraSozlesme_Table S ON S.Id=O.SozlesmeId
LEFT JOIN dbo.OdemePlani_Table P ON P.Id=O.OdemePlaniId
WHERE S.Id IS NULL OR P.Id IS NULL
   OR ISNULL(S.KiraciId,0)<>ISNULL(O.KiraciId,0)
   OR ISNULL(P.SozlesmeId,0)<>ISNULL(O.SozlesmeId,0)
ORDER BY O.SozlesmeId,O.Id;

-- Aynı sözleşmede aynı taksit sırası birden fazla kez bulunmamalıdır.
SELECT SozlesmeId,Sira,COUNT_BIG(*) AS KayitSayisi
FROM dbo.OdemePlani_Table
GROUP BY SozlesmeId,Sira
HAVING COUNT_BIG(*)>1 OR Sira<0 OR Sira>12 OR Sira IS NULL;

-- Sözleşmesi kalmamış planlar ve taşınmaz bağlantıları.
SELECT N'Ödeme planı' AS KayitTuru,P.Id,P.SozlesmeId
FROM dbo.OdemePlani_Table P
WHERE NOT EXISTS (SELECT 1 FROM dbo.KiraSozlesme_Table S WHERE S.Id=P.SozlesmeId)
UNION ALL
SELECT N'Taşınmaz bağlantısı',T.Id,T.SozlesmeId
FROM dbo.SozlesmeTasinmaz_Table T
WHERE NOT EXISTS (SELECT 1 FROM dbo.KiraSozlesme_Table S WHERE S.Id=T.SozlesmeId);

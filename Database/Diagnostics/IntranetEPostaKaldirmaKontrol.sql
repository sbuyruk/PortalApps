-- Hedef PortalApps veritabanında, geçişten ÖNCE çalıştırılır. Veri değiştirmez.
-- Listeyi değerlendirip eksik/uygunsuz InternetEPosta adreslerini uygulamadan düzeltin.
SET NOCOUNT ON;

SELECT c.name AS Sutun, TYPE_NAME(c.user_type_id) AS VeriTipi,
       c.max_length AS Uzunluk, c.is_nullable AS NullKabulEder, dc.definition AS VarsayilanDeger
FROM sys.columns c
LEFT JOIN sys.default_constraints dc ON dc.object_id=c.default_object_id
WHERE c.object_id=OBJECT_ID(N'dbo.IletisimBilgileri_Table')
  AND c.name IN (N'IntranetEPosta',N'InternetEPosta');

-- Farklı adresler otomatik birleştirilmez; yeni adresin SMTP üzerinden ulaşılabilirliğini doğrulayın.
IF COL_LENGTH(N'dbo.IletisimBilgileri_Table',N'IntranetEPosta') IS NOT NULL
    EXEC sys.sp_executesql N'
        SELECT C.Id, C.PersonelId, C.IntranetEPosta, C.InternetEPosta
        FROM dbo.IletisimBilgileri_Table C
        WHERE ISNULL(LTRIM(RTRIM(C.IntranetEPosta)),N'''')
           <> ISNULL(LTRIM(RTRIM(C.InternetEPosta)),N'''')
        ORDER BY C.PersonelId,C.Adres;';

-- Bildirimler Adres sırasındaki ilk iletişim kaydını kullanır.
SELECT P.Id AS PersonelId, C.Id AS IletisimId, C.InternetEPosta
FROM dbo.Personel_Table P
OUTER APPLY (SELECT TOP (1) Id, InternetEPosta FROM dbo.IletisimBilgileri_Table
             WHERE PersonelId=P.Id ORDER BY Adres) C
WHERE EXISTS (SELECT 1 FROM dbo.IsBilgileri_Table I WHERE I.PersonelId=P.Id AND I.CalismaDurumu=1)
  AND NULLIF(LTRIM(RTRIM(C.InternetEPosta)),N'') IS NULL
ORDER BY P.Id;

-- Dinamik SQL, şifreli modüller, SQL Agent işleri ve harici uygulamalar ayrıca incelenmelidir.
SELECT OBJECT_SCHEMA_NAME(m.object_id) AS Sema, OBJECT_NAME(m.object_id) AS Nesne
FROM sys.sql_modules m
WHERE m.definition LIKE N'%IntranetEPosta%';

SELECT OBJECT_SCHEMA_NAME(d.referencing_id) AS Sema, OBJECT_NAME(d.referencing_id) AS Nesne,
       COL_NAME(d.referenced_id,d.referenced_minor_id) AS Sutun
FROM sys.sql_expression_dependencies d
WHERE d.referenced_id=OBJECT_ID(N'dbo.IletisimBilgileri_Table');

SELECT i.name AS Indeks, c.name AS Sutun
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON ic.object_id=i.object_id AND ic.index_id=i.index_id
INNER JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
WHERE c.object_id=OBJECT_ID(N'dbo.IletisimBilgileri_Table') AND c.name=N'IntranetEPosta';

SELECT name AS CheckKisiti, definition AS Tanim
FROM sys.check_constraints
WHERE parent_object_id=OBJECT_ID(N'dbo.IletisimBilgileri_Table')
  AND definition LIKE N'%IntranetEPosta%';

SELECT name AS HesaplananSutun, definition AS Tanim
FROM sys.computed_columns
WHERE object_id=OBJECT_ID(N'dbo.IletisimBilgileri_Table')
  AND definition LIKE N'%IntranetEPosta%';

IF OBJECT_ID(N'dbo.PortalToplantiOutbox_Table',N'U') IS NOT NULL
    EXEC sys.sp_executesql N'
        SELECT Id,MeetingId,Status,AttemptCount,
               CASE WHEN DeliveryPlan IS NULL THEN 0 ELSE 1 END AS AliciPlaniHazir
        FROM dbo.PortalToplantiOutbox_Table WHERE Status<>''Sent'' ORDER BY Id;';

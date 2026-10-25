-- Eski loglama bağımlılık envanteri. Uygulamanın Test veritabanında çalıştırın.
-- Yalnızca metadata okur; nesne/veri/yetki değiştirmez, üretilen sorguları çalıştırmaz.
-- Aynı sunucudaki başka veritabanları ve dış sistemler ayrıca incelenmelidir.
IF DB_NAME() IN (N'master', N'model', N'msdb', N'tempdb')
BEGIN
    PRINT N'Uygulamanın Test veritabanını seçip betiği yeniden çalıştırın.';
    RETURN;
END;

-- 01: Ortam ve görünürlük. VIEW DEFINITION=0/NULL ise boş sonuç temizlik kanıtı değildir.
SELECT N'01-Ortam' AS Bolum, CONVERT(nvarchar(128), SERVERPROPERTY('ServerName')) AS Sunucu,
       DB_NAME() AS Veritabani, SYSUTCDATETIME() AS AlinanUtc,
       HAS_PERMS_BY_NAME(DB_NAME(), N'DATABASE', N'VIEW DEFINITION') AS ViewDefinition,
       HAS_PERMS_BY_NAME(NULL, NULL, N'VIEW ANY DEFINITION') AS ServerViewAnyDefinition,
       HAS_DBACCESS(N'msdb') AS MsdbErisimi,
       IS_SRVROLEMEMBER(N'sysadmin') AS Sysadmin;

-- 02: Mevcut eski tablolar, yaklaşık satır sayısı ve identity durumu.
-- Yaklaşık sayı kabul testi için kesin başlangıç/sonuç sayımı değildir.
SELECT N'02-Tablolar' AS Bolum, s.name AS Sema, t.name AS Tablo,
       t.object_id AS ObjectId, t.create_date AS OlusturmaTarihi,
       (SELECT SUM(p.rows) FROM sys.partitions AS p
        WHERE p.object_id=t.object_id AND p.index_id IN (0, 1)) AS YaklasikSatir,
       ic.name AS IdentityKolonu, ic.last_value AS SonIdentityDegeri
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id=t.schema_id
LEFT JOIN sys.identity_columns AS ic ON ic.object_id=t.object_id
WHERE t.name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')
ORDER BY s.name, t.name;

-- 03: SQL Server'ın çözümlediği doğrudan başvurular; üç/dört parçalı adlar dahil.
SELECT N'03-DogrudanBagimlilik' AS Bolum,
       OBJECT_SCHEMA_NAME(d.referencing_id) AS KaynakSema,
       COALESCE(o.name, tr.name) AS KaynakNesne, o.type_desc AS KaynakTip,
       d.referenced_server_name AS HedefSunucu,
       d.referenced_database_name AS HedefVeritabani,
       d.referenced_schema_name AS HedefSema, d.referenced_entity_name AS HedefNesne,
       d.is_caller_dependent AS CagirinaBagli, d.is_ambiguous AS Belirsiz
FROM sys.sql_expression_dependencies AS d
LEFT JOIN sys.objects AS o ON o.object_id=d.referencing_id
LEFT JOIN sys.triggers AS tr ON tr.object_id=d.referencing_id
WHERE d.referenced_id IN
      (SELECT object_id FROM sys.tables
       WHERE name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table'))
   OR d.referenced_entity_name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')
ORDER BY KaynakSema, KaynakNesne;

-- 04: Yerel, çözülmüş başvurular üzerinden dolaylı çağıranlar (en fazla 16 kenar).
-- Döngüler yol üzerinden kesilir. Dinamik/çözülemeyen/cross-database zincirler bu ağa dahil değildir.
;WITH Yollar AS
(
    SELECT t.object_id AS KokId, t.object_id AS NesneId, 0 AS Derinlik,
           CAST('/' + CONVERT(varchar(11), t.object_id) + '/' AS varchar(max)) AS Yol
    FROM sys.tables AS t
    WHERE t.name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')
    UNION ALL
    SELECT y.KokId, d.referencing_id, y.Derinlik + 1,
           CAST(y.Yol + CONVERT(varchar(11), d.referencing_id) + '/' AS varchar(max))
    FROM Yollar AS y
    JOIN sys.sql_expression_dependencies AS d ON d.referenced_id=y.NesneId
    WHERE y.Derinlik < 16
      AND CHARINDEX('/' + CONVERT(varchar(11), d.referencing_id) + '/', y.Yol)=0
)
SELECT DISTINCT N'04-DolayliCagiran' AS Bolum,
       OBJECT_SCHEMA_NAME(KokId) AS KokSema, OBJECT_NAME(KokId) AS KokTablo,
       OBJECT_SCHEMA_NAME(NesneId) AS KaynakSema, OBJECT_NAME(NesneId) AS KaynakNesne,
       Derinlik, Yol, CASE WHEN Derinlik=16 THEN N'Derinlik sınırı; devamını inceleyin.' ELSE N'' END AS Notlar
FROM Yollar WHERE Derinlik > 0
ORDER BY KokTablo, Derinlik, KaynakSema, KaynakNesne
OPTION (MAXRECURSION 100);

-- 05: Metin başvuruları. Dinamik SQL'deki tam adları yakalar; parçalı ad üretimini kanıtlayamaz.
-- Tanımlar/komutlar hassas bilgi içerebileceğinden tam metin döndürülmez.
SELECT N'05-ModulMetni' AS Bolum, OBJECT_SCHEMA_NAME(m.object_id) AS Sema,
       COALESCE(o.name, tr.name) AS Nesne, o.type_desc AS Tip,
       CASE WHEN m.definition COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%' THEN 1 ELSE 0 END AS OlayEslesmesi,
       CASE WHEN m.definition COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%' THEN 1 ELSE 0 END AS SqlLogEslesmesi
FROM sys.sql_modules AS m
LEFT JOIN sys.objects AS o ON o.object_id=m.object_id
LEFT JOIN sys.triggers AS tr ON tr.object_id=m.object_id
WHERE m.definition COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%'
   OR m.definition COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%'
ORDER BY Sema, Nesne;

-- 06: Doğrudan eski tabloya bağlı trigger'lar, FK'ler ve synonym'ler.
SELECT N'06a-TabloTrigger' AS Bolum, OBJECT_SCHEMA_NAME(t.object_id) AS Sema,
       t.name AS Tablo, tr.name AS TriggerAdi, tr.is_disabled AS DevreDisi,
       tr.is_instead_of_trigger AS InsteadOf,
       CASE WHEN m.definition IS NULL THEN 0 ELSE 1 END AS TanimGorunur
FROM sys.tables AS t
JOIN sys.triggers AS tr ON tr.parent_id=t.object_id
LEFT JOIN sys.sql_modules AS m ON m.object_id=tr.object_id
WHERE t.name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table');

SELECT N'06b-ForeignKey' AS Bolum, fk.name AS ForeignKeyAdi,
       OBJECT_SCHEMA_NAME(fk.parent_object_id) AS KaynakSema, OBJECT_NAME(fk.parent_object_id) AS KaynakTablo,
       OBJECT_SCHEMA_NAME(fk.referenced_object_id) AS HedefSema, OBJECT_NAME(fk.referenced_object_id) AS HedefTablo,
       fk.is_disabled AS DevreDisi
FROM sys.foreign_keys AS fk
WHERE fk.parent_object_id IN (SELECT object_id FROM sys.tables
       WHERE name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table'))
   OR fk.referenced_object_id IN (SELECT object_id FROM sys.tables
       WHERE name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table'));

SELECT N'06c-Synonym' AS Bolum, SCHEMA_NAME(s.schema_id) AS Sema,
       s.name AS SynonymAdi, s.base_object_name AS Hedef
FROM sys.synonyms AS s
WHERE s.base_object_name COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%'
   OR s.base_object_name COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%';

-- 07: Tabloları okuyabilen/yazabilen kimliklerin değerlendirilmesine girdi.
-- Bu liste etkin yetki hesabı değildir: sahiplik, roller, public ve sunucu yetkileri ayrıca değerlendirilir.
SELECT N'07a-Yetkiler' AS Bolum, pr.name AS KullaniciVeyaRol,
       dp.class_desc AS Kapsam, dp.state_desc AS Durum, dp.permission_name AS Yetki,
       CASE WHEN dp.class=1 THEN OBJECT_SCHEMA_NAME(dp.major_id)
            WHEN dp.class=3 THEN SCHEMA_NAME(dp.major_id) END AS Sema,
       CASE WHEN dp.class=1 THEN OBJECT_NAME(dp.major_id) END AS Nesne,
       dp.minor_id AS KolonId
FROM sys.database_permissions AS dp
JOIN sys.database_principals AS pr ON pr.principal_id=dp.grantee_principal_id
WHERE dp.class=0
   OR (dp.class=1 AND dp.major_id IN (SELECT object_id FROM sys.tables
       WHERE name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')))
   OR (dp.class=3 AND dp.major_id IN (SELECT schema_id FROM sys.tables
       WHERE name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')))
ORDER BY KullaniciVeyaRol, Kapsam, Sema, Nesne, Yetki;

SELECT N'07b-RolUyelikleri' AS Bolum, r.name AS Rol, u.name AS Uye
FROM sys.database_role_members AS rm
JOIN sys.database_principals AS r ON r.principal_id=rm.role_principal_id
JOIN sys.database_principals AS u ON u.principal_id=rm.member_principal_id
ORDER BY Rol, Uye;

-- 08: Tanımı kapalı/görünmeyen veya SQL metni olmayan modüller ayrıca incelenmelidir.
SELECT N'08a-KapaliTanim' AS Bolum, OBJECT_SCHEMA_NAME(m.object_id) AS Sema,
       OBJECT_NAME(m.object_id) AS Nesne,
       OBJECTPROPERTY(m.object_id, 'IsEncrypted') AS Sifreli
FROM sys.sql_modules AS m WHERE m.definition IS NULL;

SELECT N'08b-CLRModulleri' AS Bolum, OBJECT_SCHEMA_NAME(am.object_id) AS Sema,
       OBJECT_NAME(am.object_id) AS Nesne, a.name AS AssemblyAdi
FROM sys.assembly_modules AS am
JOIN sys.assemblies AS a ON a.assembly_id=am.assembly_id;

SELECT N'08c-SunucuTrigger' AS Bolum, tr.name AS TriggerAdi, tr.is_disabled AS DevreDisi,
       CASE WHEN m.definition IS NULL THEN 0 ELSE 1 END AS TanimGorunur
FROM sys.server_triggers AS tr
LEFT JOIN sys.server_sql_modules AS m ON m.object_id=tr.object_id
WHERE m.definition COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%'
   OR m.definition COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%'
   OR m.definition IS NULL;

-- 09: SQL Agent adımlarında ad eşleşmesi. Diğer DB'leri hedefleyen adımlar da gösterilir.
-- msdb erişimi başarılsa bile SQLAgent rolü yalnızca bazı job'ları gösterebilir.
BEGIN TRY
    IF HAS_DBACCESS(N'msdb')=1
    BEGIN
        SELECT N'09a-AgentGorunurluk' AS Bolum,
               N'Sonuçlar yalnızca hesabın görebildiği job kapsamındadır; DBA tamlığı doğrulamalıdır.' AS Notlar;
        SELECT N'09b-AgentAdimlari' AS Bolum, j.name AS JobAdi, j.enabled AS Etkin,
               st.step_id AS AdimNo, st.step_name AS AdimAdi,
               st.subsystem AS AltSistem, st.database_name AS HedefVeritabani,
               CASE WHEN st.command COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%' THEN 1 ELSE 0 END AS OlayEslesmesi,
               CASE WHEN st.command COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%' THEN 1 ELSE 0 END AS SqlLogEslesmesi
        FROM msdb.dbo.sysjobs AS j
        JOIN msdb.dbo.sysjobsteps AS st ON st.job_id=j.job_id
        WHERE st.command COLLATE Latin1_General_CI_AS LIKE N'%Olay[_]Table%'
           OR st.command COLLATE Latin1_General_CI_AS LIKE N'%SQLLog[_]Table%'
        ORDER BY JobAdi, AdimNo;
    END
    ELSE
        SELECT N'09-AgentEksik' AS Bolum, N'msdb erişimi yok; DBA envanteri tamamlamalıdır.' AS Notlar;
END TRY
BEGIN CATCH
    SELECT N'09-AgentEksik' AS Bolum, ERROR_NUMBER() AS HataNo,
           N'SQL Agent envanteri okunamadı; bu bölüm tamamlanmadı.' AS Notlar;
END CATCH;

-- 10: Kesin önce/sonra gözlem sorguları ÜRETİLİR; otomatik çalıştırılmaz.
-- Büyük tabloda COUNT_BIG tam tarama yapabilir. Kontrollü Test aralığında çalıştırın.
-- Sabit count/max tek başına yazılmadığının kanıtı değildir; eşzamanlı silme/yeniden ekleme gizleyebilir.
SELECT N'10-GozlemSorgusu' AS Bolum, SCHEMA_NAME(t.schema_id) AS Sema, t.name AS Tablo,
       N'SELECT SYSUTCDATETIME() AS AlinanUtc, DB_NAME() AS Veritabani, N'''
       + REPLACE(QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name), N'''', N'''''')
       + N''' AS Tablo, COUNT_BIG(*) AS SatirSayisi'
       + CASE WHEN EXISTS (SELECT 1 FROM sys.columns AS c WHERE c.object_id=t.object_id
                          AND c.name=N'Id' AND c.system_type_id IN (48, 52, 56, 127))
              THEN N', MAX(CONVERT(bigint, [Id])) AS EnBuyukId' ELSE N'' END
       + N' FROM ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N';' AS SaltOkunurSorgu
FROM sys.tables AS t
WHERE t.name COLLATE Latin1_General_CI_AS IN (N'Olay_Table', N'SQLLog_Table')
ORDER BY Sema, Tablo;

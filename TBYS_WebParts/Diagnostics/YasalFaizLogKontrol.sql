-- TEST veritabanında kullanıcı tarafından çalıştırılacak salt okunur kontrol.
-- Güncel kayıtları tarih/gösterge paneli filtrelerinden bağımsız listeler.
SELECT DB_NAME() AS Veritabani, SYSUTCDATETIME() AS SunucuUtcZamani;

SELECT TOP (100)
    Id, OccurredUtc, Kind, Project, WebPart, EventName, Operation,
    SqlOperation, RepositoryMethod, Outcome, DurationMs, OperationId
FROM dbo.PortalLog_Table
WHERE Project = N'TBYS'
  AND WebPart = N'YasalFaizWP'
ORDER BY OccurredUtc DESC, Id DESC;

-- SELECT'in Operation alanına neden yazılamadığını mevcut şemada doğrular.
SELECT name AS KisitAdi, definition AS KisitTanimi
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID(N'dbo.PortalLog_Table')
  AND name IN (N'CK_PortalLog_Operation', N'CK_PortalLog_SqlOperation');

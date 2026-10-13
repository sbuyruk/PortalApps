/*
    Ekstre aktarimi eszamanlilik incelemesi.
    Bu script salt-okunurdur; INSERT, UPDATE, DELETE veya DDL calistirmaz.
    SSMS'te hedef PortalApps veritabaninda calistirilmali.
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.NakitBagisHareket_Table', N'U') IS NULL
BEGIN
    RAISERROR('NakitBagisHareket_Table bulunamadi.', 16, 1);
    RETURN;
END;

IF COL_LENGTH(N'dbo.NakitBagisHareket_Table', N'EkstreAktarmaId') IS NULL
BEGIN
    RAISERROR('NakitBagisHareket_Table.EkstreAktarmaId bulunamadi.', 16, 1);
    RETURN;
END;

/* 1) Kolonun veri tipi, NULL kabul edip etmedigi ve varsayilan bilgisi */
SELECT
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.precision AS [Precision],
    c.scale AS [Scale],
    c.is_nullable AS IsNullable,
    dc.definition AS DefaultDefinition
FROM sys.columns c
INNER JOIN sys.types t
    ON t.user_type_id = c.user_type_id
LEFT JOIN sys.default_constraints dc
    ON dc.parent_object_id = c.object_id
   AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID(N'dbo.NakitBagisHareket_Table')
  AND c.name = N'EkstreAktarmaId';

/* 2) EkstreAktarmaId ile ilgili mevcut indexler */
SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    i.is_primary_key AS IsPrimaryKey,
    i.is_unique_constraint AS IsUniqueConstraint,
    i.is_disabled AS IsDisabled,
    ic.key_ordinal AS KeyOrdinal,
    c.name AS ColumnName,
    ic.is_included_column AS IsIncludedColumn
FROM sys.indexes i
INNER JOIN sys.index_columns ic
    ON ic.object_id = i.object_id
   AND ic.index_id = i.index_id
INNER JOIN sys.columns c
    ON c.object_id = ic.object_id
   AND c.column_id = ic.column_id
WHERE i.object_id = OBJECT_ID(N'dbo.NakitBagisHareket_Table')
  AND c.name = N'EkstreAktarmaId'
ORDER BY i.name, ic.key_ordinal;

/* 3) NULL, 0 ve dolu EkstreAktarmaId dagilimi */
SELECT
    COUNT_BIG(*) AS TotalRowCount,
    SUM(CASE WHEN EkstreAktarmaId IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN EkstreAktarmaId = 0 THEN 1 ELSE 0 END) AS ZeroCount,
    SUM(CASE WHEN EkstreAktarmaId IS NOT NULL AND EkstreAktarmaId <> 0 THEN 1 ELSE 0 END) AS ReferencedCount
FROM dbo.NakitBagisHareket_Table;

/* 4) Ayni EkstreAktarmaId icin birden fazla hareket var mi? */
SELECT
    EkstreAktarmaId,
    COUNT_BIG(*) AS MovementCount,
    MIN(Id) AS FirstMovementId,
    MAX(Id) AS LastMovementId
FROM dbo.NakitBagisHareket_Table
WHERE EkstreAktarmaId IS NOT NULL
  AND EkstreAktarmaId <> 0
GROUP BY EkstreAktarmaId
HAVING COUNT_BIG(*) > 1
ORDER BY MovementCount DESC, EkstreAktarmaId;

/* 5) Duplicate bulunan hareketlerin ayrintilari */
;WITH DuplicateEkstre AS
(
    SELECT EkstreAktarmaId
    FROM dbo.NakitBagisHareket_Table
    WHERE EkstreAktarmaId IS NOT NULL
      AND EkstreAktarmaId <> 0
    GROUP BY EkstreAktarmaId
    HAVING COUNT_BIG(*) > 1
)
SELECT h.*
FROM dbo.NakitBagisHareket_Table h
INNER JOIN DuplicateEkstre d
    ON d.EkstreAktarmaId = h.EkstreAktarmaId
ORDER BY h.EkstreAktarmaId, h.Id;

/* 6) Ilgili EkstreAktarma kayitlarinin aktarim durumu */
SELECT
    h.EkstreAktarmaId,
    COUNT_BIG(*) AS MovementCount,
    MIN(CASE WHEN e.AktarildiMi = 1 THEN 1 ELSE 0 END) AS AnyTransferredFlag,
    MAX(CASE WHEN e.AktarildiMi = 0 THEN 1 ELSE 0 END) AS AnyNotTransferredFlag
FROM dbo.NakitBagisHareket_Table h
LEFT JOIN dbo.EkstreAktarma_Table e
    ON e.Id = h.EkstreAktarmaId
WHERE h.EkstreAktarmaId IS NOT NULL
  AND h.EkstreAktarmaId <> 0
GROUP BY h.EkstreAktarmaId
HAVING COUNT_BIG(*) > 1
ORDER BY h.EkstreAktarmaId;

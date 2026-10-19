-- Portal Logging ortak kayıt tablosu.
--
-- Bu migration SQLLog_Table'a dokunmaz. SQLLog_Table eski loglama
-- akışının parçasıdır ve bu yapının dışında bırakılmıştır.
--
-- PortalActivityLog_Table ve PortalAuditLog_Table mevcut uygulamaların
-- geriye dönük uyumluluğu için korunur. Yeni Portal Logging yazıcıları
-- bu tablodan itibaren PortalLog_Table hedefini kullanacaktır.

IF OBJECT_ID(N'dbo.PortalLog_Table', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalLog_Table
    (
        Id bigint IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_PortalLog PRIMARY KEY,
        OccurredUtc datetime2(3) NOT NULL,
        Kind varchar(20) NOT NULL,
        Project nvarchar(64) NOT NULL,
        WebPart nvarchar(256) NOT NULL,
        UserLogin nvarchar(256) NOT NULL,
        PagePath nvarchar(1024) NOT NULL,
        OperationId uniqueidentifier NOT NULL,
        EventName nvarchar(128) NULL,
        Operation varchar(16) NULL,
        Outcome varchar(16) NULL,
        Detail nvarchar(max) NULL,
        EntityName nvarchar(128) NULL,
        EntityId nvarchar(256) NULL,
        ErrorType nvarchar(128) NULL,
        ErrorLevel varchar(16) NULL,
        UserMessage nvarchar(1000) NULL,
        TechnicalSummary nvarchar(2000) NULL,
        StackTrace nvarchar(max) NULL,
        InnerException nvarchar(max) NULL,
        SqlOperation varchar(8) NULL,
        RepositoryMethod nvarchar(256) NULL,
        DurationMs bigint NULL,
        CONSTRAINT CK_PortalLog_Kind
            CHECK (Kind IN ('UserActivity', 'Audit', 'Error', 'SqlTrace')),
        CONSTRAINT CK_PortalLog_Operation
            CHECK (Operation IS NULL OR Operation IN ('INSERT', 'UPDATE', 'DELETE', 'LOGIN', 'APPROVE', 'REJECT')),
        CONSTRAINT CK_PortalLog_SqlOperation
            CHECK (SqlOperation IS NULL OR SqlOperation IN ('SELECT', 'INSERT', 'UPDATE', 'DELETE')),
        CONSTRAINT CK_PortalLog_Duration
            CHECK (DurationMs IS NULL OR DurationMs >= 0)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PortalLog_Operation_Time'
      AND object_id = OBJECT_ID(N'dbo.PortalLog_Table')
)
    CREATE INDEX IX_PortalLog_Operation_Time
        ON dbo.PortalLog_Table (OperationId, OccurredUtc ASC, Id ASC);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PortalLog_Project_Time'
      AND object_id = OBJECT_ID(N'dbo.PortalLog_Table')
)
    CREATE INDEX IX_PortalLog_Project_Time
        ON dbo.PortalLog_Table (Project, OccurredUtc DESC, Id DESC);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PortalLog_WebPart_Time'
      AND object_id = OBJECT_ID(N'dbo.PortalLog_Table')
)
    CREATE INDEX IX_PortalLog_WebPart_Time
        ON dbo.PortalLog_Table (WebPart, OccurredUtc DESC, Id DESC);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PortalLog_User_Time'
      AND object_id = OBJECT_ID(N'dbo.PortalLog_Table')
)
    CREATE INDEX IX_PortalLog_User_Time
        ON dbo.PortalLog_Table (UserLogin, OccurredUtc DESC, Id DESC);
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_PortalLog_Kind_Time'
      AND object_id = OBJECT_ID(N'dbo.PortalLog_Table')
)
    CREATE INDEX IX_PortalLog_Kind_Time
        ON dbo.PortalLog_Table (Kind, OccurredUtc DESC, Id DESC);
GO

-- Yeni audit başlıkları PortalLog_Table'a yazılacaktır. Mevcut
-- PortalAuditChange_Table kayıtları ve AuditLogId ilişkisi korunur;
-- PortalLogId yeni kayıtlar için geçiş alanıdır.
IF OBJECT_ID(N'dbo.PortalAuditChange_Table', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.PortalAuditChange_Table', N'PortalLogId') IS NULL
BEGIN
    ALTER TABLE dbo.PortalAuditChange_Table
        ADD PortalLogId bigint NULL;
END;
GO

-- Eski audit başlıkları AuditLogId ile yaşamaya devam eder. Yeni ortak
-- tablo auditleri yalnızca PortalLogId ile bağlanabileceği için eski
-- kolon geçiş süresince nullable yapılır; mevcut değerler korunur.
IF OBJECT_ID(N'dbo.PortalAuditChange_Table', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.PortalAuditChange_Table', N'AuditLogId') IS NOT NULL
   AND EXISTS
   (
       SELECT 1
       FROM sys.columns
       WHERE object_id = OBJECT_ID(N'dbo.PortalAuditChange_Table')
         AND name = N'AuditLogId'
         AND is_nullable = 0
   )
BEGIN
    ALTER TABLE dbo.PortalAuditChange_Table
        ALTER COLUMN AuditLogId int NULL;
END;
GO

IF OBJECT_ID(N'dbo.PortalAuditChange_Table', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.PortalAuditChange_Table', N'PortalLogId') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.foreign_keys
       WHERE name = N'FK_PortalAuditChange_PortalLog'
         AND parent_object_id = OBJECT_ID(N'dbo.PortalAuditChange_Table')
   )
BEGIN
    ALTER TABLE dbo.PortalAuditChange_Table
        ADD CONSTRAINT FK_PortalAuditChange_PortalLog
            FOREIGN KEY (PortalLogId) REFERENCES dbo.PortalLog_Table (Id);
END;
GO

IF OBJECT_ID(N'dbo.PortalAuditChange_Table', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.PortalAuditChange_Table', N'PortalLogId') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.indexes
       WHERE name = N'IX_PortalAuditChange_PortalLog'
         AND object_id = OBJECT_ID(N'dbo.PortalAuditChange_Table')
   )
    CREATE INDEX IX_PortalAuditChange_PortalLog
        ON dbo.PortalAuditChange_Table (PortalLogId);
GO

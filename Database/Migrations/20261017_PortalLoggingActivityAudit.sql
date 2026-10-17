-- Apply to the PortalApps application database before enabling new logging.
-- The script creates new objects only; Olay_Table and SQLLog_Table are unchanged.

IF OBJECT_ID(N'dbo.PortalActivityLog_Table', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalActivityLog_Table
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PortalActivityLog PRIMARY KEY,
        OccurredUtc datetime2(3) NOT NULL,
        Project nvarchar(64) NOT NULL,
        WebPart nvarchar(256) NOT NULL,
        UserLogin nvarchar(256) NOT NULL,
        PagePath nvarchar(1024) NOT NULL,
        OperationId uniqueidentifier NOT NULL,
        EventName nvarchar(128) NOT NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PortalActivityLog_Project_Time'
               AND object_id = OBJECT_ID(N'dbo.PortalActivityLog_Table'))
    CREATE INDEX IX_PortalActivityLog_Project_Time
        ON dbo.PortalActivityLog_Table (Project, OccurredUtc DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PortalActivityLog_User_Time'
               AND object_id = OBJECT_ID(N'dbo.PortalActivityLog_Table'))
    CREATE INDEX IX_PortalActivityLog_User_Time
        ON dbo.PortalActivityLog_Table (UserLogin, OccurredUtc DESC);
GO

IF OBJECT_ID(N'dbo.PortalAuditLog_Table', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalAuditLog_Table
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PortalAuditLog PRIMARY KEY,
        OccurredUtc datetime2(3) NOT NULL,
        Project nvarchar(64) NOT NULL,
        WebPart nvarchar(256) NOT NULL,
        UserLogin nvarchar(256) NOT NULL,
        PagePath nvarchar(1024) NOT NULL,
        OperationId uniqueidentifier NOT NULL,
        Operation varchar(6) NOT NULL,
        EntityName nvarchar(128) NOT NULL,
        EntityId nvarchar(256) NOT NULL,
        CONSTRAINT CK_PortalAuditLog_Operation
            CHECK (Operation IN ('INSERT', 'UPDATE', 'DELETE'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PortalAuditLog_Entity_Time'
               AND object_id = OBJECT_ID(N'dbo.PortalAuditLog_Table'))
    CREATE INDEX IX_PortalAuditLog_Entity_Time
        ON dbo.PortalAuditLog_Table (EntityName, EntityId, OccurredUtc DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PortalAuditLog_Project_Time'
               AND object_id = OBJECT_ID(N'dbo.PortalAuditLog_Table'))
    CREATE INDEX IX_PortalAuditLog_Project_Time
        ON dbo.PortalAuditLog_Table (Project, OccurredUtc DESC);
GO

-- Field changes will be populated only after entity-specific allowlists are in place.
IF OBJECT_ID(N'dbo.PortalAuditChange_Table', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalAuditChange_Table
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PortalAuditChange PRIMARY KEY,
        AuditLogId int NOT NULL,
        FieldName nvarchar(256) NOT NULL,
        OldValue nvarchar(2000) NULL,
        NewValue nvarchar(2000) NULL,
        CONSTRAINT FK_PortalAuditChange_AuditLog
            FOREIGN KEY (AuditLogId) REFERENCES dbo.PortalAuditLog_Table (Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PortalAuditChange_AuditLog'
               AND object_id = OBJECT_ID(N'dbo.PortalAuditChange_Table'))
    CREATE INDEX IX_PortalAuditChange_AuditLog
        ON dbo.PortalAuditChange_Table (AuditLogId);
GO

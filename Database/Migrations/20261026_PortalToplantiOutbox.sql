-- Uygulama yayımlanmadan önce hedef PortalApps veritabanında çalıştırılmalıdır.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.PortalToplantiOutbox_Table',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalToplantiOutbox_Table
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PortalToplantiOutbox PRIMARY KEY,
        MeetingId int NOT NULL,
        Payload nvarchar(max) NOT NULL,
        DeliveryPlan nvarchar(max) NULL,
        CreatedUtc datetime2(3) NOT NULL,
        NextAttemptUtc datetime2(3) NOT NULL,
        Status varchar(16) NOT NULL,
        AttemptCount int NOT NULL,
        LeaseId uniqueidentifier NULL,
        LeaseUntilUtc datetime2(3) NULL,
        SentUtc datetime2(3) NULL,
        LastError nvarchar(2000) NULL,
        UserLogin nvarchar(256) NOT NULL,
        WebPart nvarchar(256) NOT NULL,
        PagePath nvarchar(1024) NOT NULL,
        OperationId uniqueidentifier NOT NULL,
        CONSTRAINT CK_PortalToplantiOutbox_Status CHECK (Status IN ('Pending','Processing','Sent','Failed')),
        CONSTRAINT CK_PortalToplantiOutbox_Attempts CHECK (AttemptCount>=0)
    );
    CREATE INDEX IX_PortalToplantiOutbox_Due ON dbo.PortalToplantiOutbox_Table(Status,NextAttemptUtc,Id) INCLUDE (LeaseUntilUtc,MeetingId);
    CREATE INDEX IX_PortalToplantiOutbox_Meeting ON dbo.PortalToplantiOutbox_Table(MeetingId,Id) INCLUDE (Status);
END;
IF OBJECT_ID(N'dbo.PortalToplantiDelivery_Table',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PortalToplantiDelivery_Table
    (
        OutboxId int NOT NULL,
        Recipient nvarchar(320) COLLATE Latin1_General_100_CI_AS NOT NULL,
        SentUtc datetime2(3) NOT NULL,
        CONSTRAINT PK_PortalToplantiDelivery PRIMARY KEY (OutboxId,Recipient),
        CONSTRAINT FK_PortalToplantiDelivery_Outbox FOREIGN KEY (OutboxId) REFERENCES dbo.PortalToplantiOutbox_Table(Id)
    );
END;
COMMIT TRANSACTION;

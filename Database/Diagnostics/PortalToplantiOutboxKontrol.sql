-- İşlem gören ve yeniden denenecek görevler; e-posta gövdesi ve alıcı bilgileri gösterilmez.
SELECT Id,MeetingId,CreatedUtc,Status,AttemptCount,NextAttemptUtc,LeaseUntilUtc,SentUtc,LastError,OperationId
FROM dbo.PortalToplantiOutbox_Table
WHERE Status<>'Sent'
ORDER BY Id;

SELECT Status,COUNT_BIG(*) Adet,MIN(CreatedUtc) EnEskiKayitUtc
FROM dbo.PortalToplantiOutbox_Table
GROUP BY Status;

-- Yalnızca sebebi giderilmiş Failed kaydı için, yönetici incelemesinden sonra çalıştırılabilecek örnek:
-- Başarılı alıcı kayıtları korunur. Pending/Processing kayıtlarını bu şekilde sıfırlamayın.
-- DECLARE @Id int = /* İncelenen görev kimliği */;
-- UPDATE dbo.PortalToplantiOutbox_Table SET Status='Pending',AttemptCount=0,
--   NextAttemptUtc=SYSUTCDATETIME(),LastError=NULL,LeaseId=NULL,LeaseUntilUtc=NULL
-- WHERE Id=@Id AND Status='Failed';

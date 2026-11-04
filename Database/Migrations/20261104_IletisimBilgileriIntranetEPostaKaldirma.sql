-- Önce Database/Diagnostics/IntranetEPostaKaldirmaKontrol.sql sonuçlarını değerlendirin.
-- Eski uygulama ve toplantı göndericileri durdurulmuş olmalıdır.
-- Yeni uygulama paketi ve bu betik tamamlanmadan trafiği/göndericileri açmayın.
-- InternetEPosta değerlerini değiştirmez. Eski alanı ayrı arşiv tablosunda saklar.
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.IletisimBilgileri_Table',N'U') IS NULL
        THROW 50001,N'İletişim bilgileri tablosu bulunamadı.',1;
    IF COL_LENGTH(N'dbo.IletisimBilgileri_Table',N'InternetEPosta') IS NULL
        THROW 50002,N'Bildirimler için gerekli InternetEPosta sütunu bulunamadı.',1;

    IF COL_LENGTH(N'dbo.IletisimBilgileri_Table',N'IntranetEPosta') IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.sql_modules WHERE definition LIKE N'%IntranetEPosta%')
            THROW 50003,N'Eski e-posta alanına başvuran veritabanı modülleri var. Bağımlılıkları inceleyin.',1;

        IF OBJECT_ID(N'dbo.IletisimBilgileriIntranetEPostaArsiv_20261104') IS NOT NULL
            THROW 50004,N'E-posta arşiv tablosu zaten var. Mevcut arşiv incelenmeden işlem sürdürülemez.',1;

        -- Kaynak sütunun türünü korur. Kilit, arşivleme ile sütun kaldırma arasında veri değişmesini önler.
        EXEC sys.sp_executesql N'
            SELECT Id AS IletisimId,PersonelId,IntranetEPosta
            INTO dbo.IletisimBilgileriIntranetEPostaArsiv_20261104
            FROM dbo.IletisimBilgileri_Table WITH (TABLOCKX,HOLDLOCK);

            ALTER TABLE dbo.IletisimBilgileri_Table DROP COLUMN IntranetEPosta;';
        -- Constraint/index gibi bağımlılıklar otomatik silinmez; SQL hatasında tüm işlem geri alınır.
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

# NBYS parça 2 — banka grupları ve döviz bağış raporu

Dal: codex/nbys-banka-doviz-log-mesaj
Başlangıç commit'i: 4f1816e938d9d598a72488363a3dd6c8beffe3b6.
Kod değişikliğinden önce yerel master temizdi ve git ls-remote ile GitHub master ile aynı commit'te olduğu doğrulandı.
Önceki NBYSParametreGirisiWP parçasının testi kullanıcı tarafından başarılı olarak bildirildi. Kontrollü rollback dahil bütün özel senaryoların ayrı ayrı yapıldığı bu genel bildirimden çıkarılmadı.

## Kapsamın kaynak kodla netleştirilmesi

Önceki sohbet önerisinde banka tanımlarının düzenleme ekranı varsayılmıştı. Güncel kaynakta BankaTanimService.Save/Update/Delete için çağıran veya ayrı banka tanımı ekleme/düzenleme WebPart'ı bulunmadı.
Bu nedenle mevcut ekranların yerine yeni bir yönetim ekranı eklenmedi ve çağıranı olmayan banka yazma/audit altyapısı hazırlanmış gibi sunulmadı.
Bu dal yalnızca DovizleYapilanBagislarWP ve kullandığı iki SELECT yolunu taşır:

- BankaTanimService.GetGroups(PortalLogContext) → BankaTanimRepository.SelectGroups(PortalLogContext).
- NakitBagisHareketService.GetCurrencyTotalsByDateBank(..., PortalLogContext) → NakitBagisHareketRepository.SelectCurrencyTotalsByDateBank(..., tlDovizCinsi, PortalLogContext).

Diğer banka okuyucuları: EkstreAktarmaEditWP, EkstreListesiWP ve NakitBagisciKarti. Bu dal onların erişimlerini dönüştürmez.
EkstreAktarmaTransferService banka adından tanım okurken GetByName(name, transaction) kullanabiliyor. İncelenen servis/repository overload'ları doğrudan aynı transaction.Select üzerinden çalışıyor; bu yol korunuyor ve bu parçada merkezi loglamaya taşınmış sayılmıyor.

## Loglama ve SQL davranışı

- İlk HTTP açılışında NbysWebPartLog.RecordOpened; postback'te açılış kaydı yok.
- Aynı istekte açılış, iki SELECT ve olası Error ortak OperationId kullanır. Sonraki filtre postback'i ayrı istektir.
- SELECT adları BankaTanimRepository.SelectGroups ve NakitBagisHareketRepository.SelectCurrencyTotalsByDateBank.
- Yeni overload'lar mevcut NbysSqlExecutor kullanır. Bağlamın NBYS olduğu doğrulanır; sorgu/parametre değerleri SQL izine verilmez. Süre ve sonuç kaydedilir.
- Eski imzalar korunur. Rapor SQL'i tek BuildCurrencyTotalsQuery yardımcısında paylaşılır; sorgunun filtre, gruplama, tarih parametre biçimi ve toplam hesaplaması değiştirilmez.
- Bağlamlı okuma log ayarı kapalıyken de DbClass SQL yazıcısına geri dönmez. Parametresiz diğer çağıranlar eski yolunda kalır.
- Rapor yalnızca okur: INSERT/UPDATE/DELETE, audit ve iş transaction'ı yoktur. İki bağımsız okuma için yeni transaction açılmadı.
- Banka grup tablosu boşsa bağlamlı servis boş liste döndürür; NULL/boş grup adları filtre seçeneklerine eklenmez. “Tüm Bankalar” seçeneği korunur.
- Yeni kaynak dosyası, proje/feature kaydı, migration veya paket değişikliği gerekmez. Üretilmiş ASCX dosyaları değiştirilmedi.

## Mesaj incelemesi ve değişiklikler

Bu ekranda mevcut kayıt başarı mesajı veya yazma işlemi yok.
TabloJson içindeki ExceptionHelper.PublishException ve hatayı yutup boş JSON döndürme kaldırıldı. Hata event sınırına ulaşır ve NbysWebPartLog ile teknik kayda yazılır.
Page_Load, tüm mevcut filtre/yenileme olayları ve kapatma olayı korunmuş imzalarıyla ortak hata sınırına bağlandı; ThreadAbortException yeniden fırlatılır ve hata diye kaydedilmez.
Teknik hata için güvenli Türkçe mesaj ve başvuru kodu gösterilir; exception.Message/SQL/stack trace kullanıcıya aktarılmaz. Aynı exception zincirinin yeniden loglanması/bildirilmesi mevcut yardımcı tarafından engellenir.
Geçersiz/eksik filtrede “Raporu görüntülemek için geçerli bir yıl, ay ve banka seçin.” uyarısı gösterilir; sorgu çalıştırılmaz.
Sorgu başarılı olup sonuç boşsa tablo boş diziyle hazırlanır ve “Seçilen filtrelere uygun döviz bağışı bulunamadı.” bilgi mesajı gösterilir. Teknik hata, boş sonuç olarak sunulmaz.

## Etkinleştirme

Çalışan SharePoint web uygulamasının web.config dosyasında PortalLogging.EnabledWebParts değerine NBYS:DovizleYapilanBagislarWP ekleyin.
Önceki NBYS:NBYSParametreGirisiWP ile IKYS/TBYS değerlerini koruyun. NBYS projesinin tamamını EnabledProjects üzerinden açmayın.
Yeni migration yoktur. Hedef ortamda mevcut birleşik dbo.PortalLog_Table şeması ve teknik kayıt izinleri ayrıca kontrol edilmelidir. Bağlantı mevcut TSKGV_Connection üzerinden okunur.
Veritabanına, yapılandırmaya ve deploy ortamına Codex tarafından müdahale edilmedi.

## Kullanıcının Visual Studio 2026 / TEST doğrulaması

1. Güncel solution/NBYS WSP'yi derleyip TEST'te yayımlayın.
2. DovizleYapilanBagislarWP'yi açın. İlk istekte bir UserActivity ve iki SELECT izi beklenir. OperationId aynı; repository/metot adları yukarıdaki gibi olmalıdır. Audit/yazma kaydı beklenmez.
3. Banka, ay ve yıl filtrelerini değiştirin; Yenile düğmesini deneyin. Her istekte rapor SELECT izi oluşmalı; yeni WebPartOpened oluşmamalıdır. Tutar/satır sonuçlarını aynı filtrelerin önceki sürüm çıktısıyla karşılaştırın.
4. Sonuç bulunmayan filtrede boş tablo ve tek bilgi mesajını kontrol edin. Boş banka tanım tablosunda sayfa teknik hataya düşmeden “Tüm Bankalar” seçeneğiyle çalışmalıdır.
5. Eksik/geçersiz filtre senaryosunda anlamlı uyarı ve sıfır rapor SELECT'i beklenir.
6. TEST'te kontrollü SELECT hatasında başarısız SqlTrace ile tek güvenli hata mesajı/başvuru kodu beklenir. Error ve SELECT OperationId eşleşmeli; ham teknik ayrıntı ve ayrıca boş sonuç mesajı görünmemelidir.
7. Kapatma yönlendirmesi teknik hata kaydı üretmemelidir.
8. Loglama kapalıyken rapor çalışmalı; bu ekranın iki okumasında yeni veya legacy SQL çift kaydı oluşmamalıdır.

Mevcut tarih davranışının sınırı: “Tüm Aylar” seçimi bitişi 1 Aralık olarak hesaplıyor; aylık sorgular bitiş gününü mevcut tarih biçimiyle kullanıyor. Bu dal tarih aralığını değiştirmez ve yılın tamamını kapsadığına dair bir düzeltme iddiası taşımaz. Tarih filtresi doğruluğu ayrı bir işlev düzeltmesi gerektirir.

Codex build/test/debug çalıştırmadı. Statik metot imzası, çağrı zinciri, SQL gövdesi, diff ve UTF-8/BOM kontrolü yapıldı. Çalışma zamanı sonucu kullanıcı doğrulamasını bekler.
Commit/push/PR/merge kullanıcıya aittir. Sonraki parçaya test sonucu veya test beklemeden devam yönünde açık talimat sonrasında geçilir.
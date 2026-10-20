# TBYS açılış ve hata logları — inceleme noktası 2

Dal: `codex/tbys-webpart-activity-errors`.
Başlangıç: `4585ee21eda6993e374c478bf53d08affb9502ca`.
Değişiklik öncesinde master temizdi; master, origin/master ve GitHub master aynı commit'teydi.

## Değişen davranış

74 WebPart'ın OnLoad akışında, yalnızca ilk istekte ortak PortalLogRepository.TryRecordWebPartOpened çağrılır. Postback açılış kaydı üretmez. Repository kayıt yazılamadığında kullanıcı işlemini engellemez.

289 mevcut aktif catch bloğuna ortak teknik hata kaydı eklendi. Önceki envanterdeki 297 metinsel catch eşleşmesi yorum satırlarını ve model kodunu da kapsıyordu. Yeni 74 OnLoad hata sınırı, Page_Load sırasında dışarı taşan hataları güvenli mesaja dönüştürür.

Yakalanan hatanın işlem adı `TBYS.WebPartAdi.MetotAdi` biçiminde EventName alanına yazılır. Mevcut şemada Operation yalnızca INSERT, UPDATE, DELETE, LOGIN, APPROVE ve REJECT değerlerini kabul ettiğinden serbest metot adı bu alana yazılmaz.

Kullanıcıya ham exception yerine şu mesaj gösterilir:

> İşlem tamamlanamadı. Lütfen tekrar deneyiniz. Başvuru kodu: {OperationId}

On doğrudan ex.Message gösterimi kaldırıldı. Loglama kapalı olsa da hata sunumu güvenli kalır; eski ExceptionHelper.PublishException/LogHelper yoluna geri dönülmez. ExceptionHelper'ın dosya yükleme ve toplu işlemlerde kullanılan hata listeleri, mevcut yardımcı metot imzalarıyla uyum için korundu; sunumları TBYS yardımcısına taşındı.

Mevcut geri dönüşler, başarısızlık bayrakları, modal kapatma ve toplu işlem devam davranışları korundu. Daha önce sessiz kalan yardımcı catch blokları teknik kayıt üretir; hepsi yeni popup açmaz. Yeniden fırlatılan hatalar stack bilgisini korur. Response.Redirect kaynaklı ThreadAbortException yeniden fırlatılır; hata kaydı veya hata bildirimi üretmez.

TBYS'ye ait TbysWebPartLog yardımcısı aynı istekte aynı exception zincirinin tekrar loglanmasını ve tekrar bildirilmesini önler. Farklı hatalar ayrı teknik kayıtlardır; toplu hata listesi tek güvenli kullanıcı mesajı üretir.

## Ortak katmandaki sınırlı değişiklik

Utility/Logging/PortalTechnicalLog.cs dosyasında TBYS hata kaydına UserMessage eklenir. Kullanıcı metni tek ortak sabitten alınır. TBYS teknik özetleri mevcut 2000 karakterlik sütuna sığdırılır; detay ve stack ayrı alanlarda tutulur.

TBYS teknik bağlantılarında Enlist=false kullanılır. Böylece hata/teknik kayıt legacy TransactionScope'a katılmaz ve iş rollback'inde kaybolmaz. Bu bağlantı tercihi yalnızca Project=TBYS için uygulanır. Diğer projelerin kaynakları değiştirilmedi.

Yeni tablo/migration yoktur. Üretilmiş ASCX dosyaları değiştirilmedi.

## Etkinleştirme ve kullanıcı doğrulaması

Birleşik PortalLog_Table şeması (`Database/Migrations/20261019_PortalLoggingUnified.sql`) TEST veritabanında mevcut olmalı. Log yazıcıları mevcut host web.config etkinleştirme ayarlarını kullanır. Dar pilot için mevcut `PortalLogging.EnabledWebParts` değerine, önceki değerleri koruyarak örneğin `TBYS:YasalFaizWP;TBYS:TaahhutListesiWP` eklenebilir. Bu parça otomatik etkinleştirme yapmaz.

Kullanıcı Visual Studio 2026'da build yapıp TEST sunucusunda aşağıdaki senaryoları doğrulamalı:

1. Etkin WebPart'ı ilk açışta Project=TBYS, Kind=UserActivity, EventName=WebPartOpened, Outcome=Succeeded kaydı oluşur. Kullanıcı, sayfa yolu, UTC zaman ve OperationId doludur. Postback yeni açılış kaydı üretmez.
2. Önceden ham mesaj gösteren bir akışta kontrollü hata oluşturulur. Kullanıcı yalnızca güvenli mesaj ve başvuru kodunu görür. Aynı kod PortalLog_Table'daki Error kaydında bulunur; teknik özet, stack ve inner exception yalnızca logda kalır.
3. Hata iç katmanda yakalanıp yeniden fırlatıldığında aynı exception zinciri için mükerrer kayıt/bildirim oluşmaz. Toplu dosya yüklemede hata listeleri tek güvenli mesajla gösterilir.
4. Başarılı yönlendirme, sayfa kapatma ve PDF/Excel akışları normal çalışır; ThreadAbortException hata kaydı oluşmaz.
5. Açılış logu yazılamadığında ekranın normal akışı sürer. Logging ayarı kapalıyken hata mesajı güvenlidir ve eski ULS hata yoluna dönmez.
6. Mevcut TransactionScope içindeki hata sonrasında iş verisi rollback olurken bağımsız teknik hata kaydı korunur.

Codex build, test veya debug çalıştırmadı. Statik kaynak incelemesinde 74 bağlam/açılış bağlantısı, mevcut catch kapsamı, proje dosyasındaki yeni kaynak kaydı, UTF-8/BOM korunumu ve üretilmiş dosyaların değişmemesi kontrol edildi. git diff --check başarılıdır. Çalışma zamanı sonuçları kullanıcı doğrulamasını bekler.

## Sonraki parçalar

Servis/repository SQL izleri, transaction-aware çağrı zincirleri, iş audit'i ve alan değişiklikleri bu parçada tamamlanmadı. Mevcut servislerdeki OlayKayit ve repository/DbClass üzerinden dolaylı eski SQL log yolları sonraki parçalarda ele alınacak. Model/TBYS/KiraBakiyeDevriOtomatik içindeki sonuç sözlüğüne taşınan hata metni ve kalan normal kullanıcı mesajları ayrıca dönüştürülecek. Bu nedenle TBYS geçişi bütünüyle tamamlandı kabul edilmemelidir.

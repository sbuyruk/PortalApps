# IKYS eski loglamayı devreden çıkarma — servisler, parça 1

Dal: `codex/ikys-eski-log-servisleri-1`

Başlangıç: yerel `master` temizdi; canlı uzak `master`, yerel `master` ve HEAD aynı commit'teydi: `9e3a792ccbc1bf49fb58e273014b26d113291a8f`. İlk kontrolde görülen eski yerel commit sonrasında eşitlik yeniden doğrulandı ve değişiklik dalı bu güncel tabandan açıldı.

## Davranış

`PortalLogContext` alan IKYS servisleri, merkezi loglama kapatıldığında bağlamsız eski overload'a dönmez. İş verisi mevcut merkezi işlem yolunda, aynı `SqlTransactionContext` üzerinden yürütülür. Kendi transaction'ını oluşturan servis onu tamamlar; dışarıdan transaction alan overload işlemi çağıranın tamamlamasına bırakır.

`PortalLogRepository.RecordAudit`, ayar kapalıysa `0` döndürür. Bu durumda `RecordChanges` yardımcıları ayrıntı yazmayı da atlar. Sıfır dışındaki kimlikler için mevcut doğrulama devam eder. Merkezi loglama açıkken audit başlığı veya ayrıntısının yazılamaması yakalanıp başarıya çevrilmez; istisna mevcut transaction'ın geri alınmasına neden olur.

`GorevTanimService.ChangeAssignment` içindeki ek kontrol iş verisinin görev ataması güncellendikten sonradır. Loglama kapalı olduğunda görev ataması yine yapılır; yalnızca audit ayrıntısı atlanır.

## Değişiklik kapsamı

25 servis dosyasında 75 yazma ve 2 okuma fallback'i kaldırıldı. 25 `RecordChanges` yardımcısına ve `ChangeAssignment` içindeki doğrudan audit ayrıntısı yazımına sıfır kimlik kontrolü eklendi.

Servisler: `Aile`, `BirimTanim`, `BulunmamaSebebi`, `DereceKademeDegisim`, `Egitim`, `GorevOnay`, `GorevTanim`, `Harcirah`, `IletisimBilgileri`, `IsBilgileri`, `IsTecrube`, `IzinHareket`, `IzinTalep`, `IzinTanim`, `Kimlik`, `Kurs`, `MaasHareket`, `Mahsup`, `Personel`, `ResmiTatil`, `TahsilTanim`, `UcretTanim`, `UnvanTanim`, `YabanciDil`, `Yoklama`. Dosyalar `Model/Services/IKYS/` altında, `Service.cs` son ekiyle bulunur.

Yazma kapsamı `Save`, `Update`, `Delete` ve mevcut iki `DeleteByGrupId` fallback'ini içerir. Okuma kapsamı `GorevTanimService.GetByPersonelId` ve `KimlikService.GetByTcKimlikNo` metotlarının bağlamlı overload'larıdır; loglama kapalıyken de bağlam korunur.

`SalaryScaleWriteService.AuditScale` zaten sıfır kimlik kontrolü içerdiğinden değiştirilmedi. Audit'in sahibi olan mevcut servis/repository katmanları korunur; ikinci audit yazıcısı eklenmedi. Audit alan listeleri, SQL izi ve teknik hata yazıcıları değiştirilmedi.

## Bu parçanın sınırı

Bu dal IKYS'nin veya solution'ın eski loglamasını bütünüyle kapatmaz. Bağlamsız overload'lar, `OlayKayit`, eski bayraklar ve `DbClass.SorguyuLogla` sonraki parçalar için hâlâ mevcuttur. Ekranlarda loglama kapalıyken bağlamsız yolu seçen koşullar da bu dalda değiştirilmedi. Böyle bir ekran çağrısı hâlâ eski tabloya yazabilir; bağlamlı servise girmeyen bir çağrı bu parçanın kabul ölçütü değildir.

Özellikle `PersonelGirisiWP`, `EskiPersonelWP`, izin/talep/dönem/mahsup ekranları ve toplu işlemler sonraki IKYS parçalarında ayardan bağımsız transaction yoluna taşınmalıdır. Buradaki servis düzeltmeleri o işlemler için ön koşuldur.

Veritabanı şeması, geçmiş veriler, SharePoint ekranları, feature/paket bağlantıları ve `*.ascx.g.cs` dosyaları değiştirilmedi. Yeni kaynak dosyası veya proje Compile kaydı eklenmedi.

## Statik kontroller

- `git diff --check` temiz.
- Düzenlenen 25 servis dosyasında `PortalLogSettings.IsEnabled` ile eski overload'a dönüş kalmadı; IKYS repository'lerinde de bu ayara göre fallback bulunmadı.
- Diff'in yalnızca belirtilen fallback kaldırmaları, 26 audit kontrolü ve bunların boşluk düzenlemelerinden oluştuğu HEAD ile karşılaştırıldı.
- Düzenlenen servis dosyaları UTF-8 olarak okunabildi; BOM durumları HEAD ile aynı, çalışma dosyalarının satır sonları CRLF.
- Kaynak üzerinden 154 transaction içeren servis metodu ve 160 transaction parametreli IKYS repository metodu tarandı. Servis repository çağrılarında transaction argümanı eksikliği veya bu metotlarda doğrudan ayrı DB erişimi işaretlenmedi. Bu tarama derleyici/çalışma zamanı doğrulaması değildir.
- Mevcut merkezi audit başlık/ayrıntı yazımı aynı transaction'ı kullanıyor. `PortalTechnicalLog` bağımsız SQL/hata kayıt tasarımı bu dalda değiştirilmedi.

Codex Windows bilgisayarında build, test veya debug çalıştırmadı. Commit, push, PR, merge, yayın ve çalışma zamanı doğrulaması kullanıcıya aittir.

## Kullanıcının uygulayacağı kabul senaryoları

İşlemleri Test veritabanında, kontrollü test kayıtları ve ayrı bir test zaman aralığı/işlem kodu ile yürütün. SQL kontrolleri salt okuma olmalıdır; eski tabloları veya geçmiş kayıtları silmeyin.

1. **Derleme ve çağrı kapsamı:** Visual Studio 2026'da solution'ı derleyin ve Test sunucusuna yayımlayın. Aşağıdaki kapalı loglama senaryolarında `PortalLogContext` alan overload'a gerçekten girildiğini Visual Studio'da doğrulayın. Eski ekran koşulu bağlamsız overload'ı seçiyorsa bu senaryoyu doğrudan bağlamlı servis çağrısı üzerinden doğrulayın ve ekranı sonraki parçaya not edin.
2. **Açık/kapalı ayar:** Bir kez IKYS merkezi loglaması açıkken, bir kez kapalıyken deneyin. `IsEnabled` iki anahtarı OR ile değerlendirir: IKYS'nin `PortalLogging.EnabledProjects` içinde bulunmaması tek başına yeterli değildir; test WebPart'ının `PortalLogging.EnabledWebParts` girdisi de kapalı olmalıdır. Diğer proje ayarlarını koruyun ve senaryo sonunda başlangıç ayarlarını geri yükleyin.
3. **Temel CRUD:** `AileService`, `EgitimService` ve `ResmiTatilService` bağlamlı ekleme/güncelleme/silme çağrılarını deneyin. Açık durumda işlem başına beklenen tek audit başlığı ve mevcut alan politikasına uygun ayrıntılar; kapalı durumda başarılı iş verisi ve audit ayrıntısı kimlik hatası olmaması beklenir. Her iki durumda bu servis çağrısından `Olay_Table` veya `SQLLog_Table` kaydı üretilmemelidir.
4. **Dış transaction — tamamla/geri al:** Aynı `SqlTransactionContext` ile bağlamlı personel ve kimlik/iş bilgileri çağrılarını çalıştırın. Başarıda ikisi de kaydedilmeli. İkinci adımda kontrollü hata oluşturulduğunda veya çağıran `Complete` yapmadığında iş verileri ve varsa audit kayıtları birlikte geri alınmalı. Servis dış transaction'ı kendi başına tamamlamamalı. Bu senaryoyu loglama açık ve kapalı tekrarlayın.
5. **Görev ataması:** Bir görevi farklı personele atayın. `GorevTanimService.Update` içindeki eski/yeni personelin `IsBilgileri.GorevId` değişiklikleri loglama kapalıyken de tamamlanmalı; `RecordAuditChange(0, ...)` hatası oluşmamalı. Açık durumda görev ve gerçekten değişen iş bilgileri için birer audit beklenir; bunlar farklı iş verisi değişiklikleridir. Sonraki adımın hatasında hepsi birlikte geri alınmalı.
6. **İzin ve mahsup:** Transaction alan `IzinHareketService`, `IzinTalepService`, `MahsupService` ve ilgili `IzinDonemService` çağrılarını aynı transaction içinde deneyin. Açık/kapalı loglama iş sonucunu veya rollback kapsamını değiştirmemeli. Bu parçaya taşınmamış ekran dallarından bütün IKYS için sonuç çıkarmayın.
7. **Grup silme:** Bağlamlı `UcretTanimService.DeleteByGrupId` ve `MaasHareketService.DeleteByGrupId` çağrılarını kontrollü test grubunda doğrulayın. Kapalı loglamada iş silmesi tamamlanmalı; açık loglamada mevcut audit kapsamı korunmalı. Ücret tanımı silmesinde başlıklar silinen satır kimlikleriyle ilişkilendirilmeli.
8. **Gerçek audit hatası:** Merkezi loglama açıkken yalnızca Test ortamında audit başlığı veya ayrıntısı yazımını kontrollü olarak başarısız kılın. Servis başarı dönmemeli; iş verisi geri alınmalı. Bu dal gerçek audit hatasını “loglama kapalı” gibi atlamamalı. Oluşturulan hata, mevcut WebPart hata sınırından geçtiğinde teknik hata kaydı bağımsız olarak kalmalı.
9. **SQL izi ve çift kayıt:** Açık loglamada başarılı/başarısız SQL izi ile hata kayıtlarının mevcut OperationId ve yöntem ilişkilerini kontrol edin. Geri alınan transaction'ın başarılı SQL adımına ait teknik izi kalabilir; bu iş verisinin commit edildiği anlamına gelmez. Kapalı bağlamda merkezi teknik yazıcılar da kayıt üretmemeli. Diğer kullanıcıların eşzamanlı işlemlerini sayımlara karıştırmayın.
10. **Geçersiz/boş kayıtlar ve geriye uyumluluk:** Geçersiz kimlik veya bulunamayan kayıtta mevcut dönüş davranışını kontrol edin. Loglama açıkken mevcut IKYS ekranlarında personel, izin, tanım ve maaş işlemlerini tekrar deneyin; yeni çift audit veya kullanıcıya ham teknik hata oluşmamalı.

İlk parçanın kabul ölçütü bağlamlı servis yollarıdır. Ekran fallback'leri ve solution genelinde eski tablolara sıfır yazı hedefi sonraki parçalarla tamamlanacaktır.

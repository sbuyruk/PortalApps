# IKYS eski loglama geçişi — parça 4

Başlangıç: temiz yerel `master` ve canlı uzak `master` aynı commit'te doğrulandı: `e3662bf903cf7dc646a311343e25e67bfe2e2608` (PR #540).

Dal: `codex/ikys-izin-talep-listeleri-eski-log`.

## Değişiklik ve inceleme noktaları

Bu parça `IzinTalepListesiWP` ve `MazeretIzinTalepListesiWP` ekranlarının kontrol, onay ve ret yazma akışlarını kapsar. Bu ekranlarda silme akışı yoktur; talep giriş ekranının silme işlemleri önceki parçadadır.

- Her iki `.ascx.cs` dosyasında `OnaylaBtn_Click`, `KabulBtn_Click` ve `ReddetBtn_Click`, yeni `TalebiIsle` yardımcısını çağırır. Log ayarı transaction seçmez; yardımcının transaction'ı her durumda açılır. `loggingEnabled`, nullable transaction, bağlamsız hareket yazımı ve eski hareket kontrol yardımcısı kaldırıldı.
- `TalebiIsle`: talep aynı transaction ile güncel olarak okunur. Mevcut repository overload'ı `UPDLOCK, HOLDLOCK` kullanır. Ekranın izin türü, güncel durum ve hareket varlığı yazmadan önce kontrol edilir. Kontrol için işlem/dilekçe bekleyen; onay için kontrol edilmiş; ret için işlem/dilekçe bekleyen veya kontrol edilmiş talep gerekir. Kayıtlara işlenmiş, reddedilmiş, silinmiş veya hareketi zaten bulunan talebe yeni yazı ve bildirim yapılmaz. Aynı talebin eşzamanlı işlemleri talep kilidinde sıralanır; diğer henüz taşınmamış yolların tamamı için eşzamanlılık garantisi verilmez.
- `TalepHareketiniKaydet`: talep, hareket, ücretli/mazeret dönem bakiyesi ve ilgili audit aynı transaction'dadır. Dönem personel ve izin türüyle eşleşmelidir. Eksik/uyumsuz dönem, başarısız bakiye yazımı, hareket ekleme veya audit hatası bütün iş yazılarını geri alır. `Complete` sonrasında `Dispose` commit'i başarısız olursa yardımcı başarıyla dönmez.
- Hareketin önceki/kullanılan/kalan izin alanları, dönem bakiyesi güncellendikten sonra hareketin tek INSERT'inde yazılır. Eski INSERT ardından yalnızca bu alanları dolduran UPDATE kaldırıldı. Böylece aynı hareketin oluşturulması için gereksiz ikinci audit üretilmez. Merkezi servislerin mevcut audit alan kümeleri değiştirilmedi; bakiye değişimi dönem auditinde, hareket ilişkileri/tarihleri hareket auditinde kalır.
- Genel listedeki ücretsiz izin yolunda `IsBilgileriService.GetById(PersonelId)` yerine `GetByPersonelId(PersonelId, transaction, context)` kullanılır. Personel de aynı transaction ile okunur. Sivil personelde dönem başlangıcının takvim günü farkı + 1 kadar ötelenmesi ve asker istisnası korunur. İş bilgileri/dönem başlangıcı eksikse kısmi onay yapılmaz. Bu kayıt okumasındaki kimlik düzeltmesi özellikle incelenmelidir.
- Girilmiş açıklama güncel talebe uygulanır; boş açıklama mevcut açıklamayı korur. Mazeret onay/ret yolunda eskiden nesne okunmadan önce yapılan ve okuma ile kaybolan açıklama ataması düzeltildi.
- Başarı mesajı ve `BildirimGonder` çağrısı, yardımcı döndükten ve commit tamamlandıktan sonradır. Bildirim hatası iş kaydını geri almış gibi gösterilmez: ayrı merkezi teknik hata ve işlemin kaydedildiğini belirten güvenli Türkçe mesaj üretilir. Bildirim kapalıysa gönderim yardımcısı çağrılmaz. Burada çalışma zamanı e-postası gönderilmedi.
- İki ekranın `ReportError` metotlarından `ExceptionHelper` geri dönüşü kaldırıldı. Merkezi teknik yazıcı ayarı kendisi değerlendirir; normal yönlendirme `ThreadAbortException` hata kaydı oluşturmaz. Teknik SQL/hata yazıcılarının bağımsız bağlantı tasarımına dokunulmadı.

## Statik kontroller

- `git diff --check` temiz. İki kaynakta UTF-8 doğrulandı; BOM durumu HEAD ile aynı, CRLF korundu.
- String ve yorumlar dışında ayraç dengesi; 11 etkin ASCX olay bağlantısı ve mevcut proje Compile kayıtları kontrol edildi. Yorum içindeki Timer bağlantısı etkin olay sayılmadı.
- İki transaction yardımcısındaki toplam 15 servis çağrısında transaction ve context aktarımı kontrol edildi. `IzinTalep`, `IzinHareket`, `IzinDonem`, `IsBilgileri`, `Personel` transaction overload'larının repository/audit zincirleri incelendi.
- Bu ekranların iş yazılarında log ayarına göre eski overload seçimi kalmadı. Yeni derlenecek dosya, ASCX, üretilen `*.ascx.g.cs`, feature, paket ve veritabanı şeması değiştirilmedi. Eski tablolar ve geçmiş veriler korunur.

Windows bilgisayarında build, test veya debug çalıştırılmadı. Commit, push, PR, merge, yayın ve çalışma zamanı kontrolleri kullanıcıya aittir.

## Kullanıcının uygulayacağı senaryolar

Kontrollü test kayıtlarıyla, merkezi IKYS/ekran log ayarları açıkken ve kapalıyken tekrarlayın. Test sonunda ayarları başlangıç değerlerine getirin.

1. Visual Studio 2026'da derleyin, Test sunucusuna yayımlayın. İki listenin açılışı, modal ayrıntıları, kontrol/onay/ret butonları ve liste yenilemesi çalışmalı.
2. Ücretli talepte kontrol ardından onay yapın. Talep kayıtlara işlenmeli; tek hareket, doğru eski/kullanılan/kalan izin alanları ve yalnız bir kez düşülmüş bakiye bulunmalı. Mazerette saat/dakika hesabını ve öğle aralığı örneklerini karşılaştırın. Diğer izin türlerinde gereksiz dönem bakiyesi yazılmamalı.
3. İşlem/dilekçe bekleyen ve kontrol edilmiş talebi reddedin; açıklama korunmalı/kaydedilmeli. Ret veya yalnız kontrol, hareket ve bakiye değişikliği oluşturmamalı.
4. Aynı talebi iki pencereden onaylayın; ayrıca kontrol/ret ve onay/ret yarışlarını deneyin. Yalnız geçerli durum geçişi yazılmalı; ikinci hareket, ikinci bakiye düşümü veya ikinci bildirim oluşmamalı. Silinmiş/yanlış ekran türündeki talep güvenli mesaj vermeli.
5. Ücretsiz izinde Personel.Id ile IsBilgileri.Id farklı olan sivil personel kullanın. Doğru iş bilgisi kaydı güncellenmeli; başlangıç takvim günü farkı + 1 kadar ötelenmeli. Askerde tarih değişmemeli. Eksik iş bilgisi/başlangıç tarihi kısmi onay bırakmamalı.
6. Dönemi olmayan veya başka personele/türe bağlı kontrollü talebi onaylamayı deneyin. Talep, hareket, bakiye ve audit değişmemeli. Bakiye, hareket, iş bilgisi ve audit yazılarında ayrı ayrı kontrollü hata oluşturun: bütün iş değişiklikleri geri alınmalı; başarı ve e-posta oluşmamalı. Commit hatasını da aynı beklentiyle doğrulayın.
7. Log açıkken talep UPDATE, dönem UPDATE ve tek hareket INSERT auditlerini inceleyin; ücretsiz izinde ilgili iş bilgisi UPDATE auditini de kontrol edin. Aynı gerçek yazı iki kez loglanmamalı. Rollback sonrasında audit kalmamalı; bağımsız SQL/hata izinin kalması beklenir.
8. Log kapalıyken bu transaction yazma zincirlerinde eski Olay/SQL log yazısı oluşmamalı; iş sonucu ve rollback kapsamı değişmemeli. Aşağıdaki eski okuma yollarını bu iddiadan ayrı değerlendirin: ekranın tamamı için sıfır eski yazı kapanışı henüz tamamlanmadı.
9. E-posta açık/kapalı senaryolarını doğrulayın. Hatalı veya tekrarlanan işlemde bildirim olmamalı. Commit sonrası bildirim hatasında iş kaydı korunmalı; ekranda kaydın tamamlandığı, bildirimin başarısız olduğu anlaşılmalı.

## Açık kalan bağımlılıklar ve sonraki parça

İş yazma yolları taşındı; bu iki ekranın tamamı eski altyapıdan henüz bağımsız değildir. Aşağıdaki mevcut okumalar `DbClass.SelectFromDb` yoluna ulaşabilir. Dolayısıyla SQL hatasında `SQLLog_Table` yazısı riski sürer; yalnız isim veya context parametresinin varlığı kapanış kanıtı değildir.

- İki ekrandaki `FillIzinHareketleriTable`, `FillIzinBilgileriTable`, `FillIzinTalebi`: dönem/hareket/tanım okumaları. `IsBilgileriRepository.SelectByPersonelId(int, PortalLogContext)` bile içeride eski `db.SelectFromDb` çağırır. `IzinDonemService` ve bazı `IzinHareketService` context overload'ları eski repository okumasını teknik izle sarmalar.
- `IKYSOrtak.IzinKabulRedOnayEPostasiGonder` (`Model/IKYS/IKYSOrtak.cs`): personel, talep, izin tanımı, dönem, iletişim ve SMTP parametresi okumaları; `KalacakIzinHesapla` alt zinciri. Bu yardımcı bu parçada aynı şekilde commit sonrasında çağrılır. E-posta hatasının yeni ekranda merkezi kayda alınması, alttaki eski SQL yazıcısını kapatmaz.

Sıradaki IKYS parçasında bu ortak okuma ve bildirim zincirleri kapatılmalı; ardından dönem listesi, mahsup ve toplu izin ekranları sürdürülmelidir. Kullanılmayan bağlamsız API'ler ve ortak eski log yazıcıları tüm çağıranlar doğrulandıktan sonra kaldırılacaktır. Bütün solution için sıfır eski yazı hedefi henüz tamamlanmadı. Eski tabloların arşivleme ve sonradan kaldırma değerlendirmesi, bütün proje bağımlılıkları kapatıldıktan sonra ayrıca yapılmalıdır.

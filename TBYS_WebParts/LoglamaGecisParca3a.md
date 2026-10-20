# TBYS SQL izleri — Yasal Faiz ve Gecikme Zammı

Dal: `codex/tbys-sql-trace-rates`.
Başlangıç commit'i: `bc718d70188a9bce22a82498b488375e4aa3a30c`.
Kod değişikliğinden önce master temizdi; yerel master, origin/master ve GitHub master aynı commit'teydi.

## Kapsam ve örnek

Bu inceleme parçası YasalFaizWP ve GecikmeZammiWP ile bu iki entity'nin servis/repository sınıflarını kapsar. İki repository'nin 20 SQL metodu yeni erişim yoluyla izlenebilir. Diğer TBYS repository'leri henüz taşınmadı.

Örneğin YasalFaizWP'deki kaydetme akışında `new YasalFaizService(LoggingContext)` kullanılır. Servis aynı bağlamı YasalFaizRepository'ye verir. Güncelleme öncesi kayıt okuması `YasalFaizRepository.SelectById`, yazma ise `YasalFaizRepository.Update` adıyla izlenir. Bu SQL kayıtları ve aynı istekteki hata kayıtları ortak OperationId taşır. Sonraki postback/yönlendirme ayrı HTTP isteğidir ve yeni OperationId alır.

GecikmeZammiService.GetByDate gibi birden fazla repository okuması yapan metotlar da aynı repository örneğini ve bağlamını kullanır. WebPart'lardaki dinamik güncelleme/silme düğmelerinin servis çağrıları da bağlamlıdır.

## SQL erişimi ve kayıt davranışı

DAO/Repositories/TBYS/TbysSqlExecutor.cs yalnızca TBYS içinde kullanılan erişim yardımcısıdır. Bağlamlı yol DbClass yazıcısını çağırmadan mevcut parametreli sorguları yürütür; teknik izi ortak PortalTechnicalLog.WriteSqlTrace üzerinden PortalLog_Table'a gönderir.

Kaydedilen bilgiler: Project=TBYS, WebPart, kullanıcı, sayfa yolu, OperationId, Kind=SqlTrace, SqlOperation, RepositoryMethod, DurationMs ve Outcome. RepositoryMethod derleyicinin CallerMemberName bilgisi ve sabit repository adından oluşur. SQL metni, parametre değerleri ve açıklama alanları SQL izine gönderilmez.

Süre bağlantı hazırlama/açma, komut yürütme ve sonuç okumasını kapsar; log yazma süresi dahil değildir. Boş SELECT başarılıdır. INSERT pozitif kimlik döndürürse başarılı kabul edilir. UPDATE/DELETE için önceki sözleşme korunur: ExecuteNonQuery hatasız tamamlanırsa true döner; etkilenen satır sayısı ayrıca başarı koşulu yapılmadı. SQL hatası bastırılmaz; finally içinde Failed SQL izi denenir ve mevcut WebPart hata sunumuna ulaşır. Teknik yazıcının kendi hata yutma davranışı iş sonucunu korur.

Parametresiz ve DbClass enjekte edilen eski servis/repository oluşturma yolları mevcut davranışlarını sürdürür. Bu iki WebPart'ın aktif çağrıları açıkça bağlamlı yola geçirildi; başka ekranlardaki veya TBYSOrtak hesaplama akışlarındaki parametresiz çağrılar otomatik olarak taşınmadı. Bağlamlı yol, loglama ayarı kapalı olsa da eski SQL log yazıcısına dönmez; yalnızca teknik kayıt üretimi ayara göre durur.

## Transaction ve audit sınırı

Bu parçada yeni SqlTransactionContext veya TransactionScope akışı eklenmedi. İş bağlantılarının mevcut bağlantı dizesine bağlı ambient transaction davranışı korundu; iş bağlantılarında Enlist=false kullanılmaz. TBYS teknik yazıcısı önceki parçadaki bağımsız bağlantısını kullanır.

Bu iki entity'nin mevcut OlayKayit çağrıları henüz ortak transaction/audit akışına taşınmadı. Bu çağrılar üzerinden ortak eski olay/SQL log altyapısına dolaylı erişim devam edebilir. Dolayısıyla tüm ekran işlemi için SQLLog_Table kullanımının bittiği veya audit atomikliğinin sağlandığı iddia edilmez. Önceki tarihi satırın bitişini güncelleme ve yeni oran ekleme gibi çok adımlı işlerin atomik hale getirilmesi audit/transaction parçasının kapsamındadır. Bu parçadaki yeni ana entity SQL yolu eski SQL log yazıcısını kullanmaz.

## Kullanıcı doğrulaması

Mevcut `PortalLogging.EnabledProjects` içinde TBYS varsa yeni web.config değişikliği gerekmez. Tek tek WebPart etkinleştiriliyorsa mevcut `PortalLogging.EnabledWebParts` değerine `TBYS:YasalFaizWP;TBYS:GecikmeZammiWP` eklenir; diğer değerler korunur. Önceki birleşik PortalLog_Table şeması kullanılır, yeni migration yoktur.

Visual Studio 2026 build ve TEST sunucusu doğrulaması kullanıcı tarafından yapılır:

1. İki ekran açıldığında mevcut açılış kayıtları yanında ilgili repository metotları için başarılı SELECT izleri oluşmalı. Aynı ilk isteğin UserActivity ve SqlTrace kayıtları aynı OperationId taşımalı.
2. Yasal faiz tek ay ve tüm ayları güncelleme denenmeli. SELECT ve UPDATE kayıtları doğru metot adını ve aynı işlemde ortak OperationId'yi taşımalı; oranlar eskisi gibi kaydedilmeli.
3. Yasal faiz yılı için mevcut otomatik oluşturma akışı gerçekleştiğinde INSERT izleri ve kayıt kimlikleri kontrol edilmeli. Akışın mevcut iş davranışı değiştirilmedi.
4. Gecikme zammı ekleme, güncelleme ve silme denenmeli; önceki satırın bitiş tarihi dahil mevcut sonuçlar korunmalı. INSERT/UPDATE/DELETE izleri kontrol edilmeli.
5. Kontrollü SQL hatasında SqlTrace.Outcome=Failed oluşmalı, hata teknik kaydıyla OperationId eşleşmeli ve kullanıcı yalnızca güvenli Türkçe mesaj görmeli. Sorgu metni veya parametre değerleri SqlTrace alanlarında bulunmamalı.
6. Logging ayarı kapalıyken işlemler normal çalışmalı; yeni SQL izleri oluşmamalı. Teknik log yazma hatası ana SQL sonucunu değiştirmemeli.

Codex build/test/debug çalıştırmadı. Statik kaynak incelemesi ve git diff --check yapıldı. Üretilmiş dosyalar, diğer projelerin kodları ve ortak DB/log sınıfları değiştirilmedi. DAO.csproj yalnızca yeni TBYS kaynak dosyasını derlemeye dahil eder.

Sonraki parçalar kalan TBYS repository/servis SQL izleri ve onların çağrı zincirleri, ardından transaction/audit ve alan değişiklikleridir.

## TEST sonucu ve tanılama temizliği

20 Ekim 2026 tarihinde kullanıcı YasalFaiz için yeni WebPartOpened, SELECT ve UPDATE kayıtlarının hem TESTDB'de hem takip ekranında oluştuğunu doğruladı. Aynı güncelleme isteğinin SQL kayıtları ortak OperationId taşıdı. Bu doğrulama diğer entity'lerin veya tüm test senaryolarının tamamlandığı anlamına gelmez.

Kayıtların yazılmamasının doğrulanan nedeni, EnabledProjects boşken EnabledWebParts listesine tek başına TBYS eklenmesiydi. Tüm TBYS için PortalLogging.EnabledProjects değerine TBYS eklenmelidir; tek tek etkinleştirmede PortalLogging.EnabledWebParts değerleri TBYS:WebPartAdi biçiminde olmalıdır. Mevcut IKYS değerleri korunur.

Geçici yönetici tanılama paneli, gözlem çağrıları, yardımcı sınıflar, kontrol SQL'i ve geçici sorun inceleme belgeleri kaldırıldı. Gerçek SELECT şema düzeltmesi (Operation=NULL, SqlOperation=SELECT) ve TBYS paketinin DAO/Model proje çıktılarını kullanması korundu. YasalFaiz URL'sindeki geçici tanılama parametresi artık bir panel açmaz; normal sayfa adresi kullanılabilir. Temizlik sonrası build ve TEST doğrulaması kullanıcıya aittir; Codex build/test/debug çalıştırmadı.

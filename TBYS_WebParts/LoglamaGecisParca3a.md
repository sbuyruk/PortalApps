# TBYS Yasal Faiz ve Gecikme Zammı — SQL izi ve audit pilotu

Dal: `codex/tbys-sql-trace-rates`.
Başlangıç commit'i: `bc718d70188a9bce22a82498b488375e4aa3a30c`.
Kod değişikliğinden önce master temizdi; yerel master, origin/master ve GitHub master aynı commit'teydi.

Audit pilotu dalı: `codex/tbys-rates-audit-pilot`. Bu parçanın başlangıcı `66e3019fc0fafe8f867d3029bcc161227608df0e`; değişiklik öncesinde master temizdi ve GitHub master ile aynı commit'teydi.

## Kapsam ve örnek

Bu inceleme parçası YasalFaizWP ve GecikmeZammiWP ile bu iki entity'nin servis/repository sınıflarını kapsar. Bu repository'lerin bağlamlı SQL metotları izlenir; audit pilotunda transaction-aware overload'lar eklendi. Diğer TBYS repository'leri henüz taşınmadı.

Örneğin YasalFaizWP'deki kaydetme akışında `new YasalFaizService(LoggingContext)` kullanılır. Servis aynı bağlamı YasalFaizRepository'ye verir. Güncelleme öncesi kayıt okuması `YasalFaizRepository.SelectById`, yazma ise `YasalFaizRepository.Update` adıyla izlenir. Bu SQL kayıtları ve aynı istekteki hata kayıtları ortak OperationId taşır. Sonraki postback/yönlendirme ayrı HTTP isteğidir ve yeni OperationId alır.

GecikmeZammiService.GetByDate gibi birden fazla repository okuması yapan metotlar da aynı repository örneğini ve bağlamını kullanır. WebPart'lardaki dinamik güncelleme/silme düğmelerinin servis çağrıları da bağlamlıdır.

## SQL erişimi ve kayıt davranışı

DAO/Repositories/TBYS/TbysSqlExecutor.cs yalnızca TBYS içinde kullanılan erişim yardımcısıdır. Bağlamlı yol DbClass yazıcısını çağırmadan mevcut parametreli sorguları yürütür; teknik izi ortak PortalTechnicalLog.WriteSqlTrace üzerinden PortalLog_Table'a gönderir.

Kaydedilen bilgiler: Project=TBYS, WebPart, kullanıcı, sayfa yolu, OperationId, Kind=SqlTrace, SqlOperation, RepositoryMethod, DurationMs ve Outcome. RepositoryMethod derleyicinin CallerMemberName bilgisi ve sabit repository adından oluşur. SQL metni, parametre değerleri ve açıklama alanları SQL izine gönderilmez.

Süre bağlantı hazırlama/açma, komut yürütme ve sonuç okumasını kapsar; log yazma süresi dahil değildir. Boş SELECT başarılıdır. INSERT pozitif kimlik döndürürse başarılı kabul edilir. UPDATE/DELETE için önceki sözleşme korunur: ExecuteNonQuery hatasız tamamlanırsa true döner; etkilenen satır sayısı ayrıca başarı koşulu yapılmadı. SQL hatası bastırılmaz; finally içinde Failed SQL izi denenir ve mevcut WebPart hata sunumuna ulaşır. Teknik yazıcının kendi hata yutma davranışı iş sonucunu korur.

Parametresiz ve DbClass enjekte edilen eski servis/repository oluşturma yolları mevcut davranışlarını sürdürür. Bu iki WebPart'ın aktif çağrıları açıkça bağlamlı yola geçirildi; başka ekranlardaki veya TBYSOrtak hesaplama akışlarındaki parametresiz çağrılar otomatik olarak taşınmadı. Bağlamlı yol, loglama ayarı kapalı olsa da eski SQL log yazıcısına dönmez; yalnızca teknik kayıt üretimi ayara göre durur.

## Transaction ve audit pilotu

Bağlamlı YasalFaizService ve GecikmeZammiService yazmaları SqlTransactionContext kullanır. İş kaydı, audit başlığı ve izinli alan değişiklikleri aynı transaction içinde yazılır. Eski kayıt UPDLOCK/HOLDLOCK ile aynı transaction'dan okunur. Kaydedilen kayıt tekrar okunarak audit değerleri veritabanında saklanan sayı/tarih değerlerinden alınır. Veritabanındaki NULL, audit'te sıfıra veya boş tarihe çevrilmez. Transaction-aware metotlar ayrı DbClass veya SqlConnection açmaz. Teknik SQL/hata kayıtları önceki bağımsız ortak yazıcıyı kullanır.

Pilot ekranların bağlamlı akışları OlayKayit çağırmaz; ayar kapalı olsa da eski yazıcıya geri dönmez. Bağlam verilmeden kullanılan eski servis oluşturma yolları diğer çağıranlar için korunur; onlar bu pilotla otomatik taşınmış sayılmaz.

YasalFaiz: tek ay güncelleme, Hepsini Kaydet ile gönderilen bütün aylar ve yeni yılın 12 aylık oluşturulması kendi işlem bütünlüğü içinde tamamlanır. Bir ayda veya audit yazmasında hata olursa aynı işlemdeki bütün aylar geri alınır. Yılın varlığı ve başlangıç oranları transaction içinde tekrar okunur. GecikmeZammi: ekleme/güncelleme/silme ile önceki dönemin bitiş tarihi düzeltmesi tek transaction içindedir. Değişmesi gerekmeyen komşu dönem tekrar yazılmaz. Yeni tarih komşuları seçilirken güncellenen kaydın kendisi dışlanır. Ekranın başlangıç tarihi alanının mevcut salt okunur davranışı değiştirilmedi.

Audit başlıkları Kind=Audit, EntityName=YasalFaiz/GecikmeZammi, EntityId=uygulama kayıt kimliği ve Operation=INSERT/UPDATE/DELETE taşır. INSERT/DELETE için izinli alanların var/yok değerleri; UPDATE için yalnızca değişen izinli alanlar PortalAuditChange_Table'a yazılır. Alan izin listeleri:

- YasalFaiz: Yil, Ay, FaizOrani, Tufe, Ufe.
- GecikmeZammi: BaslangicTarihi, BitisTarihi, ZamOrani.

Serbest Aciklama, kullanıcı adları, teknik oluşturma/değiştirme zamanları ve türetilmiş AyAdi alanı değişiklik ayrıntısına alınmaz. Aciklama iş verisinde eskisi gibi kaydedilir. Hiçbir izinli alan değişmeden UPDATE çalışırsa başlık oluşabilir, ayrıntı sayısı sıfır olur. Audit kapalıysa transaction'lı iş işlemi devam eder ve audit ayrıntısı yazılmaz. Audit açıkken başlık/ayrıntı yazma hatası işlemi başarısız kılar; SQL/hata izlerinin bağımsız kalması audit'in rollback olmasını engellemez. SQL izinde Succeeded, SQL komutunun yürüdüğünü ifade eder; üst iş transaction'ının daha sonra rollback olmadığını garanti etmez.

## Kullanıcı doğrulaması

Mevcut `PortalLogging.EnabledProjects` içinde TBYS varsa yeni web.config değişikliği gerekmez. Tek tek WebPart etkinleştiriliyorsa mevcut `PortalLogging.EnabledWebParts` değerine `TBYS:YasalFaizWP;TBYS:GecikmeZammiWP` eklenir; diğer değerler korunur. Önceki birleşik PortalLog_Table şeması kullanılır, yeni migration yoktur.

Visual Studio 2026 build ve TEST sunucusu doğrulaması kullanıcı tarafından yapılır:

1. İki ekran açıldığında mevcut açılış kayıtları yanında ilgili repository metotları için başarılı SELECT izleri oluşmalı. Aynı ilk isteğin UserActivity ve SqlTrace kayıtları aynı OperationId taşımalı.
2. Yasal faiz tek ay ve tüm ayları güncelleme denenmeli. SELECT ve UPDATE kayıtları doğru metot adını ve aynı işlemde ortak OperationId'yi taşımalı; oranlar eskisi gibi kaydedilmeli.
3. Yasal faiz yılı için mevcut otomatik oluşturma akışı gerçekleştiğinde INSERT izleri ve kayıt kimlikleri kontrol edilmeli. Akışın mevcut iş davranışı değiştirilmedi.
4. Gecikme zammı ekleme, güncelleme ve silme denenmeli; önceki satırın bitiş tarihi dahil mevcut sonuçlar korunmalı. INSERT/UPDATE/DELETE izleri kontrol edilmeli.
5. Kontrollü SQL hatasında SqlTrace.Outcome=Failed oluşmalı, hata teknik kaydıyla OperationId eşleşmeli ve kullanıcı yalnızca güvenli Türkçe mesaj görmeli. Sorgu metni veya parametre değerleri SqlTrace alanlarında bulunmamalı.
6. Logging ayarı kapalıyken işlemler normal çalışmalı; yeni SQL izleri oluşmamalı. Teknik log yazma hatası ana SQL sonucunu değiştirmemeli.

Audit pilotunda ayrıca şu doğrulamalar kullanıcı tarafından yapılır:

1. Tek ayın yalnızca FaizOrani değeri değiştirilir. YasalFaiz UPDATE başlığında ilgili Id bulunmalı; ayrıntıda yalnızca FaizOrani eski/yeni değeri yer almalı. SQL ve audit aynı isteğin OperationId değerini taşımalı.
2. Yıl oluşturma, toplu güncelleme ve GecikmeZammi ekleme/güncelleme/silme denenir. INSERT/UPDATE/DELETE başlıkları, izinli alanlar ve komşu dönemin bitiş tarihi kontrol edilir.
3. TEST ortamında kontrollü olarak audit ayrıntısı yazması başarısız kılınır; hata senaryosu PortalLog_Table'a teknik hata yazma yetkisini de engellememeli. Ana iş kaydı ve aynı işlemdeki önceki dönem/ay değişiklikleri ile bütün audit satırları geri alınmalı; güvenli hata mesajı ve bağımsız Error kaydı korunmalı. Bu hata oluşturma işlemi Codex tarafından yürütülmez.
4. Aynı şekilde toplu işlemde sonraki ayın SQL yazmasının başarısız olduğu kontrollü senaryoda önceki aylar ve audit'leri kalmamalı. Yeni yıl oluşturma yarıda kalmamalı.
5. Logging kapalıyken işlemler transaction'lı kalmalı ve eski OlayKayit yoluna dönmemeli. Açıkken Aciklama alanının audit ayrıntısına girmediği doğrulanmalı.

Codex build/test/debug çalıştırmadı. Statik kaynak incelemesi ve git diff --check yapıldı. Üretilmiş dosyalar, diğer projelerin kodları ve ortak DB/log sınıfları değiştirilmedi. DAO.csproj yalnızca yeni TBYS kaynak dosyasını derlemeye dahil eder.

Sonraki parçalar kalan TBYS repository/servis SQL izleri, çağrı zincirleri ve bu audit yaklaşımının diğer akışlara yayılmasıdır. Audit pilotu için yeni dosya, migration veya etkinleştirme ayarı eklenmedi. Mevcut TBYS etkinleştirmesi ve birleşik PortalLog/PortalAuditChange şeması kullanılır. Codex build/test/debug çalıştırmadı; pilotun çalışma zamanı sonucu kullanıcı doğrulamasını bekler.

## TEST sonucu ve tanılama temizliği

20 Ekim 2026 tarihinde kullanıcı YasalFaiz için yeni WebPartOpened, SELECT ve UPDATE kayıtlarının hem TESTDB'de hem takip ekranında oluştuğunu doğruladı. Aynı güncelleme isteğinin SQL kayıtları ortak OperationId taşıdı. Bu doğrulama diğer entity'lerin veya tüm test senaryolarının tamamlandığı anlamına gelmez.

Kayıtların yazılmamasının doğrulanan nedeni, EnabledProjects boşken EnabledWebParts listesine tek başına TBYS eklenmesiydi. Tüm TBYS için PortalLogging.EnabledProjects değerine TBYS eklenmelidir; tek tek etkinleştirmede PortalLogging.EnabledWebParts değerleri TBYS:WebPartAdi biçiminde olmalıdır. Mevcut IKYS değerleri korunur.

Geçici yönetici tanılama paneli, gözlem çağrıları, yardımcı sınıflar, kontrol SQL'i ve geçici sorun inceleme belgeleri kaldırıldı. Gerçek SELECT şema düzeltmesi (Operation=NULL, SqlOperation=SELECT) ve TBYS paketinin DAO/Model proje çıktılarını kullanması korundu. YasalFaiz URL'sindeki geçici tanılama parametresi artık bir panel açmaz; normal sayfa adresi kullanılabilir. Temizlik sonrası build ve TEST doğrulaması kullanıcıya aittir; Codex build/test/debug çalıştırmadı.

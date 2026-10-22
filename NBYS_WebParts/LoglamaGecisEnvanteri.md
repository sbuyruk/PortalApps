# NBYS merkezi loglama geçişi — envanter ve parça 1

## Son kontrol düzeltmeleri (#522 sonrası)

Temiz ve uzak master ile aynı f8c408811c96fc8dcdde0f75028f7256b7622ee3 üzerinden codex/nbys-son-kontrol-log-rapor-duzeltme açıldı. Ekstre transaction eklemesinde merkezî audit sonrasında eski OlayKayit yolunun da çalışması engellendi. Döviz bağış raporunun yıllık ve aylık dönem sonu kayıp kayıtları, başlangıç dahil/bitiş hariç aralık ve typed tarih parametreleriyle düzeltildi. Eski tüketicilerin dahil bitiş davranışı korunur. Yeni/değişen kullanıcı mesajı yoktur. İnceleme ve TEST noktaları LoglamaGecisSonKontrolDuzeltmeleri.md içindedir. Codex build/test/debug çalıştırmadı.

## Ortak erişim kapanış parçası (#521 sonrası)

Temiz ve uzak master ile aynı 04c2bec7b87e5479089cc6a49768086a77c7bff6 üzerinden codex/nbys-kapanis-ortak-erisim-audit açıldı. Dokuz ekranın yönerge parametresi okumaları bağlamlı yapıldı. ASCX ekran sayımının dışında kalan NakitBagisciKarti'nin tüm veri okumaları, etkinlik/teknik logları ve mevcut bölge kuralıyla sunucu yetki kontrolü düzenlendi. Düzenli bağışçı için bağlamlı salt okuma yolu eklendi. Kartın beklenen uyarıları teknik hatadan ayrıldı; teknik hatada tek modal mesajı/başvuru kodu kullanılır. Ekstre dosyasının beklenen doğrulaması ayrıca FileValidation/Error üretmez; güvenli uyarı korunur.

Kapsam, mesaj farkları, derlenmeyen eski kopya, statik inceleme sınırı ve TEST noktaları LoglamaGecisKapanisOrtakErisim.md içindedir. 55/55 ekran bağlamı çalışma zamanı doğrulaması değildir; NBYS kapanışı TEST değerlendirmesine bağlıdır. Portal'a geçilmedi. Codex build/test/debug çalıştırmadı.

Dal: codex/nbys-parametre-log-transaction-mesaj
Başlangıç: e96e78959fbd585703d2154a701ad6e999c59fef (22 Ekim 2026).
Değişiklik öncesinde master çalışma ağacı temizdi; git ls-remote ile GitHub master'ın aynı commit olduğu doğrulandı.

## Önceki projelerin durumu

LoglamaGecisPlani.md IKYS uygulama parçalarını ve kullanıcı doğrulama adımlarını içeriyor; bütün IKYS kapsamı için kesin bir test kapanışı bu belgede doğrulanamadı.
TBYS_WebParts/LoglamaGecisParca3a.md, 20 Ekim 2026 YasalFaiz açılış/SELECT/UPDATE kayıtlarının kullanıcı tarafından TESTDB ve izleme ekranında doğrulandığını bildiriyor. Aynı belge bu sonucun diğer entity ve tüm senaryolar için geçerli olmadığını açıkça belirtiyor.
TBYS_WebParts/LoglamaGecisParca2.md ve LoglamaGecisEnvanteri.md kalan kapsam ve test bekleyen parçaları içeriyor. Dolayısıyla IKYS/TBYS'nin bütünüyle tamamlandığı varsayılmadı; kullanıcının açık NBYS talimatı esas alındı.

## İlk parça: NBYSParametreGirisiWP

- Tek ekranın açılış, grup listesi, düzenleme kaydı okuması, ekleme/güncelleme/silmesi taşındı.
- IKYS ResmiTatilGirisiWP ve TBYS GecikmeZammiWP, TbysWebPartLog, TbysSqlExecutor, GecikmeZammiService/Repository gerçek imzaları incelendi. Sunum için Utility.HelperClasses.Messages kullanıldı.
- Yeni NbysWebPartLog, aynı istek içindeki aynı exception/inner-exception zincirini bir kez kaydeder ve bildirir. ThreadAbortException yönlendirmesi teknik hata sayılmaz. TabloJson içindeki hatayı yutup boş JSON döndüren catch kaldırıldı; hata olay sınırına ulaşır.
- SQL yolu yalnızca bağlam verilerek seçilir. Diğer ekranların parametresiz/enjekte repository çağrıları eski davranışını sürdürür. Pilot bağlamlı yol ayar kapalıyken de eski SQL/OlayKayit yazıcısına dönmez.
- Repository SQL izinde yalnızca işlem türü, repository/metot adı, süre ve sonuç bulunur. Ham sorgu ve parametre değerleri gönderilmez.
- Yazma öncesi kayıt/mükerrerlik okumaları, INSERT/UPDATE/DELETE, kayıt sonrası okuma ve audit aynı SqlTransactionContext içindedir. Önceki kayıt ve mükerrerlik sorguları UPDLOCK/HOLDLOCK kullanır. Transaction dışındaki form okuması yazmanın doğruluk kontrolü değildir; servis transaction içinde yeniden okur.
- Transaction.Complete yalnızca tamamlanma işaretidir; gerçek commit Dispose sırasında olur. Servis başarılı sonucu using bloğu kapandıktan sonra döndürür.
- Audit entity adı NBYSParametre; EntityId uygulama Id değeridir. Eski/yeni alan izin listesi yalnızca Sira'dır. Anahtar/Deger ve kullanıcı/zaman alanları ayrıntıya alınmaz. INSERT/DELETE var/yok, UPDATE yalnızca değişen izinli alanları yazar. Anahtar/değer değişikliğinde başlık oluşur; Sira değişmiyorsa ayrıntı oluşmaz.
- Teknik SQL/hata kayıtları ortak bağımsız PortalTechnicalLog yazıcısını kullanır; audit iş transaction'ının içindedir. SQL Succeeded, komutun sonucudur; üst işlemin commit edildiği anlamına gelmez.
- NBYS paketindeki DAO/Model sabit bin/Debug DLL yolları mevcut proje çıktılarına bağlandı; Release paketlemesinde eski Debug çıktısının kullanılmasının önüne geçilir. Feature/Elements ve üretilmiş dosyalar değiştirilmedi.

## Mesaj değişiklikleri ve inceleme noktaları

- Başarı metinleri: “Parametre kaydedildi.”, “Parametre güncellendi.”, “Parametre silindi.” Yalnızca commit sonrasında gösterilir.
- Aynı Grup/Anahtar/Deger birleşimi anlamlı Warning döndürür; teknik exception üretilmez. Güncellemede kaydın kendisi hariç tutulur; yalnızca sıra değişikliği artık kendi kaydını mükerrer saymaz.
- Bulunamayan kayıt için listeyi yenileyip yeniden seçme; boş grup için grup seçme; geçersiz sıra için tam sayı girme uyarısı gösterilir.
- Yetki nedeniyle silme modalı açılmadığında eski, kanıtsız “kullanan uygulamalar var” açıklaması yerine “Bu parametreyi silme yetkiniz yok.” gösterilir. Mevcut yetki mekanizması bu parçada yeniden tasarlanmadı.
- Ham ex.Message kaldırıldı. Teknik hata bir Error kaydı ve güvenli Türkçe mesaj/başvuru kodu üretir. Uyarılar genel hata mesajına çevrilmez.
- Commit başarılı olup liste hazırlama başarısızsa tek bildirim “Parametre ... . Liste yenilenemedi. Sayfayı yenileyin.” anlamını taşır; ayrıca başarı popup'ı eklenmez. Aynı hata tekrar raporlanmaz.
- Kullanıcı bilgileri teknik hata metinlerine eklenmez. Teknik exception ayrıntıları yalnızca mevcut ortak teknik log hedefindedir.

## Yapılandırma ve veritabanı

Mevcut IKYS/TBYS ayarlarını koruyarak çalışan SharePoint web uygulamasının web.config dosyasındaki PortalLogging.EnabledWebParts değerine yalnızca NBYS:NBYSParametreGirisiWP ekleyin. Bu aşamada EnabledProjects listesine NBYS eklemeyin.
Bağlantı TSKGV_Connection üzerinden ortak DBProcess ve teknik yazıcıdan okunur.
Yeni şema/migration eklenmedi. Veritabanına bağlanılmadı; mevcut şema ve yetkiler doğrulanmış değildir.
Etkinleştirme öncesinde hedef ortamda dbo.PortalLog_Table ve dbo.PortalAuditChange_Table ile birleşik migration'ın gerekli kolonlarının bulunduğunu kontrol edin. Gereken migration'ı mevcut şemaya göre Database/Migrations/20261017_PortalLoggingActivityAudit.sql ve 20261019_PortalLoggingUnified.sql üzerinden belirleyin. WSP deploy migration çalıştırmaz; TEST'te bulunan tabloların canlıda da bulunduğu varsayılmamalıdır.
Ayar kapalıyken pilot iş yazmaları transaction'lı çalışır; yeni log kayıtları ve eski yazıcıya geri dönüş oluşmaz.

## Kullanıcının Visual Studio 2026 / TEST kontrolü

1. Solution ve NBYS WSP paketini Debug/Release kullandığınız yapılandırmada derleyin; DAO/Model/Utility güncel çıktılarını kontrol edin.
2. Ekranı ilk açın, grubu değiştirin ve düzenleme modalını açın. İlk istekte tek UserActivity; ilgili SELECT kayıtları ve aynı istekte ortak OperationId beklenir. Postback yeni açılış kaydı üretmemeli.
3. Ekleme, değiştirme, yalnızca sıra değiştirme ve silmeyi deneyin. Başarı mesajları işlem tamamlandıktan sonra tek kez görünmeli; iş kaydı ve audit başlığı birlikte oluşmalı. Sira eski/yeni değerlerini ve serbest değerlerin audit/SQL izine girmediğini kontrol edin.
4. Aynı grupta aynı anahtar/değeri yeniden ekleyin veya başka bir mevcut kayda eşitleyin. Açık mükerrerlik uyarısı, sıfır yazma/audit ve sıfır teknik Error beklenir. Modal açıkken başka oturumda silinen kayıt için bulunamadı uyarısını deneyin.
5. Boş grup ve geçersiz sıra bilgisinde anlamlı uyarıyı; iki eşzamanlı aynı parametre ekleme isteğinde mükerrer oluşmamasını kontrol edin. Kilit/deadlock teknik hatası olursa güvenli mesaj/başvuru kodu görülmeli.
6. TEST'te kontrollü yazma/audit hatasında iş verisi ve audit birlikte geri alınmalı; başarı mesajı görünmemeli. Bağımsız Error ve SQL izleri kalmalı; popup'ta SQL/exception/stack trace görünmemeli.
7. Commit sonrası liste SELECT/JSON hazırlaması hata senaryosunda kaydın tamamlandığı ve sayfanın yenilenmesi gerektiği tek bildirimle görünmeli; işlem yeniden yapılmamalı.
8. Loglama kapalıyken iş işlemleri çalışmalı ve legacy çift kayıt oluşmamalı.

Codex build/test/debug çalıştırmadı. Doğrulama statik kaynak/imza/çağrı zinciri, XML, UTF-8/BOM ve git diff kontrolleriyle sınırlıdır. Çalışma zamanı ve rollback senaryoları kullanıcı doğrulamasını bekler.
Sonraki parçaya kullanıcı test sonucu veya test beklemeden devam edilmesi yönünde açık talimat geldikten sonra geçilir. Commit/push/PR/merge kullanıcıya aittir.

## Kalan parçalar için çalışma sırası

1. Parametre pilotu (bu dal).
2. Banka/armağan tanımları ve FTK tanım/kişi/işlem akışları; her biri ayrı incelenebilir dal.
3. Salt okuma listeleri/raporlar: açılış, SELECT ve hata/mesaj sınırları.
4. Bağışçı ekleme/düzenleme, bağış hareketleri ve armağan bağlantıları; kişisel veri izin listeleri ve birden fazla entity transaction'ları.
5. Banka yükleme, ekstre import/eşleştirme/aktarma: dosya başına veya satır başına mevcut işlem sınırını kanıtlayarak, ExceptionHelper içinde iş kuralı/teknik hata ayrımı ve kısmi başarı mesajları.
6. Birleştirme, hareket silme, SMS ve belge/basım akışları; dış etkiler ile DB commit sırasının ayrıca incelenmesi.

Her parçada mevcut başarı/uyarı/doğrulama/hata mesajları, erken başarı, hatayı yutup devam etme ve çift bildirim riski incelenip ayrıca raporlanacak. Portal/MTS/MFYS ve log izleme WebPart'ları bu kapsamın dışındadır.

## Kaynak envanteri

56 ASCX code-behind, 14 NBYS repository, 17 NBYS service bulundu. Ek NakitBagisciKarti.cs sınıfı da ayrıca incelenmelidir.
Aşağıdaki liste kaynakta doğrudan görülen servis/repository oluşturma çağrılarını ve eski mesaj/log işaretlerini gösterir; dolaylı model/ortak servis erişimlerinin tam çağrı grafiği veya tamamlanmış transaction denetimi değildir.
Henüz pilot dışındaki ekranlar taşınmış sayılmaz.

| WebPart | Doğrudan oluşturulan servis/repository | Eski log/mesaj işaretleri |
| --- | --- | --- |
| ArmaganAraWP | ArmaganService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| ArmaganEditWP | ArmaganService, ArmaganTanimService, NakitBagisciService | ExceptionHelper, MessageHelper |
| ArmaganListesiWP | ArmaganService, ArmaganTanimService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper, MessageHelper |
| BagisaVesileOlanArmaganBelgesiWP | BagisaVesileOlanTesekkurService | ExceptionHelper, MessageHelper |
| BagisaVesileOlanTesekkurGirisiWP | BagisaVesileOlanTesekkurService | ExceptionHelper, MessageHelper |
| BagisaVesileOlanTesekkurGirisiWP | BagisaVesileOlanTesekkurService | ExceptionHelper, MessageHelper |
| BagisaVesileOlanTesekkurListesiWP | BagisaVesileOlanTesekkurService | ExceptionHelper, MessageHelper |
| BagisHareketListesiWP | IlService, NakitBagisHareketService | ExceptionHelper |
| BagisIadeListesiWP | NakitBagisHareketService | ExceptionHelper |
| BagisIadesiWP | ArmaganService, ArmaganTanimService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper, MessageHelper |
| BankaDosyasiYuklemeWP | EkstreAktarmaService | ExceptionHelper, LogHelper, MessageHelper |
| BeratBasimiWP | ArmaganService | ExceptionHelper |
| BeratBelgesiYazisiWP | ArmaganService, BolgeService | ExceptionHelper, MessageHelper |
| BolgeArmaganRaporuWP | ArmaganService | ExceptionHelper |
| BolgelereGoreFTKRaporuWP | FTKService, IlceService, IlService | ExceptionHelper |
| CokDefaBagisYapanBagisciListesiWP | ArmaganService, ArmaganTanimService, IlceService, IlService, NakitBagisciReportService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper, MessageHelper |
| DovizleYapilanBagislarWP | BankaTanimService, NakitBagisHareketService | ExceptionHelper |
| DuzenliBagisciBelgesiBasimiWP | ArmaganService, BolgeService | ExceptionHelper, MessageHelper |
| DuzenliBagisciDosyaYuklemeWP | DuzenliNakitBagisciService, NakitBagisciService | ExceptionHelper, MessageHelper |
| DuzenliNakitBagisciListesiWP | ArmaganService, ArmaganTanimService, DuzenliNakitBagisciService, IlceService, IlService, NakitBagisciReportService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| EkstreAktarmaEditWP | BankaTanimService, EkstreAktarmaService, IlceService, IlService, NakitBagisciService | ExceptionHelper, MessageHelper |
| EkstreListesiWP | BankaTanimService, EkstreAktarmaService, SilinenKayitService | ExceptionHelper, MessageHelper |
| FTKFahriBaskanListesiWP | FTKKisiService | ExceptionHelper, MessageHelper |
| FTKHaritasiWP | FTKService, IlService | ExceptionHelper |
| FTKIslemleriWP | BolgeService, FTKIslemService, FTKKisiService, FTKService, IlceService, IlService | ExceptionHelper, MessageHelper |
| FTKKisiGirisiWP | FTKIslemService, FTKKisiService, IlceService, IlService | ExceptionHelper, MessageHelper |
| FTKKisiListesiWP | FTKKisiService | ExceptionHelper, MessageHelper |
| FTKKuruluOlmayanIlIlceListesiWP | BolgeService, FTKIslemService, FTKService, IlceService, IlService | ExceptionHelper |
| FTKListesiWP | BolgeService, FTKIslemService, FTKService, IlceService, IlService | ExceptionHelper |
| FTKYazilariWP | BolgeService, FTKService, IlceService, IlService | ExceptionHelper, MessageHelper |
| HomeWP |  | ExceptionHelper |
| NakitBagisciAdresListesiWP | IlceService, IlService, NakitBagisciReportService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper, MessageHelper |
| NakitBagisciBagislariWP | NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| NakitBagisciBirlestirmeWP | ArmaganService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper, MessageHelper |
| NakitBagisciBulmaWP | IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| NakitBagisciByBankaWP | NakitBagisHareketService | ExceptionHelper |
| NakitBagisciByTarihWP | NakitBagisHareketService | ExceptionHelper |
| NakitBagisciEditWP | IlceService, IlService, NakitBagisciService | ExceptionHelper, MessageHelper |
| NakitBagisciEslestirWP | EkstreAktarmaService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| NakitBagisciListesiByYilAyWP | BolgeService, NakitBagisHareketService | ExceptionHelper |
| NakitBagisciListesiWP | IlceService, IlService, NakitBagisciService, NakitBagisHareketService | ExceptionHelper |
| NakitBagisHareketSilmeWP | ArmaganService, ArmaganTanimService, IlceService, IlService, NakitBagisciService, NakitBagisHareketService, SilinenKayitService | ExceptionHelper, MessageHelper |
| NakitBagisHaritasiWP | IlService, NakitBagisHareketService |  |
| NakitBagisRaporuByBolgeWP | NakitBagisHareketService | ExceptionHelper |
| NakitBagisRaporuByYil_il | NakitBagisHareketService | ExceptionHelper |
| NakitBagisRaporuWP | ArmaganTanimService, IlceService, IlService, NakitBagisHareketService | ExceptionHelper |
| NBYSParametreGirisiWP | NBYSParametreService | Messages, NbysWebPartLog |
| NBYSYetkilendirmeWP |  | ExceptionHelper |
| SMSAylikBagisCizelgesi | SMSAylikBagisService | ExceptionHelper |
| SMSAylikCizelgeReadOnlyWP | SMSAylikBagisService | ExceptionHelper |
| SPDosyaListesiWP |  | ExceptionHelper |
| TasinmazArmaganListesiWP |  | ExceptionHelper |
| TasinmazArmaganWP | TasinmazService | ExceptionHelper |
| TasinmazBagisciAdresListesiWP |  | ExceptionHelper, MessageHelper |
| TesekkurBasimiWP | ArmaganService | ExceptionHelper |
| TesekkurBelgesiYazisiWP | ArmaganService, BolgeService | ExceptionHelper, MessageHelper |

### Mevcut transaction erişimleri

Kaynakta SqlTransactionContext bulunan servis/repository dosyaları (varlığı, bütünlük garantisi değildir):

- Model\Services\NBYS\ArmaganService.cs
- Model\Services\NBYS\ArmaganTanimService.cs
- Model\Services\NBYS\BankaTanimService.cs
- Model\Services\NBYS\EkstreAktarmaImportService.cs
- Model\Services\NBYS\EkstreAktarmaService.cs
- Model\Services\NBYS\EkstreAktarmaTransferService.cs
- Model\Services\NBYS\EkstreBagisciEslestirmeService.cs
- Model\Services\NBYS\NakitBagisciService.cs
- Model\Services\NBYS\NakitBagisHareketService.cs
- Model\Services\NBYS\NBYSParametreService.cs
- DAO\Repositories\NBYS\ArmaganRepository.cs
- DAO\Repositories\NBYS\ArmaganTanimRepository.cs
- DAO\Repositories\NBYS\BankaTanimRepository.cs
- DAO\Repositories\NBYS\EkstreAktarmaRepository.cs
- DAO\Repositories\NBYS\NakitBagisciRepository.cs
- DAO\Repositories\NBYS\NakitBagisHareketRepository.cs
- DAO\Repositories\NBYS\NBYSParametreRepository.cs
- DAO\Repositories\NBYS\NbysSqlExecutor.cs


## Parametre testi ve ikinci parçanın gerçek kapsamı

Kullanıcı NBYSParametreGirisiWP testini başarılı bildirdi.
İkinci parça dalı codex/nbys-banka-doviz-log-mesaj; başlangıç 4f1816e938d9d598a72488363a3dd6c8beffe3b6.
Banka tanımları için ayrı bir ekleme/düzenleme WebPart'ı ve yazma çağıranı bulunmadığından ikinci parça mevcut DovizleYapilanBagislarWP, banka grubu okuması ve rapor SELECT'i ile sınırlandı. Banka CRUD/audit tamamlandı kabul edilmez.
Mesaj değişiklikleri, kalan banka okuyucuları, etkinleştirme, mevcut tarih filtresi sınırı ve TEST adımları LoglamaGecisParca2.md içindedir.

## Kullanıcı talebiyle toplu geçiş kapsamı

Kullanıcı daha geniş dallarla ilerlenmesini istedi. Banka/döviz parçası master'a alındıktan sonra temiz/güncel f0066ca6a67b6402d54969300955643fb20293f8 üzerinden codex/nbys-liste-rapor-toplu-gecis açıldı.
Bu dal 10 salt okuma ekranını birlikte taşır: BagisHareketListesiWP, BagisIadeListesiWP, NakitBagisciByBankaWP, NakitBagisciByTarihWP, NakitBagisciListesiByYilAyWP, NakitBagisHaritasiWP, NakitBagisRaporuByBolgeWP, NakitBagisRaporuByYil_il, FTKKisiListesiWP, FTKFahriBaskanListesiWP.
Açılış/hata kayıtları, ilgili SELECT zincirleri, filtre/modal/Excel yolları ve mevcut mesaj incelemesi aynı daldadır. Eski yukarıdaki tablo başlangıç envanteridir; bu 10 ekran artık ExceptionHelper/ham hata sunumundan NbysWebPartLog/Messages yoluna taşınmıştır.
Transaction/audit gerektiren yazma akışları sonraki ilişkili işlem gruplarıdır. Bu dal salt okuma ekranlarına audit eklendiği veya NBYS'nin tümünün tamamlandığı anlamına gelmez.
Mesaj değişiklikleri, etkinleştirme listesi, statik kontroller ve toplu TEST adımları LoglamaGecisTopluListeRapor.md dosyasındadır. Yeni dalın çalışma zamanı sonucu henüz kullanıcı tarafından doğrulanmadı.

## Bağışçı yönetimi işlem grubu

Kullanıcı önceki toplu parça için tamamlandı/devam talimatı verdi. Temiz ve güncel 2b6b0e7ca6450ea519148f132c00c6d3828ce1e1 üzerinden codex/nbys-bagisci-yonetimi-transaction-audit açıldı.
NakitBagisciEditWP, NakitBagisciListesiWP, NakitBagisciBulmaWP ve NakitBagisciBagislariWP aynı daldadır.
Bağışçı güncellemesi, tüzel kişi değişikliğindeki mevcut aylık armağan hesabı ve bağış hareketi ArmaganId bağlantıları tek transaction ve audit yoluna alındı. Liste/arama/detay okumaları aynı NBYS bağlamını kullanır.
İade/silme/birleştirme ve ekstre yazmaları bu parçayla tamamlanmış sayılmaz. Mevcut düzenleme ekranında kimliksiz yeni kayıt girişi kapalıdır; bu dal yeni oluşturma ekranı açmaz.
Alan izin listeleri, mesaj değişiklikleri, etkinleştirme ve TEST adımları LoglamaGecisBagisciYonetimi.md dosyasındadır. Çalışma zamanı sonucu henüz doğrulanmadı.

## İade, silme ve birleştirme işlem grubu

Kullanıcı bağışçı yönetimi parçasını tamamlandı bildirdi ve devam istedi.
Temiz ve güncel f6afc6f8ea92a71070d99916639b243ba2df5927 üzerinden codex/nbys-iade-silme-birlestirme-audit açıldı.
BagisIadesiWP, NakitBagisHareketSilmeWP ve NakitBagisciBirlestirmeWP aynı daldadır. İş okumaları, doğrudan bağlantı düzeltmeleri, iş yazmaları, arşiv ve audit aynı transaction'a alındı; birleştirmede tüm seçim tek işlemdir.
Süre/yetki/seçim kontrolleri kayıt anında yapılır. Silmede kalan referanslar korunur; birleştirmede geçersiz armağan geçmişi, ekstre ve düzenli bağışçı bağlantıları da taşınır.
Boş yeniden armağan hesaplama yardımcıları ve Mesaj querystring'inden sahte başarı üretimi kaldırıldı. Yeni hak kazanma algoritması eklenmedi.
Ayrıntılı davranış farkları, alan izin listeleri, mesaj değişiklikleri ve TEST adımları LoglamaGecisIadeSilmeBirlestirme.md dosyasındadır. Çalışma zamanı sonucu henüz doğrulanmadı.

## Banka yükleme, ekstre ve bağışa aktarma işlem grubu

Kullanıcı geniş kapsamlı banka/ekstre zincirine devam edilmesini istedi. Temiz ve güncel 0bf73c6150fa1fb1d05830ebe28e063c85445780 üzerinden codex/nbys-banka-ekstre-aktarim-log-mesaj açıldı.
BankaDosyasiYuklemeWP, EkstreListesiWP, EkstreAktarmaEditWP ve NakitBagisciEslestirWP aynı daldadır.
Dosya başına import ve satır başına bağışa aktarma sınırları korunarak açık bağlamlı SQL izleri ve aynı transaction'da merkezi audit eklendi. Manuel ekleme/düzenleme/eşleştirme ve seçim grubunun tamamını kapsayan arşivli ekstre silme transaction'a bağlandı.
Aktarılmış/bağış bağlantılı kayıtların değiştirilmesi/silinmesi engellendi. Dosya ve toplu aktarım sonuçları tek bildirimde sayılarla gösterilir; biçim/mükerrerlik uyarıları teknik arızadan ayrılır, URL ile sahte başarı yolu kaldırılır.
Bu kapsam bağımsız düzenli bağışçı dosya yükleme/yazma, SMS gönderimi ve belge/basım akışlarını tamamlamaz. Armağan hak kazanma algoritması yeniden tasarlanmadı.
Audit alan listeleri, mesaj farkları, etkinleştirme ve TEST adımları LoglamaGecisBankaEkstreAktarim.md içindedir. Codex build/test/debug çalıştırmadı; çalışma zamanı sonucu kullanıcı doğrulamasını bekler.

## FTK kişi, kurul işlemleri ve rapor grubu

PR #510 master'a alındıktan sonra kullanıcı devam istedi. Temiz ve güncel 848fa2ef77806af57e06524841dcba4ca46f7129 üzerinden codex/nbys-ftk-islemleri-log-transaction-mesaj açıldı.
FTKKisiGirisiWP, FTKIslemleriWP, FTKListesiWP, FTKKuruluOlmayanIlIlceListesiWP, FTKHaritasiWP ve BolgelereGoreFTKRaporuWP aynı daldadır.
Kişi yazmaları ve geçmiş bağlantısı kontrolü; kurul işlemi, sayaç ve bütün üye geçmişi; son kaydın düzeltilmesi ve onaydaki üyelerin toplu görev sonlandırması transaction ve merkezi audit yoluna alındı. Liste/rapor/harita ve yer/bölge okumaları açık bağlamlıdır.
Beklenen doğrulamalar ayrı Türkçe uyarılardır; teknik hatalar güvenli tek mesaj/başvuru koduyla gösterilir. Mesaj farkları, yeni referans/boş üye korumaları, FTK uygulama kilidi, audit alan listeleri ve TEST adımları LoglamaGecisFtkIslemleri.md içindedir.
FTKYazilariWP belge/SharePoint/e-posta kapsamıyla sonraki parçada kalır. Düzenli bağışçı, SMS ve diğer belge/basım akışları tamamlanmış sayılmaz. Codex build/test/debug çalıştırmadı; çalışma zamanı sonucu kullanıcı doğrulamasını bekler.

## Düzenli bağışçı işlem grubu

Kullanıcı FTK parçasını tamamlandı bildirdi ve devam istedi. Temiz ve güncel 75a61ca8ecfbc037f598d3c937cb560a841293b8 üzerinden codex/nbys-duzenli-bagisci-transaction-audit açıldı.
DuzenliBagisciDosyaYuklemeWP ve DuzenliNakitBagisciListesiWP'nin yükleme/eşleştirme/belge kaydı zinciri taşındı. Satır başına transaction ve kısmi sonuç korunur; armağan ve iki bağlantı tek transaction'dır. Okumalar açık bağlamlı, audit iş transaction'ındadır.
Boş/çoklu aday eşleştirmesi, beklenen doğrulamalar, tekrar belge oluşturma, yanlış tam başarı ve çift hata bildirimi düzeltildi. Excel için sayısal türleri ve tarih sistemini koruyan ayrı okuma yolu eklendi; eski okuyucu çağrıları korunur.
Mesaj farkları, audit listeleri, yapılandırma, eşleşmeyen satırların tekrar yükleme sınırı ve TEST adımları LoglamaGecisDuzenliBagisci.md içindedir. Belgeyi basma/gönderme, ArmaganEditWP, SMS ve diğer belge akışları tamamlanmış sayılmaz. Codex build/test/debug çalıştırmadı; çalışma zamanı sonucu kullanıcı doğrulamasını bekler.

Kullanıcı armağan yönetimine devam edilmesini istedi. Temiz ve güncel 0fd5be3d1843b180ad84b99aa3e70ed9385a2021 üzerinden codex/nbys-armagan-yonetimi-transaction-audit açıldı.
ArmaganEditWP, ArmaganListesiWP ve ArmaganAraWP birlikte ele alındı. Düzenleme/belge iadesi ortak transaction/audit servisine taşındı; sahiplik, bölge yetkisi ve para iadesi durumu yazma sırasında yeniden doğrulanır. Liste/arama/modal iş verisi okumaları açık log bağlamlıdır; bölge bilgisi her istekte sunucudan alınır.
Mesajlar Türkçe doğrulama/iş kuralı uyarısı, tekrar istekte Info, commit sonrası tek başarı ve teknik hata için güvenli başvuru kodu olarak ayrıldı. Yeni armağan ekleme açılmadı; ölü ekleme yolu kaldırıldı. Ayrıntılar, mesaj değişiklikleri, yapılandırma ve inceleme/test noktaları LoglamaGecisArmaganYonetimi.md içindedir.
Belge üretimi/basımı, rapor hedef ekranları ve SMS/gönderim sonraki kapsamdır. OpenXML sürüm yükseltmesi ertelidir; bu parça referans değiştirmez. Codex build/test/debug çalıştırmadı; çalışma zamanı sonucu kullanıcı doğrulamasını bekler.

## Belge ve basım işlem grubu

Kullanıcı #516 sonrasında belge/basım grubuna devam edilmesini istedi. Temiz ve güncel 21270d5d2c06e9aa7fc38cb3f29ac2511971a80f üzerinden codex/nbys-belge-basim-log-transaction-mesaj açıldı.
TesekkurBasimiWP, BeratBasimiWP, TesekkurBelgesiYazisiWP, BeratBelgesiYazisiWP ve DuzenliBagisciBelgesiBasimiWP taşındı. Belge/etiket kayıt kümesi sabitlendi; seçime bağlı durum güncellemesi küme/yetki doğrulaması ve merkezi audit ile tek SQL transaction'a alındı. Yanlış tüm-bölge UPDATE'leri ve iç hatadan sonra tam başarı mesajı kaldırıldı.
Dosya/SQL/rapor dış etkileri atomik kabul edilmez; kısmi sonuçlar tek güvenli mesajla açıklanır. Mesaj farkları, merkez/bölge erişimi değişikliği, dosya kaldırma, yapılandırma ve TEST noktaları LoglamaGecisBelgeBasim.md içindedir. Harici rapor hedefleri, FTKYazilariWP, bağışa vesile olanlar ve SMS tamamlanmış sayılmaz. Codex build/test/debug çalıştırmadı.

## Bağışa vesile olanlara teşekkür işlem grubu

Kullanıcı #517 sonrasında devam edilmesini istedi. Temiz ve güncel ad6f89609a31fec40d6fa640ecf64d49159981b0 üzerinden codex/nbys-bagisa-vesile-transaction-belge açıldı.
BagisaVesileOlanTesekkurGirisiWP, BagisaVesileOlanTesekkurListesiWP ve BagisaVesileOlanArmaganBelgesiWP taşındı. Kayıt/güncelleme/silme ve audit aynı transaction'dadır. İlk oluşturma bilgileri ve form dışı telefon/adres korunur; audit ayrıntısı yalnızca BelgeTarihi'dir. Kaynakta bölge alanı bulunmayan entity için yeni bölge yetkisi icat edilmedi.
İki ekranın Word üretimi ortaklaştırıldı. Birleşik ekranda commit edilen Id dosya adımından önce korunur; dosya hatasından sonra aynı formdan tekrar denemede yeni kayıt eklenmez. Tek mesajla SQL/SharePoint kısmi sonuçları, beklenen doğrulamalar ve güvenli teknik hata/başvuru kodu ayrılır. Eski kök code-behind kopyası Compile listesinde değildir ve değiştirilmedi.
Mesaj farkları, yapılandırma, kalıcı idempotency sınırı, Word ve TEST inceleme noktaları LoglamaGecisBagisaVesile.md içindedir. FTK yazıları, SMS ve kalan rapor/ortak ekranlar sonraki kapsamdır. Codex build/test/debug çalıştırmadı.


## Kalan raporlar ve taşınmaz armağan işlem grubu

Kullanıcı #518 sonrasında devam edilmesini istedi. Temiz ve güncel 74cc87fe00749da3ec7e69aec13e2e9b386cf33a üzerinden codex/nbys-kalan-rapor-tasinmaz-transaction açıldı.
BolgeArmaganRaporuWP, CokDefaBagisYapanBagisciListesiWP, NakitBagisciAdresListesiWP, NakitBagisRaporuWP, TasinmazArmaganListesiWP, TasinmazArmaganWP ve TasinmazBagisciAdresListesiWP taşındı. Çoklu bağıştan madalya oluşturma, dergi tercihi ve taşınmaz armağan alanları aynı transaction içinde güncel yetki/kayıt kontrolü ve audit ile yazılır. Teknik hata/başvuru kodu, anlamlı doğrulama, değişikliksiz istek ve commit sonrası tek sonuç mesajı ayrıldı. Paylaşılan TBYS erişimi NBYS için yalnızca okuma kabul eder; NBYS yazmaları ayrı transaction servisindedir. Adres etiketi üretimi tek kayıt kümesi, güvenli XML değerleri, kapatılan paketler ve tek dosya sonuç mesajıyla düzenlendi.
Mesaj farkları, yeni/korunan iş kuralları, bölge kontrolü, kitaplık sınırı, yapılandırma ve TEST inceleme noktaları LoglamaGecisKalanRaporTasinmaz.md içindedir. Bölge raporunda önceki dönemden kalan hücreler sıfırlanır ve son günün saati dahil edilir. Kaynak seviyesinde 55 Compile edilen code-behind'ın 49'unda açık istek bağlamı vardır; bu sayı runtime/test tamamlanma oranı değildir. Kalanlar: FTKYazilariWP, HomeWP, NBYSYetkilendirmeWP, SMSAylikBagisCizelgesi, SMSAylikCizelgeReadOnlyWP ve SPDosyaListesiWP. Ortak yardımcılar ve uçtan uca son tarama da kapanışta incelenmelidir. Codex build/test/debug çalıştırmadı.


## FTK yazıları, dosya listesi ve ortak ekranlar

Kullanıcı #519 sonrasında devam edilmesini istedi. Temiz ve güncel c057cf4640ac3da4ace6aed5aefa3cefdff4b066 üzerinden codex/nbys-ftk-yazilari-dosya-ortak-log açıldı.
FTKYazilariWP, SPDosyaListesiWP, HomeWP ve NBYSYetkilendirmeWP taşındı. Salt okuma bağımlılıkları NBYS bağlamlıdır; Portal/IKYS ekranlarına veya yazma servislerine geçilmedi. FTK yazısında tek üye kümesi, kapatılan Word paketleri, güvenli XML değerleri, benzersiz dosya ve dosya/SMTP kısmi sonuçları uygulanır. Eski hata yutan e-posta yardımcısı bu ekranda çağrılmaz; sunucu kabulü teslim kanıtı sayılmaz. Dosya kaldırma liste/onay/snapshot/yetki doğrulaması ve geri dönüşüm kutusuyla yapılır; çift dosya atomik sayılmaz, kısmi/belirsiz sonuç tek mesajla açıklanır. Kaydet metodu boş olan yetki ekranı açıkça yalnızca görüntülemedir; yeni yetki yazma özelliği eklenmedi.
Mesaj farkları, kitaplık/snapshot sınırı, eski etiket eşleştirmesi, e-posta tekrar deneme sınırı, yapılandırma ve TEST noktaları LoglamaGecisFtkYaziDosyaOrtak.md içindedir. Kaynak seviyesinde 55 Compile edilen code-behind'ın 53'ünde açık istek bağlamı vardır; bu sayı runtime/test tamamlanma oranı değildir. Kalan SMSAylikBagisCizelgesi ve SMSAylikCizelgeReadOnlyWP birlikte sonraki parçadır; “ReadOnly” ekranın da açılışta yıl/ay kaydı yazdığı tespit edildi. Ortak yardımcı ve uçtan uca kapanış taraması henüz tamamlanmış sayılmaz. Codex build/test/debug çalıştırmadı; DB/SharePoint/SMTP iş isteği yapmadı.


## Aylık SMS çizelgeleri

Kullanıcı #520 sonrasında devam edilmesini istedi. Temiz ve güncel f796a175c38076a9e99bc2771f92c819e24c97c1 üzerinden codex/nbys-sms-cizelge-transaction-audit açıldı.
SMSAylikBagisCizelgesi ve SMSAylikCizelgeReadOnlyWP taşındı. Salt okunur ekran artık yıl kaydı yazmaz ve yıl değişiminde düzenleme sayfasına geçmez. Düzenleme ekranının eksik ay oluşturması ve aylık adet güncellemesi, merkez yetkisi, yıl uygulama kilidi, kilitli gerçek kayıt okumaları ve audit ile tek transaction'dadır. Mevcut aylar/SMSTutari/açıklama/ilk metadata korunur; mükerrer/geçersiz ayda otomatik düzeltme yapılmaz. Popup snapshot'ı ve eşzamanlı değişiklik kontrolü eklendi. Yeni ayların Ocak tutar sınırı düzeltildi; mevcut Ocak tutarları değiştirilmedi.
Ekran/Excel satırları ortaklaştırıldı; aynı istek içi kayıt listesi, long adet toplamları, dinamik satır/düğme kimlikleri ve Excel tam postback yolu kullanılır. Beklenen uyarı, aynı değerde Info, commit sonrası tek başarı ve yenileme/teknik hata için güvenli sonuç/başvuru kodu ayrılır. Mesaj farkları, merkez yetkisi değişikliği, uygulama kilidi sınırı, Ocak tutar sınırı, yapılandırma ve TEST inceleme noktaları LoglamaGecisSmsCizelge.md içindedir.
Kaynak seviyesinde 55 Compile edilen code-behind'ın 55'inde açık istek bağlamı vardır; bu, NBYS'nin test veya uçtan uca geçişinin tamamlandığı anlamına gelmez. Ortak yardımcılar, eski erişim/log yolları, derlenmeyen kopyalar ve uçtan uca kapanış taraması sonraki kapsamdır. Portal/MTS/MFYS'ye geçilmedi. Codex build/test/debug çalıştırmadı ve veritabanına iş isteği yapmadı.

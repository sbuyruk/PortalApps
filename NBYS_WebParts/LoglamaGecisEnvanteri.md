# NBYS merkezi loglama geçişi — envanter ve parça 1

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

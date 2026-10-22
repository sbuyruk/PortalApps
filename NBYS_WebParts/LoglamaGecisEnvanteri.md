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

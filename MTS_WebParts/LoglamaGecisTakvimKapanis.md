# MTS merkezi loglama — takvim, kart ve kapanış

Dal: `codex/mts-takvim-kapanis`. Başlangıçta yerel master temizdi ve canlı uzak master ile aynı commit'teydi: `bd12cb5d57cff4253712e701dbd534e305ec2bdc` (PR #533).

Bu, planlanan dört geniş kod parçasının sonuncusudur. MTS'nin tamamlanması için bu dalın kullanıcı tarafından derlenmesi, Test sunucusunda yayımlanması ve aşağıdaki senaryoların değerlendirilmesi gerekir. Statik inceleme çalışma zamanı testi değildir. MFYS'ye geçilmedi.

## Kapsam ve mesaj inceleme noktaları

| Ekran | Değişiklik ve beklenen mesaj davranışı |
| --- | --- |
| FaaliyetViewerWP | Faaliyet, toplantı, kişi/personel kutlaması ve tatil okumaları MTS bağlamını taşır. Sürükleme ve süre değiştirme transaction servisine bağlandı. Geçersiz tarih, kayıp kayıt ve başka oturumda değişmiş kayıt anlamlı tek uyarıdır. Başarı transaction tamamlandıktan sonra gösterilir. |
| HaftalikGorunumWP | Aynı tarih değiştirme/transaction yolu ve bildirim davranışı kullanılır. Bozuk `timeGridMonth` JavaScript ifadesi kaldırıldı; varsayılan haftalık görünüm ve dört günlük görünüm düğmesi düzeltildi. |
| FaaliyetROViewerWP | Salt okunur takvimde her olayın sürükleme ve süre değiştirme özelliği kapalıdır; olayın kendi düzenlenebilirlik alanı genel ayarı aşamaz. Toplantı/faaliyet ayrıntıları güvenle okunur; bulunamayan kayıt için boş veya önceki modal açılmaz. |
| FaaliyetKartiWP | Açılış, yönlendirme, katılımcı/dağıtım okumaları ve Excel için ortak hata sınırı eklendi. Eksik kişi satır içinde belirtilir; satır başına popup açılmaz. Eksik faaliyet anlamlı uyarıdır. Yeniden oluşturulan tablolar eski satırları biriktirmez; başarısız yüklemede yarım kart/Excel sunulmaz. |
| FaaliyetViewerCustomWP | Faaliyet ve toplantı okumaları bağlamlıdır. Sabit Aralık 2022 yerine `InitialDate=yyyy-MM-dd` ile seçilen veya güncel ay gösterilir. Pazar dahil tam hafta satırları, ayın tüm günleri ve Türkçe başlıklar üretilir. Geçersiz tarih anlamlı uyarıdır. Excel mevcut tabloyu aktarır; aynı istekte ikinci sorgu turu yapmaz. Hatalı yükleme yarım takvim olarak gösterilmez. |
| TestWP | Feature2 içinde gerçekten paketlendiği doğrulandı. Örnek toplantı okumaları ve örnek bildirim düğmeleri güvenli hata sınırına alındı. Eksik örnek toplantı uyarıdır; SMTP hatası teknik log + başvuru kodudur. Başarı yalnızca gönderim döndükten sonra gösterilir. Timer'daki iki saniyelik bloklama kaldırıldı. |

Beş iş ekranının dinamik kart/modal metinleri HTML olarak çalıştırılmaz; uygulamanın ürettiği satır sonları korunur. Takvim JSON'u metin değiştirilerek birleştirilmez: diziler ayrıştırılıp yeniden serileştirilir, `<`, `>`, `&` ve JavaScript satır ayırıcıları kaçırılır. Eski `Faaliyet.ToJSON` dönüştürücüsü `startEditable` alanını üretmiyordu; bu takvim yolu doğrudan DTO'yu serileştirir. Query string tarih/görünüm değerleri doğrulanır ve JavaScript'e JSON olarak yazılır.

Kişi ve personel kutlamalarında 29 Şubat, artık yıl olmayan yıllarda 28 Şubat'ta gösterilir. Personelin kutlama seçimleri üç takvimde de uygulanır; boş tarih takvimi düşürmez. Tatil okumaları mevcut kapsamı korur; tekrar eden tatiller her hedef yıl için kaynak tarih değiştirilmeden oluşturulur.

## Transaction ve bildirim

- İki düzenlenebilir takvim, ekranda gösterilen faaliyetlerin sürümünü aynı okumanın sonucundan hesaplayıp MAC korumalı ViewState'te saklar. Postback sırasında sürüm, işlemden önce yeni okuma ile değiştirilmez. Başka oturumda değiştirilmiş kaydın üzerine yazılmaz.
- `MtsActivityWriteService.Move`, önceki parçada hazırlanan `PortalApps.MTS.Write` transaction kilidini ve aynı transaction içindeki okuma/yazma/audit yolunu kullanır. Diğer faaliyet alanları, katılım, dağıtım ve stoklar değiştirilmez. Açık tarihli faaliyeti bu yol üzerinden planlama isteği reddedilir. Tüm gün/saatli olay dönüşümü bu sürükleme akışından yapılmaz; faaliyet formu kullanılır.
- UTC `toISOString()` girdisi açık biçimde ayrıştırılıp Türkiye saatine çevrilir. Tarayıcı takvimi yerel saatle çalıştığından testte tarayıcı/bilgisayar saat dilimi Türkiye olmalıdır. Sürüklenen görüntü istekten önce geri alınır; başarısız işlem tarayıcıda kaydedilmiş gibi yerinde kalmaz. Başarılı işlem sonrası kalıcı veri tekrar yüklenir.
- Eski faaliyetlerde boş `UniqueId` okunurken rastgele GUID üretilmesi sürüm kontrolünü bozuyordu. Faaliyet okumasında boş değer kararlı `Guid.Empty` olarak eşlenir; kalıcı UID gereken başarılı yazıda bir kez atanır. Sürüm ve bildirim UID'si bu yolla tutarlıdır.
- İki takvimde de yalnızca `TakvimeIslendi` seçiliyse özel kalem bildirimi gönderilir; iptal edilmiş faaliyet CANCEL olarak gönderilir. SMTP çağrısı transaction dışındadır. Bildirim hatası veriyi geri almaz ve genel “kaydedilemedi” mesajına dönüşmez: tarih güncellemesinin tamamlandığı ve bildirimin kontrol edilmesi gerektiği tek uyarıda belirtilir. Yenileme hatası da teknik loga gider. Otomatik yeniden gönderim/outbox eklenmedi.
- Tam/async postback sonrasında takvim yeniden oluşturulurken önceki FullCalendar örneği kapatılır. Modal ve filtre postback'lerinde takvim, olay işlendikten sonra bir kez yüklenir.

## Bağımlılıklar ve son tarama

Portal toplantı ve toplantı parametrelerine açık bağlamlı salt okuma overload'ları eklendi; Portal yazma servislerinin proje kapsamı genişletilmedi. IKYS personel/kurum/tatil okumalarında kullanılan MTS yolları ortak SELECT executor'a bağlandı. Kutlama sorgusu yalnızca gereken ad/tarih/seçim alanlarını getirir. SQL trace sorgu metni veya kişisel alan değerlerini yazmaz.

MTS iş ekranlarındaki eski `ExceptionHelper`/`MessageHelper` kullanımları ve doğrudan legacy yazma çağrıları tarandı. Kalan bağlamsız kurucuların açık `LogContext` overload'larını kullandığı kontrol edildi. Bu dalda beş son iş ekranı ve paketlenen TestWP ele alındı. VisualWebPart1/VisualWebPart2 kaynakları boş şablondur; mevcut csproj Compile/Content veya feature item kayıtlarında yer almıyorlar. SharePoint'te geçmişten eklenmiş sayfa örneklerinin kullanımı kaynak depodan doğrulanamaz. Dosyalar/paket öğeleri silinmedi. TestWP'nin sabit toplantı `1773` ve sabit deneme alıcısı önceki örnek davranışıdır; iş ekranı sayılmaz, üretim kullanımı önerilmez.

## Yapılandırma ve kod üretimi

Önceki MTS anahtarlarına ek olarak şu iş ekranları seçilerek etkinleştirilebilir:

`MTS:FaaliyetViewerWP;MTS:HaftalikGorunumWP;MTS:FaaliyetROViewerWP;MTS:FaaliyetKartiWP;MTS:FaaliyetViewerCustomWP`

TestWP kullanılacaksa ayrıca `MTS:TestWP` seçilebilir; rutin iş testi için deneme e-postası göndermek gerekmez. Tüm MTS için `EnabledProjects` ayarı, dört parçanın Test sunucusu değerlendirmesi tamamlandıktan sonra açılmalıdır.

Bu dalda ASCX kaynakları veya `*.ascx.g.cs` dosyaları değiştirilmedi; yeni sunucu kontrolü eklenmedi. Yeni `Logging/MtsCalendar.cs` Compile kaydı eklendi. Önceki parçaların kaynak ASCX değişiklikleri için gereken Visual Studio kod üretimi ayrıca tamamlanmış olmalıdır.

## Kullanıcının derleme ve test sırası

1. Visual Studio 2026'da derleyin ve Test sunucusunda yayımlayın. Codex build/test/debug çalıştırmadı.
2. Beş iş ekranını dolu ve boş sonuçla açın. Doğru MTS/WebPart açılış kaydını ve etkinse SQL trace'i doğrulayın. Toplantı, tatil ve kutlama filtrelerini tam/async postback ile değiştirin; çift takvim veya eski modal kalmamalı.
3. Aylık ve haftalık takvimde saatli faaliyeti sürükleyin, süresini değiştirin, onaydan vazgeçin. Gün/saat, aynı faaliyet UID'si ve UPDATE/audit sonucunu kontrol edin. Tüm gün olayını başka güne taşıyın; saatli/tüm gün dönüşümü yapılmamalı. İşlem katılımcı ve stok miktarlarını değiştirmemeli.
4. Aynı faaliyeti iki sekmede açın. İlk sekmede kaydedin, ikinci sekmede taşıyın. İkinci istek anlamlı “kayıt değişti” uyarısı vermeli, ilk kaydı ezmemeli. Silinmiş kayıt ve geçersiz zaman girdisinde de başarı mesajı çıkmamalı.
5. Özel kalem seçeneği kapalı/açık faaliyetleri taşıyın. Kapalıysa bildirim gitmemeli. Açıksa doğru UID ve tarih ile bildirim beklenir. Kontrollü SMTP hatasında veri güncellenmeli; kullanıcıya bildirim eksikliği tek uyarıyla anlatılmalı, teknik hata aynı başvuru kodunda bulunmalı. Bu test gerçek alıcılara gönderim etkisi dikkate alınarak test verisiyle yapılmalıdır.
6. Salt okunur takvimde faaliyet, toplantı, tatil ve kutlama taşınamamalı/uzatılamamalı. Toplantı/faaliyet ayrıntıları, silinmiş faaliyet, eksik katılımcı ve açık tarihli listeyi kontrol edin.
7. Faaliyet kartını tekrar yükleyip Excel'e aktarın; aynı satırlar çoğalmamalı. Silinmiş kişi satır içinde görünmeli; olmayan faaliyet tek uyarı vermeli. `<`, `&`, tırnak ve Türkçe karakter içeren konu/açıklama/kurum/obje adları kod çalıştırmadan görünmeli.
8. Özel takvimde güncel ay, `InitialDate=2026-08-01` (altı hafta satırı), pazar faaliyetleri, yıl geçişi, artık yıl, boş ay ve Excel'i kontrol edin. Gün sonu 23:59 içindeki kayıtlar kaybolmamalı. Geçersiz `InitialDate` tek uyarı vermeli.
9. 29 Şubat doğum/evlilik kaydı, boş tarihler ve kapalı kutlama seçimlerini kontrol edin. Tatillerin geçmiş yıl/bu yıl/gelecek yıl, çok günlük ve yarım günlük görünümünü doğrulayın.
10. Kontrollü SQL hatasında tek güvenli mesaj/başvuru kodu ve teknik ayrıntı beklenir. Başarı mesajı ve yarım kart/takvim gösterilmemeli. Hata giderildikten sonra sayfayı yenileyin. Ardından önceki üç parçanın temel kişi/arama, faaliyet/katılım ve stok senaryolarını birlikte kontrol ederek MTS kapanışını değerlendirin.

Statik kontrol: UTF-8/BOM, C# ayraçları, proje Compile kayıtları, kaynak ASCX olay bağlantıları, transaction içi DB çağrıları ve diff temizliği incelendi. Derleme, SharePoint yaşam döngüsü, tarayıcı/SMTP/SQL çalışma zamanı sonucu kullanıcı testine bağlıdır.

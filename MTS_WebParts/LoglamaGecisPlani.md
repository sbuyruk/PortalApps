# MTS merkezi loglama geçişi — dört geniş parça

İlk parçanın başlangıcında master temiz ve uzak master ile aynıydı: `d06038f9f41b7f57c812322f3b05c951104dc56f`.

## Parçalar

1. **Liste ekranları ve ortak altyapı — PR #531 ile master'a alındı:** `codex/mts-listeler-log-altyapi`. FaaliyetListesiWP, FaaliyetListesiROWP, AcikTarihliFaaliyetListesiWP, KisiListesiWP, AramaGorusmeListesiWP, MtsOlayListesiWP.
2. **Kişi, kurum/görev ve iletişim — PR #532 ile master'a alındı:** `codex/mts-kisi-arama-transaction`. KisiGirisiWP, KisiKartiWP, AramaGirisiWP, BagisciListesiWP; kişi/kurum/görev okumaları, arama kaydı ve faaliyet bağlantısı, transaction/audit yolları. Ayrıntılar: [LoglamaGecisKisiArama.md](LoglamaGecisKisiArama.md).
3. **Faaliyet ve stok yazma bütünlüğü — PR #533 ile master'a alındı:** `codex/mts-faaliyet-stok-transaction`. FaaliyetGirisiWP; katılım, anı objesi dağıtımı/iadesi, depo stok güncellemeleri, silme/iptal ve takvim tarih değişikliği servisi. Aynı işleme bağlı faaliyet/katılım/stok yazıları aynı transaction'dadır. Ayrıntılar: [LoglamaGecisFaaliyetStok.md](LoglamaGecisFaaliyetStok.md).
4. **Takvim, kart ve kapanış — bu dal:** `codex/mts-takvim-kapanis`. FaaliyetKartiWP, FaaliyetViewerWP, FaaliyetViewerCustomWP, FaaliyetROViewerWP, HaftalikGorunumWP; üçüncü parçanın tarih değiştirme servislerinin ekranlara bağlanması, salt okuma bağımlılıkları ve son tarama. TestWP/VisualWebPart1/VisualWebPart2 için kullanım/paketleme durumu da incelenecek. Örnek ekranın varlığı üretim akışı sayılmayacak. Ayrıntılar ve kapanış testleri: [LoglamaGecisTakvimKapanis.md](LoglamaGecisTakvimKapanis.md).

MTS'de 16 iş ekranı ve 3 test/örnek ekran bulunuyor. Her parça güncel ve temiz master üzerinden yeni yerel dalda hazırlanacak; commit/push/PR/merge ve Windows/Test sunucusu derleme/test işlemleri kullanıcı tarafından yapılacak. Dört dal hedefi kapsam planıdır; somut bir engel çıkarsa kapsam ve etkisi bildirilecek.

## İlk parça: kapsam ve mesaj inceleme noktaları

| Ekran | Davranış ve mesaj değişikliği |
| --- | --- |
| FaaliyetListesiWP | Açılış ve SQL trace MTS bağlamını taşır. Tarih alanları Türkçe doğrulanır; boş/geçersiz veya ters aralık iş kuralı uyarısıdır. Bitiş günündeki saatler kapsanır. Filtreler işlendikten sonra tek liste sorgusu yapılır. Teknik hata tek güvenli mesaj ve başvuru kodudur. |
| FaaliyetListesiROWP | Salt okunur faaliyet listesi bağlamlı okunur. Gizlenen okuma hatası artık boş sonuç sayılmaz. Mevcut son üç aylık filtre korunur. Metin alanları HTML çalıştırmaz. |
| AcikTarihliFaaliyetListesiWP | Mevcut amaç filtreleri korunur; filtre sonrası tek yükleme, tek hata sınırı ve başvuru kodu eklendi. Boş sonuç gerçek boş tablodur; sahte boş satır kaldırıldı. |
| KisiListesiWP | Boş sorgu sonucu listeyi düşürmez. Telefon metni sayıya çevrilemediğinde tüm liste hata vermez; uluslararası/özel biçimli telefon metni korunur. Liste, dışa aktarma ve yönlendirme sınırları teknik ayrıntı göstermez. Kişi Kartı bağlantı metni düzeltildi. |
| AramaGorusmeListesiWP | Kişi, kurum/görev, görüşme, faaliyet ve kişi seçim modalı aynı MTS bağlamını taşır. Tarih aralığı anlamlı Türkçe uyarıdır. Geçersiz/eksik kişi seçimi genel teknik hata değildir. Seçilen kişi bulunamadığında yeniden seçim istenir. Modal okuma hatası boş/yarım modal gibi gösterilmez. Başlık ve randevu metinleri Türkçeleştirildi. |
| MtsOlayListesiWP | İlk istekte çift okuma ve boş sonuçta eski satırları tutan yenileme kaldırıldı. Bozuk/boş açıklama güvenle ayrıştırılır; FaaliyetId'den sonraki KatilimciId atlanmaz. Eksik kişi/faaliyet satır içinde belirtilir; satır başına popup açılmaz. Anı objesi adı sorgunun Adi alanından okunur. Dinamik açıklama metinleri HTML kodlanır. |

Altı ekranda yeni başarı mesajı eklenmedi. Teknik hatalar MtsWebPartLog ile ayrıntılı loga ve tek güvenli kullanıcı mesajına gider. Beklenen doğrulamalar MtsInputException ile uyarı olarak gösterilir. Başarısız yükleme sonrası eski tablo gizlenir; başarılı yenilemede yeniden gösterilir. JSON içinde `<`, `>` ve `&` kaçırılır. Tablo metin sütunlarında DataTables metin render'ı kullanılır; uygulamanın ürettiği işlem bağlantıları korunur. Dışa aktarma ve filtre akışları ayrıca incelenmelidir.

## Veri erişimi ve sınırlar

Faaliyet, Kisi, AramaGorusme ve MTSKurumGorev repository/service sınıflarına açık MTS bağlamı alan kurucular eklendi. Bu yolda okumalar ortak PortalSqlReadExecutor üzerinden çalışır; eski DbClass SQL loglaması devreye girmez. Sorgu metni, parametreler ve kişi bilgileri SQL trace'e yazılmaz. Bağlamsız mevcut çağıranlar önceki davranışını sürdürür; Portal'ın önceki açık-bağlam overload'ları korunur.

İlk parça salt okuma/yönlendirme kapsamındadır. Bağlamlı kuruculardan eski yazma metotlarına yanlışlıkla geçiş transaction geçişi tamamlanana kadar reddedilir. Mevcut bağlamsız yazma ekranları bu dalda değiştirilmez; henüz MTS'nin tamamı taşınmış değildir. Yeni audit olayı gerektiren iş verisi değişikliği yoktur. MTS_WebParts projesine helper için DAO proje referansı ve MtsWebPartLog Compile kaydı eklendi.

## Yapılandırma ve kod üretimi

Mevcut `PortalLogging.EnabledWebParts` değerlerine şu anahtarlar eklenebilir:

`MTS:FaaliyetListesiWP;MTS:FaaliyetListesiROWP;MTS:AcikTarihliFaaliyetListesiWP;MTS:KisiListesiWP;MTS:AramaGorusmeListesiWP;MTS:MtsOlayListesiWP`

MTS geçişi bitmeden `EnabledProjects` içine MTS eklemeyin.

Beş ASCX kaynağındaki JavaScript düzenlendi: FaaliyetListesiWP, FaaliyetListesiROWP, AcikTarihliFaaliyetListesiWP, KisiListesiWP, AramaGorusmeListesiWP. Yeni sunucu kontrolü eklenmedi. Bu kaynaklar için Visual Studio SharePoint kod üreticisiyle **Run Custom Tool** çalıştırılarak çıktıların yenilenmesi gerekir. Codex `*.ascx.g.cs` dosyalarını değiştirmedi; elle düzenlenmemelidir.

## Test ve inceleme

1. Yukarıdaki beş ASCX üzerinde Run Custom Tool çalıştırın; Visual Studio 2026/Test sunucusunda derleyip yayımlayın.
2. Altı listeyi açın. Her ilk açılış için doğru MTS/WebPart açılış kaydını, etkinse repository/metot/süre/sonuç SQL trace'ini doğrulayın.
3. Faaliyet tarih aralığında ilk/son günün saatlerini; boş, geçersiz ve ters aralığı deneyin. Uyarı anlamlı olmalı, teknik exception görünmemeli. Amaç ve açık tarih filtreleri beklenen satırları göstermeli.
4. Kişi ve faaliyet listelerinde dolu/boş sonuçları, Türkçe karakterleri, `<`, `&` içeren metni, telefon/dahili alanlarını ve işlem bağlantılarını kontrol edin. Metin HTML çalıştırmamalı; dışa aktarma kullanılabilir olmalı.
5. Arama listesinde kişi filtresi, Hepsi, görüşme şekli, tarih aralığı ve kişi seçim modalını deneyin. Tam/async postback sonrasında çift/eski satır veya DataTables yeniden başlatma hatası olmamalı. Var olmayan kişi ve seçim yapılmadan devam etme anlamlı tek uyarı üretmeli.
6. MTS olay açıklamasında faaliyet/katılımcı alanlarının sırasını, bozuk/boş açıklamayı, silinmiş kişi/faaliyeti ve anı objesi adı/adetlerini kontrol edin. Boş yenileme eski satırları temizlemeli.
7. Kontrollü SQL erişim hatasında altı ekranda tek güvenli hata ve başvuru kodu beklenir. Eski sonuçlar yeni sorgu sonucu gibi kalmamalı; hata giderildikten sonra yenileme çalışmalı. Aynı OperationId ile teknik logu bulun.
8. Düzenleme, yeni kayıt, kişi kartı, takvim, kapatma ve Excel yönlendirmelerini kontrol edin. Response.Redirect/Response.End kaynaklı ThreadAbort teknik hata sayılmamalı.

Statik kaynak kontrolü: UTF-8/BOM, C# ayraç dengesi, ASCX olay metotları, bağlam kurucuları, proje Compile kayıtları ve diff temizliği incelendi. Build/test/debug çalıştırılmadı; statik kontroller derleme ve çalışma zamanı doğrulaması yerine geçmez.

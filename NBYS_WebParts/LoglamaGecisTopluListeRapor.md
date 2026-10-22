# NBYS toplu liste ve rapor geçişi — 10 ekran

Dal: codex/nbys-liste-rapor-toplu-gecis
Başlangıç commit'i: f0066ca6a67b6402d54969300955643fb20293f8.
Kod değişikliği öncesinde master çalışma ağacı temizdi; git ls-remote ile yerel master ve GitHub master commit eşitliği doğrulandı.
Önceki banka/döviz parçası master'da bulunuyor. Kullanıcı daha geniş dallar istediği için bu parça önceki dar parçanın yeni test sonucunu beklemeden başlatıldı; önceki parça için yeni bir test onayı varsayılmadı.

## Ekran kapsamı

| WebPart | Birlikte taşınan okumalar ve işlevler |
| --- | --- |
| BagisHareketListesiWP | İl filtresi, hareket listesi/JSON, filtre postback'leri, Excel |
| BagisIadeListesiWP | İade listesi/JSON, ay/yıl filtreleri, Excel |
| NakitBagisciByBankaWP | Banka toplamları, eski/yeni bağış özetleri, tarih filtreleri |
| NakitBagisciByTarihWP | Tarih ve bölge toplamları, listeleme |
| NakitBagisciListesiByYilAyWP | Bölge/tarih listesi, bölge başlığı, bağışçı detay modalı, Excel |
| NakitBagisHaritasiWP | İl seçimi, il/yıl bağış özeti, detay modalı |
| NakitBagisRaporuByBolgeWP | Bölgelere göre aylık/yıllık toplamlar, yıl filtresi |
| NakitBagisRaporuByYil_il | Mevcut yıl/il raporu ve Excel |
| FTKKisiListesiWP | Konum bilgileriyle FTK kişi listesi, Excel ve yönlendirme |
| FTKFahriBaskanListesiWP | Aynı konumlu okumanın fahri başkan filtresi, Excel ve yönlendirme |

Bu ekranlardaki ilk açılışlar, 42 mevcut olay sınırı, filtre okumaları, mevcut detay ve dışa aktarma yolları kapsama dahildir. Yeni kayıt/düzenleme bağlantıları hedef yazma ekranını bu dalın kapsamına almaz.
Bu ekranların kodunda DB yazması yoktur; bu nedenle Audit/INSERT/UPDATE/DELETE veya yeni iş transaction'ı beklenmez.
Bağışçı/ekstre/armağan yazmaları, SMS çizelgeleri ve diğer FTK yazma ekranları taşınmış sayılmaz.

## Log ve repository sözleşmesi

- İlk istekte UserActivity; postback'lerde tekrar açılış kaydı yok. Aynı istekte bütün ilişkili kayıtlar ortak OperationId kullanır.
- NakitBagisHareketService/Repository ve FTKKisiService/Repository için açık PortalLogContext alan, yalnızca okuma amaçlı kurucu eklendi. Kapsamdaki ekranların bu servis oluşturma çağrıları bağlamlıdır.
- Bağlamlı SELECT, PortalSqlReadExecutor üzerinden ham SQL/parametre değerleri kaydedilmeden yürür. Trace adı repository/metot, tür SELECT, süre ve sonuç bilgisi taşır. Boş sonuç başarılı SELECT'tir.
- Yeni kurucular NBYS bağlamını doğrular. Yanlışlıkla yazma metodu çağrılırsa bu okuma yolu legacy yazıcıya geçmek yerine Türkçe teknik istisna üretir. Böylece henüz transaction/audit dönüşümü yapılmamış bir yazma sessizce çalışmaz.
- Varsayılan ve DbClass enjekte edilen eski kurucular mevcut davranışlarını sürdürür. Diğer yazma ekranları kendiliğinden taşınmaz; legacy iş/audit yolları korunur.
- Mevcut transaction parametreli SELECT metotları aynı transaction.Select yolunda kalır. Mevcut transaction parametreli yazmalar bu dalda merkezi audit'e geçirilmez; parametresiz kurucudan gelen eski çağıranlar eskisi gibi çalışır.
- Ortak IlService/IlRepository için GetAll/SelectAll ve GetByEnglishName/SelectByEnglishName bağlam overload'ları; BolgeService/BolgeRepository için GetById/SelectById bağlam overload'u eklendi. Yalnızca NBYS ekranlarının açıkça kullandığı overload'lar yeni okuyucuya gider; eski ortak imzalar korunur.
- Teknik Error/SQL hedefi mevcut bağımsız PortalTechnicalLog'dur. Loglama ayarı kapalı olsa da bağlamlı okumalar eski SQLLog_Table yazıcısına dönmez.
- Mevcut rapor sorguları/tarih parametreleri, filtre/gruplama ve hesaplamalar korunur. İki NBYS repository'sindeki 40 mevcut SQL metni statik karşılaştırmada aynı bulundu.
- Banka/bölge/ay başına mevcut ek sorgular korunur; bu parça sorgu sayısını azaltma çalışması değildir. Örneğin aylık raporda birden fazla SELECT izi normaldir.
- NakitBagisRaporuByYil_il mevcut 2017–2021 yıl aralığını korur. Önceki döviz raporunda belgelenen tarih aralığı konusu bu dalda değiştirilmez.

## Mesaj değişiklikleri ve inceleme noktaları

- 37 eski catch sunumu kaldırıldı. Yardımcıların exception'ı yutup boş JSON/eksik işlemle devam etmesi yerine hata tek olay sınırına ulaşır.
- NbysWebPartLog.ExecuteAction ortak hata sınırıdır; exception teknik loga, kullanıcıya güvenli Türkçe mesaj ve OperationId başvuru kodu gider. Aynı exception zincirinin tekrar loglanması/bildirilmesi mevcut yardımcıda engellenir.
- Aynı istekte bir WebPart teknik hatayla durduğunda sonraki postback olayları o WebPart için çalıştırılmaz. Örneğin Page_Load liste hatasından sonra Excel olayı aynı sorguyu yeniden çalıştırıp ikinci popup üretmez; diğer WebPart türlerinin olayları engellenmez.
- FTKKisiListesiWP ve FTKFahriBaskanListesiWP üzerindeki ham ex.Message gösterimi kaldırıldı.
- Response.Redirect/Response.End kaynaklı ThreadAbortException yeniden fırlatılır; teknik Error ve yanlış başarısızlık bildirimi üretilmez. Excel ve yönlendirmeler TEST'te ayrıca doğrulanmalıdır.
- Harita için boş il seçimi “Bağış bilgilerini görüntülemek için haritadan bir il seçin.”; bulunamayan il “Seçilen il bulunamadı. Haritayı yenileyip tekrar seçin.” Warning üretir. Bunlar teknik Error değildir.
- NakitBagisciListesiByYilAyWP bağışçı detayı boşsa Rows[0] erişimi yapılmaz; ad etiketi temizlenir, boş liste ve “Seçilen bağışçıya ait bağış kaydı bulunamadı.” Info döner. SQL hatası bu mesajla örtülmez.
- NakitBagisciByTarihWP geçersiz tarih için “Geçerli bir başlangıç ve bitiş tarihi girin.”; ters aralık için “Başlangıç tarihi bitiş tarihinden sonra olamaz. Tarihleri kontrol edin.” Warning gösterir. Bu durumlarda rapor sorgusu çalışmaz. Mevcut tarih ayrıştırma biçimi korunur; sessiz MinValue sonucu artık sorguya gönderilmez.
- İlgili rapor başlıkları, “Yıl” ve “Belge istemiyor” metinleri Türkçe karakterlerle düzeltildi.
- Bu ekranlarda kayıt başarı mesajı yoktur; bu dal yeni başarı popup'ı eklemez. Beklenen boş sonuç/uyarı ile teknik hata birbirinden ayrıdır. Rapor sorgusunun başarılı olması başka bir iş işleminin başarı kaydı olarak sunulmaz.

## Etkinleştirme ve dağıtım

Mevcut IKYS/TBYS ve önceki NBYS değerlerini koruyarak çalışan SharePoint web uygulamasının web.config dosyasındaki PortalLogging.EnabledWebParts listesine aşağıdaki değerleri ekleyin:

NBYS:BagisHareketListesiWP;NBYS:BagisIadeListesiWP;NBYS:NakitBagisciByBankaWP;NBYS:NakitBagisciByTarihWP;NBYS:NakitBagisciListesiByYilAyWP;NBYS:NakitBagisHaritasiWP;NBYS:NakitBagisRaporuByBolgeWP;NBYS:NakitBagisRaporuByYil_il;NBYS:FTKKisiListesiWP;NBYS:FTKFahriBaskanListesiWP

EnabledProjects listesine NBYS eklemeyin; yazma yapan diğer ekranlar henüz taşınmadı.
Yeni migration yok. Mevcut dbo.PortalLog_Table birleşik şeması ve yazma izinleri hedef ortamda bulunmalıdır. Bağlantı mevcut TSKGV_Connection üzerinden okunur. Veritabanı şeması, canlı ayarlar ve deploy ortamı Codex tarafından değiştirilmedi.
DAO.csproj yeni Ortak/PortalSqlReadExecutor.cs kaynağını derlemeye dahil eder. Mevcut proje referansları/paket çıktıları kullanılır; yeni feature veya ASCX kontrolü yoktur. Üretilmiş *.ascx.g.cs dosyaları değiştirilmedi.

## Toplu kullanıcı doğrulaması — Visual Studio 2026 / TEST

1. Solution/NBYS paketini kullandığınız yapılandırmada derleyip TEST'e yayımlayın; yeni DAO ve Model çıktılarının pakette olduğundan emin olun.
2. Yukarıdaki 10 ekranı açın. İlk açılışta birer UserActivity ve ilgili SELECT izleri beklenir. Aynı istekte OperationId ortak olmalı; yeni postback'ler açılış kaydı üretmemelidir. Harita ilk açılışı veri okumayabilir; il seçimiyle SELECT beklenir.
3. Listelerdeki yıl/ay/il/bölge filtrelerini, banka/tarih raporlarını ve detay modallarını deneyin. Önceki sürümle aynı filtrede satır, toplam ve detay sonuçlarını karşılaştırın. Aylık ve satır başına mevcut sorguların birden fazla iz üretmesi normaldir.
4. Excel bulunan ekranlarda çıktıyı indirin/açın; satırlar ve Türkçe karakterleri karşılaştırın. Kapat/Yeni Kayıt yönlendirmelerinde ve Excel Response.End sırasında Error oluşmamalı.
5. Haritada boş/geçersiz il seçimi, geçersiz/ters tarih aralığı ve bağışçı detayında kayıtsız durum anlamlı Warning/Info üretmeli; teknik Error veya yanıltıcı başarı mesajı üretmemelidir.
6. TEST'te kontrollü SELECT hatasında tek güvenli Türkçe hata ve başvuru kodu beklenir. Error ve Failed SqlTrace OperationId eşleşmeli; popup/mesajda exception, SQL, stack trace, kişi kimliği veya parametre değeri olmamalıdır. Hata sonrasında yardımcı akış boş JSON üretip tablo hazırlamaya devam etmemeli.
7. Kapsamdaki ekranlarda Audit/yazma kaydı oluşmamalıdır. Parametre pilotunun audit davranışını bu salt okuma ekranlarından beklemeyin.
8. Ayar kapalıyken ekranlar çalışmalı; yeni iz ve legacy çift SQL kaydı oluşmamalı. Kapsam dışındaki bir yazma ekranının mevcut davranışı da gerileme açısından kullanıcı tarafından kontrol edilmeli.

Codex build/test/debug çalıştırmadı. Kontrol kapsamı statik kaynak/imza/çağrı incelemesi, SQL metinlerinin korunumu, C# blok dengesi (derleyici kontrolü değildir), Compile kaynak/XML kayıtları, UTF-8/BOM ve git diff --check'tir.
Çalışma zamanı, Excel ve hata senaryoları kullanıcı doğrulamasını bekler. Commit/push/PR/merge kullanıcıya aittir.

## Sonraki dalların genişliği

Kalan iş ekran başına bölünmek yerine ilişkili işlem grupları halinde hazırlanacak: bağışçı ve bağış hareketi yazmaları; armağan ve bağlantıları; ekstre import/eşleştirme/aktarma; FTK kişi/işlem yazmaları; SMS ve belge akışları.
Her grupta gerçek SELECT/yazma zinciri, aynı transaction'daki audit, alan izin listesi ve mevcut mesajlar birlikte incelenecek. İlgisiz gruplar veya Portal/MTS/MFYS bu dala eklenmeyecek.
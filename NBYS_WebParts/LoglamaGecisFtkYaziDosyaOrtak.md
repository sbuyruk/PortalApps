# NBYS FTK yazıları, dosya listesi ve ortak ekranlar

Dal: `codex/nbys-ftk-yazilari-dosya-ortak-log`.
Başlangıç: `c057cf4640ac3da4ace6aed5aefa3cefdff4b066` (23 Ekim 2026).
Değişiklikten önce yerel master temizdi ve uzak master ile aynı commit olduğu doğrulandı. Kullanıcı #519 sonrasında devam edilmesini istedi.

## Kapsam ve mesaj değişiklikleri

| Ekran | Değişiklik ve sonuç mesajı |
| --- | --- |
| FTKYazilariWP | Açılış, seçimler, kuruluş/güncelleme Word belgesi ve isteğe bağlı e-posta merkezi hata sınırına alındı. Eksik/bozuk il-ilçe seçimi, boş FTK ve eksik evrak/imza bilgisi ayrı Warning verir. İl kuruluş yazısı için mevcut engel korunur, kullanıcı ilçe seçmeye veya güncelleme yazısına yönlendirilir. Güncelleme işlemi artık yanlışlıkla “kurulum yazısı” sonucu göstermez. |
| SPDosyaListesiWP | Açılış/listeleme, kaldırma onayı ve dosya işlemleri merkezi hata sınırına alındı. Seçim/yetki/değişmiş veya bulunamayan dosya anlamlı Warning verir. Başarı ancak dosyalar geri dönüşüm kutusuna taşındıktan ve liste yenilendikten sonra çıkar. İki dosyadan biri kaldırılıp diğeri hata verirse tek kısmi sonuç/başvuru kodu gösterilir. |
| HomeWP | İlk açılış UserActivity kaydı eklendi. Mesaj parametresi için oluşturulup yayımlanmayan sahte Exception listesi kaldırıldı. Düzenleme durumunda sabit Türkçe Info ve sonraki adım gösterilir; query string metni kullanıcıya basılmaz ve bu durum teknik Error sayılmaz. |
| NBYSYetkilendirmeWP | Mevcut ayar okumaları NBYS bağlamlıdır. Kaydet metodu kaynakta boştu; yeni yetki yazma özelliği eklenmedi. Ekran açıkça yalnızca görüntüleme olarak işaretlendi, checkbox ve Kaydet pasif. Eski, her postback'te form değerlerini yeniden ezme kaldırıldı. Kayıt yokluğu Info, birimlerin farklı değerler taşıması Warning'dir; farklı değerler tek bir ortak checkbox değeri gibi gösterilmez. |

Her WebPart ilk açılışta bir UserActivity üretir; postback tekrar açılış kaydetmez. Aynı istek içindeki okumalar ve teknik hatalar ortak OperationId kullanır. IKYS/TBYS'deki güvenli Türkçe mesaj ve başvuru kodu yaklaşımı, mevcut NbysWebPartLog/NbysReportFeedback üzerinden uygulanır. İç yardımcılar ExceptionHelper/MessageHelper ile ikinci mesaj üretmez. Ham exception.Message, SQL ve stack trace kullanıcıya gönderilmez.

## SQL okuma kapsamı ve proje sınırı

Bu dört ekranın iş akışları SQL iş verisi INSERT/UPDATE/DELETE yapmaz. Yeni transaction/audit yazma yolu veya migration eklenmedi. SharePoint dosya işlemleri ve SMTP, SQL transaction'ı gibi gösterilmez.

NbysSupportReadRepository/Service, bu ekranların bağımlılıklarını NBYS SqlTrace bağlamıyla okur: FTK bulunan iller/ilçeler, mevcut ProgramYetki ayarları, bildirim alıcıları ve SMTP parametresi. Parametreler SqlQuery ile bağlanır; SQL trace'e ham sorgu, parametre, kişi adı veya adres listesi yazılmaz. ProgramYetki'nin mevcut sabit 7/8/9 birim kapsamı ve iki koşul adı korunur; birim filtresi `IN (7,8,9)` olarak yazılır. Legacy çağrıdaki çift parantezli IN metni kullanılmaz. Birden fazla aynı koşul kaydı farklı değer taşıyorsa keyfi son satırın kazanması kaldırıldı.

Portal ve IKYS ekranlarına, yazma servislerine veya log izleme WebPart'larına geçilmedi. Ortak tablolardan gerekli salt okuma bağımlılıkları NBYS bağlamıyla yürür. NBYS parametresi ve FTK üyeleri mevcut bağlamlı NBYS servislerinden; il/ilçe/bölge tekil okumaları mevcut ortak bağlamlı overload'lardan alınır. Eski NBYSOrtak.FTKOlusturmaEPostasiGonder ve MailHelper metotları diğer çağıranlar için korunur; bu ekran artık o hata yutan yolu çağırmaz.

## FTK belge ve e-posta sonuçları

FTK üye kümesi belge komutu sırasında bir kez okunur ve aynı liste Word içeriğine işlenir. Bu bir SQL yazma/kurul oluşturma işlemi değildir. Kuruluş ve güncelleme şablon seçimleri korunur; eski `BASKAN ADI BOS` metni belgeye yazılmaz. Eksik başkan/fahri başkan verisi boş kalır; TEST'te belgeyi imzalamadan önce kontrol edilmelidir. Hazır imza/irtibat varsayılanları değiştirilmedi; kullanıcı gerçek güncel bilgileri doğrulamalıdır.

Kullanılmayan, açık OpenXML paketi bırakan paragraf okuması kaldırıldı. Bütün açılan Word paketleri ve bellek akışları kapatılır. Alanlar tek geçişte literal XML metni olarak escape edilir; `&`, `<` ve `$` değerleri XML/regex kodu sayılmaz. XML kaydetmeden önce doğrulanır. Şablonun yer tutucuları mevcut tek XML metin parçası biçiminde olmalıdır; parçalanmış Word run yer tutucularını birleştiren yeni bir şablon motoru eklenmedi. Belge tarih ve büyük harfleri Türkçe kültürle hazırlanır. OpenXML sürümü ve paket referansları değiştirilmedi.

Eksik/boş şablonda dosya yazılmaz. Adlandırma il/ilçe Id, zaman ve GUID içerir; overwrite=false eski dosyanın üzerine yazmayı engeller. Dosya yalnızca NBYSBELGELERI_LIB kitaplığına kaydedilir. Kullanıcı başarıyı dosya kaydından sonra görür. Upload sırasında hata olup dosyanın kaydedilip kaydedilmediği kesinleşmezse kitaplığı kontrol etme uyarısı verilir. Dosya kaydedilip bağlantı/ekran adımı hata verirse dosyanın kitaplıkta olduğu, e-postanın gönderilmediği tek hata mesajında açıklanır.

E-posta checkbox'ı boşsa SMTP çağrısı yapılmaz. Seçiliyse, ilgili bölgenin aktif personel kayıtları ve her kişinin Adres sırasına göre ilk iletişim kaydı okunur. Noktalı virgüllü adresler ayrılır, boşlar atılır ve tekrarlar çıkarılır. Gönderen mevcut uygulamadaki adrestir. SMTP parametresi yok/boşsa mevcut alternatif sunucu sabiti kullanılır. Alıcı yoksa belge hazır kalır, tek Warning ile e-postanın gönderilmediği açıklanır.

SmtpClient.Send yalnızca hatasız dönerse “e-posta bildirimi gönderim sunucusuna iletildi” denir. Bu, teslim/okunma kanıtı değildir. SMTP veya alıcı doğrulama hatası teknik loga yazılır; kullanıcıya belgenin hazır olduğu fakat e-posta gönderiminin tamamlandığının doğrulanamadığı ve tekrar göndermeden önce kontrol etmesi gerektiği tek mesajla bildirilir. Bazı alıcılara kabul olup bazılarının reddedilmesi de bu belirsiz sonuç kapsamındadır. Otomatik yeniden gönderim/geri alma yoktur. Yeniden belge komutu yeni dosya üretebilir; kalıcı e-posta idempotency kaydı eklenmedi.

## Dosya kaldırma sınırı

Liste, mevcut teşekkür belgesi dosyaları kapsamını korur: NBYSBELGELERI_LIB kök klasöründeki teşekkür adına uyan DOCX dosyaları. Query string başka kitaplık veya dosya yolu seçtiremez; şablonlar liste/kaldırma dışında tutulur. İndirme bağlantıları URL/HTML, dosya adları ve JS onay argümanları uygun biçimde encode edilir. Yazarı boş dosyada NullReferenceException oluşmaz.

Onay yalnızca sunucunun listede gösterdiği dosya/etiket çifti için verilir. ViewState'de ad, UniqueId, son değişiklik zamanı ve uzunluk saklanır. Onay postback'inde liste snapshot'ı yeniden yetkilendirilmez; önceden gösterilen tablo JSON'u korunur. Onay ve kaldırma sırasında gizli alanlar, snapshot ve SharePoint silme yetkisi tekrar karşılaştırılır. Onaydan önce değişmiş/silinmiş/yeniden yüklenmiş dosyada işlem durur. İki dosya da kaldırılmadan önce kontrol edilir; kontrol ile Recycle arasındaki yarış için SharePoint tarafında koşullu atomik bir çift dosya işlemi yoktur.

Eski kesin isim eşleşmesi bulunan etiket eşleştirmesi korunur. Eşleşmeyen, ayrı üretilmiş yeni etiketler isim benzerliğiyle tahmin edilip kaldırılmaz; bunlar belge kitaplığından ayrıca kontrol edilir. Bu ekran FTK yazılarının veya bütün adres etiketi dosyalarının genel listesi değildir.

Kalıcı Delete yerine SharePoint Recycle kullanılır; kullanıcı arayüzü geri dönüşüm kutusuna taşıma olarak güncellendi. Sonuç sayısı yalnızca tamamlanan Recycle çağrılarıyla artar. İki dosyanın kaldırılması atomik değildir; birincisi tamamlanıp ikincisi hata verirse tamamlanan sayı ve diğer dosyanın sonucunun doğrulanamadığı tek mesajda açıklanır. İlk Recycle sırasında bağlantı hatası olursa hiç dosya kaldırılmadığı varsayılmaz; kullanıcı sonucu kontrol etmeye yönlendirilir. Bütün dosyalar kaldırılıp liste yenilenemezse kaldırmanın tamamlandığı açıkça söylenir. Dosya/snapshot kontrolleri SharePoint izinlerini aşmaz, yükseltilmiş yetki kullanılmaz. Bu dosya işlemleri için SQL Audit ile dağıtık atomiklik iddiası yoktur; merkezi açılış/teknik hata logları ile SQL iş verisi audit'i ayrı kapsamlardır.

## Yapılandırma

Mevcut değerleri koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:FTKYazilariWP;NBYS:SPDosyaListesiWP;NBYS:HomeWP;NBYS:NBYSYetkilendirmeWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Önceki merkezi log tabloları/bağlantı ayarları gereklidir. Yeni şema veya migration yoktur. NBYS belge kitaplığında şablon okuma, dosya ekleme ve kaldırılacak dosya için silme izni gerekir. SMTP/kurumsal alıcı ayarları mevcut kurallardan alınır. WSP yayını bu ayarları ve izinleri kendiliğinden oluşturmaz.

ASCX değişiklikleri mevcut kontrollere ait metin/Enabled özellikleridir. Kontrol Id veya olay bağlantısı değiştirilmedi; üretilen ASCX dosyalarına dokunulmadı.

## Kullanıcının Visual Studio 2026 / TEST doğrulaması

1. Dört ekranın ilk açılışında tek UserActivity, postback'te tekrar açılış olmaması, doğru NBYS/WebPart ve ortak istek OperationId. FTK/parametre/il-ilçe/bölge/yetki/alıcı/SMTP SELECT'lerinde merkezi SqlTrace; eski SQLLog/OlayKayit'te ikinci kayıt olmaması.
2. FTK il/ilçe ve valilik seçimi, boş/bozuk veya artık bulunmayan query string Id'si, boş üye kümesi, eksik evrak/imza. Beklenen durumlar tek Warning olmalı; dosya/e-posta oluşmamalı. İl kuruluş engeli ve il/ilçe güncelleme şablonları korunmalı.
3. Kuruluş ve güncelleme yazısını e-posta kapalıyken oluşturun. Doğru belge adı, Türkçe sonuç mesajı, çalışan indirme bağlantısı ve tek başarı beklenir. Aynı anda iki komut eski dosyayı ezmemeli. Üye listesi, başkan/fahri başkan, kart numarası, tarih, imza, şablon satırları ve `& < $` karakterlerini Word'de görsel kontrol edin.
4. Eksik/boş/bozuk şablon ve upload izni yokluğu: tek güvenli teknik hata/başvuru kodu, arkasından başarı olmaması. Upload tamamlandıktan sonraki hata senaryosunda dosyanın kitaplıkta kaldığı doğru açıklanmalı.
5. TEST alıcıları/SMTP ile e-posta seçili senaryoları: alıcı yokluğu, yinelenen/noktalı virgüllü adres, bozuk adres, SMTP reddi, kısmi alıcı reddi ve normal sunucu kabulü. Belge kaybı/yanlış e-posta başarı mesajı veya çift popup olmamalı. E-posta checkbox'ı boşken hiçbir SMTP çağrısı olmamalı. Canlı alıcılara test bildirimi göndermeyin; TEST alıcı ayarlarını kullanın.
6. Dosya listesinde Türkçe/boşluk/tırnak karakterli ad, boş yazar, şablon, eski eşleşmiş belge/etiket çifti ve eşleşmeyen yeni etiket. Kitaplık/yol değiştirilen query string ve listede olmayan hidden ad kabul edilmemeli. Onay postback'inde tablo kalmalı; onay iki görünen dosyayı kapsamalı.
7. Tek dosya/çift dosya kaldırma, onaydan önce dosyanın değiştirilmesi/yeniden yüklenmesi/silinmesi, silme izninin kaldırılması ve eski onayın tekrar gönderilmesi. Başarıdan sonra dosyalar geri dönüşüm kutusunda olmalı. İkinci Recycle veya son liste yenilemesini kontrollü başarısız kılarak tek kısmi sonuç/başvuru kodunu kontrol edin; yanlış tam başarı çıkmamalı.
8. Home mesaj parametresi ilk açılışta sabit Info göstermeli, teknik Error üretmemeli. Yetki ekranı kaydetme özelliği varmış gibi davranmamalı; eşit/farklı/eksik ayar kayıtlarında doğru görüntüleme ve tek Info/Warning beklenir. ProgramYetki_Table'da yazma olmamalı.

Codex build, test veya debug çalıştırmadı; DB/SharePoint/SMTP iş isteği yapmadı ve belge üretmedi. Kontroller kaynak/metot imzaları, proje XML/Compile kayıtları, olay bağlantıları, kontrol kimlikleri, UTF-8/BOM ve diff ile sınırlıdır. Runtime ve Word görsel çıktısı kullanıcı doğrulamasını bekler.

Kaynak seviyesinde 55 derlenen code-behind'ın 53'ünde açık istek bağlamı vardır; bu, çalışma zamanı/test tamamlanma oranı değildir. SMSAylikBagisCizelgesi ve SMSAylikCizelgeReadOnlyWP bu parçada değiştirilmedi. “ReadOnly” SMS ekranının da açılışta kayıt oluşturduğu görülmüştür; iki ekran birlikte yıl/ay mükerrerliği, yetki ve transaction/audit bütünlüğüyle sonraki parçadır. Ortak yardımcılar ve uçtan uca kapanış taraması da henüz tamamlanmış sayılmaz.

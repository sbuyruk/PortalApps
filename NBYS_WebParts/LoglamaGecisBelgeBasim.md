# NBYS belge ve basım işlemleri

Dal: `codex/nbys-belge-basim-log-transaction-mesaj`.
Başlangıç: `21270d5d2c06e9aa7fc38cb3f29ac2511971a80f` (22 Ekim 2026).
Değişiklikten önce yerel master temizdi ve uzak master ile aynı commit olduğu doğrulandı. Kullanıcı #516 armağan yönetimini tamamladıktan sonra bu gruba devam edilmesini istedi.

## Kapsam

- TesekkurBasimiWP: dönem filtreleri, durum sayıları, teşekkür raporu ve etiket raporu açma istekleri.
- BeratBasimiWP: bölge/madalya durum sayıları, rapor/etiket açma istekleri, seçime bağlı durum güncellemesi.
- TesekkurBelgesiYazisiWP: Word belgesi, adres etiketleri, kitaplığa kaydetme ve indirme bağlantıları.
- BeratBelgesiYazisiWP: Word belgesi/etiket çifti, dosya listesi ve onaylı dosya kaldırma.
- DuzenliBagisciBelgesiBasimiWP: düzenli bağışçı belgesi, adres etiketleri, liste ve durum güncellemesi.
- NbysDocumentWriteService/Repository ve NbysDocumentFeedback; transaction-aware ArmaganRepository dönem sorgusu ve açık bağlamlı Bolge.GetSelected overload'u.

OpenXML sürümü/referansları değiştirilmedi. Üretilen ASCX dosyalarına dokunulmadı; ASCX değişiklikleri var olan checkbox metinleridir ve yeni kontrol bildirimi gerektirmez. FTKYazilariWP, bağışa vesile olanlar, SMS ve diğer rapor hedef ekranları bu parçada taşınmış sayılmaz.

## Belge, SQL ve dış etkilerin sınırı

Belge/etiket için kontrol edilmiş ve geçersiz olmayan armağanlar, seçilen dönem/tür/bölgeye göre bir kez okunur. Her iki dosyanın içeriği aynı DataTable görüntüsünden üretilir; belge ve etiket ayrı sorgularla farklı kayıt kümelerine kaymaz. Boş kümede belge yazılmaz ve durum değiştirilmez. ArmaganRepository'nin eski altı parametreli çağrıları eski erişim davranışını korur; yeni transaction parametreli dalda SELECT aynı transaction üzerinden yürür.

Durum güncelleme checkbox'ı seçiliyse, dosya kitaplığa kaydedildikten sonra kayıtlar transaction içinde yeniden doğrulanır. Kullanıcının güncel bölgesi, bağışçı kilitleri, kayıtların o anki durumu, dönem/tür/bölge filtresi ve belgeye giren alanların tamamı kontrol edilir. Kimlik kümesi veya belge alanları değişmişse hiçbir armağanın durumu güncellenmez; hazırlanmış belgeyi kontrol etme uyarısı verilir. Yeni eşleşen armağanlar geniş bir UPDATE ile yanlışlıkla gönderilmiş sayılmaz.

Her armağan UPDATE'i, gerçek DB önce/sonra görüntüleri ve merkezi audit aynı transaction içindedir. Audit/yazma hatasında bütün durum güncellemeleri geri alınır. Başarı yalnızca Complete ve Dispose/commit tamamlandıktan sonra bildirilir. Audit'in mevcut armağan güvenli alan listesi kullanılır; ad, kimlik, adres ve açıklamalar audit ayrıntısına veya SQL trace'e taşınmaz. Yeni şema/migration yoktur.

SharePoint kitaplığı, SQL veritabanı ve harici rapor görüntüleyicisi dağıtık transaction değildir. Dosya kaydedilmiş, sonradan SQL doğrulaması/yazması başarısız olmuşsa dosya korunur; kullanıcıya dosyanın hazır olduğu fakat gönderim durumunun güncellenmediği söylenir. Berat dosyası başarılı, etiket başarısızsa tek kısmi sonuç/hata mesajı ve başvuru kodu gösterilir; SQL durum güncellemesi yapılmaz. Otomatik dosya geri silme veya gönderimi tekrar deneme yoktur.

Rapor açma yolu yalnızca tarayıcıda raporu açma isteği hazırlar. Raporun gerçekten açıldığı, baskının tamamlandığı veya belgenin teslim edildiği bu sunucu kodundan doğrulanamaz. Checkbox seçilmişse mevcut kullanıcı talebi korunur ve ilgili kayıtlar transaction ile Gönderildi durumuna alınır; bu bir fiziksel gönderim kanıtı değildir. Mesaj bunu “Seçiminize göre ... kayıt ... güncellendi” diye belirtir. Checkbox seçilmediyse hiçbir durum yazması/audit yapılmaz. Rapor açma isteği ile SQL işlemi atomik değildir; SQL hatası olsa da tarayıcıya hazırlanmış rapor isteği kalabilir.

Dosya adlarına milisaniye ve rastgele ek kondu; aynı dakikadaki işlem eski dosyanın üzerine yazmaz. Kitaplığa boş içerik kaydedilmez; upload sırasında overwrite=false kullanılır. Belge şablonu eksik/boşsa işlem durur. Şablon paragraf/tablo elemanları kopyalanıp OpenXML paketi kapatılır; dönen elemanlar açık paket ömrüne bağlı değildir. Aynı istekte tekrar dosya üretimini engelleyen kalıcı bir işlem anahtarı eklenmedi; checkbox kapalı tekrar tıklamada ayrı dosya oluşabilir.

## Yetki ve silme

Her istekte kullanıcı bölgesi açık bağlamla yeniden okunur. Bölge bulunamadığında işlem durur; sıfır varsayılıp tüm bölgeler açılmaz. Word üretim ekranları bölge kullanıcısına yalnızca kendi aktif bölgesini sunar; güncel bölge ile eski seçim uyuşmazsa postback durur. SQL yazma sırasında bölge yetkisi ayrıca transaction içinde yeniden kontrol edilir.

İki eski toplu rapor ekranı (TesekkurBasimiWP/BeratBasimiWP), tüm bölgeleri kapsayan rapor/sayım yolları nedeniyle genel müdürlük/hepsi bölgesiyle sınırlandırıldı. Dış rapor hedeflerinin bölge yetkisini doğruladığı bu kaynakta kanıtlanamadığından bölge kullanıcılarına bu iki ekran üzerinden kapsam genişletilmez; bölgeye göre Word hazırlama ekranları kullanılabilir. Bu erişim değişikliği TEST'te kullanıcı rolleriyle ayrıca incelenmelidir. Harici rapor hedeflerinin kendi yetki/sorgu kodları bu parçanın doğrulaması değildir.

Berat dosyası silme isteğinde hidden input'taki ad tek başına kabul edilmez. Seçim sunucuda üretilmiş ve ViewState ile korunan belge/etiket çiftlerinden biri olmalı; onaylanan çift son istekte aynı kalmalıdır. Bölge değişikliğinde eski ViewState üzerinden işleme devam edilmez. Dosyalar geri dönüşüm kutusuna taşınır; boş veya daha önce kaldırılmış etiket adı yeni hata yaratmaz. Kaç dosyanın taşındığı ayrı sayaçta tutulur; ikinci dosya sonucu birincinin sonucunu ezmez. İki SharePoint dosyasının kaldırılması atomik değildir; sonradan hata olursa tamamlanan dosya sayısı tek mesajla bildirilir. Bu dosya işlemleri SQL entity audit'i veya dağıtık transaction olarak sunulmaz.

## Kullanıcı mesajları

Önceki parçalarda kaynak koddan doğrulanan IKYS/TBYS yaklaşımı sürdürülür: teknik ayrıntı Error loguna, kullanıcıya güvenli Türkçe mesaj ve OperationId başvuru kodu. NbysWebPartLog aynı hata zincirini bir kez bildirir. Bu beş ekranın tüm olayları ortak sınırdan geçer; alt metotlar hata yayınlayıp false/null ile başarı akışına devam etmez.

- Belge/etiket tamamlandı: tek Success ve indirme adımı. Checkbox seçilmediyse durumun değiştirilmediği açıkça yazılır.
- Gönderim durumu: yalnızca transaction kapandıktan sonra başarı; güncellenen kayıt adedi belirtilir. Seçili bölge yerine tüm bölgeleri güncelleyen eski çağrılar kaldırıldı.
- Boş kayıt kümesi/yetki değişikliği/kayıt kümesinin değişmesi: anlamlı Warning; teknik arıza gibi sunulmaz, yazma yapılmaz.
- Dosya başarılı/SQL başarısız veya belge başarılı/etiket başarısız: tek kısmi sonuç ve gerektiğinde başvuru kodu; tam başarı popup'ı yok.
- Commit başarılı/ekran yenileme başarısız: işlem sonucu ve sayfayı yenileme adımını içeren tek mesaj; tekrar işlem yapma önerilmez.
- Uzun adresler: kişi adı/adresi içeren satır başına popup yerine tek sonuçta kısaltılan adres sayısı ve etiketleri kontrol etme adımı.
- URL filtreleri: mevcut seçeneklere normalize edilir; yarım ay varsayılanı gün <=15 için 1, sonrası 2'dir. Sessiz catch kaldırıldı. Düzenli bağışçı tarih aralığında “Hepsi” aynı mevcut aralıkla kullanılır.
- Durum tabloları yenilenirken dinamik satırlar temizlenir, eylem düğmeleri yeniden hesaplanır; önceki sonucun düğmeleri/satırları ekranda birikmez.
- Normal yönlendirmeler hata olarak yayınlanmaz. Teknik yükleme hatası sonrası aynı isteğin diğer olayları işlenmez.

## Yapılandırma

Önceki değerleri koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:TesekkurBasimiWP;NBYS:BeratBasimiWP;NBYS:TesekkurBelgesiYazisiWP;NBYS:BeratBelgesiYazisiWP;NBYS:DuzenliBagisciBelgesiBasimiWP`

NBYS'nin tamamını EnabledProjects üzerinden açmayın. Merkezi log/audit tabloları ve önceki migration gereklidir. WSP yayını migration çalıştırmaz. Dosya kitaplığı ve şablonlar için mevcut SharePoint izinleri gereklidir; bu parça izin yükseltmez ve yeni veritabanı/sunucu ayarı uygulamaz.

## TEST inceleme noktaları

1. Beş ekranın açılışını, filtrelerini ve durum sayılarını kontrol edin. İlk istekte tek UserActivity, iş SELECT'lerinde doğru WebPart/OperationId, hatalarda tek güvenli mesaj beklenir. Bir dönemi art arda değiştirince tablo satırları birikmemeli.
2. Checkbox kapalıyken teşekkür, berat ve düzenli bağışçı Word dosyalarını/etiketlerini oluşturun. Word'de açılış, sayfa sayısı, şablon yerleşimi, ad/tarih/tutar ve adresleri kontrol edin. Durum ve SQL audit değişmemeli. İndirme bağlantıları çalışmalı; aynı dakika tekrar üretim eski dosyayı ezmemeli.
3. Checkbox açıkken yalnızca belgede bulunan seçili dönem/tür/bölge kayıtlarının Gönderildi olduğunu ve güvenli audit eski/yeni durumlarını kontrol edin. Başka bölgenin/başka türün armağanları değişmemeli. Boş, geçersiz veya para iadesi nedeniyle geçersiz armağanlar belgeye/yazmaya girmemeli.
4. Belge oluşturma sırasında başka oturumdan durum, tutar, belge adı, adres veya bağışçı bağlantısını değiştirin; yeni eşleşen kayıt eklemeyi de deneyin. Son doğrulamada bütün durum yazması durmalı; belge korunmalı ve tek kontrol uyarısı gösterilmeli. Büyük kayıt kümelerinde süre/kilit davranışını ayrıca ölçün.
5. Kontrollü audit/SQL hatasında tüm durum güncellemeleri geri alınmalı. Hazır dosyanın kaldığı ve durumun değişmediği tek mesajla anlaşılmalı. Commit sonrası liste okuma hatasında tamamlanan işlem başarısız gösterilmemeli.
6. Şablon eksik/boş, kitaplık yazma yetkisi yok ve belge başarılı/etiket hatası senaryolarında tam başarı olmamalı. Tek Error/başvuru kodu beklenir; teknik SQL/stack/exception metni kullanıcıya çıkmamalı. Uzun adreslerde isim/adres içermeyen tek toplu not beklenir.
7. Toplu rapor ekranlarında checkbox açık/kapalı davranışını ve dış raporun hangi durumları seçtiğini kontrol edin. Mesaj yalnızca rapor açma isteğini ve varsa SQL durum değişikliğini doğrulamalı; baskı/teslimat başarısı dememeli. Popup engelleyicisi ve dış rapor hatası SQL durumunu kendiliğinden geri alamaz.
8. Genel müdürlük ve bölge kullanıcılarını ayrı deneyin. Word ekranında bölge dışına seçim/URL/postback engellenmeli. Bölgesi bulunmayan kullanıcı ve bölgesi sonradan değişen oturum işlem yapamamalı. İki toplu rapor ekranının yeni merkez yetkisi sınırı beklenen rollerle doğrulanmalı.
9. Berat dosya listesinden belge/etiket çiftini onaylayıp kaldırın; taşınan sayı doğru olmalı. Etiket önceden kaldırılmışsa belge sonucu kaybolmamalı. Hidden input'a listede olmayan dosya/şablon adı koyma ve onaydan sonra çift değiştirme engellenmeli. İkinci dosya hatasında kısmi sonuç anlaşılmalı; geri dönüşüm kutusundan kurtarma mümkün olmalı.

Codex Windows üzerinde build, test veya debug çalıştırmadı; Word dosyası üretmedi, SharePoint/DB/rapor sunucusuna iş isteği göndermedi. Kaynak sınırları, transaction çağrıları, gerçek imzalar, Compile kayıtları, markup olayları/kontrol kimlikleri, XML, UTF-8/BOM ve diff statik incelendi. Çalışma zamanı, belge görsel çıktısı ve rollback doğrulaması kullanıcı tarafından Visual Studio 2026/Test sunucusunda yapılmalıdır.

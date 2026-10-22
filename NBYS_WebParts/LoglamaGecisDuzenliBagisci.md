# NBYS düzenli bağışçı yükleme, eşleştirme ve belge kaydı

Dal: `codex/nbys-duzenli-bagisci-transaction-audit`.
Başlangıç: `75a61ca8ecfbc037f598d3c937cb560a841293b8` (22 Ekim 2026).
Kod değişikliği öncesinde master çalışma ağacı temizdi; GitHub master ile aynı commit olduğu doğrulandı. Kullanıcı FTK parçasını tamamlandı bildirdi ve devam istedi. PR #511 ve aradaki IKYS yönlendirme düzeltmesi bu tabana dahildir; IKYS kaynakları bu parçada değiştirilmedi.

## Kapsam

- DuzenliBagisciDosyaYuklemeWP: Excel okuma, eşleşme araması, aktif düzenli kayıt ekleme/güncelleme, satır sonuçları ve hata sınırları.
- DuzenliNakitBagisciListesiWP: açılış/filtre/listeler, bağışçı detayı, eşleştirme dönüşü ve düzenli bağışçı armağan/belge kaydını oluşturma.
- Yeni NbysRegularWriteService/Repository ve NbysRegularFeedback; mevcut merkezi audit yazıcısının düzenli bağışçı alanları.
- NakitBagisciReportService/Repository: açık bağlamlı liste ve bağış adedi eşik okumaları. Parametresiz çağıranların erişim yolu korunur.
- ExcelHelper: yalnızca yeni çağrının kullandığı, sayısal hücre türlerini koruyan okuma yolu. Mevcut ReadXLSXAsDataTable çağrıları eski davranışını sürdürür.

Bu parça belge dosyasını üretmez, basmaz veya göndermez. DuzenliBagisciBelgesiBasimiWP, ArmaganEditWP, SMS/e-posta ve diğer belge/basım akışları sonraki kapsamdır. Eski parametresiz DuzenliNakitBagisciService/Repository ve diğer ekranların yolları değiştirilmedi.

## Dosya ve transaction sınırı

Kaynak akış satırlar bağımsız ilerlediği için **satır başına transaction** korunur. Bir satırdaki eşleşme/aktif kayıt okumaları, INSERT/UPDATE, gerçek DB öncesi/sonrası görüntüleri ve merkezi audit aynı SqlTransactionContext içindedir. Satır hatası o satırı geri alır; önceki tamamlanan satırlar korunur. Hata sonrasında sonraki satırlar işlenmeye devam eder.

Eklendi/güncellendi/eşleşmedi sayaçları yalnızca transaction Dispose/commit bittikten sonra artırılır. Update dönüşünü göz ardı edip dosyanın tamamına başarı deme ve hatada yükleme düğmesini kalıcı kapatma kaldırıldı. Tek sonuçta eklenen, güncellenen, tutarı 1'den az olduğu için atlanan, geçersiz ve teknik hata olan satır sayıları bildirilir. Veri satırı numaraları Excel'in fiziksel satır numarası değildir; başlık/atlanan üst-alt satırlardan sonra kalan veri sırasıdır.

Mevcut eşleşme önceliği korunur: pozitif TCKN → ad ve telefon → telefon → e-posta. Boş/sıfır kimlik veya boş iletişimle sorgu yapıp rastgele bağışçı seçilmez. Bir ölçütte birden fazla aday varsa ilkini seçmek yerine elle eşleştirme için eşleşmemiş kayıt bırakılır. Yeni NakitBagisci yaratılmaz; bu dosyanın önceki bağışçı kayıtlarına dayanması davranışı korunur.

Eşleşmiş bağışçıda tek aktif düzenli kayıt güncellenir; birden fazla aktif kayıt varsa satır değişmeden uyarı listesine alınır. Güncellemede mevcut armağan/bağış bağlantıları ve ilk oluşturma bilgileri korunur. Eşleşmeyen satırlar önceki davranış gibi BagisciId=-1 ile ayrı kayıt olarak eklenir; listeden elle eşleştirme gerekir.

**Tekrar yükleme sınırı:** eşleşen satırlar aktif bağışçı kaydını günceller. Eşleşmeyen satırların dosya/satır için kalıcı benzersiz anahtarı yoktur; aynı eşleşmeyen dosyanın tekrar yüklenmesinde yeni eşleşmemiş kayıt oluşabilir. Bu parça kanıtlanmamış bir isim/telefon/tutar birleşimini benzersiz anahtar yapmaz ve yeni migration eklemez. Kısmi yükleme sonrası yalnızca sorunlu satırları düzeltip yeniden yükleyin. Dosyada bulunmayan eski kayıtlar kendiliğinden pasifleştirilmez.

## Excel biçimi

Şablonun ilk 10 sütununun mevcut sırası korunur: açıklama, ad, e-posta, telefon, TCKN, eski kullanılmayan adres sütunu, aylık tutar, bağış adedi, toplam bağış, başlangıç tarihi. Kullanılmayan adresi sayıya çevirmeye çalışan eski satır kaldırıldı; yeni bir adres yazma davranışı eklenmedi.

Yeni ReadXLSXAsTypedDataTable sayısal hücreleri decimal olarak korur. Örneğin Excel'in ham `123.45` değeri Türkçe binlik ayıracı gibi yorumlanıp `12345` olmaz. Metin hücrelerde Türkçe sayı biçimi ve ₺ temizliği uygulanır. Başlık veri satırı olarak sayılmaz; ortak üst/alt satır sınırları korunur. Boş satırlar veya hücreler sütunları kaydırmaz. Yeni yolda inline/shared string hücreleri okunur.

Başlangıç tarihi Türkçe tarih metni veya Excel seri tarihi olabilir; çalışma kitabının 1900/1904 tarih sistemi dikkate alınır. Geçersiz tarih/sayı ve tam sayı olmayan kimlik/adet satırı yazılmadan uyarıya ayrılır. Aylık tutarı 1'den az olan satırın atlanması mevcut iş kuralıdır; yeni bir armağan hak kazanma kuralı eklenmedi.

## Elle eşleştirme

Mevcut NakitBagisciEslestirWP seçiminden listeye iki kimlikle dönüş davranışı korunur; bu parçaya yeni ASCX kontrolü eklenmedi. Dönüşte bağışçı ve düzenli kayıt transaction içinde yeniden okunur. Kayıt aktif olmalı, hedef bağışçı var olmalı, başka aktif düzenli kayıtla çakışmamalıdır. Armağan/bağış bağlantısı olan kayıt başka bağışçıya taşınmaz.

Aynı eşleştirme bağlantısı tekrar açılır veya sayfa yenilenirse kayıt zaten eşleşmişse yazma/audit yapılmaz; Info gösterilir. URL'nin kendisi başarı kanıtı sayılmaz. Eşleştirme değişikliği ve audit birlikte commit olduktan sonra tek başarı mesajı verilir. Liste yenileme başarısızsa eşleştirme sonucunu ve sayfayı yenileme adımını içeren tek bildirim gösterilir.

## Belge/armağan kaydı

Tek transaction'da bağışçı ve düzenli kayıt kilitlenip yeniden okunur. Kimlik uyumu, aktiflik, BelgeIstemiyor ve DuzenliNakitBagisci.GetHakedilenBelgeSirasi kuralı kontrol edilir. Belge yalnızca pozitif 12 bağış ve katlarında üretilir; gönderilemeyen/önce iade edilmiş bağışçıların mevcut durum eşlemesi korunur.

Bağışçı + düzenli belge tanımı + KacinciBelge ile mevcut geçerli armağan aranır. Eski 0/NULL KacinciBelge=1 yorumu ve geçersiz belge filtresi korunur. Mevcut belge yeniden oluşturulmaz; düzenli kaydın armağan ve gerçek bağlı bağış kimliği gerekiyorsa aynı transaction'da düzeltilir. Mevcut belgeye bağlı gerçek bağış bulunamazsa eski/başka belgeye ait hareket kimliği korunmak yerine 0'a alınır. Aynı bağlantılar zaten doğruysa yeni yazma/audit yapılmaz.

Yeni belgede mevcut bağış yılı hesabı korunur: BaslamaTarihi + (belge sırası-1)*12 ay ile bundan bir yıl sonraki tarih arasındaki son bağış alınır; bitiş mevcut kaynak gibi dahildir. Aynı tarihteki hareketlerde Id azalan sırası deterministik seçim sağlar. İlgili yılda bağış yoksa uyarı verilir ve hiçbir yazma yapılmaz.

Armağan INSERT, son bağışın ArmaganId UPDATE'i, düzenli kaydın ArmaganId/NakitBagisHareketId UPDATE'i ve bütün audit aynı transaction'dadır. Bağlantı/audit hatasında armağan dahil tamamı geri alınır. Eski SaveDuzenliBagisIfMissing'in bağımsız transaction'ı bu yeni yolda çağrılmaz. Bağışçı satırı ve armağan aralık kilitleri aynı belgeye eşzamanlı isteklerde çift üretimi önler; hedef ortamda ayrıca test edilmelidir.

**Korunan iş davranışı:** yeni düzenli belgenin bağlandığı son bağışta önceden başka ArmaganId varsa bu alan eski akıştaki gibi yeni belgeye güncellenir; eski armağan kaydı silinmez. Audit eski/yeni bağlantıyı kaydeder. Düzenli belge ile normal armağanın aynı hareket üzerindeki bu davranışı TEST inceleme noktasıdır; bu dal yeni bir çoklu armağan ilişki modeli kurmaz.

## Log, audit ve mesajlar

İlk istekte tek UserActivity, postback'te ilgili olay sınırları kullanılır. İş okumaları ve yazmaları açık PortalLogContext taşır. SQL izlerinde sorgu/parametre değerleri yoktur; repository/metot, işlem, süre ve komut sonucu bulunur. SQL Succeeded commit anlamına gelmez. İş/audit aynı transaction'dadır; bağımsız teknik Error/SqlTrace rollback sonrası kalabilir. Log kapalıyken bu yolda eski OlayKayit/SQL yazıcısına dönülmez.

| Entity | Audit alanları |
| --- | --- |
| DuzenliNakitBagisci | BagisciId, BagisAdedi, BagisToplami, BaslamaTarihi, BitisTarihi, Tutar, Aktif, ArmaganId, NakitBagisHareketId |
| Armagan | Mevcut izin listesi; ayrıca DuzenliBagis. KacinciBelge zaten izin listesindedir. |
| NakitBagisHareket | Mevcut izin listesi; bu akışta değişen bağlantı ArmaganId'dir. |

Ad, TCKN, telefon, e-posta, eşleşme açıklaması, dosya satırının serbest metni audit ayrıntısına alınmaz. Eski/yeni değerler DB görüntülerinden alınır. Henüz taşınmamış parametresiz yolların eski log davranışı korunur.

IKYS/TBYS'nin güvenli hata/başvuru kodu ve aynı hata için tek bildirim yaklaşımıyla uyumlu, önceki NBYS parçalarının Messages/NbysWebPartLog yapısı kullanıldı:

- Yükleme sonunda tek sonuç bildirimi; satır başına popup veya hem hata hem tam başarı mesajı yoktur. Teknik satır hataları tek AggregateException ve başvuru koduyla raporlanır; teknik ayrıntılar kullanıcıya aktarılmaz.
- Dosya seçilmemesi, .xlsx dışı dosya, eksik sütun, bozuk tarih/sayı, eşleşmeyen/çoklu aday ve çakışan aktif kayıtlar teknik hatadan ayrılır. Kullanıcıya dosyayı veya ilgili veri satırlarını kontrol etmesi ve gerekiyorsa eşleştirmesi söylenir.
- Excel paket/biçim istisnasının tanı ayrıntısı FileValidation teknik kaydında tutulur; kullanıcıya dosya şablonunu kontrol etmesini söyleyen Warning gösterilir.
- Eksik/aktif olmayan kayıt, bağışçı uyuşmazlığı, belge istememe, 12'nin katı olmayan adet ve ilgili yılda bağış olmaması ayrı Warning sonuçlarıdır; genel teknik hata yapılmaz.
- Yeni belge yalnızca commit sonrası başarı verir. Mevcut belge veya aynı eşleştirmede Info gösterilir; tekrar başarı iddia edilmez.
- Commit sonrası liste yenileme hatasında tek sonuç+yenileme bildirimi vardır. Yardımcıların hatayı yutup devam etmesi ve aynı exception zincirinin tekrar yayımlanması kaldırıldı. ThreadAbortException yönlendirmesi teknik hata değildir.

## Yapılandırma

Mevcut IKYS/TBYS/NBYS değerlerini koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:DuzenliBagisciDosyaYuklemeWP;NBYS:DuzenliNakitBagisciListesiWP`

NBYS projesinin tamamını EnabledProjects ile açmayın. Yeni migration yoktur; hedefte mevcut merkezi log/audit şeması ve izinleri bulunmalıdır. WSP yayını migration çalıştırmaz. **Utility, DAO, Model ve NBYS paketinin güncel çıktılarını birlikte yayımlayın**; yeni Excel okuyucu Utility'dedir. Üretilen ASCX dosyaları değiştirilmedi, kod üretimi gerekmez.

## Kullanıcının Visual Studio 2026 / TEST kontrolü

1. Solution ve NBYS WSP'yi kullanılan yapılandırmada derleyip TEST'e yayımlayın. İki ekranın ilk açılışında tek UserActivity, postback'te tekrar açılış olmaması, SELECT izleri ve ortak OperationId beklenir.
2. Kullanılan gerçek Excel şablonunda sayısal 123.45, Türkçe metin 123,45 ve 1.234,56; sayısal/metin TCKN/adet; metin ve seri başlangıç tarihi örneklerini deneyin. Sayılar yüz kat büyümemeli. Mümkünse 1904 tarih sistemli çalışma kitabını da kontrol edin. Başlık ve atlanan üst/alt satırlar sayaçlara veri olarak girmemeli.
3. Pozitif TCKN, ad+telefon, telefon ve e-posta ile ayrı eşleşme örnekleri yükleyin. Boş TCKN/iletişim ve birden fazla adayda rastgele kişi seçilmemeli; yeni NakitBagisci oluşmamalı. Eşleşmeyen satır -1 ile ayrı kayıt olmalı ve kullanıcıya eşleştirme sayısı bildirilmeli.
4. Eşleşmiş bağışçıyı tekrar yükleyin. Tek aktif düzenli kayıt güncellenmeli; ilk oluşturma bilgileri ve mevcut armağan/bağış bağlantıları korunmalı. Bir bağışçıda birden fazla aktif düzenli kayıt varsa o satır değişmemeli. Eşleşmeyen dosyanın tekrarında kalıcı benzersiz anahtar bulunmadığı sınırını ayrıca değerlendirin.
5. Aylık tutar <1, bozuk tarih/tutar/adet/kimlik, boş/yanlış şablon ve .xlsx dışı dosya deneyin. Beklenen uyarı, doğru sayaç ve geçersiz satırda sıfır yazma/audit olmalı. Teknik hatayla karıştırılmamalı.
6. İkinci satırın INSERT/UPDATE veya audit'ini TEST'te kontrollü başarısız kılın. O satır geri alınmalı, önceki tamamlanan satır korunmalı; tek bildirimde kısmi sonuç doğru olmalı. Başarısız commit satırı başarıya eklenmemeli.
7. NakitBagisciEslestirWP'den düzenli kayda bağışçı seçerek dönün. Aynı transaction'da bağlantı/audit oluşmalı. Sayfa yenilenince tekrar yazma/audit ve sahte başarı olmamalı. Eksik/aktif olmayan, başka aktif kayıtla çakışan veya belge/bağış bağlantısı olan kaydı başka kişiye taşıma denemesinde uyarı ve sıfır yazma olmalı.
8. 12/24/36 bağış örneklerinde düzenli belge oluşturun. Belge sırası, toplam, durum, yıl aralığı, seçilen son bağış ve iki bağlantı doğru olmalı. Belge istemeyen, aktif olmayan, 12'nin katı olmayan, eşleştirmesi uyuşmayan ve ilgili yılda bağışı olmayan örnekler uyarı vermeli.
9. Aynı belgeye iki kez ve iki oturumdan eşzamanlı basın. İkinci armağan üretilmemeli; mevcut belge bilgisi gösterilmeli. Eski 0/NULL belge sırası ve mevcut belgenin eksik düzenli bağlantısı senaryolarını doğrulayın.
10. Yeni armağan yazıldıktan sonra bağış bağlantısını, düzenli kaydı veya audit'i kontrollü başarısız kılın. Armağan ve iki bağlantı/audit tamamen geri alınmalı; başarı görünmemeli. Teknik Error/SqlTrace kalmalı.
11. Son bağışta mevcut normal armağan bağlantısı bulunan örnekte eski/yeni ArmaganId audit'ini ve eski armağan kaydının korunduğunu inceleyin. Önceden üretilmiş düzenli belgeye gerçek bağlı hareket bulunamıyorsa düzenli bağlantının 0'a alınmasını doğrulayın.
12. Liste ay/yıl/durum/adet filtresi, detay modalı, eşleştirme ve düzenleme bağlantılarını kontrol edin. Commit sonrası liste hatasında tek sonuç+yenileme mesajı beklenir. Log kapalıyken transaction'lar sürmeli; merkezi ve eski çift kayıt oluşmamalı.

Codex build/test/debug çalıştırmadı ve veritabanına bağlanmadı. Kontroller kaynak/transaction incelemesi, repository imzaları, C# sembol/region dengesi, XML/Compile girdileri, audit/model alanları, UTF-8/BOM ve git diff ile sınırlıdır. Excel hesapları, hedef şema, eşzamanlılık ve rollback çalışma zamanı sonucu kullanıcı doğrulamasını bekler. Commit/push/PR/merge kullanıcıya aittir.

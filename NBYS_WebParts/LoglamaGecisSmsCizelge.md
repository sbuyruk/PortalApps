# NBYS aylık SMS çizelgeleri

Dal: `codex/nbys-sms-cizelge-transaction-audit`.
Başlangıç: `f796a175c38076a9e99bc2771f92c819e24c97c1` (23 Ekim 2026).
Değişiklikten önce yerel master temizdi ve uzak master ile aynı commit olduğu doğrulandı. Kullanıcı #520 sonrasında devam edilmesini istedi.

## Kapsam ve mesaj değişiklikleri

| Ekran / işlem | Yeni davranış ve mesaj |
| --- | --- |
| SMSAylikBagisCizelgesi ilk açılış | Yetkili kullanıcı için eksik aylar tek transaction'da tamamlanır. Yeni ay varsa commit ve çizelge yenilemesi sonrasında tek başarı gösterilir. Tam yılın her açılışında tekrar başarı/audit çıkmaz. Yetkisiz kullanıcı kayıt oluşturmaz ve yalnızca görüntüler. |
| Aylık adet düzenleme | Seçim, yıl/ay, adet aralığı, merkez yetkisi ve eşzamanlı değişiklik kontrol edilir. Bozuk/negatif veya Int32 aralığı dışındaki adet açıklayıcı Warning'dir; form tekrar açılır. Commit sonrası tek başarı, aynı değerlerin tekrarında Info gösterilir. |
| SMSAylikCizelgeReadOnlyWP | Açılışta ve yıl/Excel işlemlerinde hiçbir SQL iş kaydı yazılmaz. Eski, salt okunur ekranın gizlice yıl oluşturma yolu kaldırıldı. Eksik aylar Warning ve tablo/Excel içi notla belirtilir; var olmayan kayıtlar gerçek sıfır bağış gibi sunulmaz. |
| Yıl seçimi | Geçersiz/izin verilen listede olmayan yıl Warning verir. Salt okunur ekran yıl değiştirince düzenleme sayfasına yönlenmez; iki ekran da kendi sayfasında kalır ve diğer query parametreleri korunur. |
| Excel | Ekran ile aynı istek içi kayıt görüntüsü kullanılır. İçerik hazırlanırken hata olursa indirme başlatılmadan güvenli hata gösterilir. Sonlandırma ThreadAbortException'ı teknik hata olarak kaydedilmez. |

İki ekran ilk açılışta bir UserActivity üretir; postback tekrar açılış kaydetmez. Okuma/yazma/audit/teknik hata aynı isteğin OperationId'sini taşır. Beklenen giriş/iş kuralı sonuçları teknik Error değildir. Teknik istisna NbysWebPartLog üzerinden tek kez loglanır; kullanıcıya sabit Türkçe güvenli mesaj ve başvuru kodu gösterilir. Ham exception.Message, SQL ve stack trace kullanıcıya verilmez. Yükleme hatasından sonra aynı WebPart'ın sonraki olayları çalıştırılmaz.

## Yetki ve kayıt oluşturma kararı

Çizelge ulusal operatör toplamlarını içerir; SMSAylikBagis kaydında bağışçı/il/bölge sahibi yoktur. Eski kaynakta etkin bir yazma yetkisi kontrolü bulunmuyordu; yorum içindeki grup kontrolü çalışmıyordu. Bu parçada yazma, NBYS'de kullanılan merkez/tüm bölge kimlikleriyle sınırlandı: BOLGE_GENELMUDURLUK_INT ve BOLGE_HEPSI_INT. Kullanıcı kimliği ViewState'den alınmaz, her istekte PortalLogContext.UserLogin kullanılır. Boş kimlik ve bölgesi bulunamayan kullanıcı yazamaz.

İlk UI yetki okuması yalnızca kontrollerin görünümü içindir. Asıl yetki kullanıcı bölgesini iş transaction'ı içinde yeniden okuyarak kontrol edilir. Personel/Birim/Bölge seçiminde mevcut NBYS sıralama ve eşleme yaklaşımı korunur. Yeni SharePoint grubu veya yeni bir rol/yetki tablosu eklenmedi. Mevcut SharePoint sayfa okuma erişimi bu parçada daraltılmadı. TEST'te gerçek merkez, bölge ve bölge kaydı olmayan kullanıcılarla bu erişim değişikliği özellikle değerlendirilmelidir.

Düzenleme ekranının ilk GET/açılışta eksik kayıt oluşturma davranışı yetkili kullanıcı için korunur. Postback/Excel/yıl değişikliği olayının kendisi gizli bir yıl oluşturma çağrısı yapmaz; yıl seçimi aynı sayfada yeni GET açar. Salt okunur ekran bu davranışı tamamen bırakır. Kullanıcı kayıtları yalnızca okuyan ekranı ilk açtığında kayıt yoksa yetkili birime başvurma uyarısı alır.

## Transaction ve audit bütünlüğü

NbysSmsWriteService'in EnsureYear ve UpdateCounts metotlarında yetki SELECT'i, işlem kilidi, yıl kayıtlarının UPDLOCK/HOLDLOCK ile okunması, INSERT/UPDATE, önce/sonra görüntüleri ve audit aynı SqlTransactionContext üzerindedir. İç çağrılar transaction parametresini taşır; new DbClass veya transaction dışı iş SQL'i yoktur. Teknik Error/SqlTrace mevcut bağımsız log yaklaşımındadır; iş rollback'inde de tanılama korunabilir.

Yıl başına `PortalApps.NBYS.SMS.Year.{yıl}` anahtarıyla transaction sahipli sp_getapplock alınır. Oluşturma ve aylık güncelleme aynı kilide katılır. 15 saniyelik kilit bekleme zaman aşımı tek Busy Warning'dir; diğer kilit hataları teknik hata yoluna gider. Aynı yılın iki eşzamanlı açılışı eksik ayları iki defa oluşturmaz. Yeni UNIQUE constraint/migration yoktur; uygulama kilidine katılmayan harici/ad hoc DB yazıcıları için mükerrerlik garantisi verilmez. Mevcut iki WebPart artık bu ortak yoldadır.

EnsureYear, yalnızca tamamen boş yılı değil kısmen oluşturulmuş yılı da tamamlar. Mevcut aylar, tutarlar, adetler ve metadata değiştirilmez. Aynı aya ait birden fazla kayıt veya 1–12 dışı ay varsa otomatik silme/birleştirme yapılmaz; hiçbir yeni ay eklenmeden Warning döner. Eklenen bütün aylar ve her ayın audit'i birlikte commit olur; herhangi bir INSERT/audit hatasında tamamı rollback olur.

Yeni ayların SMSTutari değeri mevcut ProjeConstants SMS tutarlarından hesaplanır. Tarih sınırı `>` yerine `>=` olduğu için yeni Ocak 2023 ve Ocak 2024 kayıtları ilgili yıl sabitini kullanır. Önceki yıllar için mevcut SMS_TUTAR_2022 fallback'i korunur. Bu bir güncel tarife araştırması veya yeni tarife tanımı değildir; mevcut sabitler kullanılır. Var olan Ocak kayıtlarının tutarları geriye dönük değiştirilmez. Bu sınır düzeltmesi TEST ve iş kuralı incelemesinin ayrı bir noktasıdır.

UpdateCounts, düzenleme popup'ı açılırken sunucudan okunan Id/yıl/ay/adet/tutar/para birimi ve zaman metadata'sının sınırlı kopyasını ViewState'de saklar. Hidden Id tek başına güvenilmez; onaylanan kayda ve seçilen yıla uymalıdır. Kayıt transaction'da tekrar okunur. Kayıt silinmiş/taşınmışsa NotFound/Conflict; adet, tutar veya izlenen metadata başka oturumda değişmişse Conflict Warning döner. İstenen adetler DB'de zaten aynıysa yeni UPDATE/audit yapılmadan Unchanged döner. Yalnızca üç operatör adedi ve Degistiren/DegistirmeTarihi yazılır; yıl/ay/SMSTutari/DovizCinsi/açıklama/ilk oluşturma bilgileri eski form nesnesiyle ezilmez.

Completed ve oluşturulan ay sayısı ancak Complete işareti ve Dispose/commit başarıyla tamamlandıktan sonra UI'ya döner. Commit sonrası SELECT/çizelge yenilemesi başarısız olursa kayıt sonucunu açıkça belirten tek güvenli hata/başvuru kodu gösterilir; arkasından ayrıca başarı çıkmaz.

Audit entity'si SMSAylikBagis, EntityId gerçek kayıt Id'sidir. Güvenli ayrıntı listesi Yil, Ay, TurkcellSMSAdedi, VodafoneSMSAdedi, TurkTelekomSMSAdedi, SMSTutari, DovizCinsi ve IslemTarihi'dir. Serbest Aciklama ve kullanıcı kimliği alan değişikliği ayrıntısına yazılmaz; işlemin aktörü log başlığındadır. Legacy OlayKayit/SQLLog'a ikinci iş kaydı gönderilmez. SMSAylikBagisRepository'nin bağlamlı transactionsız yazmaları reddedilir; eski parametresiz/enjekte çağrılar önceki davranışlarını korur.

## Çizelge, popup ve Excel

NbysSmsTable, iki ekran ve Excel için ortak satır üretir. Dinamik satırlar yeniden eklenmeden önce önceki veri satırları kaldırılır; iki statik başlık satırı korunur. Dinamik satır ViewState'i kapalıdır ve Düzenle düğmeleri kayıt Id'sinden kararlı kimlikle her istekte yeniden oluşturulur. Dinamik Click olayı da merkezi hata sınırındadır. Güncelleme sonrası kayıt görüntüsü temizlenir ve yeni veri okunur; eski popup onayı sonraki kayıtta kullanılmaz.

Operatör adetleri DB'deki Int32 alan aralığında kalır; UI girişleri 0–2.147.483.647 arasıdır. Aylık/yıllık toplamlar long, parasal hesaplar decimal'dir. Böylece geçerli üç Int32 adedi toplanırken eski int taşması oluşmaz. Tutar hesabı her kaydın mevcut SMSTutari değeriyle yapılır. Düzenleme ekranının yıl toplamı ve salt okunur ekranın yalnızca aylık gösterimi korunur. Eksik aylar tablo ve indirilen Excel içinde adlarıyla açıklanır; toplamlar mevcut kayıtları kapsar.

Excel için düzenleme düğmeleri üretilmez. Aynı istek içinde ekran ve Excel aynı GetByYear listesini kullanır. PostbackControl kaydı Excel indirmesini UpdatePanel'in async yanıtı dışında tutar. Dosya mevcut HTML-as-XLS biçimini ve Türkçe windows-1254 kodlamasını korur; gerçek XLSX üretimine geçilmedi. İçerik/başlıklar hazırlandıktan sonra Response.Clear ve indirme başlatılır; normal Response.End üst hata yardımcısında teknik hata sayılmaz. Yeni kontrol veya ASCX değişikliği yoktur; üretilen ASCX dosyalarına dokunulmadı. OpenXML paketleri değiştirilmedi.

## Yapılandırma

Mevcut değerleri koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:SMSAylikBagisCizelgesi;NBYS:SMSAylikCizelgeReadOnlyWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Önceki merkezi log tabloları/bağlantı ayarları gereklidir. Yeni şema/migration yoktur. SQL kullanıcısının mevcut FTK akışındaki gibi sp_getapplock kullanabilmesi gerekir. Uygulama kilidi mevcut iş bağlantısı ve transaction üzerinden alınır. WSP yayını log ayarlarını veya kullanıcı/bölge eşlemelerini oluşturmaz.

## Kullanıcının Visual Studio 2026 / TEST doğrulaması

1. İki ekranın ilk açılışında tek UserActivity, postback'te tekrar açılış olmaması, doğru NBYS/WebPart ve ortak OperationId. Yıl/yetki/kayıt SELECT'lerinde merkezi SqlTrace; eski SQLLog/OlayKayit'te çift kayıt olmamalı.
2. Merkez kullanıcısıyla boş bir yılı açın: 12 ay ve 12 INSERT audit birlikte oluşmalı. Sayfayı tekrar açın: yeni kayıt/audit/başarı mesajı oluşmamalı. 3–5 ayı eksik test yılında yalnızca eksikler tamamlanmalı, mevcut tutarlar/adetler/metadata korunmalı.
3. Eşzamanlı iki yetkili oturumdan aynı boş/eksik yılı açın. Her ay bir defa oluşmalı. Kilit bekleme zaman aşımı tek Warning olmalı. İkinci/son ay INSERT'ini veya audit yazmasını kontrollü başarısız kılın: yılın o işlemde eklenen bütün ayları ve audit'leri rollback olmalı; başarı çıkmamalı.
4. Salt okunur ekranı boş/eksik yıl ile açın, yılı değiştirin ve Excel indirin. Hiçbir SMS iş kaydı/audit oluşmamalı; eksik ay notu görünmeli. Yıl değişiminde aynı salt okunur sayfada kalınmalı. Düzenleme ekranında bölge/yetkisiz/bölgesi bulunmayan kullanıcı otomatik kayıt oluşturamamalı ve güncelleyememeli; görüntüleme devam etmeli.
5. Normal ayı düzenleyin: üç adet, tutarlar ve yıl toplamı doğru yenilenmeli; tek UPDATE audit ve tek başarı beklenir. SMSTutari, açıklama, yıl/ay ve ilk oluşturma metadata'sı korunmalı. Aynı adetleri tekrar gönderin: Info, sıfır yeni UPDATE/audit.
6. Boş, negatif, ondalık, harfli, Int32 üst sınırını aşan adet; değiştirilmiş hidden Id; başka yılın kaydı; popup açıkken silinmiş kayıt. Anlamlı tek Warning, sıfır yazma beklenir. İki oturum aynı ayı açsın; biri kaydettikten sonra diğeri farklı değerle kaydetsin: Conflict ve ilk kayıt korunmalı. Yetkiyi popup açıkken kaldırın: transaction içindeki kontrol yazmayı reddetmeli.
7. Üç operatör adedini yüksek ama Int32 sınırı içinde değerlerle deneyin; aylık/yıllık toplamlar negatifleşmemeli. Excel ile ekranın adet/tutarları aynı olmalı. Tekrarlanan postback/Excel isteğinde başlık/veri/toplam satırları çoğalmamalı. Düzenle popup'ı, geçersiz adet sonrası tekrar açılması ve başarılı güncellemeden sonra kapanması kontrol edilmeli.
8. Mevcut Ocak 2023/2024 kayıtlarının tutarlarının değişmediğini; yeni oluşturulan bu aylarda ilgili ProjeConstants tutarının kullanıldığını doğrulayın. Önceki yıl fallback'i ve sonraki yılların mevcut sabit kullanımını iş kuralı olarak değerlendirin.
9. Aynı yıl/ay mükerrer kayıt ve geçersiz ay içeren test verisinde işlem durmalı, otomatik silme/birleştirme olmamalı. Bozuk/bilinmeyen yıl query string'i SQL yazmasına ulaşmamalı. Salt okunur ekranın mevcut önceki yıl/bu yıl filtresi korunmalı.
10. Commit sonrası yenileme hatası ve kontrollü SQL/audit hata senaryosunda tek Türkçe güvenli mesaj/başvuru kodu, doğru kayıt sonucu ve ortak OperationId'li teknik Error beklenir. Excel'in normal sonlandırılması Error üretmemeli.

Codex build/test/debug çalıştırmadı; veritabanına iş isteği yapmadı ve Excel çıktısı üretmedi. Kontroller kaynak/metot çağrıları, C# sözcüksel parantez/blok dengesi, proje XML/Compile kayıtları, olay bağlantıları, salt okuma sınırı, UTF-8/BOM ve diff ile sınırlıdır. Runtime, yetki, mükerrerlik/rollback ve Excel sonucu kullanıcı TEST doğrulamasını bekler.

Kaynak seviyesinde 55 derlenen code-behind'ın 55'inde açık istek bağlamı vardır. Bu sayı NBYS'nin test veya uçtan uca geçişinin tamamlandığı anlamına gelmez. Ortak yardımcılar, eski erişim/log yolları, derlenmeyen kopyalar ve uçtan uca kapanış taraması sonraki kapsamdır; Portal/MTS/MFYS'ye geçilmedi.

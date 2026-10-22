# NBYS bağışa vesile olanlara teşekkür işlemleri

Dal: `codex/nbys-bagisa-vesile-transaction-belge`.
Başlangıç: `ad6f89609a31fec40d6fa640ecf64d49159981b0` (22 Ekim 2026).
Değişiklik öncesinde yerel master temizdi ve uzak master ile aynı commit olduğu doğrulandı. Kullanıcı belge/basım parçası #517 sonrasında devam edilmesini istedi. Bu parça için çalışma zamanı testi yapılmış olduğu varsayılmadı.

## Kapsam

- BagisaVesileOlanTesekkurGirisiWP: yeni kayıt, düzenleme, kayıtlı bilgiden Word belgesi ve listeye dönüş.
- BagisaVesileOlanTesekkurListesiWP: liste, düzenleme bağlantısı ve silme.
- BagisaVesileOlanArmaganBelgesiWP: formu kaydetme ve belge oluşturma birleşik işlemi, liste ve hatadan sonra tekrar deneme.
- BagisaVesileOlanTesekkurService/Repository: açık NBYS bağlamlı okumalar, yeni Persist/Remove transaction yolları ve mevcut legacy çağrıların korunması.
- NbysReferralDocument: iki ekranın ortak Word/SharePoint dosya üretimi. NbysReferralFeedback: tek işlem sonucu, kayıt sonrası yenileme ve dosya kısmi sonuçları.

Kökteki `NBYS_WebParts/BagisaVesileOlanTesekkurGirisiWP.ascx.cs` kopyası projenin Compile listesinde değildir; değiştirilmedi. Klasör içindeki derlenen kaynak esas alındı. Üretilen `.ascx.g.cs` dosyaları değiştirilmedi, yeni kontrol eklenmedi. OpenXML sürümü, diğer projeler, FTK/SMS ve kalan raporlar bu kapsamda değildir.

## Transaction ve log

Bağlamlı repository kurucusu DbClass oluşturmaz. İş okumaları NbysSqlExecutor üzerinden yürür; SQL izine yalnızca işlem türü, repository/metot, süre ve sonuç girer. İşlem kimliği aynı HTTP isteğinde ortak PortalLogContext.OperationId'dir. Her ekran ilk açılışta tek UserActivity üretir; postback aynı açılışı yeniden kaydetmez.

Persist mevcut kaydı UPDLOCK/HOLDLOCK ile aynı transaction içinde yeniden okur. INSERT/UPDATE, DB öncesi/sonrası görüntüleri ve audit aynı SqlTransactionContext içindedir. Remove kilitli kayıt okuması, DELETE ve audit'i aynı transaction'da yapar. Complete yalnızca işarettir; başarılı sonuç ve yeni Id yalnızca Dispose/commit tamamlandıktan sonra UI'ya aktarılır. Audit hatasında iş kaydı da geri alınır. Legacy OlayKayit/SQLLog'a ikinci kayıt gönderilmez. Bağlamlı repository'nin transactionsız yazma metotları reddedilir; parametresiz/enjekte eski çağıranlar önceki yollarını korur.

Audit entity adı `BagisaVesileOlanTesekkur`, EntityId gerçek kayıt numarasıdır. Güvenli değişiklik listesi yalnızca `BelgeTarihi` alanıdır. Ad, soyad, unvan, TCKN, telefon, adres, imzalayan bilgileri, serbest açıklama, verilme sebebi ve belge metinleri audit ayrıntısına alınmaz. Yalnızca bu metinlerin değiştiği güncellemede audit üst kaydı oluşur; ayrıntıda kişisel veri bulunmaz. Kayıt tarihi için en az 1753 kabul edilir ve CrudQueryBuilder'da BelgeTarihi korunarak 1901 öncesi geçerli tarihin yanlışlıkla NULL yazılması önlenir.

Güncelleme oluşturma zamanını ve oluşturan kişiyi korur; değiştiren/zaman güncellenir. Formlarda bulunmayan Telefon/Adres mevcut kilitli kayıttan alınır; boş form değerleriyle silinmez. Aynı form değerleriyle kaydetmede UPDATE/audit yapılmaz; Info gösterilir. Eksik/silinmiş kayıt başarısız UPDATE veya genel exception gibi sunulmaz.

Bu entity'de bağışçı/bölge bağlantısı yoktur. Kaynaktan kanıtlanamayan bölge/merkez kısıtı veya yeni rol adı eklenmedi; mevcut SharePoint ekran/kitaplık erişimi korunur. Uygulamaya genel yetkilendirme tasarımı bu parçanın dışında kalır. Yeni bir mükerrerlik anahtarı veya TCKN benzersizliği oluşturulmadı; aynı kişi için farklı teşekkür kayıtları engellenmez.

## Form ve mesaj farkları

IKYS BirimTanimEditWP ve TBYS TbysWebPartLog kaynaklarındaki güvenli Türkçe mesaj, teknik log, başvuru kodu ve tek hata zinciri yaklaşımı yeniden kontrol edildi. Bu üç ekran ortak NbysWebPartLog/Utility.Messages yolunu kullanır. İç yardımcılar exception yayınlayıp false/null ile başarıya devam etmez.

- İsim boşsa anlamlı Warning. Türkçe yazıyla tarih veya `dd.MM.yyyy` gibi desteklenen tarih biçimleri doğrulanır; yanlış tarih genel teknik hata olmaz.
- TCKN alanı boş bırakılabilir; doluysa negatif olmayan, long sınırları içinde rakamsal değer beklenir. Yeni 11 hane/checksum veya zorunlu kimlik kuralı eklenmedi.
- Yeni kayıt/güncelleme/silme: tek Success, yalnızca commit ve ekran yenileme tamamlandıktan sonra. Ekran yenilemesi sonradan hata verirse tamamlanan işlemi ve sayfayı yenileme adımını bildiren tek hata mesajı/başvuru kodu verilir.
- Yeni kayıt girişinde commit sonrası redirect yerine aynı form güncel Id ile düzenleme durumuna geçer. Oluşturma/güncelleme/belge düğmeleri kayıt durumuna göre ayarlanır. Listeye dönüş gerçek Id'yi taşır. Geçersiz Id yeni kayıt olarak sessizce yorumlanmaz.
- Belge düğmesi, giriş ekranında veritabanına kaydedilmiş bilgileri kullanır; kaydedilmemiş form düzenlemelerini belgeye aktarmak için önce Güncelle gerekir. Başarı metni kayıtlı bilgilerin kullanıldığını belirtir.
- Birleşik ekrandaki Kaydet ve Teşekkür Belgesini Oluştur ilk kez INSERT yapar; commit edilen Id ViewState'te dosya adımından önce tutulur. Aynı formdan dosya hatası sonrası yeniden denemede mevcut kayıt güncellenir/kullanılır; sırf tekrar dosya denendi diye yeni kayıt eklenmez. Kayıt başka oturumda silinmişse yeni INSERT'e düşülmez, bulunamadı uyarısı verilir.
- Kayıt başarılı/Word veya SharePoint başarısız: tek mesaj kayıt numarasıyla kaydın korunduğunu ve aynı kayıt için tekrar belge üretme adımını söyler. Ayrı kayıt başarı popup'ı ve ikinci genel hata popup'ı yoktur.
- Dosya kaydedilmiş/bağlantı veya liste yenileme hatalı: dosyanın kitaplıkta olduğu, bağlantı/kitaplığın kontrol edilmesi gerektiği tek mesajla bildirilir. SQL kaydı geri alınmış gibi sunulmaz.
- Silmede geçersiz seçim veya kaybolmuş kayıt Warning; aynı hata için ikinci silinemedi popup'ı yoktur. Son kayıt silinince boş DataTable yeniden çizilir; eski satırlar ekranda kalmaz.
- Liste tarihini biçimlendirmek için DataTable'ın DateTime kolonu string'e çevrilmez. Satır vurgulama Id'si JavaScript'e yalnızca doğrulanmış tam sayı olarak gider; metin kolonları metin renderer'ı kullanır.
- Bağlamlı listede `SELECT *` kaldırıldı; ekranda gerekmeyen TCKN, telefon, adres ve belge metinleri liste JSON'una gönderilmez. Düzenleme için tam kayıt okuması ayrı kalır.

## Word ve dış etki sınırı

İki ekran aynı `TemplateBagisaVesile.docx` şablonunu ortak yardımcı üzerinden kullanır. Şablon eksik/boş veya Word belge bölümü yoksa işlem durur. Şablon anahtarları tek geçişte, XML metni olarak kaçırılarak değiştirilir; `&`, `<`, `$1` veya bir başka anahtarın adını içeren kullanıcı metni XML/regex komutu hâline gelmez. XML doğrulanır ve paket/stream kapatıldıktan sonra upload yapılır. Belge tarihleri Türkçedir. Şablonda anahtarın Word run'ları arasında bölünmesini birleştiren yeni bir algoritma eklenmedi; mevcut şablon çıktısı TEST'te incelenmelidir.

Dosya adı kayıt numarası, milisaniye ve rastgele ek içerir. Aynı dakika üretilen dosya eskisinin üzerine yazmaz (overwrite=false); önceki belgeler korunur. İndirme bağlantısında SourceUrl kodlanır. Dosya üretimi kayıt üzerinde Gönderildi gibi yeni bir alan/durum oluşturmaz; fiziksel baskı veya teslimat başarısı iddia edilmez.

SQL transaction ile SharePoint upload atomik değildir. SQL başarılı/SharePoint başarısız durumunda kayıt korunur. Silme yalnızca SQL teşekkür kaydını ve audit'i kapsar; daha önce üretilmiş SharePoint dosyaları otomatik silinmez. Dosya silme/saklama politikası eklenmedi.

ViewState'te tutulan Id, aynı formun dosya hatası sonrasındaki normal yeniden denemesini korur. Yanıt kaybolması, aynı ilk POST'un paralel tekrarı, yeni sekme veya yeni formdan tekrar kayıt için kalıcı idempotency anahtarı yoktur; tam mükerrer kayıt engellemesi iddia edilmez. Sonucu belirsiz ilk kayıt isteğinde yeni kayıt açmadan önce listeyi kontrol edin. Genel iyimser eşzamanlılık sürüm kontrolü eklenmedi; farklı oturumlarda form alanları için son kaydetme geçerlidir.

## Yapılandırma

Önceki değerleri koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:BagisaVesileOlanTesekkurGirisiWP;NBYS:BagisaVesileOlanTesekkurListesiWP;NBYS:BagisaVesileOlanArmaganBelgesiWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Önceki merkezi log/audit migration ve bağlantı ayarları gereklidir; yeni migration yoktur. SharePoint kitaplığında şablon okuma/dosya oluşturma izinleri gereklidir. WSP yayını şema veya izinleri kendiliğinden oluşturmaz.

## TEST ve inceleme noktaları

1. Üç ekranı açın. İlk açılışta tek UserActivity, doğru WebPart ve ortak istek OperationId beklenir. Liste/tek kayıt SELECT'lerinde yeni SqlTrace; eski SQLLog/OlayKayit'te çift kayıt olmamalı.
2. Giriş ekranında yeni teşekkür kaydı oluşturun, düzenleyin ve listeye dönün. Kayıt numarası doğru kalmalı, oluşturma bilgileri güncellenmemeli, tek commit sonrası başarı görülmeli. Telefon/Adres daha önce dolu olan kayıtta bunlar korunmalı.
3. Aynı bilgileri yeniden kaydedin: tek Info, yeni UPDATE/audit yok. Yalnızca ad/metin değişikliğinde audit başlığı olmalı; değişiklik detayında PII/metin olmamalı. BelgeTarihi değişiminde gerçek DB eski/yeni değerleri görünmeli. Türkçe tarih ve 1753-1900 arası tarih için NULL yazılmadığını kontrol edin.
4. Boş ad, yanlış tarih, sayısal olmayan/taşan kimlik, geçersiz Id ve başka oturumda silinen kayıt için anlamlı tek uyarı ve sıfır yazma/audit beklenir. TCKN boş bırakılabilmeli. Kontrollü SQL/audit hatasında iş verisi ile audit birlikte geri alınmalı; başarı olmamalı.
5. Liste son kaydını silin: boş tablo gösterilmeli, eski satır görünmemeli. Silinen Id ile tekrar istekte bulunamadı uyarısı olmalı. SQL DELETE sonrası SharePoint'teki eski belgeler korunmalı; silme testi bu dosyaların silinmesini beklememeli.
6. Her iki ekrandan Word üretin. Word açılışı, kayıt no, isim, Türkçe tarih, imzalayan ve paragrafların yerleşimini kontrol edin. `&`, `$1`, tırnak ve bir şablon anahtarının adını içeren örnek metinleri deneyin. Başarı yalnızca dosya kaydedilip bağlantı hazırlandıktan sonra çıkmalı. Aynı dakika yeniden üretim eski dosyayı ezmemeli.
7. Birleşik ekranda ilk kayıt sonrası şablon/SharePoint hatası oluşturun; tek mesaj kaydın korunduğunu ve numarasını söylemeli. Hata giderilip aynı formdan tekrar denendiğinde aynı kayıt Id'si kullanılmalı, satır sayısı artmamalı. Aynı formda alan değiştirip tekrar üretmek aynı kaydı güncellemeli. Yeni kişi için yeni form açılmalı.
8. Giriş ekranında alanı değiştirip kaydetmeden belge üretince kayıtlı bilgilerin kullanıldığını; Güncelle sonrası yeni bilgilerin belgeye geçtiğini kontrol edin. Dosya kayıtlıyken liste/bağlantı hazırlama hatasında dosyanın kitaplıkta kaldığı tek mesajla anlaşılmalı.
9. Liste boşken birleşik ekran ilk kaydı oluşturabilmeli. Listede HTML gibi görünen metinler kod olarak çalışmamalı; gereksiz kimlik/telefon/adres/belge metinleri JSON'da bulunmamalı. Ham SQL/exception/stack kullanıcıya ulaşmamalı.

Codex Windows üzerinde build, test veya debug çalıştırmadı; DB/SharePoint'e iş isteği göndermedi ve gerçek Word belgesi üretmedi. Kaynak imzaları, transaction çağrıları, Compile kayıtları, markup/kontrol bağlantıları, XML, UTF-8/BOM ve diff statik incelendi. Çalışma zamanı, Word görsel çıktısı ve rollback senaryoları kullanıcı tarafından Visual Studio 2026/Test sunucusunda doğrulanmalıdır.

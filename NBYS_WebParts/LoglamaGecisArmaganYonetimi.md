# NBYS armağan yönetimi

Dal: `codex/nbys-armagan-yonetimi-transaction-audit`.
Başlangıç: `0fd5be3d1843b180ad84b99aa3e70ed9385a2021` (22 Ekim 2026).
Değişiklik öncesinde master çalışma ağacı temizdi; uzak master ile aynı commit olduğu doğrulandı. Kullanıcı OpenXML yükseltmesini erteledi ve armağan yönetimine devam edilmesini istedi. Bu parça OpenXML referanslarını değiştirmez. Başlangıç master'ında #514 ile alias düzeltmesi vardır; referans kaldırma dalı henüz bu tabana dahil değildir.

PR #515 birleştirildikten sonra temiz yerel master uzak master ile `c191a1a9b7e3d59c218cec1d99481378db2c6839` commit'inde eşitlendi. Armağan dalı bu tabana fast-forward edildi ve korunan 14 dosya çatışmasız geri yüklendi. Framework 3.0.2 referansının kaldırılması artık tabanda bulunur; armağan değişikliklerine dahil değildir.

## Kapsam

- ArmaganEditWP: mevcut armağanın düzenlenmesi, form doğrulaması ve bölge yetkisi.
- ArmaganListesiWP: liste/filtreler, bağışçı detayı, belge iade durumu ve rapor bağlantılarının hata sınırları.
- ArmaganAraWP: armağan numarasıyla arama, bölge filtresi ve bağışçı detayı.
- NbysGiftWriteService/Repository: iki yazma işleminin transaction ve audit sınırı. NbysGiftFeedback: tek sonuç mesajı ve commit sonrası ekran yenileme bildirimi.
- ArmaganService/Repository: bölge filtreli arama overload'u; eski iki parametreli arama çağıranları korunur.

ArmaganEditWP kaynak kodundaki ekleme dalı çalışabilir bir akış değildi: önce var olan kayıt aranıyor, bulunamayınca çıkılıyor, sonra sıfır kimlikle ekleme deneniyordu. Bu parça bu ölü yolu kaldırır; ekranda yeni armağan ekleme açmaz. Armağan oluşturma, daha önce taşınan bağış/düzenli bağış süreçlerinde kalır.

Belge dosyası üretimi, baskı, etiket/rapor hedef ekranları, SMS/e-posta gönderimi ve diğer WebPart'ların iş kuralları bu kapsamda değildir. Listeden rapor açma bağlantıları korunur; rapor açılması basım/gönderim başarısı olarak bildirilmez. Ortak UtilityHelper yapılandırma/URL yardımcıları değiştirilmez; ekranların doğrudan iş verisi servis çağrıları açık NBYS bağlamıyla çalışır.

## Transaction ve yetki

Kaydetme ve belge iadesi sırasında önce sahip bağışçı kimliği aynı transaction'da okunur, bağışçı ardından armağan UPDLOCK/HOLDLOCK ile kilitlenir. Sahiplik yeniden kontrol edilir; değişmiş bağlantıda işlem yapılmaz. Kullanıcının güncel bölgesi ve bağışçının il/bölge ilişkisi aynı transaction'dan okunur. Genel müdürlük/hepsi bölgesi mevcut merkezi servis yaklaşımına göre tüm bölgelere erişebilir; diğer kullanıcılar yalnızca kendi bölgesinde işlem yapar. Kullanıcı/bölge bilgisi yoksa erişim açılmaz.

UPDATE ve gerçek veritabanı öncesi/sonrası görüntülerinden üretilen merkezi audit aynı SqlTransactionContext içindedir. İş yazması veya audit başarısızsa ikisi birlikte geri alınır. Başarı sonucu yalnızca Complete ve Dispose/commit sonrasında döner. Eski OlayKayit/SQLLog yoluna ikinci iş kaydı yazılmaz. Form, tüm entity'yi geri yazmaz: yalnızca armağan türü, tutar, durum, tarih, açıklama, belgede yazan isim ve tutarı gizleme alanları ile değiştiren/tarih güncellenir. Bağışçı, bağış bağlantıları, düzenli/çoklu bağış, belge sırası, döviz ve iade/geçersizlik alanları korunur.

Audit alanları mevcut güvenli listeden BagisciId, ArmaganTanimId, Tarih, Durum, BagisMiktari, DovizCinsi, CokluBagis, KacinciBelge, DuzenliBagis ve eklenen BagisMiktariYazmasin alanıdır. Açıklama ve belgede yazan isim audit değişiklik detayına alınmaz; yalnızca bu alanların değiştiği işlemde audit üst kaydı bulunur. SQL trace sorgu metnini/parametre değerlerini kaydetmez. Kullanıcı mesajlarına ham teknik hata alınmaz.

Sayfa açılışında, aramada ve modal detay isteğinde sunucu tarafında bölge kontrolü vardır. BolgeId query string'den alınmaz; her istekte güncel kullanıcı bölgesi çözülür. Bölgeler arasında sonradan aktarılan bağışçıya ait eski ekranın kaydetmesi de transaction kontrolünden geçer. Sayfa yüklemesi yetki/doğrulama nedeniyle durduysa aynı istekte sonraki olay işlenmez. Teknik yükleme hatalarında mevcut NbysWebPartLog hata işareti sonraki olayları engeller.

## Durum ve mesaj değişiklikleri

IKYS BirimTanimEditWP ile TBYS TbysWebPartLog kaynaklarındaki güvenli mesaj, teknik log, başvuru kodu ve hata zincirini tek bildirme yaklaşımı incelendi. Bu üç NBYS ekranı ortak NbysWebPartLog/Utility.Messages yolunu kullanır. İlk açılış UserActivity, iş okumaları SqlTrace, yazmalar Audit, beklenmeyen hatalar Error olarak aynı istek OperationId'siyle ilişkilendirilir.

- Kaydet: eksik/geçersiz kimlik veya tür, negatif/geçersiz tutar ve geçersiz tarih için anlaşılır Warning; kayıt değişmez. Türkçe sayı ve dd.MM.yyyy tarih biçimi kullanılır. Seçim listeleri dışında kalan yeni durum reddedilir; kayıtta zaten bulunan eski durum, diğer alanlar düzenlenirken korunabilir.
- Para iadesi: bu ekran yeni para iadesi oluşturamaz. Para iadesi yapılmış armağanın durumu diğer alanlar düzenlense de korunur; işlem sırasında eşzamanlı para iadesi yapılmışsa da veritabanındaki durum esas alınır. Kullanıcı Bağış İadesi ekranına yönlendirilir.
- Belge iadesi: mevcut arayüzde yalnızca Gönderildi durumunda görünen düğmenin kuralı sunucuda da doğrulanır. Durum Belgesi İade Edildi olur; nakit para iadesi veya bağış hareketi yaratılmaz. Para iadesi yapılmış armağan bu işlemle değiştirilemez.
- Tekrar istek: aynı belge iadesinde veya zaten aynı bilgileri taşıyan kaydı kaydetmede yeni UPDATE/audit yok; tek Info verilir.
- Başarılı yazma: tek Success, yalnızca commit sonrasında ve ekran yenileme tamamlanınca gösterilir. Commit sonrası yenileme hatasında ayrı başarı popup'ı eklenmez; işlemin tamamlandığını, ekranın yenilenemediğini ve sayfayı yenileme adımını içeren tek mesaj ve başvuru kodu verilir.
- Eksik kayıt/yetki: tek Warning; eksik kimlik ardından ikinci genel kaydedilemedi mesajı üretilmez. Bölge kaydı eksikliği null exception veya tüm bölgelere erişim olarak ele alınmaz.
- Arama: pozitif armağan numarası beklenir; geçersiz giriş Warning, yetki alanında sonuç yoksa Info verilir. TextChanged ve arama düğmesi aynı istekte tetiklenirse arama/mesaj bir kez çalışır. Sonuç ve modal bölgeyle sınırlandırılır.
- Liste: geçersiz URL filtreleri mevcut seçimlere/varsayılanlara normalize edilir; sessiz catch kaldırılır. Seçilen satır kimliği JavaScript'e yalnızca doğrulanmış tam sayı olarak gider. Durum filtresi dönüş bağlantısında URL kodlanır.
- Beklenmeyen hata: ayrıntı teknik loga, kullanıcıya güvenli Türkçe mesaj ve başvuru kodu. Normal yönlendirme teknik hata sayılmaz.

## Yapılandırma

`PortalLogging.EnabledWebParts` listesine önceki değerleri koruyarak ekleyin:

`NBYS:ArmaganEditWP;NBYS:ArmaganListesiWP;NBYS:ArmaganAraWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Merkezi log/audit tabloları için önceki migration ve mevcut bağlantı ayarları gereklidir; bu parçada migration veya sunucu ayarı değiştirilmedi. WSP yayını veritabanı migration'ını kendiliğinden çalıştırmaz.

## Test ve inceleme noktaları

1. Üç ekranın ilk açılışını, liste ay/yıl/il/tür/durum filtrelerini, armağan numarasıyla aramayı ve modal bağışçı/bağış detaylarını kontrol edin. Aynı istek loglarında ortak OperationId ve doğru WebPart olmalı; eski SQL loga ikinci kayıt gitmemeli.
2. Kendi bölgenizde bir armağanın düzenlenebilir alanlarını güncelleyin. Sayfayı yeniden açınca değerler korunmalı, tek başarı mesajı çıkmalı; audit'in güvenli alanlarında gerçek eski/yeni değerler bulunmalı. Belgedeki isim/açıklama audit değişiklik detayında bulunmamalı. Bağış/düzenli bağış bağlantıları ve iade alanları değişmemeli.
3. Aynı bilgileri tekrar kaydedin: tek bilgi mesajı, yeni UPDATE/audit yok. Geçersiz tutar/tarih, boş/geçersiz kimlik, silinmiş/geçersiz armağan ve eksik tanımda anlamlı tek mesaj ve iş verisinde değişiklik olmamalı.
4. Gönderildi durumundaki armağanda Belge İadesi yapın: durum Belgesi İade Edildi olmalı; tek audit/success, para iadesi veya bağış yazması olmamalı. Tekrar istekte değişiklik yapılmamalı. Gönderilmedi gibi başka bir duruma geçmiş kaydın eski sayfasından istekte tek iş kuralı uyarısı olmalı.
5. Para iadesi yapılmış kaydın durumunu form veya eski belge iade isteğiyle değiştirmeyi deneyin. Para iadesi durumu korunmalı. Düzenleme sayfası açıkken başka oturumdan para iadesi yapılıp kaydetme denenmesini de kontrol edin.
6. Bölge kullanıcısıyla başka bölgenin armağan URL'sini ve modal/yazma kimliğini deneyin: görüntüleme/yazma engellenmeli; aramada başka bölgenin bağışçı verisi dönmemeli. Bölgesi bulunmayan kullanıcı tüm kayıtları görememeli. Genel müdürlük kullanıcısının mevcut kapsamı korunmalı.
7. Test ortamında kontrollü iş yazması/audit hatasında hiçbir kısmi değişiklik ve başarı mesajı olmamalı. Commit başarılı, sonraki okuma başarısız olduğunda tek mesaj işlemin tamamlandığını söylemeli. Ham SQL/exception/stack trace hiçbir kullanıcı mesajında görünmemeli.
8. Arama metni değişikliğiyle düğmenin aynı postback'te tetiklenmesini, geçersiz filtre URL'lerini, düzenlemeden listeye dönüşü ve rapor/etiket bağlantılarının mevcut davranışını kontrol edin. Hedef raporların kendi basım/gönderim akışları bu parçanın testi değildir.

Codex Windows üzerinde build, test veya debug çalıştırmadı. Kaynak/çağrı imzaları, transaction çağrı zinciri, proje Compile kayıtları, XML, UTF-8/BOM, generated dosya kapsamı ve diff statik olarak incelendi. Çalışma zamanı ve gerçek veritabanı doğrulaması Visual Studio 2026/Test sunucusunda kullanıcı tarafından yapılmalıdır. Tam bir iyimser eşzamanlılık sürüm kontrolü eklenmedi; transaction sahiplik/yetki/iade durumunu yeniden doğrular, ancak diğer form alanlarında son kaydetme geçerlidir.

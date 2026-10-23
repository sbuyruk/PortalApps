# MTS üçüncü parça — faaliyet, katılım ve stok

Başlangıçta master temiz ve uzak master ile aynıydı: `b3738ff0ff02d60218987838eb70c6ce36dca1cf`.
Dal: `codex/mts-faaliyet-stok-transaction`.

## Kapsam ve mesaj inceleme noktaları

Bu parça FaaliyetGirisiWP'nin yazma, listeleme, modal, timer ve takvim bildirimi yollarını kapsar. MtsActivityWriteService, MtsActivityWriteRepository ve MtsActivityNotificationService eklendi. Ortak MtsContactWriteRepository'nin transaction/audit yolu stok ve dağıtım kayıtlarına genişletildi. Depo/obje tanımı ve stok okuyucularına açık MTS bağlamı eklendi.

| Akış | Sonuç ve kullanıcı mesajı |
| --- | --- |
| Faaliyet ekleme/güncelleme | Konu, tarih/saat, tip, amaç, durum ve irtibat ilişkisi yazmadan önce doğrulanır. Bitiş başlangıçtan sonra olmalıdır. Eski form sürümüyle güncelleme reddedilir. Başarı yalnız commit ve takip adımları tamamlandıktan sonra gösterilir. Mesaj query string'i artık başarı popup'ı üretemez. |
| Aynı yeni formu yeniden gönderme | Formdaki sabit UniqueId transaction içinde aranır. Aynı oluşturucunun daha önce kaydedilmiş faaliyeti açılır; ikinci faaliyet, audit INSERT veya takvim bildirimi üretilmez. Yeni form yeni kimlik alır. |
| Silme | Arama, katılım ve dağıtım bağlantıları aynı transaction içinde yeniden kontrol edilir. Bağlantılı faaliyet silinmez; anlamlı uyarı gösterilir. İptal durumu stok iadesi veya bağlantı silme işlemini kendiliğinden yapmaz; mevcut iş ayrımı korunur. |
| Katılım/irtibat | Katılımcının ve faaliyetin varlığı, ilişki sahipliği, mükerrer katılım ve randevu kısıtı servis içinde doğrulanır. Anı objesi kaydı olan katılımcı çıkarılmaz. İrtibat ekleme/kaldırma sürüm kontrolüyle yazılır. Sonuç tek Türkçe mesajdır. |
| Stoklu dağıtım | Adet pozitif tam sayı olmalıdır. Ürün stoklu olmalı; katılım, depo ve stok satırı doğrulanır. Yetersiz stok ayrı uyarıdır. Dağıtım artışı ve stok azalışı aynı transaction'dır. Kayıt faaliyet/kişi/ürün/çıkış deposuna göre ayrılır; başka depodan verilen adet eski dağıtımın çıkış deposunu değiştirmez. |
| İade | Dağıtım kaydının faaliyetle ilişkisi, türü, sürümü ve çıkış deposu doğrulanır. Sıfır/negatif/fazla iade reddedilir. Sıfır mevcuda düşmüş depo satırı da okunur. Stok satırı gerçekten yoksa oluşturulur. Depoya ekleme ve dağıtımdan düşme/silme aynı transaction'dır. İade özgün çıkış deposuna gider. |
| Stoksuz seçim | Bütün ID/adet dizisi doğrulanır; tekrarlı/geçersiz ID, negatif adet ve stoklu ürün reddedilir. Ekleme, adet değiştirme, sıfıra düşürme ve seçimden çıkarma tek transaction'dır. Önceki kodda eksik olan yeni dağıtım ekleme yolu tamamlandı. Boş seçim mevcut stoksuz dağıtımları kaldırır; stoklu ve getirilen kayıtları etkilemez. |
| Getirilen obje | Var olan metin güncellenir veya ilk kayıt oluşturulur; boş metin kaydı kaldırır. Ayrılmış legacy 9999 obje işaretçisi korunur. Depo stoku etkilenmez. Faaliyet/katılım ve modal sürümü doğrulanır. Önceki modalın metni boş kayda taşınmaz. |
| Timer | Yeni form Id=0 için sorgu yapmaz. Faaliyet veya katılım/obje durumu değiştiğinde düzenleme durdurulur ve yenileme bağlantısı gösterilir. Karşılaştırma sınıfı, null yer metni ve takvim/yönetici notu karşılaştırmaları düzeltildi. Karşılaştırma HTML işlem düğmelerinden bağımsızdır. Silinmiş kayıt için sessizce düzenlemeye devam edilmez. |
| Takvim bildirimleri | Özel kalem ve katılımcı bildirimi business commit sonrasındadır. SMTP hatası teknik loga gider; kullanıcıya kaydın tamamlandığı, ekran/bildirim adımının tamamlanamadığı ve tekrar kaydetmeden kontrol etmesi gerektiği başvuru koduyla bildirilir. Açık tarihli ilk davet gönderilmez. Gönderildi işareti SMTP başarıyla döndükten sonra ayrı transaction/audit ile yazılır. |

Teknik ayrıntılar kullanıcıya verilmez; MtsInputException beklenen doğrulama/iş kuralı uyarısıdır. Beklenmeyen hatalar ortak MtsWebPartLog sınırında bir kez raporlanır. Takip adımında hata olursa önce başarı popup'ı gösterilip ardından çelişen hata popup'ı açılmaz. Sorgu sonucu üretilen JSON inline script için kaçırılır; kişi/kurum sütunları, obje adları, getirilen metin ve irtibat adı HTML çalıştırmaz. Uygulamanın ürettiği işlem bağlantıları korunur. Faaliyet yeri önerileri JSON serializer ile oluşturulur; boş liste hata vermez. Saat listeleri 00:00–23:55 aralığını kapsar ve varsayılan bitiş gece yarısına yanlışlıkla dönmez.

## Transaction, audit ve sınırlar

Taşınan bütün yazma servisleri önce `PortalApps.MTS.Write` uygulama kilidini, sonra aynı SqlTransactionContext içindeki kayıt kilitlerini kullanır. SELECT/INSERT/UPDATE/DELETE ve audit aynı transaction üzerinden yürür. Stok/dağıtım yazıları bağımsız eski servis çağrılarına bölünmez. Etkilenen kayıtlar yeniden okunur; başarısız yazı veya audit işleminde iş verisi de rollback olur.

Dağıtım ve stok adetleri, referanslar ve tarihler audit değişim alanlarıdır. Ad, telefon, adres, e-posta, konu, yönetici notu ve getirilen obje metni audit değişim değeri yapılmaz. SQL trace sorgu metni veya parametre değeri içermez. Salt okuma kurucuları eski yazma yoluna geçişi reddeder; bağlamsız eski çağıranlar korunur.

Depo/ürün için birden fazla stok satırı veya aynı çıkış deposunda mükerrer dağıtım tespit edilirse işlem ilişkisel uyarıyla durur; rastgele bir satır seçilip güncellenmez. Önceden yanlış birleştirilmiş dağıtımların gerçek çıkış deposu otomatik tahmin edilmez; bu dal tarihsel stok verisi temizliği yapmaz.

SMTP ve SQL ortak transaction olamaz. Gönderim başarılı olup durum yazımı başarısızsa yeniden göndermeden önce kontrol uyarısı verilir; otomatik yeniden gönderim/outbox eklenmedi. Kayıt tekrarında ikinci INSERT engellenmesi SMTP'nin tam olarak bir kez teslim edildiği garantisi değildir. Outlook UID korunur; REQUEST/CANCEL, Türkiye saatinden UTC dönüşümü ve ICS metin kaçırma kullanılır.

Takvim sürükle/bırak için sürüm kontrollü Move servisi bu dalda hazırlandı. FaaliyetViewerWP, FaaliyetViewerCustomWP ve HaftalikGorunumWP gibi diğer ekranların eski tarih yazma çağrıları dördüncü parçada bu servise bağlanacak. Bu dal yalnız FaaliyetGirisiWP'nin yazma yollarını tamamlar; henüz MTS'nin tüm yolları tamamlandı denmez.

## Yapılandırma ve kod üretimi

Mevcut `PortalLogging.EnabledWebParts` değerlerine `MTS:FaaliyetGirisiWP` eklenebilir. MTS'nin tamamı bitmeden EnabledProjects içine MTS eklemeyin.

Yeni paket veya DB migration yoktur. Üç yeni C# dosyasının DAO/Model Compile kayıtları eklendi. ASCX kaynağı ve üretilen dosyalar değişmedi; Run Custom Tool gerekmiyor. Commit/push/PR/merge kullanıcıya aittir.

## Kullanıcı testi — Visual Studio/Test sunucusu

1. Derleyip yayımlayın. Ekleme, düzenleme, amaç/tip/durum, açık tarih, tüm gün ve irtibat alanlarını kontrol edin. Boş konu, bozuk tarih/saat ve bitişin başlangıçtan önce/eşit olması tek anlamlı uyarı vermeli; satır yazılmamalı.
2. Yeni faaliyet kaydedin. Kayıt formda açılmalı, yeni kaydet gizlenmeli ve güncelleme açılmalı. Aynı eski form isteği tekrar gönderildiğinde ikinci faaliyet oluşmamalı. İki kullanıcıyla aynı faaliyeti açıp eski formdan güncellemeyi deneyin; ikinci işlem mevcut değişikliği ezmemeli.
3. İrtibat seçin/kaldırın. Katılımcı ekleyin; aynı kişiyi tekrar eklemeyi, randevu kısıtlı kişiyi ve başka faaliyete ait katılım ID'sini deneyin. Anı objesi bulunan kişi çıkarılmamalı. İlişki olmayan kişi çıkarılabilmeli.
4. Stoklu ürünü depodan verin: dağıtım +N ve stok -N olmalı. Aynı ürünü iki farklı depodan verin; iki ayrı çıkış deposu izlenmeli. Sıfır, negatif, yetersiz ve taşma yaratacak adetler veri değiştirmemeli. Aynı modalın eski isteği tekrar gönderildiğinde ikinci stok düşümü olmamalı.
5. Kısmi ve tam iade yapın: dağıtım -N ve özgün depo +N olmalı. Tam iadede dağıtım silinir. Depo mevcudu sıfır olan satıra iade edilebilmeli; satır gerçekten yoksa oluşturulmalı. Negatif/sıfır/fazla iade ve başka faaliyetin dağıtımı reddedilmeli. Eski iade isteği ikinci kez stok artırmamalı.
6. Stoksuz objelerde ilk ekleme, adet değiştirme, sıfırlama, seçim kaldırma ve bütün seçimleri boşaltmayı deneyin. Stoklu ve getirilen kayıtları korunmalı. Yeni objeler gerçekten oluşmalı. Bozuk ID/adet dizisi ve aynı ID'nin tekrarı tüm işlemi durdurmalı.
7. Getirilen metni ekleyin, güncelleyin, boşaltın. Başka kişinin modalına geçildiğinde eski metin kalmamalı. Bir başka kullanıcı aynı kişinin obje kayıtlarını değiştirirse eski modal reddedilmeli.
8. Test ortamında kontrollü dağıtım/stock/audit yazma hatası oluşturun. Stok tek başına düşmemeli veya artmamalı; dağıtım ve audit de aynı sonuçla rollback olmalı. Başvuru koduyla teknik kaydı bulun; kullanıcı teknik hata metni görmemeli.
9. Bağlantısız faaliyeti silin; arama/katılım/dağıtım bağlı faaliyeti silmeyi deneyin. İkincisinde veri korunmalı ve tek uyarı verilmeli. İptal durumuna almak stokları otomatik iade etmemeli.
10. İki kullanıcıyla faaliyet/katılım/obje değişikliği yapıp timer uyarılarını kontrol edin. Gereksiz sürekli “değişti” uyarısı olmamalı. Gerçek değişiklikte eski form yazma işlemleri durmalı; silinen kayıt düzenlenememeli.
11. Özel kalem ekleme/güncelleme/iptal, açık tarihe alma ve katılımcı davetini kontrollü alıcılarla deneyin. E-posta hatasında iş verisi kaydedilmiş kalmalı; tek uyarı ve başvuru kodu gelmeli. Gönderim durumu yalnız başarılı SMTP sonrasında yazılmalı; e-posta adresi yokluğu anlamlı uyarı olmalı. Commit sonrasında “kayıt başarısız” izlenimi verilmemeli.
12. Aynı OperationId ile faaliyet, katılım, dağıtım ve stok audit/SQL trace kayıtlarını inceleyin. Değişen alanlar ve adetler doğru olmalı; kişisel/serbest metinler audit değerlerinde bulunmamalı. Okuma, modal veya timer hatasında tek güvenli hata beklenir.

Codex Windows üzerinde build/test/debug çalıştırmadı. UTF-8/BOM, C# ayraçları, ASCX olayları, bağlam kurucuları, Compile kayıtları, eski yazma/popup çağrılarının kaldırılması ve transaction kaynak çağrıları statik olarak incelendi. Bu kontroller derleme ve çalışma zamanı testi yerine geçmez.

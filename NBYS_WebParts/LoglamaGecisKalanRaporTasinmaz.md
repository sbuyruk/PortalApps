# NBYS kalan raporlar ve taşınmaz armağan işlemleri

Dal: `codex/nbys-kalan-rapor-tasinmaz-transaction`.
Başlangıç: `74cc87fe00749da3ec7e69aec13e2e9b386cf33a` (22 Ekim 2026).
Değişiklikten önce yerel master temizdi ve uzak master ile aynı commit olduğu doğrulandı. Kullanıcı #518 sonrasında devam edilmesini istedi.

## Kapsam ve mesaj değişiklikleri

| WebPart | Değişiklik ve kullanıcı mesajı |
| --- | --- |
| BolgeArmaganRaporuWP | Bölge/ay raporu ve Excel merkezi bağlamlıdır. Ters ay aralığı açıklayıcı Warning verir; sorgu hatası boş/başarılı rapor gibi yutulmaz. Excel seçili filtreyle yeniden hazırlanır. |
| CokDefaBagisYapanBagisciListesiWP | Rapor, armağan geçmişi, modal, yönerge parametresi ve Excel açık bağlamlıdır. Armağan oluşturma transaction ve audit'e taşındı. Eksik seçim, değişen hak ediş, uygun olmayan bağış ve önceden oluşturulmuş madalya ayrı uyarılardır. Commit sonrası tek “Armağan oluşturuldu” mesajı çıkar; ekran aynı yerde yenilenir, armağan listesinden kayıt açılabilir. |
| NakitBagisciAdresListesiWP | Rapor/modal/Excel açık bağlamlıdır. Tarih aralığı ve pozitif bağışçı sayısı doğrulanır. Dergi gönderme tercihi yalnızca ilgili alanı günceller; “Dergi gönderimi açıldı/kapatıldı” sonucu doğru tercihi bildirir. Query string üzerinden başarı taşınmaz. İkinci aynı istekte Info ve yeni UPDATE/audit yoktur. Telefon2 yanlışlıkla tarih biçimine çevrilmez. |
| NakitBagisRaporuWP | Filtre, il/ilçe/armağan okumaları, rapor ve modal açık bağlamlıdır. Geçersiz tarih/tutar artık sessizce filtresiz rapora dönüşmez. Tarih ve tutar alt/üst sınırları için anlamlı Warning gösterilir. |
| TasinmazArmaganListesiWP | Ortak taşınmaz raporu NBYS bağlamıyla okunur. Gizli bağışları hariç tut checkbox'ı artık sorguya gerçekten aktarılır. Excel ve liste sorgu hataları tek güvenli teknik hata verir. |
| TasinmazArmaganWP | Form okumaları NBYS bağlamlıdır. Yalnızca armağan alanları transaction ve audit ile güncellenir; tarih/durum/kayıt/yetki doğrulanır. Kayıt numarası ilk açılışta yeniden varsayılanla ezilmez. Commit sonrasında form yenilenir ve tek başarı/tekrar istekte Info gösterilir. |
| TasinmazBagisciAdresListesiWP | Liste/modal/Excel NBYS bağlamlıdır. Boş listede dosya yazılmaz ve anlamlı Warning verilir. Etiket üretiminde hata yutulup arkasından ikinci mesaj çıkarılmaz. Upload tamamlanınca tek başarı; kısaltılan adres varsa aynı sonuç Warning içinde sayı ve kontrol adımıyla açıklanır. İsimler popup'a taşınmaz. |

IKYS/TBYS'deki `PortalLogContext`, `Messages` ve merkezi teknik hata/başvuru kodu yaklaşımı korunur. Yedi WebPart'ın ilk açılışı bir UserActivity üretir; postback tekrar açılış kaydı oluşturmaz. Beklenen giriş uyarıları teknik Error değildir. Teknik istisnalar `NbysWebPartLog` üzerinden loglanır; SQL, exception.Message ve stack trace kullanıcıya gönderilmez. Bir yükleme/olay hatasından sonra aynı istekte sonraki handler'ın yazma yapması engellenir.

## Transaction ve audit sınırı

Üç yazma akışında SELECT/INSERT/UPDATE, güncel kullanıcı bölgesi, kilitli kayıtlar ve audit aynı SqlTransactionContext içindedir. Complete işaretinden sonra Dispose/commit de başarıyla bitmeden UI'ya Completed/yeni Id aktarılmaz. SQL veya etkin audit yazımı başarısız olursa bütün iş yazmaları geri alınır. Commit sonrası ekran yenilemesi hata verirse “kayıt yapıldı, ekran yenilenemedi” sonucu tek teknik hata/başvuru koduyla bildirilir; ikinci bir başarı popup'ı çıkmaz.

- Çoklu bağış: Bağışçı kilitlenir; uygun hareketler ve aktif tutar limitleri transaction içinde yeniden okunur. Kurumsal hak ediş gerçek TuzelKisi alanıyla hesaplanır. Seçim modalında gösterilen bağışçı/tanım ViewState'de korunur ve yazma isteğiyle karşılaştırılır. Son bağış tarihi hidden alanından alınmaz, hareketlerden türetilir. En az iki uygun hareket gerekir. İade işaretli veya iade tutarı içeren kümede yeni madalya oluşturulmaz; iade sonrası yeniden hak edişin ayrı iş kuralı olarak ele alınması gerekir. Aynı geçerli madalya tanımı zaten varsa yeni kayıt oluşturulmaz. Eski yardımcıdaki dönem içindeki ilk armağanı yeniden yazma kaldırıldı: yeni madalya oluşturulur, eski armağanlar korunur, hareket bağlantıları aynı transaction'da taşınır. Eski uygunluk sorgusundaki 2/3/4 madalyasına bağlı hareketlerin dışlanması korunur; yalnızca teşekkür/başka tür bağlantısı olan uygun hareketlerin yeni madalyaya bağlanması inceleme noktasıdır. Yeni armağan ve her hareket bağlantısı audit edilir. Dönem başlangıcından önceki hareketleri de alan legacy oluşturma uygunluğu korunur; rapor görünümü başlangıç tarihi ve son bağış aralığıyla daha dar olabilir, yeniden hesaplanan tanım değişirse yazma durur.
- Dergi tercihi: UI'nin istediği son durum uygulanır, tersine çevirme yapılmaz. Kilitli gerçek bağışçı üzerinden yetki ve değişiklik kontrol edilir; sadece DergiGonderilmesin ve değişiklik metadata'sı yazılır. Başka form alanları eski nesneyle ezilmez.
- Taşınmaz: Bagis_Table'da yalnızca ArmaganId, ArmaganDurumu, ArmaganTarihi, ArmaganAciklama ve değişiklik metadata'sı yazılır. TBYS BagisService.Update ve taşınmaz/bağışçı yan güncellemeleri çağrılmaz. Mevcut durum seçenekleri korunur; bu ekran nakit para iadesi yapmaz. Merkez/tüm bölge yetkisi güncel kullanıcı bölgesinden alınır. Bölge kullanıcısı için taşınmaz bağışçısının IlId → Il.BolgeId eşleşmesi, legacy IlId boşsa Ili → IlAdi eşleşmesi aynı transaction'da kontrol edilir. Eşleşme yoksa yazma reddedilir; query string Auth yetki kanıtı sayılmaz. Liste okumalarının mevcut genel kapsamı bu parçada daraltılmadı.

Audit ayrıntıları nakit bağışçıda mevcut güvenli alan listesi, armağanda mevcut SaveGift listesi ve hareket bağlantısında ArmaganId'dir. Bagis_Table için yalnızca ArmaganDurumu ve ArmaganTarihi eklenmiştir. Serbest belge numarası/açıklama, ad, kimlik, telefon ve adres audit ayrıntısına girmez. Yalnızca açıklama/numara değiştiğinde audit üst kaydı yine oluşur. SQL izine sorgu/parametre değerleri yazılmaz; repository/metot, süre, sonuç ve işlem kimliği yazılır. Legacy OlayKayit/SQLLog'a ikinci yazma yapılmaz.

## Paylaşılan kod ve belge üretimi

`TbysSqlExecutor` NBYS bağlamıyla yalnızca mevcut taşınmaz SELECT'lerini çalıştırabilir. Transactionsız INSERT/UPDATE/DELETE girişleri NBYS bağlamını reddeder; transaction overload'larının TBYS bağlam koşulu korunur. NBYS yazmaları yeni NbysReportWriteService/Repository yolundadır. Mevcut TBYS/legacy çağrılarının davranışı korunur. UtilityHelper.YonergeURLGetir'e bağlamlı okuma overload'u eklendi; eski üç parametreli çağrı korunur.

Etiket verisi tek DataTable görüntüsünden üretilir. Şablon tabloları klonlanıp OpenXML paketi kapatılır; açık paket/dosya kaynağı bırakılmaz. XML metinlerine literal/escape edilmiş değer yazılır; `&`, `<` ve `$` adresi regex veya XML bozmaz. 21 etiket/sayfa sınırı, 1/3/21 kopya ve adresin son karakterinin gereksiz kesilmesi düzeltildi. Şablonun yer tutucuları mevcut biçimde tek XML parçasında bulunmalıdır; Word görsel çıktısı TEST'te kontrol edilmelidir.

Şablon ve çıktı mevcut NBYSBELGELERI_LIB kitaplığıyla sınırlandı; query string'den başka kitaplık seçilmez. Eksik/boş şablonda upload yoktur. GUID içeren dosya adı ve overwrite=false aynı anda üretilen dosyaların birbirini ezmesini önler. SharePoint/SQL/harici raporlar dağıtık transaction sayılmaz. Bu etiket işlemi SQL yazmaz. Upload tamamlandıktan sonra ekran/bağlantı adımı hata verirse dosyanın kitaplığa kaydedildiği tek güvenli mesajla belirtilir. Dosyalar otomatik silinmez. Excel içeriği response temizlenmeden önce hazırlanır; Response.End normal indirme sonlandırmasıdır, teknik hata olarak kaydedilmez.

OpenXML sürümü veya paket referansı değiştirilmedi. Yeni kontrol eklenmedi, üretilen ASCX dosyalarına dokunulmadı. Yeni SQL migration yoktur.

## Yapılandırma

Önceki değerleri koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

`NBYS:BolgeArmaganRaporuWP;NBYS:CokDefaBagisYapanBagisciListesiWP;NBYS:NakitBagisciAdresListesiWP;NBYS:NakitBagisRaporuWP;NBYS:TasinmazArmaganListesiWP;NBYS:TasinmazArmaganWP;NBYS:TasinmazBagisciAdresListesiWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Önceki merkezi log/audit migration ve bağlantı ayarları gereklidir. NBYS belge kitaplığında şablon okuma ve dosya ekleme izni bulunmalıdır. WSP yayını bu ayarları/izinleri kendiliğinden oluşturmaz.

## Kullanıcının TEST sunucusunda kontrol edeceği noktalar

1. Yedi ekranın açılış, filtre, modal ve Excel işlemleri: doğru NBYS/WebPart, ilk açılışta tek UserActivity, ortak istek OperationId ve SELECT SqlTrace. Eski SQLLog/OlayKayit'te çift kayıt olmamalı. Paylaşılan TBYS ekranlarında mevcut okuma/yazma davranışı korunmalı.
2. Bölge raporunda normal/boş/ters ay aralığı; nakit raporunda boş ve hatalı tarih/tutar, ters sınırlar; adres raporunda geçersiz/sıfır bağışçı sayısı. Beklenen uyarılar tek Warning olmalı, teknik Error oluşmamalı. Excel dosyaları doğru filtre ve Türkçe karakterlerle inmeli.
3. Çoklu bağışta özel/tüzel kişi, tanım limiti, önceden verilmiş aynı madalya, iade içeren küme, belge istemeyen bağışçı ve başka bölge kaydı. Modal açıldıktan sonra hidden seçim değişikliği veya hak ediş değişikliğinde yazma olmamalı. İki eşzamanlı onayda tek yeni madalya; mevcut armağanlar korunmalı. Armağan + hareket bağlantıları + audit birlikte commit/rollback olmalı. Armağan listesi/edit üzerinden yeni kaydı doğrulayın.
4. Dergi gönder/gönderme ve aynı tercihin tekrarı: doğru sonuç mesajı, doğru tek alan değişikliği, tekrar istekte yeni audit olmaması. Başka bölge ve silinmiş donor işleminde yazma olmamalı. Commit sonrası rapor yenilenememesi halinde kayıt sonucu doğru açıklanmalı.
5. Taşınmaz armağanında numara, tarih, durum ve açıklama değişikliği; değişikliksiz kaydetme; geçersiz tarih/durum/Id; merkez, kendi bölgesi ve başka bölge. BagisciId/TasinmazId/bağış tarihi ve oluşturma metadata'sı korunmalı. Audit/yazma hatasında armağan bilgileri de geri alınmalı. Açılışta kayıtlı armağan numarası korunmalı. Gizli bağış checkbox'ı liste/Excel'de etkili olmalı.
6. Adres etiketinde boş liste, şablon yokluğu, kitaplık izin hatası, 1/3/21 kopya, 20/21/22 etiket sınırı, gizli/vefat filtreleri, tek karakterli/110'dan uzun adres ve `& < $` karakterleri. Son karakter kaybolmamalı, boş son sayfa oluşmamalı; başarıdan sonra ikinci uyarı popup'ı olmamalı. Dosya kitaplığa yazılmış ama ekran tamamlanmamış senaryoda kısmi sonuç mesajı kontrol edilmeli.
7. Kontrollü SQL/audit hata senaryosunda tüm ilgili iş kayıtlarının rollback'i; teknik hata için tek Türkçe güvenli mesaj, başvuru kodu ve aynı OperationId'li teknik Error. Hiçbir hata sonrası başarı mesajı kalmamalı.

Codex Windows üzerinde build, test veya debug çalıştırmadı; DB/SharePoint üzerinde işlem yapmadı ve Word dosyası üretmedi. UTF-8/BOM, proje XML/Compile yolları, kaynak blokları, olay sınırları, gerçek metot imzaları, transaction çağrıları ve diff statik incelendi. Çalışma zamanı ve Word görsel doğrulaması kullanıcıya aittir.

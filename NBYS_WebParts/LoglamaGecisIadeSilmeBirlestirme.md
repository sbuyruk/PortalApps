# NBYS iade, silme ve bağışçı birleştirme işlem grubu

Dal: codex/nbys-iade-silme-birlestirme-audit
Başlangıç: f6afc6f8ea92a71070d99916639b243ba2df5927.
Kod değişikliği öncesinde yerel master temizdi; git ls-remote ile GitHub master ile aynı commit'te olduğu doğrulandı.
Kullanıcı önceki bağışçı yönetimi parçasını tamamlandı olarak bildirdi ve devam talimatı verdi.

## Kapsam

- BagisIadesiWP: arama/detay okumaları, iade onayı, iade tarih/sebep güncellemesi.
- NakitBagisHareketSilmeWP: liste/detay/onay, bağış silme, bağlı armağan ve bağışçı değerlendirmesi, arşiv.
- NakitBagisciBirlestirmeWP: kaynak/hedef seçimi, arama/detay, seçilen bütün kaynak bağışçıların tek işlem olarak birleştirilmesi.

Yeni NbysDonationWriteService üç iş akışını yürütür. Mevcut NbysBagisciWriteRepository transaction-aware okuma/yazma, audit ve arşiv metotlarıyla genişletildi.
Armagan/ArmaganTanim servis ve repository GetById bağlam overload'ları yalnızca açıkça bağlanan okumalarda kullanılır. Eski imzalar korunur.
Üç ekranın 34 olay sınırı ortak NbysWebPartLog'a bağlıdır; ilk istekte UserActivity kaydedilir. Postback açılış kaydı üretmez.

## İşlem ve transaction sınırı

Her iade/iade düzenleme/silme komutu tek SqlTransactionContext kullanır. Birleştirmede seçilen kaynak grubunun tamamı tek transaction'dır; kaynak başına ayrı commit yapılmaz.
Kullanıcı bölgesi oturum UserLogin üzerinden aynı transaction'da okunur. Bölgesel yetki, kayıtların güncel ili/bölgesi üzerinden kontrol edilir. Genel müdürlük/tüm bölgeler istisnaları önceki bağışçı yönetimi parçasındaki sabitlerle korunur.
Bağış işlemlerinde önce yalnızca bağışçı kimliği okunur, bağışçı kilitlenir, ardından bağış yeniden UPDLOCK/HOLDLOCK ile okunur. Bağışçısı bu sırada değişmişse işlem Conflict döndürür.
Birleştirmede kaynak ve hedef bağışçı satırları kimlik sırasıyla kilitlenir. Kayıt/yetki doğrulaması bütün seçim için tamamlanmadan yazmaya başlanmaz.
İş verisi, bağlantı düzeltmeleri, SilinenKayit_Table arşivleri, audit başlıkları ve alan değişiklikleri aynı transaction'dadır. Audit veya sonraki herhangi bir yazma hata verirse tamamı rollback olur.
Complete yalnızca tamamlanma işaretidir; başarı sonucu using/Dispose commit'i bittikten sonra döner. Transaction parametreli yeni zincir ayrı DbClass veya bağımsız iş bağlantısı kullanmaz.
Teknik Error/SqlTrace yazmaları mevcut ortak bağımsız yazıcıdadır; rollback sonrası kalabilirler. SqlTrace Succeeded yalnızca SQL komutunun sonucudur.

## İade ve iade düzenleme

İlk iade komutu kayıt anında iki yıllık süreyi tekrar kontrol eder. Daha önce iade edilmiş bağış ikinci kez iade edilmez; anlamlı iş kuralı sonucu döner.
İade tarihi geçerli değilse sorgu/iş yazmasına geçilmez. İade düzenleme yalnızca IadeEdildiMi=true kayıtlar için çalışır.
Mevcut iş alanları korunur: IadeMiktari önceki BagisMiktari olur; BagisMiktari sıfırlanır; tarih/sebep/iade eden ve mevcut açıklama güncellenir.
Geçerli armağan okumasında mevcut BelgeGecersizMi!=1 filtresi korunur. Gönderilmemiş armağan için mevcut geçersizleştirme, DURUM_PARAIADE, önceki tutar ve biriken iade tutarı davranışı aynı transaction'dadır. Diğer durumlardaki armağan korunur.
Bağış ve armağan farklı bağışçılara aitse yazma yapılmaz; bağlantı tutarsızlığı uyarısı döner.

Kaynakta TekrarArmaganHesapla yardımcılarının çalışır bir hesaplama yapmadığı, yalnızca yorumlanmış kod ve false sonucu içerdiği doğrulandı. Bu boş yardımcılar kaldırıldı; bu dal yeni aylık hak kazanma kuralı veya otomatik yeniden armağan üretme özelliği açmaz.

## Silme ve bağlantı bütünlüğü

Mevcut istemci kuralı olan 240 gün, onay anında sunucuda da doğrulanır; yalnızca düğmenin görünürlüğüne güvenilmez.
Bağış arşivi yazılır ve kayıt silinir. Gönderilmemiş armağan ancak başka bağış veya düzenli bağışçı kaydı tarafından kullanılmıyorsa arşivlenip silinir.
Gönderilmiş/durumu farklı veya başka kayda bağlı armağan korunur. Başarı mesajı bu durumda “Bağlı armağan korundu.” açıklamasını taşır.
EkstreAktarma_Table ve DuzenliNakitBagisci_Table içinde silinen bağışa ait NakitBagisHareketId bağlantıları aynı transaction'da 0'a alınır. Eski kimlik arşiv ve audit'te korunur. 0, kaynakta mevcut import başlangıç değeridir.
Ekstre AktarildiMi bayrağı ve düzenli bağışçı toplamları bu bağlantı temizliğiyle değiştirilmez. Düzenli bağış toplamı/hak kazanma hesabı bu dalın yeni bir özelliği değildir.
Otomatik bağışçı silme yalnızca bağış, bütün armağan geçmişi, ekstre ve düzenli bağışçı bağlantılarının tamamı boşsa yapılır. Böylece son bağış silindiğinde kalan armağan/ekstre/kayıtlar sahipsiz bırakılmaz.
Bu bağlantı korumaları eski akışa göre somut davranış düzeltmeleridir; silinen bağışın doğrudan işlem kapsamıyla sınırlıdır.

## Birleştirme

Seçimler tam sayı olarak doğrulanır. Boş seçim, pozitif olmayan kimlik ve hedefin kendisinin kaynak listesinde bulunması yazma yapmadan uyarı üretir. Aynı kaynak kimliğinin tekrarı tek kaynak olarak ele alınır.
Eksik/yetkisiz tek bir kaynak varsa grubun tamamı yapılmaz. Sonraki kaynakta SQL/audit hatası oluşursa önceki kaynak değişiklikleri de geri alınır.
Kaynak bağış hareketleri ve bütün armağan geçmişi (geçersiz belgeler dahil) hedefe aktarılır. EkstreAktarma.NakitBagisciId ve DuzenliNakitBagisci.BagisciId bağlantıları da taşınır.
Kaynak başına arşiv ve audit yazılır; kaynak bağışçı en son silinir. Hedefin ad/kimlik/iletişim bilgileri kaynak kişisel alanlarla değiştirilmez.
Birleştirme bu dalda armağanları aynı dönemde birleştirme/yeniden hesaplama algoritması çalıştırmaz; mevcut kayıtlar ve kimlikleri korunarak bağışçı ilişkileri taşınır.

## Audit alan izin listeleri

| İşlem/entity | Alanlar |
| --- | --- |
| Bağış UPDATE | BagisciId, ArmaganId, BagisTarihi, BagisMiktari, IadeEdildiMi, IadeMiktari, IadeTarihi |
| İade armağanı UPDATE | BagisciId, ArmaganTanimId, Durum, BelgeGecersizMi, GecersizNBHareketId, BagisMiktari, ArmaganBagisMiktari, IadeMiktari |
| Birleştirilen armağan | Önceki SaveGift izin listesi; esas değişiklik BagisciId |
| Ekstre/düzenli bağışçı bağlantısı | İlgili NakitBagisciId/BagisciId veya NakitBagisHareketId |
| Arşiv INSERT | Yalnızca TabloAdi |
| Bağışçı DELETE | Ili, Ilcesi, TuzelKisi |
| Armağan DELETE | BagisciId, ArmaganTanimId, Durum, BagisMiktari |
| Bağış DELETE | BagisciId, ArmaganId, BagisTarihi, BagisMiktari, IadeEdildiMi, IadeMiktari |

EntityId uygulama kayıt kimliğidir. UPDATE yalnızca değişen izinli alanları yazar. DELETE eski değerden NULL'a, INSERT var/yok değerleriyle kaydedilir.
Ad, T.C. kimlik numarası, iletişim, adres, iade/silme sebebi, açıklama ve SilinenKayitBilgisi merkezi audit ayrıntısına alınmaz.
Mevcut SilinenKayit_Table iş arşivi korunur; eskiden bu tabloya yazılan sınırlı silme/birleştirme ayrıntıları burada kalır. Arşiv içeriğinin merkezi loga kopyalanması yapılmaz.
Eski/yeni audit değerleri aynı transaction'daki gerçek DB okumalarından alınır. NULL ve saklanan tarih/sayı hassasiyeti korunur.

## Mesaj değişiklikleri

- İade, iade düzenleme, silme ve birleştirme için işlem başına tek başarı mesajı; yalnızca commit sonrası.
- Kayıt bulunamaması, yetkisizlik, süre aşımı, tekrar iade, iadesiz kayıt düzenleme, geçersiz tarih/seçim, kendisiyle birleştirme ve değişmiş/tutarsız bağlantılar ayrı Warning sonuçlarıdır.
- Boş arama sonucu Info'dur; genel teknik hataya çevrilmez.
- İade onay metni para transferi veya armağan silme vaadi yerine iadenin kaydedilmesini ve armağanın geçersiz işaretlenmesini açıklar.
- Teknik hata, ortak güvenli Türkçe mesaj ve başvuru koduyla gösterilir; ham exception/SQL/stack trace kullanıcıya aktarılmaz.
- Yardımcıların hatayı yutup devam etmesi ve kaynak başına başarı popup'ı kaldırıldı. Aynı WebPart'ın ilk teknik hatasından sonra sonraki olayları durur; aynı exception zinciri tekrar bildirilmez.
- Commit sonrası liste yenileme başarısızsa tek bildirim işlemin tamamlandığını ve sayfanın yenilenmesi gerektiğini söyler. Ayrıca başarı popup'ı eklenmez.
- NakitBagisHareketSilmeWP üzerindeki Mesaj querystring/ViewState başarı yolu kaldırıldı. URL'ye Mesaj=true eklemek veya sayfayı yenilemek işlem yapılmadan başarı mesajı üretmez.
- Başarı sonrasında kullanıcı arama/listede kalır; eski başarı yönlendirmelerinin mesajı kaybetmesi ve yeniden başarı üretmesi önlenir. Yönlendirme kaynaklı ThreadAbortException teknik hata sayılmaz.

## Yapılandırma ve kapsam sınırı

Mevcut IKYS/TBYS ve önceki NBYS değerlerini koruyarak PortalLogging.EnabledWebParts listesine ekleyin:

NBYS:BagisIadesiWP;NBYS:NakitBagisHareketSilmeWP;NBYS:NakitBagisciBirlestirmeWP

NBYS projesinin tamamını EnabledProjects ile açmayın.
Yeni migration yoktur. Hedefte mevcut dbo.PortalLog_Table ve dbo.PortalAuditChange_Table şeması/izinleri bulunmalıdır; bağlantı mevcut TSKGV_Connection'dan alınır. WSP deploy migration çalıştırmaz.
Model.csproj ve NBYS_WebParts.csproj yeni servis/feedback kaynaklarını içerir. Paket/feature değişikliği ve yeni ASCX kontrolü yoktur. *.ascx.g.cs dosyaları değiştirilmedi.
İş okumaları açık bağlamlıdır. Ekrandaki mevcut yönerge bağlantısını üreten ortak UtilityHelper.YonergeURLGetir davranışı bu dalda yeniden tasarlanmadı.
Bağımsız ekstre import/aktarma ve düzenli bağışçı iş kuralları bu dalda taşınmış sayılmaz; yalnızca buradaki silme/birleştirme komutlarının doğrudan bağlantı düzeltmeleri aynı transaction'a dahildir.

## Kullanıcının Visual Studio 2026 / TEST doğrulaması

1. Solution/NBYS WSP'yi derleyip TEST'te yayımlayın. Üç ekranın açılış/arama/detay ve ilgili SELECT izlerini, ortak OperationId'yi kontrol edin. Postback yeni açılış üretmemeli.
2. İki yıl sınırının içindeki test bağışını iade edin. Bağış tutarı sıfır, iade tutarı önceki tutar olmalı. Gönderilmemiş armağan geçersiz/para iade durumuna geçmeli; gönderilmiş armağan korunmalı. Tek başarı ve aynı işlemde audit beklenir.
3. Aynı iadeyi tekrar gönderin; ikinci tutar değişikliği/audit olmadan anlamlı uyarı beklenir. İade tarih/sebebini düzenleyin; iade olmayan kayıtta, geçersiz tarihte veya süresi dolmuş ilk iadede uyarı ve sıfır yazma beklenir.
4. 240 günlük sınır içindeki test bağışını silin. Bağış/arşiv/audit aynı işlemde oluşmalı. Süresi geçmiş bağışı onay ekranından/daha önce açılmış modalden göndermeyi deneyin; sunucu reddetmeli.
5. Bağışa bağlı ekstre/düzenli bağışçı satırında NakitBagisHareketId temizliğini ve eski kimliğin audit/arşivde bulunmasını kontrol edin. AktarildiMi ve düzenli bağış toplamlarının değişmediğini doğrulayın.
6. Başka bağışa veya düzenli kayda bağlı armağan ve kalan başka referanslı bağışçı silinmemeli. Başka bağlantısı olmayan son bağışta uygun armağan/bağışçı arşivlenip silinmeli. Korunan armağan başarı mesajında açıklanmalı.
7. En az iki kaynağı tek hedefte birleştirin. Bütün hareketler, geçersiz dahil armağan geçmişi, ekstre ve düzenli bağışçı bağlantıları hedefe geçmeli; kaynaklar silinmeli; hedef kişisel bilgileri değişmemeli. Tek başarı ve ortak OperationId beklenir.
8. Boş/geçersiz seçim, hedefi kaynak listesine ekleme, silinmiş kaynak ve yetkisiz bölge örneklerini deneyin. Hiçbir kaynakta kısmi değişiklik olmamalıdır.
9. TEST'te kontrollü olarak ikinci kaynak yazması, armağan yazması, bağlantı düzeltmesi veya arşiv/audit yazmasını başarısız kılın. Bütün iş/arşiv/audit değişiklikleri rollback olmalı; teknik hata ve SQL izleri kalmalı; başarı mesajı görünmemelidir.
10. Commit sonrası liste SELECT hatasında işlem tamamlandı/liste yenilenemedi mesajı tek kez çıkmalı. Mesaj=true URL'si ve sıradan sayfa yenilemede sahte başarı görünmemeli.
11. Log ayarı kapalıyken işlemler transaction'lı kalmalı ve eski çift kayıt üretmemeli. Yeni düzenleme servisinin diğer ekranların eski parametresiz yollarını değiştirmediğini gerileme açısından kontrol edin.

Codex build/test/debug çalıştırmadı. Doğrulama statik imza/transaction çağrı incelemesi, C# sembol/region dengesi (derleyici değildir), mevcut modal JS adları, proje/XML/Compile, UTF-8/BOM ve git diff kontrolleriyle sınırlıdır.
Çalışma zamanı, bağlantı alanları, iş kuralları ve rollback senaryoları kullanıcı TEST sonucunu bekler. Commit/push/PR/merge kullanıcıya aittir.
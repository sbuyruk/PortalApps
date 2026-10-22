# NBYS bağışçı yönetimi — transaction, audit ve mesajlar

Dal: codex/nbys-bagisci-yonetimi-transaction-audit
Başlangıç: 2b6b0e7ca6450ea519148f132c00c6d3828ce1e1.
Kod değişikliği öncesi yerel master temizdi; git ls-remote ile GitHub master commit eşitliği doğrulandı.
Kullanıcı önceki toplu liste/rapor parçası için “tamamlandı devam edelim” talimatı verdi. Tek tek özel rollback/Excel senaryolarının tamamlandığı ayrıca varsayılmadı.

## Birlikte ele alınan ekranlar

- NakitBagisciEditWP: bağışçı düzenleme, bölge yetkisi, il/ilçe filtreleri, form okuması ve kayıt.
- NakitBagisciListesiWP: bağışçı listesi, filtreler, detay ve bağış okumaları.
- NakitBagisciBulmaWP: arama, bağışçı ve bağış detayları.
- NakitBagisciBagislariWP: bağışçı seçimi, bağış detayları ve toplamları.

Düzenleme işleminin tüzel kişi değişikliğiyle tetiklediği Armagan_Table ve NakitBagisHareket_Table.ArmaganId yazmaları aynı işlem grubuna dahildir.
İade, bağış silme, bağışçı birleştirme, ekstre import/aktarma ve bağımsız armağan ekranı yazmaları bu dalda dönüştürülmedi. Bunlar ayrı ilişkili işlem grupları olacak.

## Gerçek işlem sınırı

Yeni NbysBagisciWriteService ve NbysBagisciWriteRepository bağışçı, bağlı armağan hesabı ve hareket bağlantılarını tek SqlTransactionContext içinde yürütür.
Kayıt sırasında oturum kullanıcısının bölgesi yeniden aynı transaction'dan okunur. Kullanıcı adı log bağlamındaki UserLogin'den alınır; istemcinin gönderdiği bölge kimliği yazma yetkisi olarak kabul edilmez.
Personel kullanıcı adı domain sonrasından alınır; Personel için ORDER BY Id, iş bilgileri için ORDER BY UnvanId ile mevcut ilk kayıt seçimi korunur. Birim ve bölge aynı mevcut tablolardan okunur.
Bölgesel kullanıcı için mevcut bağışçının ili/bölgesi transaction içinde tekrar doğrulanır. Genel müdürlük/tüm bölgeler istisnaları mevcut sabitlerle korunur.
Yetkisiz veya artık bulunamayan kayıt iş sonucu döndürür; teknik exception üretilmez ve transaction tamamlanmaz.

Yazma sırasında:

1. Kullanıcı bölgesi ve önceki bağışçı transaction üzerinden okunur.
2. Bağışçı güncellenir; oluşturma bilgileri ve formda düzenlenmeyen soyadı kilitli önceki kayıttan korunur.
3. Tüzel kişi değişmişse gönderilmemiş armağanlar, aylık bağışlar, hak kazanma tanımı ve armağan geçmişi aynı transaction üzerinden okunur.
4. Gerekli armağan kaydı ve bağış hareketi ArmaganId bağlantıları aynı transaction üzerinden yazılır.
5. Her yazmanın audit başlığı ve izinli eski/yeni değerleri aynı transaction'da yazılır.
6. Bütün adımlar başarılıysa Complete işaretlenir. Gerçek commit Dispose sırasında biter; yalnızca using kapandıktan sonra Saved sonucu arayüze döner.

Yeni transaction parametreli akışlar eski servis/repository yazmalarını veya new DbClass() çağrısını kullanmaz. Bağlantı açan eski SaveArmagan/SaveOrUpdate zinciri bu yeni akıştan çağrılmaz.
Nullable SelectUserRegion yardımcısında yalnızca transaction==null dalı bağımsız okuma yapar; transaction verilmiş dal sql.Select(query, transaction) kullanır.
Eski transaction-aware ve parametresiz çağıranlar diğer ekranlar için korunmuştur; onlar bu dal nedeniyle otomatik olarak merkezi loglamaya taşınmış sayılmaz.

## Armağan kuralı karşılaştırması

Kaynakta EkstreAktarmaTransferService.SaveArmagan, ArmaganiKaydetVeyaGuncelle, ArmaganService.SaveOrUpdate/GetDonorStatus ve gerçek repository sorguları birlikte incelendi.

- Tüzel kişi değişmemişse armağan hesabı tetiklenmez.
- Bilinmeyen bağışçı adını içeren kayıt için armağan hesabı yapılmaz.
- Mevcut aylık başlangıç/bitiş ve bitiş günü gece yarısı dahil aralık korunur. İade ayrımı eklenmez; mevcut toplam sorgusundaki bütün dönem bağışları hesaba girer.
- Hak kazanılan aktif tanım bulunmazsa veya ARMAGAN_YOKID ise mevcut armağan olduğu gibi kalır.
- Belge istememe, tek e-Devlet bağışı, önceki para iadesi ve önceki aynı tanıma ait armağanın teşekkür tanımına dönüşme kuralları korunur.
- Son durumda mevcut GetDonorStatus sırası korunur: BelgeIstemiyor, ardından Ulasilamiyor.
- Aynı dönemin mevcut armağanı güncellenir, yoksa oluşturulur; dönem bağışlarının armağan bağlantıları bu kayda bağlanır. Aynı ArmaganId zaten bağlıysa gereksiz hareket UPDATE/audit'i yapılmaz.
- Aynı ayda birden fazla gönderilmemiş armağan varsa eski akışın her başlangıç kaydı için hesaplama davranışı korunur; tek aya indirgeme veya yeni bir hak kazanma algoritması uygulanmadı.
- Önceden hata yutup devam eden alt işlemler yeni yolda yoktur. Herhangi bir bağlı yazma/audit hatasında bağışçı dahil tüm değişiklikler geri alınır.

Servis/repository INSERT yolunu da destekler. Ancak mevcut NakitBagisciEditWP Page_Load yalnızca pozitif NakitBagisciId ile düzenlemeye izin veriyor. Bu dal yeni bağışçı giriş ekranı veya mevcut olmayan bir oluşturma yetkisi açmaz; kimliksiz sayfada Kaydet kapalı kalır. TEST'te bu ekran üzerinden yeni bağışçı ekleme beklentisi olmamalıdır.

## Audit alan izin listeleri

EntityId uygulama Id değeridir; T.C. kimlik numarası gibi doğal anahtarlar kullanılmaz.

| Entity | Alanlar |
| --- | --- |
| NakitBagisci | Ili, Ilcesi, TuzelKisi, Sag, Ulasilamiyor, BelgeIstemiyor, DergiGonderilmesin |
| Armagan | BagisciId, ArmaganTanimId, Tarih, Durum, BagisMiktari, DovizCinsi, CokluBagis |
| NakitBagisHareket | ArmaganId |

Ad/soyad, T.C. kimlik numarası, adres, telefonlar, e-posta, posta kodu, meslek, açıklamalar ve belgede yazan serbest isim audit ayrıntısına alınmaz.
Yalnızca kişisel metin değiştiğinde Audit başlığı oluşur, izinli alan değişmediyse alan ayrıntısı sıfır olabilir.
Eski/yeni değerler veritabanından transaction içinde alınır; NULL değerler sıfıra veya boş metne çevrilmez. SQL tarih/sayı hassasiyeti kayıt sonrası okumayla esas alınır.

## Log ve okuma geçişi

Dört ekran ilk istekte UserActivity üretir; postback açılış kaydı üretmez.
Bağışçı okuma kurucuları açık PortalLogContext alır. Yeni salt okuma yolunda eski SQL log yazıcısına dönülmez; yanlış yazma çağrısı korumayla reddedilir.
Bağışçı listesi/arama ve eski SQL metinleri korunur; 21 mevcut SQL metni statik karşılaştırmada aynı bulundu.
İl ve ilçe okuma overload'ları log bağlamını kullanır. IlceRepository mevcut bağlamlı GetById yolunda NBYS için yeni okuyucu kullanır; diğer proje bağlamlarının eski davranışı korunur.
Yetki açılış okumaları da yeni NBYS bağlamıyla kaydedilir; eski IKYSOrtak helper'ının ayrıca popup üretmesi bu ekrandan kaldırıldı.
SQL izi işlem türü, repository/metot, süre ve sonucu taşır; ham SQL ve parametre değerleri gönderilmez.
Bağışçı yazmaları SaveDonor, armağan yazmaları SaveGift ve bağlantı değişiklikleri LinkDonation adlarıyla izlenir. Aynı isteğin UserActivity, SqlTrace, Audit ve Error kayıtlarında ortak OperationId bulunur.
Teknik Error ve SQL izleri mevcut bağımsız PortalTechnicalLog hedefindedir; rollback sonrası korunabilir. SqlTrace Succeeded yalnızca komutun başarılı olduğunu gösterir, transaction commit garantisi değildir.
Loglama kapalıyken de yeni iş yazması transaction'lı kalır ve eski OlayKayit yoluna dönmez; yeni audit/teknik kayıt üretimi mevcut ayarlara bağlıdır.

## Mesaj değişiklikleri

- Dört ekranın eski ExceptionHelper sunumları kaldırıldı. Hatalar yardımcıda yutulmadan ortak NbysWebPartLog.ExecuteAction sınırına ulaşır.
- Aynı istekte ilk teknik hatadan sonra o WebPart'ın sonraki olayları çalıştırılmaz. Aynı exception zinciri tekrar loglanıp bildirilmez. Yönlendirmedeki ThreadAbortException hata kaydı sayılmaz.
- Boş ad tek Warning üretir; arkasından ikinci “kaydedilemedi” mesajı çıkmaz.
- Kimlik numarası için sayısal olmayan/taşan girdi anlamlı Warning üretir; işleme devam edilmez. Bu kontrol T.C. kimlik numarası algoritması veya uzunluk doğrulaması iddiası taşımaz.
- Eksik bölge, yetkisizlik, bulunamayan kayıt ve yüklenemeyen il/ilçe seçenekleri ayrı Türkçe uyarılardır. Ham teknik ayrıntı gösterilmez.
- Başarı metni “Bağışçı bilgileri kaydedildi.” yalnızca bütün işlem commit edildikten sonra gösterilir.
- Teknik hatada güvenli Türkçe mesaj ve başvuru kodu gösterilir; exception.Message, SQL ve stack trace yalnızca teknik log hedefindedir.

## Yapılandırma

Mevcut IKYS/TBYS ve önceki NBYS değerlerini koruyarak PortalLogging.EnabledWebParts listesine şunları ekleyin:

NBYS:NakitBagisciEditWP;NBYS:NakitBagisciListesiWP;NBYS:NakitBagisciBulmaWP;NBYS:NakitBagisciBagislariWP

NBYS projesinin tamamını EnabledProjects ile açmayın.
Yeni migration yoktur. Hedef ortamda mevcut birleşik dbo.PortalLog_Table ve dbo.PortalAuditChange_Table şeması/izinleri bulunmalıdır. Bağlantı çalışan SharePoint web.config içindeki TSKGV_Connection üzerinden mevcut altyapıdan alınır. Codex veritabanı/yapılandırma/deploy değişikliği yapmadı.
DAO.csproj ve Model.csproj yeni repository/service dosyalarını kapsar. Mevcut paket proje çıktılarını kullanır. Yeni ASCX kontrolü veya feature değişikliği yoktur; *.ascx.g.cs dosyaları değiştirilmedi.

## Kullanıcının Visual Studio 2026 / TEST adımları

1. Solution ve NBYS WSP'yi derleyip TEST'e yayımlayın; DAO/Model/Utility çıktılarının güncel olduğunu doğrulayın.
2. Dört ekranı açın; arama, il filtresi, bağışçı detayı ve bağış detaylarını deneyin. İlk açılış UserActivity ve SELECT izlerinin OperationId eşleşmesini kontrol edin; postback'te tekrar açılış olmamalı.
3. Mevcut test bağışçısını liste üzerinden düzenleyin. Tüzel kişi değişmeden ad/iletişim veya bir izinli bayrağı değiştirin. Tek başarı mesajı, donor UPDATE audit'i ve gerçek kayıt sonucu beklenir. Kişisel metinler audit/SQL izine girmemeli.
4. Gönderilmemiş armağanı bulunan bir test bağışçısında tüzel kişi bilgisini değiştirin. Eski kurala göre farklı limit/tanımın seçildiğini, armağan durumunu, tutarı ve dönem bağışlarının ArmaganId bağlantılarını doğrulayın. Yeni akıştaki bütün audit kayıtları aynı OperationId taşımalıdır.
5. Belge istemeyen/ulaşılamayan bağışçı, tek e-Devlet bağışı, önceki para iadesi, önceki aynı tanım ve hak kazanılmayan tutar örneklerini karşılaştırın. Aynı ayda birden fazla armağan ve birden fazla ay içeren bağışçıları da deneyin.
6. TEST'te kontrollü armağan UPDATE/INSERT, hareket bağlantısı veya audit ayrıntısı hatası oluşturun. Bağışçı değişikliği dahil bütün iş/audit değişiklikleri geri alınmalı; tek güvenli hata/başvuru kodu ve bağımsız teknik kayıt kalmalı. Başarı mesajı görünmemeli.
7. Bölgesel ve genel müdürlük kullanıcılarıyla yetkiyi karşılaştırın. Yetkisiz/bölgesi bulunamayan kullanıcı kaydedememeli; modal/form açıkken donor bölgesinin değişmesi halinde kayıt anındaki kontrol esas alınmalı.
8. Boş ad ve geçersiz kimlik girdisinde tek uyarıyı, bulunamayan kayıtta yeniden seçim mesajını kontrol edin. Kimliksiz düzenleme URL'sinde mevcut Kaydet kapalı davranışını koruyun.
9. Loglama kapalıyken iş yazması transaction'lı çalışmalı; merkezi kayıt ve eski çift kayıt oluşmamalıdır. Eski ekstre/iade/silme ekranlarının parametresiz servis yollarında gerileme olmadığını kullanıcı ayrıca kontrol etmelidir.

Codex build/test/debug çalıştırmadı. Statik metot imzaları, transaction çağrı zinciri, iş kuralı kaynak karşılaştırması, proje/XML/Compile kayıtları, C# sembol dengesi (derleyici değildir), UTF-8/BOM ve git diff kontrolleri uygulandı.
Çalışma zamanı, armağan kuralları ve rollback TEST sonucu kullanıcı doğrulamasını bekler. Commit/push/PR/merge kullanıcıya aittir.
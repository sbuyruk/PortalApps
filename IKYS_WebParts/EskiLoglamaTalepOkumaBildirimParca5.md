# IKYS eski loglama geçişi — parça 5

Başlangıç: temiz yerel `master` ve canlı uzak `master` aynı commit'te doğrulandı: `b51b9b0a2e90ed8e5fc10b497062b89afcb17fdf` (PR #541).

Dal: `codex/ikys-talep-okuma-bildirim-eski-log`.

## Kapsam ve somut bulgular

Önceki parçanın `IzinTalepListesiWP` ve `MazeretIzinTalepListesiWP` yazma transaction'ları korunur. Bu parça, aynı ekranların liste/ayrıntı okumaları ve commit sonrasındaki bildirim zincirini eski SQL/hata yazıcısından ayırır. Eski tablolara veya geçmiş verilere dokunulmaz.

Context parametresi tek başına güvenli yol anlamına gelmiyordu:

- `IzinTalepRepository.TraceSelect`, `IzinTanimRepository.TraceSelect`, `PersonelRepository.SelectById`, `IsBilgileriRepository.SelectByPersonelId` ve `IletisimBilgileriRepository.SelectByPersonelId` context almasına rağmen `db.SelectFromDb` çağırıyordu. Eski DB okuyucusu SQL hatasında genel log bayrağından bağımsız olarak `SQLLog_Table` yazıcısına gidiyordu.
- `IzinDonemService.GetById/GetByIzinTarihi/GetByPersonelId` ve `IzinHareketService.GetByIzinDonemiReturnDataTable` context overload'ları bağlamsız repository okumasını teknik izle sarıyordu.
- `IKYSOrtak.IzinKabulRedOnayEPostasiGonder`, personel/talep/tanım/dönem/iletişim ve SMTP ayarını bağlamsız okuyordu. İçindeki `KalacakIzinHesapla` da ayrıca DB erişimi yapıyordu.
- `MailHelper.EPostaGonder` SMTP hatasını yutup `ExceptionHelper.PublishException` çağırıyordu. Bu yüzden önceki parçadaki `BildirimGonder` hata sınırı SMTP hatasını yakalayamıyordu.

## Proje bazında değişiklikler

### DAO — yedi repository

`IzinTalepRepository` ve `IzinTanimRepository` context alan `TraceSelect` metotları artık `PortalSqlReadExecutor.Select` kullanır. `PersonelRepository.SelectById`, `IsBilgileriRepository.SelectByPersonelId` ve `IletisimBilgileriRepository.SelectByPersonelId` context yolları da aynı yürütücüye bağlandı. Eski `DbClass` okuması ve onu çevreleyen ikinci iz katmanı bu yollardan çıkarıldı. Personel sorgusunun Portal/MTS için mevcut teknik metot adı korundu.

`IzinDonemRepository.SelectById/SelectByIzinTarihi/SelectByPersonelId` ve `IzinHareketRepository.SelectByPersonelIdDonemId` için context overload'ları eklendi. SQL metni, parametreler, tarih sınırları, sıralama ve mevcut filtreler korunur; yalnız sorgu yürütücüsü değişir.

Bu değişiklikler ilgili context overload'larını kullanan diğer ekranları da etkiler: aynı SELECT sonuçları merkezi SQL iziyle döner; hata eski SQL tablosuna yazılmaz. Bağlamsız repository metotları, henüz taşınmamış çağıranları nedeniyle durur. Transaction overload'ları değiştirilmedi.

### Model — servisler ve yardımcılar

- `IzinDonemService` kapsam içindeki üç context overload'ını doğrudan yeni repository overload'larına bağlar. `IzinHareketService.GetByIzinDonemiReturnDataTable(int, int, int, PortalLogContext)` aynı şekilde ilerler. Servis seviyesindeki mükerrer SQL iz sarmalaması kaldırılır; her gerçek sorgu yürütücüde tek iz üretir.
- `IKYSOrtak.IzinKabulRedOnayEPostasiGonder` artık context ister. Solution içindeki iki çağıran da güncellendi; kullanılmayan üç parametreli eski imza kaldırıldı. Bu yardımcıdaki bütün DB okumaları context ile yürür. Context null ise hata verir; merkezi loglama kapalı diye eski yola dönmez.
- `KalacakIzinHesapla` artık önceden okunmuş `IzinDonem` nesnesini alır ve DB erişimi yapmaz. Gün/saat hesabı kaynak karşılaştırmasıyla aynı bulundu. Bildirimde mazeret dönemi ikinci kez sorgulanmak yerine aynı nesne kullanılır. E-posta konusu, gövdesi ve belge bağlantılarının kurulması korunur.
- `UtilityHelper.ParametreDegeriSorgula(string, PortalLogContext)` eklendi. Mevcut `OrtakParametreService.GetByKey` context overload'ı ve onun güvenli repository yolu kullanılır. Parametre okunamazsa hata merkezi teknik yazıcıya gider ve null döner; mevcut alternatif SMTP adresi seçimi sürer. Eski bağlamsız yardımcı diğer çağıranlar için korunur.

### Utility — posta gönderimi

`MailHelper.EPostaGonderVeyaHataVer`, mevcut adres/konu/HTML gövde ve noktalı virgülle ayrılmış alıcı davranışını kullanır. SMTP hatasını yutmaz; alıcı yoksa da hata verir. Kaynaklar `using` ile kapatılır. Metot log veya kullanıcı mesajı üretmez; ekranın `BildirimGonder` sınırı bir merkezi hata ve güvenli Türkçe bildirim mesajı üretir. Diğer projelerin kullandığı eski posta metodu bu parçada değiştirilmedi.

### IKYS_WebParts — iki ekran

`FillIzinHareketleriTable`, `FillIzinBilgileriTable` ve `FillIzinTalebi` okumaları context overload'larına bağlandı. `IzinHareketleriBtn_Click` ayrıntı yüklemesindeki hataları merkezi `ReportError` sınırına iletir; ham teknik istisna kullanıcıya çıkmaz. Silinmiş talep ayrıntısına kontrollü hata sınırı eklendi.

`BildirimGonder` yeni context imzasını çağırır. Önceki parçadaki iş kaydı/commit sınırı değişmedi: SMTP ve bildirim hazırlama okumaları iş transaction'ı tamamlandıktan sonradır. Bildirim hatası tamamlanmış iş kaydını geri almaz ve tekrar iş yazımı başlatmaz. Kullanıcı hareketi ve audit için yeni bir log katmanı eklenmedi.

## Statik doğrulama

- `git diff --check` temiz. 14 kaynak dosyası sıkı UTF-8 ile okundu; BOM durumu HEAD ile karşılaştırıldı, CRLF korundu ve ayraç dengesi kontrol edildi.
- Mevcut 64 transaction parametreli metodun gövdeleri HEAD ile aynı. İki ekranın `TalebiIsle` ve `TalepHareketiniKaydet` yardımcıları da aynı; iş verisi/audit transaction bütünlüğü değiştirilmedi.
- Dört yeni repository overload'ının sorgu/parametre gövdeleri mevcut sorgularla eşdeğer. Kalan izin hesabı, DB okuması çıkarıldıktan sonra önceki hesap gövdesiyle aynı.
- İki ekranda 26 okuma çağrısının context aktarımı; beş doğrudan repository geçişi; bildirim, SMTP parametresi ve posta hata zinciri incelendi. Etkin 11 ASCX olay bağlantısı ve 14 dosyanın mevcut proje Compile bağlantıları doğrulandı.
- Eski bildirim imzasının iki kaynak çağırıcısı solution genelinde tarandı ve güncellendi. Teknik SQL/hata yazıcılarının bağımsız bağlantı tasarımı değiştirilmedi.
- ASCX, `*.ascx.g.cs`, proje, feature, paket, ayar ve DB şeması değişmedi. Yeni derlenecek dosya eklenmedi.

Windows bilgisayarında build, test veya debug çalıştırılmadı; e-posta gönderilmedi. Commit, push, PR, merge, yayın ve çalışma zamanı doğrulaması kullanıcıya aittir.

## Kullanıcının uygulayacağı kabul senaryoları

Kontrollü test kayıtları kullanın. Ana senaryoları merkezi IKYS ve ilgili ekran log ayarları açıkken ve kapalıyken tekrarlayın; test sonunda ayarları başlangıç değerlerine getirin.

1. Visual Studio 2026'da solution'ı derleyin ve Test sunucusuna yayımlayın. İki liste, modal ayrıntısı, geçmiş dönem/bakiye tablosu ve hareket tablosunu karşılaştırın. Ücretli, mazeret ve diğer türlerde veri/sıralama/tarih sınırları aynı olmalı. Boş liste, eksik dönem ve ayrıntısı açılırken silinmiş talebi de deneyin.
2. İki ekranda kontrol, onay ve ret yapın. Önceki parçanın tek hareket, tek bakiye düşümü, durum geçişi ve eşzamanlı tekrar engeli korunmalı. Bildirim kapalı talepte SMTP çağrısı olmamalı.
3. Ücretli ve mazeret talebinde kontrol e-postasının alıcısını, konusunu, açıklamasını, tarih/süre ve izin belgesi bağlantılarını karşılaştırın. Ret ve onay gövdeleri aynı anlamı taşımalı. Mazeret döneminin gereksiz ikinci SELECT'i artık beklenmez.
4. SMTP parametresi dolu, bulunamayan ve okunması kontrollü olarak başarısız olan durumları deneyin. Son iki durumda mevcut alternatif sunucu seçimi sürmeli; okuma hatası log açıkken merkezi hata/SQL izi üretmeli, eski SQL tablosuna gitmemeli.
5. Hatalı SMTP adresi, erişilemeyen SMTP ve boş/geçersiz alıcıyla bildirim deneyin. İş kaydı korunmalı; kullanıcı işlemin kaydedildiğini ve bildirimin gönderilemediğini anlamalı. SMTP hatası açık loglamada `LeaveRequest.Notification` olarak bir kez kaydedilmeli; eski hata yayıncısı ve ham teknik kullanıcı mesajı olmamalı. Bu senaryoda SMTP parametre hatası ayrıca oluşturulursa bu farklı bir hata olayıdır.
6. Liste, ayrıntı veya bildirim hazırlamadaki personel/talep/dönem/hareket/tanım/iletişim SELECT'lerinde kontrollü hata oluşturun. Log açıkken her gerçek SELECT için tek merkezi SQL izi ve ilgili üst hata sınırında merkezi hata beklenir. Aynı SQL ikinci bir servis katmanında tekrar izlenmemeli. Hatalı bildirim hazırlığı commit olmuş işi geri almamalı.
7. Log kapalıyken aynı başarı/hata senaryolarında iş sonucu ve transaction kapsamı değişmemeli; bu iki ekranın kapsanan uygulama yollarından `Olay_Table` ve `SQLLog_Table` tablosuna yeni kayıt oluşmamalı. Eşzamanlı başka ekranların eski yazılarını ayrı değerlendirin. Log açıkken kullanıcı hareketi ve audit sayıları önceki parçaya göre artmamalı; rollback halinde audit/iş verisi birlikte geri alınmalı, teknik kayıtların bağımsız kalması sürmeli.
8. Ortak context repository'lerini kullanan önceki IKYS personel/talep/hareket ekranlarında kısa okuma ve kayıt regresyonu yapın. Açık loglamada sorgu sonuçları korunmalı ve SQL izleri çift oluşmamalı. İş yazısı/audit hatasında önceki rollback davranışı korunmalı.

## Kalan kapsam

Bu parça iki talep listesinin önceki belgede belirtilen okuma/bildirim açıklarını kapatır. Bütün IKYS veya solution için sıfır eski yazı kapanışı değildir. `IzinDonemListesiWP`, mahsup, toplu izin ve diğer bağlamsız çağıranların taşınması sürmelidir. Başka bildirim yardımcıları, bağlamsız API'ler, eski posta/hata metotları ve ortak eski yazıcılar hâlâ kullanılmaktadır; solution genelindeki son bağımlılık taramasından sonra kaldırılabilir.

Eski tabloların bağımlılık/harici rapor envanteri, arşivleme ve sonradan kaldırma önerisi tüm proje geçişleri kapandıktan sonra ayrıca tamamlanmalıdır; bu parça tablo veya geçmiş veri silmez.

# PortalApps loglama geçişi

## Sıra ve kapsam

Proje sırası `IKYS → TBYS → NBYS → Portal → MTS → MFYS` şeklindedir. Bir proje
tamamlanıp Test sunucusunda yayımlandıktan ve kullanım sonucu değerlendirildikten
sonra sıradaki projeye geçilir. İlk ortak altyapı parçası mevcut WebPart çağrılarını
değiştirmez. Yeni günlükleme, host `web.config` dosyasında açıkça etkinleştirilene
kadar kapalıdır.

## Ortak olay sözleşmesi

`PortalLogContext`, aynı HTTP isteğindeki WebPart olaylarına ortak bir `OperationId`
verir. Proje, WebPart, SharePoint kullanıcı girişi ve sorgu parametrelerini içermeyen
sayfa yolu da bağlamda tutulur. Arka plan işleri bağlamı açıkça oluşturur.
`PortalLogRecord`, UTC zaman, olay türü, ad, sonuç ve isteğe bağlı süreyi taşır.

`PortalLogging.EnabledWebParts` ayarı noktalı virgülle ayrılmış `Proje:WebPart`
değerleri alır. Böylece bir projede birkaç WebPart yeni düzene geçerken diğerleri
eski düzende kalabilir. Proje bütünüyle taşındığında `PortalLogging.EnabledProjects`
ayarına proje adı eklenir. Her iki ayarın varsayılanı boştur. Örnek:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:EgitimGirisiWP;IKYS:YoklamaListesiWP" />
<add key="PortalLogging.EnabledProjects" value="" />
```

Bu ayarlar SharePoint uygulamasının `web.config` dosyasına uygulanır; sınıf
kütüphanesindeki `app.config` çalışma zamanı ayarı değildir.

## Dört kayıt akışı

| Akış | Kapsam | Hedef |
| --- | --- | --- |
| Kullanıcı etkinliği | İlk WebPart açılışı; kullanıcı, WebPart, zaman, OperationId | Ayrı etkinlik tablosu |
| İşlem denetimi | Başarılı ekleme, güncelleme, silme; entity ve kayıt kimliği | İş verisiyle aynı SQL transaction içindeki denetim tablosu |
| Hata | Exception ve bağlam | SharePoint ULS |
| SQL izi | Her SELECT, INSERT, UPDATE, DELETE; sorgu adı, süre, sonuç | Sınırlı pilotta ULS; proje genelinde açmadan önce dönen dosya/merkezi toplama hedefi değerlendirilecek |

İlk parça yalnızca sözleşmeyi, etkinleştirmeyi ve teknik kayıtların ULS yazıcısını
ekler. Kullanıcı etkinliği/denetim tabloları, SQL çağrı noktaları ve WebPart
entegrasyonu sonraki parçalardadır. Bu nedenle ilk parça tek başına yeni kayıt
üretmez.

İkinci parça `PortalActivityLog_Table`, `PortalAuditLog_Table` ve ileride yalnızca
izin verilen alan değişikliklerinin yazılacağı `PortalAuditChange_Table` şemasını
ve repository yazıcısını ekler. SQL betiği
`Database/Migrations/20261017_PortalLoggingActivityAudit.sql` dosyasındadır.
Veritabanına uygulanmadan yeni kayıtları etkinleştirmeyin. Bu parça da mevcut
WebPart'ları otomatik olarak yeni düzene geçirmez.

`TryRecordWebPartOpened` yalnızca kullanıcı arayüzündeki ilk açılış akışında
çağrılmalıdır; log yazma hatası sayfayı durdurmaz ve ULS'ye hata izi göndermeyi
dener. `RecordAudit`, `Complete()` çağrısından önce aynı `SqlTransactionContext`
ile çağrılır. Denetim yazma hatası iş işlemini başarısız kılar ve rollback'e gider.
EntityId olarak T.C. kimlik numarası gibi doğal kişisel anahtarlar değil, uygulama
kayıt kimliği kullanılır. Alanların eski/yeni değerleri IKYS geçişinde modül bazlı
izin listesi hazırlanıp gözden geçirilmeden kaydedilmez.

## IKYS ilk dikey geçişi: Resmî Tatil

`ResmiTatilGirisiWP`, `ResmiTatilListesiWP` ve `ResmiTatillerWP` ilk açılışta
etkinlik kaydı üretir. Bu üç WebPart'ın Resmî Tatil okuma çağrıları SELECT izini;
giriş WebPart'ının ekleme, güncelleme ve silme çağrıları SQL izini üretir.
Giriş, güncelleme ve silme sırasında iş verisi ile denetim başlığı/alan değişiklikleri
aynı transaction içinde tamamlanır. Resmî Tatil alanları için izin listesi:
`Gun`, `Ay`, `Yil`, `Tatil`, `BaslamaTarihi`, `BitisTarihi`, `IlanTarihi`,
`IptalTarihi`. Güncellemede yalnızca değişen alanlar saklanır. Olay zamanı
UTC olarak saklanır; tatil tarihleri iş verisindeki biçimiyle kaydedilir. SQL izine sorgu metni ve parametre
değerleri yazılmaz. Etkin olmayan WebPart'lar eski servis çağrılarını sürdürür.

Tüm yıllar için geçerli Resmî Tatil kayıtlarında kullanılan 1800 yılı başlangıç
ve bitiş tarihleri, yalnızca bu entity'nin yeni transaction yolunda SQL'e tarih
olarak gönderilir. Ortak `CrudQueryBuilder`'ın diğer alanlar ve eski çağrılar için
1901 öncesi tarihi `NULL` sayma davranışı korunur. Bu iki tarih de denetim
ayrıntısına yazılır. Önceden `NULL` kaydedilmiş tatil tarihleri otomatik olarak
geri doldurulmaz; ilgili kayıtlar ayrıca incelenmelidir.

Bu pilot için önce `Database/Migrations/20261017_PortalLoggingActivityAudit.sql`
uygulanır. Ardından host `web.config` içinde yalnızca seçilen WebPart'lar
etkinleştirilir; `EnabledProjects=IKYS` henüz kullanılmaz. Örnek:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP" />
```

Pilot denetiminde açılış zamanı/kullanıcı, üç yazma işleminin denetim kayıtları,
alan değişiklikleri, SELECT dahil SQL izleri ve hata başvuru kodu kontrol edilir.
Başarısız denetim yazması iş işlemini geri alır; etkinlik yazma hatası sayfayı
engellemez. ULS SQL izi hacmi yalnızca bu dar pilotta izlenir.

## IKYS ikinci dikey geçişi: Birim Tanımı

`BirimTanimEditWP` ve `BirimListesiWP` ilk açılışta kullanıcı etkinliği
kaydeder. Düzenleme ekranındaki birim ve amir listeleri, kayıt okuması ve
liste/Excel verisi için SELECT izi yazılır. Birim ekleme ve güncellemede iş
verisi, denetim başlığı ve değişen alanlar aynı transaction ile kaydedilir.
Denetim alanları: `Adi`, `KisaAdi`, `ParentId`, `AmirId`, `Sira`, `Aktif`,
`BirimKaldirildi`, `BolgeId`. Sorgu metni ve parametre değerleri SQL izine
girmez. Mevcut silme düğmesinde işlev bulunmadığından bu ekranda DELETE
işlemi oluşmaz; servis/repository transaction yolu ilerideki çağrılar için
hazırdır. `BirimSemasiWP` bu parçada etkinleştirilmez.

Mevcut `PortalActivityLog_Table`, `PortalAuditLog_Table` ve
`PortalAuditChange_Table` kullanılır; yeni veritabanı değişikliği yoktur.
Resmî Tatil pilotu da etkin kalacaksa host `web.config` içindeki aynı
`PortalLogging.EnabledWebParts` değerine bu iki ad eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP" />
```

Etkinleştirmeden önce Visual Studio incelemesi ve Test sunucusunda
yayımlama yapılır. Liste/açılış, ekleme, değiştirme, Excel, audit alanları,
ULS SELECT/yazma izleri ve hata başvuru kodu izlenir.

## IKYS üçüncü dikey geçişi: Görev/Kadro Tanımı

`GorevTanimEditWP` ve `GorevTanimListesiWP` ilk açılışta etkinlik kaydeder.
Formun görev, birim ve personel okumaları ile liste ve Excel sorguları SELECT
izi üretir. Ekleme, güncelleme ve silme için iş kaydı ve denetim kaydı aynı
transaction içinde yazılır. Görev güncellemesi personel atamasını değiştirirse
`IsBilgileri_Table.GorevId` değişiklikleri ve bunların ayrı audit kayıtları da
aynı transaction içindedir. Silmeden önce görevde personel bulunup bulunmadığı
transaction içinde tekrar kontrol edilir. Görev alan izin listesi: `BirimId`,
`PersonelId`, `Adi`, `KisaAdi`, `Vekil`, `Aktif`, `HarcirahGrupId`.

Yeni tablo veya migration gerekmez. Önceki pilotlar etkin kalacaksa host
`web.config` içindeki mevcut değere bu iki WebPart adı eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP;IKYS:GorevTanimEditWP;IKYS:GorevTanimListesiWP" />
```

Visual Studio incelemesinden ve Test sunucusu yayımlamasından sonra liste,
Excel, ekleme, personel ataması değişen güncelleme, boş/dolu görev silme,
audit alanları, ULS SQL izleri ve hata başvuru kodu kontrol edilir.

## IKYS dördüncü dikey geçişi: Birim ve Görev Şemaları

`BirimSemasiWP` ve `GorevSemasiWP` ilk açılışta kullanıcı etkinliği kaydeder.
Bu ekranlar yalnızca okuma yapar; bu nedenle audit INSERT/UPDATE/DELETE kaydı
üretmezler. Etkinleştirilen yolda birim, yönetici, aktif personel ve görev
verileri toplu sorgularla alınır. Böylece her birim veya personel için ayrı
SELECT çalıştırılıp SQL izi hacmi büyütülmez. Sorgu adı, süre ve sonuç durumu
ULS'ye yazılır; sorgu metni ve kişisel alan değerleri yazılmaz. Etkin olmayan
webpart'ların eski okuma akışı korunur.

Yeni tablo veya migration gerekmez. Önceki pilotlar etkin kalacaksa host
`web.config` içindeki mevcut değere bu iki WebPart adı eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP;IKYS:GorevTanimEditWP;IKYS:GorevTanimListesiWP;IKYS:BirimSemasiWP;IKYS:GorevSemasiWP" />
```

Visual Studio incelemesi ve Test sunucusu yayımlamasından sonra iki şemadaki
birim hiyerarşisi, yönetici adları, aktif personel ve görev düğümleri kontrol
edilir. ULS'de açılış, dört civarı toplu SELECT ve olası hata başvuru kodu
izlenir. Özellikle görev düğümleri önceki akışta personel kimliği eşlemesine
bağlı olduğundan görsel karşılaştırma yapılır.

## IKYS beşinci dikey geçişi: Aile Bilgileri

`AileGirisiWP` ilk açılışta kullanıcı etkinliği kaydeder. Personel, aile ve
meslek okumaları SELECT izi üretir; meslek listesi aynı HTTP isteğinde bir kez
okunur. Ekleme ve silmede iş kaydı ile audit aynı transaction içinde yazılır.
Servis/repository katmanında güncelleme için de transaction ve audit yolu
hazırdır; bu WebPart güncelleme yapmaz.

Aile verisi kişisel bilgi içerdiğinden audit alan ayrıntısı yalnızca
`PersonelId`, `YakinlikDerecesi` ve `Meslek` kimlik/kodlarıyla sınırlıdır.
Ad, soyad, T.C. kimlik numarası, telefon, doğum tarihi, okul ve diğer serbest
metin değerleri audit ve SQL izine yazılmaz. Audit başlığındaki `EntityId`
uygulamanın `Aile_Table.Id` değeridir. Hata kaydı istisna içeriğini ULS'ye
yazdığı için uygulama hatası metinlerine kişisel girdi eklenmemelidir.

Yeni tablo veya migration gerekmez. Önceki pilotlar etkin kalacaksa host
`web.config` içindeki mevcut değere `IKYS:AileGirisiWP` eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP;IKYS:GorevTanimEditWP;IKYS:GorevTanimListesiWP;IKYS:BirimSemasiWP;IKYS:GorevSemasiWP;IKYS:AileGirisiWP" />
```

Visual Studio incelemesi ve Test sunucusu yayımlamasından sonra açılış,
yakın ekleme/silme, aile tablosunun yeniden çizilmesi, audit alan izin listesi,
SELECT/yazma izleri ve hata başvuru kodu kontrol edilir.

## IKYS altıncı dikey geçişi: Eğitim Kayıtları

`EgitimGirisiWP` üzerindeki okul, kurs, iş tecrübesi ve yabancı dil bölümleri
birlikte geçer. İlk açılış kullanıcı etkinliği kaydeder. Personel, dört kayıt
türü ve eğitim seviyesi okumaları SELECT izi üretir. Eğitim seviyesi listesi
aynı HTTP isteğinde bir kez okunur; okul satırı başına sorgu çalışmaz.
Ekleme ve silme işlemlerinde iş kaydı ile audit aynı transaction içindedir.
Bu ekran güncelleme yapmaz; dört servis/repository için güncellemenin
transaction ve audit yolu da hazırlanmıştır.

Audit alan ayrıntısı `PersonelId` ile sınırlı ve izin verilen kod/tarih
alanlarını kapsar: eğitimde `Seviye`, `MezuniyetTar`; kursta `Tarih`;
iş tecrübesinde `BasTar`, `BitTar`; yabancı dilde `Dil`, `SinavTarihi`.
Okul, kurs, işyeri, kurum, sınav adı/notu, adres ve açıklama gibi serbest
metinler audit ve SQL izine yazılmaz. `EntityId` ilgili tablonun uygulama
kayıt kimliğidir. DAO'nun 1901 öncesi tarihleri `NULL` yazma davranışı audit
ayrıntısına da uygulanır. Hata kaydı istisna içeriğini ULS'ye yazdığı için uygulama
hatası metinlerine kişisel girdi eklenmemelidir.

Yeni tablo veya migration gerekmez. Önceki pilotlar etkin kalacaksa host
`web.config` içindeki mevcut değere `IKYS:EgitimGirisiWP` eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP;IKYS:GorevTanimEditWP;IKYS:GorevTanimListesiWP;IKYS:BirimSemasiWP;IKYS:GorevSemasiWP;IKYS:AileGirisiWP;IKYS:EgitimGirisiWP" />
```

Visual Studio incelemesi ve Test sunucusu yayımlamasından sonra dört
bölümün açılışı, her türden ekleme/silme, tablo yenilenmesi, audit alanları,
ULS SELECT/yazma izleri ve hata başvuru kodu kontrol edilir.

## IKYS yedinci dikey geçişi: Personel görüntüleme

`PersonelKartiWP` ve `IletisimBilgileriListesiWP` ilk açılışlarında kullanıcı
etkinliği kaydeder. Personel kartındaki personel, kimlik, iş, görev, aile,
iletişim, eğitim, iş tecrübesi, yabancı dil, ilçe ve meslek okumaları; iletişim
listesindeki birim ve personel okumaları SELECT izi üretir. Ekranlar veri
yazmadığı için işlem denetimi kaydı oluşturmaz. Kartın meslek adları tek bir
liste okumasıyla, iletişim listesinin alt birimleri tek bir birim listesi
okumasıyla bulunur; satır/birim başına tekrar sorgu çalışmaz.

SQL izine sorgu metni, parametreler, personel kimliği, iletişim bilgileri veya
sonuç satırları yazılmaz. Etkin olmayan WebPart'larda eski kayıt yolu devam
eder. Mevcut log tabloları yeterlidir; yeni migration gerekmez. Önceki
pilotlar etkin kalacaksa host `web.config` değerine bu iki WebPart eklenir:

```xml
<add key="PortalLogging.EnabledWebParts" value="IKYS:ResmiTatilGirisiWP;IKYS:ResmiTatilListesiWP;IKYS:ResmiTatillerWP;IKYS:BirimTanimEditWP;IKYS:BirimListesiWP;IKYS:GorevTanimEditWP;IKYS:GorevTanimListesiWP;IKYS:BirimSemasiWP;IKYS:GorevSemasiWP;IKYS:AileGirisiWP;IKYS:EgitimGirisiWP;IKYS:PersonelKartiWP;IKYS:IletisimBilgileriListesiWP" />
```

Visual Studio incelemesi ve Test sunucusu yayımlamasından sonra personel
kartının kendisi ve Excel çıktısı, iletişim listesinin birim filtresi ve Excel
çıktısı, açılış kayıtları, ULS SELECT izleri ve hata başvuru kodu kontrol edilir.

## IKYS sekizinci dikey geçişi: Personel listeleri

`PersonelListesiWP` ve `EskiPersonelListesiWP` ilk açılışlarında kullanıcı
etkinliği kaydeder. Aktif ve eski personel liste/Excel sorguları etkin akışta
SELECT izi üretir; ekranlar veri yazmadığı için audit kaydı oluşturmaz.
Etkin olmayan WebPart'lar mevcut sorgu ve hata akışını kullanmaya devam eder.
Yeni migration gerekmez. Host `web.config` içinde önceki değer korunarak
`IKYS:PersonelListesiWP;IKYS:EskiPersonelListesiWP` eklenir.

Test sunucusunda liste açılışı, personel tipi filtresi, eski personel listesi,
iki Excel çıktısı, açılış kayıtları, SELECT izleri ve hata başvuru kodu kontrol
edilir.

## IKYS dokuzuncu dikey geçişi: İzin raporları

`IzinHareketListesiWP`, `IzinKullanimRaporuWP` ve
`KullanilmayanIzinListesiWP` birlikte taşınır. Açılış etkinliği, izin dönemi ve
izin hareketi okuma yolları ile rapor filtreleri için SELECT izi eklenir.
Hata durumlarında Türkçe başvuru kodu gösterilir; etkin olmayan WebPart'lar
eski sorgu yolunu kullanır. Bu parçada iş verisi yazılmaz ve audit kaydı
oluşturulmaz. İzin talebi ve onay ekranlarının INSERT/UPDATE akışları ayrı bir
transaction ve audit parçasında ele alınacaktır.

Yeni migration gerekmez. Önceki değer korunarak host `web.config` içine
`IKYS:IzinHareketListesiWP;IKYS:IzinKullanimRaporuWP;IKYS:KullanilmayanIzinListesiWP`
eklenir. Testte üç ekranın açılışı, personel/yıl/izin filtresi, rapor sonuçları,
Excel çıktıları, SELECT izleri ve hata başvuru kodları kontrol edilir.

## IKYS onuncu dikey geçişi: Personel giriş ekranları

`PersonelGirisiWP` ve `EskiPersonelWP` aynı personel, kimlik, iş ve iletişim
akışının iki yüzüdür. Bu parçada ilk açılış kullanıcı etkinliği ile WebPart
seviyesinde Türkçe hata ve başvuru kodu kaydı hazırlanır. Çok sayıdaki entity
okuma/yazmasının servis ve repository transaction yolları, WebPart etkinliğe
alınmadan önce bu parçanın devamında birlikte tamamlanacaktır. Yeni migration
gerekmez.

Personel girişleri devam parçasında `Personel` ve `Kimlik` servis/repository
katmanlarına transaction-aware `INSERT/UPDATE` yolları eklendi. İş kaydı ile
audit aynı transaction içindedir; kimlik numarası, ad, soyad ve adres gibi
hassas/serbest alanlar audit ayrıntısına yazılmaz. Bu servis yolları WebPart’a
bağlanıp diğer personel entity’leri tamamlanmadan etkinleştirilmemelidir.

Personel girişleri devam parçasının kapsamı `IsBilgileri` ve
`IletisimBilgileri` entity’lerini de içerir. Bu iki entity için repository
katmanında transaction-aware `SELECT/INSERT/UPDATE/DELETE` ve SQL izleri,
service katmanında aynı transaction içinde audit başlığı ve güvenli alan
değişiklikleri eklendi. İletişim adresi, telefon, e-posta ve plaka gibi hassas
ve serbest metin alanları audit ayrıntısına yazılmaz; iş bilgilerinde yalnızca
personel, unvan, görev, birim ve çalışma durumu kimlikleri tutulur. WebPart
akışlarının bu context overload’larına bağlanması ve etkinleştirme ayrı bir
inceleme adımıdır.

Sonraki genişletmede `DereceKademeDegisim` ve `Mahsup` için de transaction-aware
CRUD yolları eklendi. Personel ekranlarının derece-kademe ve mahsup okumaları
SQL izi üretir; yazma yolları etkinleştirildiğinde iş kaydı ile audit aynı
transaction içinde tamamlanır. Mahsup audit ayrıntısı izin ve dönem kimlikleriyle
sınırlıdır; açıklama gibi serbest metin alanları yazılmaz.

Bu parçanın devamında `MaasHareket` için tarih bazlı personel ekranı okumaları
ve toplu silme yolu da SQL izi üretir. Maaş tutarı, ad, soyad ve unvan gibi
hassas veya serbest metin alanları audit değişiklik ayrıntısına yazılmaz;
personel, tarih, derece, kademe, grup ve protokol kimlikleriyle sınırlı kayıt
tutulur. Toplu silme işlemi de etkin akışta aynı transaction içinde audit
başlığıyla tamamlanır.

Bu geniş parçada `UnvanTanim`, `TahsilTanim` ve `BulunmamaSebebi` yardımcı
entity’leri de transaction-aware CRUD yollarına alındı. Personel ve yoklama
ekranlarının bu listeleri context ile okuması halinde SELECT izi üretilecek;
etkin yazma yollarında audit aynı transaction içinde tutulacaktır. Audit
ayrıntısı unvan kısa adı/adı, tahsil durumu ve bulunmama sebebi adıyla sınırlıdır.

## IKYS ücret tabloları ve izin/ücret tanımı altyapısı

`IzinTanim` ve `UcretTanim` servis/repository katmanlarında context destekli
okumalar ve transaction-aware ekleme, güncelleme, silme yolları hazırlandı.
İzin tanımı audit ayrıntısı `Adi`; ücret tanımı ayrıntısı `GrupId`, `Derece`,
`Kademe`, `BaslangicTarihi` ve `BitisTarihi` alanlarıyla sınırlıdır.
Ücret tutarları ve AGİ audit ayrıntısına yazılmaz. Ücret grubu silinirken eski
satırlar kilitlenerek aynı transaction içinde okunur; grup silme ve her satırın
gerçek `Id` değeriyle oluşturulan denetim kayıtları birlikte tamamlanır.
Bu yeni yazma overload'ları henüz maaş oluşturma/artış ekranlarına bağlanmadı.

`MaasTablolariWP` açılış etkinliği, SELECT izleri ve Türkçe hata başvuru koduyla
yeni yapıya bağlandı. Etkin akışta seçilen ücret grubu istek başına tek sorguyla
alınır; iki tablo aynı listeyi kullanır. Tarih filtresi ve Excel çıktıları da
bu yolu kullanır. Tablo tekrar hazırlanırken başlıklar korunup veri satırları
yenilenir. Bu ekran yalnızca okur; audit yazma kaydı üretmez.

Yeni migration gerekmez. Visual Studio incelemesi ve Test sunucusu yayımından
sonra host `web.config` içindeki mevcut `PortalLogging.EnabledWebParts`
değerine `IKYS:MaasTablolariWP` eklenebilir. Proje genelinde etkinleştirme
henüz yapılmamalıdır.

Kullanıcı kontrolünde ekran açılışı, tarih/grup seçimi, iki ücret tablosunun
eski çıktıyla karşılaştırılması ve Excel indirme incelenir. Etkinlik tablosunda
açılış; ULS'de grup listesi ve seçili grubun SELECT izleri beklenir. Boş grup
listesinde hata oluşmamalı; tutarlar ve kişisel bilgiler SQL izinde bulunmamalıdır.
Yeni servis yazma yollarının audit/rollback kontrolü, bu yolları çağıran yazma
ekranları taşındığında yapılacaktır.

## Güvenilirlik ve gizlilik

- SQL izinde sorgu adı veya sabit kimliği kullanılır; tam SQL, parametre değerleri
  ve sonuç satırları yazılmaz. Entity nesnesi doğrudan loga serileştirilmez.
- Bir `SqlTransactionContext` parametresi alan yolun denetim kaydı aynı context ile
  yazılır. Geri alınan işlemin denetim kaydı da geri alınır; hata ULS'de kalır.
- Teknik log yazma hatası asıl iş hatasını değiştirmez. ULS yazıcısı tüm hataları
  yakalar. Olay kaybını görünür kılacak sağlık kaydı ayrıca eklenecektir.
- Kullanıcı mesajını `MessageHelper` gösterir. İşlemin sonucu veya exception,
  mesajdan bağımsız olarak aynı `OperationId` ile kaydedilir.
- Etkinleştirilmiş her SQL çağrısı için iz üretilir; örnekleme yapılmaz. ULS
  yazıcısı yalnızca sınırlı pilotta kullanılır. Proje genelinde SQL izini açmadan
  önce hacme uygun ayrı hedef ve başarısız yazma davranışı kararlaştırılır.

## Her proje için tamamlanma kapısı

Entity, repository, service ve WebPart yolları birlikte taranır. Açılış, başarılı
işlem, rollback, exception ve dört SQL komutu doğrulanır; eski projelerde davranış
korunur. Kullanıcı Windows 11 Visual Studio 2026 incelemesi ile Test sunucusundaki
yayımlama ve testi yapar. Codex Windows 11'de test, build veya debug çalıştırmaz.
Son projeden sonra kullanılmayan eski kod ve tablolar, tarihsel veri ve okuyan
ekranlar doğrulanarak temizlenir.
## IzinTalep geçişi

`IzinTalepRepository` ve `IzinTalepService` için yeni loglama bağlamı desteklendi. Okuma işlemleri SQL izi üretir; ekleme, güncelleme ve silme işlemleri `SqlTransactionContext` ile aynı işlem içinde denetim kaydı ve güvenli alan değişiklikleriyle birlikte yazılır. Denetim ayrıntısında personel, izin türü/dönemi, başlangıç-bitiş tarihi, süre, onay durumu ve aktiflik alanları tutulur; adres ve açıklama gibi serbest metin alanları kaydedilmez.

İzin talep girişi, ücretli izin talep listesi ve mazeret izin talep listesi webpartları yeni bağlama geçirildi. Webpart açılışı `PortalActivityLog_Table` tablosuna, IzinTalep SQL işlemleri `PortalTechnicalLog` kaydına gider. Ayar kapalıysa mevcut eski servis davranışı korunur. Bu webpartlarda IzinHareket ve IsBilgileri ile birlikte yürüyen çoklu işlemlerin tek ortak transaction altında birleştirilmesi sonraki parçada ele alınacaktır.

## Yoklama ve Harcirah geçişi

`YoklamaRepository/Service` ile `HarcirahRepository/Service` için bağlamlı okuma, SQL izi ve transaction-aware INSERT/UPDATE/DELETE metotları eklendi. Audit ayrıntıları kimlik, tarih, durum ve tutar gibi raporlama için gerekli alanlarla sınırlandırıldı; serbest açıklama ve adres alanları audit ayrıntısına alınmadı. Ayar kapalı olduğunda mevcut servis çağrıları çalışmaya devam eder.

Yoklama giriş/liste/aylık rapor ve harcırah giriş webpartları bu bağlamlı servis çağrılarına bağlandı. İlk açılışlar kullanıcı etkinliği olarak, okuma işlemleri SQL izi olarak, kayıt değişiklikleri ise audit olarak kaydedilir.

## GorevOnay geçişi

`GorevOnayRepository` ve `GorevOnayService` bağlamlı okuma, SQL izi ve transaction-aware kayıt değişikliği desteği kazandı. Görev onay girişi ile görevli/izinli personel listesi webpartları yeni servis çağrılarını kullanır. Audit ayrıntısı personel, tarih, amir onayı, seçilme ve ödeme durumlarıyla sınırlıdır.

## EskiPersonelGorevTanim geçişi

`EskiPersonelGorevTanimRepository` ve `EskiPersonelGorevTanimService` için bağlamlı okuma, SQL izi ve transaction-aware INSERT/UPDATE/DELETE desteği eklendi. Audit alanları birim, personel, görev adı, kısa ad, vekillik ve aktiflik bilgileriyle sınırlandırıldı. Aktif webpart çağrısı bulunmadığı için bu parçada yalnızca ortak servis ve repository katmanı taşındı.

## IzinHareket geçişi

`IzinHareketRepository` ve `IzinHareketService` için transaction-aware INSERT/UPDATE/DELETE, SQL izi ve alan bazlı audit desteği eklendi. `IzinHareketEditWP` açılış, okuma, güncelleme ve silme işlemlerinde yeni bağlamlı çağrıları kullanır. Audit ayrıntısı personel, izin türü/talep/dönem, tarih ve mahsup durumuyla sınırlıdır.

## İzin raporları ve dönem listeleri

`IzinDonemListesiWP`, `SonIzinDurumuWP` ve `TarihBazliIzinKullanimRaporuWP` webpartlarının açılış ve okuma akışları bağlamlı servis çağrılarına geçirildi. Bu parçada mevcut yazma akışları korunmuştur; IzinDonem ve IzinHareket birlikte güncellenen toplu işlemler sonraki transaction parçasında ele alınacaktır.

## Toplu izin ve mahsup webpartları

`TopluIzinGirisiWP`, `MazereteMahsupWP` ve `UcretliMahsupWP` açılışları yeni kullanıcı etkinliği kaydına bağlandı. IzinHareket ekleme, silme ve okuma çağrıları bağlamlı servis metotlarını kullanır. IzinDonem ve IzinHareket birlikte değişen mahsup işlemlerinin tek transaction altında birleştirilmesi sonraki güvenilirlik parçasıdır.

## Görev onayı listeleri

`GorevOnayListesiWP`, `GorevOnayListesiKisiselWP` ve `BekleyenIslemlerWP` için açılış, liste okuma, kayıt okuma ve onay güncelleme çağrıları yeni loglama bağlamına geçirildi. Toplu seçim/ödeme bayraklarını değiştiren eski yardımcı metotlar sonraki transaction-aware görev onayı parçasında ele alınacaktır.

## Derece, maaş ve protokol webpartları

`DereceKademeListesiWP`, `MaasArtisiWP`, `MaasOlusturmaWP` ve `ProtokolSirasiWP` webpartlarının açılış ve desteklenen okuma/yazma çağrıları yeni loglama bağlamına geçirildi. Maaş ve derece işlemlerinin aynı iş akışında birden fazla entity değiştirdiği bölümler sonraki ortak transaction çalışmasına bırakıldı.

## Kişisel sayfa webpartları

`KisiselSayfaWP` ve `KisiselSayfaListesiWP` açılışları kullanıcı etkinliği kaydına bağlandı. Personel, izin talebi ve görev onayı için mevcut bağlamlı servis çağrıları kullanıldı; kişisel sayfanın diğer legacy okuma akışları sonraki ortak okuma standardizasyonunda ele alınacaktır.

## IKYS exception standardizasyonu (pilot grup)

Bu parçada `AylikYoklamaRaporuWP`, `GorevliIzinliPersonelWP`, `TarihBazliIzinKullanimRaporuWP`, `SonIzinDurumuWP`, `YoklamaListesiWP`, `HarcirahGirisWP`, `UcretliMahsupWP` ve `MaasOlusturmaWP` içindeki yakalanan exception akışları ortak `ReportError` davranışına bağlandı. Loglama etkin olduğunda teknik kayıt `PortalTechnicalLog` üzerinden aynı `OperationId` ile yazılır ve kullanıcıya Türkçe, güvenli bir başvuru kodu mesajı gösterilir. Loglama kapalı olduğunda mevcut `ExceptionHelper`/ULS davranışı korunur.

Bu pilotta build, test ve debug çalıştırılmadı; doğrulama kaynak taraması, diff kontrolü ve UTF-8 kontrolüyle sınırlıdır. Kalan IKYS webpartlarındaki exception catch blokları sonraki parçalarda aynı standarda geçirilecektir. Exception standardizasyonu tamamlandıktan sonra çoklu entity transaction çalışmasına dönülecektir.
## IKYS exception standardizasyonu (kalan webpartlar)

Kalan 14 IKYS webpartındaki exception yakalama akışları da ortak `ReportError` davranışına geçirildi: `BekleyenIslemlerWP`, `DereceKademeListesiWP`, `GorevOnayGirisiWP`, `GorevOnayListesiKisiselWP`, `GorevOnayListesiWP`, `IzinDonemListesiWP`, `IzinHareketEditWP`, `KisiselSayfaListesiWP`, `KisiselSayfaWP`, `MaasArtisiWP`, `MazereteMahsupWP`, `ProtokolSirasiWP`, `TopluIzinGirisiWP` ve `YoklamaGirisiWP`. Böylece IKYS webpartlarında `ExceptionHelper` kullanılan dosyaların tamamında teknik loglama ve Türkçe kullanıcı mesajı için ortak yol bulunur. Loglama kapalıyken eski ULS fallback davranışı korunur.

Bu parça da yalnızca kaynak/diff/UTF-8 kontrolleriyle doğrulanmıştır. Visual Studio derleme ve Test sunucusu testi kullanıcı tarafından yapılacaktır.
## IzinDonem transaction altyapısı

`IzinDonemRepository` ve `IzinDonemService` için transaction-aware SELECT/INSERT/UPDATE/DELETE overloadları eklendi. Bu overloadlar aynı `SqlTransactionContext` üzerinden çalışır, SQL izini ve audit kaydını aynı transaction içinde üretir; transaction'ın tamamlanması çağıran çoklu entity akışına bırakılır. Böylece IzinHareket ve IzinTalep ile birlikte yürüyen izin dönemi değişiklikleri sonraki webpart parçasında tek transaction altında birleştirilebilir.
## IzinHareket transaction overloadları

`IzinHareketService` için dışarıdan verilen `SqlTransactionContext` ile çalışan `GetById`, `Save`, `Update` ve `Delete` overloadları eklendi. Bu yollar kendi transaction'ını tamamlamaz; IzinDonem ve IzinTalep değişikliklerini aynı işlem içinde yöneten üst servis/webpart akışına bırakır. Repository SQL izi ve audit kayıtları aynı transaction üzerinden devam eder.
## IzinDonemListesi webpart transaction kullanımı

`IzinDonemListesiWP` içindeki dönem ekleme ve güncelleme işlemleri, loglama etkin olduğunda `SqlTransactionContext` üzerinden çalışacak şekilde güncellendi. Başarılı işlemde transaction açıkça tamamlanır; hata veya başarısız sonuçta değişiklikler commit edilmez. Loglama kapalıyken mevcut legacy servis yolu korunur.
## IzinHareketEdit silme akışı

`IzinHareketEditWP` içinde mazeret ve ücretli izin hareketi silinirken kullanılan izin dönemi güncellemesi ile hareket silme işlemi aynı `SqlTransactionContext` altında birleştirildi. Dönem güncellemesi başarısız olursa hareket silinmez; başarılı iki işlemden sonra transaction tamamlanır. Loglama kapalıyken eski davranış korunur. İzin talebi kaydının ayrıca silinmesi sonraki çoklu entity adımında aynı transaction zincirine alınacaktır.
## Izin talebi silme zinciri

`IzinHareketEditWP` silme akışında ilişkili `IzinTalep` kaydı da aynı transaction içine alındı. Loglama etkin olduğunda dönem kullanımı güncellemesi, IzinHareket silme ve ilişkili IzinTalep silme işlemleri aynı context ile yürür; herhangi biri başarısız olursa tüm değişiklikler geri alınır. Loglama kapalıyken önceki bağımsız silme davranışı korunur.
## IzinDonem transaction-aware dönem oluşturma/güncelleme

Çoklu entity izin hareketi güncellemesine hazırlık olarak `IzinDonemService` için transaction-aware dönem oluşturma ve güncelleme yolları eklendi. İş bilgileri SELECT'i, dönem arama, dönem INSERT/UPDATE, SQL izi ve audit yazımı aynı `SqlTransactionContext` üzerinden yürür. Bu overloadlar transaction'ı tamamlamaz; üst akışın IzinHareket ve IzinTalep işlemleriyle birlikte commit etmesine bırakır.
## IzinHareketEdit güncelleme transaction bağlantısı

`IzinHareketEditWP` güncelleme akışı transaction-aware IzinDonem ve IzinHareket servis yollarına bağlandı. Dönem arama, gerekirse dönem oluşturma/güncelleme, kullanılan izin düşme/ekleme ve IzinHareket güncellemesi loglama açıkken aynı transaction içinde yürür; başarılı işlem sonunda tek commit yapılır. Legacy yol loglama kapalıyken korunur. İlişkili IzinTalep güncellemesinin aynı transaction zincirine alınması sonraki alt adımda tamamlanacaktır.
## İzin talebi güncellemesinin zincire alınması

`IzinHareketEditWP` güncelleme akışında ilişkili `IzinTalep` kaydının güncellenmesi de aynı transaction'a alındı. Böylece dönem kullanımı, IzinHareket ve IzinTalep değişiklikleri birlikte commit edilir; herhangi biri başarısız olursa tamamı geri alınır. `IzinTalepService` için transaction-aware `Update` overloadı eklendi.
## IzinTalepListesi onay transaction akışı

`IzinTalepListesiWP` onay akışı transaction-aware servis yollarına bağlandı. IzinTalep onay durumu güncellemesi, IzinHareket oluşturma/güncelleme, ücretli izin dönemindeki kullanılan izin değişikliği ve ücretsiz izin için IsBilgileri güncellemesi loglama açıkken aynı transaction içinde yürür. Onay işlemindeki exception akışları da teknik log ve Türkçe kullanıcı mesajı standardına bağlandı. Legacy servis yolu loglama kapalıyken korunur.
## IzinTalepListesi kabul ve red akışları

`IzinTalepListesiWP` içindeki kontrol edildi/kabul ve reddet işlemleri transaction-aware `IzinTalepService` çağrılarına geçirildi. Durum değişikliği, audit ve SQL izi aynı transaction üzerinden yazılır; başarılı güncelleme olmadan e-posta ve başarı mesajı gönderilmez. Exception akışları teknik log ve Türkçe kullanıcı mesajına bağlandı.
## Mahsup servislerinin transaction hazırlığı

`MahsupService` için dış transaction kullanan INSERT ve DELETE overloadları eklendi. `MazereteMahsupWP` ve `UcretliMahsupWP` içindeki Mahsup INSERT çağrıları bağlamlı servis yoluna geçirildi; audit ve SQL izi aynı OperationId ile üretilir. Mahsup, IzinHareket ve IzinDonem geri alma zincirinin tek transaction altında birleştirilmesi sonraki mahsup parçasında yapılacaktır.
## Mahsup ve toplu izin çoklu entity transaction akışları

`MazereteMahsupWP` ve `UcretliMahsupWP` içinde Mahsup, IzinHareket ve IzinDonem değişiklikleri loglama açıkken aynı `SqlTransactionContext` altında yürütülecek şekilde birleştirildi. Başarısız akışta transaction tamamlanmadığı için manuel geri alma yalnızca legacy yolda çalışır. `TopluIzinGirisiWP` içinde dönem arama/oluşturma/güncelleme, kullanılan izin değişikliği ve IzinHareket INSERT'i aynı transaction'a bağlandı. Loglama kapalıyken mevcut servis çağrıları korunur.
## Mazeret onayı ve izin talebi giriş transaction akışları

`MazeretIzinTalepListesiWP` onay akışında IzinTalep durumu, IzinHareket oluşturma/güncelleme ve mazeret izin dönemi kullanımı loglama açıkken aynı transaction altında yürütülür. `IzinTalepGirisWP` içinde yeni talep ve talep güncelleme sırasında dönem arama/oluşturma/güncelleme ile IzinTalep INSERT/UPDATE işlemleri transaction-aware servis yollarına bağlandı. Başarılı işlemden sonra e-posta ve kullanıcı mesajı akışları korunur; legacy yol loglama kapalıyken devam eder.
## Personel izin dönemi yardımcıları

`EskiPersonelWP` ve `PersonelGirisiWP` içindeki ücretli izin dönemi oluşturma/güncelleme yardımcıları transaction-aware `IzinDonemService` yollarına geçirildi. Loglama açıkken dönem SELECT ve INSERT/UPDATE işlemleri aynı transaction içinde SQL izi ve audit ile tamamlanır; loglama kapalıyken mevcut legacy yol korunur.
## Mazeret izin talebi karar hata logları

`MazeretIzinTalepListesiWP` kabul ve red kararlarındaki exception akışları teknik log ve Türkçe kullanıcı mesajı standardına bağlandı. Böylece onay, kabul ve red işlemleri aynı OperationId üzerinden izlenebilir hale geldi.

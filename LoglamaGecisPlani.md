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

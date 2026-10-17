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

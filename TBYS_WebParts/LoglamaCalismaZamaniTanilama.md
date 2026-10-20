# TBYS kayıt yazma tanılaması

Dal: `codex/tbys-runtime-log-diagnostics`.
Başlangıç: `6b3cfe4221c009145cfda8d7fe59b46cae20ca78`.
Değişiklik öncesinde master temizdi ve GitHub master ile aynı commit'teydi.

## Durum

Kullanıcı, SQL izi SELECT düzeltmesi ve bağımlılık paketleme düzeltmesi dağıtıldıktan sonra da yeni TBYS kayıtlarının oluşmadığını bildirdi. Kök neden henüz doğrulanmadı. İlk açılış kaydının çağrı yeri son SQL izi parçasında kaldırılmadı veya değiştirilmedi. Yerel kaynak incelemesi çalışan IIS sürecinin etkin ayarını, yüklü DLL'lerini veya veritabanındaki gerçek hatayı kanıtlamaz.

Bu parça, mevcut log yazarlarının sessizce döndüğü nedeni yalnızca isteğe bağlı yönetici tanılamasında görünür kılar. Logları zorla etkinleştirmez, yeni tablo/ULS/dosya logu eklemez ve tanılama için ek SQL komutu yürütmez. İşlevsel bir kök neden düzeltmesi olarak sunulmaz.

## TEST'te kullanım

Kullanıcı build ve deploy yaptıktan sonra siteyi yönetme yetkisi olan hesapla YasalFaiz ekranının mevcut adresine şu parametre eklenir:

```text
?tbysLogDiagnostics=1
```

Adreste zaten sorgu parametresi varsa `&tbysLogDiagnostics=1` kullanılır. Bu parametre ile sayfa ilk GET isteği olarak açılır. Ardından aynı sayfada kayıt güncellenir. Sayfanın altındaki **TBYS log tanılaması — TBYS-LOG-DIAG-1** panelinin hem ilk açılış hem kaydetme isteği için görüntüsü paylaşılır. Panel yalnızca o isteğin sonuçlarını içerir.

Panel yoksa yeni tanılama kodunun çalışan WebPart'ta bulunup bulunmadığı, URL parametresi ve ManageWeb yetkisi kontrol edilir. Panel query parametresi olmadan veya yetkisiz kullanıcıda gösterilmez.

## Panelin gösterdikleri

- Etkin uygulama yolu, proje/WebPart, OperationId ve isteğin ilk GET mi postback mi olduğu.
- PortalLogSettings.IsEnabled sonucunun Etkin/Kapalı değeri ve bu sonucun hesaplandığı ConfigurationManager.AppSettings değerleri.
- DAO bağlantısının ve teknik log bağlantısının hedef veritabanı adları. Bağlantı dizesi, sunucu adresi veya parola gösterilmez.
- Çalışan WebPart, Model, DAO ve Utility assembly'lerinin adı, sürümü ve derleme kimliği (MVID). Sürüm 1.0.0.0 olarak sabit kalsa da MVID farklı derlemeleri ayırmaya yardımcı olur.
- O istekte açılış/hata/SQL izi yazma sonucunun ilk 50 kaydı. Açılış yazmasında ExecuteNonQuery satır sayısı; hata halinde güvenli Türkçe sınıflandırma ve varsa SQL hata numarası.

Ham exception.Message, stack trace, sorgu veya parametre değerleri panelde gösterilmez. Dinamik metinler HTML encode edilir. Sonuçlar yalnızca yetki kontrolüyle etkinleştirilmiş HttpContext.Items içinde, istek süresince tutulur; başka isteklere aktarılmaz.

## Sonucun yorumu

- **Loglama kapalı / yazma denenmedi:** Çalışan uygulamanın etkin EnabledProjects/EnabledWebParts değeri beklenen bağlamı kapsamıyor. Ekrandaki gerçek değer esas alınır.
- **SQL 229:** Uygulamanın kullandığı SQL bağlantısının yetkisi yetersiz.
- **SQL 208 / 207:** Hedef veritabanında beklenen tablo veya alan bulunmuyor.
- **SQL 547:** Veritabanı kısıtı kaydı reddediyor; ilgili şema incelenmeli.
- **SQL 8152 / 2628:** Bir alanın uzunluğu şema sınırını aşıyor.
- **Yazma komutu tamamlandı, sorguda kayıt yok:** Paneldeki hedef veritabanı ile kontrol sorgusunun veritabanı karşılaştırılır. Açılış kaydının satır sayısı da değerlendirilir.
- **Tanılama sonucu yok:** İstek türü, etkin assembly kimlikleri ve çağrı yolu incelenir. Postback'in yeni WebPartOpened üretmemesi normaldir; oran kayıt/okuma işlemlerinin SQL izi durumu ayrıca görülmelidir.

## İnceleme ve sınırlar

YasalFaizWP'ye tanılama bağlantısı, TBYS'ye yönetici paneli ve ortak log yazarlarına sonuç gözlem noktaları eklendi. Ortak yazıcıların kayıt hatalarında iş akışını sürdürme davranışı korundu; yeni gözlemci kendi hatalarını da dışarı taşımaz. Diğer projelerin kaynaklarına ve üretilmiş dosyalara dokunulmadı. Kaynak dosyaları mevcut proje dosyalarına eklendi.

Codex build/test/debug çalıştırmadı. Statik incelemede yetki/URL kapısı, HTML kodlama, parola/ham hata metni göstermeme, proje kaynak kayıtları ve git diff --check kontrol edildi. Canlı neden, kullanıcının paylaşacağı tanılama sonucuyla kesinleştirilecek. Sorun çözüldükten sonra geçici tanılama kodu ayrı inceleme parçasıyla kaldırılabilir.

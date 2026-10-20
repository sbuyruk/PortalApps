# SQL izi SELECT kısıt düzeltmesi

Dal: `codex/tbys-sql-trace-select-fix`.
Başlangıç commit'i: `784fd573d5db724ce44c9082f6affb9773cd4d00`.
Kod değişikliği öncesinde master temiz ve GitHub master ile aynı commit'teydi.

## Doğrulanan kod hatası

Ortak SQL izi yazıcısı SELECT/INSERT/UPDATE/DELETE değerini hem Operation hem SqlOperation alanlarına gönderiyordu. Birleşik şemada CK_PortalLog_Operation kısıtı SELECT'e izin vermiyor. Bu nedenle SELECT izi INSERT'i başarısız oluyor; teknik yazıcının iş akışını koruyan catch bloğu hatayı dışarı taşımıyor ve SQL izi kayboluyor.

Düzeltmede SELECT için Operation=NULL, SqlOperation=SELECT yazılır. INSERT/UPDATE/DELETE kayıtları önceki alan değerlerini korur. Şema değiştirilmez, mevcut iş verisine dokunulmaz. Ortak teknik log yazıcısındaki aynı SELECT sorunu diğer proje çağrıları için de giderilir; diğer projelerin kaynakları değiştirilmez.

Bu kusur yeni WebPartOpened kaydının veya UPDATE SQL izinin görünmemesini tek başına açıklamaz. Bunların çalışma zamanı nedeni henüz doğrulanmadı. Build/deploy yapıldığı ve TBYS etkinleştirme ayarının bulunduğu kullanıcı tarafından bildirildi.

## Kalan belirtiyi ayırma

1. YasalFaiz sayfasını adresinden yeniden açarak yeni ilk GET isteği oluşturun. Salt kaydetme postback'i yeni WebPartOpened üretmez.
2. Bir değeri güncelleyip kaydedin. Bu aşamada UPDATE kaydı SQL izleri bölümünde beklenir; audit dönüşümü henüz yapılmadığından Audit bölümünün boş olması beklenir.
3. LogDurumTakip sayfasını tarayıcının yenile düğmesiyle yeniden yükleyin. Ekran otomatik sorgulamıyor; metin filtreleri yalnızca zaten yüklenmiş satırları süzüyor. Başka sekmedeki işlem açık takip ekranına otomatik yansımaz.
4. Yeni kayıtlar görünmüyorsa Diagnostics/YasalFaizLogKontrol.sql dosyasını TEST uygulamasının kullandığı veritabanında çalıştırın. Sorgu yalnızca okur; çalıştırma kullanıcıya aittir.

Tabloda yeni UPDATE/açılış kayıtları var ama ekranda yoksa görüntüleme/yenileme/tarih filtresi yönü incelenir. Tabloda da yoksa kayıt üretimi, etkin çalışan assembly ve uygulama/veritabanı bağlantısı yönü incelenir. Ekran görüntüsündeki önceki kayıtlar yeni isteğin kayıt ürettiğini kanıtlamaz.

SELECT düzeltmesi sonrası beklenen kayıtlar: Kind=SqlTrace, Operation=NULL, SqlOperation=SELECT, doğru RepositoryMethod ve DurationMs. Aynı istekteki kayıtlar ortak OperationId taşır. Kaydetme isteği, ekranın ilk açılış isteğinden farklı OperationId taşır.

Codex build/test/debug çalıştırmadı. SQL kısıtı ile INSERT parametreleri statik olarak karşılaştırıldı; git diff --check yapıldı. Üretilmiş dosyalar değiştirilmedi. Canlı ortam nedenini kesinleştirmek için kullanıcı kontrol sonuçları bekleniyor.

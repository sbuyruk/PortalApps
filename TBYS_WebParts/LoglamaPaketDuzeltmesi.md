# TBYS bağımlılık paketleme düzeltmesi

Dal: `codex/tbys-package-project-outputs`.
Başlangıç: `9e38401cea2f9b466859b2ed8df390bcdace0162`.
Değişiklik öncesinde master temizdi ve GitHub master ile aynı commit'teydi.

TBYS paket tanımı DAO.dll ve Model.dll için sabit bin/Debug dosyalarını kullanıyordu. Özellikle Release build sonrasında bu dosyalar güncel proje çıktılarıyla aynı olmayabilir. Bu durumda yeni WebPart ile eski servis/repository DLL'lerinin birlikte paketlenmesi mümkündür.

Bu iki bağımlılık mevcut Utility.dll tanımı ve IKYS paketindeki yöntemle aynı biçimde projectOutputAssembly olarak tanımlandı. Paketleme aktif yapılandırmanın proje çıktısını kullanır. Diğer projelerin paketleri ve kaynakları değiştirilmedi.

Bu değişiklik paket tanımındaki eski DLL riskini giderir; TEST'teki log eksikliğinin kesin nedeninin bu olduğu henüz doğrulanmadı. Log yazıcılarının hataları dışarı taşımaması nedeniyle canlı hata nedeni yalnızca kaynak koddan belirlenemez.

Kullanıcı doğrulaması:

1. Mevcut dağıtımdaki veritabanı sonucunu ayırmak için Diagnostics/YasalFaizLogKontrol.sql sorgusunu TEST uygulamasının veritabanında çalıştırın. Son denemenin kayıtları varsa görüntüleme/yenileme tarafı; yoksa etkin ayarlar, çalışan assembly'ler ve kayıt yazma tarafı incelenir.
2. Paket düzeltmesinin build/package işlemleri kullanıcı tarafından Visual Studio 2026'da yapılır. Oluşan WSP içindeki DAO.dll ve Model.dll dosyalarının aktif yapılandırmanın güncel proje çıktılarıyla eşleştiği doğrulanır.
3. TEST dağıtımından sonra YasalFaiz yeni istekle açılır, kayıt güncellenir ve takip sayfası yenilenir. Yeni WebPartOpened, SELECT ve UPDATE kayıtları hem tabloda hem ekranda kontrol edilir.

Codex build/test/debug çalıştırmadı. Paket XML'i ve proje referansları statik olarak incelendi; git diff --check yapıldı. Üretilmiş dosyalar değiştirilmedi. SQL sorgu sonucu ve çalışma zamanı doğrulaması bekleniyor.

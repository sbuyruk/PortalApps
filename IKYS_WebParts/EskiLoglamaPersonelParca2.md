# IKYS eski loglamayı devreden çıkarma — personel ekranları, parça 2

Dal: `codex/ikys-personel-eski-log-yollarini-kaldir`

Başlangıçta yerel `master` temizdi; HEAD, yerel `master` ve canlı uzak `master` aynı commit'teydi: `6293420acfb3af848f2ee0afd6e92b0e9269b643` (ilk servis parçasının merge commit'i, PR #538). Kullanıcı commit/push/PR merge/pull işlemlerini tamamladığını ve devam için hazır olduğunu bildirdi. Bu bildirim çalışma zamanı testinin yapıldığı anlamına gelmez.

## Kapsam

- `PersonelGirisiWP` ve `EskiPersonelWP`: yeni personel kaydı, personel/kimlik, iş bilgileri, kadrosuz iş bilgileri ve iletişim güncellemeleri.
- Ekran açılışında eksik ücretli/mazeret izin döneminin otomatik oluşturulması.
- `PersonelService.GetById` için mevcut transaction üzerinden okuyan overload.
- `IzinDonemService` transaction yolları ve `IKYSOrtak` içinde aynı transaction'da okunan iş bilgileriyle çalışan saf izin hakkı hesabı.

## Davranış

İki ekranın yazma işlemleri artık `PortalLogSettings.IsEnabled` sonucuna göre iş akışı değiştirmez. Yeni personel, kimlik, iş bilgileri ve iletişim kayıtları tek `SqlTransactionContext` içindedir. Güncellemede personelin okunması da aynı transaction'da yapılır.

Personel/kimlik güncellemesi ile ona bağlı ücretli izin dönemi işlemi tek transaction'dadır. İş bilgileri, mevcut görev atama güncellemeleri ve ona bağlı ücretli izin dönemi işlemi de tek transaction'dadır. Bir adım başarısızsa `Complete` yapılmaz; kısmi kayıt başarı gibi gösterilmez. Görev atamasının mevcut iş kuralları korunmuştur. Kadrosuz iş bilgileri ve iletişim güncellemeleri de kendi transaction'larında yürür.

Bağlamsız sekiz eski özel yardımcı her ekrandan kaldırıldı. Güncelleme handler'ları ortak güvenli hata sınırından geçer; yeni teknik hata yazıcısı ayarı kapalıyken kendisi yazmayı atlar. `ExceptionHelper`/ULS geri dönüşü kaldırıldı. Yönlendirme nedeniyle oluşan `ThreadAbortException` uygulama hatası olarak kaydedilmez.

`PersonelGirisiWP` üzerindeki mevcut bildirim koşulları merkezi iş yoluna taşındı: yeni personel kaydı; birim/görev/çalışma durumu değişikliği; kadrosuz personelde birim/çalışma durumu değişikliği; boş olmayan e-posta/cep telefonu ekleme veya değiştirme. Bildirim yalnızca transaction'ın `Dispose` ile commit edilmesinden sonra çağrılır. E-posta transaction içine taşınmadı; rollback durumunda çağrılmaz. `EskiPersonelWP` için yeni bildirim eklenmedi.

Ekran açılırken izin dönemi bulma/oluşturma ortak bir yardımcı üzerinden merkezi transaction yolundadır. Önceden oluşturulan izin dönemi sonucu alınmayıp boş `IzinDonem` nesnesiyle devam edilebiliyordu; şimdi gerçek sonuç kullanılır. İşe/izin dönemine başlama tarihi yoksa mevcut servis davranışı gibi dönem oluşturulmaz.

## İnceleme sırasında kapatılan transaction açığı

`IzinDonemService.CreateForPersonel/UpdateForPersonel` transaction overload'ları, eski `IKYSOrtak.UcretliIzinHakkiHesapla` metoduna gidiyordu. Bu metot iş bilgilerini ayrı bağlantıyla okuyup hatayı yakalayarak varsayılan değerle devam edebiliyordu. Personel/iş bilgileri kilidi aynı transaction'da tutulurken bu yol kilit beklemesine de neden olabilir.

Transaction yolu artık `GetIzinBaslangic` içinde aynı transaction'da okunan `IsBilgileri` nesnesini yeni hesap overload'larına geçirir. Yeni hesap DB, log veya UI erişimi yapmaz; hesap hatasını yutmaz. İzin hakkı ve kalan izin aynı hesap sonucundan oluşturulur. İlk yıl, asker/sivil, prim günü ve yıl eşikleri mevcut formülle korunur: ilk yıl 0; sivilde 360 prim günü bir yıl; 5 yıl üstü ikinci eşik; 15 yıl ve üstü son eşik. İş kuralı değişikliği yapılmadı.

Bu düzeltme aynı transaction overload'larını kullanan diğer IKYS çağıranlarına da uygulanır. Bağlamsız legacy hesap API'leri diğer çağıranlar taşınana kadar duruyor.

## Statik kontroller

- `git diff --check`; değişen kaynakların sıkı UTF-8 okuması, BOM ve CRLF kontrolü.
- C# kaynaklarında string/yorumlar dışındaki ayraç dengesi; özel yardımcıların çağrı/imza eşleşmeleri.
- İki ekranın bütün iş verisi yazılarında transaction argümanı bulunduğu; eski log ayarı, nullable transaction seçimi ve eski hata yayınlama yolu kalmadığı kontrol edildi.
- Transaction parametreli yardımcıların servis/repository/audit çağrıları ve izin hesabının alt çağrı zinciri incelendi. Bildirim transaction parametreli yardımcıların dışındadır.
- Mevcut ASCX olay adları ve handler imzaları korundu. ASCX, `*.ascx.g.cs`, csproj, feature ve paket dosyaları değiştirilmedi. Yeni derlenecek kaynak dosyası eklenmedi.

Build, test ve debug çalıştırılmadı. Commit, push, PR, merge, yayın ve çalışma zamanı testleri kullanıcıya aittir.

## Kullanıcının uygulayacağı kabul senaryoları

Test ortamında kontrollü personel kayıtları kullanın. Aşağıdaki iş senaryolarını hem merkezi loglama açıkken hem kapalıyken tekrarlayın. Kapalı senaryoda hem `PortalLogging.EnabledProjects` içindeki IKYS girdisinin hem de ilgili `PortalLogging.EnabledWebParts` girdilerinin kapalı olduğundan emin olun; diğer proje ayarlarını koruyun.

1. Visual Studio 2026'da solution'ı derleyip Test sunucusuna yayımlayın. İki ekranın mevcut kayıt, yeni kayıt ve ilgili personel tipi görünümlerini açın. Butonların ASCX olay bağlantıları ve sekme seçimi çalışmalı.
2. Yeni personel kaydedin. Personel, kimlik, iş bilgileri ve iletişim satırları birlikte oluşmalı. Kontrollü olarak ikinci/son yazımda hata oluşturduğunuzda hiçbir satır veya audit başlığı/ayrıntısı kısmen kalmamalı. Başarısız kayıtta bildirim gönderilmemeli. Mevcut TC numarası kontrolü ve yönlendirme çalışmalı.
3. Personel ve kimlik bilgilerini birlikte güncelleyin. Kimlik bulunamazsa veya güncellenemezse personel güncellemesi de geri alınmalı; “eksik güncellendi” başarı/bilgi mesajı çıkmamalı. İzin dönemi yazımında kontrollü hata da personel/kimlik değişikliğini geri almalı.
4. Kadrolu iş bilgileri/görev değişikliğini deneyin. İş bilgileri, ilgili görev atamaları ve izin dönemi birlikte kaydedilmeli; sonraki adım hatasında hepsi geri alınmalı. Başlama tarihi veya prim günü değişikliğinde izin hakkı aynı transaction'daki yeni iş bilgilerine göre hesaplanmalı; ayrı bağlantı kilit beklemesi olmamalı.
5. Kadrosuz personelin birim, başlama/ayrılma tarihi, çalışma durumu ve açıklamasını değiştirin. Bu işlem artık merkezi audit yolundadır; eski olay kaydı üretmemeli. İki ekranın farklı birim kontrollerinin mevcut davranışını kontrol edin.
6. İletişim bilgilerini değiştirin. Kayıt/audit birlikte tamamlanmalı. `PersonelGirisiWP` için e-posta/cep telefonu ekleme veya değiştirmede mevcut bildirim koşullarını; ilgisiz alan değişikliğinde bildirim olmamasını kontrol edin. Bildirimin yalnızca commit sonrasında çağrıldığını doğrulayın.
7. Eksik ücretli veya mazeret izin dönemi bulunan personelin ekranını açın. Uygun başlama tarihi varsa dönem merkezi transaction ile oluşmalı; gerçek kayıt kimliğiyle listeye devam edilmeli. Başlama tarihi yokken dönem oluşturulmamalı. Mevcut dönem yeniden açılışta yeniden eklenmemeli.
8. İzin hesabında ilk yıl, asker/sivil, 5/6 ve 14/15 yıl eşikleri, 359/360 prim günü ve prim gününün yıl eşiğini değiştirdiği kayıtları kontrol edin. Başlama tarihi değişen kayıtta eski DB değerinin kullanılması veya hatanın varsayılan izin hakkıyla gizlenmesi olmamalı. Mevcut tarih hesaplamasının 29 Şubat gibi sınırlarını da değerlendirin; takvim iş kuralları bu parçada değiştirilmedi.
9. Loglama açıkken her gerçek iş değişikliği için mevcut merkezi audit kapsamını ve aynı OperationId ilişkisini kontrol edin. Aynı kimlikteki farklı iş verisi değişiklikleri ayrı audit olabilir; aynı yazı iki katmanda tekrar kaydedilmemeli. SQL izi ve hatalar mevcut bağımsız teknik yazıcıda kalmalı; rollback sonrasında teknik kayıt kalması iş verisinin commit edildiği anlamına gelmez.
10. Loglama kapalıyken iş sonucu ve rollback kapsamı aynı olmalı; bu iki ekranın yazma yolları `Olay_Table`/`SQLLog_Table` tablosuna yeni kayıt üretmemeli. Diğer kullanıcı/ekranların eşzamanlı eski yazılarını bu senaryoya karıştırmayın. SQL/audit erişim hatasında tek güvenli Türkçe hata mesajı; normal yönlendirmede sahte teknik hata olmaması beklenir.

## Kalan kapsam

İzin/talep/dönem/mahsup ve toplu işlem ekranları sonraki IKYS parçasındadır. Diğer bağlamsız servis API'leri, eski olay sınıfları, bayraklar ve genel SQL logger henüz kaldırılmadı. Bu dal bütün solution için sıfır eski yazı garantisi değildir. Eski tablolar ve geçmiş veriler korunur.

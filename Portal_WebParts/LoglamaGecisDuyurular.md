# Portal duyuru loglama, transaction ve mesaj geçişi

Başlangıç: temiz yerel master ve uzak master `f1d0a15e17e3fa8fb39d279fa2d9817728c81557` (#523).
Dal: `codex/portal-duyuru-log-transaction`.
Ekranlar: DuyuruGirisiWP, DuyuruListesiWP, DuyuruPopupWP.

## DB ve log kapsamı

Duyuru, gösterim sürümü ve okuma repository'lerine açık PortalLogContext ile salt okuma yolu eklendi. Eski çağrılar korunur; bağlamlı çağrıda DbClass/legacy SQL logger kullanılmaz. Bu üç ekranın Personel okumaları mevcut bağlamlı IKYS okuma overload'larını kullanır; IKYS yazma akışları değiştirilmez.

Yeni PortalDuyuruWriteService/Repository üzerinden:

- Duyuru ekleme/güncelleme, gösterim sürümü ve her iki kaydın merkezî audit'i aynı SqlTransactionContext içinde yazılır. Başarı, Dispose/commit sonrasında döner. Güncellemede ilk metadata, açıklama ve yükleme yapılmadıysa mevcut resim gerçek kilitli kayıttan korunur. Eski akıştaki ardışık INSERT/UPDATE/UPDATE çağrıları kaldırıldı.
- Resim bağlantısı kaldırma, duyuruyu ve yeni gösterim sürümünü birlikte günceller. Böylece popup eski sürümün resmini göstermeye devam etmez. Fiziksel dosya silinmez; önceki sürümün dosyası korunur.
- Silme, duyuru ve audit'i tek transaction'da yapar. Mevcut davranış gibi gösterim/okuma geçmişini ve kitaplık dosyalarını ayrıca silmez; cascade veya veri temizliği eklenmedi.
- Okundu kaydında personel kimliği login'den sunucuda bulunur. İstekten gelen personel kimliği kullanılmaz. Gösterilen sürümler ViewState'te sınırlandırılır; ayrıca DB'deki güncel duyuru, son sürüm ve alıcı listesi transaction içinde doğrulanır. Birden çok okuma birlikte commit edilir; geçersiz/eski sürümde tamamı geri alınır. Aynı personel/sürümün tekrarlı isteği ikinci kayıt oluşturmaz.

Güncelleme, silme, resim kaldırma ve okuma işlemleri duyuru kimliğine göre transaction sahipli uygulama kilidi kullanır. Çoklu okumada duyuru kilitleri sıralıdır. Bu kilit yeni servisin çağrılarını koordine eder; başka eski yazma tüketicileri aynı uygulama kilidine katılmış sayılmaz. Yeni servis satır/range kilitlerini de kullanır. Eşzamanlı form güncellemelerinde son kaydedenin metin/başlık değişikliği geçerlidir; yeni bir form snapshot çatışma özelliği eklenmedi.

Audit alanları açık listedir: yayın aralığı, tekrar, aktif/popup, duyuru/sürüm kimlikleri ve okuma/gösterim tarihi. Duyuru gövdesi, başlık, alıcı listesi, personel kimliği ve resim adı alan farklarına yazılmaz. Teknik loglarda SQL metni veya parametreler yoktur. Etkinlik ve teknik hata logları mevcut altyapı gibi iş transaction'ından bağımsızdır.

## Resim işleminin sınırı

SharePoint ve SQL ortak transaction paylaşmaz. Resim önce mevcut duyuru resim kitaplığına GUID içeren benzersiz adla, overwrite=false ile yüklenir; ardından duyuru ve sürüm bu adı birlikte kaydeder. Eski resim üzerine yazılmaz. Yeni yükleme yardımcısı form digest ve SharePoint'in mevcut kullanıcı izinlerini kullanır; yetki yükseltmez veya AllowUnsafeUpdates açmaz.

JPG uzantısı/içeriği, 6.000.000 bayt sınırı ve 40 milyon piksel sınırı kontrol edilir. Resim mevcut 1000x1200 üst boyutuna oranı korunarak küçültülür; kaynak/çıktı nesneleri kapatılır. Eski yardımcının hata yutan ve başarı popup'ı üreten yolu bu ekranlarda çağrılmaz.

Yükleme sonrası DB reddinde resmin yüklendiği fakat duyurunun kaydedilmediği tek uyarıyla bildirilir. Teknik hata/commit belirsizliğinde kaydı ve kitaplığı kontrol etme adımı, başvuru koduyla gösterilir. Otomatik dosya silme/yeniden deneme yapılmaz; bağlantısız kalmış yeni dosya test sırasında kitaplıktan incelenebilir.

## Mesaj ve ekran değişiklikleri

- Duyuru girişinde başlık, metin, tarih/saat ve alıcı doğrulaması anlamlı Türkçe Warning'dir. Önceki OR doğrulaması kaldırıldı: tek dolu alan kayıt için yeterli değildir. Bitiş başlangıçtan önce olamaz.
- Kayıt/güncelleme sonrası aynı ekranda tek Success gösterilir; yeni kayıt düzenleme durumuna geçer. URL'deki Mesaj parametresi artık başarı üretmez. Silme sonrası tek başarı ve listeye dönüş yönlendirmesi gösterilir; otomatik redirect yerine sonuç ekranda kalır. Kapat düğmesi listeye döner.
- Teknik hata bir kez loglanır ve güvenli Error/OperationId gösterilir. İç yardımcılar ayrı popup üretmez. Commit sonrası ekran yenilemesi başarısızsa kaydın tamamlandığı açıkça söylenir.
- Resim yoksa kaldırma Info; varsa commit sonrası tek Success'tir. Okundu işlemi normal durumda ek popup üretmez; başarısızlıkta tek uyarı/teknik hata gösterir.
- Personel kaydı olmayan kullanıcıya popup duyuru listesi açık kalmaz. Elle açılan duyuruda da alıcı/yayın listesi sunucuda doğrulanır. Duyuru yönetiminin sayfa/web erişim izinleri mevcut host düzeninde kalır; yeni bir uygulama rol sistemi eklenmedi.
- Kalıcı okundu çerezi, sunucu kaydı commit ettikten sonra oluşturulur. Mevcut kısa süreli (30 saniye) gösterim çerezi korunur. Marquee'den elle açma ile otomatik popup ayrımında boolean artık gerçek JavaScript boolean'ı gönderilir.
- Başlık/personel listesi metinleri HTML olarak çalıştırılmaz. Rich text duyuru gövdesinin mevcut HTML gösterimi korunur. Liste JSON'unda script kapanışı kaçırılır; Excel içeriği Response.Clear öncesinde hazırlanır ve Excel düğmesi tam postback'e kaydedilir. Redirect/Excel kaynaklı ThreadAbort teknik hata sayılmaz.

## Yapılandırma

PortalLogging.EnabledWebParts listesine mevcut değerleri koruyarak noktalı virgülle şu değerler eklenir:

`Portal:DuyuruGirisiWP;Portal:DuyuruListesiWP;Portal:DuyuruPopupWP`

Portal projesinin tüm ekranları taşınmadığı için bu parça adına EnabledProjects'e Portal eklenmez. Log tabloları mevcut merkezî altyapıdır; bu parçada yeni DB şeması/migration yoktur. Uygulama kilidi ve audit tablolarına erişim TEST'te mevcut uygulama DB hesabıyla doğrulanmalıdır.

## Kullanıcı tarafından TEST / inceleme

1. Resimsiz ve JPG resimli yeni duyuru kaydedin; duyuru, gösterim sürümü ve audit'in birlikte oluştuğunu, tek başarı mesajını ve ekranın düzenleme durumuna geçtiğini kontrol edin. Sayfayı Mesaj=true ile açmanın başarı mesajı üretmediğini doğrulayın.
2. Başlık/metin/tarih/alıcı eksikliği, ters tarih, yanlış uzantı ve büyük resimde anlamlı tek uyarı olmalı; DB'de kısmi duyuru/sürüm oluşmamalı. JPG'nin kitaplığa yüklendiği fakat DB'nin reddettiği durumda kitaplık ve kısmi sonuç mesajını inceleyin.
3. Metni ve resmi güncelleyin: duyuru ile en son gösterim aynı içeriği/resmi göstermeli. Resim kaldırıldığında yeni popup sürümünde resim görünmemeli. Eski sürümün resmi fiziksel olarak korunmalı.
4. Alıcı olan, olmayan ve personel kaydı bulunmayan kullanıcılarla açılış/elle açma deneyin. Başka duyuru/sürüm kimliği gönderiminde okuma yazılmamalı. Sürüm değiştikten sonra eski popup için okundu isteği uyarı vermeli ve yeni sürümü okundu yapmamalı.
5. Aynı personelin aynı sürümü iki sekmeden/tekrarlı postback ile okumasında tek DB kaydı kalmalı. Çoklu okuma ortasında hata olursa tüm okuma/audit kayıtları geri alınmalı; kalıcı okundu çerezi oluşmamalı. Elle açılan popup kapanışı yeni okuma kaydı oluşturmamalı.
6. Silme ve eşzamanlı değişiklik senaryolarını, liste/okuyanlar modalı ve Türkçe Excel çıktısını kontrol edin. Kontrollü DB hatasında tek güvenli mesaj/başvuru kodu ve aynı OperationId teknik logu beklenir. Commit sonrası yenileme hatası ayrıca kayıt başarısızmış gibi bildirilmemeli.

Codex build/test/debug çalıştırmadı; DB, SharePoint veya gerçek kullanıcı akışına iş isteği yapmadı. Kaynak, UTF-8/BOM, proje Compile yolları ve diff kontrolleri statiktir. Üretilen ASCX dosyaları düzenlenmedi. Commit/push/PR/merge kullanıcıya aittir.

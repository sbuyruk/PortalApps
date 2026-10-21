# TBYS loglama ve mesaj dönüşümü — envanter ve ilerleme

## Güncel parça: taşınmaz kopyalama ve ilişkileri taşıma

Dal: `codex/tbys-property-copy-audit`. Başlangıç commit'i: `db24c8f2682882c8fa9dee1f48cd70e49ffd6b41`. Kod değişikliğinden önce yerel master temizdi ve GitHub master ile aynı commit'teydi. Önceki taşınmaz parçası PR #459 ile birleştirildi. Kullanıcı toplu test yapacağını bildirmişti; bu parçanın çalışma zamanı doğrulaması henüz yapılmadı.

KopyalaNowBtn_Click yalnızca bağlamlı TasinmazService.CopyOutOfInventory çağrısını ve başarılı işlemden sonraki yönlendirmeyi yapar. Kaynak taşınmaz, bölümler, bağış, son sigorta, onarımlar ve sözleşme bağlantıları aynı SqlTransactionContext ile okunur ve kilitlenir. Yeni taşınmaz/bölüm/bağış kayıtları, ilişki taşımaları ve tüm audit kayıtları bu transaction içindedir. Herhangi bir adım başarısız olursa hepsi geri alınır. ThreadAbortException yönlendirmede yeniden fırlatılır; diğer hatalar ortak güvenli Türkçe mesaj ve başvuru koduyla bildirilir.

İş davranışı ve inceleme noktaları:

- Kaynak yalnızca envanterden çıkmış bir taşınmaz olabilir. Kaynak taşınmaz güncellenmez. Yeni taşınmazda envanter durumu açılır, çıkış sebebi/bedeli/tarihi temizlenir, oluşturulma bilgileri yeniden üretilir ve eski değiştiren/zaman bilgisi taşınmaz. Resim alanlarının referansları korunur; sabit 100TapuFoto.jpg / 100test.jpg örnek kopyalama çağrısı kaldırıldı. Gerçek resim dosyaları çoğaltılmaz veya silinmez.
- Bağımsız bölümler artık yeni Id ve yeni TasinmazId ile kopyalanır; eski bölümler kaynakta kalır. Taşınan sigorta ve sözleşmedeki BolumId yeni bölümün Id değerine eşlenir. Böylece yeni taşınmaza ait ilişkiler eski taşınmazın bölümünü göstermeyi sürdürmez. NULL ve sıfır olan bölüm bağlantıları kendi değerlerinde korunur. Kaynağa ait olmayan veya bulunamayan bir bölüm bağlantısı bütün kopyalamayı iptal eder.
- Mevcut bağış seçimi gibi tek bağış esas alınır (Id sırası). Bağışçı kimliği bu bağıştan okunur ve bağışçı kaydı aynı transaction ile doğrulanır; taşınmazdaki eski BagisciId değeriyle farklıysa yeni taşınmazda düzeltilir. Yeni bağışa BagisciId, BagisTarihi, BagisYili ve envanter durumu aktarılır; kaynak bağış ve bağışçı korunur. Armağan bilgileri önceki akıştaki gibi yeni bağışa kopyalanmaz. Bağış yoksa bağış oluşturulmaz.
- Yalnızca son sigorta taşınır (SigortaBitTar DESC, eşitlikte Id DESC); önceki sigortalar kaynakta kalır. Sigorta yoksa bu kopyalama akışı otomatik sigorta yaratmaz.
- Tüm onarımlar ve tüm SozlesmeTasinmaz bağlantıları mevcut kodun yaptığı gibi yeni taşınmaza taşınır. Eski yorumdaki aktif sözleşme sınırlaması gerçek kodda yoktu; bu parçada yeni bir aktif/pasif filtresi eklenmedi. Sözleşme Id değişmez; ödeme planı tablosuna ayrıca yazılmaz.
- Taşıma UPDATE'leri yalnızca TasinmazId, gerektiğinde BolumId ve değiştiren/zaman alanlarını yazar. Onarım tutarını yuvarlayan eski adresli rapor sorgusundan model üretip tüm satırı güncelleme kaldırıldı. Tutar, açıklama, poliçe ve oluşturulma bilgileri taşımada yeniden yazılmaz. UPDATE kaynak Id koşuluyla tam bir satır etkilemelidir; aksi durumda exception ile rollback olur.

Audit: Yeni Tasinmaz ve BagimsizBolum kayıtları önceki parçalardaki izin listeleriyle INSERT üretir. Yeni Bagis INSERT ayrıntıları BagisciId, TasinmazId, BagisTarihi, BagisYili ve Envanterde alanlarıyla sınırlıdır. Sigorta UPDATE TasinmazId/BolumId, Onarim UPDATE TasinmazId, SozlesmeTasinmaz UPDATE değişen TasinmazId/BolumId alanlarını yazar; değişmeyen SozlesmeId ayrıntı üretmez. Eski/yeni değerler transaction'dan okunan gerçek satırlardan alınır. Kişisel ad, adres, açıklama ve poliçe metni ilişki audit'ine yazılmaz. Tüm audit başlıkları aynı istek OperationId değerini ve gerçek entity Id değerini taşır.

Teknik SQL izleri ortak bağımsız yazıcıdadır; komutun Succeeded olması daha sonra iş transaction'ının rollback olmayacağı anlamına gelmez. Logging kapalıyken iş transaction'ı çalışmaya devam eder; audit ve eski OlayKayit üretilmez. Önceki ekleme/güncelleme/silme yollarının davranışı bu kopyalama metodu tarafından değiştirilmez.

Kullanıcı TEST kontrolleri:

1. Bağlantısız, envanter dışı taşınmazı kopyalayın. Yeni taşınmaz envanterde olmalı; kaynak kayıt ve resim referansları korunmalı. Tasinmaz INSERT ve SQL izleri TasinmazGirisiWP bağlamıyla görünmeli.
2. Bağımsız bölümleri, bağışı, birden fazla sigortası, onarımları ve sözleşme bağlantıları bulunan kaynakla deneyin. Yeni bölümlerin Id değerleri ayrı olmalı; kaynak bölümler korunmalı. Yeni bağış doğru bağışçıya bağlanmalı; yalnızca son sigorta, tüm onarımlar ve tüm sözleşme bağlantıları taşınmalı.
3. Sigorta/sözleşme BolumId alanları yeni bölümleri göstermeli. NULL ve 0 bağlantılarını ayrıca deneyin. Başka taşınmazın bölümüne bağlanmış bozuk test kaydında kopyalama bütünüyle geri alınmalı.
4. Taşımada onarım Tutar değerinin tam hassasiyeti, açıklamalar, poliçe verileri ve oluşturulma bilgileri değişmemeli. Audit UPDATE ayrıntıları yalnızca değişen ilişki alanlarını göstermeli. Yeni INSERT audit kimlikleri yeni uygulama kayıtlarıyla eşleşmeli.
5. Birkaç alt kayıt oluşturulduktan/taşındıktan sonra kontrollü audit veya iş yazma hatası üretin. Yeni taşınmaz, bölümler, bağış ve tüm ilişki değişiklikleri ile audit kayıtları birlikte rollback olmalı; teknik Error kaydı kalmalı.
6. Envanterde olan veya bulunmayan kaynak Id ile çağrı yeni kayıt üretmemeli. Logging kapalı kullanımda iş akışı ve rollback kuralı çalışmalı. Başarılı yönlendirme hata olarak loglanmamalı.

Bu akış kaynak üzerinde kalıcı bir kopyalandı işareti oluşturmaz; aynı kaynak için yeniden verilen kopyalama komutu, önceki davranış gibi tekrar yeni kayıt oluşturabilir. Genel bağış/sigorta/onarım/sözleşme ekranlarının bağımsız yazmaları ve envanterden çıkarma sonraki parçalardır. Yeni dosya, şema veya tanılama altyapısı eklenmedi. Üretilmiş ASCX dosyaları değişmedi. Mevcut EnabledProjects=TBYS varsa web.config değişmez; tek WebPart etkinleştirmesinde TBYS:TasinmazGirisiWP yeterlidir. Codex build/test/debug çalıştırmaz; yalnızca statik kaynak incelemesi ve git diff --check yapılır.

## Önceki parça: taşınmaz ve bağlı sigorta transaction/audit dönüşümü

Dal: `codex/tbys-tasinmaz-transaction-audit`. Başlangıç commit'i: `21b1127bf934abcc030b70f3c5f6281c882c9075`. Kod değişikliğinden önce yerel master temizdi ve GitHub master ile aynı commit'teydi. Önceki bağımsız bölüm parçası PR #458 ile birleştirildi; kullanıcı testleri toplu yapacak. Bu parçanın çalışma zamanı doğrulaması henüz yapılmadı.

TasinmazGirisiWP ve MulkiyetiOlmayanTasinmazGirisiWP ekleme/güncelleme akışlarındaki TransactionScope blokları kaldırıldı. Taşınmaz yazması, gerekli bağış/bağışçı okumaları, son sigortanın okunması ve oluşturulması/güncellenmesi, audit başlığı ve alan ayrıntıları tek SqlTransactionContext kullanır. İşlem veya audit yazımı hata verirse tamamı rollback olur. Güncellemede bağışçı eşitlemesi ana UPDATE öncesinde yapılır; aynı taşınmaz için gereksiz ikinci UPDATE oluşmaz.

Mevcut ekran farkları korunur: normal taşınmaz girişinde bağışçı adı ve BagisciId iş kaydına eşitlenir; envanterden çıkan taşınmaz güncellemesinde sigorta eşitlenmez. Mülkiyeti olmayan taşınmaz ekranında bağışçı eşitlemesi yapılmaz, sigorta eşitlenir. Son sigorta SigortaBitTar azalan sırayla seçilir; eşit tarihlerde Id azalan sıra belirleyicidir. Sigorta türü değişmemişse gereksiz sigorta UPDATE/audit üretilmez.

İki ekrandaki taşınmaz okumaları bağlam taşır. Normal taşınmaz girişindeki Değerleri Kaydet işlemi de transaction ve audit yolunu kullanır. Bağlamsız eski servis çağıranları kendi eski davranışlarında kalır. Bağlamlı yazmalar ayar kapalıysa da iş transaction'ını kullanır; eski OlayKayit yazıcısına geri dönmez. Aynı istek içinde SQL, hata ve audit kayıtları ortak OperationId taşır; yönlendirmeden sonraki açılış yeni istektir.

Taşınmaz silinmeden önce eski kayıt ve bağlı bağımsız bölüm, bağış, sözleşme, onarım ve sigorta varlığı aynı transaction'dan UPDLOCK/HOLDLOCK ile kontrol edilir. Herhangi bir bağlantı varsa silme ve DELETE audit yapılmaz. Mülkiyeti olmayan taşınmaz silmesi de bu kontrollerden geçer. Bağımsız ekrandaki ön kontroller veri bütünlüğü kararı sayılmaz. Silme düğmesindeki sabit adlarla örnek resim kopyalama çağrısı kaldırıldı; null kontrolü nesne alanlarına erişimden önce yapılır. Tek sözleşme de sözleşme varlığı kabul edilir.

Audit entity adları Tasinmaz ve Sigorta, kimlikler uygulamadaki Id değerleridir. İzin listeleri servislerde açıkça tanımlıdır. Taşınmazda bölge kimlikleri, mülkiyet/kullanım/kira/sigorta ve envanter durumları, değerleme tutarları, alanlar, tarihler, BagisciId ve sayısal plan/hisse alanları izlenir. Sigortada TasinmazId, BolumId, SigortaCinsi, SigortaBasTar, SigortaBitTar, SigortaBedeli, Prim ve KullanimSekli izlenir. Ad-soyad, adres, poliçe/belge numarası ve serbest açıklamalar audit ayrıntısına alınmaz. Eski/yeni değerler doğrudan transaction'dan okunan veritabanı satırlarıyla karşılaştırılır; NULL ve sıfır ayrıdır, tarih/sayılar kültürden bağımsız biçimlenir. İzinli alanı değişmeyen taşınmaz UPDATE başlığında sıfır ayrıntı bulunabilir.

Teknik SQL izleri ortak bağımsız yazıcıyı kullanır. SqlTrace Succeeded yalnızca komutun yürütülmesini gösterir; sonradan rollback olan işin tamamlandığı anlamına gelmez. Audit başlığı ve ayrıntıları ise iş transaction'ının parçasıdır. Yeni olay yakalayıcıları teknik hatayı güvenli Türkçe mesaj ve başvuru koduyla bildirir; Redirect kaynaklı ThreadAbortException hata kaydına dönüşmez.

Kapsam sınırı: bağışçı/bağış/sigorta/onarım/sözleşme ekranlarının kendi yazmaları, envanterden çıkarma ve KopyalaNowBtn_Click içindeki tüm ilişkileri taşıyan kopyalama zinciri bu parçada dönüştürülmedi. Bu akışlar ve kalan TBYS servisleri sonraki parçalardır. Repository'ye transaction overload'u eklenmiş olması ilgili modülün bütünü tamamlandı anlamına gelmez.

Kullanıcı TEST kontrolleri (önceki bağımsız bölüm kontrollerine ek):

1. İki ekranda ayrı ayrı yeni taşınmaz oluşturun. Taşınmaz ve ilk sigorta iş kayıtları ile Tasinmaz INSERT ve Sigorta INSERT audit başlıkları aynı OperationId altında oluşmalı; gerçek EntityId değerleri doğru olmalı.
2. Normal taşınmazda tek değerleme tutarını ve sigorta türünü değiştirip kaydedin. Tasinmaz UPDATE ve gerekiyorsa Sigorta UPDATE, yalnızca değişen izinli alanların eski/yeni değerleriyle görünmeli. Sigorta türü değişmediyse sigorta UPDATE beklenmez. Değerleri Kaydet düğmesini de ayrıca deneyin.
3. Bağışçısı olan taşınmazı kaydedin. BagisRepository.SelectByTasinmazId ve TasinmazBagisciRepository.SelectById SELECT izleri görünmeli; iş tablosundaki ad-soyad eşitlenmeli, audit ayrıntısına ad-soyad/adres/açıklama yazılmamalı.
4. Envanterden çıkan taşınmaz güncellemesinin sigortayı değiştirmediğini ve mülkiyeti olmayan taşınmazın sigorta eşitlemesini koruduğunu kontrol edin.
5. Her bağlantı türü için taşınmaz silmeyi deneyin: bölüm, bağış, sözleşme, onarım veya sigorta varsa silinmemeli ve DELETE audit oluşmamalı. Modal açıldıktan sonra bağlantı ekleme senaryosunu da deneyin. Bağlantısız bir test kaydında başarılı DELETE ve eski izinli alanlar görünmeli. Otomatik oluşturulan sigorta da bir bağlantıdır; silme test verisini buna göre hazırlayın.
6. Kontrollü sigorta veya audit ayrıntısı yazma hatasında taşınmaz, sigorta ve audit değişikliklerinin birlikte geri alındığını; teknik hata kaydının kaldığını doğrulayın. Teknik log yazma yetkisini kapatmak bu kontrolle aynı senaryo değildir.
7. Logging kapalıyken iş akışı çalışmalı; yeni audit ve eski OlayKayit oluşmamalı. Başarılı yönlendirme hata olarak kaydedilmemeli.

EnabledProjects içinde TBYS zaten varsa ek web.config değişikliği gerekmez. Tek tek etkinleştirmede mevcut değerleri koruyarak TBYS:TasinmazGirisiWP ve TBYS:MulkiyetiOlmayanTasinmazGirisiWP kullanılır. Yeni dosya, şema veya tanılama altyapısı eklenmedi; üretilmiş ASCX dosyaları değiştirilmedi. Codex build/test/debug çalıştırmadı; yalnızca statik kaynak incelemesi ve git diff --check yapıldı. Build ve TEST kullanıcıya aittir.

## Önceki parça: bağımsız bölüm SQL izi ve audit

Dal: `codex/tbys-bagimsiz-bolum-audit`. Başlangıç commit'i: `8cf737492011691415b9836b33284615c1c2429c`. Kod değişikliğinden önce master temizdi ve GitHub master ile aynı commit'teydi. Bu parçanın çalışma zamanı sonucu kullanıcı doğrulamasını bekliyor.

BagimsizBolumWP'nin okumaları ve ekleme/güncelleme/silme çağrıları LoggingContext taşır. İlgili taşınmaz ve sözleşme okumaları SQL izi üretir. TasinmazGirisiWP içindeki bağımsız bölüm okuma çağrıları da kendi WebPart bağlamıyla izlenir. Aynı HTTP isteğinin SQL, hata ve audit kayıtları ortak OperationId taşır; yönlendirme yeni istektir.

Bağımsız bölüm yazması, audit başlığı ve değişen izinli alanlar aynı SqlTransactionContext içindedir. Eski kayıt ve bağlı taşınmaz aynı transaction'dan okunur. Silme kararı sırasında sözleşme bağlantısı UPDLOCK/HOLDLOCK ile tekrar kontrol edilir; bağlantı varsa kayıt silinmez. Önceden gösterilmiş sil düğmesi bu kontrolü atlayamaz. Yeni bağlantı eklemeye çalışan diğer akışların ve taşınmaz silmenin bütünü bu parçada dönüştürülmüş sayılmaz.

Audit entity adı BagimsizBolum, kimlik uygulamadaki Id alanıdır. İzinli alanlar: TasinmazId, BolumNo, BBBrutAlan, BBNetAlan, KullanimAmaci, MuhasebeyeKayitliDeger, TahminiRayicDegeri, EmlakBeyanDegeri, YaklasikPiyasaDegeri. Serbest Nitelik ve Aciklama metinleri iş verisinde korunur; audit ayrıntısına alınmaz. Teknik kullanıcı/zaman alanları da ayrıntıya alınmaz. Eski/yeni değerler doğrudan veritabanı kayıtlarından karşılaştırılır; NULL sıfıra çevrilmez ve kayıttan sonra okunan gerçek sayı hassasiyeti esas alınır. Güncellemede yalnızca değişen izinli alanlar yazılır; izinli alan değişmeden yapılan UPDATE başlığının ayrıntı sayısı sıfır olabilir.

Bağlamlı bağımsız bölüm yazmaları eski OlayKayit yoluna dönmez; logging kapalıysa iş transaction'ı devam eder, audit yazılmaz. Bağlamsız eski çağıranlar korunur. Bağımsız bölümde audit yazma hatası iş verisini de geri alır; teknik hata/SQL izleri ortak bağımsız yazıcı üzerinden gider. SQL izindeki Succeeded, ilgili komutun yürütülmesini gösterir; iş transaction'ının daha sonra rollback olmasını dışlamaz.

Taşınmaz ve sözleşme servislerine bu parçada yalnızca ihtiyaç duyulan bağlamlı okuma overload'ları eklendi. Bu önceki parçada taşınmazın bağışçı/sigorta içeren yazma zinciri ve sözleşme yazmaları kapsam dışıydı. Taşınmaz zincirinin güncel durumu üstteki yeni parçada açıklanır; sözleşme yazmaları sonraki parçalardır.

Kullanıcı TEST kontrolleri:

1. Bağımsız bölüm ekranı açıldığında bağlı taşınmaz ve bölüm SELECT izleri doğru WebPart/metot adıyla görünmeli. Taşınmaz girişindeki bağımsız bölüm okumaları TasinmazGirisiWP bağlamını taşımalı.
2. Bölüm ekleme/güncelleme/silmede INSERT/UPDATE/DELETE audit başlığı, doğru EntityId ve izinli alanlar kontrol edilmeli. Tek alan güncellemede yalnızca o alanın eski/yeni değeri ayrıntıya yazılmalı. Açıklama/nitelik metni ayrıntıda bulunmamalı.
3. Sözleşmeyle bağlı bölüm silinememeli. Modal açıldıktan sonra bağlantı eklenmiş olsa bile son silme kontrolü kaydı korumalı. Bu başarısız silmede DELETE audit kaydı oluşmamalı.
4. Geçersiz veya artık bulunmayan taşınmaza bölüm eklenmemeli. Kontrollü audit ayrıntısı yazma hatasında ana kayıt ve audit başlığı rollback olmalı; teknik Error kaydı korunmalı. Hata senaryosu teknik log tablosuna yazma yetkisini de kapatmamalı.
5. Başarılı yönlendirme hata olarak loglanmamalı. Yeni hatalar kullanıcıya teknik ayrıntı yerine mevcut güvenli Türkçe mesajla gösterilmeli. Logging kapalı senaryoda eski log yazıcısına dönüş olmamalı.

Mevcut EnabledProjects=TBYS etkinleştirmesi varsa yeni web.config değişikliği gerekmez. Tek tek etkinleştirmede mevcut değerler korunarak TBYS:BagimsizBolumWP ve ilgili okumalar için TBYS:TasinmazGirisiWP kullanılır. Yeni şema, dosya veya tanılama kodu eklenmedi; üretilmiş dosyalar değiştirilmedi. Codex build/test/debug çalıştırmadı. Statik kaynak kontrolleri ve git diff --check tamamlandı; build ve TEST kullanıcıya aittir.

## İlk envanter — başlangıç durumu

Tarih: 20 Ekim 2026. Başlangıç commit'i: 6e6978d2fff9a5524b5315f49a9df88fb051d045.
Dal: codex/tbys-logging-message-migration. Değişiklik öncesi master temizdi ve GitHub master ile aynı commit'teydi.

## Kapsam

74 WebPart, 29 model, 29 servis ve 29 repository incelendi. Başlangıçta 4 WebPart'ta log bağlamı vardı, 70'inde yoktu. TBYS WebPart'larında TryRecordWebPartOpened çağrısı yoktu. Kapsamdaki kaynaklarda 297 catch satırı bulundu; dosya ve satırları aşağıda listelenmiştir.
Bu parçada eksik 70 bağlam eklenir. Açılış, hata, SQL izi, audit ve mesaj dönüşümü henüz tamamlanmadı. Diğer projeler, ortak logging ve üretilmiş dosyalar değiştirilmez.
Aşağıdaki satır numaraları değişiklik ÖNCESİ kaynak envanteridir. Metin araması yorumları da içerebilir; çalışma zamanı ve çağrı zinciri doğrulaması değildir.

## Ortak altyapı ve açıklar

PortalLogContext aynı HTTP isteğinde OperationId paylaşır, SharePoint/HTTP login bilgisini ve sorgusuz sayfa yolunu alır. UTC zamanı PortalLogRecord.OccurredUtc üretir.
PortalLogRepository başlıkları PortalLog_Table'a, audit değişikliklerini PortalAuditChange_Table'a yazar. PortalTechnicalLog iş transaction'ından bağımsız bağlantıyla Error ve SqlTrace yazar.
Ortak hata yazıcısında Operation ve UserMessage alanları INSERT'e dahil değil; hata entegrasyonu parçasında mevcut ortak sınıfta tamamlanması değerlendirilmeli.
Logging ayarları kapalıysa ortak yazıcılar kayıt üretmez. Bu parçada etkinleştirme yapılmaz. Kullanıcı birleşik şemayı (Database/Migrations/20261019_PortalLoggingUnified.sql) ve host web.config ayarlarını doğrulamalı.
Mevcut dört ReportError yardımcısı ayar kapalıyken ExceptionHelper'a döner; bu da LogHelper yolunu kullanır. TBYS hata entegrasyonunda bu eski yol kaldırılmalı.
Repository'ler DbClass kullanır. Ortak DbClass içinde SQLLog_Table yazıcısı var; TBYS içinde doğrudan referans bulunmaması dolaylı kullanımın bittiğini göstermez.
Servislerde OlayKayit çağrıları var. Yeni audit yolu aynı transaction'a taşınmadan eski kayıt akışının kaldırıldığı söylenemez.

## Transaction ve audit

TBYS model/servis/repository kaynaklarında SqlTransactionContext parametreli metot bulunmadı. WebPart'larda 7 TransactionScope noktası var; bunlar SqlTransactionContext ile eşdeğer kabul edilmemeli.
Çok adımlı yazmalar transaction-aware servislere taşınmalı. Transaction verilen yolda okumalar, yazmalar, audit başlığı ve alan değişiklikleri aynı transaction'ı kullanmalı.
Audit alanları entity bazında izin listesiyle seçilmeli; kimlik, iletişim, hesap ve serbest açıklamalar otomatik topluca loglanmamalı. Eski kayıt aynı transaction ile okunmalı; yalnızca değişen izinli alanlar kaydedilmeli.
Onay/reddetme ve durum değişimleri ortak RecordAudit sözleşmesine uygun UPDATE ve ilgili alan değişiklikleriyle kaydedilmeli.
Ödeme ayrıştırma, toplu ödeme, taşınmaz/bağış ilişkileri, sözleşme onayı ve bakiye devri alt kayıtlarıyla birlikte ele alınmalı.
KiraBakiyeDevriOtomatik arka plan kullanımında bağlam HTTP isteğinden alınmamalı; açıkça oluşturulup servis zincirine aktarılmalı.

## Mesaj bulguları

Doğrudan ex.Message gösteren 10 WebPart çağrısı bulundu; aşağıdaki teknik metin adayları bölümünde listelenmiştir.
KiraBakiyeDevriOtomatik ex.Message değerini sonuç sözlüğüne taşır; tüketicileri güvenli mesaj dönüşümünde takip edilmeli.
ExceptionHelper genel hatayı güvenli Türkçe metne çevirse de eski LogHelper yolunu kullanır. TBYS çağrıları ortak teknik yazıcıya taşınmalı.
İstekteki Operation failed, Record not found, You are not authorized örnekleri birebir bulunmadı. İngilizce başlıklar ve biçim şablonları dahil mesaj adayları aşağıda inceleme için listelendi; tümünün Türkçe olduğu henüz doğrulanmadı.
ArgumentNullException gibi teknik mesajlar UI sınırında güvenli Türkçe karşılıkla ele alınmalı.

## Parçalar ve doğrulama

1. Bu dal: envanter ve WebPart log bağlamları.
2. Sonraki dal: ilk açılış, hata kayıtları ve güvenli Türkçe hata sunumu.
3. Sonraki dal: transaction-aware repository/servis SQL izleri ve bağlam aktarımı.
4. Sonraki dal: audit, izinli alan değişiklikleri ve atomik iş akışları.
5. Sonraki dal: kalan kullanıcı mesajları, modal/form/grid metinleri.
6. Sonraki dal: eksikler ve son kontroller.

Her parça öncesi temiz ve güncel master kontrolü yapılacak. Commit/push/PR/merge kullanıcıya aittir.
Bu parçada build/test/debug çalıştırılmaz. Kullanıcı Visual Studio 2026 build ve TEST sunucusunda WebPart açılışlarını doğrular. Bu parça tek başına yeni log üretmez.
İlgili sonraki parçalarda açılış, başarılı yazma, onay/reddetme, alan değişimi, hata, SQL izi, yetki, OperationId ve rollback senaryoları kullanıcı tarafından test edilmeli.

## WebPart envanteri (başlangıç durumu)

| WebPart | Bağlam | catch satırı |
| --- | --- | --- |
| BagimsizBolumWP | Var | 2 |
| BagisciBagislariWP | Var | 5 |
| BagisciTaahhutleriWP | Var | 6 |
| BagisciTalepleriWP | Var | 5 |
| BagisciYakinlariWP | Yok | 2 |
| BorcluKiraciIslemleriWP | Yok | 4 |
| BorcluKiracilarByBolgeWP | Yok | 4 |
| BorcluKiracilarWP | Yok | 2 |
| BorcluKiraciTakibiByBolgeWP | Yok | 4 |
| DevirGerekenSozlesmelerWP | Yok | 3 |
| EmptyWP | Yok | 1 |
| EnvanterdenCikanTasinmazListesiWP | Yok | 4 |
| EnvanterdenCikarmaWP | Yok | 6 |
| GecikmeZammiWP | Yok | 3 |
| GenelSozlesmeListesiWP | Yok | 4 |
| GerceklesenKiraArtislariWP | Yok | 4 |
| HukukiTakipListesiWP | Yok | 2 |
| KiraArtisCizelgesiWP | Yok | 4 |
| KiraArtisYazisiWP | Yok | 4 |
| KiraBakiyeDevriWP | Yok | 6 |
| KiraBankaEkstreListesiWP | Yok | 7 |
| KiraBorcuYazisiWP | Yok | 4 |
| KiraciAylikOdemeWP | Yok | 6 |
| KiraciEslestirWP | Yok | 6 |
| KiraciGirisiWP | Yok | 10 |
| KiraciListesiWP | Yok | 4 |
| KiradaOlanTasinmazlarWP | Yok | 0 |
| KiraGeliriTahakkukTahsilWP | Yok | 0 |
| KiraGelirleriAyDokumuWP | Yok | 0 |
| KiraKartiWP | Yok | 3 |
| KiraOdemeDosyasiYuklemeWP | Yok | 6 |
| KiraSozlesmeListesiWP | Yok | 4 |
| KiraSozlesmeSiralamaWP | Yok | 2 |
| KiraSozlesmesiWP | Yok | 16 |
| KirayaVerilmeyenTasinmazlarWP | Yok | 0 |
| KPSSorguListesiWP | Yok | 3 |
| KPSSorguSonucuWP | Yok | 0 |
| MevcutKiracilarWP | Yok | 2 |
| MulkiyetiOlmayanTasinmazGirisiWP | Yok | 1 |
| MulkiyetiOlmayanTasinmazListesiWP | Yok | 4 |
| OdemeAyristirmaWP | Yok | 5 |
| OdemeGirisWP | Yok | 3 |
| OdemePlaniListesiWP | Yok | 4 |
| OdemePlaniWP | Yok | 5 |
| SharePointLibraryFileListWP | Yok | 1 |
| SigortaDurumuDaskWP | Yok | 0 |
| SigortaEkleSilWP | Yok | 2 |
| SozlesmeTasinmazWP | Yok | 0 |
| TaahhutListesiWP | Yok | 5 |
| TasinmazBagisciBilgiListesiWP | Yok | 3 |
| TasinmazBagisciGirisiWP | Yok | 9 |
| TasinmazBagisciKartiWP | Yok | 3 |
| TasinmazBagisciListesiWP | Yok | 5 |
| TasinmazBagisHaritasiWP | Yok | 1 |
| TasinmazBagisWP | Yok | 2 |
| TasinmazBolgeRaporuWP | Yok | 0 |
| TasinmazDurumRaporuWP | Yok | 1 |
| TasinmazGirisiWP | Yok | 6 |
| TasinmazKartiWP | Yok | 4 |
| TasinmazListesiWP | Yok | 7 |
| TasinmazOnarimGirisiWP | Yok | 1 |
| TasinmazOnarimListesiWP | Yok | 4 |
| TasinmazResimleriWP | Yok | 10 |
| TasinmazSigortaGirisWP | Yok | 9 |
| TasinmazSigortaListesiWP | Yok | 5 |
| TasinmazSorgulamaWP | Yok | 3 |
| TeminatDurumRaporuWP | Yok | 0 |
| TeminatIslemleriWP | Yok | 9 |
| TeminatListesiWP | Yok | 4 |
| VasiyetciAdresListesiWP | Yok | 5 |
| VasiyetciGirisiWP | Yok | 7 |
| VasiyetciListesiWP | Yok | 4 |
| YasalFaizWP | Yok | 5 |
| YilBazindaSozlesmeWP | Yok | 4 |

## Model, servis ve repository dosyaları

~~~text
DAO/Repositories/TBYS\BagisciTalepleriRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\BagisciBagisRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\BagimsizBolumRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\KiraBorcuTakipRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\HukukiTakipRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\GecikmeZammiRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\BagisRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\BagisciYakinlariRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemeAyristirmaRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:7:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\KiraEkstreAktarmaRepository.cs:7:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\KiraciRepository.cs:7:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\SerhBeyanIrtifakRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OnarimRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemeSebebiTanimRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemeRepository.cs:6:namespace DAO.Repositories.TBYS
Model/TBYS\YasalFaiz.cs:4:namespace Model.TBYS
DAO/Repositories/TBYS\YasalFaizRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemePlaniRepository.cs:5:namespace DAO.Repositories.TBYS
Model/TBYS\VasiyeteKonuVarlik.cs:4:namespace Model.TBYS
DAO/Repositories/TBYS\VasiyeteKonuVarlikRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\OdemePlaniRaporRepository.cs:5:namespace DAO.Repositories.TBYS
Model/TBYS\Vasiyetci.cs:4:namespace Model.TBYS
DAO/Repositories/TBYS\TasinmazBagisciRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\VasiyetciRepository.cs:5:namespace DAO.Repositories.TBYS
Model/TBYS\TeminatIslem.cs:4:namespace Model.TBYS
DAO/Repositories/TBYS\TasinmazBagisciReportRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\TeminatIslemRepository.cs:5:namespace DAO.Repositories.TBYS
Model/TBYS\TBYSOrtak.cs:19:namespace Model.Ortak
DAO/Repositories/TBYS\SozlesmeTasinmazRepository.cs:5:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\TasinmazTaahhutRepository.cs:5:namespace DAO.Repositories.TBYS
Model/TBYS\TasinmazTaahhut.cs:4:namespace Model.TBYS
DAO/Repositories/TBYS\SigortaRepository.cs:8:namespace DAO.Repositories.TBYS
DAO/Repositories/TBYS\TasinmazRepository.cs:6:namespace DAO.Repositories.TBYS
Model/TBYS\TasinmazBagisci.cs:4:namespace Model.TBYS
Model/Services/TBYS\BagisciTalepleriService.cs:10:namespace Model.Services.TBYS
Model/Services/TBYS\BagisService.cs:11:namespace Model.Services.TBYS
Model/TBYS\Tasinmaz.cs:5:namespace Model.TBYS
Model/Services/TBYS\BagisciBagisService.cs:10:namespace Model.Services.TBYS
Model/Services/TBYS\HukukiTakipService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\KiraBorcuTakipService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\GecikmeZammiService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\KiraciService.cs:10:namespace Model.Services.TBYS
Model/TBYS\Kiraci.cs:4:namespace Model.TBYS
Model/TBYS\OdemeAyristirma.cs:4:namespace Model.TBYS
Model/TBYS\KiraBorcuTakip.cs:4:namespace Model.TBYS
Model/TBYS\OdemeAyrinti.cs:4:namespace Model.TBYS
Model/Services/TBYS\BagisciYakinlariService.cs:10:namespace Model.Services.TBYS
Model/TBYS\Odeme.cs:4:namespace Model.TBYS
Model/TBYS\SozlesmeTasinmaz.cs:4:namespace Model.TBYS
Model/Services/TBYS\BagimsizBolumService.cs:11:namespace Model.Services.TBYS
Model/TBYS\KiraBakiyeDevriOtomatik.cs:7:namespace Model.TBYS
Model/TBYS\Onarim.cs:4:namespace Model.TBYS
Model/TBYS\KiraEkstreAktarma.cs:4:namespace Model.TBYS
Model/TBYS\HukukiTakip.cs:4:namespace Model.TBYS
Model/TBYS\OdemePlani.cs:4:namespace Model.TBYS
Model/TBYS\OdemeSebebiTanim.cs:4:namespace Model.TBYS
Model/TBYS\Bagis.cs:4:namespace Model.TBYS
Model/TBYS\BagisciBagis.cs:4:namespace Model.TBYS
Model/TBYS\KiraSozlesme.cs:4:namespace Model.TBYS
Model/TBYS\GecikmeZammi.cs:4:namespace Model.TBYS
Model/Services/TBYS\OdemeAyrintiService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\KiraSozlesmeService.cs:11:namespace Model.Services.TBYS
Model/TBYS\BagimsizBolum.cs:4:namespace Model.TBYS
Model/TBYS\Sigorta.cs:4:namespace Model.TBYS
Model/TBYS\BagisciTalepleri.cs:4:namespace Model.TBYS
Model/TBYS\BagisciYakinlari.cs:4:namespace Model.TBYS
Model/Services/TBYS\OdemeAyristirmaService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\OdemePlaniRaporService.cs:7:namespace Model.Services.TBYS
Model/Services/TBYS\KiraEkstreAktarmaService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\OdemeSebebiTanimService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\OnarimService.cs:11:namespace Model.Services.TBYS
Model/TBYS\SerhBeyanIrtifak.cs:4:namespace Model.TBYS
Model/Services/TBYS\OdemeService.cs:12:namespace Model.Services.TBYS
Model/Services/TBYS\OdemePlaniService.cs:12:namespace Model.Services.TBYS
Model/Services/TBYS\SerhBeyanIrtifakService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\SigortaService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\TasinmazBagisciReportService.cs:5:namespace Model.Services.TBYS
Model/Services/TBYS\TasinmazBagisciService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\TasinmazService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\SozlesmeTasinmazService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\TasinmazTaahhutService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\VasiyetciService.cs:10:namespace Model.Services.TBYS
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\YasalFaizService.cs:11:namespace Model.Services.TBYS
Model/Services/TBYS\TeminatIslemService.cs:11:namespace Model.Services.TBYS
~~~

## Hata yakalama noktaları

~~~text
Model/TBYS\TBYSOrtak.cs:579:            catch (Exception ex)
Model/TBYS\OdemePlani.cs:95:            catch (Exception)
Model/Services/TBYS\OdemeService.cs:71:            catch (Exception exception1)
Model/Services/TBYS\OdemeService.cs:109:            catch (Exception exception1)
Model/Services/TBYS\OdemeService.cs:176:            catch (Exception exception1)
Model/Services/TBYS\OdemePlaniService.cs:62:            catch (Exception) { return false; }
Model/TBYS\KiraBakiyeDevriOtomatik.cs:53:                catch (Exception ex)
Model/TBYS\KiraBakiyeDevriOtomatik.cs:192:            catch
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:157:            catch (Exception)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:488:            catch (Exception ex)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:508:            catch (Exception ex)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:526:            catch (Exception ex)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:541:            catch (Exception ex)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:555:            catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:62:            catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:77:            catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:90:            catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:322:                        catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:330:            catch (Exception ex)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:529:            catch (Exception ex)
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:143:                    catch (Exception)
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:179:                    catch (Exception)
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:205:            catch (Exception ex)
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:175:            catch (Exception ex)
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:197:            catch (Exception ex)
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:211:            catch (Exception ex)
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:201:            catch (Exception exception)
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:371:            catch (Exception ex)
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:410:            catch (Exception ex)
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:457:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:72:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:97:            catch (Exception exception)
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:229:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:278:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:170:            catch (Exception exception)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:216:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:230:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:255:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:287:            catch (Exception ex)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:339:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:142:            catch (Exception)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:331:            catch (Exception exception)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:449:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:730:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:750:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:768:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:783:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:797:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:811:            catch (Exception ex)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:895:            catch (Exception)
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:90:            catch (Exception ex)
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:113:            catch (Exception exception)
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:218:            catch (Exception ex)
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:252:            catch (Exception ex)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:258:            catch (Exception exception)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:286:			catch (Exception exception)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:331:            catch (Exception ex)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:517:            catch (Exception ex)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:570:            catch (Exception exception)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:660:            catch (Exception exception)
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:244:            catch (Exception)
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:384:            catch (Exception ex)
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:423:            catch (Exception ex)
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:460:            catch (Exception)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:209:                catch (Exception ex)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:233:            catch (Exception exception)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:319:                catch (Exception e)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:445:                    catch (Exception)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:572:            catch (Exception ex)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:590:            catch (Exception ex)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:695:            catch (Exception ex)
TBYS_WebParts\EmptyWP\EmptyWP.ascx.cs:42:            catch (Exception ex)
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:134:            catch (Exception ex)
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:254:            catch (Exception)
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:327:            catch (Exception ex)
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:356:            catch (Exception ex)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:122:            catch (Exception ex)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:145:            catch (Exception exception)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:309:            catch (Exception ex)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:333:            catch (Exception ex)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:351:            catch (Exception ex)
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:376:            catch (Exception exception)
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:192:            catch (Exception ex)
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:206:            catch (Exception ex)
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:135:            catch (Exception ex)
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:179:            catch (Exception exception)
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:361:            catch (Exception ex)
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:407:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:472:                catch (Exception)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:600:            catch (Exception e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:629:            catch (Exception e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:711:            catch (Exception)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:819:            catch (Exception e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:845:            catch (Exception e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:942:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1377:            catch (Exception e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1401:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1417:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1435:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1505:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1603:            catch (Exception exception)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1745:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1781:            catch (Exception exception)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1823:            catch (Exception exception)
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:93:            catch (Exception ex)
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:105:            catch (Exception ex)
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:152:            catch (Exception ex)
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:356:            catch (Exception exception)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:79:            catch (Exception)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:169:            catch (Exception ex)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:223:            catch (Exception ex)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:252:            catch (Exception ex)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:270:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:201:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:249:            catch (Exception)
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:46:            catch (Exception ex)
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:212:            catch (Exception ex)
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:217:                catch (Exception ex)
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:240:            catch (Exception)
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:89:            catch (Exception ex)
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:112:            catch (Exception exception)
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:236:            catch (Exception ex)
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:283:            catch (Exception ex)
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:83:            catch (Exception exception)
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:291:                    catch (Exception ex)
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:359:            catch (Exception exception)
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:349:            catch (Exception ex)
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:91:            catch (Exception ex)
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:331:            catch (Exception)
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:407:            catch (Exception ex)
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:436:            catch (Exception ex)
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:84:            catch (Exception ex)
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:107:            catch (Exception exception)
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:121:            catch (Exception ex)
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:155:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:165:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:287:            catch (Exception exception)
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:421:            catch (Exception ex)
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:468:            catch (Exception ex)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:312:            catch (Exception exception)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:358:            catch (Exception exception)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:746:            catch (Exception ex)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:88:            catch (Exception)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:249:            catch (Exception ex)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:355:            catch (Exception ex)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:401:            catch (Exception ex)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:418:            catch (Exception ex)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:486:            catch (Exception ex)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:138:            catch (Exception exception)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:194:                catch (Exception exception)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:381:            catch (Exception ex)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:703:            catch (Exception ex)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:752:            catch (Exception ex)
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:68:            catch (Exception ex)
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:91:            catch (Exception exception)
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:191:            catch (Exception ex)
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:203:            catch (Exception ex)
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:159:            catch (Exception ex)
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:292:            catch (Exception)
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:221:                catch (Exception ex)
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:244:            catch (Exception)
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:932:            catch (Exception exception)
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:966:            catch (Exception exception)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:157:            catch (Exception exception)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:337:            catch (Exception ex)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:355:            catch (Exception ex)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:391:            catch (Exception exception)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:435:            catch (Exception exception)
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:460:            catch (Exception ex)
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:493:            catch (Exception ex)
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:505:            catch (Exception ex)
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:160:            catch (Exception e)
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:336:            catch (Exception ex)
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:350:            catch (Exception ex)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:257:            catch (Exception ex)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:270:            catch (Exception ex)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:607:            catch (Exception ex)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:632:            catch (Exception exception)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:826:            catch (Exception ex)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:847:            catch (Exception ex)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:868:            catch (Exception ex)
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:146:            catch (Exception ex)
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:159:            catch (Exception ex)
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:181:            catch (Exception exception)
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:386:            catch (Exception ex)
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:433:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:48:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:77:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:171:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:532:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:552:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:579:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:56:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:123:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:147:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:160:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:182:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:379:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:426:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisHaritasiWP\TasinmazBagisHaritasiWP.ascx.cs:217:            catch (Exception exception)
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:172:            catch (Exception exception)
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:247:            catch (Exception exception)
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:452:            catch (Exception)
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:754:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:222:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:408:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:519:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:555:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:584:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:640:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:702:            catch (Exception exception)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:727:            catch (Exception ex)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:752:            catch (Exception ex)
TBYS_WebParts\TasinmazDurumRaporuWP\TasinmazDurumRaporuWP.ascx.cs:234:            catch (Exception ex)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:141:            catch (Exception ex)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:154:            catch (Exception ex)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:177:            catch (Exception exception)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:474:            catch (Exception exception)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:499:            catch (Exception exception)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:613:            catch (Exception ex)
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:660:            catch (Exception ex)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:490:            catch (Exception ex)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:973:            catch (Exception ex)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1111:            catch (Exception ex)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1214:            catch (Exception ex)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1346:            catch (Exception exception)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1429:            catch (Exception exception)
TBYS_WebParts\TasinmazOnarimGirisiWP\TasinmazOnarimGirisiWP.ascx.cs:268:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:361:            catch (Exception exception)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:556:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:570:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:607:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:635:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:675:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:793:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:829:            catch (Exception exception)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:871:            catch (Exception exception)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:290:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:306:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:337:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:544:            catch (Exception exception)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:671:            catch (Exception exception)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:698:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:722:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:746:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:770:            catch (Exception ex)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:794:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:424:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:503:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:550:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:591:            catch (Exception ex)
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:618:            catch (Exception exception)
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:41:            catch (Exception ex)
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:64:            catch (Exception exception)
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:137:            catch (Exception ex)
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:184:            catch (Exception ex)
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:43:            catch (Exception ex)
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:66:            catch (Exception exception)
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:130:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:161:            catch (Exception exception)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:280:            catch (Exception exception)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:406:                catch (Exception exception)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:636:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:707:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:729:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:747:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:762:            catch (Exception ex)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:784:            catch (Exception ex)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:393:            catch (Exception ex)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:408:            catch (Exception ex)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:472:            catch (Exception ex)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:529:            catch (Exception exception)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:664:            catch (Exception e1)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:690:            catch (Exception e1)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:731:            catch (Exception ex)
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:118:            catch (Exception ex)
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:141:            catch (Exception exception)
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:360:            catch (Exception ex)
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:407:            catch (Exception ex)
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:72:            catch (Exception ex)
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:176:            catch (Exception ex)
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:378:            catch (Exception ex)
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:403:            catch (Exception ex)
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:420:            catch (Exception ex)
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:102:            catch (Exception ex)
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:226:            catch (Exception exception)
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:260:            catch (Exception ex)
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:307:            catch (Exception ex)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:116:            catch (Exception)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:158:            catch (Exception)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:489:            catch (Exception ex)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:537:            catch (Exception)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:691:            catch (Exception)
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:119:            catch (Exception ex)
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:303:            catch (Exception ex)
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:425:            catch (Exception ex)
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:473:            catch (Exception ex)
~~~

## Transaction kullanım noktaları

~~~text
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:491:                using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1493:                    using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:309:                using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:386:            using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:448:                using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:575:            using (TransactionScope scope = new TransactionScope())
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:697:                using (TransactionScope scope = new TransactionScope())
~~~

## Audit adayları: servis ve repository yazma metotları

~~~text
DAO/Repositories/TBYS\BagimsizBolumRepository.cs:18:        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "BagimsizBolum_Table")); }
DAO/Repositories/TBYS\BagimsizBolumRepository.cs:19:        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "BagimsizBolum_Table")); }
DAO/Repositories/TBYS\BagimsizBolumRepository.cs:20:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagimsizBolum_Table", id), ""); }
DAO/Repositories/TBYS\BagisciTalepleriRepository.cs:14:        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciTalepleri_Table")); }
DAO/Repositories/TBYS\BagisciTalepleriRepository.cs:15:        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciTalepleri_Table")); }
DAO/Repositories/TBYS\BagisciTalepleriRepository.cs:16:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciTalepleri_Table", id), ""); }
DAO/Repositories/TBYS\BagisciBagisRepository.cs:14:        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciBagis_Table")); }
DAO/Repositories/TBYS\BagisciBagisRepository.cs:15:        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciBagis_Table")); }
DAO/Repositories/TBYS\BagisciBagisRepository.cs:16:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciBagis_Table", id), ""); }
DAO/Repositories/TBYS\BagisciYakinlariRepository.cs:14:        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "BagisciYakinlari_Table")); }
DAO/Repositories/TBYS\BagisciYakinlariRepository.cs:15:        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "BagisciYakinlari_Table")); }
DAO/Repositories/TBYS\BagisciYakinlariRepository.cs:16:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("BagisciYakinlari_Table", id), ""); }
DAO/Repositories/TBYS\KiraciRepository.cs:128:        public int Insert<T>(T entity) { return db.Insert(new CrudQueryBuilder().BuildInsert(entity, "Kiraci_Table")); }
DAO/Repositories/TBYS\KiraciRepository.cs:129:        public bool Update<T>(T entity) { return db.Update2Db(new CrudQueryBuilder().BuildUpdate(entity, "Kiraci_Table")); }
DAO/Repositories/TBYS\KiraciRepository.cs:130:        public bool Delete(int id) { return db.DeleteFromDb(new CrudQueryBuilder().BuildDelete("Kiraci_Table", id), ""); }
DAO/Repositories/TBYS\GecikmeZammiRepository.cs:112:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\GecikmeZammiRepository.cs:117:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\GecikmeZammiRepository.cs:122:        public bool Delete(int id)
DAO/Repositories/TBYS\YasalFaizRepository.cs:56:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\YasalFaizRepository.cs:61:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\YasalFaizRepository.cs:66:        public bool Delete(int id)
DAO/Repositories/TBYS\KiraEkstreAktarmaRepository.cs:159:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\KiraEkstreAktarmaRepository.cs:164:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\KiraEkstreAktarmaRepository.cs:169:        public bool Delete(int id)
DAO/Repositories/TBYS\VasiyeteKonuVarlikRepository.cs:47:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\VasiyeteKonuVarlikRepository.cs:52:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\VasiyeteKonuVarlikRepository.cs:57:        public bool Delete(int id)
DAO/Repositories/TBYS\BagisRepository.cs:113:        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Bagis_Table")); }
DAO/Repositories/TBYS\BagisRepository.cs:114:        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Bagis_Table")); }
DAO/Repositories/TBYS\BagisRepository.cs:115:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Bagis_Table", id), ""); }
DAO/Repositories/TBYS\HukukiTakipRepository.cs:77:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\HukukiTakipRepository.cs:82:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\HukukiTakipRepository.cs:87:        public bool Delete(int id)
DAO/Repositories/TBYS\HukukiTakipRepository.cs:92:        public bool DeleteBySozlesmeId(int sozlesmeId)
DAO/Repositories/TBYS\VasiyetciRepository.cs:89:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\VasiyetciRepository.cs:94:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\VasiyetciRepository.cs:99:        public bool Delete(int id)
DAO/Repositories/TBYS\KiraBorcuTakipRepository.cs:87:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\KiraBorcuTakipRepository.cs:92:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\KiraBorcuTakipRepository.cs:97:        public bool Delete(int id)
DAO/Repositories/TBYS\SerhBeyanIrtifakRepository.cs:43:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\SerhBeyanIrtifakRepository.cs:48:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\SerhBeyanIrtifakRepository.cs:53:        public bool Delete(int id)
DAO/Repositories/TBYS\OnarimRepository.cs:103:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OnarimRepository.cs:108:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OnarimRepository.cs:113:        public bool Delete(int id)
DAO/Repositories/TBYS\SigortaRepository.cs:161:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\SigortaRepository.cs:166:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\SigortaRepository.cs:171:        public bool Delete(int id)
Model/Services/TBYS\YasalFaizService.cs:31:        public int Save(YasalFaiz item)
Model/Services/TBYS\YasalFaizService.cs:40:        public bool Update(YasalFaiz item)
Model/Services/TBYS\YasalFaizService.cs:50:        public bool Delete(YasalFaiz item)
DAO/Repositories/TBYS\TasinmazBagisciRepository.cs:69:        public int Insert<T>(T item)
DAO/Repositories/TBYS\TasinmazBagisciRepository.cs:74:        public bool Update<T>(T item)
DAO/Repositories/TBYS\TasinmazBagisciRepository.cs:79:        public bool Delete(int id)
DAO/Repositories/TBYS\OdemeSebebiTanimRepository.cs:33:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OdemeSebebiTanimRepository.cs:38:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OdemeSebebiTanimRepository.cs:43:        public bool Delete(int id)
DAO/Repositories/TBYS\TeminatIslemRepository.cs:71:        public bool DeleteBySozlesmeId(int id)
DAO/Repositories/TBYS\TeminatIslemRepository.cs:78:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\TeminatIslemRepository.cs:83:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\TeminatIslemRepository.cs:88:        public bool Delete(int id)
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:40:        public int Save(VasiyeteKonuVarlik item)
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:57:        public bool Update(VasiyeteKonuVarlik item)
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:79:        public bool Delete(VasiyeteKonuVarlik item)
DAO/Repositories/TBYS\OdemeRepository.cs:185:        public bool DeleteBySozlesmeId(int id)
DAO/Repositories/TBYS\OdemeRepository.cs:192:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OdemeRepository.cs:197:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OdemeRepository.cs:202:        public bool Delete(int id)
Model/Services/TBYS\SozlesmeTasinmazService.cs:48:        public bool DeleteBySozlesmeId(SozlesmeTasinmaz item, int sozlesmeId)
Model/Services/TBYS\SozlesmeTasinmazService.cs:59:        public int Save(SozlesmeTasinmaz item)
Model/Services/TBYS\SozlesmeTasinmazService.cs:73:        public bool Update(SozlesmeTasinmaz item)
Model/Services/TBYS\SozlesmeTasinmazService.cs:92:        public bool Delete(SozlesmeTasinmaz item)
Model/Services/TBYS\VasiyetciService.cs:58:        public int Save(Vasiyetci item)
Model/Services/TBYS\VasiyetciService.cs:75:        public bool Update(Vasiyetci item)
Model/Services/TBYS\VasiyetciService.cs:97:        public bool Delete(Vasiyetci item)
DAO/Repositories/TBYS\OdemePlaniRepository.cs:80:        public bool DeleteBySozlesmeId(int id)
DAO/Repositories/TBYS\OdemePlaniRepository.cs:87:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OdemePlaniRepository.cs:92:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OdemePlaniRepository.cs:97:        public bool Delete(int id)
Model/Services/TBYS\HukukiTakipService.cs:50:        public int Save(HukukiTakip item)
Model/Services/TBYS\HukukiTakipService.cs:67:        public bool Update(HukukiTakip item)
Model/Services/TBYS\HukukiTakipService.cs:89:        public bool Delete(HukukiTakip item)
Model/Services/TBYS\HukukiTakipService.cs:106:        public bool DeleteBySozlesmeId(int sozlesmeId)
DAO/Repositories/TBYS\OdemeAyristirmaRepository.cs:48:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OdemeAyristirmaRepository.cs:53:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OdemeAyristirmaRepository.cs:58:        public bool Delete(int id)
DAO/Repositories/TBYS\TasinmazRepository.cs:231:        public int Insert<T>(T item) { return db.Insert(queryBuilder.BuildInsert(item, "Tasinmaz_Table")); }
DAO/Repositories/TBYS\TasinmazRepository.cs:232:        public bool Update<T>(T item) { return db.Update2Db(queryBuilder.BuildUpdate(item, "Tasinmaz_Table")); }
DAO/Repositories/TBYS\TasinmazRepository.cs:233:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("Tasinmaz_Table", id), ""); }
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:69:        public bool DeleteBySozlesmeId(int id)
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:76:        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId)
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:105:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:110:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\OdemeAyrintiRepository.cs:115:        public bool Delete(int id)
Model/Services/TBYS\BagisciTalepleriService.cs:19:        public int Save(BagisciTalepleri item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return item.Id; }
Model/Services/TBYS\BagisciTalepleriService.cs:20:        public bool Update(BagisciTalepleri item) { if (item == null) throw new ArgumentNullException("item"); BagisciTalepleri old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return ok; }
Model/Services/TBYS\BagisciTalepleriService.cs:21:        public bool Delete(BagisciTalepleri item) { if (item == null || item.Id == 0) return false; BagisciTalepleri old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return ok; }
DAO/Repositories/TBYS\SozlesmeTasinmazRepository.cs:125:        public bool DeleteBySozlesmeId(int sozlesmeId)
DAO/Repositories/TBYS\SozlesmeTasinmazRepository.cs:132:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\SozlesmeTasinmazRepository.cs:137:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\SozlesmeTasinmazRepository.cs:142:        public bool Delete(int id)
Model/Services/TBYS\GecikmeZammiService.cs:86:        public int Save(GecikmeZammi item)
Model/Services/TBYS\GecikmeZammiService.cs:103:        public bool Update(GecikmeZammi item)
Model/Services/TBYS\GecikmeZammiService.cs:125:        public bool Delete(GecikmeZammi item)
DAO/Repositories/TBYS\TasinmazTaahhutRepository.cs:115:        public int Insert<T>(T entity)
DAO/Repositories/TBYS\TasinmazTaahhutRepository.cs:120:        public bool Update<T>(T entity)
DAO/Repositories/TBYS\TasinmazTaahhutRepository.cs:125:        public bool Delete(int id)
Model/Services/TBYS\OdemeAyristirmaService.cs:27:        public int Save(OdemeAyristirma item)
Model/Services/TBYS\OdemeAyristirmaService.cs:36:        public bool Update(OdemeAyristirma item)
Model/Services/TBYS\OdemeAyristirmaService.cs:46:        public bool Delete(OdemeAyristirma item)
Model/Services/TBYS\KiraEkstreAktarmaService.cs:39:        public int Save(KiraEkstreAktarma item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return item.Id; }
Model/Services/TBYS\KiraEkstreAktarmaService.cs:40:        public bool Update(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
Model/Services/TBYS\KiraEkstreAktarmaService.cs:41:        public bool Delete(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
Model/Services/TBYS\BagisService.cs:37:        public int Save(Bagis item)
Model/Services/TBYS\BagisService.cs:46:        public bool Update(Bagis item)
Model/Services/TBYS\BagisService.cs:57:        public bool Delete(Bagis item)
Model/Services/TBYS\OnarimService.cs:77:        public int Save(Onarim item)
Model/Services/TBYS\OnarimService.cs:94:        public bool Update(Onarim item)
Model/Services/TBYS\OnarimService.cs:116:        public bool Delete(Onarim item)
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:44:        public int Insert<T>(T entity) { return db.Insert(queryBuilder.BuildInsert(entity, "KiraSozlesme_Table")); }
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:45:        public bool Update<T>(T entity) { return db.Update2Db(queryBuilder.BuildUpdate(entity, "KiraSozlesme_Table")); }
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:46:        public bool Delete(int id) { return db.DeleteFromDb(queryBuilder.BuildDelete("KiraSozlesme_Table", id), ""); }
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:47:        public bool UpdateByKiraciId(int bolgeId, int kiraciId)
DAO/Repositories/TBYS\KiraSozlesmeRepository.cs:54:        public bool UpdateActiveStatus(int id, bool aktif, string durum, string durumDegistirmeTar, string degistiren, DateTime degistirmeTarihi)
Model/Services/TBYS\SigortaService.cs:103:        public int Save(Sigorta item)
Model/Services/TBYS\SigortaService.cs:120:        public bool Update(Sigorta item)
Model/Services/TBYS\SigortaService.cs:142:        public bool Delete(Sigorta item)
Model/Services/TBYS\KiraciService.cs:38:        public int Save(Kiraci item)
Model/Services/TBYS\KiraciService.cs:47:        public bool Update(Kiraci item)
Model/Services/TBYS\KiraciService.cs:57:        public bool Delete(Kiraci item)
Model/Services/TBYS\BagisciBagisService.cs:19:        public int Save(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return item.Id; }
Model/Services/TBYS\BagisciBagisService.cs:20:        public bool Update(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); BagisciBagis old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
Model/Services/TBYS\BagisciBagisService.cs:21:        public bool Delete(BagisciBagis item) { if (item == null || item.Id == 0) return false; BagisciBagis old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
Model/Services/TBYS\OdemeService.cs:41:        public int Save(Odeme item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return item.Id; }
Model/Services/TBYS\OdemeService.cs:42:        public bool Update(Odeme item) { if (item == null) throw new ArgumentNullException("item"); Odeme old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
Model/Services/TBYS\OdemeService.cs:43:        public bool Delete(Odeme item) { if (item == null || item.Id == 0) return false; Odeme old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
Model/Services/TBYS\OdemeService.cs:44:        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
Model/Services/TBYS\KiraBorcuTakipService.cs:23:        public int Save(KiraBorcuTakip item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return item.Id; }
Model/Services/TBYS\KiraBorcuTakipService.cs:24:        public bool Update(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
Model/Services/TBYS\KiraBorcuTakipService.cs:25:        public bool Delete(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
Model/Services/TBYS\BagisciYakinlariService.cs:19:        public int Save(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return item.Id; }
Model/Services/TBYS\BagisciYakinlariService.cs:20:        public bool Update(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); BagisciYakinlari old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
Model/Services/TBYS\BagisciYakinlariService.cs:21:        public bool Delete(BagisciYakinlari item) { if (item == null || item.Id == 0) return false; BagisciYakinlari old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:27:        public int Save(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return item.Id; }
Model/Services/TBYS\OdemePlaniService.cs:28:        public bool Update(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); OdemePlani old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:29:        public bool Delete(OdemePlani item) { if (item == null || item.Id == 0) return false; OdemePlani old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:30:        public bool DeleteBySozlesmeId(OdemePlani item, int id) { bool ok = repository.DeleteBySozlesmeId(id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:64:        private void SaveNew(KiraSozlesme kiraSozlesme, int yil, string ay, DateTime vadeBaslangic, DateTime vadeBitis, DateTime odemeBaslangic, DateTime odemeBitis, decimal aylikKira, int sira)
Model/Services/TBYS\OdemePlaniService.cs:68:        private void SaveCarryOver(KiraSozlesme kiraSozlesme, decimal anaPara, decimal faizTutari, string aciklama, int sira)
Model/Services/TBYS\TasinmazTaahhutService.cs:72:        public int Save(TasinmazTaahhut item)
Model/Services/TBYS\TasinmazTaahhutService.cs:89:        public bool Update(TasinmazTaahhut item)
Model/Services/TBYS\TasinmazTaahhutService.cs:111:        public bool Delete(TasinmazTaahhut item)
Model/Services/TBYS\SerhBeyanIrtifakService.cs:40:        public int Save(SerhBeyanIrtifak item)
Model/Services/TBYS\SerhBeyanIrtifakService.cs:57:        public bool Update(SerhBeyanIrtifak item)
Model/Services/TBYS\SerhBeyanIrtifakService.cs:79:        public bool Delete(SerhBeyanIrtifak item)
Model/Services/TBYS\OdemeAyrintiService.cs:28:        public int Save(OdemeAyrinti item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return item.Id; }
Model/Services/TBYS\OdemeAyrintiService.cs:29:        public bool Update(OdemeAyrinti item) { if (item == null) throw new ArgumentNullException("item"); OdemeAyrinti old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return ok; }
Model/Services/TBYS\OdemeAyrintiService.cs:30:        public bool Delete(OdemeAyrinti item) { if (item == null || item.Id == 0) return false; OdemeAyrinti old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return ok; }
Model/Services/TBYS\OdemeAyrintiService.cs:31:        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
Model/Services/TBYS\OdemeAyrintiService.cs:32:        public bool DeleteByOdemeIdOdemePlaniId(int odemeId, int planId) { return repository.DeleteByOdemeIdOdemePlaniId(odemeId, planId); }
Model/TBYS\OdemePlani.cs:103:        private OdemePlani SaveNew(KiraSozlesme kiraSozlesme, int pYil, string pAy, DateTime pVadeBasTar, DateTime pVadeBitTar,
Model/TBYS\OdemePlani.cs:124:        private OdemePlani SaveDevir(KiraSozlesme kiraSozlesme, decimal devirAnaPara, decimal devirFaizTutari, string aciklama, int sira)
Model/Services/TBYS\TeminatIslemService.cs:25:        public int Save(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return item.Id; }
Model/Services/TBYS\TeminatIslemService.cs:26:        public bool Update(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); TeminatIslem old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
Model/Services/TBYS\TeminatIslemService.cs:27:        public bool Delete(TeminatIslem item) { if (item == null || item.Id == 0) return false; TeminatIslem old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
Model/Services/TBYS\TeminatIslemService.cs:28:        public bool DeleteBySozlesmeId(int id) { return repository.DeleteBySozlesmeId(id); }
Model/Services/TBYS\OdemeSebebiTanimService.cs:28:        public int Save(OdemeSebebiTanim item)
Model/Services/TBYS\OdemeSebebiTanimService.cs:42:        public bool Update(OdemeSebebiTanim item)
Model/Services/TBYS\OdemeSebebiTanimService.cs:61:        public bool Delete(OdemeSebebiTanim item)
Model/Services/TBYS\TasinmazBagisciService.cs:24:        public int Save(TasinmazBagisci item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); item.Id=repository.Insert(item); if(item.Id>0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return item.Id; }
Model/Services/TBYS\TasinmazBagisciService.cs:25:        public bool Update(TasinmazBagisci item) { if(item==null) throw new ArgumentNullException("item"); TasinmazBagisci old=GetById(item.Id); if(item.Id==0) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); bool ok=repository.Update(item); if(ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item,old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
Model/Services/TBYS\TasinmazBagisciService.cs:26:        public bool Delete(TasinmazBagisci item) { if(item==null || item.Id==0) return false; TasinmazBagisci old=GetById(item.Id); if(old==null) return false; bool ok=repository.Delete(item.Id); if(ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
Model/Services/TBYS\BagimsizBolumService.cs:23:        public int Save(BagimsizBolum item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return item.Id; }
Model/Services/TBYS\BagimsizBolumService.cs:24:        public bool Update(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
Model/Services/TBYS\BagimsizBolumService.cs:25:        public bool Delete(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
Model/Services/TBYS\KiraSozlesmeService.cs:29:        public int Save(KiraSozlesme item)
Model/Services/TBYS\KiraSozlesmeService.cs:38:        public bool Update(KiraSozlesme item)
Model/Services/TBYS\KiraSozlesmeService.cs:52:        public bool Delete(KiraSozlesme item)
Model/Services/TBYS\KiraSozlesmeService.cs:61:        public bool UpdateByKiraciId(int bolgeId, int kiraciId) { return repository.UpdateByKiraciId(bolgeId, kiraciId); }
Model/Services/TBYS\KiraSozlesmeService.cs:62:        public bool UpdateActiveStatus(KiraSozlesme item, string durum, string degistirmeTar, bool aktif)
Model/Services/TBYS\TasinmazService.cs:144:        public int Save(Tasinmaz item)
Model/Services/TBYS\TasinmazService.cs:154:        public bool Update(Tasinmaz item)
Model/Services/TBYS\TasinmazService.cs:166:        public bool Delete(Tasinmaz item)
~~~

## WebPart durum, onay ve yazma akışları

~~~text
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:344:        private void BagimsizBolumSilModalAc(Tasinmaz tasinmaz, BagimsizBolum bagimsizBolum)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:385:        private bool BagimsizBolumSilinebilirMi(BagimsizBolum bagimsizBolum)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:396:        protected void KaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:420:        protected void GuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:465:        protected void SilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:212:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:202:        protected void TalepKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:229:        protected void TalepGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:275:        protected void TalepSilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:518:        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:940:        protected void TakipIslemiGuncelleNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:323:        protected void TaahhutKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:361:        protected void TaahhutGuncelle_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:423:        protected void TaahhutSilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:326:        private bool BagisSil(Bagis bagis)
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:261:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:26:        public KiraBakiyeDevriWP()
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:244:        private void OdemeleriHesaplaOdemePlaniniGuncelle(int kiraciId)
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:299:        protected void SilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:245:        protected void SecilenListeyiKaydet()
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:329:        private List<KiraEkstreAktarmaListItem> GetDataList()
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:659:        protected void AktarilanlarHaricChk_CheckedChanged(object sender, EventArgs e)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:669:        protected void SecilenleriKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:689:        protected void KaydetNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:176:        private Kiraci SaveKiraciData2Db()
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:202:        private Kiraci UpdateKiraciData2Db()
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:435:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:455:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:478:        protected void SilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:595:        protected void YeniSozlesmeOnayBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:612:        protected void BitenSozlesmeOnayBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:850:        protected void DeleteNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:865:        private bool KiraciSilinebilirMi(Kiraci kiraci)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:84:        protected void KaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:96:        private void Vakifbank2Save()
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:113:        public ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:248:        protected void DosyayaKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:443:        protected void KaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:468:        protected void GuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:493:        protected void SilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:518:        protected void OnaylaBtn_Click(object sender, EventArgs e)
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:203:        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\SozlesmeTasinmazWP\SozlesmeTasinmazWP.ascx.cs:272:        private void SecileniKaydet(KiraSozlesme kiraSozlesme)
TBYS_WebParts\SozlesmeTasinmazWP\SozlesmeTasinmazWP.ascx.cs:318:        private void SecilenKiraciyiKaydet(KiraSozlesme kiraSozlesme)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:295:        protected void KaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:304:        private void OdemeAyristirmaAtomicKaydet(KiraEkstreAktarma kiraEkstreAktarma)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:565:        protected void ModalGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:570:        private void OdemeGuncelleModalAc()
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:594:        protected void ListeyiGuncelleNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:681:        protected void SatirSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:740:        protected void KiraEkstreAktarmaBtn_Click(object sender, EventArgs e)
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:23:        public SigortaEkleSilWP()
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:381:        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:221:        protected void KaydetNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:360:        private Tasinmaz SaveTasinmazData2Db()
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:471:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:490:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:561:        protected void DeleteBtn_Click(object sender, EventArgs e)
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:233:        private bool DosyayiSPListesindenSil(string libName, string fileName)
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:282:        protected void DosyayiSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:289:        protected void DosyayiSilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:501:        private Tasinmaz SaveTasinmazData2Db()
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:617:        private bool UpdateTasinmazData2Db(int tId)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:748:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:764:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:883:        protected void DegerleriKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:959:        protected void DeleteBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1015:        protected void DeleteNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1088:        private void TasinmaziSil(Tasinmaz silinecekTasinmaz)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1601:        protected void DuzenleSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1608:        protected void SBIKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1640:        protected void SBIDeleteBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1669:        protected void SBIGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:778:        private bool OdemePlaniniKontrolEtVeSil(KiraSozlesme kiraSozlesme)
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:813:        protected void OdemePlaniSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:509:        protected void HepsiniKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:543:        protected void Kaydet1Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:433:        private bool UpdateSigorta(Sigorta sigorta)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:476:        private Sigorta SaveSigorta()
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:588:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:613:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:641:        protected void SilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:654:        protected void SilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:756:        protected void BelgeSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:799:        private void PDFKaydet(Sigorta sigorta)
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:322:        protected void OdemePlanlariniGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:317:        private TasinmazBagisci SaveBagisciData2Db()
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:359:        private TasinmazBagisci UpdateBagisciData2Db()
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:414:        private void SaveImageFiles2SP(string fotoFile)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:442:        protected void DeleteBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:529:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:561:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:604:        private void PDFKaydet(int bagisciId)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:711:        protected void BelgeSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:734:        protected void TaahhutSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:451:        private bool UpdateKiraSozlesme()
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:486:        private KiraSozlesme UpdateKiraSozlesmeData2Db()
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:560:        private void SozlesmeSil()
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:610:        private bool SozlesmeGuncelle(KiraSozlesme kiraSozlesme)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:637:        private bool OdemePlaniSil(KiraSozlesme kiraSozlesme)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:801:        private bool HukukiIslemleriGuncelle(KiraSozlesme kiraSozlesme)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1209:        protected void UpdateBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1242:        protected void DeleteBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1260:        protected void OnaylaBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1714:        protected void BelgeSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1751:        private void PDFKaydet(KiraSozlesme kiraSozlesme)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:174:        protected void SaveBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:344:        protected void ResimSil1Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:348:        protected void ResimSil2Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:352:        protected void ResimSil3Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:356:        protected void ResimSil4Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:360:        protected void ResimSil5Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:364:        protected void ResimSil6Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:368:        protected void ResimSil7Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:372:        protected void ResimSil8Btn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:376:        private void ResmiSil(string foto)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:454:        private void PDFKaydet(int tasinmazId)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:680:        protected void EmlakBeyaniSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:704:        protected void YapiKayitSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:728:        protected void TapuKayitSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:752:        protected void ImarDurumuSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:776:        protected void TasinmazRaporuSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:341:        protected void KaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:414:        protected void GuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:538:        protected void VarlikKaydetBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:634:        private void VasiyetciyiSilPopupAc(object sender)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:644:        protected void VasiyetciSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:673:        protected void VasiyetciSilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:699:        protected void NiteligiSilNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:379:        protected void TeminatGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:419:        private void OdemeyiGuncelle(KiraSozlesme kiraSozlesme, DateTime odemeTarihi, decimal odenenTutar, int odemeId, string aciklama)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:463:        protected void ModalGuncelleBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:486:        protected void ModalSilBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:542:        protected void ModalGuncelleNowBtn_Click(object sender, EventArgs e)
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:589:        protected void ModalSilNowBtn_Click(object sender, EventArgs e)
~~~

## Eski log ve hata yardımcıları

~~~text
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:24:            if (!PortalLogSettings.IsEnabled(context)) { new ExceptionHelper(error).PublishException(); return; }
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:23:            if (!PortalLogSettings.IsEnabled(context)) { new ExceptionHelper(error).PublishException(); return; }
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:27:            if (!PortalLogSettings.IsEnabled(context)) { new ExceptionHelper(error).PublishException(); return; }
Model/Services/TBYS\BagisciBagisService.cs:19:        public int Save(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return item.Id; }
Model/Services/TBYS\BagisciBagisService.cs:20:        public bool Update(BagisciBagis item) { if (item == null) throw new ArgumentNullException("item"); BagisciBagis old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
Model/Services/TBYS\BagisciBagisService.cs:21:        public bool Delete(BagisciBagis item) { if (item == null || item.Id == 0) return false; BagisciBagis old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIBAGIS); return ok; }
Model/Services/TBYS\BagisciTalepleriService.cs:19:        public int Save(BagisciTalepleri item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return item.Id; }
Model/Services/TBYS\BagisciTalepleriService.cs:20:        public bool Update(BagisciTalepleri item) { if (item == null) throw new ArgumentNullException("item"); BagisciTalepleri old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return ok; }
Model/Services/TBYS\BagisciTalepleriService.cs:21:        public bool Delete(BagisciTalepleri item) { if (item == null || item.Id == 0) return false; BagisciTalepleri old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCITALEPLERI); return ok; }
Model/Services/TBYS\BagimsizBolumService.cs:23:        public int Save(BagimsizBolum item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return item.Id; }
Model/Services/TBYS\BagimsizBolumService.cs:24:        public bool Update(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
Model/Services/TBYS\BagimsizBolumService.cs:25:        public bool Delete(BagimsizBolum item) { if (item == null || item.Id == 0) return false; BagimsizBolum old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIMSIZBOLUM); return ok; }
Model/Services/TBYS\BagisciYakinlariService.cs:19:        public int Save(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return item.Id; }
Model/Services/TBYS\BagisciYakinlariService.cs:20:        public bool Update(BagisciYakinlari item) { if (item == null) throw new ArgumentNullException("item"); BagisciYakinlari old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
Model/Services/TBYS\BagisciYakinlariService.cs:21:        public bool Delete(BagisciYakinlari item) { if (item == null || item.Id == 0) return false; BagisciYakinlari old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGISCIYAKINLARI); return ok; }
Model/Services/TBYS\GecikmeZammiService.cs:96:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\GecikmeZammiService.cs:118:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\GecikmeZammiService.cs:135:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\BagisService.cs:43:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
Model/Services/TBYS\BagisService.cs:54:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
Model/Services/TBYS\BagisService.cs:63:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_BAGIS);
Model/Services/TBYS\HukukiTakipService.cs:60:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\HukukiTakipService.cs:82:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\HukukiTakipService.cs:99:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\KiraBorcuTakipService.cs:23:        public int Save(KiraBorcuTakip item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return item.Id; }
Model/Services/TBYS\KiraBorcuTakipService.cs:24:        public bool Update(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
Model/Services/TBYS\KiraBorcuTakipService.cs:25:        public bool Delete(KiraBorcuTakip item) { if (item == null || item.Id == 0) return false; KiraBorcuTakip old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRABORCUTAKIP); return ok; }
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:194:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:195:                exHelper.PublishException();
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:208:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BagisciYakinlariWP\BagisciYakinlariWP.ascx.cs:209:                exHelper.PublishException();
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:23:            if (!PortalLogSettings.IsEnabled(context)) { new ExceptionHelper(error).PublishException(); return; }
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:93:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:94:                exh.PublishException();
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:409:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:410:                exHelper.PublishException();
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:438:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiracilarByBolgeWP\BorcluKiracilarByBolgeWP.ascx.cs:439:                exHelper.PublishException();
Model/Services/TBYS\KiraSozlesmeService.cs:35:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
Model/Services/TBYS\KiraSozlesmeService.cs:49:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
Model/Services/TBYS\KiraSozlesmeService.cs:58:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRASOZLESME);
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:136:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:137:                exh.PublishException();
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:329:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:330:                exHelper.PublishException();
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:358:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiraciTakibiByBolgeWP\BorcluKiraciTakibiByBolgeWP.ascx.cs:359:                exHelper.PublishException();
Model/Services/TBYS\KiraEkstreAktarmaService.cs:39:        public int Save(KiraEkstreAktarma item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return item.Id; }
Model/Services/TBYS\KiraEkstreAktarmaService.cs:40:        public bool Update(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); bool ok = repository.Update(item); if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
Model/Services/TBYS\KiraEkstreAktarmaService.cs:41:        public bool Delete(KiraEkstreAktarma item) { if (item == null || item.Id == 0) return false; KiraEkstreAktarma old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRAEKSTREAKTARMA); return ok; }
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:74:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:75:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:99:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:101:                exceptionHelper.PublishException();
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:231:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:232:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:280:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikanTasinmazListesiWP\EnvanterdenCikanTasinmazListesiWP.ascx.cs:281:                exHelper.PublishException();
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:177:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:178:                exHelper.PublishException();
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:199:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:200:                exHelper.PublishException();
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:213:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:214:                exHelper.PublishException();
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:490:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:491:                exHelper.PublishException();
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:510:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:511:                exHelper.PublishException();
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:528:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:529:                exHelper.PublishException();
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:543:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:544:                exHelper.PublishException();
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:557:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:558:                exHelper.PublishException();
Model/Services/TBYS\KiraciService.cs:44:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
Model/Services/TBYS\KiraciService.cs:54:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
Model/Services/TBYS\KiraciService.cs:63:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_KIRACI);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:172:                ExceptionHelper exh = new ExceptionHelper(exception);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:218:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:219:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:232:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:233:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:257:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:258:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:289:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:291:                exHelper.PublishException();
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:342:                ExceptionHelper eh= new ExceptionHelper(ex);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:343:                eh.PublishException();
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:224:                    ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:225:                    exh.PublishException();
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:934:                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:969:                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:972:                exceptionHelper.PublishException();
Model/Services/TBYS\OdemeAyristirmaService.cs:33:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
Model/Services/TBYS\OdemeAyristirmaService.cs:43:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
Model/Services/TBYS\OdemeAyristirmaService.cs:52:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRISTIRMA);
TBYS_WebParts\EmptyWP\EmptyWP.ascx.cs:44:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\EmptyWP\EmptyWP.ascx.cs:45:                exHelper.PublishException();
Model/Services/TBYS\OdemeService.cs:41:        public int Save(Odeme item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return item.Id; }
Model/Services/TBYS\OdemeService.cs:42:        public bool Update(Odeme item) { if (item == null) throw new ArgumentNullException("item"); Odeme old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
Model/Services/TBYS\OdemeService.cs:43:        public bool Delete(Odeme item) { if (item == null || item.Id == 0) return false; Odeme old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEME); return ok; }
Model/Services/TBYS\OdemeService.cs:73:                ExceptionHelper exHelper = new ExceptionHelper();
Model/Services/TBYS\OdemeService.cs:76:                exHelper.PublishException();
Model/Services/TBYS\OdemeService.cs:111:                ExceptionHelper exHelper = new ExceptionHelper();
Model/Services/TBYS\OdemeService.cs:114:                exHelper.PublishException();
Model/Services/TBYS\OdemeService.cs:140:                        new ExceptionHelper(new Exception("Ödeme Tablosunda OdemePlaniId=0 oldugundan kayit yapilamadi")).PublishException();
Model/Services/TBYS\OdemeService.cs:179:                ExceptionHelper exHelper = new ExceptionHelper();
Model/Services/TBYS\OdemeService.cs:182:                exHelper.PublishException();
Model/Services/TBYS\OdemePlaniService.cs:27:        public int Save(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return item.Id; }
Model/Services/TBYS\OdemePlaniService.cs:28:        public bool Update(OdemePlani item) { if (item == null) throw new ArgumentNullException("item"); OdemePlani old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:29:        public bool Delete(OdemePlani item) { if (item == null || item.Id == 0) return false; OdemePlani old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemePlaniService.cs:30:        public bool DeleteBySozlesmeId(OdemePlani item, int id) { bool ok = repository.DeleteBySozlesmeId(id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEPLANI); return ok; }
Model/Services/TBYS\OdemeAyrintiService.cs:28:        public int Save(OdemeAyrinti item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return item.Id; }
Model/Services/TBYS\OdemeAyrintiService.cs:29:        public bool Update(OdemeAyrinti item) { if (item == null) throw new ArgumentNullException("item"); OdemeAyrinti old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return ok; }
Model/Services/TBYS\OdemeAyrintiService.cs:30:        public bool Delete(OdemeAyrinti item) { if (item == null || item.Id == 0) return false; OdemeAyrinti old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_ODEMEAYRINTI); return ok; }
Model/Services/TBYS\OdemeSebebiTanimService.cs:36:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\OdemeSebebiTanimService.cs:55:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\OdemeSebebiTanimService.cs:69:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\OnarimService.cs:87:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\OnarimService.cs:109:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\OnarimService.cs:126:                OlayKayit olayKayit = new OlayKayit();
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:220:                    ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:221:                    exh.PublishException();
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:203:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:205:                exceptionHelper.PublishException();
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:375:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:376:                exh.PublishException();
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:413:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:414:                exh.PublishException();
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:48:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:49:                exHelper.PublishException();
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:214:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\HukukiTakipListesiWP\HukukiTakipListesiWP.ascx.cs:215:                exHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:124:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:125:                exHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:147:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:149:                exceptionHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:311:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:312:                exHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:335:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:336:                exHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:353:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:354:                exHelper.PublishException();
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:378:                ExceptionHelper ex = new ExceptionHelper(exception);
TBYS_WebParts\KiraciEslestirWP\KiraciEslestirWP.ascx.cs:379:                ex.PublishException();
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:95:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:96:                exHelper.PublishException();
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:107:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:108:                exHelper.PublishException();
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:154:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:155:                exHelper.PublishException();
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:358:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:360:                exceptionHelper.PublishException();
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:203:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:204:                exHelper.PublishException();
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:92:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:93:                exHelper.PublishException();
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:115:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:117:                exceptionHelper.PublishException();
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:220:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:221:                exHelper.PublishException();
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:254:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraArtisCizelgesiWP\KiraArtisCizelgesiWP.ascx.cs:255:                exHelper.PublishException();
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:207:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:208:                exHelper.PublishException();
Model/Services/TBYS\SerhBeyanIrtifakService.cs:50:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SerhBeyanIrtifakService.cs:72:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SerhBeyanIrtifakService.cs:89:                OlayKayit olayKayit = new OlayKayit();
Model/TBYS\TBYSOrtak.cs:96:                        ExceptionHelper exhelper = new ExceptionHelper();
Model/TBYS\TBYSOrtak.cs:100:                        exhelper.PublishException();
Model/TBYS\TBYSOrtak.cs:581:                ExceptionHelper exHelper = new ExceptionHelper(ex);
Model/TBYS\TBYSOrtak.cs:583:                exHelper.PublishException();
Model/Services/TBYS\SigortaService.cs:113:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SigortaService.cs:135:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SigortaService.cs:152:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SozlesmeTasinmazService.cs:53:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SozlesmeTasinmazService.cs:67:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SozlesmeTasinmazService.cs:86:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\SozlesmeTasinmazService.cs:100:                OlayKayit olayKayit = new OlayKayit();
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:387:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:388:                exh.PublishException();
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:426:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:427:                exh.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:211:                    ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:212:                    exHelper.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:235:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:237:                exceptionHelper.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:261:                        exceptionHelper.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:274:        public ExceptionHelper OdemeIslemleriniYap(List<KiraEkstreAktarma> listEkstreAktarma, string currentUser)
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:276:            ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:574:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:575:                exHelper.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:592:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:593:                exHelper.PublishException();
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:697:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:698:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:167:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:168:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:289:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:291:                exceptionHelper.PublishException();
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:423:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:424:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:470:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmeListesiWP\KiraSozlesmeListesiWP.ascx.cs:471:                exHelper.PublishException();
Model/Services/TBYS\TasinmazBagisciService.cs:24:        public int Save(TasinmazBagisci item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi=DateTime.Now; item.Olusturan=UtilityHelper.GetCurrentUserName(); item.Id=repository.Insert(item); if(item.Id>0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return item.Id; }
Model/Services/TBYS\TasinmazBagisciService.cs:25:        public bool Update(TasinmazBagisci item) { if(item==null) throw new ArgumentNullException("item"); TasinmazBagisci old=GetById(item.Id); if(item.Id==0) return false; item.DegistirmeTarihi=DateTime.Now; item.Degistiren=UtilityHelper.GetCurrentUserName(); bool ok=repository.Update(item); if(ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item,old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
Model/Services/TBYS\TasinmazBagisciService.cs:26:        public bool Delete(TasinmazBagisci item) { if(item==null || item.Id==0) return false; TasinmazBagisci old=GetById(item.Id); if(old==null) return false; bool ok=repository.Delete(item.Id); if(ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old,ProjeConstants.TBYS,ProjeConstants.TBYS_TASINMAZBAGISCI); return ok; }
Model/Services/TBYS\TasinmazService.cs:150:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
Model/Services/TBYS\TasinmazService.cs:162:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
Model/Services/TBYS\TasinmazService.cs:172:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(oldItem, ProjeConstants.TBYS, ProjeConstants.TBYS_TASINMAZ);
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:86:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:87:                exHelper.PublishException();
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:109:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:111:                exceptionHelper.PublishException();
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:123:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:124:                exHelper.PublishException();
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:157:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\GerceklesenKiraArtislariWP\GerceklesenKiraArtislariWP.ascx.cs:158:                exHelper.PublishException();
Model/Services/TBYS\TasinmazTaahhutService.cs:82:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\TasinmazTaahhutService.cs:104:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\TasinmazTaahhutService.cs:121:                OlayKayit olayKayit = new OlayKayit();
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:137:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:138:                exHelper.PublishException();
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:181:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:183:                exceptionHelper.PublishException();
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:363:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:364:                exHelper.PublishException();
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:409:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciListesiWP\KiraciListesiWP.ascx.cs:410:                exHelper.PublishException();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:260:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:262:                exceptionHelper.PublishException();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:288:				ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:290:				exceptionHelper.PublishException();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:333:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:334:                exHelper.PublishException();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:519:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:520:                exHelper.PublishException();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:663:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:665:                exceptionHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:333:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:335:                exceptionHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:451:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:452:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:732:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:733:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:752:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:753:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:770:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:771:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:785:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:786:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:799:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:800:                exHelper.PublishException();
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:813:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:814:                exHelper.PublishException();
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:462:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:463:                exHelper.PublishException();
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:495:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:496:                exHelper.PublishException();
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:507:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:508:                exHelper.PublishException();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:64:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:65:                exHelper.PublishException();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:79:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:80:                exHelper.PublishException();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:92:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:93:                exHelper.PublishException();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:104:                    exceptionHelper.PublishException();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:113:        public ExceptionHelper SaveVakifBank2File(Stream fileStream, DateTime processTime, string currentUser)
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:115:            ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:531:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraOdemeDosyasiYuklemeWP\KiraOdemeDosyasiYuklemeWP.ascx.cs:532:                exHelper.PublishException();
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:315:                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:316:                exceptionHelper.PublishException();
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:361:                ExceptionHelper exceptionHelper = new ExceptionHelper(exception);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:362:                exceptionHelper.PublishException();
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:748:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:749:                exHelper.PublishException();
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:91:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:92:                exHelper.PublishException();
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:114:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:116:                exceptionHelper.PublishException();
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:238:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:239:                exHelper.PublishException();
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:285:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\MulkiyetiOlmayanTasinmazListesiWP\MulkiyetiOlmayanTasinmazListesiWP.ascx.cs:286:                exHelper.PublishException();
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:162:                ExceptionHelper eh = new ExceptionHelper(e);
TBYS_WebParts\SharePointLibraryFileListWP\SharePointLibraryFileListWP.ascx.cs:163:                eh.PublishException();
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:70:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:71:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:93:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:95:                exceptionHelper.PublishException();
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:193:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:194:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:205:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniListesiWP\OdemePlaniListesiWP.ascx.cs:206:                exHelper.PublishException();
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:85:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:87:                exceptionHelper.PublishException();
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:294:                        ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:295:                        exh.PublishException();
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:361:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KPSSorguListesiWP\KPSSorguListesiWP.ascx.cs:363:                exceptionHelper.PublishException();
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:351:                ExceptionHelper exhelper = new ExceptionHelper();
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:355:                exhelper.PublishException();
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:141:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:142:                exhelper.PublishException();
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:196:                    ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:198:                    exceptionHelper.PublishException();
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:705:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:706:                exHelper.PublishException();
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:754:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:755:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:609:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:610:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:634:                ExceptionHelper exHelper = new ExceptionHelper(exception);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:636:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:732:            //    ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:734:            //    exHelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:828:                ExceptionHelper exhelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:829:                exhelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:849:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:850:                exHelper.PublishException();
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:870:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:871:                exHelper.PublishException();
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:162:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:163:                exh.PublishException();
Model/Services/TBYS\TeminatIslemService.cs:25:        public int Save(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); item.OlusturmaTarihi = DateTime.Now; item.Olusturan = UtilityHelper.GetCurrentUserName(); item.Id = repository.Insert(item); if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return item.Id; }
Model/Services/TBYS\TeminatIslemService.cs:26:        public bool Update(TeminatIslem item) { if (item == null) throw new ArgumentNullException("item"); TeminatIslem old = GetById(item.Id); bool ok = false; if (item.Id != 0) { item.DegistirmeTarihi = DateTime.Now; item.Degistiren = UtilityHelper.GetCurrentUserName(); ok = repository.Update(item); } if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
Model/Services/TBYS\TeminatIslemService.cs:27:        public bool Delete(TeminatIslem item) { if (item == null || item.Id == 0) return false; TeminatIslem old = GetById(item.Id); if (old == null) return false; bool ok = repository.Delete(item.Id); if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_TEMINATISLEM); return ok; }
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:50:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:51:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:79:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:81:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:173:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciBilgiListesiWP\TasinmazBagisciBilgiListesiWP.ascx.cs:174:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:162:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:163:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:184:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:186:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:381:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:382:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:428:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:429:                exHelper.PublishException();
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:161:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:162:                exHelper.PublishException();
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:183:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:185:                exceptionHelper.PublishException();
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:388:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:389:                exHelper.PublishException();
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:435:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:436:                exHelper.PublishException();
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:338:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:339:                exHelper.PublishException();
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:352:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\SigortaEkleSilWP\SigortaEkleSilWP.ascx.cs:353:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:534:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:535:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:554:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:555:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:581:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciKartiWP\TasinmazBagisciKartiWP.ascx.cs:582:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:603:                ExceptionHelper exhelper = new ExceptionHelper(e);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:604:                exhelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:632:                ExceptionHelper exhelper = new ExceptionHelper(e);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:633:                exhelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:686:            ExceptionHelper exHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:714:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:821:                ExceptionHelper exHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:847:                ExceptionHelper exHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1379:                ExceptionHelper exHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1403:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1404:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1419:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1420:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1437:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1438:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1507:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1508:                exHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1605:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1607:                exceptionHelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1747:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1748:                eh.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1784:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1786:                exhelper.PublishException();
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1826:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1828:                exhelper.PublishException();
Model/Services/TBYS\YasalFaizService.cs:37:            if (item.Id > 0 && ProjeConstants.TBYS_SAVE_LOG) new OlayKayit().GirisOlayKaydet(item, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
Model/Services/TBYS\YasalFaizService.cs:47:            if (ok && ProjeConstants.TBYS_UPDATE_LOG) new OlayKayit().GuncellemeOlayKaydet(item, old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
Model/Services/TBYS\YasalFaizService.cs:56:            if (ok && ProjeConstants.TBYS_DELETE_LOG) new OlayKayit().SilmeOlayKaydet(old, ProjeConstants.TBYS, ProjeConstants.TBYS_YASALFAIZ);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:174:                ExceptionHelper exHelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:175:                exHelper.PublishException();
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:250:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:252:                exhelper.PublishException();
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:756:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:757:                exHelper.PublishException();
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:50:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:72:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\VasiyeteKonuVarlikService.cs:89:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\VasiyetciService.cs:68:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\VasiyetciService.cs:90:                OlayKayit olayKayit = new OlayKayit();
Model/Services/TBYS\VasiyetciService.cs:107:                OlayKayit olayKayit = new OlayKayit();
TBYS_WebParts\TasinmazBagisHaritasiWP\TasinmazBagisHaritasiWP.ascx.cs:220:                ExceptionHelper eh = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazBagisHaritasiWP\TasinmazBagisHaritasiWP.ascx.cs:221:                eh.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:224:                ExceptionHelper ex = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:225:                ex.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:410:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:411:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:418:            ExceptionHelper exhelper = new ExceptionHelper();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:426:                exhelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:521:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:522:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:557:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:558:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:586:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:587:                exHelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:643:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:645:                exhelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:705:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:707:                exhelper.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:729:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:730:                eh.PublishException();
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:754:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:755:                eh.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:492:                ExceptionHelper exhelper = new ExceptionHelper();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:496:                exhelper.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:975:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:976:                exHelper.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1022:            ExceptionHelper eh = new ExceptionHelper();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1113:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1114:                exHelper.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1216:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1217:                exHelper.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1235:            ExceptionHelper eh = new ExceptionHelper();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1348:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1350:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1431:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1433:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:58:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:60:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:125:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazBagisWP\TasinmazBagisWP.ascx.cs:127:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:426:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:427:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:505:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:506:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:552:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:553:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:593:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:594:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:620:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:622:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazDurumRaporuWP\TasinmazDurumRaporuWP.ascx.cs:236:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazDurumRaporuWP\TasinmazDurumRaporuWP.ascx.cs:237:                exHelper.PublishException();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:410:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:411:                exHelper.PublishException();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:532:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:534:                exhelper.PublishException();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:666:                ExceptionHelper eh = new ExceptionHelper();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:670:                eh.PublishException();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:692:                ExceptionHelper eh = new ExceptionHelper();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:696:                eh.PublishException();
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:733:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:734:                exHelper.PublishException();
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:104:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:105:                exHelper.PublishException();
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:228:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:230:                exceptionHelper.PublishException();
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:262:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:263:                exHelper.PublishException();
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:309:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\YilBazindaSozlesmeWP\YilBazindaSozlesmeWP.ascx.cs:310:                exHelper.PublishException();
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:305:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:306:                exHelper.PublishException();
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:427:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:428:                exHelper.PublishException();
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:475:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:476:                exHelper.PublishException();
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:74:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:75:                exHelper.PublishException();
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:178:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:179:                exHelper.PublishException();
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:381:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:382:                exh.PublishException();
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:406:                ExceptionHelper exh = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:407:                exh.PublishException();
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:422:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:423:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:164:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:165:                exhelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:282:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:284:                exceptionHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:409:                    ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:410:                    exhelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:638:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:639:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:709:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:710:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:731:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:732:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:749:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:750:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:764:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:765:                exHelper.PublishException();
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:786:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:787:                exHelper.PublishException();
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:491:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:492:                exHelper.PublishException();
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:68:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:70:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:132:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:133:                exHelper.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:180:                ExceptionHelper exhelper = updateTasinmazFoto2Db(tasinmaz);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:187:                    exhelper.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:197:        private ExceptionHelper     updateTasinmazFoto2Db(Tasinmaz tasinmaz)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:200:            ExceptionHelper exhelper = new ExceptionHelper();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:273:        private ExceptionHelper saveImageFiles2SP(Tasinmaz tasinmaz, string fotoFile, FileUpload FotoFileBrowser, ExceptionHelper exhelper)
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:308:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:309:                exHelper.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:339:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:340:                eh.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:547:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:549:                exhelper.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:674:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:676:                exhelper.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:700:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:701:                eh.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:724:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:725:                eh.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:748:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:749:                eh.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:772:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:773:                eh.PublishException();
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:796:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:797:                eh.PublishException();
TBYS_WebParts\TasinmazOnarimGirisiWP\TasinmazOnarimGirisiWP.ascx.cs:270:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazOnarimGirisiWP\TasinmazOnarimGirisiWP.ascx.cs:271:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:363:                ExceptionHelper exh = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:558:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:559:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:572:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:573:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:609:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:610:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:637:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:638:                exHelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:678:                ExceptionHelper exhelper = new ExceptionHelper();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:681:                exhelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:795:                ExceptionHelper eh = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:796:                eh.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:832:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:834:                exhelper.PublishException();
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:874:                ExceptionHelper exhelper = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:876:                exhelper.PublishException();
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:120:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:121:                exHelper.PublishException();
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:143:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:145:                exceptionHelper.PublishException();
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:362:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:363:                exHelper.PublishException();
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:409:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TeminatListesiWP\TeminatListesiWP.ascx.cs:410:                exHelper.PublishException();
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:66:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:68:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:139:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:140:                exHelper.PublishException();
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:186:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:187:                exHelper.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:156:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:157:                exHelper.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:179:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:181:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:476:                ExceptionHelper ex = new ExceptionHelper(exception);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:477:                ex.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:501:                ExceptionHelper exceptionHelper = new ExceptionHelper();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:503:                exceptionHelper.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:615:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:616:                exHelper.PublishException();
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:662:                ExceptionHelper exHelper = new ExceptionHelper(ex);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:663:                exHelper.PublishException();
~~~

## Ham teknik hata metni adayları

~~~text
Model/TBYS\KiraBakiyeDevriOtomatik.cs:55:                    sonuc.HataliSozlesmeler.Add(kiraSozlesme.Id, ex.Message);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:945:                MessageHelper.PublishMessage("Hata" + ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:383:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:148:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:149:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:143:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:43:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:45:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:396:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:475:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:121:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
~~~

## Kullanıcı mesajları ve ASCX metin adayları

~~~text
Model/TBYS\TBYSOrtak.cs:374:                MessageHelper.PublishMessage("Gecikme zammi orani bulunamadi, islem yapabilmek için Gecikme Zammi Oranlarini giriniz.", ProjeConstants.MESAJ_HATA);
Model/TBYS\TBYSOrtak.cs:383:        //        MessageHelper.PublishMessage("OdemePlaniId=" + odemePlani.Id + " olan ve " + odemePlani.Ay + " Ayina ait gecikme zammi hesabinda hata olustu.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:284:                    MessageHelper.PublishMessage("(Envanterde olmayan) Taşınmaz Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\EnvanterdenCikarmaWP\EnvanterdenCikarmaWP.ascx.cs:336:                    MessageHelper.PublishMessage("Envanterden çikarma islemi basarisiz oldu", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:132:                    MessageHelper.PublishMessage("Kiracı bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:159:                MessageHelper.PublishMessage("Kiracı bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:443:                                    MessageHelper.PublishMessage("Devir alma işlemi tamamlandı.", ProjeConstants.MESAJ_BASARILI,2000);
TBYS_WebParts\KiraBakiyeDevriWP\KiraBakiyeDevriWP.ascx.cs:485:                        MessageHelper.PublishMessage("Bu kiracıya ait sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:379:                    MessageHelper.PublishMessage("Dosyalar hazırlandı, Dosya ismine basarak yazıyı indirebilirsiniz", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:382:                    MessageHelper.PublishMessage("Hata Oluştu", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBorcuYazisiWP\KiraBorcuYazisiWP.ascx.cs:600:                    MessageHelper.PublishMessage(message + Environment.NewLine +
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:25:            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:219:                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:246:                        MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:248:                        MessageHelper.PublishMessage("Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciTalepleriWP\BagisciTalepleriWP.ascx.cs:287:                MessageHelper.PublishMessage("Talep bulunamadi",ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:135:                            MessageHelper.PublishMessage("Kayıt Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:146:                        MessageHelper.PublishMessage("Gecikme Zammı Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:172:                            MessageHelper.PublishMessage("Kayıt Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:182:                        MessageHelper.PublishMessage("Faiz Oranları Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\GecikmeZammiWP\GecikmeZammiWP.ascx.cs:246:            MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:26:            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:107:                MessageHelper.PublishMessage("Bağışçının TC Kimlik Numarası girilmemiş.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:351:                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:395:                        MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:397:                        MessageHelper.PublishMessage("Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:435:                MessageHelper.PublishMessage("Taahhüt bulunamadı", ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:478:                        MessageHelper.PublishMessage("Bağışçı zaten listede var.",ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\BagisciTaahhutleriWP\BagisciTaahhutleriWP.ascx.cs:483:                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:183:                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:316:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:348:                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:366:                    MessageHelper.PublishMessage("Kiracıya ait bir Kira Sözleşmesi bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:413:                    MessageHelper.PublishMessage("OdemePlani Bulunamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciAylikOdemeWP\KiraciAylikOdemeWP.ascx.cs:418:                MessageHelper.PublishMessage("Kira Sözlesmesi Bulunamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:158:                    MessageHelper.PublishMessage("Sözlesme bulunamadi!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:166:                    MessageHelper.PublishMessage("Devir alma islemi tamamlandi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:170:                    MessageHelper.PublishMessage("Devir alinacak fark bulunamadi.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:190:                MessageHelper.PublishMessage(
TBYS_WebParts\DevirGerekenSozlesmelerWP\DevirGerekenSozlesmelerWP.ascx.cs:208:                MessageHelper.PublishMessage("Ödeme planlari güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:196:                                    MessageHelper.PublishMessage("Aktarma tamamlandı.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:198:                                    MessageHelper.PublishMessage("Aktarma yapılamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:271:                MessageHelper.PublishMessage("Hiç kayıt seçilmedi. Devam etmek için en az bir kayıt seçiniz.", ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:447:                        MessageHelper.PublishMessage("İşlem Tamamlanamadı. EkstreAktarmaId=" + ekstreAktarmaListItem.KiraEkstreAktarmaId, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraBankaEkstreListesiWP\KiraBankaEkstreListesiWP.ascx.cs:677:                MessageHelper.PublishMessage("Kaydetmek için kayıt seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:29:            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:142:                    MessageHelper.PublishMessage("Taşınmaz Eklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:153:                    MessageHelper.PublishMessage("Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:382:                            MessageHelper.PublishMessage("Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:387:                        MessageHelper.PublishMessage("Bağışlardan Çıkarıldı", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagisciBagislariWP\BagisciBagislariWP.ascx.cs:519:                    MessageHelper.PublishMessage("Bağış Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:365:                    MessageHelper.PublishMessage("Dosyalar hazirlandi, Dosya ismine basarak yaziyi indirebilirsiniz", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:368:                    MessageHelper.PublishMessage("Hata Olustu", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraArtisYazisiWP\KiraArtisYazisiWP.ascx.cs:660:                    MessageHelper.PublishMessage(message + Environment.NewLine +
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:25:            MessageHelper.PublishMessage("İşlem tamamlanamadı. Lütfen tekrar deneyin. Başvuru kodu: " + context.OperationId.ToString("N"), ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:446:                    MessageHelper.PublishMessage("Bağımsız Bölüm güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:456:                    MessageHelper.PublishMessage("Bağımsız Bölüm güncellenemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:461:                MessageHelper.PublishMessage("Bağımsız Bölüm bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:480:                    MessageHelper.PublishMessage("Bağımsız Bölüm silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\BagimsizBolumWP\BagimsizBolumWP.ascx.cs:492:                MessageHelper.PublishMessage("Bağımsız Bölüm silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:153:                        MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:158:                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:194:                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\KiraKartiWP\KiraKartiWP.ascx.cs:219:                            MessageHelper.PublishMessage("Bağımsız bölüm bulunamadı. Sözleşmeden bağımsız bölüm kaydını düzeltmeniz gerekmektedir.",ProjeConstants.MESAJ_HATA);
TBYS_WebParts\GenelSozlesmeListesiWP\GenelSozlesmeListesiWP.ascx.cs:250:                MessageHelper.PublishMessage("Borçlu Kiraci bulunamadi", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:64:    //alert( 'Table redrawn' );
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:247:                MessageHelper.PublishMessage("Siralama kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraSozlesmeSiralamaWP\KiraSozlesmeSiralamaWP.ascx.cs:251:                MessageHelper.PublishMessage("Siralama kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BorcluKiracilarWP\BorcluKiracilarWP.ascx.cs:405:                MessageHelper.PublishMessage("Borçlu Kiraci bulunamadi", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:423:                MessageHelper.PublishMessage("Borçlu Kiraci bulunamadi", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:810:                MessageHelper.PublishMessage("Kiraci bulunamadi",ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:889:                    MessageHelper.PublishMessage("Kiraci bulunamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:930:                MessageHelper.PublishMessage("Takip islemi kaydedildi",ProjeConstants.MESAJ_BASARILI,2000);
TBYS_WebParts\BorcluKiraciIslemleriWP\BorcluKiraciIslemleriWP.ascx.cs:963:                    MessageHelper.PublishMessage("Takip islemi güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\MevcutKiracilarWP\MevcutKiracilarWP.ascx.cs:267:                MessageHelper.PublishMessage("Borçlu Kiraci bulunamadi", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:227:                MessageHelper.PublishMessage("Taşınmaz Bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:488:                MessageHelper.PublishMessage("Taşınmaz Kaydı başarısız oldu.-TS001", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:495:                MessageHelper.PublishMessage("Taşınmaz kaydı güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:499:                MessageHelper.PublishMessage("Taşınmaz Güncellenemedi.-TS001", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:531:            //    MessageHelper.PublishMessage("Bu tasinmazin Sigortasi Bulunamadı. Lütfen sigorta Durumunu seçip kaydettikten sonra tekrar deneyiniz.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:575:                    MessageHelper.PublishMessage("Taşınmaz Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:578:                MessageHelper.PublishMessage("Taşınmaz Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:604:                MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmaz henüz bir bağışçıyla ilişkilendirilmemiş. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx.cs:612:                    MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmazın ilişkilendirildiği bağışçı bulunamadı. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\MulkiyetiOlmayanTasinmazGirisiWP\MulkiyetiOlmayanTasinmazGirisiWP.ascx:125:                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" OnClick="DeleteBtn_Click" />
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx:59:                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemeTutariTxt" ForeColor="Red" ErrorMessage="Ödeme Tutarı giriniz"> </asp:RequiredFieldValidator>
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx:64:                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemeTarihiTxt" ForeColor="Red" ErrorMessage="Ödeme tarihi seçiniz"> </asp:RequiredFieldValidator>
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx:73:                            <asp:RequiredFieldValidator runat="server" ControlToValidate="SozlesmeDDL" ForeColor="Red" ErrorMessage="Sözleşme seçiniz"> </asp:RequiredFieldValidator>
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx:78:                            <asp:RequiredFieldValidator runat="server" ControlToValidate="OdemePlaniDDL" ForeColor="Red" ErrorMessage="Ödeme ayı seçiniz"> </asp:RequiredFieldValidator>
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:309:                    MessageHelper.PublishMessage("Ödeme yapmak için bir kiracı seçmelisiniz.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:354:                        MessageHelper.PublishMessage("Ödeme yapmak için bir kiracı seçmelisiniz.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:465:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:490:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:515:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:582:                        MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:590:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:687:                MessageHelper.PublishMessage("Kiraci Bulunamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:705:                    MessageHelper.PublishMessage("Kiracıya ait bir Kira Sözleşmesi bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:720:                    MessageHelper.PublishMessage("Ödeme Planı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeGirisWP\OdemeGirisWP.ascx.cs:725:                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:119:                MessageHelper.PublishMessage("Faiz oranları oluşturulamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:454:        //                            MessageHelper.PublishMessage("Kayit Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:465:        //                    MessageHelper.PublishMessage("Faiz Oranlari Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:540:                MessageHelper.PublishMessage("Faiz Oranları Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:687:                    MessageHelper.PublishMessage("Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\YasalFaizWP\YasalFaizWP.ascx.cs:694:                MessageHelper.PublishMessage("Faiz Oranlari Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:81:                        MessageHelper.PublishMessage("Kiraci bulunamadi!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:144:                MessageHelper.PublishMessage("Kiraci ekrana getirilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:446:                    MessageHelper.PublishMessage("Kiraci Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:464:                MessageHelper.PublishMessage("Kiraci Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:468:                MessageHelper.PublishMessage("Kiraci Güncellenemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:581:                    MessageHelper.PublishMessage("Zaten Bir kira sözlesmesi mevcut", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:609:                MessageHelper.PublishMessage("Sözlesme Kaydi Açilamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:692:                MessageHelper.PublishMessage("Sözlesme Kaydi Açilamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:727:                        MessageHelper.PublishMessage("Bu kiraciya ait sözlesme kaydi bulunamadi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:872:                    MessageHelper.PublishMessage("Kiraci bulunamadi.",ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:879:                        MessageHelper.PublishMessage("Kiraciya ait sözlesme bulundugundan kiraci silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraciGirisiWP\KiraciGirisiWP.ascx.cs:885:                            MessageHelper.PublishMessage("Kiraci daha önce ödeme yaptigindan kiraci silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:134:                        MessageHelper.PublishMessage("Ödeme Bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:374:                    MessageHelper.PublishMessage("Ödeme Bilgileri Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:378:                    MessageHelper.PublishMessage("Ödeme Bilgileri Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:383:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:546:                    MessageHelper.PublishMessage("Ödenen tutarı aştınız.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:589:                MessageHelper.PublishMessage("Kayıt bulunamadı", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:619:                MessageHelper.PublishMessage("Ödenen tutarı aştınız.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\OdemeAyristirmaWP\OdemeAyristirmaWP.ascx.cs:660:                MessageHelper.PublishMessage("Toplam ödenen tutarı aştınız, girdiğiniz tutar listeye eklenmedi.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\TasinmazSigortaListesiWP\TasinmazSigortaListesiWP.ascx.cs:396:                    MessageHelper.PublishMessage("Sigorta Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:162:                    MessageHelper.PublishMessage("Sözleşme Bulunamadı",ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:463:                        MessageHelper.PublishMessage("Kira Sözleşmesi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:469:                        MessageHelper.PublishMessage("Kira Sözleşmesi Kayıt işlemi başarısız oldu.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:475:                    MessageHelper.PublishMessage(" Geçerli bir Kira Bedeli girmediniz!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:482:                MessageHelper.PublishMessage("Sözleşme Başlangıç ve Bitiş Tarihleri ile Kira Bedeli alanları dolu olmalıdır", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:781:                MessageHelper.PublishMessage("Ödeme planı Açılamıyor. Lütfen sözleşmenin başlama ve bitiş tarihlerini girerek tekrar deneyin.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:945:                MessageHelper.PublishMessage("Hata" + ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1726:                        MessageHelper.PublishMessage("Silinecek dosya bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1731:                        MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1741:                        MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx.cs:1819:                    MessageHelper.PublishMessage("Sözleşme bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\KiraSozlesmesiWP\KiraSozlesmesiWP.ascx:153:                                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:110:            //    MessageHelper.PublishMessage("Resimler Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:117:                    MessageHelper.PublishMessage("Resimler kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:119:                    MessageHelper.PublishMessage("Resimler kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:441:                    MessageHelper.PublishMessage("Resim Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:446:                    MessageHelper.PublishMessage("Resim Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:465:                        MessageHelper.PublishMessage("Emlak Beyan Formu Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:473:                    MessageHelper.PublishMessage("Lütfen Emlak Beyan Formu yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:482:                        MessageHelper.PublishMessage("Yapı Kayıt Belgesi Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:490:                    MessageHelper.PublishMessage("Lütfen Yapı Kayıt Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:499:                        MessageHelper.PublishMessage("Tapu Kayıt Belgesi Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:507:                    MessageHelper.PublishMessage("Lütfen Tapu Kayıt Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:516:                        MessageHelper.PublishMessage("İmar Durumu Belgesi Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:524:                    MessageHelper.PublishMessage("Lütfen İmar Durumu Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:533:                        MessageHelper.PublishMessage("Taşınmaz Raporu Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:541:                    MessageHelper.PublishMessage("Lütfen İmar Durumu Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:577:                        MessageHelper.PublishMessage("Lütfen Emlak Beyan Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:599:                        MessageHelper.PublishMessage("Lütfen Yapı Kayıt Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:620:                        MessageHelper.PublishMessage("Lütfen Tapu Kayıt Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:641:                        MessageHelper.PublishMessage("Lütfen Imar Durumu Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:662:                        MessageHelper.PublishMessage("Lütfen Taşınmaz Raporu Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:667:                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:687:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:695:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:711:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:719:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:735:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:743:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:759:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:767:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:783:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx.cs:791:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciListesiWP\VasiyetciListesiWP.ascx.cs:121:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx:154:                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx:168:                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx:182:                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx:196:                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazResimleriWP\TasinmazResimleriWP.ascx:212:                                    OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:283:                    MessageHelper.PublishMessage("Sigorta Bitis Tarihi Bos!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:287:                    MessageHelper.PublishMessage("Sigorta Vadesi " + sigorta.SigortaBitTar.ToString("dd.MM.yyyy") + " Tarihinde dolacaktır", ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:416:                        MessageHelper.PublishMessage("Taşınmazın ALTBÖLÜMÜ VAR seçilmesine rağmen taşınmaza ait bölüm tanımlanmamıştır.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:596:                    MessageHelper.PublishMessage("Sigorta Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:602:                    MessageHelper.PublishMessage("Bağışçı Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:624:                        MessageHelper.PublishMessage("Sigorta Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:628:                        MessageHelper.PublishMessage("Sigorta Güncellenemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:672:                    MessageHelper.PublishMessage("Sigorta Silinemedi", ProjeConstants.MESAJ_BILGI);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:770:                            MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:780:                            MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:785:                        MessageHelper.PublishMessage(dosyaAdi + " Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx.cs:867:                    MessageHelper.PublishMessage("Poliçe bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciListesiWP\TasinmazBagisciListesiWP.ascx.cs:149:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:165:                        MessageHelper.PublishMessage("Taşınmaz Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:221:                        MessageHelper.PublishMessage("Lütfen Emlak Beyan Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:238:                        MessageHelper.PublishMessage("Lütfen Yapı Kayıt Belgesini pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:243:                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazKartiWP\TasinmazKartiWP.ascx.cs:785:                MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\TasinmazSigortaGirisWP\TasinmazSigortaGirisWP.ascx:165:                        OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:241:                    MessageHelper.PublishMessage("Vasiyetçi Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:371:                    MessageHelper.PublishMessage("Vasiyetçi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:375:                    MessageHelper.PublishMessage("Vasiyetçi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:384:                        MessageHelper.PublishMessage("Vasiyetçi Kaydedildi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:388:                        MessageHelper.PublishMessage("Vasiyetçi Kaydedilemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:396:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:452:                    MessageHelper.PublishMessage("Vasiyetçi Adı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:456:                    MessageHelper.PublishMessage("Vasiyetçi Soyadı Boş Olamaz.", ProjeConstants.MESAJ_HATA, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:463:                        MessageHelper.PublishMessage("Vasiyetçi Güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:467:                        MessageHelper.PublishMessage("Vasiyetçi Güncellenmedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:475:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:520:                        MessageHelper.PublishMessage("PDF Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:526:                    MessageHelper.PublishMessage("Lütfen bir pdf belgesi seçiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:660:                        MessageHelper.PublishMessage("Vasiyete konu varlık listesi dolu olduğundan Vasiyetçi silinemiyor. Lütfen önce Vasiyete Konu Varlık Listesindeki varlıkları siliniz", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:687:                    MessageHelper.PublishMessage("Vasiyetçi Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:710:                    MessageHelper.PublishMessage("Kayıt Silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:714:                    MessageHelper.PublishMessage("Kayıt Silinemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciGirisiWP\VasiyetciGirisiWP.ascx.cs:719:                MessageHelper.PublishMessage("Vasiyet Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazOnarimListesiWP\TasinmazOnarimListesiWP.ascx.cs:43:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TaahhutListesiWP\TaahhutListesiWP.ascx.cs:148:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx:170:                                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx:187:                                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" Visible="False" />
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx:202:                            OnClientClick="if(confirm(' Silme İşlemini Onaylıyor musunuz?')){return true;} else{return false;};" />
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:156:                    MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:371:                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:388:                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:393:                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:410:                    MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:415:                MessageHelper.PublishMessage("Kiracı Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:451:                MessageHelper.PublishMessage("Bu sözleşme için ödeme Plani oluşturulamaz. Ödeme Plani oluşturabilmek için, Lütfen Kira Sözleşmesinde Sözleşme Başlangıç ve Bitiş tarihini giriniz", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:477:                MessageHelper.PublishMessage("Ödeme planı oluşturulamıyor. Lütfen sözleşmenin başlama ve bitiş tarihlerini girerek tekrar deneyin.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:523:                    MessageHelper.PublishMessage("Ödeme planı oluşturulamaz. Zaten geçerli bir ödeme planı mevcut.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\OdemePlaniWP\OdemePlaniWP.ascx.cs:529:                MessageHelper.PublishMessage("Ödeme planı oluşturuldu. Ödeme planı geçerlilik tarihi " + kiraSozlesme.SozBitTar + " -" + kiraSozlesme.SozBitTar, ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:430:                MessageHelper.PublishMessage("İşlem Tamamlandı. Resim yüklendi.", ProjeConstants.MESAJ_BASARILI);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:461:                    MessageHelper.PublishMessage("Bağışçiya ait talepler, kayıtlı bağışçı yakınları, bağışçı taahhütleri veya verilmiş armağan olduğundan dolayı bu bağışçı silinemez",ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:473:                MessageHelper.PublishMessage("Hata Bağışçı bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:544:                    MessageHelper.PublishMessage("Bağışçı Kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:550:                    MessageHelper.PublishMessage("Bağışçı Kaydedilemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:575:                    MessageHelper.PublishMessage("Bağışçı Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:579:                    MessageHelper.PublishMessage("Bağışçı Güncellenemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:615:                        MessageHelper.PublishMessage("Bilgi Formu Yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:622:                    MessageHelper.PublishMessage("Lütfen Bağışçı Bilgi ve Talep Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:631:                        MessageHelper.PublishMessage("Taahhüt Belgesi yüklendi", ProjeConstants.MESAJ_BASARILI, 2000); 
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:637:                    MessageHelper.PublishMessage("Lütfen Taahhüt Belgesi yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:673:                        MessageHelper.PublishMessage("Lütfen Bağışçı Bilgi ve Talep Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:693:                        MessageHelper.PublishMessage("Lütfen Bağışçı Taahhüt Formunu pdf olarak yükleyiniz.", ProjeConstants.MESAJ_BILGI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:698:                    MessageHelper.PublishMessage("Bağışçı bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:718:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:724:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:742:                    MessageHelper.PublishMessage(dosyaAdi + " Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazBagisciGirisiWP\TasinmazBagisciGirisiWP.ascx.cs:749:                    MessageHelper.PublishMessage(dosyaAdi + " Silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazListesiWP\TasinmazListesiWP.ascx.cs:143:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx:363:                                                            <asp:GridView ID="BagimsizBolumGridView" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="False" EmptyDataText="Alt bölüm kaydı bulunamadı">
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:189:                MessageHelper.PublishMessage("Taşınmaz Bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:762:                MessageHelper.PublishMessage("Taşınmaz Kaydı başarısız oldu.-TS001", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:769:                MessageHelper.PublishMessage("Taşınmaz kaydı güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:773:                MessageHelper.PublishMessage("Taşınmaz Güncellenemedi.-TS001", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:796:                MessageHelper.PublishMessage("Taşınmaz bilgileri alınamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:809:                MessageHelper.PublishMessage("Bu taşınmaza ait bağımsız bölüm bulunamadı.", ProjeConstants.MESAJ_BILGI,2000);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:849:                MessageHelper.PublishMessage("Taşınmaz bilgileri alınamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:889:                MessageHelper.PublishMessage("Taşınmaz bilgileri alınamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:981://                MessageHelper.PublishMessage("Taşınmaz silme yetkiniz bulunmamaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1004://                            MessageHelper.PublishMessage("Taşınmaz silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1008://                        MessageHelper.PublishMessage("Taşınmaza ait sözleşme bulunmamaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1012://                    MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1027:                MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1035:                    MessageHelper.PublishMessage("Taşınmaza ait bağımsız bölümler bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1043:                    MessageHelper.PublishMessage("Taşınmaza ait bağışçı bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1051:                    MessageHelper.PublishMessage("Taşınmaza ait sözleşmeler bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1059:                    MessageHelper.PublishMessage("Taşınmaza ait sözleşme bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1068:                    MessageHelper.PublishMessage("Taşınmaza ait onarım bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1076:                    MessageHelper.PublishMessage("Taşınmaza ait sigorta bulunmaktadır. Taşınmaz silinemez.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1095:                    MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1107:                    MessageHelper.PublishMessage("Taşınmaz silinemedi", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1142:                MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmaz henüz bir bağışçıyla ilişkilendirilmemiş. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1150:                    MessageHelper.PublishMessage("Bağışçı Bulunamadı. Bu taşınmazın ilişkilendirildiği bağışçı bulunamadı. Lütfen Bağışçı sayfasından bağışçı ataması yapınız.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1313:                MessageHelper.PublishMessage("Taşınmaz Kaydı başarısız oldu.-TS001", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1476:                MessageHelper.PublishMessage("Taşınmaz bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1560:                    MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1631:                MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı eklendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1635:                MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı eklenemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1659:                    MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı silindi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1663:                    MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı silinemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1696:                    MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı güncellendi.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TasinmazGirisiWP\TasinmazGirisiWP.ascx.cs:1700:                    MessageHelper.PublishMessage("Şerh Beyan ve İrtifak kaydı güncellenemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:312:                    MessageHelper.PublishMessage(message + Environment.NewLine +
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:396:                    MessageHelper.PublishMessage("Vasiyetçi adres etiketleri hazirlandi, Dosya ismine basarak yaziyi indirebilirsiniz.", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\VasiyetciAdresListesiWP\VasiyetciAdresListesiWP.ascx.cs:399:                    MessageHelper.PublishMessage("Adres etiketleri olusturulamadi.", ProjeConstants.MESAJ_BILGI, 3000);
TBYS_WebParts\TasinmazSorgulamaWP\TasinmazSorgulamaWP.ascx.cs:45:                MessageHelper.PublishMessage(ex.Message, ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:156:                        MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:399:                        MessageHelper.PublishMessage("Teminat bilgileri güncellendi.", ProjeConstants.MESAJ_BASARILI);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:403:                        MessageHelper.PublishMessage("Teminat bilgileri güncellenemedi.", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:415:                MessageHelper.PublishMessage("Kira Sözleşmesi Bulunamadı ", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:438:                MessageHelper.PublishMessage("Sözleşme Bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:536:                    MessageHelper.PublishMessage("Mahsup edilecek bir sözleşme bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:569:                            MessageHelper.PublishMessage("Teminat İşlemi Güncellendi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:579:                        MessageHelper.PublishMessage("Teminata ait sözleşme bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:585:                MessageHelper.PublishMessage("Teminat İşlemi bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:615:                    MessageHelper.PublishMessage("Teminat İşlemi Silindi", ProjeConstants.MESAJ_BASARILI, 2000);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:619:                    MessageHelper.PublishMessage("Teminat İşlemi Silindi. Teminata ait sözleşme bulunamadı!", ProjeConstants.MESAJ_BILGI, 4000);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:624:                MessageHelper.PublishMessage("Teminat İşlemi bulunamadı!", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:686:                MessageHelper.PublishMessage("Sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
TBYS_WebParts\TeminatIslemleriWP\TeminatIslemleriWP.ascx.cs:704:                    MessageHelper.PublishMessage("Bu kiracıya ait sözleşme kaydı bulunamadı", ProjeConstants.MESAJ_HATA);
~~~

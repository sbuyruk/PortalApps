# NBYS eski loglamayı kapatma — parça 1/2

Tarih: 23.10.2026  
Dal: `codex/nbys-eski-log-parca1`  
Başlangıç master commit'i: `e002463b0a3d7e56209953b4a2e6961a3d2a6690` (PR #547 sonrası).

Değişiklikten önce yerel master temizdi ve canlı uzak master ile aynı commit'te doğrulandı. Kullanıcı NBYS çalışmasını iki dalda istedi. Bu dal ilk parçadır; NBYS genel kapanışı değildir. Commit, push, PR, merge, yayın ve çalışma zamanı kabulü kullanıcıya aittir.

## İki dalın sınırı

| Dal | Kapsam |
| --- | --- |
| 1 — bu dal | Bağışçı, nakit bağış, armağan ve düzenli bağışçı eski yazma API'leri; bunlara bağımlı ekstre aktarımının bağış/armağan oluşturma zinciri; bilinmeyen bağışçı oluşturma audit'i. |
| 2 — sonraki dal | Banka dosyası yükleme/eşleştirme, kalan ekstre, FTK, tanımlar/SMS/belge yardımcıları; kalan bağlamsız okumalar ve gizli hata yolları; bayrak/referans ve ekran/proje bağımlılıklarının genel temizliği. |

Ekstre aktarımının bağış/armağan üretimi ilk dala bağımlılık nedeniyle alındı: kaldırılan eski servis yazmalarının kalan tüketicisi eski transfer yardımcılarıydı. Banka dosyasının ayrıştırılması ve kalan ekstre iş akışları bu dalda yeniden yazılmadı.

İkinci dal için ilk parça kullanıcı tarafından birleştirilip master'a alındıktan ve test değerlendirmesi veya testten önce açık devam isteği geldikten sonra temiz master/uzak eşitliği yeniden doğrulanmalıdır.

## Bulgular, karşılıklar ve değişiklikler

### 1. Eski servis yazmaları

`Model/Services/NBYS/NakitBagisciService.cs`, `NakitBagisHareketService.cs`, `ArmaganService.cs` ve `DuzenliNakitBagisciService.cs` içinden 21 eski metot kaldırıldı. Bunlar transaction'sız CRUD, eski iade/arşiv/birleştirme, eski armağan üretimi ve ilgili özel yardımcılardır. Kaldırılan imzaların kaynak çağrıları tarandı; gerekli yedi transaction'lı yazma girişi korunmuştur.

| Eski yol | Etkin karşılık / karar |
| --- | --- |
| NakitBagisciService bağlamsız Save/Update ve tüzel kişi değişikliğinde eski armağan zinciri | Mevcut NbysBagisciWriteService.Save; ekstre aktarımı için korunmuş transaction+bağlam bağlı servis yolu |
| NakitBagisHareketService.UpdateRefund / DeleteWithArchive / MergeDonor | Mevcut NbysDonationWriteService.Refund / Delete / Merge |
| ArmaganService.SaveOrUpdate ve eski durum/yazma yardımcıları | Mevcut NbysGiftWriteService.Update / MarkCertificateReturned; transfer oluşturması için aşağıdaki bağlamlı transaction zinciri |
| DuzenliNakitBagisciService eski CRUD ve ArmaganService.SaveDuzenliBagisIfMissing | Mevcut NbysRegularWriteService.Import / Match / CreateCertificate |
| Bağımsız bağışçı/armağan Delete ve eski bulk API'ler | Kaynakta etkin kullanıcıları kalmadığı doğrulanarak kaldırıldı; yeni, ilave bir UI silme yeteneği oluşturulmadı |

Dört serviste OlayKayit/PublishException başvurusu kalmadı. Ekstre aktarımının kullandığı NakitBagisci Save/Update/CreateBilinmeyenBagisci, NakitBagisHareket Save/Update ve Armagan Save/Update transaction overload'ları artık repository.HasLogContext gerektirir. Bu özellik logun açık olup olmadığını değil, açık NBYS bağlamının sağlanıp sağlanmadığını ölçer. Dolayısıyla yeni log kapatıldığında iş işlemi çalışmaya devam eder; bağlamsız eski yazıcıya dönmez.

Audit eski/yeni snapshot'larını zaten aynı transaction içinde alan NbysTransactionalWriter kullanılır. Yalnız eski OlayKayit için yapılan gereksiz previous okumaları kaldırıldı. Modern iade/arşiv/birleştirme servisleri ve iş arşivleri korunmuştur.

### 2. Repository yazmaları

`DAO/Repositories/NBYS/NakitBagisciRepository.cs`, `NakitBagisHareketRepository.cs`, `ArmaganRepository.cs` ve `DuzenliNakitBagisciRepository.cs` içinden 18 eski yazma metodu kaldırıldı. Eski LegacyWriteDb/RequireLegacyWrite yardımcıları ve bilinmeyen bağışçının elle bağlantı/transaction açıp DbClass.SorguyuLogla çağıran yolu kaldırıldı.

Korunan yedi transaction yazması açık bağlam ister ve doğrudan merkezi transaction writer'a gider. Aynı SqlTransactionContext iş SQL'i ve audit için korunur. Bağlam veya transaction eksikliği eski sisteme dönüş anlamına gelmez; işlem reddedilir.

ArmaganRepository.SelectByDurumTarih nullable transaction uyumluluğu korunur: yalnız transaction==null dalı bağlamsız/bağlamlı normal okumayı seçer. Transaction verilmiş dal doğrudan aynı transaction ile SELECT yapar. Bu eski okuma uyumluluğu ikinci dalın genel incelemesine dahildir.

### 3. Ekstre aktarımı

`Model/Services/NBYS/EkstreAktarmaTransferService.cs`:

- SaveAll girişlerinde NBYS bağlamı zorunludur; bağlam null varsayılanı kaldırıldı.
- Kullanılmayan SaveArmagan, ArmaganiKaydetVeyaGuncelle ve bool döndüren SaveArmaganYeni eski girişleri kaldırıldı.
- İç bağışçı/bağış/armağan/banka/tanım/il/ilçe servis fabrikaları yalnız açık NBYS bağlamıyla oluşturulur. Eski varsayılan veya yalnız transaction taşıyan bağlamsız repository fabrikasına dönüş kaldırıldı.
- İç yazma/okuma yardımcıları transaction ister; transaction==null durumunda eski overload'a dönen dallar kaldırıldı.
- Düzenli belge üretiminin eski, bu zincirde zaten desteklenmeyen kolu kaldırıldı. Aktif düzenli belge işlevi NbysRegularWriteService.CreateCertificate içinde korunur.
- Mevcut satır başına transaction, sıra, mükerrer aktarım kontrolleri ve sonuç sayaçları korunur. Bir ekstre satırında hata, o satırın bağışçı/bağış/armağan/ekstre ve audit yazılarını geri alır; önce tamamlanan başka satırlar kalır.
- ExceptionHelper sonuç taşıyıcısı korunur; PublishException çağrılmaz. Teknik hata mevcut NbysEkstreFeedback/NbysWebPartLog sınırında ele alınır.

Mevcut transaction'lı 27 servis çağrısının yöntem/argüman sözleşmesi başlangıçla karşılaştırıldı; değişen sözleşme yok. Bağış tutarı, tekli/çoklu armağan hakkı ve durum kuralları yeniden tasarlanmadı.

### 4. Bilinmeyen bağışçı audit'i

`NakitBagisciRepository.CreateBilinmeyenBagisci(..., transaction)` kimlik alıp adı kimlikle tamamlarken iki gerçek SQL komutu çalıştırır. Önceden bunlar iki ayrı INSERT/UPDATE audit başlığı üretiyordu.

Şimdi iki komut da aynı transaction'da NbysSqlExecutor üzerinden izlenir. Nihai satır okunduktan sonra `NbysTransactionalWriter.RecordInserted` ile tek INSERT audit'i üretilir. İsim tamamlama UPDATE'i için ikinci iş audit başlığı yazılmaz. Her iki teknik SQL izi korunur. Herhangi bir SQL veya audit hatası bütün oluşturmayı geri alır.

NbysTransactionalWriter'ın normal Insert metodu da aynı RecordInserted yardımcısını kullanır; diğer kayıt türlerinin normal INSERT audit davranışı değişmez. Alan izin listeleri ve kişisel veri kapsamı genişletilmedi.

## İlk parçanın kapanış sınırı

Bu dal NBYS'nin tamamında eski log tablolarına sıfır yazma kabulü değildir. Diğer NBYS servis/repository'lerindeki eski yazmalar, global NBYS log bayrakları, bağlamsız okuma uyumlulukları, NBYSOrtak gibi eski yardımcılar ve ortak eski ekranlar ikinci parçada değerlendirilecektir.

Bu dört repository'nin bağlamsız okuma kurucuları/DbClass okuma yolları henüz kaldırılmadı. Bunlar ikinci parçada tüketicileriyle birlikte ele alınmalıdır; yalnız transaction'lı yazma yolunun kapanmış olması bütün eski okuma/hata yollarının kapandığı şeklinde yorumlanmamalıdır.

Yeni etkinlik/audit/SQL/hata altyapısı yeniden kurulmadı. Merkezi teknik hata/SQL loglarının bağımsız bağlantı tasarımı ve SqlTransactionContext değiştirilmedi. Başka projeye geçilmedi.

## Statik doğrulama

- 10 değiştirilmiş C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi yönünden incelendi: sıfır hata. Semantik derleme/emission/test/debug yapılmadı.
- Solution genelinde saptanan 314 NBYS servis çağrısı ve EkstreAktarmaTransferService statik çağrılarının argüman sayıları kontrol edildi: çözülemeyen çağrı yok. Servis/repository imza ve overload taraması temiz.
- Yerel metot çağrısı taraması temiz; bu kontrol semantik tip kontrolünün yerine geçmez.
- Değişen dosyalardaki 62 transaction parametreli metot ve 47 DB çağrısı aktarımı tarandı. Bir çağrı yalnız açık transaction==null uyumluluk dalındadır; transaction verilen dallarda eksik transaction argümanı bulunmadı.
- Başlangıç sürümünde de bulunan 74 repository SQL/parametre sözleşmesi ve transferin 27 transaction servis çağrısı karşılaştırıldı: fark yok.
- UTF-8/CRLF, başlangıç BOM durumları ve proje Compile kayıtları doğrulandı. git diff --check temiz.
- WebPart, ASCX/g.cs, csproj, feature/paket dosyaları değiştirilmedi. Yeni kontrol alanı/handler değişmedi; ASCX üretimi gerekmez.
- Kaynak incelemesi solution dışı önceden derlenmiş istemcilerin bulunmadığını kanıtlamaz. Kullanıcı bütün solution derlemesini ve varsa dış istemcileri doğrulamalıdır.

## Kullanıcının uygulayacağı kabul testleri

1. Visual Studio 2026'da solution'ı derleyin/paketleyin; Test sunucusunda yayımlayın. Kaldırılan API'ler ve çağrı zincirlerini semantik olarak doğrulayın.
2. NBYS merkezi loglaması açık/kapalıyken aynı akışları tekrarlayın. Tam kapatma için ilgili proje EnabledProjects içinde ve ilgili WebPart EnabledWebParts içinde etkin olmamalıdır; iki ayar OR ile değerlendirilir.
3. Bağışçı kaydet/güncelle, iade/silme/birleştirme, armağan düzenleme/iade işaretleme ve düzenli bağışçı import/eşleştirme/belge üretiminde mevcut iş sonuçları ve yetki kontrolleri korunmalı.
4. Ekstre aktarımında mevcut bağışçı, yeni bağışçı ve bilinmeyen bağışçı; farklı TC/telefon/ad eşleşmeleri ve eksik il/ilçe sonuçlarını deneyin. Yazılar ve okumalar aynı satır transaction'ında kalmalı.
5. Bilinmeyen bağışçı oluşturulunca kimlikli nihai isim/açıklama doğru olmalı. Audit açıkken bağışçı için bir INSERT başlığı bulunmalı; isim tamamlama için ikinci UPDATE başlığı oluşmamalı. INSERT ve UPDATE SQL izleri ayrı gerçek komutlar olarak görünmeli.
6. Bilinmeyen bağışçı INSERT'i sonrasında, isim UPDATE'inde ve audit yazımında ayrı ayrı kontrollü hata: bütün oluşturma/bağlantılı satır işlemi geri alınmalı, yarım isimli bağışçı kalmamalı.
7. Tekli/çoklu armağan eşikleri, daha önce verilen armağan, teşekkür, e-Devlet, belge istemeyen/ulaşılamayan ve iade geçmişi bulunan bağışçılarda önceki iş kurallarını karşılaştırın. Bağışın ArmaganId bağlantısı aynı transaction'da tamamlanmalı.
8. Toplu ekstre aktarımında bir satırı başarısız kılın: o satırın bütün yazıları/audit'i geri alınmalı; önceki başarılar korunmalı. Başarı/atlandı/zaten aktarılmış/hata sayaçları doğru olmalı.
9. Aynı ekstreyi tekrar ve eşzamanlı aktarın: ikinci bağış/armağan oluşmamalı. Başarısız satırın yeniden denenmesinde yarım kayıt bulunmamalı.
10. Log kapalıyken aynı iş akışları tamamlanmalı; bu parçada kapatılan yazma zinciri eski Olay_Table/SQLLog_Table yazıcısına dönmemeli. Diğer, henüz kapanmamış NBYS yollarının eşzamanlı trafiğini sayımlardan ayırın.
11. Eksik bağlam/transaction ile kaldırılan eski yola dönme girişimi reddedilmeli; iş verisi/audit kısmen yazılmamalı. Teknik hata kullanıcıya ham istisna olarak çıkmamalı.
12. Ortak RecordInserted refaktörü için FTK, SMS ve normal ekstre INSERT'lerinde de önceki tek audit başlığının ve mevcut alan kapsamının korunduğunu kontrol edin. Bu alanların eski API temizliği ikinci dalda yapılacaktır.

Veritabanına bağlanılmadı; eski tablolar, geçmiş loglar, iş arşivleri veya şema değiştirilmedi/silinmedi. Tüm projeler ve dış bağımlılıklar bitmeden eski tablolara global yazma engeli uygulanmamalıdır. Bağımlılık envanteri, saklama politikasına uygun salt okunur arşiv ve geri yükleme kontrolü sonrasında olası tablo kaldırma ayrı DBA sürecidir.

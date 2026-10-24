# NBYS eski loglamayı kapatma — parça 2/2

Tarih: 24.10.2026  
Dal: codex/nbys-eski-log-kapanis  
Başlangıç master: 028e6f11e23a0c77f2b7060b11766e50d6b1dc4d (PR #548 sonrası).

Yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı. Bu dal NBYS kaynak temizliğinin ikinci ve son parçasıdır. Çalışma zamanı kabulü henüz yapılmadı. Commit, push, PR, merge, pull, derleme, yayın ve kabul testleri kullanıcıya aittir.

## Kapatılan yollar ve etkin karşılıkları

Aşağıdaki dosyalar repository köküne göredir. Önceki geçiş notlarındaki “parametresiz/enjekte çağrılar eski davranışını korur” ve “bağlamsız eski tüketici eski olayı yazar” ifadeleri bu kapanıştan sonra geçerli değildir.

| Eski dosya/metot | Karar / korunmuş etkin karşılık |
| --- | --- |
| Model/Services/NBYS/EkstreAktarmaService.cs: transaction'sız Save/Update/Delete | Kullanılmayan girişler kaldırıldı. Ekranlar NbysEkstreWriteService.Save/Match/Delete kullanır. Import/transfer için Save(item, transaction) ve Update(item, transaction) korunur. |
| DAO/Repositories/NBYS/EkstreAktarmaRepository.cs: bağlamsız transaction.Insert/Update dalları | Kaldırıldı. Insert/Update/Delete açık NBYS bağlamı ve NbysTransactionalWriter ile aynı transaction'da çalışır. |
| Model/Services/NBYS/EkstreAktarmaImportService.cs: SaveEkstreAktarma, BuKayitDahaOnceGirilmisMiByFisNo, GetIlNameById | Bağlam null olduğunda yalnız transaction taşıyan eski repository kurucularına dönen dört seçim kaldırıldı. İl sorgusu, bağışçı eşleştirme, mükerrer sorgusu ve INSERT aynı transaction'ı taşır. |
| Model/Services/NBYS/FTKService.cs, FTKKisiService.cs, FTKIslemService.cs: eski CRUD; DeleteByOperationAndCounter / DeactivateByIdList | Kaynak tüketicisi kalmayan girişler kaldırıldı. Güncel karşılık NbysFtkWriteService.SavePerson/DeletePerson/Deactivate/SaveCommittee ve NbysFtkWriteRepository'dir. Kurul düzeltme/sayaç/geçmiş işlemleri modern serviste korunur. FTKKisiRepository.SelectMembers içindeki bağlamlı operationId=0 filtresi sqlReader.HasLogContext üzerinden aynı anlamıyla korunur. |
| Model/Services/NBYS/SMSAylikBagisService.cs: Save/Update/Delete | Eski girişler kaldırıldı. Etkin ekranlar NbysSmsWriteService.EnsureYear/UpdateCounts kullanır. |
| Model/Services/NBYS/BagisaVesileOlanTesekkurService.cs: Save/Update/Delete | Eski girişler kaldırıldı. Güncel Persist/Remove, gerçek önce/sonra görüntüleri ve aynı transaction'da audit ile korunur. |
| Model/Services/NBYS/NBYSParametreService.cs: Save/Update/Delete ve context yoksa eski CRUD | Kullanılmayan girişler kaldırıldı. Parametre ekranı SaveChecked/UpdateChecked/DeleteChecked kullanır. Duplicate kontrolü, SQL, snapshot ve audit aynı transaction'dadır. |
| Model/Services/NBYS/ArmaganTanimService.cs ve BankaTanimService.cs: eski CRUD | Kaynakta yazma tüketicisi/etkin düzenleme ekranı kalmadığı için kaldırıldı. Mevcut tanım okumaları, banka grupları ve armağan eşikleri korunur. Yeni bir tanım yazma ekranı eklenmedi. |
| Model/NBYS/NBYSOrtak.cs | Kaynak/markup/proje taramasında sınıfın kendisi ve Compile kaydı dışında tüketici bulunmadı. Eski mail/parametre/yetki yardımcılarıyla birlikte dosya ve Model.csproj Compile kaydı kaldırıldı. FTK bildirimi NbysFtkNotificationService.Send; bağışçı yetkisi NbysBagisciWriteService.CanReadDonor üzerinden sürer. |
| Utility/ProjeGlobal/ProjeConstants.cs | Tüketicileri kaldırılmış NBYS_SAVE_LOG, NBYS_UPDATE_LOG, NBYS_DELETE_LOG kaldırıldı. Diğer proje sabitlerine dokunulmadı. |

Dokuz serviste 27 eski CRUD girişi ve iki FTK yardımcısı; dokuz repository'de 29 eski yazma girişi kaldırıldı. Ayrıca LegacyWriteDb/RequireLegacyWrite ve artık kullanılmayan tablo/builder alanları temizlendi.

## Okuma, kapalı loglama ve hata zinciri

DAO/Repositories/NBYS altındaki 14 eski repository/report sınıfından DbClass alanları, DbClass enjeksiyon kurucuları ve varsa yalnız SqlTransactionContext alan eski kurucular kaldırıldı. Dört ilk-parça repository'sinin kalan okuma yolları da bu kapsamdadır.

NbysSqlExecutor(string repositoryName) yalnız bağlamsız okuma uyumluluğu sağlar. Varsayılan servisleri kullanan mevcut rapor tüketicilerinin sorgu/parametre ve null sonuç davranışı korunur; DbClass, SQLLog_Table veya eski hata yazıcısına dönülmez. Bu uyumluluk yolu otomatik NBYS bağlamı üretmez ve bağlamsız okuma için merkezi log kaydı vaat etmez. NBYS ekranları mevcut açık LoggingContext kurucularını veya context parametreli okuma overload'larını kullanır.

Bağlamsız executor Insert/Write kabul etmez. NBYSParametreRepository.RequireTransaction ve SMSAylikBagisRepository.RequireTransaction, artık executor'ın varlığını değil gerçek bağlamın varlığını kontrol eder. HasLogContext log ayarının açık olması anlamına gelmez. Yeni log kapalı olsa da bağlam ve iş transaction'ı korunur; eski loga geri dönüş yoktur.

Importun 24 public girişinde fileName/context varsayılanları kaldırıldı. ExecuteFileImport DB erişiminden önce NBYS bağlamını doğrular. Dosya ayrıştırma kuralları, import kilidi, dosya başına transaction ve hata halinde dosyanın tüm yazılarını/audit'ini geri alma davranışı korunur. Transferdeki satır başına transaction davranışı ilk parçadaki gibi devam eder.

EkstreAktarmaService.Update(item, transaction) mevcut pozitif bağışçı bağlantısını korumak için önceki kaydı aynı transaction ile okumaya devam eder. Audit'in önce/sonra görüntüsü repository writer tarafından alınır; eski OlayKayit için ek kayıt yoktur.

ExceptionHelper yalnız import/transfer sonucu taşır; PublishException/PublishImportExceptions çağrılmaz. NbysEkstreFeedback.Batch ve NbysWebPartLog.ReportError mevcut tek teknik hata sınırıdır. Importun kullandığı HelperFunctions.ConvertTextFileToList(Stream) hata yutmaz; farklı, Encoding alan eski overload çağrılmaz. EntityBase.ToList/ToJSON eski log yazmaz. Yönerge bağlantılarının bağlamlı NBYSParametreService yolu ve FTK bildirimindeki bağlamlı il/ilçe/bölge/SMTP okumaları doğrulandı.

PortalTechnicalLog, PortalLogRepository, SqlTransactionContext, audit alan izin listeleri ve kullanıcı hareketi sınırları değiştirilmedi. Teknik Error/SqlTrace bağımsız bağlantıyla yazılmaya devam eder. Gerçek SQL izi bir commit garantisi değildir. İş verisi ile audit aynı transaction'dadır; log ayarına göre audit atlanması eski audit yazıcısını devreye sokmaz.

## Ekran / SharePoint bağımlılıkları

NBYS_WebParts.csproj, Features/Feature1/Feature1.feature, Package/Package.package, Elements.xml ve SharePointProjectItem.spdata envanterinde NBYS'ye ait eski olay/SQL log listeleme WebPart'ı yoktur. NBYS'de sırf log temizliği için iş ekranı/feature/paket öğesi kaldırılmadı. ASCX ve üretilmiş *.ascx.g.cs değiştirilmedi; yeni kontrol/handler eklenmedi.

| Paylaşılan ekran | Veri / yeni karşılık / sonraki adım |
| --- | --- |
| Portal_WebParts/OlayListesiWP/OlayListesiWP.ascx.cs: TabloJson / OlayService.GetByDateAndProgram | Olay_Table geçmişini okur. Yeni audit/alan farkı karşılığı Portal_WebParts/LogDurumTakipWP/LogDurumTakipWP.ascx.cs: AppendAuditTable ve merkezi dashboard servisidir. Eski geçmiş otomatik taşınmaz. Portal aşamasında arşiv erişimi ve feature/paket bağlantılarıyla değerlendirilmelidir. |
| MTS_WebParts/MtsOlayListesiWP | MTS eski olay ekranıdır; MTS aşamasında değerlendirilmelidir. NBYS kapanışında kaldırılmaz. |
| Portal_WebParts/LogIzlemeWP/LogIzlemeWP.cs | Merkezi UserActivity/Error listesi; NBYS proje filtresi, tarih/kullanıcı/OperationId ve sayfalama vardır. Korunur. |
| Portal_WebParts/LogDurumTakipWP/LogDurumTakipWP.ascx.cs | Merkezi UserActivity/Error/Audit/SqlTrace ve audit farklarını gösterir. Korunur. |

Diğer projeler ve ortak eski OlayService/OlayRepository/DbClass halen kullanıldığı için global dosyalar/assembly referansları bu dalda silinmedi. Bunların kaynak kapanışı Portal → MTS → MFYS sırasındadır. Kaynak incelemesi çözüm dışı derlenmiş istemcileri, SharePoint sayfa içeriklerini veya DB trigger/job'larını kanıtlayamaz; bunlar kullanıcı/DBA kabulünde doğrulanmalıdır.

## Statik kontroller

- 26 değişmiş C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi açısından incelendi: hata yok. Semantik derleme/emission/test/debug yapılmadı.
- Solution genelinde 334 NBYS servis/static import-transfer çağrısı ve 392 repository çağrısı argüman sayısı kontrolü: eksik overload yok. Servis/repository katalog kontrolü temiz.
- 335 yerel metot çağrısı argüman kontrolü temiz. Bu kontroller semantik tip denetiminin yerine geçmez.
- NBYS service/repository genelinde 200 transaction parametreli metot ve 118 doğrudan DB çağrısı aktarımı incelendi. Dört okumada açık transaction==null kolu vardır; transaction verilen kollarda eksik transaction argümanı yoktur. Fabrika, callback ve writer zincirleri ayrıca kaynak üzerinden incelendi.
- Başlangıçla ortak 137 repository SQL/parametre sözleşmesinde değişiklik yok. Teşekkür listelemesinin bağlamlı ve bağlamsız ayrı SELECT projeksiyonları korunur.
- NBYS C# kaynaklarında OlayKayit/OlayService/OlayRepository/Olay_Table/SQLLog_Table/DbClass/PublishException/LegacyWriteDb/RequireLegacyWrite başvurusu kalmadı. Üç eski NBYS log bayrağı solution C# kaynaklarından kaldırıldı.
- UTF-8, CRLF, BOM ve Compile kayıtları kontrol edildi. git diff --check temiz.
- Veritabanına bağlanılmadı; eski tablo, geçmiş log, iş arşivi veya şema değiştirilmedi.

## Kullanıcının uygulayacağı kabul senaryoları

1. Visual Studio 2026'da tüm solution'ı derleyin/paketleyin ve Test sunucusunda yayımlayın. Varsa çözüm dışı istemcilerde kaldırılan API/kurucu kullanımını doğrulayın.
2. Aşağıdaki akışları merkezi log açık ve tamamen kapalı durumda tekrarlayın. Tam kapatma için NBYS hem EnabledProjects hem ilgili EnabledWebParts kapsamı dışında olmalıdır; mevcut ayarlar OR ile değerlendirilir.
3. Banka dosyası yüklemede kullanılan tüm formatlar: geçerli dosya, bozuk tarih/tutar, boş dosya, mükerrer fiş, tekrar yükleme, eşzamanlı yükleme ve akış okuma hatası. Hatalı dosyanın hiçbir ekstre/audit'i kalmamalı; sayaçlar geri alınan INSERT'leri başarılı saymamalı.
4. Dosyanın ilk INSERT'inden sonra, sonraki satırda ve audit yazımında ayrı ayrı kontrollü hata uygulayın. Dosyanın tamamı geri alınmalı; teknik SQL/hata kayıtları mevcut tasarıma uygun kalabilmeli. Tek hata bildirimi, Türkçe güvenli mesaj beklenir.
5. TC/telefon/ad ile bağışçı eşleştirme, bulunamayan/çakışan kayıt, il kodu dönüştürme; ekstre elle ekleme/güncelleme/eşleştirme/silme. Mevcut pozitif bağışçı ilişkisi ve aktarılmış kayıt korumaları sürmeli.
6. Ekstreden bağışa aktarımda mevcut/yeni/bilinmeyen bağışçı, tekli/çoklu armağan ve tekrar/eşzamanlı aktarım. Hata yalnız ilgili satırı geri almalı; önceki tamamlanmış satırlar kalmalı. Bilinmeyen bağışçı için ilk parçadaki tek INSERT audit kabulünü yineleyin.
7. FTK kişi oluştur/güncelle/sil/pasifleştir, kurul oluştur/güncelle/düzelt, tarih/kimlik çakışması, geçmişi olan kişi, il/ilçe/bölge sınırları, işlem filtresindeki tümü/0 seçimi ve bildirim hatası. İş yazısı/audit birlikte geri alınmalı; commit sonrası bildirim hatası başarılı işi geri almış gibi gösterilmemeli.
8. SMS yıl/ay oluşturma ve adet güncelleme; tekrar/çakışan güncelleme ve yetkisiz kullanıcı. Aylar ve audit aynı transaction'da kalmalı.
9. Teşekkür/armağan belgesi kaydet/güncelle/sil/bas ve değişmemiş form; NBYS parametresi ekle/güncelle/sil/duplicate; banka grubu ve armağan eşik okumaları. Audit başlığı ve güvenli alan farklarında eksik veya çift iş kaydı olmamalı.
10. Diğer NBYS raporları, Excel çıktıları, yönerge bağlantıları ve ilk parçadaki bağışçı/bağış/armağan/düzenli bağış akışlarını gerileme açısından kontrol edin. İlk açılışta tek UserActivity ve aynı istekte ortak OperationId beklenir.
11. Log açıkken LogIzlemeWP/LogDurumTakipWP üzerinden NBYS UserActivity, Audit, SqlTrace, Error ve audit farklarını kontrol edin. Log kapalıyken iş işlemleri devam etmeli; eski tabloların NBYS trafiğine ait yeni kaydı oluşmamalı.
12. Eski tablo yazısını ölçerken diğer projelerin eşzamanlı trafiğini ayırın. Trigger, stored procedure, job ve çözüm dışı uygulamaların da eski tabloları yazmadığını DBA ile doğrulayın.

## Eski tablolar / arşivleme / ileride kaldırma

Bu dal tablo veya geçmiş veri silmez. Olay_Table ve SQLLog_Table'a global yazma engeli de koymaz; sıradaki projelerin eski yolları henüz kapanmadı.

1. DBA ile tabloya bağımlı uygulama/assembly, SP/trigger/view/synonym, SQL Agent job, rapor ve dış tüketici envanteri çıkarılmalı. İlgili tarih/uygulama/hesap bazında yazma izlemesiyle kaynak taraması tamamlanmalı.
2. Kurumun saklama ve erişim politikasına uygun salt okunur arşiv hazırlanmalı; satır sayıları, dönem aralıkları, anahtar/ilişki bütünlüğü ve geri yükleme doğrulanmalı. Yeni merkezi tablolar geçmiş kayıtların bire bir kopyası varsayılmamalı.
3. Tüm projeler ve dış tüketiciler kapandıktan, test/kullanım gözlemi tamamlandıktan sonra eski ekranın arşiv işlevi veya kaldırılması kararlaştırılmalı. SharePoint sayfalarındaki WebPart örnekleri ve feature/paket/GAC bağımlılıkları kontrol edilmeli.
4. Eski tablolara yazmayı kapatma, gözlem ve olası tablo kaldırma ayrı DBA değişiklikleri olarak onaylanmalı; geri dönüş planı olmalı. Bu dalda hiçbir DB kaldırma script'i uygulanmadı.

# Portal eski loglamayı kapatma — parça 1/2

Tarih: 24.10.2026  
Dal: codex/portal-eski-log-parca1  
Başlangıç master: d0c9709212e3e46f7ad87350143dc66d4b9e5018 (NBYS PR #549 sonrası).

Yerel master temizdi ve canlı uzak master ile aynı commit'te doğrulandı. Kullanıcı Portal çalışmasını 1–2 dalda istedi; kapsam iki dala ayrıldı. Bu dal iş servislerinin/repository'lerin temizliğidir; Portal genel kapanışı ve çalışma zamanı kabulü değildir.

## İki dalın planı

| Dal | Kapsam |
| --- | --- |
| 1 — bu dal | Duyuru, gösterim, okuma, toplantı, katılım ve toplantı parametresi eski yazmaları; bağlamsız DbClass okumaları; kullanılmayan ProgramYetki service/repository ve eski Portal log bayrakları. |
| 2 — kapanış | OlayListesiWP geçmiş erişimi, merkezi LogIzlemeWP/LogDurumTakipWP karşılıkları, SharePoint proje/feature/paket ve sayfa bağımlılıkları; ortak yardımcıların hata/callback zincirleri ve Portal genel kapanış taraması. |

İkinci dal için bu parça kullanıcı tarafından birleştirilip master'a alındıktan sonra temiz master/canlı uzak eşitliği yeniden doğrulanacaktır. Kullanıcı testten önce devam isteyebilir; commit, push, PR, merge, pull, derleme, yayın ve çalışma zamanı testleri kullanıcıya aittir.

## Bulgular ve değişiklikler

Aşağıdaki yollar repository köküne göredir.

| Eski giriş | Etkin karşılık / karar |
| --- | --- |
| Model/Services/Portal/DuyuruService.cs: Save/Update/Delete | Kaynak tüketicileri kalmamıştı; kaldırıldı. DuyuruGirisiWP mevcut PortalDuyuruWriteService.Save/Delete/ClearImage yolundadır. |
| Model/Services/Portal/DuyuruGosterimService.cs: Save/Update/Delete/SaveDuyuru | Eski sürüm kopyalama zinciri kaldırıldı. PortalDuyuruWriteService.SaveVersion, duyuru ve gösterimi aynı transaction'da yazar. SaveVersion ayrı kullanıcı API'si değildir. |
| Model/Services/Portal/DuyuruOkumaService.cs: Save/Update/Delete | Eski girişler kaldırıldı. DuyuruPopupWP mevcut PortalDuyuruWriteService.MarkRead kullanır; kullanıcı/alıcı/sürüm doğrulaması ve tekil okuma korunur. |
| Model/Services/Portal/ToplantiService.cs: Save/Update/Delete | Eski OlayKayit dallarıyla birlikte kaldırıldı. ToplantiGirisiWP mevcut PortalToplantiWriteService.Save/Delete kullanır. |
| Model/Services/Portal/ToplantiKatilimService.cs: Save/Update/Delete/DeleteByMeeting | Eski bağımsız katılım yazmaları kaldırıldı. Toplantı servisi katılımcı farklarını, katılım silmelerini ve toplantıyı aynı transaction'da işler. |
| Model/Services/Portal/ToplantiParametreService.cs: Save/Update/Delete | Kaynak tüketicisi olmayan girişler kaldırıldı. ToplantiParametreGirisiWP mevcut PortalToplantiWriteService.SaveParameter kullanır. |
| DAO/Repositories/Portal altındaki aynı altı repository | Transaction'sız Insert/Update/Delete ve ToplantiKatilimRepository.DeleteByMeeting kaldırıldı. DbClass alanları/enjeksiyon kurucuları, eski yazma koruyucuları ve kullanılmayan CrudQueryBuilder alanları kaldırıldı. |
| Model/Services/Portal/ProgramYetkiService.cs ve DAO/Repositories/Portal/ProgramYetkiRepository.cs | Sınıfların kendileri ve Compile kayıtları dışında kaynak/proje başvurusu bulunmadı. İki sınıf ve DAO.csproj/Model.csproj Compile kayıtları kaldırıldı. ProgramYetki modeli, tablo/veri ve NBYS'nin NbysSupportReadRepository.PermissionSettings okuması korunur. |
| Utility/ProjeGlobal/ProjeConstants.cs | PORTAL_SAVE_LOG, PORTAL_UPDATE_LOG, PORTAL_DELETE_LOG tüketicileriyle birlikte kaldırıldı. Bu sabitler başlangıçta false olsa da eski public CRUD girişleri, DbClass SQL/hata yollarıyla çağrılabilir durumdaydı. |

Altı serviste 20 eski yazma/yardımcı metot; altı repository'de 19 eski yazma metodu kaldırıldı. Tamamen kaldırılan ProgramYetki sınıfları ayrıca üçer eski yazma girişi içeriyordu. Yeni bir iş ekranı/yazma yeteneği oluşturulmadı.

## Okuma ve yeni log kapalıyken davranış

DAO/Repositories/Portal/PortalSqlExecutor.cs içine yalnız bağlamsız okuma uyumluluğu sağlayan repositoryName kurucusu eklendi. Altı varsayılan repository artık bu okuyucuyu kullanır; SELECT aynı SQL ve parametreleri çalıştırır, sıfır satırda null davranışı korunur. SQL hatası eski DbClass hata yazıcısına gönderilmez; çağıranın mevcut hata sınırına ulaşır.

Açık PortalLogContext kurucuları ve context parametreli mevcut okuma overload'ları korunur. Portal ekranları bu yollarla merkezi SQL izi üretmeye devam eder. Bağlamsız uyumluluk okuması otomatik bir Portal/WebPart kimliği üretmez veya merkezi log kaydı vaat etmez; ortak rapor/eski MTS yardımcılarının okumaları için eski loga yazmayan uyumluluktur.

PortalSqlExecutor.Insert/Write hem gerçek bağlam hem SqlTransactionContext ister. Bağlamsız okuyucu yazma yapamaz. Bağlamın varlığı log ayarının açık olması anlamına gelmez: PortalLogging kapalı olduğunda iş SQL'i mevcut transaction'da çalışır, log ayarına göre audit/teknik kayıt atlanır, eski yazıcıya geri dönülmez.

PortalDuyuruWriteService/Repository, PortalToplantiWriteService/Repository ve PortalToplantiNotificationService dosyaları başlangıçla aynıdır. Şu sözleşmeler korunur:

- Duyuru, gösterim sürümü ve audit birlikte; okundu kaydı, doğrulamalar ve audit birlikte commit/rollback edilir.
- Toplantı, katılımcılar, parametre, yetki/çakışma/sürüm kontrolleri ve audit aynı SqlTransactionContext üzerinden yürür.
- Başarı commit sonrasındadır; SMTP/takvim bildirimi iş transaction'ının dışındadır. Bildirim hatası kaydedilmiş işi geri almaz.
- Audit alan izin listeleri genişletilmedi; duyuru gövdesi, başlık/alıcılar ve toplantı serbest metinleri alan farklarına eklenmedi.
- PortalTechnicalLog'un bağımsız teknik Error/SqlTrace yazma tasarımı, PortalLogRepository ve SqlTransactionContext değiştirilmedi.
- UI kullanıcı hareketi/hata sınırları değiştirilmedi. Aynı SQL için ek bir trace veya ikinci iş audit çağrısı eklenmedi.

## Bu dal sonunda kalan erişilebilir eski yollar

Portal iş service/repository C# kaynaklarında DbClass, OlayKayit, RequireLegacyWrite ve eski Portal log bayrakları kalmadı. Bu sonuç tüm solution'ın eski loglardan arındığı anlamına gelmez.

Portal_WebParts/OlayListesiWP/OlayListesiWP.ascx.cs: TabloJson hâlâ Model.Services.Ortak.OlayService.GetByDateAndProgram(..., LogContext) üzerinden geçmiş Olay_Table verisini okur. ParseAciklama içindeki MTS faaliyet/kişi/obje okumaları da kapanış dalında tekrar izlenecektir. Tarihsel kayıtları silmek veya merkezi audit kaydı gibi yeniden yazmak bu dalın parçası değildir.

Merkezi karşılıklar Portal_WebParts/LogIzlemeWP/LogIzlemeWP.cs (UserActivity/Error) ve Portal_WebParts/LogDurumTakipWP/LogDurumTakipWP.ascx.cs (Audit/alan farkı/SqlTrace dahil dashboard) dosyalarıdır. Eski ekranın gerekli geçmiş erişimi ve yeni kayıtların izlenmesi ikinci dalda açıkça ayrılacak; gereksiz ekran kararı verilirse proje/feature/paket bağlantıları birlikte ele alınacaktır. Yeni merkezi sistemin geçmiş Olay açıklamasının bire bir kopyası olduğu varsayılmayacaktır.

Eski OlayService/OlayRepository/DbClass solution'ın diğer projelerinde kullanıldığı için ortak sınıflar/assembly referansları global olarak silinmedi. MTSOrtak içindeki eski toplantı bildirim yardımcılarına bu dalda dokunulmadı; Portal'ın etkin bildirim zinciri PortalToplantiNotificationService'dir. MTS genel temizliği kendi proje sırasındadır.

Hiçbir ASCX, üretilmiş *.ascx.g.cs, SharePoint feature veya paket dosyası değiştirilmedi. Yeni kontrol/handler eklenmedi; bu dal için ASCX üretimi gerekmez.

## Statik doğrulama

- 14 değişmiş mevcut C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi açısından kontrol edildi: hata yok. Semantik compilation/emission, build, test veya debug çalıştırılmadı.
- Solution genelinde 52 Portal servis çağrısı ve 111 repository çağrısının argüman/overload sayıları kontrol edildi: çözülemeyen çağrı yok. Kaldırılan ProgramYetki sınıfları için kaynak/proje referansı kalmadı.
- 72 yerel metot çağrısının argüman kontrolü temiz.
- Portal service/repository kaynaklarında 31 SqlTransactionContext parametreli metot ve 31 doğrudan DB/audit çağrısı aktarımı incelendi. Eksik transaction argümanı yok. Executor.Select yalnız transaction null kaldığında normal bağlantı açar; transaction verilmiş dal doğrudan transaction.Select kullanır.
- Başlangıçla ortak 35 repository SQL/parametre sözleşmesi aynı. Alt/üst tarih sınırları, tekrar sorguları ve sonuç projeksiyonları yeniden tasarlanmadı.
- Beş mevcut merkezi yazma/bildirim dosyasının içerikleri başlangıçla aynı.
- 16 değişmiş mevcut kaynak/proje dosyasında UTF-8, BOM ve CRLF; 44 XML/proje/feature/paket/öğe dosyasında XML yapısı; dört projede Compile dosyalarının varlığı kontrol edildi. git diff --check temiz.
- Bu kontroller semantik derleme ve çalışma zamanı doğrulamasının yerine geçmez. Solution dışı derlenmiş istemci/reflection bağımlılığı kullanıcı tarafından ayrıca kontrol edilmelidir.

## Kullanıcının uygulayacağı kabul senaryoları

1. Visual Studio 2026'da solution'ı derleyin/paketleyin; Test sunucusunda yayımlayın. Kaldırılan API ve kurucuları kullanan çözüm dışı bileşen varsa tespit edin.
2. Aşağıdaki akışları merkezi log açık ve tamamen kapalı durumda tekrarlayın. Tam kapatma için Portal hem EnabledProjects hem ilgili EnabledWebParts kapsamı dışında olmalıdır; mevcut yapı OR ile değerlendirilir.
3. Duyuru ekle/güncelle/sil/resim kaldır; popup tekrar/yayın aralığı/alıcı kontrolü ve okuyanlar listesi. Duyuru ve yeni gösterim birlikte değişmeli; geçmiş sürüm ve dosyaları korunmalı.
4. Aynı sürümü iki sekmeden okundu yapın; eski veya başkasına ait sürümü gönderin. Tek okuma kalmalı; geçersiz çoklu istekte kısmi okuma/audit olmamalı.
5. Duyuru/gösterim/okuma SQL'i veya audit yazımında kontrollü hata uygulayın: ilgili iş transaction'ı bütünüyle geri alınmalı. Resim yüklendikten sonra DB reddi için mevcut kısmi sonuç mesajını ve kitaplık dosyasını kontrol edin.
6. Toplantı oluştur/güncelle/sil, katılımcı ekle/çıkar/bilgi durumunu değiştir, tutanak/liste/takvim ve parametre ekle/güncelle/sil. İş ve audit birlikte; SMTP commit sonrasında çalışmalı.
7. Aynı salon/saat için eşzamanlı kayıt, eski form güncellemesi, yetkisiz kullanıcı, aynı adlı parametre, kullanılan yer ve “Diğer” kaydı korumasını deneyin. İş kuralları ve tek sonuç mesajı korunmalı.
8. Katılım ve audit yazımında kontrollü hata: toplantı dahil tüm yazmalar geri alınmalı, SMTP çağrılmamalı. SMTP hatasında başarılı DB işlemi korunmalı ve mevcut güvenli kısmi sonuç mesajı çıkmalı.
9. Portal ana sayfa toplantı/duyuru bileşenlerini ve ortak toplantı okumalarını gerileme açısından kontrol edin. SQL erişim hatası boş sonuç veya eski başarılı veri gibi gösterilmemeli.
10. Log açıkken açılışta tek UserActivity, gerçek SQL komutları için mevcut SqlTrace, iş kayıtlarında mevcut Audit/alan farkları ve teknik hatada mevcut Error/OperationId bekleyin. Çift audit/trace/popup oluşmamalı.
11. Log kapalıyken iş akışları çalışmalı; bu dalda kapatılan Portal iş zincirlerinden Olay_Table veya SQLLog_Table'a yeni kayıt gitmemeli. Diğer projelerin eşzamanlı trafiğini ölçümden ayırın.
12. Eski olay ekranının geçmiş erişimini mevcut haliyle gerileme açısından kontrol edin; genel ekran kapanışı ikinci dalın kabul kapsamı olacaktır.

## Veritabanı ve arşiv sınırı

Veritabanına bağlanılmadı; eski tablolar, geçmiş loglar, ProgramYetki tablosu veya iş verileri silinmedi/değiştirilmedi. Global eski tablo yazma engeli konulmadı.

Tüm projeler ve dış tüketiciler kapandıktan sonra DBA ile trigger/SP/view/job/rapor/uygulama envanteri ve yazma izlemesi tamamlanmalı. Saklama politikasına uygun salt okunur arşiv, satır/dönem/ilişki kontrolü ve geri yükleme doğrulanmalı. Eski tabloya yazmayı kapatma ve olası sonraki kaldırma, SharePoint sayfalarındaki WebPart örnekleri de kontrol edilerek ayrı onaylı DBA/yayın değişiklikleri olmalıdır.

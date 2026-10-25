# MTS eski loglamayı kapatma — parça 1

Tarih: 24.10.2026  
Dal: codex/mts-eski-log-parca1  
Başlangıç master: 9d1ad69173aab2b636ec9f18f905a480e6c9207e (Portal kapanışı PR #552 sonrası).

Yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı. Kullanıcının devam isteğiyle proje sırasındaki MTS'ye geçildi. Bu dal eski iş yazma API'lerini ve DB sınıfı bağımlılıklarını kapatır; MTS'nin genel kaynak/çalışma zamanı kabulü değildir.

## Kapsam ve sonraki parça

İlk parça: kişi, arama/görüşme, faaliyet, katılım, anı objesi dağıtımı/tanımı, depo, kurum/görev ve unvan eski yazmaları; dokuz repository'nin okumaları; Faaliyet.ToJSON hata yan etkisi ve üç eski MTS log bayrağı.

Kapanış parçası: MtsOlayListesiWP'nin geçmiş erişimi/yeni sistem karşılıkları; MTSOrtak içindeki kullanılmayan eski toplantı/faaliyet e-posta yardımcıları ve kalan ortak çağrı zincirleri; TestWP/VisualWebPart1/VisualWebPart2 proje/feature/paket bağımlılıkları ve genel kapanış. MFYS'ye henüz geçilmedi.

Yeni parçadan önce kullanıcı bu dalı birleştirip master'a alacak; temiz master/canlı uzak eşitliği tekrar doğrulanacaktır. Kullanıcı runtime testten önce açık devam isteyebilir. Commit, push, PR, merge, pull, derleme, yayın ve çalışma zamanı testleri kullanıcıya aittir.

## Bulgular / kaldırılan girişler / etkin karşılıklar

Yollar repository köküne göredir. Model/Services/MTS altında dokuz servisten 28 eski yazma metodu kaldırıldı. DAO/Repositories/MTS altındaki karşılıklarından 28 eski yazma metodu kaldırıldı. Kaynak çağrılarındaki kalan tüketiciler ve overload'lar tarandı; etkin ekranlar zaten merkezi yazma servislerini kullanmaktadır.

| Eski servis / repository | Kaldırılan yol | Etkin karşılık / karar |
| --- | --- | --- |
| KisiService / KisiRepository | Save/Update/Delete ve Insert/Update/Delete | MtsContactWriteService.SavePerson/DeletePerson; mevcut kimlik/ilişki/sürüm kontrolü, aynı transaction'da audit |
| AramaGorusmeService / AramaGorusmeRepository | Eski CRUD | MtsContactWriteService.SaveCall/DeleteCall/CreateActivity; arama-faaliyet bağlantısı aynı transaction'da |
| FaaliyetService / FaaliyetRepository | Eski CRUD, bağımsız eski UniqueId oluşturma ve OlayKayit | MtsActivityWriteService.Save/Move/Delete; kişi/aramadan oluşturma için MtsContactWriteService.CreateActivity; mevcut idempotent kimlik ve sürüm akışları korunur |
| FaaliyetKatilimService / FaaliyetKatilimRepository | Eski CRUD | MtsActivityWriteService.AddParticipant/RemoveParticipant/SetContact ve merkezi kişi/arama oluşturma zinciri |
| AniObjesiDagitimService / AniObjesiDagitimRepository | Eski CRUD ve DeleteByActivityAndParticipant | MtsActivityWriteService.Deliver/Return/SaveObjects/SaveReceived; stok/dağıtım yazıları aynı transaction'da, stoksuz seçim ve getirilen obje ayrımı korunur |
| AniObjesiTanimService / AniObjesiTanimRepository | Eski CRUD | Kaynakta etkin yazma tüketicisi yok; mevcut obje tanım okumaları korunur. Yeni tanım düzenleme ekranı eklenmedi |
| DepoTanimService / DepoTanimRepository | Eski CRUD | Kaynakta etkin yazma tüketicisi yok; mevcut depo okumaları korunur. Stok hareketleri merkezi faaliyet servisi/repository'sindedir |
| MTSKurumGorevService / MTSKurumGorevRepository | Eski CRUD | Kaynakta etkin yazma tüketicisi yok; mevcut kurum/görev okumaları ve merkezi katılımcı metni oluşturma sorgusu korunur |
| MTSUnvanTanimService / MTSUnvanTanimRepository | Eski CRUD | Kaynakta etkin yazma tüketicisi yok; mevcut unvan okumaları korunur |

AniObjesiDagitimRepository.DeleteByActivityAndParticipant, önce bağımsız SELECT sonra DELETE yapıp servis içinde eski olay kaydı üreten erişilebilir public API idi. Artık bu yol yoktur. Gerekli etkin stoksuz seçim silmeleri SaveObjects içinde transaction/sürüm kontrolüyle çalışır. Stoklu ürün iadesi Return ile özgün depoya gider; toplu eski silme geri getirilmedi.

MTS_SAVE_LOG, MTS_UPDATE_LOG ve MTS_DELETE_LOG, Utility/ProjeGlobal/ProjeConstants.cs içinden tüketicileriyle birlikte kaldırıldı. Başlangıçta false olmaları eski CRUD ve DbClass yazmalarını erişilemez yapmıyordu; DbClass mutasyonlarının hata yolları eski SQL log yazıcısına ulaşabiliyordu.

## Dokuz repository'nin okuma yolu

DbClass alanları/enjeksiyon kurucuları, RequireLegacyWrite ve kullanılmayan CrudQueryBuilder alanları kaldırıldı. Sınıfların okumaları kullanıldığı için bu sınıflar ve mevcut Compile kayıtları korunur; kullanılmayan eski yazma yeteneği kaldırılır.

DAO/Repositories/MTS/MtsSqlExecutor.cs içine repositoryName alan bağlamsız, yalnız okuma uyumluluğu sağlayan kurucu eklendi. Varsayılan repository okumaları artık bu okuyucuyla çalışır. SELECT metni/parametreler ve sıfır satırda null sonucu korunur. Otomatik MTS/WebPart kimliği üretilmez; bağlamsız uyumluluk okumaları için merkezi kayıt vaat edilmez.

Açık MTS bağlamlı kurucular aynı SELECT'leri merkezi SQL iziyle yürütür. Read yardımcısı CallerMemberName değerini executor'a aktarır; repository/metot etiketi korunur. SQL metni/parametre değerleri trace'e eklenmez.

MtsSqlExecutor.Insert/Write, gerçek MTS bağlamı ve SqlTransactionContext ister. Yeni bağlamsız okuyucudan yazma yapılamaz. Context varlığı logun açık olduğu anlamına gelmez: merkezi log kapalıyken iş SQL'i çalışır; audit/teknik log mevcut ayara göre atlanır, eski yazıcıya dönüş yapılmaz.

Kisi/Faaliyet/AniObjesiDagitim repository'lerinin açık PortalLogContext parametreli mevcut okuma overload'ları korunur. Portal'ın geçmiş olay arşivindeki kişi/faaliyet/verilen-getirilen obje okumaları bu overload'ları kullanmaya devam eder. MTS kurucuları başka projenin bağlamını yazma yetkisi olarak kabul edecek şekilde genişletilmedi.

DbClass.SelectFromDb(SqlQuery, string) overload'ı zaten eski SQL izi yazmıyordu. Okuma refaktörü mevcut bir SELECT log yazma hatasını kapatmak diye sunulmamalıdır; eski DB sınıfına bağımlılığı kaldırır. Eski mutasyon API'leri ve hata yazma yolları ise tamamen kaldırıldı.

## Faaliyet.ToJSON hata yolu

Model/MTS/Faaliyet.cs: ToJSON(List<CalendarEvent>) içinde catch, ExceptionHelper.PublishException çağırdıktan sonra aynı hatayı yeniden fırlatıyordu. Bu, çağıranın hata sınırına ek bir eski hata yayımlama yan etkisi oluşturuyordu.

Serializer'ın try/catch sarmalı kaldırıldı. Metot yalnız JSON üretir veya hatayı çağırana yayar. Null liste için [] sonucu, MaxJsonLength, dokuz JSON anahtarı ve Faaliyet veri özellikleri korunur. Takvim renderer'ları veya iş kuralları yeniden tasarlanmadı. Mevcut merkezi hata sınırı MtsWebPartLog/çağıran ekrandır; model kendi başına hata logu/popup üretmez.

## Korunan transaction / audit / teknik kayıt tasarımı

Aşağıdaki dosyalar başlangıç sürümüyle aynı:

- DAO/Repositories/MTS/MtsContactWriteRepository.cs
- DAO/Repositories/MTS/MtsActivityWriteRepository.cs
- Model/Services/MTS/MtsContactWriteService.cs
- Model/Services/MTS/MtsActivityWriteService.cs
- Model/Services/MTS/MtsActivityNotificationService.cs
- DAO/Ortak/SqlTransactionContext.cs
- MTS_WebParts/Logging/MtsCalendar.cs

PortalApps.MTS.Write kilidi, kayıt/sürüm/ilişki kontrolleri, aynı transaction'da iş/audit yazımı ve rollback korunur. Audit whitelist'i ve kişisel veri kapsamı değişmedi. Stok düşme/artırma ile dağıtım yazıları ayrı eski servis çağrılarına bölünmedi. SMTP commit sonrasındadır; gönderildi işaretinin ayrı transaction/audit tasarımı korunur. Teknik Error/SqlTrace mevcut bağımsız yazma tasarımındadır; SQL başarısı commit garantisi değildir.

Bu dalda yeni iş audit kaydı, ikinci SQL trace veya yeni açılış kaydı eklenmedi. Merkezi yazma servislerinin tablo izin listeleri değişmedi.

## İlk parça sonunda kalan eski kapsam

MTS service/repository C# kaynaklarında OlayKayit, DbClass, RequireLegacyWrite ve eski üç MTS log bayrağı kalmadı. Faaliyet modelinde PublishException kalmadı.

MTS_WebParts/MtsOlayListesiWP/MtsOlayListesiWP.ascx.cs: GetDataList hâlâ OlayService.GetByDateAndProgram(..., LogContext) üzerinden eski olay tablosunu okur. Bu geçmiş erişimi ve Portal'daki yeni Log İzleme sekmeleriyle işlev eşleşmesi kapanış dalında ele alınacaktır.

Model/MTS/MTSOrtak.cs içindeki eski e-posta yardımcıları ve örnek ekranların proje/paket durumu bu ilk parçada kaldırılmadı. İş ekranlarının mevcut nitelikli MTSOrtak çağrıları ParseFaaliyetAmaci/ParseFaaliyetDurumu okumalarıdır; gerekli bu yardımcılar kapanışta korunmalıdır. Eski bildirim köklerinin kendi iç çağrıları ve dış tüketicileri ayrı doğrulanacaktır.

Ortak OlayService/OlayRepository/DbClass diğer solution tüketicileri bitmeden global silinmedi. Bu dal MTS genel kapanışı veya bütün solution için eski tablolara sıfır yazma kabulü değildir. Kaynak dışı derlenmiş istemci, reflection, DB trigger/job/rapor ve SharePoint sayfa örnekleri kullanıcı/DBA kabulünde doğrulanmalıdır.

## Statik kontroller

- 21 değişmiş C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi yönünden incelendi: hata yok. Build, test, debug, semantik compilation/emission veya runtime çalıştırılmadı.
- Solution genelinde saptanan 109 MTS servis ve 170 repository çağrısının argüman/overload kontrolü temiz. 94 yerel metot çağrısında eksik arity bulunmadı. Bu taramalar genel semantik tip kontrolü yerine geçmez.
- Başlangıçla ortak 45 repository SQL/parametre sözleşmesinde değişiklik yok.
- Yedi merkezi yazma/transaction/bildirim/takvim dosyasının içeriği aynı. İş/audit transaction çağrıları ve mevcut tablo whitelist'i kaynak üzerinden incelendi.
- Faaliyet veri özellikleri ve serializer'ın dokuz JSON anahtarı başlangıçla aynı.
- 61 XML/proje/feature/paket/öğe dosyası ve dört projenin Compile dosyalarının varlığı kontrol edildi.
- UTF-8/BOM/CRLF ve git diff --check temiz. ASCX/g.cs, WebPart, proje, feature ve paket dosyaları değiştirilmedi; yeni alan/handler eklenmedi.
- Veritabanına veya çalışan SharePoint'e bağlanılmadı; eski tablo/geçmiş veri değiştirilmedi.

## Kullanıcının uygulayacağı kabul senaryoları

1. Visual Studio 2026'da tüm solution'ı derleyin/paketleyin; Test sunucusunda yayımlayın. Kaldırılan public API/DbClass kurucularını kullanan çözüm dışı bileşen varsa tespit edin.
2. Aşağıdaki akışları merkezi log açık ve tamamen kapalıyken tekrarlayın. Tam kapatma için MTS hem EnabledProjects hem ilgili EnabledWebParts kapsamı dışında olmalıdır; mevcut yapı OR ile değerlendirir.
3. Kişi ekle/güncelle/sil, kimlik/ilişki korumaları, eski sürüm; arama ekle/güncelle/sil ve kişi/aramadan faaliyet oluşturmayı deneyin. Tekrarlı onayda ikinci faaliyet oluşmamalı.
4. Faaliyet ekle/güncelle/sil, irtibat ve katılımcı ekle/çıkar, eski form/UID ve bağlı kayıt korumalarını kontrol edin. Katılım/audit hatasında yarım faaliyet veya katılım kalmamalı.
5. Stoklu obje dağıtımı ve kısmi/tam iade: dağıtım ve özgün depo stoku birlikte değişmeli; farklı depoların dağıtımları birleşmemeli. Sıfır/negatif/fazla iade, yetersiz stok ve eski modal isteği reddedilmeli.
6. Stoksuz obje ilk ekleme/adet değiştirme/sıfırlama/seçimden çıkarma/boş seçim ile getirilen obje metni ekle/güncelle/silmeyi deneyin. Stoklu ve getirilen kayıtlar stoksuz temizliğinden etkilenmemeli.
7. Stok güncellendikten sonra dağıtım veya audit yazımında ayrı kontrollü hata: tüm ilgili iş verileri/audit geri alınmalı. Eşzamanlı dağıtım/iade ve aynı isteği yeniden göndermede ikinci stok hareketi oluşmamalı.
8. Depo/obje/unvan/kurum-görev listeleri, kişi/faaliyet kartları ve listeler, takvim/Excel çıktıları; Portal geçmiş olay arşivinin kişi/faaliyet/obje açıklamalarını gerileme açısından kontrol edin.
9. Takvimde tarih/süre değişikliği, davet edilmiş katılımcı ve özel kalem bildirimi. SMTP hatası tamamlanmış işi geri almamalı; mevcut kısmi sonuç mesajı korunmalı. JSON/okuma hatasında modelden ikinci eski popup/log gelmemeli.
10. Log açıkken doğru MTS/WebPart/OperationId, tek UserActivity, gerçek komutların SqlTrace'leri ve mevcut tekil Audit/alan farklarını doğrulayın. Teknik hata güvenli Türkçe mesajla aynı OperationId'de bulunmalı.
11. Log kapalıyken iş transaction'ları çalışmalı; bu dalda kapatılan eski servis/repository zincirlerinden Olay_Table/SQLLog_Table'a yeni kayıt gitmemeli. MTS'nin bu dal dışında kalan yardımcılarını ve diğer projelerin eşzamanlı trafiğini ölçümden ayırın.

## Veritabanı / arşiv sınırı

Olay_Table, SQLLog_Table, geçmiş loglar ve iş arşivleri silinmedi; global yazma engeli konulmadı. Kapanışta eski ekran/okuma bağımlılıkları ayrıca envantere alınacaktır.

Tüm projeler ve dış tüketiciler bittikten sonra DBA ile trigger/SP/view/synonym/job/rapor/uygulama envanteri ve hesap/tarih bazlı yazma izlemesi tamamlanmalı. Saklama politikasına uygun salt okunur arşiv, sayım/dönem/ilişki kontrolü ve geri yükleme doğrulanmalı. Yazmayı kapatma ve olası sonraki tablo kaldırma ayrı onaylı DBA/yayın değişiklikleridir.

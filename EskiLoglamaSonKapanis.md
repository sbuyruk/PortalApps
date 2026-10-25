# Eski loglama altyapısı — son ortak kapanış

Tarih: 2026-10-25
Dal: codex/ortak-loglama-son-kapanis
Başlangıç: 3745626c0ade5b70764008c59275d2c33cc3a738 (PR #555).
Değişiklik öncesinde yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı.

## Bulgular ve yapılan değişiklikler

Proje sırası IKYS → TBYS → NBYS → Portal → MTS → MFYS tamamlandıktan sonra ortak yardımcıların kalan çağrı zincirleri incelendi. PR #555 eski tablo yazıcılarını kaldırmıştı. Bu dal eski sunum/hata yutma yollarını ve bunlara özgü son bağımlılığı kapatır.

| Dosya / metot | Önceki yol | Sonuç |
| --- | --- | --- |
| Model/Ortak/UtilityHelper.cs: ParametreDegeriSorgula(string) | Bağlamsız DB okuması; hatada eski LogHelper, ardından null | Dış çağrısı olmayan overload kaldırıldı. |
| Aynı dosya: üç parametreli UploadFileToSharePoint, DeleteFileFromSharePointLib, GetFileListFromSharePointLib, GetFileNameListFromSharePointLib | ExceptionHelper.PublishException; hata/boş sonuç ayrımı olmayan eski yollar | Dış çağrıları yok; dört metot kaldırıldı. Kullanılan bağlamlı dosya overload'ları aynen korundu. |
| Aynı dosya: ParametreDegeriSorgula(string, PortalLogContext) | Merkezi hata yazıp null dönüyordu; çağıranda ikinci hata oluşabilir veya SMTP alternatifine devam edilebilirdi | DB hatası artık eksik parametre sonucu sayılmaz; istisna mevcut bağlamlı bildirim sınırına iletilir. Yardımcı ikinci teknik hata yazmaz. Gerçekten eksik parametre hâlâ null döner; bu durumdaki mevcut alternatif SMTP davranışı korunur. |
| Aynı dosya: IsGroup | AD hatasını ham exception.Message ile gösterip false dönüyordu | Gerçek grup yokluğu false; AD erişim hatası mevcut IKYS bildirim sınırına yükselir. PrincipalContext ve GroupPrincipal dispose edilir. |
| Utility/HelperClasses/MailHelper.cs | Kullanılmayan EPostaGonder, TakvimeEkle, TakvimdenSil, CreateCalenderObject, BodyOlustur | Beş metot kaldırıldı. Kullanılan EPostaGonderVeyaHataVer metot gövdesi aynen korundu. |
| Utility/HelperClasses/ExceptionHelper.cs | Kullanılmayan PublishException / PublishImportExceptions → LogHelper + popup | İki sunum metodu kaldırıldı. Sonuç listesi, sayaçlar, kurucular, HasException ve HasFatalException korunur. |
| Utility/HelperClasses/LogHelper.cs, Utility/Utility.csproj | Eski özel ULS sınıfı ve Compile kaydı | Tüm tüketicileri yukarıdaki kullanılmayan yollar içindeydi. Dosya ve Compile kaydı kaldırıldı. |

Toplam 12 yardımcı metot ile artık tüketicisi olmayan LogHelper sınıfı kaldırıldı. MessageHelper sınıfı veritabanı yazıcısı değildir: kullanılan Türkçe Messages katmanına yönlendirir, bu yüzden korunur. ExceptionHelper da kullanılan iş sonucu/aktarım sözleşmesidir; sınıfın tamamı kaldırılmadı.

Sadece isim eşleşmesiyle karar verilmedi. Örneğin IzinTalep.EPostaGonder alanı eski MailHelper.EPostaGonder metodu değildir; korunur. MTS TestWP düğme olaylarının benzer adları da eski MailHelper takvim metotlarına bağımlı değildir.

## Etkin hata zincirleri

- UtilityHelper.IsGroup'un tek çağrısı Model/IKYS/IKYSOrtak.cs: BilgiSistemMailGrubunaEPostaGonder içindedir. Mevcut catch, PortalTechnicalLog.WriteError(context, "IKYS.Notification", error) ve iş kaydının tamamlandığını belirten Türkçe uyarıyı üretir. AD hatası artık sessizce grup yokluğu sayılmaz.
- Bağlamlı ParametreDegeriSorgula için altı IKYS bildirim çağrısı ve TBYS TasinmazResimleriWP.BildirimEPostasiGonder çağrısı incelendi. Hata mevcut bağlamlı bildirim/ekran sınırında raporlanır. Bu yardımcıdan bağımsız ikinci Error kaydı çıkmaz; başarısız SELECT izi mevcut executor'da kalır.
- EPostaGonderVeyaHataVer için dokuz çağrı vardır: sekiz IKYS ve bir TBYS. Gönderim/sahiplik davranışı korunur; hatayı çağıran katman ele alır.
- Dosya işlemlerinde 10 bağlamlı upload, 9 bağlamlı delete, 4 bağlamlı dosya adı listeleme çağrısı korunur. Kaldırılan üç parametreli yolların dış çağrısı yoktur.
- Güncel MTS ve Portal takvim/bildirim üretimi kendi MtsActivityNotificationService ve PortalToplantiNotificationService sınıflarındadır; eski MailHelper takvim kodunu çağırmaz.
- Portal LogIzlemeWP.LoadResults içindeki SPDiagnosticsService.Local.WriteTrace, kendi hata tanılaması için korunur. Kullanılmayan özel LogHelper sınıfının kaldırılması bu etkin ULS çağrısını kaldırmaz.

## Son kaynak durumu ve korunan tasarım

C# uygulama kaynakları/proje kayıtları/konfigürasyon taramasında OlayKayit, OlayService, OlayRepository, SorguyuLogla, SQLLog_Table yazıcısı, eski *_SAVE/UPDATE/DELETE_LOG bayrakları, PublishException/PublishImportExceptions ve LogHelper bağımlılığı kalmadı.

Eski tablolara kalan tek etkin kaynak erişimi DAO/Repositories/Portal/PortalEventArchiveRepository.SelectDay içindeki salt okunur Olay_Table SELECT'idir. Portal ve MTS geçmiş ekranları arşiv olarak korunur. Yeni kayıtlar Portal Log İzleme'de proje filtresi ve Kullanıcı hareketleri / İşlem denetimi / SQL izi / Hata sekmelerindedir. Arşiv okumak geçmiş kayıtları yeni sisteme taşımaz veya çoğaltmaz.

SqlTransactionContext, PortalLogRepository, PortalTechnicalLog, PortalLogSettings, PortalSqlReadExecutor ve iş yazıcıları değiştirilmedi. İş verisi ve audit aynı transaction içinde kalır; SQL/hata kayıtları mevcut bağımsız yazma tasarımını sürdürür. Merkezi log kapalıyken eski yazıcıya dönüş yolu bulunmaz.

SharePoint ekran/proje/feature/paket kararları önceki proje kapanış notlarında bulunur. Bu dal yeni ekran veya paket değişikliği getirmez. MTS TestWP, önceki parçada mevcut sayfa örnekleri için inert tip olarak korunmuştu; fiziksel kaldırma farm kullanım envanterine bağlıdır. Paket dışındaki boş VisualWebPart şablonları etkin dağıtım öğesi sayılmadı.

## Statik kontroller

- Değişen üç C# kaynağı Roslyn ile yalnızca sözdizimi bakımından incelendi: 0 hata. Derleme/semantic compilation, test veya debug çalıştırılmadı.
- Korunan 42 metot gövdesi önceki sürümle aynı. Yalnızca IsGroup ve bağlamlı ParametreDegeriSorgula hata akışları değişti.
- ExceptionHelper'ın sonuç alanları/özellikleri/kurucuları aynen korundu.
- Dokuz csproj ve 808 Compile yolu kontrol edildi: eksik kaynak yok. Derlenen kaynaklar, üretilmiş dosyalar dahil, kaldırılan API bağımlılıkları için incelendi: etkin referans yok.
- 986 UtilityHelper ve 9 MailHelper çağrısı için metot/argüman sayısı kontrolünde uyumsuzluk yok. Tam tip/overload çözümü kullanıcı derlemesinde doğrulanmalıdır.
- Merkezi transaction/log/arşiv, güncel MTS/Portal bildirim ve MessageHelper dosyalarından dokuzu önceki sürümle karşılaştırıldı: değişmedi.
- UTF-8/BOM ve git diff --check kontrol edildi. Hiçbir .ascx veya .ascx.g.cs değiştirilmedi; kod üretimi gerekmiyor.

## Kullanıcı kabul senaryoları

1. Visual Studio 2026'da solution'ı derleyin; Test sunucusunda güncel Utility/Model/DAO ve uygulama paketlerini tutarlı sürümlerle yayımlayın.
2. IKYS personel bildirimi, izin talebi ve görev onayı e-postaları ile TBYS taşınmaz bildiriminin normal alıcı/SMTP akışlarını deneyin. Başarılı iş kaydı ve gönderim sonucu değişmemeli.
3. Kontrollü AD erişim hatasında IKYS personel bildirimi tek merkezi hata ve başvuru kodlu Türkçe uyarı vermeli. Ham AD mesajı, ikinci popup veya yanlış başarı olmamalı. Gerçekte bulunmayan grup teknik arıza gibi sayılmamalı.
4. Bildirim sırasında ortak parametre SELECT'ini kontrollü olarak hataya düşürün. Bir başarısız SQL izi ve bir teknik hata bekleyin; ikinci null erişim hatası veya alternatif SMTP ile gönderime devam oluşmamalı. Gerçekten eksik SMTP parametresinde mevcut alternatif adres davranışını ayrıca kontrol edin.
5. TBYS dosya yükleme/silme, taahhüt ve poliçe dosya listelerini normal/başarısız durumlarda deneyin. Bağlamlı davranış, güvenli kullanıcı mesajı ve merkezi hata ilişkisi korunmalı.
6. NBYS içe aktarma/toplu işlem/armağan işlerinde ExceptionHelper sayaçları, atlanan/daha önce işlenen kayıt ve gerçek hata ayrımını kontrol edin. Koleksiyon sonuçlarının değişmediğini doğrulayın; eski otomatik popup metotları kullanılmamalı.
7. Altı projede log açık/kapalı temsilî yazma ve rollback senaryolarını uygulayın: iş sonucu/audit transaction bütünlüğü, bağımsız teknik hata/SQL izi, çift kayıt olmaması ve eski iki tabloya sıfır yeni satır koşullarını birlikte doğrulayın. Portal/MTS arşivleri açılmalı ve eski satırlar korunmalı.
8. Sonuçları değerlendirip geçişin çalışma zamanı kabulünü bildirin. Kaynak temizliğinin tamamlanması bu kabulün yerine geçmez.

## Veritabanı, dış bağımlılık ve sonraki kaldırma

Bu çalışma eski tablo, geçmiş veri, şema, yetki veya migration silmez/değiştirmez. Önceki migration yorumları ve uygulamaya derlenmeyen eski test fixture'ı tarihsel adları içerebilir; gerçek tablo yazıcısı değildir.

Ayrı envanter: Olay_Table / SQLLog_Table kullanan trigger, procedure, view/function, SQL Agent işi, rapor/ETL, dinamik SQL, harici uygulama ve dağıtılmış eski assembly/eklenti. Depodaki kaynak taraması bu dış tüketicilerin kapandığını kanıtlamaz.

Ayrı arşivleme kararı: kurumun saklama süresini ve erişim gereksinimini belirleyin; doğrulanmış yedek/geri yükleme ile erişimi sınırlı salt okunur geçmişi koruyun. Tüm paketler ve dış yazıcılar kapandıktan sonra yazma yetkilerini kısıtlayıp izleme süresini tamamlayın. Tablo kaldırmayı ancak bağımlılık onayı, arşiv geri yükleme doğrulaması ve ayrıca verilen kullanıcı kararı sonrasında planlayın.
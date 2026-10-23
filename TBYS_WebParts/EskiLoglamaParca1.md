# TBYS eski loglamayı devreden çıkarma — iki parçalık plan, parça 1

Tarih: 23.10.2026  
Dal: `codex/tbys-eski-log-parca1`  
Başlangıç: `e2df84ff2fd867c88c4951857e056064f1d1c1a2` (PR #545 sonrası).

Değişiklikten önce yerel master temizdi ve canlı uzak master ile aynı commit'te olduğu doğrulandı. Kullanıcı, IKYS sonrasında TBYS'ye geçilmesini ve TBYS'nin iki parçada yapılmasını istedi. Bu dal yalnızca ilk parçadır; commit/push/PR/merge/yayın kullanıcıya aittir.

## İki parçanın sınırı

| Parça | Kapsam |
| --- | --- |
| 1 — bu dal | Ekranların kalan doğrudan bağlamsız okumaları; bağlamlı ödeme zincirinin eski hata yayını; ödeme/ayrıntı toplu silmelerinde eksik audit; ödeme planı üretiminde yutulan hatalar ve aynı planın iki kez güncellenmesi. |
| 2 — sonraki dal | Kira/sözleşme/tahsilat/teminat/ekstre/bakiye devri ve yardımcıların kalan eski yolları; eksik transaction/audit kapsamı; kullanıcıları doğrulanarak eski servis/repository overload'larının, bayrakların ve referansların kaldırılması; TBYS genel kapanışı ve ekran/SharePoint bağımlılıkları. |

İkinci parça ilk parçanın kullanıcı tarafından birleştirilmesi, master'a pull edilmesi ve test değerlendirmesi veya testten önce açık devam isteği sonrasında, tekrar temiz master/uzak eşitliği doğrulanarak yeni bir codex/ dalında yapılmalıdır. NBYS'ye geçilmedi.

## Bulgular ve uygulanan değişiklikler

### Ekran okumaları ve ortak yardımcılar

34 WebPart'ın okuma çağrılarında kalan doğrudan bağlamsız yollar değiştirildi. TBYS servisleri mevcut `Service(LoggingContext)` kurucularına; il/ilçe/bölge ve IKYS personel iş bilgisi/birim okumaları açık bağlam overload'larına bağlandı. Toplam 59 ortak servis/IKYS yardımcı çağrısına bağlam eklendi; buna ek olarak kalan bağlamsız TBYS servis kurucuları bağlandı.

Örnek inceleme noktaları:

- `TBYS_WebParts/KiradaOlanTasinmazlarWP/KiradaOlanTasinmazlarWP.ascx.cs` ve `KirayaVerilmeyenTasinmazlarWP`: TasinmazService kurucuları.
- `EnvanterdenCikanTasinmazListesiWP`, `MulkiyetiOlmayanTasinmazListesiWP`: liste sorguları.
- `MevcutKiracilarWP`: SozlesmeTasinmazService metrekare okuması.
- `KiraSozlesmesiWP`, `KiraciGirisiWP`, taşınmaz/bağışçı/vasiyetçi girişleri: il/ilçe/bölge okumaları.
- `BorcluKiraciIslemleriWP`: IsBilgileriService/BirimTanimService okumaları.
- Bölge yetkisi için `IKYSOrtak.BolgeGetirByUserName` çağrıları.

`Model/IKYS/IKYSOrtak.cs::BolgeGetirByUserName(string, PortalLogContext)` eklendi. Personel, iş bilgisi, birim ve bölge okumalarının tümü bağlamlıdır; eski lazy PersonelItem zincirine girmez. Eksik personelde Türkçe mesaj, eksik iş/birim kaydında null döner. Diğer projelerin bağlamsız overload'ı değiştirilmedi.

`Model/Services/Ortak/IlService.GetByName(string, PortalLogContext)` ve `DAO/Repositories/Ortak/IlRepository.SelectByName(string, PortalLogContext)` eklendi. Mevcut LOWER karşılaştırması ve parametre korunur; doğrudan PortalSqlReadExecutor kullanılır. Bağlamsız ve transaction overload'ları korunmuştur.

### Ödeme ve hata zinciri

`Model/Services/TBYS/OdemeService.cs` içindeki transaction alan:

- OdemeyiKaydetOdemePlaniniGuncelle
- OdemeyiSilOdemePlaniniGuncelle
- OdemeyiVeOdemePlaniniGuncelle

metotlarının ExceptionHelper.PublishException çağırıp hatayı null/false'a çeviren catch blokları kaldırıldı. Artık hata transaction sahibine çıkar; tamamlanmayan transaction geri alınır. Teknik hata ekran sınırındaki mevcut TbysWebPartLog ile kaydedilir; kapalı loglama eski hata tablosuna dönüş yapmaz. Ödeme insert'i pozitif kimlik üretmezse açıkça başarısız olur.

`Model/TBYS/TBYSOrtak.cs::OdemeYap(..., PortalLogContext)` içindeki eski hata yayını da kaldırıldı. ref odemeId yalnızca transaction Dispose/commit başarılı olduktan sonra doldurulur; exception halinde başlangıçtaki sıfır değeri kalır.

Yeni hata yayılmasının kullanıcıya ham istisna olarak çıkmaması için mevcut catch'i olmayan beş ekran handler'ına TbysWebPartLog.ReportError sınırı eklendi:

- OdemeGirisWP.OnaylaBtn_Click
- KiraciAylikOdemeWP.SilNowBtn_Click
- OdemePlaniWP.OdemePlaniEkleBtn_Click
- TeminatIslemleriWP.ModalEkleNowBtn_Click
- TeminatIslemleriWP.ModalGuncelleNowBtn_Click

Mevcut banka ekstresi satır yakalaması ve ödeme planı ödeme handler'ı zaten merkezi sınır kullanır. ThreadAbort yeniden fırlatılır. Ödeme/teminat/ekstre gibi daha geniş, birden fazla bağımsız transaction içeren ekran işlemlerinin tamamını bu parça tek transaction'a çevirmemiştir; bunların değerlendirmesi parça 2 kapsamındadır.

Aynı ödeme planına yapılan ödeme güncellemesinde önceki/yeni plan aynı kimlikse artık plan bir kez güncellenir. İki farklı plan söz konusuysa ikisinin tutarları mevcut şekilde güncellenir.

### Toplu silme audit'i

`DAO/Repositories/TBYS/OdemeRepository.SelectBySozlesmeId(..., transaction, context)` ve `OdemeAyrintiRepository` karşılığı eklendi. Sözleşmeye ait satırlar UPDLOCK/HOLDLOCK ile aynı transaction'da okunur.

`OdemeService.DeleteBySozlesmeId(..., tx)`, `OdemeAyrintiService.DeleteBySozlesmeId(..., tx)` ve `DeleteByOdemeIdOdemePlaniId(..., transaction)`:

1. Silinecek gerçek satırları aynı transaction ile okur.
2. Silmeyi aynı transaction ile yapar.
3. Her gerçek satır kimliği için bir DELETE audit başlığı yazar.

Boş kümede başarı döner, hayali audit yazılmaz. Audit başarısızsa silme de geri alınır. Aynı satırlar için ayrıca tekil Delete çağrılmadığından ikinci audit üretilmez. Mevcut alan gizliliği kapsamı korunur; serbest metin/kimlik/adres ayrıntısı audit'e eklenmedi.

OdemeAyrintiService'in iki kimlik alan bağlamlı, transaction sahibi olmayan silme girişi de transaction overload'ını çağıracak şekilde düzeltildi. Eskiden bağlamlı kurucuya rağmen doğrudan ayrı bağlantıda DELETE yapıp audit'i atlayabiliyordu.

### Ödeme planı üretimi

`OdemePlaniService.CreatePaymentPlan(..., transaction)` hata yutup false döndürmez. SaveNew/SaveCarryOver pozitif kayıt kimliği gerektirir. Böylece ara satır/audit hatası kaybolmaz; bağlamlı transaction sahibi bütün planı geri alır ve hata ekran sınırında görünür. Bağlamsız eski plan üretimi ikinci parçada kaldırılacak uyumluluk yoludur; o yol için bu parça atomiklik iddiasında bulunmaz.

## Statik kontroller

- 45 değiştirilmiş C# dosyası Roslyn ParseText/GetDiagnostics ile yalnızca sözdizimi yönünden incelendi: sıfır hata. Derleme, compiler emission, test veya debug yapılmadı.
- Servis/repository imza ve çağrı argüman sayısı taraması: sıfır sorun.
- 489 doğrudan servis/repository çağrısının argüman sayısı; 772 yerel metot çağrısı kontrol edildi: çözülemeyen argüman sayısı yok.
- TBYS servis/repository/UI kapsamındaki 333 transaction parametreli metot tarandı; saptanan 178 doğrudan DB çağrısı aktarımında eksik transaction argümanı bulunmadı. Bu metinsel kontrol semantik derleme/çalışma zamanı kanıtı değildir.
- Değişen repository dosyalarında başlangıç sürümünde de bulunan 48 sorgu/parametre sözleşmesi karşılaştırıldı: fark yok. Yeni kilitli toplu silme okumaları ayrıca kaynak üzerinden incelendi.
- 45 kaynak dosyasında UTF-8/CRLF, başlangıç BOM durumu ve proje Compile kayıtları kontrol edildi. 36 değiştirilmiş WebPart kod dosyasının 272 ASCX olay bağlantısında eksik handler yok.
- git diff --check temiz. Üretilen ASCX, ASCX kaynakları, proje Compile kayıtları, feature/paket ve merkezi log/transaction altyapısı değiştirilmedi.

## Kullanıcının uygulayacağı testler

1. Visual Studio 2026'da solution derleme/paketleme; Test sunucusunda yayın. Bu kaynak kontrolleri semantik derlemenin yerine geçmez.
2. TBYS proje ve ilgili WebPart log ayarlarının açık/kapalı birleşimlerinde aynı işlemleri tekrarlayın. İlgili bağlam kapalıyken iş transaction'ı korunmalı; bu dalda taşınan yollar eski loga dönmemeli.
3. Taşınmaz/bağışçı/vasiyetçi/kiracı listeleri, harita, adres seçimleri, boş sonuç ve Excel; bölgesel yetki filtreleri önceki veri kapsamını korumalı. Eksik personel/iş/birim kaydını da deneyin.
4. Ödeme ekleme, silme ve plan değiştirmede başarılı sonuç; aynı plan üzerinde güncellemede tek plan UPDATE/audit, farklı plana taşımada iki planın doğru tutarı.
5. İş yazısı veya audit adımında kontrollü SQL hatası: ilgili ödeme transaction'ının bütün iş/audit kayıtları geri alınmalı; eski hata tablosuna yazılmamalı. Merkezi hata kaydı aynı exception için çoğalmamalı; kullanıcı Türkçe genel hata ve başvuru kodu görmeli.
6. OdemeYap commit hatası: ref odemeId sıfır kalmalı, başarı yönlendirmesi olmamalı. Teknik SQL izinin bağımsız kalması iş kaydının commit edildiği anlamına gelmez.
7. Sözleşme bazlı ödeme ve ayrıntı silme; ödeme/plan bazlı ayrıntı silme: gerçek satır başına tek audit, boş/ikinci silmede hayali audit yok. Audit hatasında satırlar korunmalı.
8. Ödeme planı üretiminde ara satır/audit hatası: bağlamlı üretimde önceki satırlar da geri alınmalı; görünür teknik hata oluşmalı. Aylık/yıllık ve farklı taksit sayılarını deneyin.
9. Eşzamanlı aynı sözleşmeye ödeme/ayrıntı ekleme ve toplu silme: kilit bekleme, deadlock ve son iş/audit tutarlılığını değerlendirin.
10. Banka ekstresi ve teminat ekranlarında hata mesajları/ThreadAbort/Excel regresyonu. Bu ekranların farklı transaction'lara yayılmış bütün iş zincirinin atomik kapanışı parça 2 kabulünde ayrıca kontrol edilecek.

## Açık kalan eski yollar ve veritabanı

Bu dal **TBYS genelinde eski tablolara sıfır yazma kapanışı değildir**. Model/Services/TBYS içindeki bağlamsız Save/Update/Delete ve legacy yardımcılar; Model/TBYS/TBYSOrtak.cs içindeki bağlamsız hesap/ödeme/teminat yolları; Model/TBYS/KiraBakiyeDevriOtomatik.cs içindeki doğrudan OlayService.Save yolu ikinci parçanın somut inceleme kapsamıdır.

TBYS_SAVE_LOG/TBYS_UPDATE_LOG/TBYS_DELETE_LOG ve bunların kullanıcıları bu dalda kaldırılmadı. Ortak OlayService/OlayRepository ve diğer projelerin bağımlılıkları korunur. Başarı ölçümü yalnız isim aramasıyla yapılmayacak; ikinci parçada çağrı zincirleri, overload'lar, catch blokları ve eski/yeni ekran karşılıkları birlikte doğrulanacaktır.

Veritabanına bağlanılmadı; Olay_Table, SQLLog_Table veya geçmiş kayıtlar değiştirilmedi/silinmedi. Bütün projeler/dış yazıcılar kapanmadan ortak tablolara global yazma engeli konulmamalı. Arşivleme ve daha sonra kaldırma için DBA bağımlılık envanteri, saklama politikası, salt okunur arşiv ve geri yükleme doğrulaması ayrı süreçtir.

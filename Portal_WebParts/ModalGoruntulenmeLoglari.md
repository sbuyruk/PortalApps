# Portal modal görüntülenme kayıtları

Çalışma dalı: `codex/portal-modal-goruntulenme-loglari`

Başlangıç: temiz yerel `master` ve `origin/master`, `2808391e65638c4d4a30add7834ac5b73b61c380`.

## Değişen davranış

| Webpart | Yeni etkinlik kaydı koşulu |
| --- | --- |
| GunlukYemekListesiMoveWP | Yemek modalının `shown.bs.modal` olayı |
| KutlamaMoveWP | Kutlama modalının `shown.bs.modal` olayı; otomatik açılış da dahildir |
| DuyuruPopupWP | Bu webparta ait bir duyuru modalının `shown.bs.modal` olayı; otomatik açılış da dahildir |
| BildirimWP | `ModalAcDiv` bildirim modalının `shown.bs.modal` olayı |
| GununSozuWP | Otomatik ana sayfa içeriği için açılış kaydı kaldırıldı; ayrı modal yoktur |

BildirimWP'nin mevcut `Messages.Info` mesajları ve görev bağlantıları korunur. Bilgi mesajının otomatik çıkması, bildirim modalının açılması sayılmaz. Görev bağlantısının hedefindeki ekranın mevcut kayıt akışı devam eder.

DuyuruPopupWP artık her postback'te açılış kaydı yazmaz. Duyuru/kutlama çerezi modalı engelliyorsa kayıt oluşmaz. Duyuru görüntülenmesi, “okudum” denetim kaydının yerine geçmez; mevcut okundu işlemi korunur.

## Uygulama ve inceleme noktaları

- Ortak `Logging/PortalModalActivity.cs` yardımcısı `OnInit` sırasında kaydedilir. Callback'i mevcut webpartın `ICallbackEventHandler` uygulaması karşılar; kontrol ağacına yeni kontrol eklenmez. Yeni ASCX alanı veya üretilen dosya değişikliği yoktur.
- Sunucu bildirimi ASP.NET `ICallbackEventHandler` kanalıyla yapılır. Kullanıcı, proje ve webpart bilgileri sunucudan alınır. İstek kimliği/event validation, ViewState ziyaret belirteci ve SharePoint form digest kontrolü uygulanır.
- Dinleyici modal açan startup scriptlerinden önce kaydedilir. Duyurularda webpartın `ClientID` işareti kullanılarak ilgili modal seçilir. Dinleyici belge üzerinde olduğundan UpdatePanel ile yeniden oluşturulan modallarda da çalışır.
- Callback gönderilmeden önce form verisi güncellenir; kısmi postback sonrası ViewState ve digest değerleri kullanılır.
- Callback isteğinde `PortalWebPartLog.ExecuteAction` ile sarmalanan Portal işlemleri çalıştırılmaz. Böylece modal bildirimi diğer webpartların yükleme sorgularını ve SQL izlerini tekrar üretmez. Diğer isteklerin davranışı değişmez.
- İstemcide bekleyen/tamamlanmış bildirim kontrolü, sunucuda atomik `HttpRuntime.Cache.Add` ile tekrar engeli vardır. Ziyaret belirteci ViewState içinde normal ve kısmi postback boyunca korunur. Birden fazla duyuru modalı aynı webpart için tek açılış sayılır.
- Sunucu tekrar engeli 8 saatlik uygulama önbelleğidir. Önbellek tahliyesi, uygulama havuzu yeniden başlaması veya farklı sunucuya yönlendirme sonrası yeniden bildirim gelirse ek kayıt oluşabilir; veritabanında kalıcı benzersizlik kısıtı eklenmemiştir.
- Kayıt yazılamazsa önbellek engeli kaldırılır. Modal tekrar açıldığında yeniden denenebilir; kullanıcıya teknik hata gösterilmez. Mevcut teknik hata kayıt yolu korunur.

## Ana sayfa açılış hatası düzeltmesi

Düzeltme dalı: `codex/portal-modal-callback-kontrol-hatasi`. Başlangıç: temiz `master` ve güncel `origin/master`, `8f1c71386895abb889b7dd6dd4d7ba0231cbc983`.

Test sunucusundaki çağrı yığını, `KutlamaMoveWP.OnInit → PortalModalActivity.Register → ControlCollection.Add` yolunda hata oluştuğunu doğruladı. ASCX içindeki `<% ... %>` kod blokları kök denetim koleksiyonunu salt okunur yaptığından, önceki `owner.Controls.Add` çağrısı ana sayfanın ilk açılışını engelliyordu.

Yemek, kutlama, duyuru ve bildirim webpartları artık callback'i doğrudan karşılayıp ortak yardımcıya yönlendirir. Yardımcı sınıf bir sunucu kontrolü olmaktan çıkarılmıştır; `Controls.Add` kullanılmaz. Ziyaret belirteci webpartın kendi ViewState alanında saklanır. Event validation kaydı `Page.SaveStateComplete` olayında, ViewState serileştirildikten sonra yapılır; böylece doğrulama verisi gönderilecek ViewState ile eşleşir. Modal gösterimi, form digest ve tekrar engeli kontrolleri korunur.

Event validation ile ViewState ilişkisi için teknik başvuru: [Microsoft .NET Framework kaynak kodu — ClientScriptManager](https://github.com/microsoft/referencesource/blob/main/System.Web/UI/ClientScriptManager.cs).

Bu düzeltmede `.ascx` ve `.ascx.g.cs` değişmedi; Run Custom Tool gerekmiyor. Build/test/debug çalıştırılmadı. Öncelikli Test sunucusu kontrolleri: ana sayfanın ilk açılması, dört modalın ilk açılış kaydı, duyuru UpdatePanel işlemi sonrasında callback doğrulaması ve aynı ziyarette tekrar açılışın yeni kayıt üretmemesidir.

ASP.NET callback yaşam döngüsü için başvuru: [Microsoft Learn — Implementing Client Callbacks](https://learn.microsoft.com/en-us/previous-versions/aspnet/ms178208%28v%3Dvs.100%29).

## Kullanıcı tarafından yapılacak doğrulama

Windows 11 üzerinde build, test veya debug çalıştırılmadı. Kod farkları, proje dosyası ve kodlama statik olarak incelenmiştir. Visual Studio 2026 derlemesi ve Test sunucusu doğrulaması gereklidir.

1. Ana sayfayı açın. Bu beş webpart için yalnız yüklenmeden kaynaklanan yeni `WebPartOpened` kaydı oluşmamalı. Otomatik gerçekten açılan kutlama/duyuru modalı kendi kaydını oluşturmalı.
2. Yemek modalını açın, kapatın, tekrar açın. Bir kayıt oluşmalı; tekrar açılışta sayı artmamalı. Sayfayı yenileyip açınca yeni ziyaret için bir kayıt daha oluşmalı.
3. Kutlama ve duyurularda hem otomatik hem tıklayarak açılışı, hem de çerezin gösterimi engellediği durumu kontrol edin. Birden çok duyuru açılması aynı ziyarette bir webpart kaydı üretmeli.
4. Bildirim modalını açın. Açılmadan kayıt oluşmamalı; otomatik görev bilgi mesajı tek başına modal açılışı sayılmamalı. Amir onayı kapalıyken görünmeyen modal için kayıt oluşmamalı.
5. Duyuruya tıklayarak UpdatePanel postback'i gerçekleştirin. Modal açılışının kaydedildiğini ve “okudum” işleminin çalıştığını kontrol edin. Sonraki postback ve yeniden açılışlar aynı ziyaretin kayıt sayısını artırmamalı.
6. Tarayıcı ağ sekmesinde modal açılışının tam sayfa postback'i yapmadığını, callback yanıtının başarılı olduğunu kontrol edin. Log İzleme'de callback'in diğer Portal webpartları için yeni açılış ve yükleme SQL izleri üretmediğini doğrulayın.

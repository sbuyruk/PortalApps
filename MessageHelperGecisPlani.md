# Mesaj gösterimi

Yeni kullanım için yalnızca `using Utility.HelperClasses;` eklenir. Nesne oluşturma,
Page parametresi veya ek kurulum gerekmez:

```csharp
Messages.Success("Kayıt kaydedildi.");
Messages.Info("Kayıt bulunamadı.");
Messages.Warning("Lütfen bir kayıt seçin.");
Messages.Error("İşlem tamamlanamadı. Lütfen yeniden deneyin.");
```

İsteğe bağlı ikinci parametre, mesajın kaç milisaniye açık kalacağını belirtir.
Verilmezse veya sıfırsa kullanıcı kapatana kadar açık kalır. Negatif süre geçersizdir:

```csharp
Messages.Success("Kayıt kaydedildi.", 3000);
```

Bağlantılı bilgi mesajı da tek çağrıdır. HTML yazılmaz:

```csharp
Messages.Info(amirOnayMesaji, "Onay bekleyen görev listesini aç", newUrl);
```

Mevcut hata loguyla ilişkilendirmek için isteğe bağlı başvuru kodu verilebilir:

```csharp
// Teknik hata, WebPart'ın mevcut log yöntemiyle ayrıca kaydedilir.
Messages.Error("İşlem tamamlanamadı. Lütfen yeniden deneyin.",
    referenceCode: context.OperationId.ToString("N"));
```

## Dosyalar ve sorumluluklar

- `Utility/HelperClasses/Messages.cs`: Dört kısa mesaj metodu, bağlantı kontrolü,
  sayfa başına kuyruk ve script kaydı. Mesajın türünü çağrılan metot belirler.
- `Messages.js`: Güvenli DOM oluşturma, kapatma ve erişilebilirlik.
- `Messages.css`: Bildirimlerin görünümü. JS ve CSS Utility.dll içine gömülür;
  ayrıca SharePoint kitaplığına dosya yüklemek gerekmez.
- Mevcut `MessageHelper.cs`: Eski `PublishMessage` imzalarını korur ve çağrıyı
  `Messages` sınıfına yönlendirir. Yeni kodda `Messages` kullanılır.

Ayrı mesaj modeli, enum, presenter, interface veya factory bulunmaz. Bu yardımcı
Web Forms sunum katmanına aittir. İş servisleri mesaj göstermez; sonuç/istisna
döndürür ve hangi mesajın gösterileceğini WebPart belirler.

## Davranış

- Metinler HTML olarak yorumlanmaz. Önceden `HtmlEncode` yapılmaz. Satır sonları
  korunur. Bağlantı metni ve adresi ayrı parametrelerdir.
- Bağlantılarda HTTP/HTTPS ve `/` ile başlayan yerel adresler kabul edilir.
  `javascript:`, `data:`, `//`, ters eğik çizgi ve kontrol karakterleri reddedilir.
- Aynı yanıttaki bütün mesajlar GUID kimlikleriyle tek script bloğunda, çağrı
  sırasıyla gönderilir. jQuery veya global snackbar fonksiyonu gerekmez.
- Mesajlar Load, kontrol olayları veya PreRender içinde üretilir. Kuyruk
  PreRenderComplete sırasında gönderilir. Bu aşamadan sonra, SaveStateComplete
  veya Render içinde mesaj oluşturulmaz.
- Tam postback ve UpdatePanel için aynı kayıt yöntemi kullanılır. Bildirimler
  redirect veya navigasyon arasında saklanmaz; hedef sayfa gerekiyorsa yeniden üretir.
- Fare üzerindeyken veya klavye odağı bildirimdeyken kapanma süresi duraklar.
  Enter/Space ile düğme, bildirim içindeyken Escape ile kapatma kullanılabilir.
  Hatalar `alert`, diğer bildirimler `status` olarak duyurulur. Kapanan öğeler
  DOM'dan silinir. Ekran okuyucu davranışı Test ortamında ayrıca doğrulanmalıdır.
- Inline script/stil kullanımı mevcut Web Forms yaklaşımına uygundur; katı CSP
  desteği bu parçanın kapsamında değildir.

## Mevcut çağrıların durumu

Eski `MessageHelper.PublishMessage(...)` çağrıları korunur. Bilinen CSS türleri
başarı/bilgi/uyarı/hata metoduna eşlenir; serbest CSS HTML'e eklenmez. Legacy API
önceki davranışına uygun olarak negatif süreyi sıfır kabul eder. Boş mesaj gösterilmez.

`BildirimWP`, bağlantılı `Messages.Info(...)` kullanımına geçirildi. Banka yükleme
hatasındaki ön HTML kodlaması kaldırıldı. `ExceptionHelper` HTML üretmez; beklenen
`ImportAlreadyProcessedException` uyarısını gösterir, diğer istisnalarda standart
Türkçe hata kullanır. Teknik ayrıntı logda kalır. Bu, import hatalarının önceki
ayrıntılı genel Exception metni yerine standart kullanıcı mesajı göstermesi demektir.

Model/servis katmanındaki eski gösterim çağrıları ile diğer WebPart'lardaki doğrudan
`ex.Message` gösterimleri bu ortak altyapı parçasında topluca değiştirilmedi.
İş katmanını sunumdan ayırma geçişi ve bu çağrıların temizliği sonraki inceleme
parçalarında proje sırasına göre yapılmalıdır. Düz metin göstermek tek başına
teknik hata ayrıntılarını gizlemez.

## Kullanıcının doğrulayacağı noktalar

Codex build, test veya debug çalıştırmaz. Visual Studio 2026 ve Test sunucusunda:

1. Solution'ı derleyin. `Messages.cs` ve gömülü JS/CSS kaynaklarının Utility projesinde
   bulunduğunu doğrulayın. Güncel Utility ve etkilenen WebPart paketlerini yayımlayın.
2. Dört mesaj türünü, boş mesajı, çoklu mesajı ve eski PublishMessage çağrılarını
   tam postback ve UpdatePanel ile kontrol edin. Yalnızca seçilen mesaj kapanmalıdır.
3. Tırnak, `&`, Türkçe karakter, satır sonu, `<img src=x onerror=alert(1)>` ve
   `</script><script>alert(1)</script>` içeren metinler kod çalıştırmadan görünmelidir.
4. Süreli mesajın belirtilen süre boyunca kaldığını, fare/klavye ile durakladığını,
   kalıcı mesajın açık kaldığını ve kapanınca DOM'dan silindiğini doğrulayın.
5. Tab, Enter, Space, Escape, ekran okuyucu ve dar ekranla kullanımı kontrol edin.
6. BildirimWP bağlantısını ve geçersiz URL'lerin reddedilmesini kontrol edin.
7. Banka yüklemede beklenen mükerrer uyarısı ve beklenmeyen hatayı ayrı ayrı kontrol
   edin. Teknik ayrıntı ekranda görünmemeli; özel karakterler çift kodlanmamalıdır.

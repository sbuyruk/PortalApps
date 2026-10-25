# IKYS mesaj katmanı temizliği — parça 1

Tarih: 2026-10-25  
Dal: `codex/ikys-mesaj-katmani-temizligi`  
Başlangıç: `2881c274a282dbbbb2b295127fc974035798535f`

Değişiklik öncesinde yerel master temizdi ve GitHub üzerinden sorgulanan uzak master ile aynı commit'teydi.

## Değişiklikler

- 31 IKYS WebPart kaynağındaki 200 etkin `MessageHelper.PublishMessage` çağrısı doğrudan `Messages.Success/Info/Error` kullanımına geçirildi. Mevcut mesaj metinleri, türleri ve süreleri korundu. Koşula göre başarı/hata gösteren dokuz çağrı açık if/else akışına dönüştürüldü. Sekiz eski yorum çağrısı kaldırıldı.
- `IKYSOrtak.IzinTalepOlusturmaEPostasiGonder`, `GorevOnayEPostasiGonder` ve `BilgiSistemMailGrubunaEPostaGonder` artık mesaj göstermez. Beş public imza (overload'lar dahil) bool döndürür; forwarding overload'ları sonucu iletir.
- Bildirim yardımcıları istisnayı mevcut `IKYS.Notification` adı ve aynı bağlam ile kaydedip false döndürür. ThreadAbortException yükselmeye devam eder. True, yardımcıda istisna oluşmadığını belirtir; mevcut kayıt/grup/alıcı yokluğunda gönderimi atlama davranışları korunur.
- Yedi WebPart çağrısı false sonucunu kontrol eder ve başvuru kodlu Türkçe uyarıyı sunum katmanında gösterir. Gönderim hatası iş kaydını başarısız saydırmaz. Model yardımcısında yakalanan aynı hata için WebPart yeniden teknik hata yazmaz.

## İnceleme sınırları

İş transaction'ları, repository'ler ve audit yazımları değiştirilmedi. ASCX kaynakları ve üretilen `.ascx.g.cs` dosyaları değiştirilmedi; kod üretimi gerekmez. MessageHelper ortak uyumluluk sınıfı korunur.

IKYSOrtak içindeki eski bölge yardımcıları TBYS/NBYS tarafından kullanılır. Bunların mesajları ve bağlamsız okuma zincirleri ilgili sonraki parçalarda ele alınacaktır. Ortak SharePoint dosya yardımcılarının sunum ayrımı TBYS parçasındadır; bu dal Model katmanındaki tüm mesaj çağrılarını kaldırmış değildir.

## Statik inceleme

- Değişen 32 C# kaynağı Roslyn ile yalnızca sözdizimi bakımından incelendi: 0 hata.
- IKYS WebPart C# kaynaklarında MessageHelper referansı kalmadı.
- Yedi bildirim çağrısının tamamı dönüş sonucunu kontrol ediyor; iki forwarding overload sonucu döndürüyor.
- UTF-8/BOM korunumu ve `git diff --check` incelendi.
- Build, test, debug ve semantic compilation çalıştırılmadı. Çalışma zamanı kabulü kullanıcı tarafından yapılmalıdır.

## Yayımlama ve kullanıcı kabulü

Dönüş tipi değiştiği için Visual Studio 2026'da Model ve IKYS_WebParts projelerini birlikte derleyip uyumlu assembly/paket sürümlerini Test ortamına yayımlayın. Yeni Model ile eski IKYS assembly'sini karıştırmayın. Depo dışında bu public metotları kullanan derlenmiş bir eklenti varsa onu da yeniden derleyin.

1. Başarılı/hatalı personel, kimlik, iş bilgileri ve iletişim güncellemelerinde doğru mesaj türünün gösterildiğini kontrol edin. İzin hareketi güncellemesinde başarı 2000 ms, hata kullanıcı kapatana kadar açık kalmalıdır.
2. İzin talebi, şehir içi görev, yurt içi/yurt dışı görev girişi ile görev onay/red bildirimlerini normal SMTP akışında deneyin; alıcı ve içerik davranışı korunmalıdır.
3. Aynı akışlarda kontrollü SMTP/AD/parametre okuma hatası oluşturun: iş kaydı korunmalı, bildirim hatası için tek teknik hata ve başvuru kodlu uyarı görülmeli, ham istisna metni görünmemelidir. Personel bilgi sistemleri bildirimini de dahil edin.
4. Bildirim devre dışıyken, grup/kayıt/alıcı bulunmadığında mevcut atlama davranışını kontrol edin.
5. Tam postback ve UpdatePanel'de birden fazla mesajın sırası, süreleri, kapanması ve Türkçe karakterlerini kontrol edin.
6. Temsilî IKYS yazma/rollback işleminde mevcut audit bütünlüğünü doğrulayın.

Commit, push, PR ve birleştirme kullanıcı tarafından yapılır. Sonraki dala geçiş, Test sonucu veya kullanıcının açık devam talebi sonrasındadır.

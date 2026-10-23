# Log takip ve sigorta güncelleme düzeltmeleri

Başlangıç: master temiz ve uzak master ile aynı: `8bf6763c22ffd2745d1fa613f2ee018bd3edeb2c`.
Dal: `codex/log-takip-sigorta-kontrol`.

## Kapsam ve mesajlar

- LogIzlemeWP hata satırlarının “Ayrıntıyı göster” bölümüne Teknik özet, Çağrı yığını (Stack trace) ve İç hata eklendi. Metinler HTML kodlanır; satır sonları korunur, uzun alanlar kaydırılır ve kopyalanabilir. Boş alanlar “Kaydedilmemiş.” gösterir. Her istekte uygulanan mevcut ManageWeb yetki kontrolü korunur; normal iş ekranlarının hata mesajları teknik metin içermez.
- LogDurumTakipWP SQL tablosu artık başlıklarına uygun olarak WebPart, SQL işlemi, repository/metot, milisaniye ve başarı durumunu gösterir. Süre yoksa “Veri yok”, tanınmayan sonuçta “Bilinmiyor” yazılır. Bütün dinamik metinler HTML kodlanır.
- LogDurumTakipWP satır tarihleri gün filtresiyle aynı Türkiye saat diliminde gösterilir; sunucunun yerel saat dilimine bağlı dönüşüm kaldırıldı.
- TasinmazSigortaGirisWP alt bölümü olmayan taşınmazlarda bölüm numarasını metin olarak gösterir, bölüm ID'sini 0 gönderir. Böylece numara, başka bir bağımsız bölümün ID'si sanılmaz. Gerçek alt bölümler kendi ID'lerini göndermeye devam eder. İlişki doğrulaması ve transaction/audit kuralları korunur; toplu veri düzeltmesi yapılmaz.
- Sigorta güncellemesinde başarı mesajı belge adımlarından sonra gösterilir. Veritabanı güncellenip belge adımı hata verirse başarı popup'ı ile genel hata birlikte gösterilmez. Tek uyarı, kaydın güncellendiğini, belge işleminin tamamlanamadığını ve sonraki adımı başvuru koduyla açıklar. Teknik ayrıntı loga yazılır. Diğer belge çağrıları mevcut hata sınırını kullanır.

## Bildirilen TBYS hatasının durumu

`e8c7a730-84a7-40b2-a988-31ab92db3994` kaydının teknik özeti “Seçilen bağımsız bölüm bu taşınmaza ait değil.” şeklindedir. Stack trace, EnsureRelation → Update → UpdateWithPropertyStatus → UpdateSigorta → UpdateBtn_Click yolunu doğrular; iç hata yoktur. Hata veritabanı güncellemesinden önce ilişki doğrulamasında oluşur. Bu bulgu, kaynakta bulunan bölüm numarası/ID uyumsuzluğuyla uyumludur. Log seçilen ID ve taşınmazın alt bölüm durumunu içermediğinden, etkilenen kaydın alt bölümü olmadığı kullanıcı testiyle ayrıca doğrulanmalıdır. Gerçek alt bölüm kaydı taşınmaza ait değilse doğrulama reddi korunur.

Bu dal yayımlandıktan sonra LogIzlemeWP hata ayrıntıları teknik metinleri de gösterir. İşlem kodu ve tarih filtresiyle ilgili kayıt bulunabilir. LogDurumTakipWP de teknik özeti gösterebilir; ancak yalnız günün son 200 ortak log kaydını aldığı için eski kayıt görünmeyebilir. Alternatif olarak uygulamanın log bağlantısının kullandığı veritabanında şu salt okuma sorgusu çalıştırılabilir:

```sql
SELECT OccurredUtc, Project, WebPart, EventName, ErrorType,
       TechnicalSummary, StackTrace, InnerException
FROM dbo.PortalLog_Table
WHERE OperationId = 'e8c7a730-84a7-40b2-a988-31ab92db3994'
  AND Kind = 'Error'
ORDER BY OccurredUtc;
```

## Kullanıcı doğrulaması

1. Visual Studio/Test sunucusunda derleyin.
2. Log takip ekranında başarılı/başarısız SQL satırlarını kontrol edin: süre ve repository doğru sütunlarda olmalı; satır saatleri Türkiye gün filtresiyle uyumlu olmalı.
3. Alt bölümü olmayan ve bölüm numarası sayısal olan taşınmazın sigortasını güncelleyin. Bölüm ID'si 0, görüntülenen bölüm numarası mevcut metin olmalı. Ardından gerçek alt bölümü olan taşınmazı deneyin; seçilen bölümün ID'si korunmalı.
4. Belgesiz ve belge yükleyerek güncelleyin. Başarılı işlemde tek başarı mesajı; veritabanı commit'i sonrası kontrollü belge hatasında “Sigorta güncellendi; poliçe belgesi işlemi tamamlanamadı…” uyarısı ve başvuru kodu beklenir. Aynı işlem için ek hata popup'ı olmamalı.
5. Hatayı veren taşınmazın alt bölüm durumunu kontrol ederek aynı sigorta kaydını yeniden açıp güncelleyin. Alt bölümü olmayan kayıtta bölüm ID'si 0 gönderilmelidir. Gerçek alt bölümü bulunan kayıtta hata sürerse seçilen bölümün TasinmazId ilişkisini ayrıca inceleyin; ilişki doğrulamasını kaldırmayın.
6. LogIzleme ekranında Hatalar sekmesini açıp işlem koduyla arayın. Ayrıntıda üç teknik alan görünmeli; boş değerler “Kaydedilmemiş.” olmalı. Uzun stack trace kaydırılabilmeli ve kopyalanabilmeli. HTML içeren teknik metin düz metin görünmeli. Yetkisiz kullanıcı logları veya teknik alanları görememeli; Kullanıcı etkinlikleri sekmesinde teknik hata alanları bulunmamalı.

Codex Windows üzerinde build/test/debug çalıştırmadı. Üretilen ASCX dosyaları değiştirilmedi. Commit/push/PR/merge kullanıcıya aittir.

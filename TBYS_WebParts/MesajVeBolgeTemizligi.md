# TBYS mesaj, dosya hata sunumu ve eski bölge temizliği — parça 2

Tarih: 2026-10-25  
Dal: `codex/tbys-mesaj-ve-eski-bolge-temizligi`  
Başlangıç: `499bed1f2f3ee329b44862510cf3fd615f56f3e6`

IKYS parçasının tamamlandığı kullanıcı tarafından bildirildikten sonra başlandı. Değişiklik öncesinde yerel master temizdi ve GitHub üzerinden sorgulanan uzak master ile aynı commit'teydi.

## Mesaj çağrıları

38 TBYS kaynağındaki 251 etkin `MessageHelper.PublishMessage` çağrısı doğrudan `Messages.Success/Info/Error` kullanımına geçirildi. Mevcut metinler, türler ve süreler korundu. Kira bakiye devrindeki koşullu çağrı, iki dalda da 2000 ms süre korunarak Success/Info if/else akışına dönüştürüldü. Sekiz yorum satırı çağrısı kaldırıldı. Ortak MessageHelper uyumluluk sınıfı korunur.

## Bölge okuması

KiraSozlesmeListesiWP artık eski `PersonelinBolgesiniGetir_Deprecated` ile bağlamlı bölge yardımcısını art arda çağırmaz. `KullaniciBolgesiniYukle` personel, iş bilgileri, birim ve bölgeyi aynı TBYS log bağlamıyla birer kez okur. Entity özelliklerindeki bağlamsız DB okumaları bu akıştan çıkarıldı. AuthQS ve başlık için mevcut birim adında İstanbul/İzmir/Mersin arama sırası korunur; bölge tablosunun adıyla yeni bir eşleme yapılmadı. İş bilgileri yoksa listeyi doldurmaya devam edilmez; hata mevcut WebPart sınırına aktarılır.

Deprecated metodun tek tüketicisi bu ekrandı; metot Model/IKYS/IKYSOrtak.cs içinden kaldırıldı. Diğer bölge yardımcıları ve diğer projelerdeki tüketicileri sonraki parçalardadır. Query string/ViewState ve mevcut liste filtre sözleşmeleri bu dalda değiştirilmedi.

## Dosya yardımcıları

UtilityHelper içindeki UploadFileToSharePoint, DeleteFileFromSharePointLib ve GetFileNameListFromSharePointLib artık popup üretmez ve teknik hata yazmaz; istisnayı çağıran WebPart'a iletir. İmzaları ve log bağlamı zorunluluğu korunur. Kaynakta 10 yükleme, 9 silme ve 4 dosya listesi çağrısı vardır; hepsi TBYS'dedir.

- Yükleme/silme hatasını ilgili olay veya PDFKaydet yakalama bloğu, listeleme hatasını ilgili TabloJson yakalama bloğu mevcut TbysWebPartLog üzerinden raporlar. ThreadAbortException yükselmeye devam eder.
- KiraSozlesmesi, TasinmazBagisciGirisi, TasinmazResimleri, TasinmazSigortaGiris ve VasiyetciGirisi ekranlarındaki beş yükleme yakalama bloğu sadeleştirildi. Gerçek istisnanın yanına ikinci yapay “Dosya/PDF Yüklenemedi” istisnası eklenmez; ReportError gerçek hata için tek kayıt ve tek güvenli mesaj üretir.
- Gerçekten bulunmayan dosyayı silmek false döndürmeye devam eder. Teknik erişim/silme hatası false sayılmaz. Başarılı boş dosya listesi boş döner; erişim hatasında boş/kısmi liste dönmez.
- Çoklu yüklemede bir dosya hatası kalan yüklemeleri o yordamın yakalama noktasına kadar durdurur. Daha önce tamamlanan SharePoint dosya işlemleri geri alınmaz; işlem SQL transaction'ı değildir.
- Sigorta güncellemesindeki throwOnError ve committed ayrımı korunur. Kayıt tamamlanıp belge işlemi başarısız olursa mevcut “Sigorta güncellendi; poliçe belgesi işlemi tamamlanamadı” uyarısı kullanılır.
- Yükleme ve dosya listelemede oluşturulan SPSite/SPWeb nesneleri using ile kapatılır. Dosya yolu, overwrite, alt site ve elevated listeleme davranışı korunur.

## Statik inceleme ve sınırlar

Değişen toplam 41 C# kaynağı yalnızca sözdizimi bakımından Roslyn ile incelendi: 0 hata. TBYS WebPart kaynaklarında MessageHelper referansı ve uygulama kaynaklarında deprecated bölge metodu kalmadı. UTF-8/BOM korunumu ve git diff --check kontrol edildi. Hiçbir ASCX veya üretilen .ascx.g.cs değiştirilmedi; kod üretimi gerekmez.

SQL yazıcıları, iş transaction'ları ve audit kodu değiştirilmedi. Build, test, debug ve semantic compilation çalıştırılmadı.

## Yayımlama ve kullanıcı kabulü

Visual Studio 2026'da Model ve TBYS_WebParts projelerini birlikte derleyip uyumlu assembly/paket sürümlerini Test ortamına yayımlayın. Deprecated public metot kaldırıldığı için eski TBYS assembly'si ile yeni Model sürümünü karıştırmayın.

1. İstanbul, İzmir, Mersin ve diğer birimlerde kira sözleşme listesi başlığını, satır kapsamını ve düzenleme görünürlüğünü önceki sürümle karşılaştırın. İlk açılıştaki personel/iş bilgileri/birim/bölge okumalarının SQL izini kontrol edin. Eksik iş bilgileri veya kontrollü DB hatasında liste sorgusuna devam edilmemelidir.
2. Temsilî taşınmaz, kiracı, ödeme planı, vasiyetçi ve teminat işlemlerinde mesaj metni/türü/süresi, Türkçe karakterler ve çoklu mesaj sırasını tam postback/UpdatePanel ile kontrol edin. Bakiye devrinin değişiklik olan/olmayan dallarında Success/Info ve 2000 ms süre beklenir.
3. Beş yükleme ekranında normal PDF yükleme ve kontrollü SharePoint yetki/kütüphane hatasını deneyin. Hata için tek teknik kayıt ve başvuru kodlu güvenli mesaj beklenir; ham istisna veya ek yapay hata olmamalıdır. Taşınmaz belgelerinde çoklu yüklemenin ilk/ara dosyada hata durumunu ayrıca değerlendirin.
4. Sözleşme, bağışçı, taşınmaz belgeleri ve sigorta poliçesi silmede mevcut dosya, olmayan dosya ve gerçek erişim hatasını ayrı ayrı deneyin. Başarısız silme sonrasında başarı mesajı üretilmemelidir.
5. Bağışçı, taahhüt ve sigorta listelerinin dosya bağlantılarını kontrol edin. Dosya kütüphanesi erişim hatası, belgeler yokmuş gibi kısmi listeyle sürdürülmemelidir.
6. Sigorta güncellemesi tamamlandıktan sonra belge yükleme hatası oluşturun: iş kaydı korunmalı ve mevcut kısmi tamamlanma uyarısı görülmelidir. Aynı kaydı tekrar güncellemeden önce belge durumunu kontrol edin.
7. Temsilî TBYS yazma/rollback akışında audit bütünlüğünü doğrulayın.

Commit, push, PR ve birleştirme kullanıcı tarafından yapılır. Sonraki NBYS parçasına Test sonucu veya kullanıcının açık devam talebi sonrasında geçilir.

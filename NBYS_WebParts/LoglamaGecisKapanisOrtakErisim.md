# NBYS ortak erişim ve kapanış incelemesi

Başlangıç: temiz yerel master ve uzak master aynı commit: `04c2bec7b87e5479089cc6a49768086a77c7bff6` (#521).
Dal: `codex/nbys-kapanis-ortak-erisim-audit`.

## Kapsam ve mesaj değişiklikleri

- ArmaganListesiWP, BagisIadesiWP, DuzenliBagisciBelgesiBasimiWP, DuzenliBagisciDosyaYuklemeWP, DuzenliNakitBagisciListesiWP, EkstreListesiWP, FTKIslemleriWP, FTKKisiGirisiWP ve NakitBagisHareketSilmeWP yönerge parametresini mevcut dört parametreli yardımcıyla kendi LoggingContext'i üzerinden okur. Bağlantı seçme kuralı ve bu ekranların işlem mesajları değişmez.
- EkstreListesiWP içinde kullanılan, ASCX ekran sayımına girmeyen NakitBagisciKarti kontrolünün bağışçı, hareket, armağan, düzenli bağışçı, banka, armağan tanımı, il ve ilçe okumaları merkezî bağlamlıdır. Kart yükleme isteği etkinlik kaydı üretir; yalnızca sayfada bulunması ayrıca kart açılışı sayılmaz.
- Kart, kullanıcı bölgesini sunucudan okur ve mevcut NbysBagisciWriteService.CanReadDonor kuralını kişisel bilgileri okumadan önce uygular. Genel Müdürlük/tüm bölgeler yetkisi korunur; bölge kullanıcısı kendi bölgesini görür. Eksik yetki veya bulunamayan kayıt için tek uyarı gösterilir. Bu, eski kartın eksik yetki kontrolünü kapatan davranış değişikliğidir.
- Geçersiz seçim ve bulunamayan/yetkisiz kayıt kart içinde sarı uyarıdır. Teknik hata tek kırmızı kart mesajıdır: tekrar deneme adımı ve başvuru kodu içerir. Exception ayrıntısı PortalTechnicalLog'a gider; ayrıca Messages.Error çağrılmadığı için ikinci popup oluşmaz. Sonraki kart yüklemesi eski içeriği yeniden hesaplar.
- DuzenliNakitBagisciService/Repository bağlamlı salt okuma yolu eklendi. Bağlamlı repository üzerinden transaction'sız yazma reddedilir; mevcut düzenli bağışçı yazma servisinin transaction/audit yolu kullanılır. Bu parçada yeni iş yazması/transaction eklenmedi.
- NbysEkstreFeedback, kontrollü NbysImportValidationException sonuçlarını artık FileValidation/Error olarak ayrıca kaydetmez. Güvenli doğrulama metni ve geri alma açıklaması tek Warning mesajında korunur. Teknik hata ve yenileme hatası mevcut ReportError yolunda kalır. Alt SQL katmanının başarısız sorgu izleri korunur; beklenen biçim hatasının exception ayrıntısı ayrıca Error kaydı üretmez. Başarı koşulu ve transaction davranışı değiştirilmedi.

## Kaynak incelemesinin sınırı

55 Compile edilen ASCX code-behind'ın bağlam içermesi geçişin çalışma zamanı kanıtı değildir. Yeniden kullanılan kartın bu sayım dışında kaldığı bu incelemede görüldü.

Kök dizindeki BagisaVesileOlanTesekkurGirisiWP.ascx.cs eski kopyası csproj Compile listesinde değildir; çalışan sürüm aynı adlı alt klasördedir. Eski kopya silinmedi/değiştirilmedi. Banka yükleme ve ekstre aktarmada kalan ExceptionHelper kullanımları sonuç taşıyıcılarıdır; tek başına eski popup yolu anlamına gelmez. DovizleYapilanBagislarWP'nin varsayılan servis nesnesiyle çağırdığı rapor metodu açık LoggingContext alır; varsayılan constructor tek başına bağlamsız sorgu kanıtı sayılmadı.

İncelenen NakitBagisHareketService/ArmaganService transaction overload'ları ve ekstre aktarımının nullable transaction dallarında transaction verilmiş yolun transaction overload'larını kullanması kontrol edildi. Bu çalışma bütün solution için otomatik veya eksiksiz çağrı grafiği doğrulaması değildir. Eski bağlamsız API'ler başka tüketiciler nedeniyle korunur.

## Kullanıcı tarafından inceleme ve TEST

1. Ekstre listesinde kartı merkez ve bölge kullanıcısıyla açın: yetkili kayıtta bağış, iade, armağan, düzenli bağışçı ve konum bilgilerini önceki verilerle karşılaştırın. Bağışsız/armağansız ve düzenli olmayan bağışçıyı da deneyin.
2. Geçersiz/silinmiş ve başka bölgeye ait bağışçı seçiminde kişisel bilgi gösterilmemeli; tek sarı uyarı olmalı. Sunucu tarafındaki kimlik kontrolü gizli alan değişikliğinde de uygulanmalı. Yetki kaydı olmayan kullanıcı kartı açamamalı.
3. Kontrollü teknik hata halinde yalnızca kartta güvenli hata ve başvuru kodu görülmeli; aynı OperationId ile teknik log bulunmalı. Sonraki başarılı açılış eski hata içeriğini değiştirmeli. Kartın asenkron postback ile açıldığını da doğrulayın.
4. Dokuz ekranda yönerge bağlantısını ve parametre SELECT izlerini kontrol edin. Proje genelinde NBYS henüz etkin değilse mevcut ekran bayraklarına ek olarak PortalLogging.EnabledWebParts listesine `NBYS:NakitBagisciKarti` ekleyin (noktalı virgülle ayırın). SQL izlerinin görünmesi ilgili ayarların açık olmasına bağlıdır.
5. Ekstre dosyası biçim/mükerrer kayıt doğrulamasında anlamlı tek Warning ve geri alma açıklaması kalmalı; ayrıca FileValidation/Error oluşmamalı. Gerçek bağlantı/SQL hatasında teknik Error ve güvenli kullanıcı mesajı devam etmeli. Başarısız dosya için başarı gösterilmemeli.

Codex build, test veya debug çalıştırmadı; DB/SharePoint iş isteği yapmadı. UTF-8/BOM, diff ve proje kaynak referansları statik kontrol edildi. Üretilen ASCX dosyaları değiştirilmedi. Commit/push/PR/merge kullanıcıya aittir. NBYS'nin son kapanışı test ortamındaki değerlendirmeye bağlıdır; Portal/MTS/MFYS geçişi veya OpenXML sürüm yükseltmesi bu kapsamda değildir.

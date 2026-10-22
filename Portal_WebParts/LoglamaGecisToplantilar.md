# Portal toplantı akışları — log, transaction ve mesaj düzenlemesi

Başlangıç: temiz yerel master ve uzak master `57edfb52096cee93e6be21b8cfdef22f491fd245`.
Dal: `codex/portal-toplanti-log-transaction`.

## Kapsam ve inceleme noktaları

Bu parça beş ekranı birlikte kapsar: `ToplantiGirisiWP`, `ToplantiListesiWP`, `ToplantiViewerWP`, `ToplantiKatilimTutanagiWP`, `ToplantiParametreGirisiWP`.

- Toplantı ekleme/güncelleme/silme, katılımcı değişiklikleri ve audit aynı transaction'dadır. Silmede önce katılımcılar, sonra toplantı silinir. Katılımcı silinmesinin toplantı kaydından veya SMTP'den sonra ayrı bağlantıda yapılması kaldırıldı.
- Yetki, toplantı yeri, katılımcı varlığı, rezervasyon çakışması, aynı adlı parametre ve kullanılan yer kontrolleri transaction içinde yapılır. Bütün yeni toplantı/parametre yazmaları aynı transaction sahipli uygulama kilidini kullanır. Bu sırada SMTP veya Active Directory çağrısı yapılmaz.
- Mevcut salon kontrolünün sınır davranışı korunur: bitiş ile başlangıcın aynı ana denk gelmesi de çakışmadır. Yeni kontrol bir toplantının diğerini tamamen kapsamasını da yakalar. “Diğer” yer için salon çakışması aranmaz; açıklama zorunludur.
- Kimlik sunucunun oturumundan alınır. Yönetici veya yetkili kişinin kendi/birim toplantısı için yazma izni transaction içinde doğrulanır. Güncelleme toplantının asıl yetkilisini ve koordinatörünü değiştirmez. Parametre ekranının erişimi mevcut SharePoint sayfa izinlerine bağlıdır; yeni bir rol sistemi eklenmedi.
- Paylaşılan statik eski toplantı nesnesi kaldırıldı. Form için toplantı/katılımcı özeti birlikte okunur; imzalı ViewState içindeki sürüm özeti güncelleme/silmede karşılaştırılır. Eski form başka bir işlemin değişikliklerini ezemez. Değişiklik olmayan güncelleme yazma veya e-posta üretmez.
- Parametre kayıtları da audit kapsamındadır. Kullanılan toplantı yeri ve sistemin “Diğer” kaydı silinemez. “Diğer” kaydının adı değiştirilemez. Aynı grupta aynı adın eşzamanlı eklenmesi engellenir.
- SQL trace ve audit bağlamı `Portal` ve ilgili WebPart'tır. Audit alanları sınırlıdır: tarih, yer kimliği, çevrim içi/ikram durumu, toplantı ilişkisi, bilgi durumu, parametre grubu/sırası. Konu, açıklama, isimler, katılımcı kimlikleri ve adresler alan değişikliklerine yazılmaz; değişen kaydın audit olayı yine oluşur.
- Duyuru için oluşturulmuş SQL yürütücüsü `PortalSqlExecutor` adıyla ortaklaştırıldı; duyuru davranışı değiştirilmedi. Personel/ortak parametre okumalarına açık log bağlamlı overload eklendi. Eski çağıranlar kendi yollarını korur. Bu ekranların bildirim yolu artık popup üreten/hata yutan `MTSOrtak` ve `MailHelper` yollarını çağırmaz.

## E-posta ve takvim

Commit tamamlandıktan sonra ayrı bildirim servisi çalışır. İptal bildiriminde silinmiş satırlar yeniden sorgulanmaz; transaction'da alınmış önceki katılımcı listesi kullanılır. Katılımcı ekleme/çıkarma ve bilgi↔katılımcı geçişleri REQUEST/CANCEL takvim eki üretir. İletiler Türkçedir; gövdedeki serbest metin HTML olarak çalıştırılmaz. SMTP nesneleri kapatılır; hatalar yutulmaz, aynı OperationId ile teknik loga gider. Alıcı takibi istekler arasında paylaşılan statik liste değildir.

Toplantı yetkilisi, toplantı grubu, gerektiğinde bilgi sistem ve ikram grupları korunur. Grup çözümlemesi uygulama havuzu kimliğiyle ve doğrudan üyeler üzerinden yapılır. E-posta/takvim gönderimi veritabanı transaction'ının parçası değildir; bu parça kalıcı bildirim kuyruğu veya otomatik yeniden gönderim eklemez. Testte gerçek alıcılara gitmeyecek uygun SMTP/alıcı yapılandırması kullanılmalıdır.

## Mesaj değişiklikleri

- Eksik konu/yer, geçersiz tarih/saat, ters tarih, bulunamayan kayıt, yetki, çakışma ve eski form anlamlı tek Türkçe uyarıdır. İş kuralı uyarısı teknik hata diye loglanmaz.
- Başarı yalnız commit sonrası gösterilir. Bildirimlerin tamamı gönderilmeden “e-posta gönderildi” denmez. Bir kısmı başarısızsa toplantının kaydedildiği/silindiği açıkça belirtilir; tekrar kaydetmeme adımı ve başvuru kodu verilir.
- Başarılı işlemden sonra otomatik yönlendirme yerine sonuç ekranda kalır; işlem düğmeleri kapanır. Kullanıcı “Toplantı Listesi” ile devam eder. URL'deki `Mesaj` değeri başarı bildirimi oluşturmaz.
- Teknik DB/commit hatasında güvenli hata ve başvuru kodu gösterilir; tekrar yazmadan önce listenin kontrol edilmesi istenir. Commit sonrası parametre listesi yenileme hatası, kayıt başarısızmış gibi sunulmaz.
- Liste/takvim/modal/tutanak hataları tek merkezi hata sınırından geçer. Page_Load başarısızsa aynı isteğin sonraki işlem olayı çalışmaz. Ham exception, SQL veya stack trace kullanıcıya verilmez.
- Tutanak yalnız gerçek katılımcıları içerir; bilgi alıcılarına imza satırı açılmaz. Dosya içeriği indirme yanıtına geçmeden hazırlanır. Excel düğmesi tam postback kullanır; Türkçe içerik UTF-8 olarak sunulur. Mevcut HTML tabanlı `.xls` biçimi korunur.

## Yapılandırma

Mevcut `PortalLogging.EnabledWebParts` değerlerine şunları ekleyin:

`Portal:ToplantiGirisiWP;Portal:ToplantiListesiWP;Portal:ToplantiViewerWP;Portal:ToplantiKatilimTutanagiWP;Portal:ToplantiParametreGirisiWP`

Portal'ın tüm ekranları henüz taşınmadığı için `EnabledProjects` içine bu parça adına Portal eklenmez. Yeni DB şeması veya paket güncellemesi yoktur. Uygulama hesabının `sp_getapplock` ve mevcut merkezi audit tabloları erişimi test ortamında doğrulanmalıdır.

## Kullanıcı tarafından derleme ve TEST

1. Beş ekranı açın; liste, takvim detayı ve tutanak içeriklerini kontrol edin. Açılış Activity ve etkinse SQL trace aynı WebPart bağlamını taşımalı.
2. Yeni toplantıya iki katılımcı ve bir bilgi alıcısı ekleyin. Tek başarı, toplantı/katılımcı/audit bütünlüğü, e-posta ve Outlook takvim davetlerini kontrol edin. Hiç katılımcısı olmayan toplantıyı da deneyin.
3. Konu/tarih/yer değiştirin; katılımcı çıkarın/ekleyin, bilgi↔katılımcı geçişlerini deneyin. Takvim güncelleme/iptali ve gerçek katılımcılardan oluşan tutanağı doğrulayın. Değişiklik yapmadan güncellemede yalnız bilgi mesajı beklenir.
4. Toplantıyı silin: toplantı ve tüm katılımcılar birlikte silinmeli, iptal e-postaları eski katılımcılara gitmeli; tek sonuç mesajı görülmeli.
5. Kontrollü katılımcı/audit yazma hatasında toplantı dahil tüm yazmalar geri alınmalı, başarı ve SMTP gönderimi olmamalı. Commit sonucu belirsiz bağlantı hatasında tekrar kayıt yerine liste kontrolü istenmeli.
6. SMTP kapalı veya bir alıcının adresi eksikken kaydedin: DB kaydı korunmalı, tek kısmi sonuç/başvuru kodu görülmeli. Sonraki işlem başka kullanıcıya ait alıcı listesinden etkilenmemeli. İkram/çevrim içi seçimini kaldırınca ilgili grupların güncelleme almasını kontrol edin.
7. İki sekmede aynı salon ve çakışan saatlerle kayıt deneyin; yalnız biri tamamlanmalı. Aynı toplantıyı iki sekmede açıp ilkinden değiştirin, eskisinden güncelleme/silme deneyin: eski form reddedilmeli. Yetkisiz kullanıcıyla doğrudan yazma isteği de reddedilmeli.
8. Parametre ekleme/güncelleme/silme; boş ad, aynı grup/ad, kullanılan yer ve “Diğer” korumasını deneyin. İki sekmeden aynı adı kaydetmede tek kayıt kalmalı. Başarı yalnız commit sonrası görülmeli.
9. Geçersiz tarih, eksik kayıt ve kontrollü okuma hatasında tek mesaj beklenir. Liste tarih aralığının son günündeki toplantıları, takvim detayını ve Türkçe Excel çıktısını kontrol edin.

Windows üzerinde build/test/debug çalıştırılmadı. Statik inceleme: kaynak sözdizimi ayraç dengesi, UTF-8/BOM, üç csproj Compile kayıtları, beş ekranın ASCX olay bağlantıları, eski mesaj/hata yolları taraması ve `git diff --check`. Bu kontroller derleme veya çalışma zamanı testi yerine geçmez. `*.ascx.g.cs` dosyaları değiştirilmedi. Commit/push/PR/merge kullanıcıya aittir.

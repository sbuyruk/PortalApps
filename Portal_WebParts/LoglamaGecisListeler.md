# Portal liste ekranları — log ve mesaj geçişi

Başlangıçta yerel master temizdi; uzak master ile aynı commit: `11fdc0ac45dd8804b2c03b1a079392a962cf118e`.
Dal: `codex/portal-liste-log-kontrol`.

## Kapsam ve mesaj değişiklikleri

| WebPart | Değişiklik ve inceleme noktası |
| --- | --- |
| DavaDurusmaListesiWP | Açılış kaydı ve tek hata sınırı eklendi. SharePoint okuma hatası artık boş veya kısmi sonuç gibi sunulmaz. Geçersiz durum seçimi “Geçerli bir dava durumu seçin.” uyarısıdır. Teknik hata güvenli mesaj ve başvuru koduyla gösterilir. CAML durum filtresindeki fazladan boşluk kaldırıldı. Liste metinleri HTML olarak çalıştırılmaz. |
| OlayListesiWP | İlk açılıştaki iki sorgulama kaldırıldı. Olay, faaliyet, kişi ve anı objesi okumaları açık Portal log bağlamını kullanır. FaaliyetId sonrasındaki KatilimciId artık atlanmaz; bozuk veya boş açıklama alanı güvenle ayrıştırılır. Eksik kişi/faaliyet için satır içinde Türkçe açıklama gösterilir; satır başına popup açılmaz. Anı objesi sorgusunun döndürdüğü Adi alanı okunur. Teknik hata tek güvenli mesaj ve başvuru kodudur. Açıklamadaki dinamik metinler HTML kodlanır. |
| LogDurumTakipWP | Mevcut ManageWeb yetki denetimi korunur. Yetkili açılış kaydedilir. Yükleme hatası teknik loga yazılır; ekran içinde “Log kayıtları yüklenemedi. Lütfen yeniden deneyin.” ve başvuru kodu gösterilir. Ek popup açılmaz. |

Dava/olay listesi yenilenemezse eski tablo gizlenir; eski satırlar yeni sorgunun sonucu gibi gösterilmez. Başarılı yüklemede tablo tekrar gösterilir. Yeni başarı popup'ı eklenmedi. Olay listesindeki mevcut ham iş olayı açıklaması sütunu korunur ve metin olarak çizilir; yeni teknik istisna metni bu sütuna aktarılmaz.

## Veri erişimi ve sınırlar

Üç ekran salt okuma/gösterim yapar. İş verisi yazma, yeni transaction veya audit işlemi eklenmedi. MTS repository/service değişiklikleri yalnız Portal olay ekranının okuma bağımlılıklarıdır; MTS projesinin genel geçişi değildir. Eski overload'lar korunur. Açık bağlamlı okumalar PortalSqlReadExecutor üzerinden yürür; SQL trace sorgu metni/parametre değerleri taşımaz.

Log takip ekranının mevcut merkezi log sorguları yeniden SQL trace üretmez; okuyucu hatası dış sınıra ulaşır. SharePoint okumaları da SQL trace değildir. Yeni paket, DB migration, sunucu kontrolü veya proje Compile kaydı gerekmez. Üretilen ASCX dosyaları değiştirilmedi.

## Yapılandırma ve kullanıcı testi

Mevcut `PortalLogging.EnabledWebParts` değerlerine ekleyin:

`Portal:DavaDurusmaListesiWP;Portal:OlayListesiWP;Portal:LogDurumTakipWP`

Portal genel taraması ve testleri tamamlanmadan tüm projeyi EnabledProjects ile açmayın.

1. Visual Studio/Test sunucusunda derleyin; üç ekranı açın ve açılış kayıtlarının doğru WebPart/OperationId ile oluştuğunu kontrol edin.
2. Dava listesinin üç durum filtresini, dolu/boş sonuçları, sıralama ve dışa aktarmayı deneyin. Özel karakterler metin olarak görünmeli.
3. Olay listesini Program filtresi bulunan/bulunmayan adreslerle açın. İlk açılışta olay ana sorgusu bir kez çalışmalı. Postback sonrası çift satır veya DataTables yeniden başlatma hatası olmamalı.
4. Açıklamada FaaliyetId önce, KatilimciId sonra olan kayıtta kişi ve anı objeleri görünmeli. Boş/bozuk açıklama ve silinmiş kişi/faaliyet kaydında gereksiz popup olmamalı. Anı objesi adı ve adedi doğru olmalı.
5. Kontrollü SQL/SharePoint erişim hatasında tek güvenli hata ve başvuru kodu bekleyin. Eski tablo görünmemeli; hata giderilip yeniden yüklenince sonuçlar görünmeli. Teknik ayrıntıyı kullanıcı popup'ında değil, ilgili OperationId kaydında kontrol edin.
6. Log takip ekranını yetkili/yetkisiz kullanıcıyla açın. Yetkisiz kullanıcı veriye ulaşmamalı. Kontrollü yükleme hatasında tek ekran içi mesaj ve başvuru kodu görünmeli.

Codex Windows üzerinde build/test/debug çalıştırmaz. Statik kaynak incelemesi derleme ve çalışma zamanı doğrulaması yerine geçmez. Commit, push, PR ve merge kullanıcıya aittir. Bu parçadan sonra Portal genel kapanış taraması ve test değerlendirmesi yapılmalıdır.

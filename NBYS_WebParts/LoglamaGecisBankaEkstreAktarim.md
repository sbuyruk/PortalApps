# NBYS banka yükleme, ekstre ve bağışa aktarma geçişi

Dal: `codex/nbys-banka-ekstre-aktarim-log-mesaj`.
Başlangıç: `0bf73c6150fa1fb1d05830ebe28e063c85445780` (22 Ekim 2026).
Kod değişikliği öncesinde master çalışma ağacı temizdi; GitHub master ile aynı commit olduğu `git ls-remote` ile doğrulandı. Kullanıcı bu geniş parçaya devam edilmesini istedi; önceki parçanın TEST sonucu varsayılmadı.

## Kapsam ve işlem sınırları

- **BankaDosyasiYuklemeWP:** ortak import servisindeki 24 girişin açık log bağlamı, dosya başına sonuç ve toplu bildirim; yükleme kontrolleri ve takvim okumaları. Ekrandaki 21 aktif banka girişinin tamamı bağlandı; diğer girişlerde bağlam varsayılan olarak boş bırakılarak mevcut çağıranlar korunur.
- **EkstreListesiWP:** açılış, banka/durum/listeler, dışa aktarma, seçim, onay, toplu bağışa aktarma ve silme.
- **EkstreAktarmaEditWP:** açılış, yeni manuel kayıt, düzenleme, döviz alanları ve bağışçı eşleştirme.
- **NakitBagisciEslestirWP:** mevcut bağışçı arama/detay/seçim okumaları ve hata sınırları. Bu ekran doğrudan kayıt yazmaz; seçilen bağışçı editör onayına gider. Düzenli bağışçıya dönen mevcut seçim bağlantısı korunur; düzenli bağışçı yazma akışı bu parçada taşınmış sayılmaz.

Import **dosya başına** tek transaction kullanmayı sürdürür. Mevcut uygulama kilidi ve fiş/mükerrerlik kontrolleri korunur. Dosyada hata varsa o dosyanın iş/audit değişiklikleri geri alınır, başarı sayısı sıfırlanır. Aynı istekte başka bankanın başarıyla tamamlanan dosyası geri alınmaz.

Bağışa aktarma **ekstre satırı başına** tek transaction'dır. Bağışçı oluşturma/güncelleme, bağış, armağan, ekstre işareti ve audit aynı transaction içindedir. Satır hatası o satırı geri alır; diğer satırların tamamlanmış işlemleri korunur. Sonuçta başarı, önceden aktarılmış, bulunamayan ve hata sayıları ayrı bildirilir. Aynı seçimde yinelenen kimlikler tek kez işlenir. Başarı sayacı ancak transaction Dispose/commit bittikten sonra artırılır.

Manuel ekleme/düzenleme/eşleştirme kendi transaction'ını kullanır. Güncellemede güncel ekstre kilitlenerek yeniden okunur, yalnızca izin verilen form alanları taşınır; AktarildiMi, bağış bağlantısı ve import kimliği formdan ezilmez. Aktarılmış kayıt veya ters bağış bağlantısı bulunan kayıt düzenlenemez/eşleştirilemez. Eksik kayıt yeni kayıt gibi eklenmez. Seçilen bağışçının varlığı aynı transaction'da doğrulanır.

Toplu ekstre silme **seçim grubunun tamamı için** tek transaction'dır. Önce tüm seçim kimlik sırasıyla kilitlenir ve doğrulanır. Aktarılmış/bağış bağlantılı/eksik tek kayıt varsa hiçbir kayıt silinmez. Her silinen ekstre için mevcut SilinenKayit_Table arşivi ve merkezi DELETE audit aynı transaction'da yazılır. Arşiv/audit hatasında grubun tamamı geri alınır.

Armağan hak kazanma algoritması yeniden tasarlanmadı. Önceki tekli/çoklu bağış, önceki armağan, iade, bilinmeyen bağışçı ve e-Devlet belge durumu dalları korunur. Bağışçı ve armağan transaction okumalarındaki kilitler, aynı bağışçıya paralel aktarımın eski veriden armağan üretmesini engellemek üzere güçlendirildi. Kilit bekleme/deadlock davranışı TEST'te ayrıca incelenmelidir.

## Log ve audit

Açık `PortalLogContext` WebPart → import/aktarma → servis/repository zincirinde taşınır. Transaction alan yardımcıların okumaları/yazmaları ve audit aynı verilen transaction üzerinden yürür; işlem içinde ayrı iş bağlantısı açılmaz. Nullable legacy yollarda yalnızca transaction yokken eski erişim kullanılabilir. Parametresiz diğer ekranların davranışı korunur.

`NbysTransactionalWriter`, yalnızca izin verilen dört tabloda gerçek DB öncesi/sonrası görüntüleriyle audit yazar. Context verilen yazmalar transaction gerektirir. Eski OlayKayit yazıcısı bu yolda çağrılmaz; log ayarı kapalıyken de eski yazıcıya dönüş olmaz. Bilinmeyen bağışçı oluşturmanın INSERT ve kimlikle adını tamamlama UPDATE işlemleri aynı transaction'dadır.

| Entity | Merkezi audit alanları |
| --- | --- |
| EkstreAktarma | NakitBagisciId, NakitBagisHareketId, AktarildiMi, BagisTarihi, Tutar, DovizCinsi, DovizTutari, DovizKuru, KurTarihi, ElleKayit, TuzelKisi, BelgeIstemiyor |
| NakitBagisci | Ili, Ilcesi, TuzelKisi, Sag, Ulasilamiyor, BelgeIstemiyor, DergiGonderilmesin |
| NakitBagisHareket | BagisciId, ArmaganId, EkstreAktarmaId, BankaId, BagisTarihi, BagisMiktari, DovizCinsi, DovizTutari, DovizKuru, KurTarihi |
| Armagan | BagisciId, ArmaganTanimId, Tarih, Durum, BagisMiktari, DovizCinsi, CokluBagis, KacinciBelge |
| SilinenKayit | Önceki arşiv yazıcısındaki izin listesi: yalnızca TabloAdi |

Ad, T.C. kimlik numarası, telefon, adres, açıklama, fiş numarası, dosya yolu ve arşiv içeriği merkezi audit alan ayrıntılarına alınmaz. Mevcut iş tablosu/arşiv alanları korunur. SQL izleri sorgu/parametre değerlerini içermez; işlem türü, repository/metot, süre ve komut sonucu yazılır. SQL Succeeded, üst transaction'ın commit edildiği anlamına gelmez.

Teknik hata kayıtları bağımsız ortak teknik yazıcıya gider ve rollback sonrasında kalabilir. Import satırının teknik ayrıntıları ve inner exception zinciri yalnızca teknik logdadır. Dosya biçimi/mükerrer anahtar doğrulamasının tanı ayrıntıları FileValidation işlemiyle teknik kayda yazılır; kullanıcıya güvenli Warning gösterilir. Önceden işlenmiş fiş uyarıları teknik hata olarak yayımlanmaz.

## Mesaj değişiklikleri ve inceleme noktaları

TBYS `TbysWebPartLog.ReportErrors` içindeki çoklu teknik hatayı tek kullanıcı bildirimiyle sunma ve başvuru kodu yaklaşımı kaynakta kontrol edildi. Önceki NBYS parçalarında benimsenen ortak `Messages` ve `NbysWebPartLog` kullanımı bu dört ekrana genişletildi.

- Yüklemede satır başına hata popup'ı ve ayrıca genel hata/başarı popup'ı kaldırıldı. İstek sonunda tek toplu bildirim; eklenen/önceden işlenen/atlanan sayıları ve tamamlanamayan banka dosyaları gösterilir.
- Dosya biçimi, geçersiz tarih/tutar ve mükerrer kayıt uyarıları teknik arızadan ayrılır. Kullanıcıya dosya şablonunu/fiş bilgisini kontrol etmesi söylenir. Ham exception.Message, SQL veya stack trace kullanıcıya gösterilmez.
- Kısmi bağışa aktarmada tamamlanan kayıt sayısı korunur; hata olan satır için başarı iddia edilmez. Teknik hatalar tek AggregateException kaydı ve tek güvenli mesaj/başvuru kodu üretir.
- Ekstre düzenlemedeki bozuk Türkçe ve başarısız işlemden sonra tekrarlanan “bağışçı adı boş” mesajı kaldırıldı. Ad, tarih, sayısal tutar, kimlik ve döviz alanlarının biçim uyarıları ayrı ve Türkçedir. Yeni pozitif tutar/hak kazanma kuralı eklenmedi.
- Başarı yalnızca commit sonrası gösterilir. Commit sonrası liste/form yenilemesi hata verirse sonuç ile “Liste yenilenemedi. Sayfayı yenileyin.” tek bildirimde verilir; işlemi yeniden yapmaya yönlendiren ikinci başarı/hata mesajı üretilmez.
- URL'deki Mesaj/Islem parametrelerinden sonuç türeten yol kaldırıldı. `?Mesaj=true&Islem=kaydet` işlemsiz başarı üretmez. Başarı sonrası aynı ekranda kalınır; editörden ekstre listesine mevcut düğmeyle dönülebilir.
- Boş/geçersiz seçim, bulunamayan bağışçı/ekstre ve başka oturumda aktarılmış ekstre anlamlı Warning sonuçlarıdır. Silme/aktarma onayında boş seçim artık modal açmaz. İşlem sonrasında seçim temizlenir.
- Olay sınırlarındaki teknik hatalar ortak NbysWebPartLog üzerinden bir kez raporlanır; yönlendirme/dışa aktarmanın ThreadAbortException sonucu teknik hata sayılmaz. Liste JSON hazırlığındaki hatayı yutup boş veriyle devam etme kaldırıldı.

## Yapılandırma

Mevcut IKYS/TBYS/NBYS ayarlarını koruyarak `PortalLogging.EnabledWebParts` listesine ekleyin:

`NBYS:BankaDosyasiYuklemeWP;NBYS:EkstreListesiWP;NBYS:EkstreAktarmaEditWP;NBYS:NakitBagisciEslestirWP`

NBYS projesinin tamamını EnabledProjects ile açmayın. Yeni migration yoktur; mevcut merkezi log/audit tabloları ve izinleri hedefte bulunmalıdır. WSP yayını migration çalıştırmaz. DAO/Model/Utility ve NBYS paketinin güncel çıktıları birlikte kullanılmalıdır.

## Kullanıcının Visual Studio 2026 / TEST kontrolü

1. Solution ve NBYS WSP'yi kullanılan yapılandırmada derleyip TEST'te yayımlayın. Dört ekranın ilk açılışında tek UserActivity; postback'te yeni açılış olmaması, SELECT izleri ve ortak OperationId beklenir.
2. Kullandığınız her banka şablonuyla geçerli dosya yükleyin. Ekstre ve INSERT audit birlikte oluşmalı; tek bildirimde doğru kayıt sayısı görünmeli. Aynı dosya/fişin tekrarında mükerrer oluşmamalı; uyarı ve uygun sayaç beklenir.
3. Bir dosyanın geç satırında biçim/audit/DB hatası oluşturun. O dosyanın önceki satırları ve audit'i geri alınmalı, başarı sayısı sıfır olmalı. Aynı istekte başka bankanın başarılı dosyası korunmalı ve toplu sonuç bunu doğru göstermeli.
4. Manuel ekleme/düzenleme/eşleştirme yapın. Kaydedilen alanlar ile audit izin listelerini; eksik ad, bozuk tarih/sayı/döviz alanında tek uyarı ve sıfır yazmayı kontrol edin. Ad/telefon/açıklama audit ayrıntısında bulunmamalı.
5. Düzenleme modalı/sayfası açıkken başka oturumdan ekstreyi aktarın veya silin. İlk oturumun kaydet/eşleştir işlemi mevcut kaydı ezmemeli veya yerine yeni kayıt eklememeli; uyarı beklenir.
6. Birkaç ekstreyi tek onayla bağışa aktarın. Her satırın bağışçı/bağış/armağan/ekstre/audit işlemleri birlikte commit olmalı. Yeni/mevcut/bilinmeyen bağışçı, tekli/çoklu armağan, armağan gerekmeyen ve önceden armağan verilmiş örnekler önceki iş davranışıyla karşılaştırılmalı.
7. İkinci satırın armağan/bağlantı/audit yazmasını kontrollü başarısız kılın. O satır tamamen geri alınmalı, diğer tamamlanan satır korunmalı. Tek bildirimde başarı ve hata sayıları doğru olmalı. Tekrar denemede tamamlanan satır yeniden bağış üretmemeli.
8. Aynı ekstreyi ve aynı bağışçıya ait farklı ekstreleri iki oturumda aktarın. Çift bağış/armağan oluşmamasını ve kilit bekleme durumunda güvenli tek hata/başvuru kodunu doğrulayın.
9. Aktarılmamış birkaç ekstreyi silin: kayıt/arşiv/audit birlikte tamamlanmalı. Gruba aktarılmış/bağış bağlantılı/eksik kayıt eklenince hiçbiri silinmemeli. Arşiv/audit hatasında grubun tamamı geri alınmalı.
10. Commit sonrası liste/form/takvim okuma hatasında tek sonuç bildirimi ve yenileme yönlendirmesi; URL ile veya sıradan yenilemede sahte başarı olmaması beklenir. Arama, filtre, eşleştirme dönüşü, tümünü seç, onay modalı ve Excel dışa aktarmasını kontrol edin.
11. Log ayarı kapalıyken iş transaction'ları sürmeli; merkezi kayıt ve eski OlayKayit/SQL çift yazımı oluşmamalı. Bu parça dışındaki parametresiz servis çağrılarının davranışını gerileme açısından kontrol edin.

Codex build/test/debug çalıştırmadı ve veritabanına bağlanmadı. Kontroller kaynak çağrı zinciri/transaction incelemesi, UTF-8/BOM, C# sembol/region dengesi, iç yardımcı çağrı argüman sayıları, proje XML/Compile kayıtları ve git diff ile sınırlıdır; derleyici veya çalışma zamanı doğrulaması yerine geçmez. ASCX üretilmiş dosyaları değiştirilmedi. Commit/push/PR/merge kullanıcıya aittir.

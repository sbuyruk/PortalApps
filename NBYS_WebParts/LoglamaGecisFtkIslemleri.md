# NBYS FTK kişi, kurul işlemleri ve rapor geçişi

Dal: `codex/nbys-ftk-islemleri-log-transaction-mesaj`.
Başlangıç: `848fa2ef77806af57e06524841dcba4ca46f7129` (22 Ekim 2026).
Değişiklikten önce master çalışma ağacının temiz ve GitHub master ile aynı commit'te olduğu doğrulandı. PR #510 master'a alınmıştı; kullanıcı devam talimatı verdi. Önceki parçanın çalışma zamanı TEST sonucu varsayılmadı.

## Kapsam

Altı ekran birlikte taşındı:

- FTKKisiGirisiWP: kişi ekleme/güncelleme/silme, isim uyarısı, form/yer/görev okumaları.
- FTKIslemleriWP: kurul kaydı, yeni üye geçmişi, son geçmişin düzeltilmesi, onaydaki üyelerin görevini topluca sonlandırma; il/ilçe/vali/kaymakam ve liste/modal okumaları.
- FTKListesiWP: filtreler, liste, gruplama, Excel ve işlem ekranına geçiş.
- FTKKuruluOlmayanIlIlceListesiWP: bölge/il/ilçe, liste ve Excel.
- FTKHaritasiWP: il seçimi ve üye detay modalı.
- BolgelereGoreFTKRaporuWP: kurul/il/ilçe sayıları, oranlar, rapor ve Excel.

FTKKisiListesiWP ve FTKFahriBaskanListesiWP önceki salt okuma parçasında taşınmıştı. FTKYazilariWP dosya üretimi/SharePoint dosyası/e-posta içerdiği için belge ve dış etki parçasında kalır. Düzenli bağışçı, SMS ve diğer belge/basım akışları bu parçanın kapsamında değildir. Kod, e-posta veya başka dış mesaj gönderilerek doğrulanmadı.

## Transaction ve iş davranışı

Yeni `NbysFtkWriteService` ve `NbysFtkWriteRepository`, yalnızca bu iki yazma ekranının açık bağlamlı iş yoludur. Mevcut parametresiz FTK servisleri ve diğer çağıranlar korunur. UI içindeki bağımsız Save/Update/Delete ve DeactivateByIdList çağrıları kaldırıldı.

Her yazma komutu tek `SqlTransactionContext` kullanır. Province/district, kişi/kimlik mükerrerliği, kurul, aktif üyeler, sayaç ve geçmiş okumaları ile bütün iş/audit yazmaları aynı transaction'dadır. İş transaction'ı içinde ayrı `DbClass`, bağımsız repository erişimi veya NOLOCK yoktur. Yeni repository metotlarının transaction parametresi zorunludur.

Geçiş yapılan FTK yazmaları transaction sahibinde tutulan `PortalApps.NBYS.FTK.Write` uygulama kilidini paylaşır; bekleme sınırı 15 saniyedir. Böylece kişi değişikliği, üyelik değişikliği ve kurul geçmişi hazırlama birbirleriyle yarışmaz. İlgili SELECT'ler ayrıca UPDLOCK/HOLDLOCK kullanır. Bu, taşınmamış başka uygulamaların aynı kilidi kullandığı anlamına gelmez. Hedef SQL Server'da sp_getapplock erişimi ve eşzamanlılık davranışı TEST'te doğrulanmalıdır.

### Kişi

Ad ve soyad zorunludur. Pozitif T.C. kimlik numarasında aynı transaction içinde, güncellenen kişinin kendisi hariç mükerrerlik kontrol edilir. Boş/sıfır kimlik için mevcut isteğe bağlı davranış korunur; aynı ad/soyad ayrı kişi olabileceğinden kayıt yasağına çevrilmez, yalnızca ön uyarı verilir. Güncellemede olmayan kişi ekleme gibi değerlendirilmez.

Güncel kişi kilitlenip yalnızca formda düzenlenen alanlar kopyalanır; ilk oluşturma bilgileri korunur. İl ve ilçe/valilik ilişkisi veritabanından doğrulanır. Valilik için mevcut -1 sabiti korunur. Kurul bölgesi, değiştirilebilir form metni yerine seçilen ilin veritabanı kaydından alınır; yeni bir bölgesel yetki sistemi eklenmedi.

Kişi silmede FTK_Table.KisiId geçmiş bağlantısı kontrol edilir. Geçmişte kullanılan kişi silinmez; kullanıcıya üyeliğini aktif değil yapması söylenir. Geçmişi olmayan kişinin DELETE'i ve audit'i birlikte tamamlanır. Bu, eski akışa göre referans bütünlüğü düzeltmesidir; mevcut kurul geçmişi topluca silinmez veya yeniden yazılmaz.

### Kurul ve üye geçmişi

İşlem bilgisi ile FTK_Table üye geçmişinin tamamı tek transaction'da yazılır. Yeni kayıtta il/ilçenin mevcut en büyük sayacı bir artırılır. Normal kayıtta aynı güncelleme tarihine ait FTK varsa yazma yapılmaz ve kayıt düzeltmesi uyarısı gösterilir.

Kayıt düzeltmesinde aynı kurulun son sayacı korunur; o sayaca ait geçmiş satırları silinip güncel aktif üyelerden yeniden oluşturulur. Eski satırların silinmesi, yeni satırlar, işlem bilgisi ve audit birlikte tamamlanır. Son üyenin/audit'in yazılması hata verirse eski geçmiş dahil tamamı geri alınır. Düzeltilecek geçmiş yoksa işlem bilgisi de güncellenmez.

Aktif üye yoksa boş bir kurul işlemi veya geçmiş oluşturulmaz. İlk kuruluşta henüz FTKIslem satırı olmadığı için üyeleri A.Id=0 filtresiyle gizleyen eski okuma, yalnızca açık bağlamlı yolda düzeltildi. Yazma servisi üyeleri doğrudan il/ilçe üzerinden aynı transaction'da okur. Görev adlarının Fahri Başkan/Başkan/Genel Sekreter/Üye eşlemesi ve mevcut üye bilgilerinin geçmişe kopyalanması korunur. Aynı yerde birden fazla FTKIslem kaydı varsa rastgele birini değiştirmek yerine tutarsızlık uyarısı verilir.

Başarı sonucu ve yeni kayıt kimliği yalnızca using/Dispose commit'i bittikten sonra UI'a döner; Complete tek başına kullanıcıya başarı bildirmez.

### Görev sonlandırma

Onay listesindeki kişi kimlikleri tekilleştirilir. Her kişi transaction içinde yeniden okunur; hâlâ aynı il/ilçede ve aktif olmalıdır. Eksik, taşınmış veya üyeliği değişmiş tek kişi varsa hiçbirinin durumu değiştirilmez. Bütün seçim ve kişi başına audit tek transaction'dadır. Bu işlem geçmiş snapshot'larını değiştirmez ve onay listesine sonradan eklenmemiş kişileri kendiliğinden pasifleştirmez.

GetDataByIlIlce içinde henüz hazırlanmakta olan listeye ek olarak eski ViewState listesine tekrar üye ekleme kaldırıldı. Onay listesi, yeni hazırlanmış listeyle bir kez değiştirilir. Liste URL'deki FTKList metninden alınmaz; yalnızca sunucunun hazırladığı türü doğrulanmış ViewState listesi kullanılır.

## Log ve audit

Altı WebPart'ta ilk istekte tek UserActivity; postback'te yalnızca ilgili olay sınırı kullanılır. FTK, FTKIslem, FTKKisi ve gerekli ortak yer/bölge okumaları açık PortalLogContext taşır. Bölge filtresi için mevcut kullanıcı/bölge seçiminin açık bağlamlı repository yolu kullanılır. SQL izlerinde sorgu veya parametre değerleri bulunmaz; işlem türü, repository/metot, süre ve komut sonucu vardır.

Merkezi audit, mevcut NbysTransactionalWriter'ın üç tablo için genişletilen izin listelerini kullanır:

| Entity | Audit alanları |
| --- | --- |
| FTKKisi | Ili, Ilcesi, Vali, Kaymakam, FTKGorevi, UyelikDurumu |
| FTKIslem | Ili, Ilcesi, BolgeId, KurulusTarihi, GuncellemeTarihi |
| FTK | FTKIslemId, Ili, Ilcesi, BolgeId, KurulusTarihi, GuncellemeTarihi, Sayac, KisiId, FTKGorevi |

Eski/yeni değerler transaction içindeki gerçek veritabanı görüntülerinden alınır. INSERT/DELETE var/yok; UPDATE yalnızca değişen izinli alanları kaydeder. Ad/soyad, T.C. kimlik numarası, doğum tarihi, telefon, adres, unvan, kart numarası ve açıklama audit ayrıntısına alınmaz. Yalnızca bu alanların değiştiği bir güncellemede audit başlığı olur, alan ayrıntısı olmayabilir. Kişisel bilgiler mevcut iş/geçmiş tablolarında kalır.

Bu yolda eski OlayKayit yazımı yoktur. Log ayarı kapalıyken transaction'lar sürer, merkezi kayıt ve eski yazıcıya geri dönüş oluşmaz. Bağımsız teknik Error/SqlTrace rollback sonrası kalabilir; SQL Succeeded iş transaction'ının commit edildiği anlamına gelmez.

## Mesaj değişiklikleri ve inceleme noktaları

IKYS ResmiTatilGirisiWP'nin güvenli Türkçe hata/başvuru kodu yaklaşımı ve TBYS TbysWebPartLog'un aynı hata zincirini tek bildirimle sunması kaynakta kontrol edildi. Önceki NBYS parçalarındaki Messages/NbysWebPartLog bu ekranlara genişletildi; log kapalıyken eski mesaj/log yazıcısına dönüş eklenmedi.

- Ad/soyad, mükerrer kimlik, doğum tarihi, il/ilçe, kurul tarihleri, aynı güncelleme tarihi, düzeltilecek geçmiş olmaması, aktif üye olmaması ve değişmiş seçimler ayrı Warning sonuçlarıdır. İş kuralı için exception üretip genel teknik hata yayımlanmaz.
- Aynı ad/soyad ön uyarısı korunur, düzenlenen kişinin kendisi dışarıda bırakılır. Soyadı TextChanged, Kaydet/Güncelle postback'inde ikinci uyarı üretmez; kendi AutoPostBack isteğinde çalışır.
- Kişi kaydetme/güncelleme/silme, kurul/geçmiş kaydetme ve görev sonlandırma yalnızca commit sonrası tek başarı mesajı verir. Önce bir kişi/işlem kaydedilip sonraki hatadan sonra başarının kalması kaldırıldı.
- Commit sonrası form/liste yenilemesi başarısızsa işlemin sonucunu ve “Ekran yenilenemedi. Sayfayı yenileyin.” adımını içeren tek bildirim gösterilir. Ayrıca başarı popup'ı eklenmez. Kişi yenilemede bulunamazsa hem “bulunamadı” hem “kaydedildi” mesajı üretilmez.
- Kişi silmede koşulsuz listeye yönlendirme ve sonucu kontrol etmeden devam etme kaldırıldı. Başarıdan sonra form yazma düğmeleri kapanır; yeni kayıt/liste düğmeleri kullanılabilir. Geçmiş bağlantılı kişi için pasifleştirme önerisi verilir.
- FTK açılışındaki iki bilgilendirme tek mesaja indirildi. Boş aktif üye listesinde onay modalı açılmaz. Tarih, seçim ve uyarı metinleri kısa Türkçe olarak düzenlendi.
- Liste/JSON yardımcılarının hatayı yutup boş veriyle devam etmesi kaldırıldı. Teknik hatalar tek olay sınırına ulaşır; kullanıcıya ham exception.Message, SQL veya stack trace gösterilmez, başvuru kodu verilir.
- Haritada geçersiz il seçimi anlamlı uyarıdır. Detay tablosunun önceki satır/başlıkları yenilemeden önce temizlenir; tekrarlanan seçim satırları biriktirmez.
- Yönlendirme ve Excel dışa aktarmadaki ThreadAbortException teknik hata olarak kaydedilmez.

## Yapılandırma

Mevcut değerleri koruyarak `PortalLogging.EnabledWebParts` listesine ekleyin:

`NBYS:FTKKisiGirisiWP;NBYS:FTKIslemleriWP;NBYS:FTKListesiWP;NBYS:FTKKuruluOlmayanIlIlceListesiWP;NBYS:FTKHaritasiWP;NBYS:BolgelereGoreFTKRaporuWP`

NBYS'nin tamamını EnabledProjects ile açmayın. Yeni migration yoktur; hedefte mevcut merkezi log/audit tabloları ve izinleri bulunmalıdır. WSP yayını migration çalıştırmaz. DAO/Model/Utility ve NBYS paketini güncel çıktılarla birlikte yayımlayın. Üretilen ASCX dosyaları değiştirilmedi; yeni kontrol veya kod üretimi gerekmez.

## Kullanıcının Visual Studio 2026 / TEST doğrulaması

1. Solution ve NBYS WSP'yi kullanılan yapılandırmada derleyip TEST'e yayımlayın. Altı ekranın ilk açılışında tek UserActivity, postback'te tekrar açılış olmaması, SELECT izleri ve aynı istekte ortak OperationId beklenir.
2. FTKKisiGirisiWP'de yeni kişi ekleyin; ad/soyad, isteğe bağlı kimlik/doğum tarihi, il/ilçe/valilik, görev ve üyelik durumu örneklerini deneyin. Tek başarı ve INSERT audit beklenir. Boş ad/soyad, bozuk kimlik/tarih veya geçersiz yer ilişkisinde tek uyarı ve sıfır yazma olmalı.
3. Aynı pozitif T.C. kimlik numarasıyla ikinci kişi eklemeyi ve başka kişiyi bu kimliğe güncellemeyi deneyin. Mükerrerlik uyarısı ve sıfır yazma/audit beklenir. Kişinin kendi kimliğiyle güncellenmesi engellenmemeli. Aynı ad/soyad ön uyarısı kayıt yasağına dönüşmemeli ve Kaydet postback'inde çift uyarı çıkmamalı.
4. Kişiyi güncelleyin; ilk oluşturma bilgilerinin korunmasını ve audit izin listesini kontrol edin. Modal/form açıkken başka oturumda kişi silinirse bulunamadı uyarısı olmalı; yeni kişi eklenmemeli. Kişisel alanların audit ayrıntısına girmediğini doğrulayın.
5. Geçmişi olmayan test kişisini silin; DELETE audit ve tek başarı beklenir. FTK geçmişinde kullanılan kişiyi silmeyi deneyin; kişi/geçmiş korunmalı ve üyelik pasifleştirme uyarısı gösterilmeli.
6. İlk kuruluş için aktif üyeleri olan, henüz FTKIslem kaydı olmayan bir il/ilçe seçin. Önizlemede üyeler görünmeli. Kurul işlemi ve bütün üye geçmişi tek transaction'da oluşmalı; aynı sayaç ve aynı OperationId beklenir. Bölge veritabanındaki ile göre alınmalı.
7. Yeni güncelleme tarihiyle ikinci geçmiş oluşturun; sayaç artmalı. Aynı tarihte normal kayıt engellenmeli. Kayıt düzeltmesiyle son sayaç korunmalı ve o sayacın üyeleri yenilenmeli. Düzeltilecek geçmiş/aktif üye yokken hiçbir işlem bilgisi değişmemeli.
8. TEST'te ikinci/son üye INSERT'ini veya audit yazmasını kontrollü başarısız kılın. İşlem bilgisi, tüm üye geçmişi ve audit geri alınmalı. Düzeltmede silinen eski geçmiş de geri gelmeli; başarı mesajı görünmemeli. Teknik hata/SQL izleri kalmalı.
9. Aynı yerde eşzamanlı iki kurul kaydı ve aynı kimlikle eşzamanlı iki kişi kaydı deneyin. Çift tarih/kişi/sayaç oluşmamasını, uygulama kilidinin alınmasını ve kilit bekleme hatasında tek güvenli hata/başvuru kodunu kontrol edin.
10. Aktif üyelerin görevlerini topluca sonlandırın. Bütün seçim ve kişi başına audit birlikte commit olmalı; FTK geçmişi değişmemeli. Onaydan önce başka oturumda bir üyeyi taşıyın/silin/pasifleştirin: grup kısmen güncellenmemeli. Bir üyenin audit yazma hatasında grubun tamamı geri alınmalı.
11. FTK listesi/kurulu olmayan yerler/harita/bölge raporunda filtre, gruplama, sayaç, modal, haritada tekrarlanan seçim, boş sonuç ve Excel'i önceki davranışla karşılaştırın. FTKYazilariWP ve belge/e-posta davranışının bu parçada taşınmadığını dikkate alın.
12. Commit sonrası form/liste okuma hatasında tek sonuç+yenileme mesajını; log kapalıyken iş transaction'larının sürmesini ve eski OlayKayit/SQL çift yazımı olmamasını doğrulayın.

Codex build/test/debug çalıştırmadı ve veritabanına bağlanmadı. Kontroller kaynak/transaction çağrı incelemesi, C# sembol/region dengesi, proje XML/Compile girdileri, model/audit alanları, olay sınırları, UTF-8/BOM ve git diff ile sınırlıdır; derleyici veya çalışma zamanı doğrulaması değildir. Hedef şema, uygulama kilidi, iş kuralları ve rollback senaryoları kullanıcı TEST sonucunu bekler. Commit/push/PR/merge kullanıcıya aittir.

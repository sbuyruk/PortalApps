# IKYS eski loglama geçişi — parça 6

Başlangıç: temiz yerel `master` ve canlı uzak `master` aynı commit'te doğrulandı: `d75ee066e8c457726b3df95c9d366fcc44554c1e` (PR #542).

Dal: `codex/ikys-izin-donem-eski-log`.

## Kapsam ve bulgular

`IzinDonemListesiWP.UpdateNowBtn_Click` ve `DonemEkleNowBtn_Click`, merkezi loglama kapalıyken bağlamsız `IzinDonemService.Update/Save` üzerinden eski olay ve SQL yazıcılarına dönüyordu. Yazma öncesindeki personel, iş bilgisi ve dönem kontrolleri transaction dışında kalıyordu. Güncelleme hataları da genel bir tamsayı giriş hatası gibi gösteriliyor, teknik hata merkezi kayda ulaşmıyordu.

Ekranın personel seçimi bağlamsız okuyordu. Dönem tablosunun context alan servis overload'ı eski repository okumasını teknik izle sarıyor; personel listesinin context overload'ı bile içeride `db.SelectFromDb` kullanıyordu. Bu nedenle yalnız yazma dallarını kaldırmak SQL hatasında eski tabloya yazmayı engellemeye yetmiyordu.

Ortak `IzinDonemService` transaction yazıları merkezi audit başlığı üretiyor, değişen alanları kaydetmiyordu. Manuel bakiye düzenlemesini eski olay sisteminden ayırırken bu eksik de kapatıldı.

## Proje bazında değişiklikler

### IKYS_WebParts — IzinDonemListesiWP.ascx.cs

- `UpdateNowBtn_Click` önce mevcut üç tamsayı alanını doğrular; `DonemiGuncelle` güncel dönem kaydını kendi transaction'ında kilitleyerek okur, seçilen personel ve ücretli izin türüyle eşleştirir, aynı transaction ile günceller. Modal açıldıktan sonra kayıt silinmiş/değişmişse eski nesneyle yazılmaz.
- `DonemEkleNowBtn_Click` tarih ve tamsayı girişini kontrol eder. `DonemiKaydet`, personel, iş bilgisi ve mevcut dönemleri aynı transaction ile okur; işe başlama ve çakışma kontrollerinden sonra dönemi ve auditini birlikte yazar. Dönem listesi `UPDLOCK, HOLDLOCK` ile okunur; kontrol ile INSERT arasındaki aralık aynı transaction'dadır.
- İki yardımcıda da transaction her durumda açılır. Başarı ancak yardımcıdan dönüldükten, dolayısıyla `Dispose` içindeki commit tamamlandıktan sonra gösterilir. DB/audit/commit hatası güvenli merkezi `ReportError` sınırına ulaşır. Teknik SQL/hata kayıtlarının bağımsız yazılması değişmedi.
- `DonemDegerleriGecerli` mevcut tamsayı şartını korur. İzin hakkı, kullanılan ve kalan izin yönetici tarafından ayrı ayrı girilmeye devam eder; yeni bir bakiye denklemi veya negatif değer yasağı eklenmedi.
- `ReportError` eski `ExceptionHelper` geri dönüşünü kaldırır; yönlendirme kaynaklı `ThreadAbortException` hata kaydı sayılmaz. Personel seçim olayına hata sınırı eklendi.
- Personel, iş bilgisi, personel seçme listesi ve dönem tablosu okumaları context üzerinden ilerler. Personel seçimi yoksa kimlik 0 kabul edilir; dönem tablosu boşken null tablo dolaşılmaz. Yeni kontrol alanı veya ASCX değişikliği yoktur.

Kontroller transaction içine taşınırken netleştirilen mevcut dönem ekleme davranışları:

1. Dönemler başlangıca göre azalan sıralıdır. Modal varsayılanı eski `FirstOrDefault` yerine `LastOrDefault` ile gerçekten en eski dönemi kullanır; önizlemenin eski dönemin öncesini önerme amacı korunur.
2. Dönem bitişi mevcut en eski dönemin başlangıcına eşitse de çakışma vardır. Kontrol artık `>=` kullanır. Eksik dönem öncesine ekleme amacı korunur; araya veya sonraya dönem ekleme açılmaz.
3. Eksik iş bilgisi veya geçerli işe başlama tarihiyle kayıt yapılmaz. İşe başlama tarihi boşsa izin dönemi başlangıcı yedek olarak kullanılır; önceki non-nullable `DateTime != null` kontrolü kaldırıldı.
4. Ters/geçersiz tarih aralığı reddedilir. Dönemi olmayan personelde modal eski tarihleri taşımaz; tarihler boş başlar ve geçerli girişle ilk manuel dönem oluşturulabilir. Önceki boş listede `LastOrDefault` sonucuna erişim hatası giderildi.

### Model — IzinDonemService.cs

- `GetByPersonelId(int, int, SqlTransactionContext, PortalLogContext)` eklendi; transaction ve context zorunludur.
- `GetByPersonelIdReturnDataTable(int, int, PortalLogContext)` mevcut güvenli context repository overload'ına doğrudan bağlandı. Eski okuma ve ikinci SQL iz katmanı kaldırıldı.
- Transaction alan `Save`, `Update`, `Delete`, mevcut tek audit başlığının kimliğiyle `RecordChanges` çağırır. İkinci audit başlığı eklenmedi. Başlık ve bütün ayrıntılar aynı transaction'dadır; log kapalıysa auditId 0 kontrolü ayrıntı yazımını atlar.
- Ayrıntılar: PersonelId, IzinTipi, BaslangicTarihi, BitisTarihi, Adi, IzinHakki, KullanilanIzin, KalanIzin, Birim, Aciklama. Güncellemede yalnız farklı değerler yazılır. Bu iyileştirme ortak servisi kullanan önceki izin akışlarını da etkiler.
- Merkezi ayrıntı yazıcısının mevcut 2000 karakter değer sınırı korunur. Aşan bir alan değişikliği sessizce kesilmez; ayrıntı yazımı hata verirse iş verisi ve audit birlikte geri alınır. Uzun açıklama senaryosu kabul kontrolüne dahildir.

### DAO — IzinDonemRepository.cs, PersonelRepository.cs

`IzinDonemRepository.SelectByPersonelId` için transaction overload'ı eklendi. Mevcut SELECT alanları, filtreler, parametreler ve azalan sıralama korunur; yalnız `UPDLOCK, HOLDLOCK` ve aynı transaction üzerinden yürütme eklenir.

`PersonelRepository.SelectActiveEmployees(int, PortalLogContext)` mevcut sorgu üreticisini `PortalSqlReadExecutor` ile çalıştırır. Sonuç ve filtre değişmez; SQL hatasında eski yazıcıya gidilmez ve her gerçek sorgu için tek merkezi SQL izi oluşur. Diğer ekranların aynı context overload'ını kullanması da bu güvenli yolu seçer. Bağlamsız overload henüz taşınmamış çağıranlar için korunur.

## Statik kontroller

- `git diff --check` temiz. Dört kaynak dosyasında sıkı UTF-8 okuma, HEAD ile BOM eşitliği, CRLF ve string/yorum dışı ayraç dengesi kontrol edildi.
- Ekranın beş etkin ASCX olay bağlantısı ve dört dosyanın mevcut proje Compile kayıtları doğrulandı. ASCX, üretilen `*.ascx.g.cs`, proje, feature, paket ve ayar dosyaları değiştirilmedi.
- Yazma yardımcılarındaki altı servis çağrısının transaction/context aktarımı ve bütün kapsam içi ekran okumalarının context kullanımı kontrol edildi. Repository, servis ve audit alt çağrıları incelendi.
- Yeni dönem sorgusu mevcut context sorgusuyla karşılaştırıldı: yalnız kilitler, transaction doğrulaması ve yürütücüye transaction aktarımı farklıdır.
- Üç transaction yazma metodunda birer audit başlığı ve birer ayrıntı çağrısı, 10 alan ve kapalı log guard'ı kontrol edildi. Ortak teknik yazıcılar değiştirilmedi.

Windows bilgisayarında build, test veya debug çalıştırılmadı. Commit, push, PR, merge, yayın ve çalışma zamanı doğrulaması kullanıcıya aittir. Eski tablolar, geçmiş veriler ve DB şeması korunur.

## Kullanıcının uygulayacağı kabul senaryoları

Kontrollü test kayıtları kullanın. Merkezi IKYS ve ekran log ayarları açıkken ve kapalıyken tekrarlayın; sonunda ayarları başlangıç değerlerine getirin.

1. Visual Studio 2026'da derleyip Test sunucusuna yayımlayın. Personel seçimi, dönem tablosu, düzenleme ve izin hareketleri modalları, dönem ekleme ve yönlendirmeler çalışmalı. Personel/dönem bulunmayan ve boş liste durumlarını da deneyin.
2. Mevcut ücretli dönemin üç sayısal alanını ayrı ayrı güncelleyin. Diğer alanlar korunmalı. Geçersiz/taşan tamsayı girişinde yazı olmamalı. Modal açıldıktan sonra silinen veya başka personel seçiliyken gönderilen dönem kimliğine yazı yapılmamalı.
3. Eksik eski dönem ekleyin. Modal birden fazla dönem arasından en eskisini esas almalı. Kaydetmede güncel iş bilgisi/dönemler tekrar okunmalı. İlk dönemi olmayan personel için de geçerli manuel tarih girişiyle kayıt yapılmalı; önceki modalın tarihleri taşınmamalı.
4. Başlangıçtan önce bitiş, geçersiz tarih, işe başlamadan önce başlangıç, mevcut en eski dönemle örtüşme ve bitişin onun başlangıcına eşit olması durumlarında yazı olmamalı. Eski dönemden bir gün önce biten geçerli aralık kabul edilmeli. Eksik iş bilgisi/başlangıç tarihleri güvenli mesaj üretmeli.
5. Aynı personel için aynı eski dönemi iki pencereden eşzamanlı ekleyin. Yeni transaction zincirleri arasında ikinci işlem güncel dönemleri görmeli ve çakışan kayıt oluşmamalı. Henüz taşınmamış diğer yolların tamamı için genel eşzamanlılık garantisi olarak değerlendirmeyin.
6. INSERT/UPDATE, audit başlığı, audit alan ayrıntısı ve commit adımlarında ayrı ayrı kontrollü hata oluşturun. İş verisi ve audit bütünüyle geri alınmalı; başarı mesajı çıkmamalı. Ayrıntı değeri 2000 karakter sınırını aşan açıklamayla da rollback davranışını doğrulayın.
7. Log açıkken INSERT/UPDATE/DELETE audit alanlarını inceleyin: tek başlık ve doğru eski/yeni değerler beklenir. Değişmemiş alanın ayrıntısı oluşmamalı. Log kapalıyken iş işlemi sürmeli, auditId 0 nedeniyle ayrıntı hatası oluşmamalı ve bu ekranın yollarından `Olay_Table`/`SQLLog_Table` yazısı çıkmamalı. Diğer ekranların eşzamanlı eski yazılarını ayrı değerlendirin.
8. Personel seçimi/dönem tablosu/movement SELECT hatası oluşturun. Log açıkken gerçek SQL başına tek merkezi iz ve üst hata sınırında güvenli hata mesajı olmalı. Log kapalıyken eski SQL yazıcısına dönülmemeli. Rollback sonrası bağımsız teknik kayıt kalabilmesi iş/audit atomikliğini bozmaz.
9. Ortak dönem servisini kullanan önceki personel, izin talep/onay ve hareket güncelleme/silme senaryolarını kısa regresyondan geçirin. Artık dönem alan ayrıntıları da görünmeli; audit başlığı sayısı artmamalı. Alan ayrıntısındaki hata, o akışın tüm transaction'ını geri almalı.

## Kalan kapsam

Bu parça izin dönemi listesini kapsar. Mazerete/ücretliye mahsup, toplu izin, diğer kalan bağlamsız çağrılar ve son bağımlılık temizliği sürmektedir. Bütün IKYS/solution için sıfır eski yazı hedefi henüz tamamlanmadı. Eski tabloların bağımlılık, arşivleme ve sonradan kaldırma değerlendirmesi ayrıca tamamlanacaktır.

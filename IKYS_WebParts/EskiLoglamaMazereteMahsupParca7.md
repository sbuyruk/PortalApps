# IKYS eski loglama geçişi — parça 7

Başlangıç: temiz yerel `master` ile canlı uzak `master` aynı commit'te doğrulandı: `24a86fe65bc8c50c9cffe8b461ca1e463c13c363` (PR #543).

Dal: `codex/ikys-mazerete-mahsup-eski-log`.

## Kapsam ve bulgular

Bu parça yalnız `MazereteMahsupWP` ve onun gerekli okuma/audit bağımlılıklarını kapsar. `UcretliMahsupWP` ve toplu izin sonraki parçadadır.

`BirGunMahsupEt` merkezi loglama kapalıyken ayrı bağlantılardan hareket, mahsup ve iki dönem bakiyesi yazıyordu. İlk personel/dönem okumaları merkezi log açıkken bile transaction dışında kalıyordu. Geri alma için saklanan nesneler kopya değildi; değiştirilmiş aynı referanslardı. Ayrıca mahsup kaydının başarı kimliği toplam başarı koşuluna dahil değildi.

`FillIzinTable` ilk açılışta, postback'te ve Excel hazırlığında eksik dönemleri otomatik oluşturuyordu. Bu gizli yazılar da bağlamsız `CreateForPersonel` üzerinden eski yazıcıya ulaşıyordu. Bu yüzden yalnız onay butonunu taşımak yeterli değildi.

## Değişiklik ve inceleme noktaları

### IKYS_WebParts/MazereteMahsupWP/MazereteMahsupWP.ascx.cs

- `BirGunMahsupEt` her durumda tek transaction açar. Güncel personel ve iki dönem aynı transaction ile okunur; mevcut repository overload'larının `UPDLOCK, HOLDLOCK` kilitleri kullanılır. Eksik ücretli dönem de aynı transaction ile oluşturulur. Mazeret dönemi bulunmazsa işlem yazmadan reddedilir.
- Bir hareket INSERT'i, bir mahsup INSERT'i ve iki dönem UPDATE'i aynı transaction'dadır. Her yazının sonucu doğrulanır; mahsup kimliği sıfırsa da işlem başarısız olur. Audit başlıkları ve alan ayrıntıları aynı transaction üzerinden yürür. Yalnız bütün adımlar tamamlanınca `Complete` çağrılır; commit hatasında yardımcı başarıyla dönmez.
- Nullable transaction, log ayarına göre eski overload seçimi, bağlamsız `MahsupTablosunaEkle` overload'ı ve elle telafi yazıları kaldırıldı. Hata halinde DB rollback'i kullanılır; eski nesne referanslarını tekrar yazarak geri alma denenmez.
- Ücretli kullanılan izin +1, ücretli kalan izin -1, mazeret hakkı +9 saat ve mazeret kalanı yeni hak eksi kullanılan süre hesabı korunur. Ücretli bakiyenin sıfır/eksi olması mahsup yasağına dönüştürülmedi. Kaynak/hedef dönem kimliklerinin mevcut mahsup alanlarına yazılma yönü de korunur.
- UI'da zaten bulunan `(int)kalan.TotalHours < 8` şartı sunucuda güncel, kilitli mazeret bakiyesine uygulanır. Bakiye 8 saat veya üzerine çıkmışsa yeni mahsup yapılmaz. Çok negatif bakiyede bir aktarım sonrasında şart hâlâ sağlanabilir; bu durumda sonraki ayrı aktarım geçerlidir. Bu kontrol her koşulda yalnız bir mahsup kaydı garantisi değildir.
- Hareket tarihi mevcut davranıştaki gibi ücretli dönemin başlangıcıdır. Önceki `resmiTatilMi = false; while (resmiTatilMi)` gövdesi hiç çalışmıyordu; ölü döngü ve kullanılmayan nesne kaldırıldı. Tatil hesabında yeni bir iş kuralı uygulanmadı.
- `DonemleriHazirla` liste hazırlığında personel başına tek transaction kullanır. Personel ve mevcut dönemler güncel okunur; eksikler transaction-aware `CreateForPersonel` ile hazırlanır. İki dönemden biri hazırlanamazsa o çağrının bütün yazıları/auditleri geri alınır. Var olan dönemler yeniden yazılmaz.
- Liste hazırlığı ile sonraki onay farklı transaction'lardır. Liste/Excel hazırlığında önceden oluşturulmuş dönemler, daha sonraki mahsup rollback'inde kalır. Listedeki önceki personellerin tamamlanmış hazırlıkları da sonraki personelin hatasıyla geri alınmaz. Bu mevcut liste yan etkisi kaldırılmadı; güvenli yazma yoluna taşındı.
- Personel listesi, modal personeli, iş bilgileri ve modal dönemleri context ile okunur. Boş personel tablosu dolaşılmaz; modalda eksik dönem kontrollü hata verir. Ayrıntı ve Excel olaylarına merkezi hata sınırı eklendi. Excel çıktı biçimi/kodlaması ve dilekçe bağlantıları değiştirilmedi.
- `ReportError` eski hata yayıncısına dönmez. Normal yönlendirme/`Response.End` kaynaklı `ThreadAbortException` teknik hata sayılmaz. Başarı/yönlendirme mevcut onay handler'ında commit tamamlandıktan sonradır.

### DAO ve Model personel okuması

`PersonelService.GetActiveEmployeesReturnDT(PortalLogContext)` ve `PersonelRepository.SelectActiveEmployeesReturnDT(PortalLogContext)` eklendi. SELECT alanları, join'ler, aktiflik filtresi ve sıralama eski sorguyla aynıdır. Yeni overload `PortalSqlReadExecutor` kullanır; SQL hatasında eski SQL tablosuna yazmaz. Bağlamsız overload diğer çağıranlar nedeniyle korunur.

### Model audit ayrıntıları

`IzinHareketService.RecordChanges` mevcut yedi alanı korur; Sure, Birim, Adres, Aciklama ayrıntıları eklenir. `MahsupService.RecordChanges` mevcut beş ilişki alanına Aciklama ekler. Böylece mahsup hareketinin süresi ve kullanıcı açıklaması merkezi audit içinde de izlenir. Yeni bir audit başlığı veya ikinci log katmanı eklenmedi; yalnız değişen alanlar yazılır ve auditId 0 ise ayrıntı atlanır.

Bu ortak ayrıntı yardımcılarını kullanan önceki hareket/mahsup işlemleri de yeni alanları kaydeder. Merkezi yazıcının mevcut 2000 karakter değer sınırı korunur; aşan bir alan değişikliği sessizce kesilmez, hata verirse iş/audit birlikte geri alınır. Teknik SQL/hata kayıtlarının bağımsız bağlantı tasarımı değişmez.

## Statik doğrulama

- `git diff --check` temiz. Beş kaynak dosyasında sıkı UTF-8 okuma, HEAD ile BOM eşitliği, CRLF, ayraç dengesi ve mevcut proje Compile kayıtları doğrulandı.
- Dört etkin ASCX olay bağlantısı; 18 servis çağrısında context; onay/hazırlık/mahsup yardımcısındaki 13 çağrıda aynı transaction aktarımı kontrol edildi. `CreateForPersonel` içindeki iş bilgisi ve izin hakkı hesabının mevcut transaction/pure hesap zinciri incelendi.
- UI dışındaki 18 mevcut transaction metodunun gövdesi HEAD ile aynı; ortak servislerde yalnız audit ayrıntı yardımcıları genişletildi. Personel sorgusu yürütücü değişimi dışında kaynakla eşdeğer.
- Ekranda log ayarına göre geri dönüş, eski rollback nesneleri ve bağlamsız yazma yardımcısı kalmadı. ASCX, `*.ascx.g.cs`, proje, feature, paket, ayar veya DB şeması değiştirilmedi. Eski tablolar ve geçmiş kayıtlar korunur.

Windows bilgisayarında build, test veya debug çalıştırılmadı. Commit, push, PR, merge, yayın ve çalışma zamanı kontrolleri kullanıcıya aittir.

## Kullanıcının uygulayacağı kabul senaryoları

Kontrollü test verisiyle, merkezi IKYS/ekran log ayarları açıkken ve kapalıyken tekrarlayın; sonunda ayarları başlangıç değerlerine getirin.

1. Visual Studio 2026'da derleyip Test sunucusuna yayımlayın. Liste, birim grupları, modal, dilekçe ve Excel çıktısını kontrol edin. Boş personel listesi ile eksik personel/dönem durumları ham teknik mesaj üretmemeli. Normal yönlendirme/Excel bitişi teknik hata kaydı oluşturmamalı.
2. Dönemleri olmayan uygun personelde liste açın. Eksik ücretli ve mazeret dönemleri merkezi audit ile hazırlanmalı. Hazırlığın ikinci dönem veya audit adımında kontrollü hata oluşturun; o personel için aynı çağrıda oluşturulan dönemler birlikte geri alınmalı. Listeyi tekrar açınca mevcut dönemler yeniden oluşturulmamalı.
3. Mazeret kalanı 8 saatten az personelde bir gün mahsup edin. Tek hareket ve mahsup kaydı; ücretli kullanılan +1/kalan -1; mazeret hakkı +9 saat ve doğru kalan beklenir. Dönem bağlantıları, hareket tarihi, süre, açıklamalar ve audit alanlarını karşılaştırın. Ücretli bakiyesi sıfır/eksi örnekte mevcut izin verme davranışı sürmeli.
4. Modal açıldıktan sonra bakiyeyi değiştiren başka işlem yapın; onay güncel bakiyeyi kullanmalı. 8 saat ve üzerindeki bakiye yazı oluşturmamalı. İki pencereden eşzamanlı onayda güncellemeler kaybolmamalı. İlk aktarım sonrası bakiye hâlâ 8 saat altındaysa ikinci aktarımın geçerli olabileceğini dikkate alın; örneğin 0 saatten başlayan durumda ilk aktarım sonrası ikinci istek engellenmeli.
5. Ücretli dönemin mahsup sırasında yeniden oluşturulması gereken kontrollü durumda sonraki adımda hata oluşturun; aynı transaction'daki dönem de geri alınmalı. Liste hazırlığında önceden oluşturulmuş dönemleri bu beklentiye dahil etmeyin. Kaybolmuş mazeret dönemi veya eksik iş bilgisi kısmi yazı bırakmamalı.
6. Hareket INSERT, mahsup INSERT, iki bakiye UPDATE'i, audit başlığı/ayrıntısı ve commit hatalarını ayrı ayrı deneyin. Bütün mahsup yazıları ve audit geri alınmalı; başarı/yönlendirme olmamalı. Elle telafi yazısı veya telafi auditleri oluşmamalı. Kimliklerin artmış olması commit kanıtı sayılmaz.
7. Log açıkken her gerçek iş yazısı için tek merkezi audit başlığı ve doğru ayrıntılar beklenir. Yeni hareket alanlarını ve mahsup açıklamasını kontrol edin. Birden fazla dönem yazımı farklı gerçek yazılardır. Ayrıntısı 2000 karakter sınırını aşan değişiklikte rollback ve güvenli hata davranışını doğrulayın.
8. Liste/modal SELECT hataları oluşturun. Açık loglamada gerçek SQL başına tek merkezi iz ve üst hata sınırında merkezi hata beklenir. Kapalı loglamada iş sonucu ve rollback kapsamı korunmalı; bu ekranın liste/Excel hazırlığı ve mahsup yollarından `Olay_Table`/`SQLLog_Table` yazısı oluşmamalı. Diğer ekranların eşzamanlı eski yazılarını ayrı tutun.
9. Önceki izin hareketi oluşturma/güncelleme/silme ve mahsup servis çağrıları için kısa regresyon uygulayın. Yeni alan ayrıntıları görünmeli; audit başlık sayısı artmamalı. Ayrıntı hatası ilgili çağıranın bütün transaction'ını geri almalı. Bağımsız teknik SQL/hata kayıtları rollback sonrasında kalabilir.

## Kalan kapsam

Ücretli mahsup ekranı, toplu izin, diğer bağlamsız çağrılar ve son bağımlılık temizliği devam eder. Bütün IKYS/solution için sıfır eski yazı kapanışı henüz tamamlanmadı. Eski tabloları kaldırma veya geçmiş veriyi silme işlemi yoktur; bağımlılık ve arşivleme önerisi tüm proje geçişleri kapandıktan sonra ayrıca tamamlanacaktır.

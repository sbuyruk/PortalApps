# IKYS eski loglamayı devreden çıkarma — izin hareketi ve talep girişi, parça 3

Dal: `codex/ikys-izin-hareket-talep-eski-log`

Başlangıçta çalışma ağacı ve yerel `master` temizdi. HEAD, yerel `master` ve canlı uzak `master` aynı commit'teydi: `15ba2b0e17414768969f882906c97f0b23a244ae` (PR #539 merge commit'i). Kullanıcının devam talebiyle bu tabandan yeni dal açıldı. Önceki parçanın Test yayını veya çalışma zamanı sonucu bu incelemede doğrulanmadı.

## Kapsam

Bu parça `IzinHareketEditWP` ve `IzinTalepGirisWP` ekranlarını kapsar. Talep listelerinin onay/ret işlemleri, izin dönemi yönetim ekranları, mahsup ve toplu işlemler sonraki parçalardadır.

### İzin hareketi

- Güncelleme ve silme işlemi loglama ayarından bağımsız olarak tek `SqlTransactionContext` açar. Hareket kaydı transaction içinde yeniden okunur; personel, hareketin gerçek `PersonelId` bağlantısından aynı transaction ile alınır.
- Ücretli/mazeret izninin eski bakiyeden düşülmesi, yeni dönemin bulunması/oluşturulması/güncellenmesi, yeni bakiyeye eklenmesi, hareket ve bağlı talep güncellemesi birlikte tamamlanır veya geri alınır. Bakiye güncellemelerinin `false` sonuçları kontrol edilir.
- Silmede dönem bakiyesi, hareket ve bağlı talep aynı transaction'dadır. Dönem bakiyesi olmayan diğer izin türlerinde de varsa bağlı talep aynı işlemde silinir; önceki bağlamlı yol bu türlerde talebi bırakabiliyordu.
- Ücretli/mazeret hareketinde mevcut dönem bulunamazsa eksik bakiyeyle devam edilmez. İşlem hata vererek geri alınır. Bağlı talep zaten yoksa mevcut davranış gibi hareket işlemi devam edebilir.
- İki eski transaction dışı talep güncelleme/silme yardımcısı kaldırıldı. Hatalar işlem sınırında bir kez raporlanır. Commit hatası çağırana başarı olarak ulaşmaz.

### Talep girişi

- Yeni talep ve güncellemede personel, talep, gerekli dönem ve audit aynı transaction'dadır. Güncellemede personel talebin gerçek bağlantısından okunur. Döneme bağlı izin türünde dönem hazırlanamazsa `IzinDonemId=0` ile talep kaydedilmez.
- Loglama kapalıyken nullable transaction ve eski overload seçimi kaldırıldı. Audit kapalı durumunu ilk parçadaki merkezi servis kontrolleri karşılar.
- Beş satır silme butonu ortak yardımcıyı çağırır. Talebin güncel onay durumu transaction içinde okunur ve yalnızca mevcut iki izin verilen durumda silinir: işlem bekliyor veya dilekçe bekliyor. Okuma ve silme arasındaki ayrı bağlantı boşluğu kaldırıldı.
- Kalan izin ön kontrolünün otomatik dönem oluşturması korunur, fakat merkezi transaction yoluna taşınır. Ön kontrol ayrı bir işlemdir: ön kontrolde daha önce commit edilmiş dönem, sonraki talep kaydı başarısız oldu diye geri alınmaz. Talep kaydının kendi transaction'ında oluşturulan veya değiştirilen dönem ise o taleple birlikte geri alınır.
- Ön kontrolde dönem oluşturulamıyorsa null nesneyle devam etmek yerine güvenli bilgi mesajı ve başarısız sonuç döner. Ön kontrol ile ilgili seçim/tarih olaylarına teknik hata sınırları eklendi.
- Mevcut talep e-postası çağrısının yeri korunur: kayıt yardımcısı başarıyla döndükten, yani commit tamamlandıktan sonradır. Bu parçada e-posta gönderilmedi.

### Hesap ve okuma bağımlılıkları

`IKYSOrtak.IzinSuresiHesapla` ücretli izinde resmî tatilleri bağlamsız servisle sorguluyordu. Bağlamlı ve transaction alan overload'lar eklendi. Bütün overload'lar aynı hesap gövdesini kullanır; tatil sorgusunun yürütücüsü parametre olarak verilir. İş günü, hafta sonu, öğle arası ve diğer izin türlerinin mevcut hesabı değiştirilmedi. Kaynak karşılaştırmasında hesap gövdesinin yalnızca tatil sorgusu callback'i bakımından değiştiği doğrulandı.

Transaction alan hesap yolu `ResmiTatilService` ve `ResmiTatilRepository` üzerinden aynı transaction ile SELECT yapar. Ön kontrol hesabı bağlamlı SELECT kullanır. Güncel kullanıcı üzerinden personel bulma için `PersonelService/PersonelRepository` transaction overload'ları eklendi. Yeni sorgular mevcut filtre ve parametre kurallarını korur; teknik SQL izleri mevcut bağımsız yazıcıda kalır.

İki ekranın `ReportError` metotlarındaki eski `ExceptionHelper`/ULS geri dönüşü kaldırıldı. Merkezi teknik yazıcı kapalı ayarı kendisi değerlendirir. Yönlendirme nedeniyle oluşan `ThreadAbortException` uygulama hatası sayılmaz.

## Statik kontroller

- `git diff --check` temiz; yedi kaynak dosyası sıkı UTF-8 ile okundu, BOM durumları HEAD ile karşılaştırıldı ve satır sonları kontrol edildi.
- String/yorum dışındaki ayraç dengesi; 62 yerel yardımcı deklarasyonu/çağrısında argüman sayısı ve 26 ASCX olay bağlantısı kontrol edildi.
- 36 transaction içeren metotta 53 servis/repository/audit çağrısının transaction aktarımı tarandı. Doğrudan `transaction.Insert/Update` çağrıları kendi transaction alıcısıyla değerlendirildi.
- İki ekranın bütün iş verisi yazılarında transaction argümanı bulunduğu ve eski log ayarı/bağlamsız hata yayınlama yolu kalmadığı doğrulandı.
- Resmî tatil sorgusunun alt çağrı zinciri ve işlem sonrası e-posta sınırı incelendi. Ortak teknik log yazıcıları değiştirilmedi.
- Yeni derlenecek dosya eklenmedi; mevcut dosyaların proje Compile bağlantıları korundu. ASCX, `*.ascx.g.cs`, feature, paket veya DB şeması değiştirilmedi. Geçmiş veriye dokunulmadı.

Windows bilgisayarında build, test veya debug çalıştırılmadı. Commit, push, PR, merge, yayın ve çalışma zamanı doğrulaması kullanıcıya aittir.

## Kullanıcının uygulayacağı kabul senaryoları

Kontrollü test kayıtları kullanın. Ana senaryoları merkezi loglama hem açıkken hem kapalıyken tekrarlayın. Kapalı senaryoda hem `PortalLogging.EnabledProjects` içindeki IKYS girdisi hem ilgili `PortalLogging.EnabledWebParts` girdileri kapalı olmalıdır. Diğer projelerin ayarlarını koruyun ve test sonunda başlangıç ayarlarına dönün.

1. Visual Studio 2026'da solution'ı derleyin ve Test sunucusuna yayımlayın. İki ekranın ilk açılışı, postback, izin türü/personel/tarih/saat seçimi, beş silme butonu ve yönlendirmeleri çalışmalı. Normal yönlendirme teknik hata üretmemeli.
2. Ücretli izni aynı dönem içinde uzatın/kısaltın. Bakiye önceki hareket süresi düşülüp yeni süre eklenmiş net sonucu göstermeli; hareket ve varsa talep yeni tarih/süre/dönem bilgilerini birlikte taşımalı. Dönem değişen tarih seçin; eski/yeni dönem bakiyeleri birlikte güncellenmeli.
3. Mazeret izninde başlangıç/bitiş saati ve öğle aralığını kapsayan kayıtları deneyin. Diğer izin türlerinde mevcut gün/süre hesabını karşılaştırın. Ücretli izin için hafta sonu, tek seferlik ve yıllık tekrarlanan resmî tatil, ilan/iptal tarih sınırlarını doğrulayın.
4. Hareket güncellemesinde eski bakiyeden düşme, yeni bakiyeye ekleme, hareket yazımı veya bağlı talep yazımında kontrollü hata oluşturun. Bütün iş verileri ve audit birlikte geri alınmalı; başarı mesajı çıkmamalı. Mevcut dönem kaydı eksikse kısmi hareket güncellemesi yapılmamalı.
5. Ücretli/mazeret hareketini silin; bakiyesi iade edilmeli, hareket ve varsa talebi birlikte silinmeli. Başka izin türünde de bağlı talep bırakılmamalı. Dönem veya bağlı talep zaten bulunmayan kontrollü kayıtlarda yukarıdaki davranışları doğrulayın. Silme zincirinin ortasında hata olduğunda hepsi geri alınmalı.
6. Ücretli/mazeret talebi oluşturun ve güncelleyin. Eksik dönem talebin transaction'ında hazırlanmışsa sonraki talep/audit hatasında dönem değişikliği de geri alınmalı. Ön kontrolde önceden oluşturulmuş dönemi bu rollback'e dahil saymayın. İşe başlama verisi eksikse dönem kimliği sıfır olan talep oluşturulmamalı.
7. Diğer izin türlerinin talep kaydını ve güncellemesini deneyin. Başvuru formunun mevcut alan ve süre kuralları korunmalı; loglama kapalı olduğu için farklı iş akışı seçilmemeli. Commit veya audit yazımı başarısızsa pozitif talep kimliği/başarı sonucu ve e-posta oluşmamalı.
8. Beş talep silme butonunu deneyin. İşlem/dilekçe bekleyen kayıt silinmeli; onay durumu değişmiş kayıt korunmalı. Aynı kaydı eşzamanlı onaylama/silme durumunda transaction içindeki güncel durum kontrolünü doğrulayın. Silinmiş kayıt güvenli bilgi mesajı vermeli.
9. Loglama açıkken gerçek yazma adımları için merkezi audit ve alan ayrıntıları aynı transaction'da olmalı. Tek kullanıcı eylemi birden fazla dönem/bakiye yazımı yapabilir; bu adımların auditleri farklı gerçek yazılardır. Aynı yazı ikinci bir katmanda tekrar loglanmamalı. Resmî tatil/personel SELECT izleri aynı OperationId ile ilişkilendirilmeli. Rollback sonrasında teknik SQL/hata izi kalabilir; audit ve iş verisi kalmamalı.
10. Loglama kapalıyken iş sonucu ve rollback kapsamı aynı kalmalı; bu ekranların yazma yollarından `Olay_Table`/`SQLLog_Table` tablosuna yeni kayıt oluşmamalı. Başka ekranların eşzamanlı yazılarını ayrı tutun. Kontrollü SQL/audit erişim hatasında ham teknik metin yerine tek güvenli Türkçe hata mesajı beklenir.

## Kalan kapsam

Talep listelerinin onay/ret akışları, `IzinDonemListesiWP`, mahsup ve toplu izin ekranları henüz bu parçaya dahil değildir. Diğer bağlamsız API'ler ve eski ortak yazıcılar sonraki parçalar bitene kadar durur. Bütün solution için sıfır eski yazı hedefi henüz tamamlanmadı.

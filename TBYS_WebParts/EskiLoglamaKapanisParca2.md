# TBYS eski loglama kapanışı — parça 2/2

Tarih: 23.10.2026  
Dal: `codex/tbys-eski-log-kapanis`  
Başlangıç master: `bc772e528a31210ea7da28d167e640b671b6d569` (PR #546 sonrası).

Değişiklik öncesinde çalışma ağacı temizdi; yerel master ile canlı uzak master aynı commit'te doğrulandı. Bu dal kullanıcı tarafından istenen iki parçalık TBYS çalışmasının ikinci kod parçasıdır. Çalışma zamanı kabulü kullanıcıya aittir; kod kapanışı ile test/yayın kabulü ayrı değerlendirilmelidir.

## Eski yolların kapanışı

### Servis ve repository

- `Model/Services/TBYS/*Service.cs`: 81 transaction sahibi girişte bağlam varsa yeni, yoksa eski yazma yolunu seçen dallar kaldırıldı. İşlem açık bir log bağlamı gerektirir; yeni loglama ayarı kapalı olsa da aynı transaction yolu çalışır. Bağlamsız yazmaya veya OlayKayit'a dönüş yoktur.
- Dokuz eski servis yazma gövdesi ve üç özel Legacy ödeme metodu kaldırıldı. Kullanıcısı kalmayan dört servis yazma imzası kaldırıldı. KiraBorcuTakip ve SerhBeyanIrtifak ekranlarının hâlâ kullandığı beş tek parametreli giriş, eski gövdeleri korunarak bırakılmadı: mevcut transaction+context API'sine giden yeni transaction sahipleri olarak uygulandı.
- `DAO/Repositories/TBYS/*Repository.cs`: transaction almayan 90 eski yazma metodu kaldırıldı. 29 repository'nin DbClass alan kurucusu kaldırıldı; varsayılan okuma kurucuları TbysSqlExecutor'ın salt okuma uyumluluğunu kullanır.
- `DAO/Repositories/TBYS/TbysSqlExecutor.cs`: eski DbClass yönlendirmesi ve ayrı bağlantıda çalışan transaction'sız INSERT/UPDATE/DELETE yürütücüleri kaldırıldı. İş yazmaları yalnız SqlTransactionContext üzerinden yapılır. Bağlamlı okumaların SQL izleri korunur; diğer projelerin bağlamsız rapor okumaları eski log tablolarına dönmeden aynı veri sorgularını çalıştırabilir.
- Üç eski TBYS_SAVE_LOG/TBYS_UPDATE_LOG/TBYS_DELETE_LOG sabiti kaldırıldı. Modül adları, iş tanımları ve diğer projelerin bayrakları yalnız isme dayanarak silinmedi.
- KiraBorcuTakipService.Save/Update ile SerhBeyanIrtifakService.Save/Update/Delete artık aynı transaction içinde merkezi audit başlığı üretir. Bu girişler bağlamlı kurucu kullanmalarına rağmen önceki eski gövdelerden geçebiliyordu.
- Audit detay yazan 13 yardımcıya auditId==0 koruması eklendi. Gerçek audit hataları yutulmaz; kapalı loglama nedeniyle başlık üretilmemesi, iş transaction'ını bozmaz. Mevcut kişisel veri/alan politikaları topluca genişletilmedi.

### Gizli yardımcı yollar

`Model/TBYS/TBYSOrtak.cs` içindeki dokuz bağlamsız hesaplama/ödeme/teminat yardımcısı kaldırıldı. Etkin ekran çağrıları bağlamlı karşılıklarına taşındı.

`BakiyeBorcHesapla(..., context, transaction)` sözleşmeyi aynı transaction'da yeniden okur; aylık/günlük hesaplamanın bütün DB çağrıları o transaction'a gider. Aylık hesapta eksik yasal faiz oranı artık sessizce atlanmaz; mevcut sözleşme transaction'ı başarısız olur.

`AktifSozleslemelerinBakiyeBorcunuHesapla(context)` çağrıları BorcluKiracilar, BorcluKiracilarByBolge, MevcutKiracilar ve DevirGerekenSozlesmeler ekranlarına bağlandı. Toplu hesapta sözleşme başına işlem sınırı korunur; bir sonraki sözleşme başarısız olursa önce tamamlanan sözleşmeler kalır. Bütün toplu listenin tek transaction'da geri alınacağı iddia edilmez.

Teminat toplamlarının hesaplanıp sözleşmeye yazılması bağlamlı yola taşındı; transaction alan API mevcut sözleşmeyi aynı transaction'da yeniden okur. Ekranlar başarısız hesap sonucunda sessiz başarıyla devam etmez.

`Model/Ortak/UtilityHelper.cs` için bağlamlı BolgeGetir, UploadFileToSharePoint, DeleteFileFromSharePointLib ve GetFileNameListFromSharePointLib overload'ları eklendi. TBYS'nin 31 çağrısı bu yollara taşındı. Dosya hataları merkezi teknik yazıcıya gider; eski ExceptionHelper.PublishException çağrısına dönülmez. Diğer projelerin eski overload'ları kendi geçişleri için korundu.

`TasinmazResimleriWP.BildirimEPostasiGonder` bağlamlı parametre okuması ve MailHelper.EPostaGonderVeyaHataVer kullanır. SMTP hatası eski loga gitmez; tamamlanmış resim/belge işi için Türkçe bildirim uyarısı gösterilir. Son kullanıcıya ham istisna verilmez.

## İşlem ve audit düzeltmeleri

### Otomatik ve manuel bakiye devri

`Model/TBYS/KiraBakiyeDevriOtomatik.cs::DevirAl` yeniden düzenlendi:

1. Güncel sözleşme, önceki sözleşme ve planlar aynı transaction'da okunur.
2. Sözleşmenin devir tutarları ve Sira==0 plan satırı karşılaştırılır; yalnız gerekenler değiştirilir.
3. Kiracının plan hesaplamaları da aynı transaction ile yapılır.
4. Bütün iş ve audit kayıtları birlikte tamamlanır veya geri alınır.

Daha önce manuel çağrıda true verilen atlaBayrakKontrolu parametresi koşulsuz false döndürüyordu. Modelde bu açıklamaya karşılık gelen kalıcı otomatik devir bayrağı yoktu. İşlevsiz parametre kaldırıldı; kaynakta mevcut iki manuel devir ekranı aynı bağlamlı işlem yolunu kullanır. Yeni bir veritabanı alanı veya iş bayrağı uydurulmadı.

Eski OtomatikDevirOlayKaydet ve doğrudan OlayService.Save kaldırıldı. Karşılığı KiraSozlesme UPDATE audit'i ve üç devir alanının eski/yeni detaylarıdır. Aynı iş için ayrıca eski olay veya ikinci özel audit başlığı üretilmez. Plan hesabının değiştirdiği farklı plan satırları kendi audit'lerini üretir.

Eksik devir satırı başarısız işlemdir. Sözleşme ile planın yalnız bir tarafı farklıysa bu taraf düzeltilebilir; fark yoksa yeni devir yazısı üretilmez. Önceki sözleşme/plan yoksa işlem atlanır.

Toplu devirde sözleşme başına commit sınırı korunur; teknik hatalar merkezi sisteme yazılır, sonuç sözlüğüne ham ex.Message konulmaz. Tamamlanan, atlanan ve başarısız sözleşmeler ayrıdır. UI çağrılarındaki koşulsuz atlama kaldırılmıştır.

### Toplu silme ve bölge güncellemesi

- `TeminatIslemService.DeleteBySozlesmeId` ve `HukukiTakipService.DeleteBySozlesmeId`: satırlar UPDLOCK/HOLDLOCK ile okunur; silme ve gerçek satır kimliği başına audit aynı transaction'dadır. Boş kümede hayali audit yazılmaz.
- Yeni teminat transaction okumasında mevcut model/sorgulardaki IslemTarihi alanı kullanılır; eski bağlamsız sorgudaki TeminatIslemTarihi sıralaması yeni silme yoluna taşınmadı.
- `KiraSozlesmeService.UpdateByKiraciId`: kilitli sözleşmeler aynı transaction'da okunur; yalnız bölgesi değişenler normal Update/audit yoluyla yazılır.
- KiraSozlesme audit izin listesine üç devir, üç teminat toplamı ve BolgeId eklendi. Bunlar yukarıdaki işlerin önceki audit kapsamı boşluklarını kapatır.

### Mevcut TransactionScope blokları

`OdemeAyristirmaWP.OdemeAyristirmaAtomicKaydet` ve `KiraSozlesmesiWP.DosyaNoDegistirNowBtn_Click` açık SqlTransactionContext'e geçirildi. İçeride çağrılan servisler kendi yerel transaction'larını açmaz.

Ödeme ayrıştırmada ödeme/teminat/ayrıştırma/ekstre yazıları, ekstre ve sözleşme/plan okumaları aynı transaction'dadır. Ekstre kilit altında yeniden okunur; zaten aktarılmış veya boş ayrıştırma reddedilir. Hata veya audit hatasında tüm bu blok geri alınır.

Dosya numarası değiştirmede liste okuması, güncel sözleşme okuması, değişiklik ve audit aynı transaction'dadır. Aynı dosya numarasındaki satırlar tekrar güncellenmez; arayüz sonucu commit sonrasında değiştirilir.

Bu parça uygulamadaki her çok adımlı iş akışını yeni tek bir büyük transaction'a dönüştürmez. Önceden bağımsız transaction'larla çalışan sözleşme yenileme, bazı teminat/ekstre ve SharePoint dosya+SQL işlemlerinin bu sınırları genel olarak korunur. SQL transaction'ı SharePoint dosyasını veya SMTP gönderimini geri almaz. Buradaki kapanış, eski log yazma yollarının kaldırılması ve işlem başına iş/audit transaction'ının korunmasıdır.

## Ekran ve SharePoint bağlantıları

TBYS projesinde ayrı bir eski olay/SQL log listeleme WebPart'ı veya buna bağlı feature/paket öğesi saptanmadı. Bu nedenle ekran/feature/paket silinmedi. TBYS iş ekranları korunmuştur. Üretilen ASCX ve ASCX kaynakları değiştirilmedi; kod üretimi gerekmiyor.

Ortak Portal/MTS eski log ekranları ve yeni sistemdeki tarihsel audit/SQL sorgulama karşılıkları, ilgili projelerin aşamasında ele alınacaktır. Yeni izleme ekranının kullanıcı hareketi/hata işlevlerinin bulunması tek başına eski audit/SQL listeleme gereksinimlerinin tamamlandığı anlamına gelmez.

## Statik doğrulama

- 80 değişen C# dosyasında Roslyn ParseText/GetDiagnostics: sıfır sözdizimi hatası. Semantik derleme veya emission yapılmadı.
- Solution genelinde taranan 870 TBYS servis çağrısında çözülemeyen argüman sayısı yok. Servis/repository imza ve çağrı taraması temiz.
- Yerel metot çağrıları ve TBYSOrtak statik çağrıları kontrol edildi; yeni yardımcı overload'ları kaynak üzerinden doğrulandı.
- 351 transaction parametreli metot ve bunlarda saptanan 428 DB çağrısı aktarımı incelendi: eksik transaction argümanı yok.
- Başlangıçta da bulunan 303 repository SQL/parametre sözleşmesi karşılaştırıldı: fark yok. Yeni transaction okumaları ayrıca incelendi.
- 80 kaynak dosyasında katı UTF-8, CRLF, başlangıç BOM durumu ve C# Compile kayıtları doğrulandı. Değişen 19 WebPart kod dosyasının 207 ASCX olay bağlantısında eksik handler yok.
- TBYS kaynak kapsamındaki eski Olay/SQLLog/DbClass yazma bağımlılıkları ve bayraklar tarandı: etkin kaynak başvurusu kalmadı. TBYS'den erişilen 26 UtilityHelper imzası ayrıca incelendi; erişilen modern yollarda eski hata yayını kalmadı.
- Merkezi Utility/Logging ve DAO/Ortak transaction dosyalarında; ASCX/g.cs, csproj, feature/paket dosyalarında değişiklik yok. git diff --check temiz.
- Metinsel çağrı ve transaction taramaları semantik derleme/çalışma zamanı kanıtı değildir. Build, test ve debug Codex tarafından Windows'ta çalıştırılmadı.

## Kullanıcının uygulayacağı kabul senaryoları

1. Visual Studio 2026'da bütün solution'ı derleyin, SharePoint paketleyip Test sunucusunda yayımlayın. Kaldırılan overload/kurucuların tüm kaynak çağrılarını ve olası solution dışı entegrasyonları doğrulayın.
2. TBYS proje ve WebPart log ayarlarının açık/kapalı birleşimlerinde temel CRUD ve hesaplama işlemlerini tekrarlayın. İşlem transaction'ları kapalı ayarda da çalışmalı; eski loga dönüş olmamalı.
3. İzole TBYS trafiğinde Olay_Table/SQLLog_Table başlangıç-son kimlik/tarih/satır farkını ve yazar kaynağını izleyin. Diğer projelerin eşzamanlı eski yazmalarını bu sonuçla karıştırmayın.
4. Borç takip kaydı ve taşınmaz şerh/beyan/irtifak ekle/güncelle/sil: merkezi audit başlığı aynı iş transaction'ında olmalı. Audit hatasında ilgili yazı geri alınmalı.
5. Manuel devir ekranlarının ikisinde de gerçek fark, fark yok, yalnız sözleşme veya yalnız plan farkı, eksik önceki plan ve eksik devir satırını deneyin. Her işlemde devir ve hesaplanan planlar birlikte tutarlı olmalı.
6. Devir hesabının ortasında SQL/audit hatası oluşturun: mevcut sözleşmenin devir ve plan yazıları birlikte geri alınmalı. Eski Otomatik Devir olayı yazılmamalı; yeni audit devir alanlarını içermeli.
7. Toplu devirde bir sözleşmeyi kontrollü olarak başarısız kılın: önce tamamlananlar korunmalı, sonuçlar doğru ayrılmalı, ham hata metni görünmemeli. Kalanların/başarısızların tekrar işlenmesini doğrulayın.
8. Toplu aylık/günlük borç hesaplama: resmî oranlar, eksik yasal faiz ve farklı tarihler. Sözleşme başına rollback sınırını doğrulayın; önce tamamlanan sözleşmelerin kalması beklenir.
9. Teminat toplamı hesaplama, teminat/hukuki takip toplu silme ve tekrar silme: gerçek satır başına tek audit, boş kümede hayali audit olmamalı. Audit hatasında silme geri alınmalı.
10. Kiracı bölgesi değişikliği: ilgili değişen sözleşmeler audit'li güncellenmeli; aynı bölgeye ikinci işlem ek sözleşme UPDATE/audit üretmemeli.
11. Ödeme ayrıştırmada birden çok kira/teminat satırı, eksik plan/sözleşme, boş liste, aktarılmış ekstre ve eşzamanlı ikinci işlem. Bir adım/audit hatasında ödeme, teminat, ayrıştırma ve ekstre birlikte geri alınmalı.
12. Toplu dosya numarası değiştirmede ara hata ve aynı numaraya tekrar işlem: tüm blok rollback olmalı; kullanıcıya commit öncesi başarı gösterilmemeli.
13. SharePoint belge yükleme/silme/listeleme hataları ile resim/belge SMTP hatasını tetikleyin. Eski hata tablosuna yazılmamalı; Türkçe genel mesaj/başvuru kodu ve tamamlanan iş için doğru bildirim uyarısı görünmeli.
14. TBYS raporları/Excel/şema ve bölge filtreleri; NBYS'nin TBYS taşınmaz/bağış okumaları ile MTS bağışçı raporunu kontrol edin. Bağlamsız okuma uyumluluğu korunmalı.
15. Kira sözleşmesi yenileme/fesih/silme, ödeme/plan, banka ekstresi ve teminat akışlarını iş sonucu ve audit açısından regresyon testinden geçirin. Yukarıda açıklanan mevcut çok adımlı transaction sınırlarını dikkate alın.
16. Gerçekçi sözleşme/plan sayısıyla süre, kilit bekleme ve deadlock kontrolü yapın. SQL/hata izleri bağımsız kalırken geri alınmış iş için kalıcı audit bulunmamalı; aynı exception gereksiz tekrar kaydedilmemeli.

## Eski tablolar ve sonraki aşama

Veritabanına bağlanılmadı; tablo, geçmiş veri, şema veya indeks değiştirilmedi/silinmedi. Diğer projeler ve dış bağımlılıklar bitmeden eski tablolara global yazma engeli veya DROP uygulanmamalıdır.

DBA ile procedure/view/function/trigger/job, dinamik SQL, dış uygulama ve rapor okuyucu/yazıcı envanteri çıkarılmalı; tüm yazarlar taşındıktan sonra gözlem dönemi yapılmalıdır. Kurumun saklama politikasına uygun salt okunur arşiv; satır sayısı, tarih/kimlik aralıkları, uygun checksum ve Test ortamında geri yükleme doğrulamasıyla hazırlanmalıdır. Kaldırma ancak bağımlılıklar ve saklama/geri dönüş süreleri bittikten sonra ayrı onaylı DBA işlemi olabilir.

Bu iki parça TBYS'nin kaynak kod geçişini kapatır; çalışma zamanı kabulü yukarıdaki kullanıcı testlerinden sonra değerlendirilir. Sıradaki proje NBYS'dir. NBYS'ye kullanıcı kabulü veya açık erken devam isteği olmadan başlanmadı.

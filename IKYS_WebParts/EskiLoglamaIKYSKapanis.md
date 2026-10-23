# IKYS eski loglama geçişi — kapanış parçası

Tarih: 23.10.2026  
Dal: `codex/ikys-eski-log-kapanis`  
Başlangıç master commit'i: `9e750765c0dd2a6d0164fdfdb658b958cd9a5b26`

Değişiklikten önce çalışma ağacı temizdi; yerel master ile canlı uzak master aynı commit'te doğrulandı. Kullanıcının IKYS'nin kalanını tek seferde tamamlama isteği üzerine bu kapsam tek dalda hazırlandı. Commit, push, PR, merge, build, debug, yayın ve çalışma zamanı testleri yapılmadı.

## Kapsam ve inceleme noktaları

| Katman | Son davranış / kaynak |
| --- | --- |
| DAO | `DAO/Repositories/IKYS/*Repository.cs`: bağlam alan okumalar doğrudan merkezi SQL yürütücüsüne gider. Eski okumayı çağıran sarmalayıcılar kaldırıldı; mevcut sorgu/parametre sözleşmeleri korundu. Kullanılmayan eski yazma overload'ları kaldırıldı. |
| Model/services | `Model/Services/IKYS/*Service.cs`: ekranlara açık bağlamlı okuma API'leri tamamlandı. İş yazıları ile audit aynı transaction'da; kullanılmayan bağlamsız yazma API'leri kaldırıldı. |
| WebParts | 45 ekranın kod dosyasında servis çağrıları açık log bağlamına geçirildi; log ayarına bağlı eski okuma/yazma/hata yayını dönüşleri kaldırıldı. SQL izi ve teknik hatalar mevcut bağımsız merkezi yazıcıları kullanır. |
| Ortak yardımcılar | `Model/IKYS/IKYSOrtak.cs`: izin hesabı, hiyerarşi, personel ve bildirim okumaları için bağlamlı yollar; transaction verilen izin hesaplarında aynı transaction. Bildirim hataları kaydedilmiş işi geri alınmış gibi göstermez. |
| Model nesneleri | `PersonelItem`, `IsBilgileriItem`: IKYS bağlamı lazy okumalara aktarılır; alan serialize edilmez. Diğer projelerin kullandığı bağlamsız uyumluluk yolları korunur. |
| Temizlik | Kullanıcısı bulunmayan 89 servis yazma, 90 repository yazma ve 14 yardımcı overload kaldırıldı. `PersonelItemListFactory.cs` ve `Model.csproj` kaydı, üç eski IKYS log bayrağı kaldırıldı. |

### Transaction ve audit

- `IKYS_WebParts/UcretliMahsupWP/UcretliMahsupWP.ascx.cs::MahsupIsleminiyap`: hareket, mahsup ve iki dönemin bakiyeleri tek transaction'da. Hareket kilit altında yeniden okunur; aynı dönem, farklı personel/tür veya eksik dönem reddedilir. Manuel telafi yazıları kaldırıldı.
- `TopluIzinGirisiWP.ascx.cs::TopluIzinGir / YeniIzinHareketiKaydet`: mevcut **personel başına transaction** sınırı korunur. Süre/tatil/dönem okumaları, dönem oluşturma, bakiye ve hareket aynı transaction'dadır. Başarı sayacı commit/Dispose sonrasında artar. Başarısız kişiler ayrı bildirilir; tamamlananlar kalır, yalnız başarısız kişiler yeniden denenmelidir.
- `MaasOlusturmaWP.ascx.cs::TabloyuKaydet`: maaş için kullanılan snapshot okumaları, derece/kademe ve maaş yazıları tek transaction'da. Aynı tarih için mevcut kayıt kilit altında kontrol edilir; boş liste kaydı reddedilir.
- `MaasOlusturmaWP.ascx.cs::TabloyuSil` ve `MaasHareketService.DeleteByGrupId`: grup okuma/silme ve silinen her satırın kendi kimliğiyle audit'i aynı transaction'da.
- `ProtokolSirasiWP.ascx.cs`: log ayarından bağımsız tek transaction.
- `GorevOnayService.SetSelectedIds / SetPaidIds`: seçim sıfırlama+seçme veya ödeme işaretleme birer transaction'da; yalnız değişen satırlar normal audit yoluyla güncellenir.
- `GorevTanimEditWP.ascx.cs`: modern servisin transaction içinde yaptığı atamaları log kapalıyken yeniden yapan ikinci yazma yolu kaldırıldı.
- 17 servisin `RecordChanges` kapsamına 110 eksik iş alanı eklendi. Mevcut audit başlıkları korunur; alan detayları genişletilir. Merkezi 2000 karakterlik değer sınırı değişmedi; sınırı aşan kayıt audit açıkken transaction'ı geri alabilir, sessiz kırpma yapılmaz.
- `SqlTransactionContext`, merkezi teknik hata/SQL yazıcıları ve ortak log repository tasarımı değiştirilmedi. Log kapalı olması iş işleminin transaction'sız çalışmasına yol açmaz; eski sisteme dönüş yapılmaz.

### Ekranlar, raporlar ve bildirimler

- `BirimSemasiWP`, `GorevSemasiWP`: modern toplu okuma/cache yolu log ayarından bağımsızdır.
- `EskiPersonelListesiWP.GetDataList`: JSON listesi, Excel ile aynı bağlamlı tablo sorgusundan üretilir. Singleton fabrika ve paralel lazy okumalar kaldırıldı; ekran adı işlev gereği korunur.
- Personel kartı/listeleri, kişisel sayfalar, yoklama, izin raporları, iletişim, maaş tabloları ve görev ekranlarının okumaları bağlamlıdır.
- `IKYSOrtak.IzinTalepOlusturmaEPostasiGonder / GorevOnayEPostasiGonder / BilgiSistemMailGrubunaEPostaGonder` bağlamlı yolları merkezi hata kaydı ve Türkçe sonuç bildirir; iş kaydından sonraki SMTP hatası kaydın başarısız olduğu şeklinde sunulmaz.
- `Response.End` kaynaklı ThreadAbort hata kaydı olarak tekrar yazılmaz.
- IKYS içinde kaldırılacak ayrı eski olay/SQL log WebPart'ı saptanmadı. Bu nedenle ASCX, üretilen ASCX, feature veya paket kaldırılmadı. Eski personel ekranı log ekranı değildir.
- Ortak eski olay/log ekranları ve yeni sistemin audit/SQL sorgulama karşılıklarının tamamlanması Portal/MTS aşamalarında ayrıca ele alınmalıdır; mevcut yeni izleme ekranının kullanıcı hareketi/hata işlevleri tüm tarihsel audit/SQL listeleme gereksinimlerinin tamamlandığı anlamına gelmez.

## Kalan ortak bağımlılıklar

Bu parça bütün solution'ın kapanışı değildir. TBYS/NBYS vb. projelerin kullandığı bağlamsız personel/bölge okuma uyumluluğu, ortak DbClass, OlayService/OlayRepository, diğer proje bayrakları ve eski tablolar korunmuştur. Eski IKYS modül adı sabitleri ortak geçmiş filtrelerinin bağımlılıkları bitmeden silinmedi; yetki birimi sabiti de iş kuralıdır.

IKYS'nin etkin WebPart/servis/repository kaynaklarında Olay_Table, SQLLog_Table, OlayKayit, PortalLogSettings veya üç eski IKYS yazma bayrağına başvuru kalmadı. Bu kaynak bulgusu, diğer projelerden gelen trafiğin eski tablolara yazmayacağını garanti etmez. Personel resim yardımcılarındaki ExceptionHelper nesnesi sonuç toplamak için kalır; ilgili yol PublishException eski log yayınına gitmez.

## Statik doğrulama

- 105 mevcut değiştirilmiş C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi yönünden incelendi: sıfır hata. Derleme/emission/test yapılmadı.
- Servis/repository overload ve çağrı argüman sayısı taraması: sıfır sorun; 1901 yerel metot çağrısında çözülemeyen argüman sayısı yok.
- UI/model çağrılarında açık bağlam kontrolü yapıldı. Maaş artışındaki üç Writer çağrısı bağlamı constructor'da alan mevcut SalaryScaleWriteService kullanımıdır.
- 301 transaction parametreli metot tarandı; doğrudan repository/servis aktarımı saptanan 103 çağrıda eksik transaction argümanı bulunmadı. Bu metinsel tarama bütün programın semantik doğruluğunun kanıtı değildir; karmaşık iş akışları ayrıca kaynak üzerinden incelendi.
- 149 okuma SQL/parametre sözleşmesi başlangıç sürümüyle karşılaştırıldı: değişen sözleşme yok.
- 106 mevcut değiştirilmiş kaynak/proje dosyasında katı UTF-8, BOM korunumu, CRLF ve C# Compile proje kayıtları doğrulandı; 221 ASCX olay bağlantısında eksik handler bulunmadı.
- IKYS kapsamındaki NOLOCK, eski log başvuruları ve kaldırılan fabrika/flag çağrıları tarandı. Merkezi log/transaction kodunda ve üretilen ASCX/feature/paket dosyalarında değişiklik yok.
- git diff --check temiz. Bu kontroller C# semantik derlemesi, SharePoint paketlemesi veya çalışma zamanı kabulünün yerine geçmez.

## Kullanıcının uygulayacağı kabul senaryoları

Aşağıdaki senaryolar Test sunucusunda, ilgili proje/WebPart log ayarlarının açık ve kapalı birleşimleriyle tekrarlanmalıdır. Kapalı ayarlarda merkezi kayıtların ayarlara göre azalması beklenir; eski tabloya dönüş veya iş işleminin bozulması beklenmez.

1. Visual Studio 2026'da solution derleme ve SharePoint paketleme/yayın. Kaldırılan overload'lar, kaynak üretimi ve referanslar için tüm projelerin semantik derlemesini doğrulayın. Üretilen ASCX dosyalarını elle düzeltmeyin.
2. İzole IKYS trafiğinde Olay_Table/SQLLog_Table öncesi-sonrası kimlik/tarih/satır değişimini izleyin. Aynı anda diğer proje trafiğinin sonucu kirletmesini engelleyin; SQL Extended Events/izleme ile yazar kaynağını ayırın.
3. Personel ve alt kayıtlar: ekle/değiştir/sil, iş/kimlik/aile/eğitim/tecrübe/kurs/dil/iletişim. İş verisiyle audit başlık+detay aynı commit'te; kullanıcı hareketi/SQL/hata türlerinde beklenmeyen çift kayıt yok.
4. Audit yazma hatasını kontrollü ortamda tetikleyin: ilgili iş yazıları geri alınmalı; teknik hata/SQL izi mevcut bağımsız tasarım gereği kalabilir. Log kapalıyken iş normal transaction ile tamamlanmalı.
5. Ücretli mahsupta başarılı aktarım; eksik/farklı personele ait/aynı hedef dönem ve ikinci tıklama. Hareket, mahsup, iki bakiye birlikte tutarlı; ara yazma hatasında hiçbiri kısmen kalmamalı.
6. Toplu izinde tümü/seçilenler, farklı cinsiyet ve izin türleri, hafta sonu/resmî tatil, eksik dönem, ters tarih. Bir kişide zorlanmış hata: o kişinin tüm yazıları geri alınmalı; başarılı kişiler kalmalı; sonuç doğru kişi listesini vermeli. Tüm grubu yeniden çalıştırmadan yalnız başarısızları deneyin.
7. Maaşta görüntüle/Excel/kaydet/sil; aynı tarihe tekrar ve eşzamanlı kaydetme; boş liste; ara satırda hata. Derece/kademe ve maaş grubu birlikte tamamlanmalı/geri alınmalı. Silme audit'i her gerçek maaş kimliğini içermeli.
8. Protokol sırası ve görev atamalarında ara hata/eşzamanlı istek: kısmi sıralama/atama ve log kapalıyken ikinci yazma olmamalı.
9. Görev onay listesinde sıfırla/seç/öde, aynı kimliği tekrar gönderme, zaten ödenmiş kayıt: yalnız gerçekten değişen kayıt audit'i oluşmalı.
10. İzin talep/onay/iptal, dönem ve hareket düzenleme, mazerete mahsup, yoklama/tatil/harcırah ve tanım ekranlarında önceki iş sonuçları korunmalı.
11. Birim/görev şemaları, personel/eski personel JSON ve Excel listesi, iletişim ve tüm izin raporlarında filtreler, yetki birimleri, boş sonuç ve cache yenileme. Log kapatmak veri içeriğini değiştirmemeli.
12. Kişisel sayfa/personel kartı lazy alanları ve resim yükleme; kullanıcıya ham istisna gösterilmemeli. Excel indirmesinde ThreadAbort teknik hata olarak çoğalmamalı.
13. Talep/görev/bilgi sistem bildiriminde geçerli alıcı, eksik alıcı ve SMTP hatası: kaydedilmiş iş korunmalı, Türkçe bildirim doğru olmalı, merkezi hata tekrar üretilmemeli.
14. Uzun açıklama/not ve tarih-saat değişikliği: genişletilen audit detaylarını, hassasiyetini ve mevcut 2000 karakter sınırındaki rollback davranışını doğrulayın.
15. TBYS/NBYS'nin paylaşılan IKYS personel/bölge okuma ekranlarında regresyon kontrolü; gerçekçi personel sayısıyla toplu izin/maaş/şema sorgularında süre, kilit bekleme ve deadlock değerlendirmesi.

## Eski tablolar için ayrı öneri

Veritabanına bağlanılmadı; tablo, geçmiş veri, trigger, indeks veya şema değiştirilmedi. Diğer projeler kapanmadan ortak tablolara global yazma engeli/drop uygulanmamalıdır.

DBA ile veritabanı bazında tablo bağımlılıkları; procedure/view/function/trigger, job, dinamik SQL, dış uygulama ve rapor kullanıcıları çıkarılmalıdır. Kaynak taraması bu dış bağımlılık envanterinin yerine geçmez. Tüm projeler ve dış yazarlar taşındıktan sonra kontrollü gözlem döneminde son yazma/okuma kaynakları doğrulanmalıdır.

Kurumun saklama politikasına göre salt okunur arşiv alınmalı; satır sayısı, tarih/kimlik aralıkları ve uygun checksum kontrolleri kaydedilmeli, geri yükleme DBA tarafından Test ortamında denenmelidir. Arşiv erişimi mevcut yetki ve veri gizliliği kurallarını korumalıdır. Yalnız bütün bağımlılıklar bittikten, saklama ve geri dönüş süresi dolduktan sonra ayrıca onaylanan DBA değişikliğiyle kaldırma değerlendirilebilir. Bu parça DROP veya veri silme betiği içermez.

Sonraki proje TBYS'dir; kullanıcının yayın/kabul değerlendirmesi veya testten önce açık devam isteği sonrasında başlanmalıdır.

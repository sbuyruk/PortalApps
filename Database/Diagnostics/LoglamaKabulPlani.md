# Loglama geçişi — envanter ve kabul kaydı

Hazırlık tarihi: 2026-10-25. Dal: `codex/loglama-kabul-envanteri`.
Başlangıç: `4b632b1d4ce383ad386ce8468f93946303af0cb3` (PR #556).
Yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı.

Bu dal yalnızca salt okunur SQL envanteri ve bu kontrol listesini ekler. Uygulama kodu, paketler, ayarlar, şema ve geçmiş veriler değişmez. Codex build/test/debug, yayım veya veritabanı sorgusu çalıştırmadı. Aşağıdaki sonuçlar henüz **bekliyor**; bu dosyanın hazırlanması kabul sayılmaz.

## 1. Test ortamını kaydedin

Kullanıcı Visual Studio 2026'da PR #556'yı içeren master'ı derler; Test sunucusunda ilgili tüm güncel paket/assembly'lerin yayımlandığını doğrular. Bu doküman dalı için yeni uygulama derlemesi gerektiren kod değişikliği yoktur.

| Alan | Kaydedilecek bilgi |
| --- | --- |
| Uygulama commit'i | Bekliyor |
| Derleme / yayım tarihi ve sonucu | Bekliyor |
| Test URL'si / sunucu(lar) | Bekliyor |
| Uygulamanın sunucu / veritabanı adı | Bekliyor |
| Test kullanıcısı ve test veri kimlikleri | Bekliyor |
| Mevcut iki PortalLogging ayarının güvenli kopyası | Bekliyor |
| Testi yapan / değerlendiren | Bekliyor |

Bağlantı parolalarını veya tam connection string'i kabul kaydına koymayın. Birden fazla uygulama DB'si/sunucusu varsa envanteri her birinde ayrı alın.

## 2. Önce bağımlılık envanterini alın

SSMS'de uygulamanın **Test veritabanını** seçip [EskiLogBagimlilikEnvanteri.sql](EskiLogBagimlilikEnvanteri.sql) dosyasını çalıştırın. Betik migration değildir; uygulama otomatik çalıştırmaz. Metadata sorguları veri/nesne/yetki değiştirmez. 10. bölümde dönen SELECT metinleri de otomatik çalıştırılmaz.

Sonuç kümelerini bölüm adlarıyla kaydedin:

| Bölüm | Değerlendirme |
| --- | --- |
| 01 | Doğru sunucu/DB mi; DB ve sunucu metadata görünürlüğü yeterli mi? |
| 02 | İki eski tablo hangi şemada; yaklaşık hacim ve identity durumu nedir? Tablo görünmüyorsa yokluğu ile yetki eksikliğini ayırın. |
| 03–04 | Doğrudan/dolaylı nesne çağıranlarını inceleyin. Her nesneyi okuma/yazma/kullanılmayan olarak, tanımını ve gerçek çağrı yolunu okuyarak sınıflandırın. |
| 05–06 | Dinamik SQL ad eşleşmeleri, trigger, FK ve synonym'ler. Synonym adını kullanan çağıranları ayrıca izleyin. |
| 07 | Yazma yetkisi bulunan hesap/rolleri belirlemek için girdi. Bu liste etkin yetki hesabı değildir; sahiplik, rol zincirleri ve sunucu haklarını DBA değerlendirmelidir. Yetki değiştirmeyin. |
| 08 | Şifreli/görünmeyen tanımlar, CLR modülleri ve sunucu trigger'ları ayrıca incelenmeli. |
| 09 | Job adımlarının hedef DB'si, etkinliği ve gerçek komutu DBA tarafından incelenmeli. Komut metinleri betikte gösterilmez. |
| 10 | Kontrollü testten önce/sonra kullanılabilecek kesin sayım SELECT'leri. Büyük tabloda tam tarama maliyetini değerlendirin. |

Boş sonuç tek başına bağımlılık yok demek değildir. `VIEW DEFINITION`, sunucu görünürlüğü veya msdb erişimi yetersizse envanter **eksik** olarak işaretlenir. SQL Agent rolü yalnızca bazı job'ları gösterebilir; erişim hatası olmaması tamlık kanıtı değildir. DBA tüm job'ların kapsandığını doğrulamalıdır.

Metin taraması parçalı ad üretimi (`'Olay_' + 'Table'`), şifreli tanım veya dış dosyadaki komutu bulamayabilir. Yerel metadata zinciri en fazla 16 kenardır; sınırdaki nesnelerin devamı ayrıca incelenir. Diğer DB/sunuculardan gelen başvurular, synonym zincirleri, dış ETL/SSIS, raporlar, PowerShell/CmdExec dosyaları, zamanlayıcılar ve dağıtılmış eski assembly'ler ayrıca envantere eklenir.

| Bağımlılık / kaynak | Okuma mı yazma mı? | Sahibi | Gereklilik / yapılacak iş | Durum / kanıt |
| --- | --- | --- | --- | --- |
| Portal/MTS geçmiş arşivi | Okuma | Uygulama | Geçmiş erişimi korunacak | Kaynakta mevcut; runtime bekliyor |
| SQL nesneleri / trigger / synonym | İncelenecek | Belirlenecek | 03–08 sonuçlarına göre | Bekliyor |
| SQL Agent / ETL / rapor / dış uygulama | İncelenecek | Belirlenecek | DBA ve sistem sahipleriyle tamamlanacak | Bekliyor |

## 3. Eski tablolarda gözlem başlangıcını kaydedin

10. bölümde üretilen salt okunur sorguları uygun bir Test zamanında çalıştırıp UTC zamanını, tabloyu, kesin satır sayısını ve varsa en büyük Id'yi kaydedin. Aynı sorguları aşağıdaki her test grubundan sonra aynı DB'de tekrarlayın. Tabloya deneme satırı eklemeyin; eski satırları temizlemeyin.

| Test grubu / başlangıç–bitiş UTC | Tablo | Önce sayı / max Id | Sonra sayı / max Id | Fark / inceleme |
| --- | --- | --- | --- | --- |
| Bekliyor | Olay_Table | Bekliyor | Bekliyor | Bekliyor |
| Bekliyor | SQLLog_Table | Bekliyor | Bekliyor | Bekliyor |

Beklenti sıfır yeni kayıttır. Artış kalmış bir yazıcıyı gösterir. Sabit count/max, eşzamanlı silme/yeniden ekleme veya geri alınmış girişimleri tek başına dışlamaz; bu yüzden kontrollü zaman aralığı, dış yazıcı envanteri ve gerektiğinde DBA'nın mevcut izleme kanıtıyla birlikte değerlendirilir. Bu hazırlık yeni SQL Audit/Extended Events oturumu kurmaz.

## 4. Proje sırasıyla kabul testini yapın

`PortalLogging.EnabledProjects` ve `PortalLogging.EnabledWebParts` birlikte değerlendirilir; kaynak `PortalLogSettings.IsEnabled` içinde OR kullanır. Bir projeyi kapatma testinde hem proje girdisini hem o projeye ait WebPart girdilerini devreden çıkarın. Diğer projeleri koruyun, ilk değerleri kaydedin ve test sonunda geri yükleyin. Ayar değişikliğinin Test'teki tüm ilgili uygulama örneklerine uygulandığını doğrulayın.

Her proje için önce normal log açık, sonra kontrollü hata/rollback, son olarak log kapalı işlem çalıştırın. Bir projeyi değerlendirdikten sonra sıradakine geçin.

| Proje | Temsilî işlem | Log açık | Hata / rollback | Log kapalı | Kanıt / OperationId |
| --- | --- | --- | --- | --- | --- |
| IKYS | Personel/izin talebi/görev onayı ve bildirim | Bekliyor | Bekliyor | Bekliyor | Bekliyor |
| TBYS | Ödeme/ayrıştırma ve belge yükleme-silme | Bekliyor | Bekliyor | Bekliyor | Bekliyor |
| NBYS | Bağış/armağan ve içe aktarma sonucu | Bekliyor | Bekliyor | Bekliyor | Bekliyor |
| Portal | Duyuru/toplantı ve bildirim | Bekliyor | Bekliyor | Bekliyor | Bekliyor |
| MTS | Kişi/faaliyet/katılım/anı objesi ve takvim | Bekliyor | Bekliyor | Bekliyor | Bekliyor |
| MFYS | Dört raporda dolu/boş sonuç ve filtreler | Bekliyor | SQL hata senaryosu bekliyor; iş rollback'i uygulanmaz | Bekliyor | Bekliyor |

Her satır için kabul koşulları:

- Log açık: iş sonucu doğru; ilk sayfa açılışı için beklenen kullanıcı hareketi var. Her gerçek SQL çalıştırması için beklenen iz var. İş audit'i değişen ilgili kayıtlarla aynı transaction'da commit olur.
- Rollback: hedef iş kaydı ve aynı transaction'daki audit kalmaz; teknik SQL/hata kayıtları bağımsız tasarımla korunur. İlgisiz önceden kaydedilmiş audit satırlarını bu işlemin sonucu saymayın.
- Log kapalı: iş işlemi/rapor devam eder; ilgili bağlam için yeni merkezi hareket/audit/SQL/hata kaydı beklenmez. Eski tabloya geri dönüş olmamalıdır.
- Her durumda: Olay_Table ve SQLLog_Table'a yeni satır yazılmamalı. Kullanıcıya ham exception/SQL gösterilmemeli.
- Hata senaryosu: aynı arıza için tek güvenli bildirim ve beklenen merkezi hata ilişkisi olmalı. Bir istek birden çok SQL ve birden çok iş kaydı değiştirebilir; OperationId başına tek satır beklemeyin. Çift kayıt incelemesini olay, ilgili kayıt ve gerçek komut sayısıyla yapın.

Özellikle son iki parçanın regresyonları: IKYS AD erişim hatası; bildirim sırasında ortak parametre SELECT hatası; TBYS bildirimleri/dosyalar; NBYS sonuç sayaçları. Gerçek AD yokluğu ile AD erişim arızasını ve eksik parametre ile başarısız parametre sorgusunu ayırın. Hata, ikinci null hatasına veya yanlış SMTP devamına dönüşmemeli. Ayrıntılar [son kapanış notunda](../../EskiLoglamaSonKapanis.md).

## 5. Geçmiş erişimini doğrulayın

Portal/MTS arşivlerinde kayıtlı ve kayıtsız günleri, program/tarih filtrelerini ve silinmiş ilişkili iş verilerini kontrol edin. Yeni işlem geçmiş arşivinde görünmemeli; uygun merkezi kayıt Log İzleme'de görünmeli. Arşiv okumak eski veriyi değiştirmemeli veya merkezi audit'e yeniden eklememeli.

MTS TestWP eski sayfa örneğinde yalnızca devre dışı bildirimi beklenir; örnek e-posta veya timer çalışmamalı. Mevcut galeri öğesi önceki metadata güncellemesini almamışsa farm envanterinde ayrıca kaydedin; tüm feature'ı kaldırmayın.

## 6. Kabul ve arşivleme kararını ayırın

| Karar | Sonuç |
| --- | --- |
| Altı proje çalışma zamanı kabulü | Bekliyor |
| Eski tablolara sıfır yeni kayıt gözlemi | Bekliyor |
| Dış bağımlılık envanteri ve sahip onayı | Bekliyor |
| Saklama süresi / salt okunur arşiv erişim sorumlusu | Belirlenmedi |
| Yedek ve geri yükleme doğrulaması | Bekliyor |
| Yazma yetkisi kısıtlama kararı ve geri dönüş planı | Ayrı karar; uygulanmadı |
| Tablo kaldırma | Bu kapsamda yok |

Uygulama kabulü ve dış yazıcıların kapanışı doğrulanınca arşivleme/yetki kısıtlama adımı ayrıca hazırlanabilir. Okuma arşivinin ihtiyaç duyduğu erişim korunmalı; şema/DB düzeyindeki ortak haklar körlemesine kaldırılmamalıdır. Geçmiş tablolar ve veriler bu aşamada silinmez.

## Hazırlık doğrulaması

SQL, yerel Microsoft.SqlServer.TransactSql.ScriptDom TSql160Parser ile yalnızca sözdizimi bakımından kontrol edildi: 0 hata. Çalıştırılabilir token'larda DML/DDL, yetki değişimi, SELECT INTO veya EXEC bulunmadı. Bu kontrol betiği SQL Server'da çalıştırmaz; sunucu kataloğu/izin/sürüm uyumu ve gerçek sonuçlar kullanıcı çalıştırmasıyla doğrulanacaktır. İki yeni dosyanın UTF-8 ve satır sonu boşluk kontrolleri temizdir. Uygulama dosyaları ve üretilmiş ASCX dosyaları değiştirilmemiştir.

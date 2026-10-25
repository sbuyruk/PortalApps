# MFYS kapanışı ve ortak eski log yazıcılarının kaldırılması

Tarih: 2026-10-25
Dal: codex/mfys-eski-log-kapanis
Başlangıç: fef323fcb70c60893764ace06c348bd06459457e (PR #554).
Değişiklik öncesinde yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı. Kullanıcının devam isteğiyle MFYS incelemesine geçildi; Test yayımı/kullanım kabulü ayrıca kullanıcıya aittir.

## MFYS sonucu

Dört rapor ekranı önceki merkezi loglama geçişinde taşınmış. Bu parçada rapor davranışına yeni değişiklik gerekmedi:

| Ekran / metot | Veri erişimi ve merkezi karşılığı |
| --- | --- |
| AylikParaGirisleriWP.GetDataList / IslemDDLDoldur | MfysReportService.Payments / PaymentReasons → MfysReportRepository |
| BankaBazindaGunlukBagisListesiWP.GetDataList / BankaDDLDoldur | DailyTlTotals / BankGroups → MfysReportRepository |
| BankaBazindaGunlukDovizBagisListesiWP.GetDataList / BankaDDLDoldur | CurrencyDonations / CurrencyBankGroups → MfysReportRepository |
| TarihBazindaGunlukBagisListesiWP.GetDataList / BankaDDLDoldur | DailyDonations / BankGroups → MfysReportRepository |

DAO/Repositories/MFYS/MfysReportRepository.Select yalnızca PortalSqlReadExecutor.Select kullanır. Bağlam MFYS ile sınırlandırılır. Repository'de yalnızca yedi rapor okuması vardır; NBYS/TBYS'nin eski mutasyon servisleri veya DbClass çağrılmaz. SQL parametreli tarih/döviz/banka/sebep filtreleriyle yürütülür.

MFYS_WebParts/Logging/MfysWebPartLog.RecordOpened → PortalLogRepository.TryRecordWebPartOpened, ExecuteAction → PortalTechnicalLog.WriteError; SQL izi PortalSqlReadExecutor'ın finally bloğunda oluşur. İlk açılış ve OnPreRender sorgu akışı, overload'lar, hata/boş sonuç ayrımı ve kullanıcıya gösterilen mesajlar incelendi. Log ayarları kapalıysa merkezi yazıcılar yazmadan döner; eski bir servise geçiş yoktur. Raporlar iş verisi yazmadığından iş transaction'ı veya audit satırı üretilmesi gerekmiyor.

Yeni kayıtların görüntüleme karşılığı Portal Log İzleme ekranında MFYS proje filtresiyle Kullanıcı hareketleri, SQL izi ve Hata sekmeleridir. MFYS için ayrı eski olay/log listeleme ekranı bulunmuyor. Feature1'deki dört ekran gereklidir. DAO/Model/Utility proje ve paket assembly bağlantıları merkezi altyapı için gereklidir; kaldırılmadı.

## Ulaşılan son ortak yazıcı ve değişiklikler

MFYS kaynakları temiz olsa da DAO/Ortak/DBClass.cs ortak sınıfında eski SQL log yazıcısı duruyordu. Sadece GENEL_* bayraklarını kapatmak yeterli değildi: örneğin Insert(string), Update2Db(string), DeleteFromDb, ExecuteTransaction(List<SqlQuery>), parametreli mutasyon overload'ları ve ExecuteTransaction() hata blokları koşulsuz SorguyuLogla çağırıyordu. SQLLog_Table yazısı ayrı bağlantıda çalışıyor, log yazma hatası asıl hatayı veya rollback'e ulaşılmasını da etkileyebiliyordu.

Bu dalda:
- DbClass.SorguyuLogla ve SQLLog_Table INSERT kodu kaldırıldı. On üç başarı log dalı ile on koşulsuz hata log çağrısı kaldırıldı. SELECT/INSERT/UPDATE/DELETE komutları, parametreler, kimlik/etkilenen satır dönüşleri ve commit/rollback çağrıları korundu.
- DeleteFromDb'nin iki generic overload'unda yalnızca eski log için hesaplanan yansımalı eski değer metni ve özel GetEskiDeger kaldırıldı. İmza/parametreler tüketiciler için korundu.
- ExecuteTransaction() içindeki ham exception popup'ı kaldırıldı. İstisna mevcut throw ile çağırana iletilir; bağlamlı ekran sınırı teknik kaydı ve güvenli kullanıcı mesajını üretir. DbClass'a bağlamsız ikinci bir merkezi yazıcı eklenmedi.
- Utility/ProjeGlobal/ProjeConstants.cs içindeki üç GENEL_SAVE_LOG / GENEL_UPDATE_LOG / GENEL_DELETE_LOG alanı kaldırıldı.
- Dış tüketicisi kalmayan Model/Ortak/OlayKayit.cs, Model/Services/Ortak/OlayService.cs, DAO/Repositories/Ortak/OlayRepository.cs ve Model/Ortak/Olay.cs kaldırıldı. Transaction alan ve almayan tüm eski Save/Update overload'ları da bu kaldırmaya dahildir. Artık arşiv DTO'su PortalEventArchiveEntry kullanılır.
- Yalnızca bu eski zincirce kullanılan Model/Ortak/Extentions.cs (DetailedCompare/GetFields), Model/Ortak/Variance.cs ve DAO/Ortak/Global.cs kaldırıldı. Global sınıfının kalan metotlarının da dış tüketicisi bulunmadı. DAO/Ortak/SqlQuery.cs içindeki yalnızca eski SQL loga sorgu ve parametre değerlerini açan ToLogString kaldırıldı; Sql/Parameters/kurucular/AddParameter korundu.
- Silinen yedi dosyanın DAO/Model Compile kayıtları kaldırıldı. Model/DAO/Utility assembly'leri hâlâ iş verisi ve merkezi log için gerektiğinden referansları korunur.

Eski tabloya kalan tek C# erişimi DAO/Repositories/Portal/PortalEventArchiveRepository.SelectDay içindeki salt okunur Olay_Table SELECT'idir. Portal ve MTS geçmiş ekranları bunu kullanır; arşiv geçmişi yeni audit'e yeniden yazmaz. SQLLog_Table'a C# yazıcısı kalmadı. Tarihsel migration yorumları geçmiş tabloların korunmasını anlatmaya devam eder.

tests/Verify-DuzenliBagisciArmagan.ps1 eski bir test fixture'ı içinde OlayKayit adında yerel bir taklit tip tanımlar; uygulama assembly'sine dahil değildir ve gerçek eski tabloya yazmaz. Bu script önceki kaynak imzalarına bağlıdır, bu dalda çalıştırılmadı veya güncellenmedi. DAO/Ortak/DBProcess.cs içindeki eski Global.ConnectionString satırı da yalnızca yorumdur; etkin bağlantı ayarı değildir.

## Korunanlar ve statik kontroller

SqlTransactionContext, PortalLogRepository, PortalTechnicalLog, PortalLogSettings, PortalSqlReadExecutor ve proje bazındaki güncel iş/audit yazıcıları değiştirilmedi. İş verisi ile audit aynı SqlTransactionContext üzerinde kalır. Teknik SQL/hata kayıtlarının bağımsız bağlantı tasarımı ve log kapalıyken işin devam etmesi değiştirilmedi.

- Roslyn ile 11 C# kaynak dosyasının yalnızca sözdizimi kontrolü: 0 hata. Semantic compilation/build/test/debug çalıştırılmadı.
- DbClass'ın kalan 14 metodu için önceki sürümle imza, log dışı nesne/komut oluşturma ve metot çağrı dizisi karşılaştırıldı: fark yok.
- Solution'ın dokuz csproj dosyası ve 809 Compile yolu kontrol edildi: eksik dosya yok. Derlemeye dahil kaynaklar, üretilmiş dosyalar dahil, Roslyn identifier token'larıyla tarandı: kaldırılan sınıf/metot/bayraklara etkin referans yok. Yorum, metin ve küçük harfli olay değişkeni eski Olay tipi olarak sayılmadı.
- MFYS service çağrıları: 8; repository çağrıları: 7. Metot/argüman sayısı uyumsuzluğu yok. Bu kontrol tam tip kontrolünün yerine geçmez.
- MFYS SharePoint metadata: 16 XML dosyası parse edildi; Feature1'in dört öğesi proje kimlikleriyle, 21 ASCX olay bağı kaynak metotlarıyla eşleştirildi.
- Düzenlenen dosyalar UTF-8, mevcut BOM durumu korundu; git diff --check temiz. Hiçbir .ascx veya .ascx.g.cs değiştirilmedi. Kod üretimi gerekmiyor.

## Kullanıcının kabul ve regresyon senaryoları

1. Visual Studio 2026'da solution'ı derleyin; yeni DAO/Model/Utility assembly'lerini içeren paketleri Test ortamında tutarlı sürümlerle yayımlayın. Ortak public eski tipler kaldırıldığından depo dışı eski assembly/eklenti çağıranları varsa envanterlerini ayrıca kontrol edin.
2. MFYS'nin dört raporunda dolu/boş sonuç, ay/yıl/Tüm Aylar, tarih sınırı, banka/döviz ve ödeme sebebi/ayrıştırma filtrelerini deneyin. Satır/toplamlar, yenileme ve dışa aktarma önceki davranışla aynı olmalı. Ayrıntılı hesaplama örnekleri LoglamaGecisPlani.md'dedir.
3. Log açıkken ilk açılışta bir kullanıcı hareketi ve her gerçek SELECT için tek SQL izi bekleyin. Kontrollü DB hatası tek güvenli mesaj ve aynı OperationId'li merkezi hata üretmeli; eski sonuç veya yanlış sıfır toplam gösterilmemeli.
4. Merkezi loglama kapalıyken MFYS raporları ve diğer projelerde temsilî ekleme/güncelleme/silme işlemleri çalışmalı. Olay_Table ve SQLLog_Table'da yeni satır oluşmamalı. Log açıkken de eski tablolara yeni satır oluşmamalı.
5. IKYS izin/personel, TBYS ödeme/ayrıştırma, NBYS bağış/armağan, Portal duyuru/toplantı ve MTS kişi/faaliyet/anı objesi için temsilî yazma ve kontrollü hata senaryosu uygulayın. İş ve audit aynı transaction ile birlikte commit/rollback olmalı; teknik SQL/hata kayıtları bağımsız tasarımını korumalı. Eski log tablolarının yazma erişimi engellense bile iş ve rollback bunlara bağlı olmamalı.
6. Portal/MTS geçmiş arşivinde tarih/proje filtreleri ve mevcut eski sayfa örnekleri açılmalı. Eski satırlar korunmalı; arşiv okumak INSERT veya audit çoğaltması üretmemeli.

## Veri koruma ve kalan işler

Bu dal veritabanı tablosu, geçmiş veri, şema, yetki veya migration değiştirmez. Kaynakta eski yazıcıların kaldırılması, DB'deki trigger/procedure/view/function, SQL Agent, rapor/ETL veya dış uygulamaların yazmadığını kanıtlamaz.

Tüm paketler yayımlanıp kabul edildikten sonra dış bağımlılıkları envanterleyin; saklama süresini belirleyip yedeği ve geri yüklemeyi doğrulayın. Geçmişi erişimi sınırlı salt okunur arşivle koruyun. Eski tablo yazma yetkilerinin kaldırılması, izleme süresi ve sonunda tablo kaldırma işlemi ayrı kullanıcı kararıyla yapılmalıdır; bu dal bunları yapmaz.

Proje sırasındaki kaynak incelemeleri MFYS ile tamamlandı. Çalışma zamanı kabulü kullanıcıdadır. Son kapanışta ortak yardımcıların kalan PublishException/mesaj/rapor sonuç sözleşmeleri, dağıtılmış eski ekran örnekleri ve depo dışı bağımlılıklar ayrıca değerlendirilmelidir; yalnızca isimlerinde exception/log bulunması kullanılmakta olan iş sonuç tiplerini kaldırmak için yeterli değildir.
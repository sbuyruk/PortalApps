# MTS eski loglama kapanışı

Tarih: 2026-10-25
Dal: codex/mts-eski-log-kapanis
Başlangıç: f07d3930e744f6ec2698df651f11de3fa089ee2d (PR #553).
Değişiklik öncesinde yerel master temiz ve canlı origin/master ile aynı commit'te doğrulandı.

## Değişiklikler ve inceleme noktaları

- MtsOlayListesiWP/MtsOlayListesiWP.ascx.cs: GetDataList artık OlayService yerine PortalEventArchiveService.GetDay kullanır. Sorgu daima MTS programıyla sınırlandırılır. Gün seçimi ArsivTarihi query string'i üzerinden yapılır; varsayılan bugün, kabul edilen aralık 1753-01-01–9999-12-30'dur. Hatalı tarih Türkçe uyarıdır, önceki tablo gizlenir. Sayfa açılışı mevcut merkezi kayıt sınırında bir kez kaydedilir; veri okuma OnPreRender içinde bir kez yürütülür.
- Aynı ekranın webpart başlık/açıklaması salt okunur geçmiş arşivi olarak güncellendi. Tip kimliği, proje/feature/paket bağlantıları korunur. Yeni kayıtların karşılığı Portal Log İzleme ekranında MTS proje filtresiyle Kullanıcı hareketleri, Hata, İşlem denetimi ve SQL izi sekmeleridir. Arşiv geçmiş kayıtları merkezi sisteme yeniden yazmaz.
- DAO/Repositories/Portal/PortalEventArchiveRepository.cs: yalnızca bu salt okunur repository Portal ve MTS bağlamlarını kabul eder. MTS bağlamında MTS dışındaki, boş veya tüm programlar filtresi reddedilir. Tarih aralığı ve program parametreleriyle çalışan SELECT değişmedi. PortalSqlReadExecutor mevcut bağımsız teknik SQL izini üretir; ayrı hata kaydı ekran sınırında kalır. PortalSqlExecutor ve iş yazma izinleri genişletilmedi. Portal arşivinin mevcut tüm program/tek program sorguları korunur.
- Arşiv açıklamasının kişi/faaliyet/anı objesi detayları mevcut açık bağlamlı servislerden okunur; bunlar tarihsel snapshot değil güncel iş kayıtlarıdır. Bu sınırlama ekranda açıklanır. Silinmiş ilişkili kayıtlar ve HTML kodlama davranışı korunur.
- Model/MTS/MTSOrtak.cs: kullanılmayan altı eski toplantı e-posta giriş metodu ve sekiz özel yardımcısı kaldırıldı. Dış çağrı, overload, yerel aynı adlı metotlar ve kaynak metinlerdeki referanslar incelendi. IKYSOrtak içindeki aynı adlı farklı bildirim metodu korunur. İş ekranlarının kullandığı ParseFaaliyetAmaci ve ParseFaaliyetDurumu gövdeleri aynen korunur. Etkin bildirim yolları MtsActivityNotificationService ve PortalToplantiNotificationService'dir.
- TestWP/TestWP.ascx.cs: sabit toplantı 1773 okuması, sabit alıcıya REQUEST/update/CANCEL e-postaları ve timer yenilemesi kaldırıldı. Üç düğme ve giriş alanı gizlenir, timer her istekte kapatılır, Türkçe devre dışı bildirimi gösterilir. Geçmiş postback olayları da yalnızca bu bildirimi yeniler; DB/SMTP çağırmaz. Mevcut sayfa örnekleri için sınıf ve üretilmiş olay bağları korunur.
- TestWP/Elements.xml: galeri grup metadatası _Hidden yapıldı. Bu metadata değişikliğinin mevcut galeri öğesine çözüm güncellemesiyle yansıması Test sunucusunda kontrol edilmelidir; mevcut öğe güncellenmezse galeri yönetiminden gizlenmeli/kaldırılmalıdır. Bu durum kodda kapatılmış deneme gönderimini tekrar açmaz.

## SharePoint ve kaldırma kararı

Feature2 yalnızca TestWP içermez; arşiv ve başka iş ekranları da aynı feature içindedir. Feature veya paket referansı toptan kaldırılmadı. TestWP'nin Compile/Content, spdata/SafeControl ve feature bağlantıları, mevcut sayfalardaki tiplerin yüklenebilmesi için bilinçli olarak korundu. Kalan tip işlevsel bir test/gönderim ekranı değildir. Fiziksel kaldırma, farm sayfa/galeri envanterinde bu tipin hiçbir örneği kalmadığı doğrulandıktan sonra ayrı parçada yapılmalıdır.

VisualWebPart1 ve VisualWebPart2 boş, projeye dahil olmayan şablonlardır; mevcut csproj Compile/Content veya feature bağlantıları yoktur, bu paketle dağıtılmazlar. Üretilmiş dosyalara dokunulmadı. Kaynakların bulunması etkin bir çağrı yolu olarak değerlendirilmez.

Hiçbir .ascx veya .ascx.g.cs değiştirilmedi; bu parça için kod üretimi gerekmiyor. Yeni tarih alanı mevcut UpdatePanel'e çalışma anında LiteralControl olarak eklenir.

## Statik doğrulamalar

- Değişen dört C# dosyası Roslyn ParseText ile yalnızca sözdizimi bakımından incelendi: 0 hata. Derleme/semantic compilation yapılmadı.
- Solution kaynaklarındaki 109 MTS service ve 170 MTS repository çağrısının metot/argüman sayısı kontrolü: 0 uyumsuzluk. Değişen dosyalarda 20 yerel çağrı kontrolü: 0 uyumsuzluk.
- Arşivde kullanılan altı DTO üyesi, altı ASCX olay bağı ve kullanılan sekiz üretilmiş kontrol alanı kaynak tanımlarıyla eşleştirildi. Kaldırılan 14 yardımcıya dış kaynak bağı bulunmadı; aynı adlı yerel metotlar ve IKYS overload'ı ayrıştırıldı.
- Arşiv SELECT/parametre sözleşmesi aynen korundu. Dört MTS iş yazma dosyası, güncel bildirim servisi, SqlTransactionContext ve MtsCalendar değişmedi; Faaliyet DTO ve dokuz takvim JSON anahtarı korundu.
- DAO/Model/Utility/MTS proje dosyaları ve MTS SharePoint XML kayıtları: 61 dosya parse edildi, eksik Compile yolu bulunmadı.
- MTS model/service/repository/WebPart kaynaklarında OlayKayit, OlayService, OlayRepository, Olay_Table, SQLLog_Table, PublishException, ExceptionHelper ve kaldırılmış MTS log bayrağı referansı kalmadı. Geçmiş tablo okuması yalnızca ortak salt okunur arşiv repository'sindedir.
- UTF-8/BOM, üretilmiş dosyaların değişmemesi ve git diff --check kontrol edildi. Bu kontroller derleme ve çalışma zamanı kabulünün yerine geçmez.

## Kullanıcının uygulayacağı kontroller

1. Visual Studio'da solution'ı derleyin; paket güncellemesini Test sunucusuna uygulayın. Arşiv ekranının eski sayfa örneğinin açıldığını ve feature içindeki diğer iş ekranlarının çalıştığını doğrulayın.
2. Arşivde bilinen kayıtlı gün, kayıtsız gün, 1753-01-01 ve 9999-12-30 seçin. Geçersiz tarih/1753 öncesi/9999-12-31 için Türkçe uyarı ve gizlenen tablo bekleyin. Program query string'iyle başka proje geçmişi alınamadığını kontrol edin. Türkçe karakter ve HTML içeren veriler kod olarak çalışmamalı.
3. Silinmiş kişi/faaliyet ilişkileri ve anı objesi detaylarını inceleyin. Arşiv okuması eski tablolara INSERT üretmemeli; merkezi kullanıcı hareketi ve SQL izi yinelenmemeli. Portal arşivinde tüm program ve tek program filtrelerini gerileme kontrolünden geçirin.
4. Eski TestWP örneğini açın: devre dışı bildirimi görünmeli, düğmeler/giriş görünmemeli, timer isteği, örnek toplantı sorgusu ve SMTP gönderimi olmamalı. Önceden açık sayfadan kalan düğme/timer postback'leri de gönderim yapmamalı. Galeri grubunun _Hidden olduğunu kontrol edin; güncelleme mevcut galeri kaydını değiştirmediyse ayrıca galeri yönetiminden düzeltin.
5. MTS faaliyet/kişi/katılım/arama/anı objesi ekleme-değiştirme-silme, takvim taşıma ve gerçek bildirim yollarını deneyin. Log açıkken audit ile iş verisi aynı transaction sonucuna sahip olmalı; rollback'te ikisi de kalmamalı. SQL/hata kayıtları mevcut bağımsız yazma davranışını korumalı, hata tek kez ve başvuru koduyla gösterilmeli.
6. Merkezi loglama kapalıyken aynı iş senaryolarını tekrarlayın: iş sonucu korunmalı, Olay_Table/SQLLog_Table'a yeni kayıt düşmemeli. Merkezi açık senaryolarda aynı işlem kimliğiyle kullanıcı hareketi/audit/SQL/hata bağlantısını ve çift kayıt oluşmadığını kontrol edin.

## Veritabanı ve sonraki iş

Eski tablolar, geçmiş satırlar, şema ve SQL yetkileri değiştirilmedi. MTS için kaynak temizliği bu parçayla kapanır; yayım/çalışma zamanı kabulü kullanıcıdadır. Sıradaki proje MFYS'dir, bu dalda değiştirilmedi.

Ortak eski altyapı henüz solution genelinden silinmedi. MFYS ve kalan ortak tüketiciler kapandıktan sonra OlayService/OlayRepository/Olay/SQL log yardımcıları, genel bayraklar ve proje referansları için son bağımlılık taraması yapılmalıdır. Veritabanında ayrıca trigger, procedure/function/view, SQL Agent işi, rapor/ETL, dış uygulama ve dinamik SQL bağımlılıkları incelenmelidir; kaynak taraması bunları doğrulamaz.

Saklama süresi ve erişim ihtiyacı belirlendikten sonra geçmiş veriyi doğrulanmış yedek ve erişimi sınırlandırılmış salt okunur arşivle koruyun; tarih/proje indeks ihtiyacını veri hacmiyle değerlendirin. Eski tablolara yazma yetkisini ancak tüm projeler/dış tüketiciler kapandıktan sonra kaldırın. Tablo kaldırmayı arşiv geri yükleme kontrolü, bağımlılık onayı ve ayrı kullanıcı kararı sonrasına bırakın; bu parça hiçbir geçmiş veriyi silmez.
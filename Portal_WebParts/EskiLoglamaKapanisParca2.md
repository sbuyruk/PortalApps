# Portal eski loglamayı kapatma — parça 2/2

Tarih: 24.10.2026  
Dal: codex/portal-eski-log-kapanis  
Başlangıç master: 57f8d6ce052e19c90a5a22d16d15ffdee78a69ba (Portal PR #550 ve TBYS düzeltmesi PR #551 sonrası).

Yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı. Bu dal Portal kaynak kapanışıdır; çalışma zamanı kabulü henüz yapılmadı. İlk dalın EskiLoglamaParca1.md notuyla birlikte değerlendirilmelidir. Önceki LoglamaGecis*.md belgeleri aşamalı geçişin tarihsel kayıtlarıdır; eski servislerin/DbClass uyumluluğunun korunacağı ifadeleri kapanıştan sonra geçerli değildir.

## Bulgular ve uygulanan kararlar

### Eski olay ekranı artık salt okunur arşiv

Portal_WebParts/OlayListesiWP/OlayListesiWP.ascx.cs: GetDataList, TabloJson ve ParseAciklama artık eski OlayService/OlayRepository/Olay DTO'sunu kullanmaz. Yeni Model/Services/Portal/PortalEventArchiveService.cs ve DAO/Repositories/Portal/PortalEventArchiveRepository.cs yalnız geçmiş olay okumasını kapsar.

PortalEventArchiveRepository.SelectDay açık Portal bağlamı ister. Olay_Table'dan gereken yedi sütunu parametreli SELECT ile okur; INSERT/UPDATE/DELETE API'si yoktur. Çağrı PortalSqlExecutor.Select üzerinden merkezi SQL izine gider; log ayarı kapalıyken de eski yazıcıya dönmez. PortalEventArchiveEntry yalnız arşiv görünümünün veri sözleşmesidir; eski yazma modelini/servisini yeniden getirmez.

Ekrandaki başlık ve yeni galeri açıklaması “Geçmiş Olay Arşivi” olarak değişti. Yeni kayıtların Log İzleme'de olduğunu belirten açıklama eklendi. Önceden ekran yalnız bugünden itibaren sorguluyordu; şimdi tarih seçilebilir ve sorgu seçilen günün başlangıcı dahil, sonraki gün hariç aralıktadır. Program filtresi ve mevcut tablo/sıralama/yazdırma/Excel/PDF/kopyalama özellikleri korunur. Yeni sorguda eşit zamanlar Id ile kararlı sıralanır.

ReadArchiveDay geçersiz tarihi Türkçe uyarıyla reddeder; SQL tarihi alt sınırı ve ertesi gün taşması doğrulanır. Tarih alanı/düğme kaynak code-behind'da LiteralControl olarak oluşturulur ve aynı sayfaya tarih filtresiyle gider. Yeni ASCX alanı/handler yoktur; .ascx ve *.ascx.g.cs hiç değiştirilmedi.

ParseAciklama'nın faaliyet/kişi/dağıtılan-getirilen anı objesi işlevi korunur. Dört okuma açık Portal bağlamlı MTS overload'larından PortalSqlReadExecutor'a ulaşır; transaction'sız eski yazma, MailHelper, PublishException veya OlayKayit çağrısı yoktur. Bu yardımcıların varsayılan kurucuları MTS repository nesnesi oluştursa da kullanılan overload'lar DbClass üzerinden sorgu çalıştırmaz. MTS'nin genel yazma temizliği bu dalda yapılmadı.

Arşivdeki kişi/faaliyet/obje açıklaması mevcut iş kayıtlarından tamamlanır; tarihsel snapshot olduğu iddia edilmez. Ham eski açıklama ayrıca korunur. Ekrandaki açıklama bunu belirtir. Sayfanın mevcut SharePoint erişim kapsamı değiştirilmedi; arşiv yayını öncesinde sayfa izinleri kurumun geçmiş kayıt erişimi politikasına göre kontrol edilmelidir.

### Yeni sistemde eksik izleme karşılıkları tamamlandı

Portal_WebParts/LogIzlemeWP/LogIzlemeWP.cs önceden yalnız UserActivity ve Error sekmelerini içeriyordu. İşlem denetimi (Audit) ve SQL izi (SqlTrace) sekmeleri eklendi. Yeni kayıt incelemesi şu şekilde tamamlanır:

| Gereksinim | Merkezi karşılık |
| --- | --- |
| Kullanıcı hareketi / açılış | LogIzlemeWP, Kullanıcı etkinlikleri |
| Teknik hata / başvuru kodu / teknik ayrıntı | LogIzlemeWP, Hatalar |
| İş kaydı / işlem türü / kayıt türü ve Id / alan önce-sonra farkları | LogIzlemeWP, İşlem denetimi (Audit) |
| SQL işlem türü / repository-metot / süre / sonuç / OperationId | LogIzlemeWP, SQL izi |
| Günlük genel özet ve mevcut dört tür dashboard | LogDurumTakipWP, korunmuş mevcut görünüm |
| Eski Olay_Table açıklamaları ve dışa aktarma | Geçmiş Olay Arşivi, eski OlayListesiWP kimliğiyle |

Yeni sekmeler mevcut tarih aralığı, proje, kullanıcı, OperationId ve sayfalama filtrelerini kullanır. Model/Ortak/PortalLogDashboardModels.cs: PortalLogMonitorSummary içine AuditCount/SqlTraceCount eklendi. DAO/Repositories/Ortak/PortalLogReadRepository.SelectMonitorSummary aynı filtrelerle bu iki türü sayar; Model/Services/Ortak/PortalLogDashboardService.GetMonitorSummary yeni alanları eşler. Böylece audit/SQL sayfaları etkinlik sayısıyla sınırlandırılmaz.

Audit sekmesi gösterilen sayfadaki kayıtların GetChanges(row.Id) alan farklarını okur. Alan farkı olmayan başlıklar da görünür; ayrıntı yok diye kayıt gizlenmez. Alan adı/değerleri, kullanıcı/URL ve diğer dinamik metinler HTML kodlanır. SQL izi ham sorgu/parametre göstermez; komut sonucunun commit garantisi olmadığı belirtilir. Mevcut 25 satırlık sayfada en fazla 25 audit ayrıntı okuması yapılır.

ManageWeb denetimi her istekte korunur; yetkisiz kullanıcıda sorgu/alan farkı okuması yapılmaz. Filtre veya sekme değişiminde ilk sayfaya dönüş ve aktif sekmenin aria-pressed durumu korunur. Yeni sekmeler code-only WebPart'ın dinamik kontrol ağacındadır; ASCX üretimi gerekmez.

LogIzlemeWP'nin yetkili ilk açılışı artık mevcut merkezi RecordOpened yoluyla bir kez kaydedilir; postback'te ikinci açılış yoktur. Önceden bu ekranda açılış kaydı bulunmuyordu. Okuma hatasında aynı request OperationId ile PortalTechnicalLog.WriteError kullanılır; mevcut SharePoint ULS tanısı korunur ve kullanıcı yalnız güvenli Türkçe ekran içi mesajı görür. ThreadAbort hata sayılmaz. Log veritabanı erişilemiyorsa teknik yazma başarısızlığı kullanıcı mesajını gizlemez.

### Log okuyucusunun eski DB sınıfıyla bağı kesildi

DAO/Repositories/Ortak/PortalLogReadRepository artık DbClass alanı/kurucusu taşımaz; parametreli doğrudan SELECT kullanır. DbClass enjeksiyon kurucusunun kaynak tüketicisi bulunmadı. Parametresiz kurucu, sınıfın varsayılan public kurucusu olarak çalışır; dashboard servisinin mevcut oluşturması korunur.

Önemli ayrım: başlangıçtaki DbClass.SelectFromDb(SqlQuery, string) overload'ı zaten eski SQL logunu yazmıyordu. Bu değişiklik mevcut bir yazma bulgusunu kapatmak diye sunulmamalıdır; eski sınıf bağımlılığını kaldırır. Normal sonuç/sıfır satırda null ve istisna iletimi korunur. İzleme okumaları kendileri için teknik SQL izi/audit üretmez; kendi izini okuyup yeni iz üretme döngüsü oluşmaz.

## SharePoint proje / feature / paket kararı

Geçmiş kayıt erişimi gerekli olduğundan eski ekran silinmedi. Kimlik, namespace, Elements.xml, SafeControl ve feature öğesi korunur; mevcut sayfa örnekleri boşa düşmez. OlayListesiWP.webpart içindeki yeni galeri Title/Description değişti; mevcut sayfa örneklerinin özel Chrome başlıkları yeniden dağıtımda otomatik değişmiş varsayılmamalıdır. İç başlık code-behind'da güncellenir.

Doğrulanan öğe kimlikleri:

- OlayListesiWP: c591681a-fe12-4670-ae75-d410db7152f6
- LogIzlemeWP: 306db8df-26e1-4b12-b26c-5c9bc84f5ed1
- LogDurumTakipWP: 7a1b9b59-af4c-4374-a623-b3c21bc8f204
- Package.package featureReference: ab83d726-7c76-44ad-b4a3-be98ec7a02c9; Portal_WebParts.csproj içindeki Feature1 proje öğesiyle aynı.

Portal_WebParts/Package/Package.package içinde DAO.dll ve Model.dll sabit bin\Debug yollarına bağlı customAssembly idi. İki kayıt ilgili DAO.csproj/Model.csproj projectOutputAssembly çıktısına çevrildi; Utility zaten bu biçimdeydi. GAC deployment hedefi korunur. Bu, arşiv service/repository'sinin ve yeni summary özelliklerinin güncel proje derlemesinden paketlenmesi içindir; eski Debug çıktısının yanlışlıkla seçilmesine bağlı kalmaz. Visual Studio Debug/Release paketleme ve WSP içeriği kullanıcı tarafından doğrulanmalıdır.

İki yeni C# kaynak DAO.csproj/Model.csproj Compile kayıtlarına eklendi. Feature/Elements/SafeControl dosyaları silinmedi veya kimlikleri değiştirilmedi.

## Transaction / log ayarı ve kalan solution kapsamı

PortalDuyuruWriteService/Repository, PortalToplantiWriteService/Repository, PortalToplantiNotificationService ve SqlTransactionContext başlangıç sürümüyle aynı. İş verisi/audit aynı transaction'dadır; teknik Error/SqlTrace bağımsız yazılır. Yeni audit alanı veya ikinci iş kaydı üretimi eklenmedi. Görüntüleme değişiklikleri iş yazma transaction'ını açmaz.

Portal iş service/repository/WebPart kaynaklarında OlayService, OlayRepository, OlayKayit, DbClass, PublishException ve eski Portal log bayraklarına erişim kalmadı. Bilinçli tek eski tablo erişimi PortalEventArchiveRepository.SelectDay içindeki salt okunur Olay_Table sorgusudur. Ortak log okuyucusu yalnız merkezi PortalLog_Table/PortalAuditChange_Table'ı okur.

Ortak UtilityHelper tüketicileri (oturum kimliği, sayfa/site URL, script, dropdown/saat) ve arşivdeki açık bağlamlı MTS okumaları kontrol edildi. Portal bildirim zinciri PortalToplantiNotificationService'dir; eski MTSOrtak/MailHelper çağrısı yoktur. Başka projelerin eski global sınıfları bu dalda silinmedi. OlayService.GetByDateAndProgram(..., context) hâlâ MtsOlayListesiWP tüketicisidir; global kaldırma MTS/MFYS ve dış bağımlılık kapanışı sonrasındadır.

## Statik kontroller

- 7 yeni/değişen C# dosyasında Roslyn ParseText/GetDiagnostics sözdizimi incelemesi: hata yok. Derleme/emission, build, test veya debug çalıştırılmadı.
- 145 yerel metot çağrısında argüman kontrolü ve izleme ekranının üç dashboard servis çağrısı temiz.
- İzleme ekranındaki 39 DTO üye başvurusu gerçek model özelliklerine karşı kontrol edildi. Altı summary SQL alias/model/mapper eşleşmesi ve yeni arşiv DTO'sunun sorgu alanları doğrulandı. Bunlar genel semantik derleme yerine geçmez.
- Merkezi okuyucudaki dört SQL/parametre sözleşmesi değişmedi. Tek kasıtlı SQL değişimi SelectMonitorSummary'ye iki COUNT_BIG sütunudur; filtreler ve tür adları korunur.
- Arşiv repository'sinin tek public metodu SELECT-only; yazma API'si yok.
- Beş iş yazma/bildirim dosyası ve SqlTransactionContext içeriği başlangıçla aynı.
- 62 XML/proje/feature/paket/öğe dosyası, Compile yolları, üç WebPart'ın feature üyeliği, paket feature ve proje çıktı bağlantıları kontrol edildi.
- UTF-8/BOM/CRLF ve git diff --check temiz. .ascx/*.ascx.g.cs değişmedi; yeniden üretim gerektiren bir alan/handler eklenmedi.
- Veritabanına veya çalışan SharePoint'e bağlanılmadı. Sayfa örnekleri, çözüm dışı derlenmiş tüketiciler ve DBA bağımlılıkları kaynak incelemesiyle kanıtlanamaz.

## Kullanıcının uygulayacağı kabul senaryoları

1. Visual Studio 2026'da tüm solution'ı derleyin. Debug ve Release paketlemede WSP içindeki DAO/Model/Utility DLL'lerinin güncel çıktılar olduğunu doğrulayın; Test sunucusunda yayımlayın. Yeni DTO/service/repository ve summary özellikleri için eski GAC DLL'i kalmamalı.
2. Mevcut OlayListesiWP sayfasını açın: iç başlık arşiv olmalı, mevcut WebPart örneği çalışmalı. Geçmiş gün, bugün, boş gün ve Program filtresini deneyin. Gece yarısı kayıtları doğru güne ait olmalı; geçersiz tarih güvenli uyarı vermeli ve eski sonuçlar gösterilmemeli.
3. Arşiv açıklamasında faaliyet/katılımcı sırası farklı, boş/bozuk açıklama, silinmiş kişi/faaliyet ve verilen/getirilen obje kayıtlarını deneyin. Ham açıklama ve tamamlanmış açıklama korunmalı; özel karakterler HTML/script olarak çalışmamalı. Yazdırma/Excel/PDF/kopyalama çalışmalı.
4. Log İzleme'de dört sekmeyi tarih/proje/kullanıcı/OperationId ile filtreleyin. Audit ve SqlTrace için 25'ten fazla kayıtla ileri/geri, son sayfa, filtre/sekme değişimi ve boş sonuçları doğrulayın. Sayılar seçilen türün sayısı olmalı.
5. Audit'te INSERT/UPDATE/DELETE, kayıt türü/Id, alan eski-yeni değerleri ve ayrıntısı olmayan başlıkları kontrol edin. Sayfa/OperationId doğru kayda ait olmalı. Sıralama ve UTC→Türkiye saati dahil tarih aralığı sınırlarını kontrol edin.
6. SQL sekmesinde SELECT/INSERT/UPDATE/DELETE, başarılı/hatalı sonuç, süre, repository/metot ve OperationId görünmeli; ham SQL/parametreler görünmemeli. İş rollback'inde teknik iz kalmasının commit anlamına gelmediğini kontrol edin.
7. ManageWeb yetkili/yetkisiz kullanıcıyla merkezi ekranı deneyin; yetkisiz istekte log/alan farkı sorgusu yapılmamalı. Arşiv sayfasının mevcut izinlerini ayrıca doğrulayın.
8. Merkezi izlemeyi ilk açışta tek UserActivity; sekme/sayfa/filtre postback'inde ikinci açılış olmaması beklenir. İzleme sorguları yeni SqlTrace üretip kayıt sayısını sürekli artırmamalı.
9. Arşiv SELECT'inde ve merkezi summary/liste/alan farkı okumasında kontrollü hata: tek güvenli mesaj/OperationId; önceki sonuçlar görünmemeli. Merkezi yazma da kapalı/erişilemez olduğunda mesaj gösterimi sürmeli; log hatası asıl hatayı maskelememeli.
10. Portal merkezi logu açık ve tamamen kapalıyken ilk parça duyuru/toplantı/katılım/parametre akışlarını tekrarlayın. Tam kapatma için Portal hem EnabledProjects hem ilgili EnabledWebParts kapsamı dışında olmalıdır; mevcut yapı OR ile değerlendirir. İş/audit transaction ve SMTP commit sonrası davranışı korunmalı; eski tablolara Portal kaynaklı INSERT oluşmamalı.
11. DB/audit hatası ile iş rollback'ini ve SMTP hatasında tamamlanmış işin korunmasını tekrar doğrulayın. Yeni kayıtlar merkezi izleme ekranında tekil olmalı; arşivde görünmeleri beklenmez.
12. Ölçüm sırasında MTS ve diğer projelerin eşzamanlı eski yazılarını ayırın. Kaynak dışı trigger/SP/job/uygulama yazılarını DBA ile ayrıca doğrulayın.

## Eski tablolar ve ileride kaldırma

Eski tablolar, geçmiş kayıtlar ve iş verileri silinmedi; yeni merkezi tablolara kopyalanmadı. Arşiv ekranının çalışması Olay_Table ve açıklamayı tamamlayan iş tablolarına okumayla bağlıdır. Bu bağımlılık varken tablo kaldırılmamalıdır.

Tüm projeler ve dış tüketiciler kapandıktan sonra DBA ile uygulama/assembly, trigger/SP/view/synonym, job ve rapor envanteri ve tarih/hesap bazında yazma izlemesi yapılmalı. Saklama politikasına göre salt okunur arşiv, sayım/dönem/ilişki doğrulaması ve geri yükleme kontrolü tamamlanmalı. Arşiv ekranı yeni arşiv kaynağına taşınmalı veya kurum kararıyla kaldırılmalı; sonra sayfa örnekleri/feature/paket/GAC ve global eski sınıflar temizlenmelidir. Tabloya yazma engeli ve olası tablo kaldırma ayrı onaylı DBA/yayın değişiklikleridir.

Portal kaynak kapanışından sonraki proje MTS'dir. Kullanım kabulü veya kullanıcının açık devam isteği olmadan sonraki projeye geçilmez. Commit, push, PR, merge, pull, yayın ve runtime testleri kullanıcıya aittir.

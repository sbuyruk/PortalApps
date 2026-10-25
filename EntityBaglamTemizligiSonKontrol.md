# Entity, log bağlamı ve son kaynak kontrolü — parça 5

Tarih: 2026-10-25  
Dal: `codex/entity-baglamli-okuma-temizligi`  
Başlangıç: `a6d8c1d0830f6a45bb5103faefb34651458853f2`

Kullanıcının devam talebiyle başlandı. Önceki parçanın Test kabulü bu sohbette ayrıca bildirilmedi. Değişiklik öncesinde yerel master temizdi ve GitHub üzerinden sorgulanan uzak master ile aynı commit'teydi.

## Personel entity okuma zinciri

Model.IKYS.PersonelItem ve IsBilgileriItem artık kurucuda log bağlamı ister. Sekiz bağlamsız servis dönüş yolu kaldırıldı; bütün DB okumaları mevcut bağlamlı overload'lara gider. PersonelItem alt IsBilgileriItem nesnesini aynı bağlamla oluşturur. Bağlam alanı NonSerialized kalır; bağlamını kaybeden bir nesne DB okumadan önce Türkçe teknik istisna yükseltir. Internal bağlam ataması null kabul etmez.

Kendi kendine atama yapan ve tüketicisi olmayan dokuz private ve dokuz public setter kaldırıldı. Okunan özellikler korunur; getter'ların mevcut sorgu/güncellik davranışı devam eder, bu dal önbellek veya salt veri entity'sine dönüşüm getirmez.

Gerçek dış tüketici PersonelGirisiWP.NotifyInformationSystems içindedir ve zaten LoggingContext ile kurulum yapmaktadır. BilgiSistemMailGrubunaEPostaGonder mevcut bağlamı atamaya devam eder; AD/SMTP/DB hataları mevcut log ve false sonuç üzerinden WebPart uyarısına gider. Portal DuyuruGirisiWP içindeki aynı adlı nested PersonelItem farklı bir veri sınıfıdır ve değiştirilmedi.

## Model mesajı ve bölge sonucu

IKYSOrtak.BolgeGetirByUserName içindeki son Messages.Error kaldırıldı. Eksik personel/iş bilgileri/birim/bölge veya geçersiz bölge kimliği artık istisna ile çağırana bildirilir. Eksik bilgi 0 kimliğiyle bütün bölgeleri sorgulamaya dönüşmez. Geçerli kullanıcı ve genel müdürlük bölge eşlemesi değişmez.

Bu yardımcının 20 tüketicisi TBYS Page_Load metotlarındadır. Kaynaklarda mevcut yerel catch veya OnLoad hata sınırı bulunur; teknik kayıt ve başvuru kodlu Türkçe mesaj mevcut TbysWebPartLog üzerinden üretilir. Model popup göstermez. NBYS yıl/ay ekranı önceki parçadaki kendi bağlamlı/eksiklikte duran zincirini korur. TBYS KiraSozlesmeListesiWP önceki parçadaki ayrı okuma yolunu kullanmaya devam eder.

## Son taramada bulunan yönerge okumaları

IKYS EskiPersonelWP ve PersonelGirisiWP yönerge bağlantısını üç parametreli UtilityHelper.YonergeURLGetir üzerinden bağlamsız okuyordu. İki çağrı LoggingContext ile güncellendi; üç parametreli yardımcı kaldırıldı. Helper'ın nullable bağlam dönüş yolu da kaldırıldı.

NBYSParametreService/Repository içine açık PortalLogContext alan salt okunur GetByGroupAndKey/SelectByGroupAndKey overload'ları eklendi. Bu yol mevcut PortalSqlReadExecutor kullanır; paylaşılan NBYSParametre_Table okuması IKYS veya NBYS çağıranının projesi/WebPart'ı/OperationId değeriyle izlenir. NBYS yazıcılarının proje doğrulaması gevşetilmedi. Aynı parametreli SELECT, sıralama, dosya yolu ve parametre bulunmadığında default.pdf davranışı korunur. DB hatası eksik parametre sayılmaz; mevcut ekran hata sınırına yükselir.

## Statik inceleme ve kapsam

- Sekiz düzenlenen C# kaynağı Roslyn ile yalnızca sözdizimi bakımından incelendi: 0 hata.
- Model kaynaklarında Messages/MessageHelper çağrısı ve taranan nullable bağlam fallback ifadeleri kalmadı.
- 13 yönerge çağrısının tamamı dört argüman ve log bağlamı taşır.
- Gerçek Model entity kurulumları bağlamlıdır; Portal'daki aynı adlı yerel tip ayrıca ayırt edildi. Kaldırılan property setter'larının kaynak tüketicisi yoktur.
- Takip edilen C#, ASCX, proje ve konfigürasyon kaynaklarında eski Olay/SQLLog tablo erişimi, eski log yazıcı adları, DBObject/DbParam ve ExecuteTransaction referansı bulunmadı. Tarihsel belgeler bu kaynak taramasının dışında tutuldu.
- Uygulama C# kaynaklarında MessageHelper tüketicisi kalmadı; ortak sınıf mevcut public uyumluluk sözleşmesi olarak korunur. Kullanılan DbClass ve bağlamsız salt okuma API'lerinin genel kaldırılması bu dalın kapsamı değildir.
- UTF-8/BOM ve git diff --check temiz. ASCX ve üretilen .ascx.g.cs değiştirilmedi; kod üretimi gerekmez.
- İş transaction'ları, audit yazımları, teknik log depolaması ve veritabanı şeması değiştirilmedi. Build, test, debug veya semantic compilation çalıştırılmadı.

## Yayımlama ve kullanıcı kabulü

Entity kurucu/setter ve helper imzaları değiştiği için Visual Studio 2026'da solution'ı derleyin; DAO, Model ve IKYS_WebParts sürümlerini uyumlu biçimde Test ortamına yayımlayın. TBYS kaynakları değişmese de ortak bölge yardımcısının davranışı bu ekranları etkiler. Depo dışındaki public API tüketicileri ayrıca yeniden derlenmelidir.

1. Personel girişinde bilgi sistemleri bildirimini normal akışta ve kontrollü AD/SMTP/DB hatasıyla deneyin. Personel/birim/unvan/iletişim okumaları aynı IKYS OperationId ile izlenmeli; hata tek teknik kayıt ve WebPart uyarısıyla bildirilmelidir.
2. PersonelItem/IsBilgileriItem için parametresiz kurucu veya kaldırılan setter kullanan derlenmiş tüketici kalmadığını solution derlemesiyle doğrulayın. Bildirim içeriğinin önceki sürümle aynı olduğunu kontrol edin.
3. İki IKYS personel ekranının yönerge bağlantısını normal parametre, eksik parametre ve SQL hatasıyla deneyin. SELECT izinde Project=IKYS olmalı; eksik parametrede default.pdf, teknik hatada mevcut hata sunumu beklenir. Temsilî bir NBYS yönerge bağlantısını da kontrol edin.
4. Yardımcıyı kullanan 20 TBYS ekranında geçerli bölge/genel müdürlük açılışını kontrol edin. Eksik personel, iş bilgileri, birim veya bölgeyle liste sorgularına devam edilmemeli, bütün bölgeler verisi gösterilmemelidir. Başvuru kodlu güvenli hata beklenir.
5. Merkezi log kapalıyken geçerli okumalar çalışmalı; eksik bağlam/eksik bölge yine bağlamsız veya bütün bölgeler yoluna dönmemelidir.
6. Önceki dört parçanın ilgili kullanıcı kabul senaryolarını ve temsilî iş/audit rollback kontrolünü tamamlayın.

Bu parça beş dallık kaynak temizliği planının son parçasıdır. Kaynak temizliği; Test kabulü, farm'daki eski assembly/eklenti envanteri veya gerçek veritabanındaki trigger/procedure/iş bağımlılıklarının kapandığı anlamına gelmez. Mevcut Database/Diagnostics/EskiLogBagimlilikEnvanteri.sql ile veritabanı envanteri kullanıcı tarafından alınmalıdır. Bu dal tablo veya geçmiş veri silme kararı içermez.

Commit, push, PR ve birleştirme kullanıcı tarafından yapılır.

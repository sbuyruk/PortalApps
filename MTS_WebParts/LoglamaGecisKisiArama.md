# MTS ikinci parça — kişi ve arama/görüşme

Başlangıçta master temizdi ve uzak master ile aynıydı: `440540a559492f72cd8edc9e7c459321bc8183ac`.
Dal: `codex/mts-kisi-arama-transaction`.

## Ekran kapsamı ve mesaj inceleme noktaları

| Ekran | Değişiklik ve kullanıcı mesajı |
| --- | --- |
| KisiGirisiWP | Kişi kaydetme/güncelleme/silme transaction ve audit üzerinden çalışır. Ad, soyad, sayısal kimlik numarası ve doğum tarihi yazmadan önce doğrulanır. Tekrarlanan kimlik numarası ayrı uyarıdır; boş kimlik numarası mevcut davranışla 0 kabul edilir. Geçersiz ünvan/il/ilçe ilişkisi servis içinde reddedilir. Başarı yalnız transaction tamamlandıktan sonra gösterilir. Kayıttan sonra formda kalınır, yeni kaydet düğmesi gizlenir ve güncelleme açılır. |
| KisiGirisiWP — silme | Onay penceresindeki eski ön kontroller korunur; asıl silme sırasında ilişkiler aynı transaction içinde yeniden kontrol edilir. Faaliyet katılımı, dış irtibat, arama, anı objesi veya kurum/görev kaydı varsa silinmez; anlamlı uyarı verilir. Mevcut kişi silme düğmesinin görünürlük politikası genişletilmedi. |
| KisiGirisiWP — faaliyet | Faaliyet, seçilmişse katılım ve audit birlikte kaydedilir. Katılım eklenemezse faaliyet de rollback olur. İrtibat/katılımcı seçenekleri korunur. Randevu kısıtlı kişi için faaliyet oluşturulmaz. Commit sonrası faaliyet ekranına gidilir. Aynı formda tamamlanmış işlem yeniden onaylanırsa mevcut faaliyet açılır. |
| AramaGirisiWP | Konu, kişi, tarih/saat ve görüşme şekli yazmadan önce doğrulanır. Önceden konu boşken kayıt yazılıp ardından hata verilmesi düzeltildi. Kişinin randevu kısıtı servis içinde yeniden kontrol edilir. Görüntüleme sırasında arama kaydını sessizce değiştiren FaaliyetBtnEnable yazıları kaldırıldı. |
| AramaGirisiWP — faaliyet/silme | Faaliyet oluşturma ve aramaya bağlama tek transaction'dır. Yeniden onay, aramaya zaten bağlı faaliyet varsa ikinci kayıt oluşturmaz. Silme yalnız aramayı siler; ilişkili faaliyet korunur. Silme onayındaki bağlı faaliyet açıklamasının üstüne yanlış “bağlı faaliyet yok” metni yazılması düzeltildi. |
| KisiKartiWP | Kişi, il/ilçe, faaliyet, arama ve anı objesi okumaları MTS bağlamını taşır. Eksik kişi anlamlı uyarıdır. Teknik okuma hatası boş/kısmi başarılı sonuç gibi yutulmaz. Boş telefon metinleri NullReference üretmez. Karttaki dinamik metinler HTML kodlanır; uygulamanın ürettiği faaliyet bağlantısı ve satır sonları korunur. |
| BagisciListesiWP | Nakit/taşınmaz bağışçı okumaları açık MTS bağlamıyla izlenir. Bir veya iki kaynak boş döndüğünde null Merge hatası oluşmaz. Başarısız kaynak boş sonuç gibi kabul edilmez. Tablo metinleri HTML çalıştırmaz; sahte boş başlangıç satırı kaldırıldı. |

Teknik ayrıntılar kullanıcıya verilmez; ortak MtsWebPartLog tek güvenli hata ve başvuru kodu gösterir. Beklenen doğrulama/ilişki/çakışma durumları MtsInputException uyarısıdır. İki formda kayıt ilk açıldığında sürüm özeti ViewState'e alınır; farklı bir kullanıcı kaydı değiştirmişse eski formdan güncelleme veya silme reddedilir. Kaydetme sonrası formu yenileme hatası, “kayıt tamamlandı, ekran yenilenemedi; listeyi kontrol edin” mesajı ve başvuru koduyla ayrı gösterilir. Yeni kişi/arama formlarında anlamlı seçim uyarıları korunur; ham exception metinleri ve iç içe ExceptionHelper popup'ları kaldırıldı.

## Transaction ve veri erişimi

Yeni MtsContactWriteService, MtsContactWriteRepository ve MtsSqlExecutor yalnız açık MTS bağlamını kabul eder. Kişi/arama CRUD, ilişki/kimlik kontrolü, faaliyet oluşturma ve audit yazıları aynı SqlTransactionContext üzerinden yürür. Generic repository tablo adları allowlist ile sınırlandırılır; her yazı sonrası kayıt okunur ve audit aynı transaction'a eklenir. Sorgu metinleri ve değerleri SQL trace'e yazılmaz; kişi adları, kimlik numarası, telefon, adres, doğum tarihi ve serbest metinler audit alanı yapılmaz.

Yazma servisleri satır kilitlerinden önce `PortalApps.MTS.Write` uygulama kilidini alır. Sonraki faaliyet/stok geçişi bu kilit sırasını paylaşmalıdır. Henüz taşınmamış bağlamsız legacy yazma yolları bu dalda topluca değiştirilmez; üçüncü parçada ele alınacaktır. Bu nedenle MTS'nin tamamı için geçiş tamamlandı denmez.

FaaliyetKatilim, AniObjesiDagitim ve MTSUnvanTanim için açık bağlamlı salt okuma kurucuları eklendi; bu kuruculardan legacy yazma çağrıları reddedilir. Bağlamsız çağıranlar korunur. Kurum/görev alanları bu ekranlarda mevcut salt okuma davranışını sürdürür; yeni kurum/görev yönetim ekranı eklenmedi. Faaliyet katılımcısının kurum/görev metni oluşturulurken aynı transaction'dan okunur.

NBYS/TBYS bağışçı raporlarında yalnız MTS'nin kullandığı okumalar için açık bağlam overload'ları eklendi; ilgili projelerin genel geçişi tekrar açılmadı. Yeni paket veya DB migration yoktur. Üç yeni C# dosyasının Compile kayıtları DAO/Model projelerine eklendi.

## Yapılandırma ve kod üretimi

Mevcut `PortalLogging.EnabledWebParts` değerlerini koruyarak ekleyin:

`MTS:KisiGirisiWP;MTS:KisiKartiWP;MTS:AramaGirisiWP;MTS:BagisciListesiWP`

MTS tamamlanmadan `EnabledProjects` içine MTS eklemeyin.

Yalnız BagisciListesiWP.ascx kaynağındaki tablo JavaScript'i değişti; yeni sunucu kontrolü eklenmedi. Visual Studio'da bu ASCX üzerinde **Run Custom Tool** çalıştırılmalıdır. Codex üretilen `*.ascx.g.cs` dosyalarını düzenlemedi; elle düzenlenmemelidir.

## Kullanıcı testi

1. BagisciListesiWP.ascx için Run Custom Tool çalıştırın; Visual Studio 2026/Test sunucusunda derleyin ve yayımlayın.
2. Kişide ad/soyad boş, geçersiz doğum tarihi, rakam olmayan kimlik numarası ve mükerrer kimlik numarası deneyin. Başarısız doğrulamada yeni satır ve başarı mesajı oluşmamalı. Boş kimlik numarasıyla izin verilen kayıt senaryosunu da kontrol edin.
3. Kişi ekleyin ve güncelleyin. Formda doğru kayıt ve güncelleme düğmesi görünmeli. İl/ilçe ve ünvan seçimi korunmalı; oluşturma bilgisi güncellemede değişmemeli. İki ayrı tarayıcı/formla eski sürümün güncelleme/silme denemesinin uyarıyla durduğunu doğrulayın.
4. Testte erişilebilir kişi silme yoluyla ilişkisiz ve ilişkili kişiyi deneyin. Arama/faaliyet/irtibat/kurum-görev/objesi olan kişi silinmemeli; transaction içindeki yetkili kontrol son sözü vermeli. Silinen kişiye tekrar yazma denemesi başarı göstermemeli.
5. Kişiden faaliyet oluşturmayı katılımcı ve irtibat kutularının farklı seçimleriyle deneyin. Katılımcı seçiliyse doğru kişi ve kurum/görev eklenmeli; seçili değilse katılım eklenmemeli. Randevu kısıtı anlamlı uyarıyla reddedilmeli. Test ortamında kontrollü katılım/audit yazma hatasında yarım faaliyet kalmamalı.
6. Arama konusunu boş bırakarak kaydetmeyi deneyin: artık önce kayıt oluşmamalı. Tarih/saat ve kişi seçimini doğrulayın. Kayıt/güncelleme/silmede tek sonuç mesajı beklenir. Arama silindiğinde faaliyet korunmalı.
7. Aramadan faaliyet oluşturun ve aynı onayı tekrar gönderin. Aynı aramaya ikinci faaliyet oluşmamalı. Kontrollü arama bağlantısı/audit hatasında yeni faaliyet rollback olmalı. Ekranı yalnız görüntülemek arama kaydını değiştirmemeli.
8. Kişi kartını dolu/boş geçmişle, null telefonla, özel karakterli metinlerle ve Excel dışa aktarmayla deneyin. Teknik hata tek güvenli mesaj/başvuru kodu üretmeli; ham hata görünmemeli.
9. Bağışçı listesinde iki kaynağın dolu, birinin boş ve ikisinin boş olduğu durumları kontrol edin. Adres/telefon metni HTML olarak çalışmamalı. Bir kaynakta SQL hatası, başarılı boş sonuç gibi görünmemeli.
10. Açılış, SQL trace, INSERT/UPDATE/DELETE audit ve değişen alanları aynı OperationId ile inceleyin. Kişisel/serbest metinlerin audit değişimlerinde bulunmadığını, başarısız transaction'ın iş verisi ve audit bırakmadığını doğrulayın.

Codex Windows üzerinde build/test/debug çalıştırmadı. UTF-8/BOM, C# ayraçları, ASCX olay metotları, bağlam kurucuları, proje Compile kayıtları ve diff temizliği statik olarak incelendi. Statik inceleme derleme veya çalışma zamanı testi yerine geçmez. Commit/push/PR/merge kullanıcıya aittir.

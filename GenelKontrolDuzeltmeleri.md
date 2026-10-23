# Genel kontrol bulgularının düzeltilmesi

Dal: `codex/genel-kontrol-duzeltmeleri`. Başlangıçta master temizdi ve canlı uzak master ile aynı commit'teydi: `9b6544ca75101004d8ab6c003b76bf4142a17110` (PR #535).

## Kapsam ve kullanıcı mesajları

| Bulgu | Düzeltme ve inceleme noktası |
| --- | --- |
| IKYS maaş artışı kısmi kayıt/silme | MaasArtisiWP, yeni SalaryScaleWriteService/Repository üzerinden çalışır. Tüm yeni ücret satırları tek transaction'da kaydedilir. Son grubun ücret tanımları ve bağlı maaş hareketleri aynı transaction'da silinir. Her satırın audit'i de aynı transaction'dadır. INSERT kimliği başarısızsa işlem commit edilmez. |
| IKYS ham hata ve başarı davranışı | Exception mesajı kullanıcıya eklenmez; yeniden fırlatma ve legacy popup yolu kaldırıldı. Teknik hata başvuru kodlu tek güvenli mesajdır. Tarih, yüzde, AGİ, eksik tanım ve değişmiş kayıt beklenen Türkçe uyarılardır. Başarı yalnızca transaction Dispose/commit tamamlandıktan sonra gösterilir. |
| MFYS ödeme sebebi filtresi | Ayrıştırma varsa C satırının ödeme sebebi esas alınır; ayrıştırma yoksa ana ödemenin sebebi kullanılır. Ana sebep eşleştiği için diğer ayrıştırmaların da tek sebep toplamına katılması önlendi. Hepsi filtresi ve TutarB/TutarA hesabı korunur. |
| MTS katılımcı takvim güncellemeleri | Aylık/haftalık takvimden tarih veya süre değiştiğinde, önceden davet gönderilmiş katılımcılara da aynı faaliyet UID'siyle güncelleme gider. Özel kalem seçimi yalnızca özel kalem alıcısını yönetir; katılımcı güncellemelerini kapatmaz. Davet gönderilmemiş katılımcıya yeni davet gönderilmez. |

## Maaş transaction ve onay ayrıntıları

`SalaryScaleWriteRepository`, işlem başında `PortalApps.IKYS.SalaryScale` transaction kilidini alır; ücret tablolarını `UPDLOCK,HOLDLOCK` ile okur. Maaş satırları da silmeden önce aynı transaction'da kilitli okunur. SELECT, INSERT, DELETE ve audit, üst servisin tek SqlTransactionContext nesnesiyle yürür. Transaction parametresi olmayan legacy yazma/okuma yoluna geçilmez. Mevcut diğer ücret servislerinin davranışı değiştirilmez.

Ekrandaki son grup satırları ve ilk grubun silinip silinemeyeceği SHA-256 sürümüyle temsil edilir. Kullanıcının açtığı sayfanın sürümü ViewState'te tutulur, kayıt/silme onayı sırasında korunur. İşlem kilidi altında tekrar hesaplanan sürüm farklıysa hiçbir yazı yapılmadan uyarı döner. Eski sekme veya aynı onayın tekrar gönderilmesi yeni bir artış oluşturamaz, daha yeni grubu silemez. Tek grubun korunması satır sayısıyla değil farklı grup sayısıyla kontrol edilir.

Yeni dönem var olan bir dönemle çakışamaz. Negatif yüzde/AGİ ve hesaplama taşması reddedilir. Artış formülleri ve derece/kademe/unvan kopyalama davranışı korunur. Tarihler Türkçe `gg.aa.yyyy`, yüzde/AGİ virgüllü ondalık biçiminde doğrulanır. İşlem onayı açılmadan kayıt veya silme başlatılamaz. Kullanıcının işlem yapabilmesi için mevcut SharePoint sayfa erişimi gerekir; yeni bir yetki sistemi eklenmedi.

Kayıt/silme tamamlandığında başka sayfaya otomatik yönlendirmek yerine aynı ekranda başarı mesajı ve güncel sonraki dönem bilgileri gösterilir. Böylece yönlendirmeden kaynaklanan ThreadAbort bir kayıt hatası gibi sunulmaz ve başarı mesajı kaybolmaz. Commit sonrası form yenilemesi başarısızsa “işlem tamamlandı, ekran yenilenemedi” anlamında tek uyarı verilir ve işlem düğmeleri kapatılır; kayıt tekrar denenmeden sayfanın yeniden açılması istenir. Kapat yönlendirmesi ayrı güvenli sınır içindedir.

Maaş tutarları mevcut metadata audit yaklaşımına uygun olarak loga kopyalanmaz; UcretTanim için grup, derece, kademe ve dönem alanları; MaasHareket için silinen kayıt kimlikleri kaydedilir. Transaction güvenliği loglama ayarından bağımsızdır; merkezi audit/SQL trace mevcut EnabledWebParts/EnabledProjects ayarına bağlıdır. Ekran anahtarı `IKYS:MaasArtisiWP` olarak kalır.

## MTS bildirim ayrıntıları ve sınırlar

Bildirimler faaliyet commit edildikten sonra gönderilir; SMTP sırasında iş transaction'ı açık tutulmaz. Gönderime başlamadan kalıcı faaliyet sürümü kontrol edilir. Faaliyet yeniden değişmişse eski tarihle gönderime devam edilmez; kayıt tamamlandığı ve bildirimlerin kontrol edilmesi gerektiği kullanıcıya bildirilir.

`SendCalendarUpdates`, katılımcıları mevcut `TakvimDaveti=Gönderildi` bilgisiyle seçer. Kişi bazında tekrarlar ve aynı e-posta adresleri tekilleştirilir. Özel kalem ve katılımcı aynı adrese sahipse tek deneme yapılır. İptal edilmiş faaliyet için CANCEL gönderilir. Bir alıcıda eksik kişi/e-posta, geçersiz adres veya SMTP hatası oluşursa teknik log tutulur; diğer alıcılar denenir. Ekranda alıcı başına popup çıkmaz. Tüm alıcı denemeleri sonunda gerekirse tek “tarih güncellendi, bazı bildirimler tamamlanamadı” uyarısı gösterilir. Başarısız yenileme de ayrı teknik loga gider; ikinci kullanıcı mesajı oluşturmaz.

Gönderilmiş katılımcı işareti bu güncellemede yeniden yazılmaz; yeni davet gönderme akışı değiştirilmez. Otomatik yeniden gönderim, outbox ve SMTP için tam bir kez teslim garantisi eklenmedi. Eşzamanlı bir sonraki değişiklik, ilk sürüm kontrolünden sonra yine başlayabilir; transaction dışı e-posta gönderimi nedeniyle bu pencere tamamen kapanmaz. Başarısız/şüpheli gönderimde faaliyeti tekrar taşımadan önce alıcı takvimleri ve teknik log kontrol edilmelidir.

## Kullanıcının derleme ve test senaryoları

1. Visual Studio 2026'da solution'ı derleyip Test sunucusunda yayımlayın. Yeni DAO/Model C# dosyalarının Compile kayıtları eklendi. ASCX veya `*.ascx.g.cs` değiştirilmedi; yeni kontrol ve bu dal için kod üretimi gereksinimi yoktur.
2. Maaş artışını küçük bir test grubuyla oluşturun. Yeni grubun tüm derece/kademe satırları, yüzdeleri, AGİ ve tarihleri doğru olmalı. Başarı mesajı bir kez görünmeli. İşlem sonrasında aynı sayfa güncel kalmalı.
3. Kontrollü bir INSERT veya audit hatasını grubun ortasında tetikleyin. Yeni grubun hiçbir satırı kalmamalı; tek güvenli hata/başvuru kodu görülmeli. Ham SQL/exception/stack trace kullanıcıya çıkmamalı. İşlem koduyla teknik logu bulun.
4. Son gruba maaş hareketleri ekleyip silmeyi deneyin. Başarılı durumda ücret tanımları ve bağlı hareketler birlikte silinmeli. Silme/audit aşamalarında kontrollü hata olduğunda iki tablo da eski halinde kalmalı. İlk ve tek grup birden çok satırlı olsa da silinememeli.
5. İki sekmede aynı son grubu açın. İlkinde artış/silme yapın; ikinci sekmedeki eski onay “kayıt değişti” uyarısı vermeli ve hiçbir veri değiştirmemeli. Aynı onay isteğini tekrar gönderin; ikinci yeni grup veya daha yeni grubun silinmesi oluşmamalı.
6. Boş/geçersiz tarih, ters aralık, mevcut dönemle çakışma, negatif/geçersiz yüzde veya AGİ deneyin. Anlamlı tek uyarı olmalı; teknik hata ve başarı popup'ı oluşmamalı. Kapat işlemi hata kaydı üretmemeli.
7. MFYS Aylık Para Girişleri'nde ana sebebi kira olan, 100 TL kira + 50 TL teminat olarak ayrıştırılmış bir kayıt kullanın. Kira filtresinde yalnızca 100 TL, teminat filtresinde yalnızca 50 TL, Hepsi'nde toplam 150 TL beklenir. Ayrıştırılmamış ödeme de kendi sebebi filtresinde görünmeli. Liste/alt toplam/dışa aktarma aynı sonucu taşımalı.
8. Bir MTS faaliyetinde iki katılımcıya davet gönderin; üçüncü katılımcıyı davetsiz bırakın. Hem aylık hem haftalık takvimde tarih taşıma ve süre değişimini deneyin. İki mevcut alıcı aynı UID ile güncel tarih almalı; üçüncü kişiye yeni davet gitmemeli. Özel kalem seçimi açık/kapalı durumunu ayrı kontrol edin.
9. Bir alıcının adresini geçersiz bırakın veya kontrollü SMTP hatası oluşturun. Diğer alıcıların denemeleri sürmeli; veri güncellenmiş kalmalı ve kullanıcı yalnızca tek kısmi bildirim uyarısı almalı. Aynı adrese bağlı iki katılımda tekrar gönderim olmamalı. Eksik/silinmiş kişi durumunu da deneyin.
10. MTS'de iptal edilmiş faaliyet için güncellemenin CANCEL olduğunu ve bildirim sonrası ekran yenileme hatasının başarıyı genel kayıt hatası gibi göstermediğini kontrol edin. Stok ve katılım verileri takvim taşıma nedeniyle değişmemeli.

Codex build/test/debug çalıştırmadı; veritabanına iş isteği veya e-posta gönderimi yapmadı. Kaynak, ayraçlar, olay bağlantıları, Compile/referans kayıtları, transaction çağrıları, UTF-8/BOM ve diff statik olarak kontrol edildi. Commit/push/PR/merge kullanıcıya aittir.

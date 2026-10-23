# Portal ana sayfa bileşenleri — log ve mesaj geçişi

Başlangıç: yerel master temiz; GitHub master ile aynı commit: `3b8c501952ecefdd9389f6cdcbd8422d2cca70c5`.
Dal: `codex/portal-anasayfa-log-mesaj`.

## Kapsam ve mesaj değişiklikleri

| WebPart | Değişiklik ve inceleme noktası |
| --- | --- |
| BildirimWP | Oturumdaki personel ve bekleyen görevler Portal log bağlamıyla okunur. Birden fazla birimin görevleri tek bildirimde toplanır; bağlantı güvenli Messages.Info bağlantısıdır. Bekleyen görev olmaması artık “Kullanıcı bilgisi alınamadı” hatası üretmez. Gerçek personel eksikliği ayrı uyarıdır. Popup yalnız ilk açılışta gösterilir. Özellik kapalıysa sorgu yapılmaz. |
| KutlamaMoveWP | Doğum/evlilik, görev, kimlik ve tatil okumaları Portal bağlamını taşır. Teknik hata güvenli tek hata/başvuru koduyla gösterilir; boş liste gibi sunulmaz. Tekrarlanan postback kutlama popup'ını yeniden açmaz. Türkçe kutlama metinleri düzeltildi. Yıl sonu tatilinde gelecek yılın yıldönümü doğru tarihle gösterilir; boş personel adı fotoğraf üretiminde Substring hatası doğurmaz. |
| GunlukNobetcilerMoveWP | SharePoint listesi bir kez okunur; ana görünüm ve popup aynı sonucu kullanır. RowLimit sorgudan önce ayarlanır. Gerçek boş sonuç boş durum alanını açar; erişim/okuma hatası teknik log ve başvuru kodu üretir. Alan adları korunur; bölüm başlıkları Türkçeleştirildi. |
| GunlukYemekListesiMoveWP | Hata yutup null dönen yol kaldırıldı. Boş menü ile SharePoint hatası ayrıldı. Kayıt/kalori yoksa önceki toplam gösterilmez. “Menü tanımlanmamış” mesajı yalnız başarılı boş okumada gösterilir. Yemek isimleri HTML olarak çalıştırılmaz. |
| GununSozuWP | Açılış Activity ve teknik hata sınırı eklendi. Bugüne ait söz yoksa sakin bir boş durum metni gösterilir; liste/erişim hatası boş sonuç sayılmaz. Ekleme yetkisi olmayan kullanıcıda mevcut bağlantı gizleme davranışı korunur. Söz metni HTML olarak çalıştırılmaz. |
| WebPartSearchWP | Ham exception metni kaldırıldı. Boş/geçersiz site adresi ayrı doğrulanır. Önceki sonuçlar arama başında temizlenir. Bazı sayfalar okunamazsa mevcut sonuçlar korunur, “sonuçlar eksik olabilir” ve başvuru kodu gösterilir; sıfır tam sonuç gibi sunulmaz. Ayrıntılar tek AggregateException ile teknik loga gider. Bu ekran mevcut durum etiketini kullanır, aynı hata için ek popup açmaz. |

## Veri erişimi ve sınırlar

Bu altı ekran okuma/gösterim ağırlıklıdır; iş verisi yazma veya SQL transaction eklenmedi. Günün sözü ekleme bağlantısı mevcut SharePoint formunu açar; bu parça o formu değiştirmez. SharePoint liste okumaları SQL trace olarak kaydedilmez; ekran açılışı ve teknik hatalar merkezi loga gider.

Ortak IKYS repository/service sınıflarına gerekli salt okuma overload'ları eklendi. Personel/kimlik/görev tanımı mevcut overload'larında yalnız `context.Project == "Portal"` yolu eski DbClass loglaması yerine açık bağlamlı SQL okuyucuyu kullanır. IKYS ve bağlamsız çağıranların davranışı korunur. SQL trace sorgu metni veya kişi bilgisi taşımaz. Yeni audit olayı gerektiren iş verisi değişikliği yoktur.

Nöbetçi/kutlama ASCX dosyalarında mevcut bağlamaların metin kodlaması düzenlendi; yeni sunucu kontrolü eklenmedi. `*.ascx.g.cs` dosyaları değiştirilmedi. Yeni paket veya DB migration yoktur.

## Yapılandırma

Mevcut `PortalLogging.EnabledWebParts` değerlerini koruyarak ekleyin:

`Portal:BildirimWP;Portal:KutlamaMoveWP;Portal:GunlukNobetcilerMoveWP;Portal:GunlukYemekListesiMoveWP;Portal:GununSozuWP;Portal:WebPartSearchWP`

Portal'ın kalan liste ekranları tamamlanmadığından bu parça için `EnabledProjects` içine Portal eklenmez.

## TEST ve inceleme

1. Ana sayfayı normal kullanıcıyla açın. Altı ekranın açılış bağlamlarını, etkinse SQL trace kayıtlarını kontrol edin. Bildirim özelliği kapalıyken bildirim sorgusu beklenmez.
2. Hiç bekleyen görevi olmayan, bir birimde görevi bulunan ve birden çok birimi yöneten kullanıcılarla deneyin. İlkinde gereksiz hata olmamalı; diğerlerinde tek bildirim ve doğru toplam/bağlantı görülmeli. Personel kaydı eksik kullanıcı ayrı uyarı almalı.
3. Bugün kutlama olan/olmayan günleri, hafta sonu ve resmî tatil öncesini kontrol edin. Yıl sonundan ocak ayına uzanan kutlamanın tarihi yeni yıl olmalı. Bir başka WebPart postback'i kutlama popup'ını yeniden açmamalı.
4. Nöbetçi kaydı olan/olmayan günlerde ana görünüm ve popup aynı kişileri göstermeli. Boş kullanıcı adı veya beklenmeyen login biçimi ekranı düşürmemeli.
5. Menü bulunan, bulunmayan, yemek satırları boş ve kalorisi boş kayıtları deneyin. Boş menüde eski toplam/liste kalmamalı. Kontrollü SharePoint erişim hatasında “menü yok” mesajı yerine tek güvenli hata/başvuru kodu gelmeli.
6. Günün sözü bulunan/bulunmayan gün ve ekleme yetkisi olan/olmayan kullanıcıları deneyin. Okuma erişimi sorunu, boş sözle karıştırılmamalı.
7. Aramada boş alan, geçersiz URL, tam sonuç, sıfır sonuç ve bazı sayfalara erişilemeyen durumları deneyin. Kısmi arama açıkça belirtilmeli; ham hata görünmemeli. Önceki arama satırları yeni boş/hatalı aramada kalmamalı.
8. Kontrollü SQL/SharePoint hatasında OperationId ile teknik kayda ulaşın. Başarısız okuma normal boş sonuç gibi gösterilmemeli; aynı WebPart içinde tek hata mesajı beklenir.

Windows üzerinde build/test/debug çalıştırılmaz. Statik inceleme; UTF-8/BOM, C# ayraç dengesi, proje Compile kayıtları, ASCX olayları, yeni kontrol eklenmemesi ve diff temizliğiyle sınırlıdır. Derleme ve çalışma zamanı doğrulaması Visual Studio/Test sunucusunda kullanıcı tarafından yapılır. Commit/push/PR/merge kullanıcıya aittir.

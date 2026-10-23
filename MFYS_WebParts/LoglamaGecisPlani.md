# MFYS merkezi loglama geçişi

Dal: `codex/mfys-raporlar-loglama`. Başlangıçta yerel master temizdi ve canlı uzak master ile aynı commit'teydi: `ff9e636978f00b22ed9dc891847e30e20186a9fd` (PR #534).

Kullanıcı MTS testini yaparken MFYS'ye devam edilmesini açıkça istedi. MTS'nin test/kullanım değerlendirmesi hâlâ ayrı takip edilmelidir. Bu dal MFYS'nin Feature1 içinde paketlenen dört rapor ekranının tamamını kapsar. Kod hazırlığı tamamlanması, Test sunucusu yayını ve kullanım doğrulaması yerine geçmez.

## Kapsam ve mesaj değişiklikleri

| Ekran | Davranış ve mesaj inceleme noktaları |
| --- | --- |
| AylikParaGirisleriWP | Ödeme sebepleri ve ödeme/ayrıştırma raporu MFYS bağlamıyla okunur. Gün başına ayrı sorgu yerine dönem tek sorguda okunur. `Tüm Aylar` seçilen yılın tamamıdır; ay=0 tarih hatası kaldırıldı. İşlem türü/yıl/ay geçersizse anlamlı tek Türkçe uyarı gösterilir. Toplam aynı sonuç kümesinden hesaplanır. |
| BankaBazindaGunlukBagisListesiWP | BankaGrup2 filtresi ve günlük TL toplamları bağlamlıdır. Tek dönem sorgusu günlere bölünür; kayıtsız günler mevcut davranış gibi sıfır satırı olarak gösterilir. Ayın tüm günleri, artık yıl ve `Tüm Aylar` desteklenir; yıllık rapor için sayfalama kullanılabilir. Banka tanımı olmayan boş durumda null erişimi oluşmaz. |
| BankaBazindaGunlukDovizBagisListesiWP | Dönem/döviz filtresine ait banka grupları ve bağışlar bağlamlı okunur. Banka listesi yeniden oluşturulurken önceki banka seçimi korunur; döviz seçimi banka anahtarı gibi kullanılmaz. Boş sonuçta döviz ve TL toplamları sıfırlanır. Eski RowGroup demo kodu (tanımsız salaryAvg, yanlış kolonlar ve İngilizce ortalama satırları) kaldırıldı. |
| TarihBazindaGunlukBagisListesiWP | Gün, banka grubu ve döviz seçimiyle hesap bazındaki rapor tek sorguda okunur. Liste ve toplam için iki ayrı sorgu yoktur. Boş/geçersiz tarih `gg.aa.yyyy` biçimini belirten uyarıdır. Son günün saatli kayıtları kapsanır. TL karşılığı ve gerçek döviz tutarı ayrı gösterilir. |

Dört ekranda ham exception/SQL/stack trace kullanıcıya gösterilmez. Teknik hata, tek güvenli “Rapor hazırlanamadı” mesajı ve başvuru kodu üretir; ayrıntılar aynı OperationId ile teknik loga gider. Beklenen giriş hataları teknik hata gibi loglanmaz; anlamlı uyarı gösterilir. Başarısız sorgu boş rapor veya sıfır toplam gibi sunulmaz: toplam temizlenir, önceki tablo ve dışa aktarma düğmeleri gizlenir. Başarılı boş sonuç geçerli boş tablo ve sıfır toplamdır. Yeni başarı popup'ı eklenmedi; bunlar salt okuma ekranlarıdır.

İlk açılışta WebPartOpened kaydı, her sorguda repository/metot/süre/sonuç SQL trace'i MFYS projesi ve ilgili ekran adıyla yazılır. SQL metni, parametreler, kişi/ad/açıklama alanları SQL trace'e eklenmez. LogIzleme'nin mevcut proje filtresinde MFYS zaten bulunuyor.

## Okuma altyapısı ve hesaplama sınırları

`DAO/Repositories/MFYS/MfysReportRepository.cs` ile `Model/Services/MFYS/MfysReportService.cs` eklendi. MFYS'ye özgü salt okuma repository'si NBYS ve TBYS tablolarını ortak PortalSqlReadExecutor üzerinden sorgular. NBYS/TBYS'nin yazma veya proje kapsamı kontrolleri genişletilmedi. Eski rapor servisleri değiştirilmedi; diğer çağıranlar mevcut davranışını sürdürür.

Raporlarda iş verisi INSERT/UPDATE/DELETE işlemi bulunmadığından yeni iş transaction'ı veya audit yazısı gerekmiyor. WebPart açılış ve teknik logları mevcut merkezi altyapıyı kullanır. MFYS_WebParts projesine bu altyapı için doğrudan DAO proje referansı eklendi; paket zaten DAO/Model/Utility assembly'lerini içeriyor.

Tarih filtreleri parametreli `>= başlangıç AND < sonraki gün/ay/yıl` aralığı kullanır. Böylece `23:59` sonrasındaki kayıtlar ve ayın son günü kaybolmaz. İlk açılışta güncel ay/yıl, günlük raporda önceki gün seçilidir. Aylar Türkçe ve Ocak–Aralık sırasındadır; yıl seçenekleri önceki davranış gibi bu yıl ve önceki yıldır.

Tutarların inceleme noktaları:

- Günlük banka TL raporunda mevcut `SUM(BagisMiktari + ISNULL(IadeMiktari,0))` hesabı korundu. Döviz detay raporunda mevcut `DovizTutari`, `DovizKuru` ve `BagisMiktari` alanları kullanılır. İade işareti veya kur hesaplama kuralı değiştirilmedi.
- Günlük hesap raporunda TL toplamı `BagisMiktari`, döviz toplamı `DovizTutari` alanından gelir. Eski ekran aynı TL miktarını seçili döviz etiketiyle tekrar gösteriyordu. TL seçiminde iki tutar TL olarak tutarlıdır. Özellikle dövizli kayıtlarda iki alanı kaynak kayıtla karşılaştırın.
- Aylık para girişlerinde ödeme/ayrıştırma join'i korunur. Genel kontrol düzeltmesinde ödeme sebebi filtresi etkin satıra bağlandı: ayrıştırma satırı varsa kendi sebebi, ayrıştırma yoksa ana ödemenin sebebi kullanılır. Ayrıştırılmış tutar varsa `TutarB`, yoksa `TutarA` kullanılır. Ana ödemenin sebebi eşleşti diye başka sebeplere ayrılmış tutarlar artık tek sebep filtresine dahil edilmez. Birden çok ayrıştırma içeren kayıtlar kabul testine dahil edilmelidir.
- Tüm toplamlar biçimlendirilmiş metinlerden değil decimal verilerden hesaplanır; aynı ekrandaki satırların kaynak kümesiyle aynı anda üretilir. Birden çok rapor isteği arasında eşzamanlı yeni veri girişi varsa farklı raporlar doğal olarak farklı anları gösterebilir.

## Tarayıcı ve kod üretimi

Tablo filtreleri postback olayları işlendikten sonra OnPreRender aşamasında bir kez sorgulanır. Ortak DataTables kodu önceki örneği kapatıp yeni veriyle kurar; eski JSON try/catch'inin teknik hatayı yutup yarım tablo oluşturması kaldırıldı. JSON içinde `<`, `>`, `&` ve JavaScript satır ayırıcıları kaçırılır; hücreler DataTables metin render'ıyla gösterilir. HTML içeren hesap adı/açıklama kod çalıştıramaz. Yazdır/Excel/PDF/kopyala başlıkları Türkçedir. DataTables ve Buttons, Responsive, moment tarih sıralama gibi mevcut tarayıcı bağımlılıkları portal tarafından sağlanmaya devam eder.

ASCX veya `*.ascx.g.cs` dosyaları değiştirilmedi; yeni sunucu kontrolü eklenmedi. Bu dal için ASCX yeniden üretimi gerektiren kaynak değişikliği yoktur. Yeni C# dosyalarının DAO/Model/MFYS_WebParts Compile kayıtları eklendi. Kaynak dosyalar UTF-8'dir; düzenlenen mevcut dosyaların BOM durumu korundu.

## Yapılandırma ve kullanıcı testi

Önce kontrollü olarak `PortalLogging.EnabledWebParts` listesine şu anahtarları ekleyin:

`MFYS:AylikParaGirisleriWP;MFYS:BankaBazindaGunlukBagisListesiWP;MFYS:BankaBazindaGunlukDovizBagisListesiWP;MFYS:TarihBazindaGunlukBagisListesiWP`

Mevcut IKYS/TBYS/NBYS/Portal/MTS değerlerini koruyun. MFYS'nin tamamını `EnabledProjects` ile açmayı, Test sunucusu yayını ve değerlendirmesi sonrasında yapın. Yeni şema/migration eklenmedi; mevcut merkezi log tablolarının kurulu olduğu varsayılır.

1. Kullanıcı Visual Studio 2026'da solution'ı derleyip Test sunucusunda yayımlar. Codex build/test/debug çalıştırmadı.
2. Dört raporu açın; WebPartOpened ve SELECT SQL trace kayıtlarının `MFYS`/doğru ekran/metot/süre/sonuç alanlarını kontrol edin. Banka listesi ve ödeme sebebi sorguları da MFYS bağlamında olmalı.
3. Güncel ay, önceki yılın bir ayı, Şubat/29 Şubat, Aralık ve `Tüm Aylar` filtrelerini deneyin. TL raporunda 28/29/30/31 veya 365/366 günlük sıfır/dolu satırlar ve sayfalama doğru olmalı. Yıllık toplamı aylık toplamlarla karşılaştırın.
4. Ay/gün sınırında `00:00`, son gün `23:59:59.997` ve sonraki gün `00:00` kayıtlarını deneyin. Seçili döneme ait saatli kayıtlar dahil, sonraki dönem hariç olmalı. Geçersiz/boş günlük tarih tek Türkçe uyarı vermeli.
5. Dolu döviz raporundan boş döneme/döviz cinsine geçin; önceki toplam kalmamalı. Aynı banka yeni listede varsa seçim korunmalı; yoksa geçerli ilk seçenek seçilmeli. Hiç banka/grup tanımı veya sonuç olmayan durumda ekran düşmemeli.
6. Günlük raporda TL/EUR/USD/GBP ve Tüm Bankalar/tek banka seçin. Hesap satırlarının TL ve döviz tutarlarını kaynak kayıtlarla karşılaştırın; döviz tutarı TL tutarının başka para birimi etiketiyle kopyası olmamalı. Alt toplamlar satırlarla tutarlı olmalı.
7. Aylık para girişinde Hepsi/tek ödeme sebebi, ayrıştırılmamış ve birden çok ayrıştırması olan kayıtları kontrol edin. `TutarB` seçimi ve toplamları mevcut muhasebe beklentisiyle karşılaştırın.
8. `<`, `&`, tırnak, Türkçe karakter içeren hesap adı ve açıklamaları kontrol edin. Metin HTML çalıştırmamalı; tam/async postback, Yenile, tarih sıralama, sayfalama ve yazdır/Excel/PDF/kopyala işlemleri çalışmalı. Döviz raporunda demo ortalama satırı/JavaScript hatası olmamalı.
9. Kontrollü SQL erişim hatasında tek güvenli mesaj ve başvuru kodu beklenir. Eski tablo/yanlış sıfır toplam sunulmamalı; dışa aktarma kapalı olmalı. LogIzleme'den aynı işlem koduyla teknik ayrıntıyı bulun. Sorun giderildikten sonra Yenile veya geçerli filtreyle rapor yeniden açılabilmeli.
10. Kapat yönlendirmesi teknik hata üretmemeli. Sonuçları değerlendirip MFYS geçişinin kabulünü bildirin; MTS test durumunu ayrıca kapatın.

Statik inceleme: C# ayraçları, UTF-8/BOM, proje Compile/referans kayıtları, ASCX olay bağlantıları, sorgu parametreleri, salt okuma kapsamı ve diff temizliği. Derleme ve çalışma zamanı doğrulaması kullanıcıya aittir.

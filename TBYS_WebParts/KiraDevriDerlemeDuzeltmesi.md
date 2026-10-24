# TBYS kira devri derleme düzeltmesi

Tarih: 24.10.2026
Dal: codex/tbys-kira-devri-derleme-duzeltme
Başlangıç master: 47390d3de7928836c0ca7781cf9e7d5d6303d59f

Yerel master temiz ve canlı uzak master ile aynı commit'te doğrulandı. Portal ikinci parçaya geçilmeden önce kullanıcının tüm solution derlemesinde bildirdiği dört TBYS hatası düzeltildi.

## Neden ve değişiklik

- Model/TBYS/KiraBakiyeDevriOtomatik.cs içindeki KiraBakiyeDevriSonuc, önceki TBYS kapanışında IslenenSayisi, AtlananSayisi ve HataliSayisi özelliklerini kaybetmişti. DevirGerekenSozlesmelerWP.TumunuDevirAlNowBtn_Click hâlâ bunları okuyordu. Özellikler ilgili listelerin/dictionary'nin Count değerinden türeyen salt okunur getter'lar olarak geri eklendi; ayrı sayaç durumu tutulmaz.
- TBYS_WebParts/KiraBakiyeDevriWP/KiraBakiyeDevriWP.ascx.cs içindeki Page_Load, kaldırılmış OdemeleriHesaplaOdemePlaniniGuncelle(kiraci.Id) yardımcısını çağırıyordu. Ekranın açılışta hesaplama davranışı kaldırılmadı; yardımcı tamamlandı.
- Yardımcı, kiracının sözleşmelerini GetAllByKiraciId(kiraciId, transaction) ile okur, başlangıç tarihine göre sıralar ve BakiyeBorcHesapla(sozlesme, LoggingContext, transaction) çağırır. Tüm sözleşmelerin hesaplama yazmaları ve audit'i aynı SqlTransactionContext üzerinden yürür. Başarısız hesaplama Türkçe istisna üretir; Complete çağrılmadan Dispose ile tüm kiracı hesaplaması geri alınır.
- Eski yardımcı her sözleşmeyi ayrı transaction açabilen overload ile hesaplıyordu. Yeniden eklenen yardımcı kiracı bazında tek transaction kullanır; bir sonraki sözleşmedeki hata önceki sözleşmelerin hesaplamasını kısmen kalıcı bırakmaz.
- Mevcut DevirAl transaction zinciri, toplu devir sonuç sınıflandırması, teknik hata/SQL kayıtlarının bağımsız yazımı ve audit alan kapsamı değiştirilmedi. Eski loglama yolu geri eklenmedi.
- *.ascx.g.cs, ASCX kontrol/handler ve proje kayıtları değiştirilmedi.

## Statik kontroller

İki C# dosyası Roslyn ParseText/GetDiagnostics ile yalnız sözdizimi açısından incelendi; hata yok. Bildirilen dört sembolün tanımları, üç sayacın kaynak koleksiyonları, kullanılan transaction overload'ları, aynı transaction ile ödeme planı güncelleme/audit zinciri ve TBYS_WebParts'ın DAO proje referansı kaynak üzerinden doğrulandı. UTF-8/BOM/CRLF korundu; git diff --check temiz.

Önceki kaynak/argüman sayısı taraması bu üye eksikliklerini yakalamamıştı. Bu düzeltmedeki kaynak kontrolleri de tüm solution'ın semantik derlemesinin yerine geçmez. Windows'ta build, test, debug veya runtime çalıştırılmadı.

## Kullanıcının uygulayacağı kontroller

1. Visual Studio 2026'da tüm solution'ı yeniden derleyin; ekrandaki üç CS1061 ve bir CS0103 hatasının kalktığını doğrulayın.
2. Kira Bakiye Devri ekranını önceki/yeni birden fazla sözleşmesi olan kiracıyla açın. Aylık ve günlük gecikme zamlı sözleşmelerin ödeme planı ve gösterilen devir tutarlarını karşılaştırın. Sözleşmesiz kiracıda hata olmamalı.
3. Sonraki bir sözleşmenin hesaplama/ödeme planı/audit yazımında kontrollü hata uygulayın: o kiracının bu açılıştaki bütün hesaplama yazmaları ve audit'i geri alınmalı; kısmi başarılı hesaplama kalmamalı.
4. Devir Gereken Sözleşmeler ekranındaki toplu devirde işlenen, atlanan ve hata sonuçlarını deneyin. Üç sayı ilgili kimlik listesi/hata sözlüğünün sayısıyla eşleşmeli.
5. Log açık ve tamamen kapalı durumda aynı akışları tekrarlayın; iş transaction'ları sürmeli, eski Olay_Table/SQLLog_Table yazıcısına dönüş olmamalı. İş rollback'inde bağımsız teknik kayıtlar kalabilir.

Commit, push, PR, merge, yayın ve çalışma zamanı kabulü kullanıcıya aittir. Veritabanına bağlanılmadı; tablo veya geçmiş veri değiştirilmedi.

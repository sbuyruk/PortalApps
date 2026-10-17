# PortalApps proje kuralları

## Üretilen ASCX dosyaları — kesin kural

- `*.ascx.g.cs` dosyalarını ASLA elle düzenleme. Bu yasak tüm proje ve alt dizinleri için geçerlidir.
- Bu dosyalara elle patch uygulama; script, metin değiştirme veya başka bir araçla doğrudan içerik yazma.
- Gerekli değişiklikleri kaynak `.ascx`, `.ascx.cs` veya ilgili kaynak kontrol dosyalarında yap.
- `*.ascx.g.cs` çıktıları yalnızca ilgili SharePoint/Visual Studio kod üreticisiyle (örneğin `.ascx` üzerinde Run Custom Tool) yeniden üretilebilir.
- Kod üreticisi mevcut değilse üretilen dosyayı değiştirme; yeniden üretimin gerekli olduğunu kullanıcıya bildir. Elle düzenlemeyi alternatif olarak önerme.

## SqlTransactionContext — transaction bütünlüğü kuralı

- `SqlTransactionContext transaction` parametresi alan bir metot, veritabanına yaptığı tüm `SELECT`, `INSERT`, `UPDATE` ve `DELETE` işlemlerini aynı transaction üzerinden çalıştırmalıdır.
- Bu metotların içinden transaction parametresi olmayan repository/service metotları, `new DbClass()` veya ayrı bağlantı açan başka bir DB erişimi çağrılamaz.
- Transaction parametresi nullable olan legacy yardımcı metotlarda yalnızca `transaction == null` dalı eski bağlantıyı kullanabilir; transaction verilmiş dalda tüm iç çağrılar transaction-aware overload olmalıdır.
- Olay/audit kayıtları da aynı transaction üzerinden yazılmalıdır. `NOLOCK` kullanılarak bu kural aşılamaz.

## Loglama geçişi çalışma düzeni

- Proje sırası: `IKYS → TBYS → NBYS → Portal → MTS → MFYS`. Bir proje tamamlanıp test ortamında yayımlandıktan ve kullanım sonucu değerlendirildikten sonra sıradaki projeye geç.
- Her kod değişikliği parçasından önce yerel `master` dalının temiz olduğunu ve uzak `master` ile aynı commit'te bulunduğunu kontrol edip kullanıcıya bildir.
- Kod değişikliklerini incelemeye uygun büyüklükte parçalara böl ve her parça için yeni bir yerel dal aç. Değişiklikler tamamlandığında dalı ve inceleme noktalarını kullanıcıya bildir.
- Commit, push, GitHub PR açma ve birleştirme işlemlerini kullanıcı yapar. Kullanıcı sonrasında Windows 11 bilgisayarında ve Test sunucusunda `master` dalına geçip pull yapar; Test sunucusunda yayımlar ve test sonucunu bildirir. Kullanıcı test yapmadan da sonraki parçaya geçilmesini isteyebilir.
- Windows 11 bilgisayarında Codex test, build veya debug çalıştırmaz. Bu işlemler kullanıcı tarafından Visual Studio 2026 ve/veya Test sunucusunda yapılır.
- Son kullanıcıya gösterilen tüm yeni mesajlar Türkçe olmalıdır. Kullanıcıya ulaşabilecek hata/istisna metinlerini de Türkçe yaz; geliştiriciye yönelik kod yorumları, teknik log alan adları ve tanılama bilgileri İngilizce olabilir. Ham teknik istisna metnini kullanıcıya gösterme.
- Solution içindeki kaynak ve metin dosyaları UTF-8 kodlamasında olmalıdır. Yeni dosyaları UTF-8 oluştur; düzenlenen dosyaların mevcut BOM kullanımını gereksiz yere değiştirme.

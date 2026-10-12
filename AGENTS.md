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

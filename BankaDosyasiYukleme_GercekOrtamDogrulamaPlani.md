# BankaDosyasiYuklemeWP gerçek ortam doğrulama planı

Bu plan kod veya veritabanı değiştirmez. Test sunucusunda, düşük riskli bir test tarihi ve yalnızca test banka dosyalarıyla uygulanmalıdır. Üretim verisi üzerinde çalıştırılmamalıdır.

## Ön koşullar

- Uygulama ve veritabanı sürümünün aynı `master` commit’inden dağıtıldığını doğrulayın.
- Test kullanıcısının `EkstreAktarma_Table` üzerinde beklenen INSERT/SELECT yetkileri ve `sp_getapplock` çalıştırma yetkisiyle bağlantı açabildiğini doğrulayın.
- Test tarihini ve banka adını diğer kullanıcıların işlem yapmadığı bir değer seçin.
- Test öncesi ve sonrası şu sayımı kaydedin:

```sql
SELECT COUNT_BIG(*) AS RowCount
FROM dbo.EkstreAktarma_Table
WHERE IslemTarihi = @TestIslemTarihi AND BankaAdi = @TestBankaAdi;
```

## 1. İki eşzamanlı istek ve dosya başına kilit

1. İki ayrı tarayıcı oturumunda aynı tarih için farklı, geçerli test dosyası seçin.
2. İki `Kaydet` isteğini mümkün olduğunca aynı anda gönderin.
3. Uygulama loglarında iki isteğin de `PortalApps.EkstreAktarma.Import` kaynağıyla ilişkisini ve sürelerini kaydedin.
4. Sonuç tablosunu ve dosya sonuç mesajlarını kontrol edin.

Beklenen sonuçlar:

- İsteklerden biri kilidi alır ve transaction tamamlanana kadar diğer istek bekler.
- İkinci istek kilit zaman aşımına uğrarsa kullanıcıya güvenli bekleme/tekrar deneme mesajı gösterilir; ham SQL veya stack trace gösterilmez.
- Aynı satırlar iki kez oluşmaz; başarılı dosyanın satırları tek transaction olarak görünür.
- Bir dosyanın sonucu diğer dosyanın transaction sonucunu değiştirmez.

## 2. Kilit yetkisi ve zaman aşımı

Yetki doğrulamasını uygulama bağlantısının kullandığı aynı hesapla yapın:

```sql
DECLARE @result int;
BEGIN TRANSACTION;
EXEC @result = sys.sp_getapplock
    @Resource = N'PortalApps.EkstreAktarma.Import',
    @LockMode = N'Exclusive',
    @LockOwner = N'Transaction',
    @LockTimeout = 1000;
SELECT @result AS AppLockResult;
ROLLBACK TRANSACTION;
```

Beklenen sonuç `0` veya `1`’dir. `-1` timeout, `-2` request cancelled, `-3` deadlock, `-999` ise yetki/parametre/başka API hatasıdır. Uygulama bağlantısının yetkisi yoksa bunu başarılı test saymayın; DBA ile yetkiyi düzeltin.

Zaman aşımını doğrulamak için ilk SSMS oturumunda yukarıdaki lock’u açık transaction içinde tutun; ikinci oturumdan gerçek yükleme isteği gönderin. İkinci isteğin yaklaşık 30 saniye içinde kontrollü sonuç vermesi ve transaction’ın açık kalmaması beklenir. İlk oturumdaki transaction’ı mutlaka rollback edin.

## 3. Hata sonrası rollback

1. Yeni ve mükerrer satırları birlikte içeren geçerli bir dosyanın satır sayısını bilin.
2. Aynı dosyaya, ortasında kesin biçimde hatalı bir satır bulunan test kopyası hazırlayın.
3. Yükleme öncesi, yükleme sonrası ve hata mesajından sonra `EkstreAktarma_Table` sayımını karşılaştırın.

Beklenen sonuçlar:

- Hatalı dosyada daha önce işlenmiş satırlar mükerrer olarak raporlanabilir; ancak dosyada yeni eklenen satırlar commit edilmez.
- `SuccessCount` kullanıcıya başarı olarak gösterilmez; uygulama sonucu rollback olarak görünür.
- Hata sonrası aynı tarih/banka için yeni ve temiz bir dosya tekrar yüklenebilir.
- `EkstreAktarma_Table` ve aynı transaction’a bağlı audit/event kayıtlarında kısmi kayıt kalmaz.

## 4. Başka kod yollarından yazma kontrolü

- Kaynak kodda `EkstreAktarma_Table` ve `EkstreAktarmaRepository.Insert`/`Update` kullanan tüm yolları envanterleyin. Transaction parametresi alan yolların `SqlTransactionContext` overload kullandığını doğrulayın; transaction dışı yolların bu yükleme testiyle eşzamanlı çalışmadığını ayrıca belgeleyin.
- Test sırasında veritabanı erişimini izlemek için DBA tarafından kısa süreli Extended Events oturumu açılmasını isteyin. `sql_statement_completed` olayında `database_name`, `client_app_name`, `session_id`, `sql_text` ve `username` alanlarını kaydedin; yalnızca hedef test tarihi/banka ile ilişkili ifadeleri filtreleyin.
- İz kaydında `EkstreAktarma_Table` için yükleme transaction’ı dışındaki INSERT/UPDATE/DELETE görülürse kaynağı ve transaction bağını belirlemeden testi başarılı saymayın.
- Test bitince Extended Events oturumunu kapatın ve çıktıyı saklayın; tabloya doğrudan test INSERT’i yapmayın.

## Sonuç kaydı

Her senaryo için tarih/saat, kullanıcı, test dosyası adı, `AppLockResult`, başlangıç-bitiş satır sayıları, uygulama mesajı, log korelasyon bilgisi ve beklenen/gerçekleşen sonucu kaydedin. Veritabanı veya test sunucusu erişimi yoksa bu plan çalıştırılmış sayılmaz.

# Ortak eski DB altyapısı temizliği — parça 4

Tarih: 2026-10-25  
Dal: `codex/ortak-eski-db-altyapi-temizligi`  
Başlangıç: `5d5ae2510f01afc2a00a23af204159e6cf413ba4`

Kullanıcının devam talebiyle başlandı. Önceki parçanın Test kabulü bu sohbette ayrıca bildirilmedi. Değişiklik öncesinde yerel master temizdi ve GitHub üzerinden sorgulanan uzak master ile aynı commit'teydi.

## Kaldırılan parçalar

| Parça | Kaldırma gerekçesi |
| --- | --- |
| DAO/Ortak/DBObject.cs ve DbParam.cs | DBObject yalnızca kullanılmayan eski transaction yürütücüsünde, DbParam yalnızca DBObject içinde kullanılıyordu. |
| DbClass.ExecuteTransaction() ve ExecuteTransaction(List<SqlQuery>) | Kaynakta dış çağrıları yoktu. SqlTransactionContext, SQL izi ve audit akışı dışında kalan iki eski yürütücüydü. DBObjectList, DBTransaction, kullanılmayan created alanı ve eski yorum transaction taslağı da kaldırıldı. |
| ProjeConstants içindeki yedi SQL işlem sabiti | SQL_SELECT, SQL_UPDATE, SQL_DELETE, SQL_INSERT, SQL_BOS ve SQL_GENERIC_INT_VALUE yalnızca eski yürütücü içindi; SQL_SELECTWITHFILTER kullanılmıyordu. |
| SilinenKayitService ve SilinenKayitRepository | Güncel tüketicileri yoktu; eski bağımsız bağlantılı yazma yolunu tutuyorlardı. İki dosya ve Compile kayıtları kaldırıldı. |
| OrtakParametreService.Save/Update/Delete ve OrtakParametreRepository.Insert/Update/Delete | Kaynakta tüketicileri yoktu. Transaction/audit dışındaki yazma yolları kaldırıldı; repository'nin yalnızca bu metotlar için kullandığı queryBuilder alanı ve kurulumu da çıkarıldı. |
| HelperFunctions.ConvertTextFileToList(Stream, Encoding) | Tüketicisi olmayan overload, hatada ex.Message gösterip boş/kısmi sonuç döndürüyordu. Kullanılan tek parametreli overload korunur. |

Toplam dört sınıf dosyası ve bunlara ait dört Compile girdisi kaldırıldı. Düzenlenen dosyalarda yalnızca silme yapıldı.

## Korunan yollar

DbClass'ın diğer overload'ları bu parçanın kapsamında değildir ve gövdeleri değişmedi. SqlTransactionContext, güncel repository/service yazıcıları ve teknik log/audit kodu değiştirilmedi.

SilinenKayit entity'si ve SilinenKayit_Table korunur. NBYS bağış ve ekstre silme akışları bu entity'yi NbysBagisciWriteRepository.SaveArchive üzerinden mevcut transaction ve audit düzeniyle kullanmaya devam eder. Veritabanı nesnesi, veri, şema veya yetki silinmedi; migration eklenmedi.

OrtakParametreService/Repository sınıfları ve okuma metotları korunur. UtilityHelper.ParametreDegeriSorgula ve PortalToplantiNotificationService içindeki bağlamlı GetByKey çağrıları aktiftir. HelperFunctions'ın tek parametreli dosya okuma overload'ını kullanan dokuz banka aktarım çağrısı korunur.

## Statik inceleme

- Takip edilen C#, ASCX, proje ve konfigürasyon kaynaklarında kaldırılan sınıf, işlem sabiti ve ExecuteTransaction referansı kalmadı. Tarihsel Markdown belgeleri değiştirilmedi.
- Beş düzenlenen C# kaynağı Roslyn ile yalnızca sözdizimi bakımından incelendi: 0 hata.
- Bu kaynaklarda korunan 65 metodun gövdesi önceki sürümle aynı. OrtakParametreRepository kurucusundan yalnızca artık kullanılmayan queryBuilder kurulumu çıkarıldı.
- Dokuz projedeki 798 Compile yolu incelendi: eksik kaynak yok.
- Düzenlenen yedi dosyada UTF-8/BOM korunumu ve git diff --check kontrol edildi.
- ASCX ve üretilen .ascx.g.cs değiştirilmedi; kod üretimi gerekmez.
- Build, test, debug ve semantic compilation çalıştırılmadı.

## Yayımlama ve kullanıcı kabulü

Public sınıf/metot/sabitler kaldırıldığı için Visual Studio 2026'da solution'ı derleyin; güncel DAO, Model ve Utility assembly'lerini uyumlu sürümlerle Test ortamına yayımlayın. Depo dışında bu eski API'leri kullanan bir derlenmiş tüketici varsa kaynak güncellemesi ve yeniden derleme gerekir.

1. Solution derlemesinde kaldırılan sınıf/metot/sabitlere bağımlılık hatası bulunmadığını doğrulayın.
2. IKYS bildirimleri ve Portal toplantı bildirimlerinde ortak parametre/SMTP okumalarını deneyin. Bağlamlı SELECT izi ve mevcut hata davranışı korunmalıdır.
3. NBYS ekstre ve bağış silme/arşivleme akışlarını kontrollü kayıtlarla deneyin. SilinenKayit_Table kaydı ile iş değişikliği/audit aynı transaction'da tamamlanmalı; kontrollü rollback'te birlikte geri alınmalıdır.
4. Banka metin dosyası aktarımında kullanılan tek parametreli okuma yolunu normal ve bozuk dosyayla kontrol edin; aktarımın mevcut sonuç/hata sözleşmesi korunmalıdır.
5. Korunan DbClass okumalarını kullanan temsilî bir listeyi ve merkezi log açık/kapalı temsilî iş yazımını kontrol edin. Eski yürütücülere dönüş yolu olmamalıdır.

Commit, push, PR ve birleştirme kullanıcı tarafından yapılır. Sonraki entity/bağlam temizliği parçasına Test sonucu veya açık devam talebi sonrasında geçilir.

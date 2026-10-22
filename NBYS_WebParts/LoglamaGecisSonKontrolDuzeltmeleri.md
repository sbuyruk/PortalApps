# NBYS son kontrol düzeltmeleri

Başlangıç: temiz yerel master ve uzak master `f8c408811c96fc8dcdde0f75028f7256b7622ee3` (#522).
Dal: `codex/nbys-son-kontrol-log-rapor-duzeltme`.

## Ekstre ekleme logu

EkstreAktarmaService.Save(item, transaction), bağlamlı repository merkezî audit yazdıktan sonra eski OlayKayit girişini de çağırıyordu. Diğer transaction servisleriyle aynı `!repository.HasLogContext` koşulu eklendi. Bağlamlı eklemede yalnızca merkezî audit yolu kullanılır; bağlamsız eski tüketici kendi transaction'ında eski olay kaydını korur. Transaction veya commit sınırı değiştirilmedi. Log yapılandırması kapalıysa bağlamlı çağrı eski loga geri dönmez; mevcut geçiş yaklaşımı korunur.

## Döviz bağış raporu tarih aralığı

DovizleYapilanBagislarWP yıllık seçimde 1 Aralık'ta kesiliyor, aylık seçimde son gün 00:00 sonrasını dışarıda bırakıyordu. Rapor artık başlangıç dahil, bitiş hariç aralık kullanır: ayın ilk anından sonraki ayın ilk anına; yıllık seçimde 1 Ocak'tan sonraki 1 Ocak'a kadar.

Bağlamlı servis/repository çağrısına açık `endExclusive: true` seçeneği iletilir. Bu yolda SQL koşulu `< @Bitis` olur; tarih parametreleri metne çevrilmeden DateTime olarak gönderilir. Diğer mevcut çağrılarda varsayılan dahil bitiş davranışı korunur. Banka ve döviz filtreleri ile toplama kuralları değiştirilmedi.

## Kullanıcı mesajları

Bu parçada yeni veya değişen kullanıcı mesajı yoktur. Ekstrede mevcut güvenli teknik hata/başvuru kodu ve commit sonrası sonuç akışı korunur. Döviz raporunda değişen yalnızca dönem kapsamı ve buna bağlı toplamdır.

## Kullanıcının TEST kontrolü

1. Bağlamlı manuel ekstre ekleme veya banka dosyası yükleme sonrası merkezî INSERT audit ve alan farklarını kontrol edin; aynı ekleme için eski Olay tablosunda yeni giriş oluşmamalı. Güncelleme ve silmede mevcut davranış sürmeli. Audit'in görünmesi ilgili log yapılandırmasının açık olmasına bağlıdır.
2. Aralık aylık seçiminde 31 Aralık gününün saatli kayıtları dahil, sonraki 1 Ocak 00:00 kaydı hariç olmalı.
3. Yıllık seçimde 1 Aralık sonrası ve 31 Aralık saatli kayıtları dahil, sonraki yılın ilk kaydı hariç olmalı.
4. Şubat, artık yıl ve 30 gün süren ay seçimlerinde son gün kayıtlarını; başlangıç anındaki kaydı ve sonraki dönem başlangıcını karşılaştırın. Banka filtresi ve döviz toplamlarını kaynak kayıtlarla doğrulayın.

Codex build/test/debug çalıştırmadı ve veritabanına iş isteği göndermedi. Diff, çağrı zinciri ve UTF-8/BOM kaynak kontrolleri yapıldı. Üretilen ASCX dosyaları değiştirilmedi. Commit/push/PR/merge kullanıcı tarafından yapılır.

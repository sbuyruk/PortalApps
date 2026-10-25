# NBYS bölge log bağlamı temizliği — parça 3

Tarih: 2026-10-25  
Dal: `codex/nbys-bolge-log-baglami`  
Başlangıç: `bafd6018e00b318e10673880489beffca2305307`

Kullanıcının devam talebiyle başlandı. TBYS Test kabulü bu sohbette ayrıca bildirilmedi. Değişiklik öncesinde yerel master temizdi ve GitHub üzerinden sorgulanan uzak master ile aynı commit'teydi.

## Değişiklik

NakitBagisciListesiByYilAyWP ilk açılışta bölgeyi artık PersonelItem/IsBilgileriItem özelliklerinin bağlamsız sorgularından almaz. WebPart içindeki KullaniciBolgesiniGetir, personel, iş bilgileri, birim ve bölgeyi aynı NBYS log bağlamıyla okur. Her aşamada mevcut bağlamlı servis/repository yolu kullanılır; SQL izleri bu WebPart'ın OperationId değeriyle ilişkilendirilir.

Eksik personel/iş bilgileri/birim/bölge veya geçersiz bölge kimliğinde yükleme durur. Eksiklik 0 değerine çevrilmez: GetByRegionDate içinde 0 tüm bölgeler filtresidir. Hata mevcut ExecuteAction sınırına yükselir; NbysWebPartLog tek teknik hata ve başvuru kodlu Türkçe genel mesaj üretir. Ham istisna metni gösterilmez. Aynı istekte sonraki olayların çalışmasını önleyen mevcut mekanizma korunur.

Geçerli bölgenin kimliği ve genel müdürlük için mevcut tüm bölgeler davranışı korunur. Tarih aralığı, bağış sorgusu, filtre sözleşmesi ve tablo/Excel üretimi değiştirilmedi. Bu parça ekranın mevcut yetkilendirme veya postback tasarımının genel denetimi değildir.

Son tüketici kaldırıldığı için Model/IKYS/IKYSOrtak.cs içindeki tek parametreli BolgeGetirByUserName ve PersonelGetir metotları kaldırıldı. İki parametreli bağlamlı metotlar ve TBYS/IKYS tüketicileri korunur. Ortak bağlamlı bölge yardımcısında kalan Model mesaj gösterimi sonraki entity/bağlam temizliği parçasındadır.

## Statik inceleme

- Değişen iki C# kaynağında Roslyn sözdizimi incelemesi: 0 hata.
- Ortak personel/bölge çağrılarının argüman sayıları kaynak üzerinden incelendi; kaldırılan tek parametreli yola tüketici kalmadı.
- UTF-8/BOM korunumu ve git diff --check kontrol edildi.
- ASCX ve üretilen .ascx.g.cs dosyaları değiştirilmedi; kod üretimi gerekmez.
- İş yazımı, transaction ve audit kodu değiştirilmedi. Build, test, debug veya semantic compilation çalıştırılmadı.

## Yayımlama ve kullanıcı kabulü

Public overload'lar kaldırıldığı için Model ve NBYS_WebParts birlikte derlenip uyumlu sürümlerle Test ortamına yayımlanmalıdır. Kaynakta korunan TBYS/IKYS çağrıları bağlamlı overload'ları kullanır. Depo dışındaki derlenmiş bir tüketici eski imzaları kullanıyorsa o tüketici de güncellenmelidir.

1. Bölge kullanıcısı ve genel müdürlük kullanıcısıyla yıl/ay listesini açın. Satır kapsamı, tarih seçimi ve Excel çıktısını önceki sürümle karşılaştırın.
2. İlk açılışta personel, iş bilgileri, birim ve bölge SELECT izlerinin Project=NBYS, WebPart=NakitBagisciListesiByYilAyWP ve aynı OperationId ile oluştuğunu kontrol edin. Diğer mevcut rapor sorguları da iz üretmeye devam eder.
3. Kontrollü olarak personel, iş bilgileri, birim ve bölge kayıtlarını ayrı ayrı eksik bırakın. Liste sorgusu çalışmamalı; tüm bölgeler verisi üretilmemelidir. Tek başvuru kodlu hata beklenir.
4. Bölge okumasında kontrollü SQL hatasında başarısız SELECT izi ve tek teknik hata bekleyin; ham SQL mesajı gösterilmemelidir.
5. Merkezi log kapalıyken geçerli kullanıcının ekranını ve eksik bölge durumunu kontrol edin. Geçerli akış çalışmalı; eksik bilgi yine tüm bölgeler filtresine dönüşmemelidir.

Commit, push, PR ve birleştirme kullanıcı tarafından yapılır. Sonraki ortak eski DB altyapısı parçasına Test sonucu veya açık devam talebi sonrasında geçilir.

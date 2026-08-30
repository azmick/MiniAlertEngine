# MiniAlertEngine
Bu projenin amacı dosya yolunda verilen kurallara ve fiyat akışına göre sinyal üreten bir uyarı motoru geliştirmektir.

Program fiyat ve kural dosyalarını alır. Fiyat dosyasını sırayla gezer ve her saat için her kuralı kontrol eder. 
Eğer bir kuralın koşulları sağlanıyorsa, o kural için bir sinyal üretilir ve bu sinyal kullanıcıya iletilir.

## Proje Yapısı

- `src/AlertEngine` -> kural değerlendirme motorumuz. (class library) Bütün kural mantığı burada.
- `src/AlertEngine.ConsoleApp` -> konsol uygulaması. Kullanıcıdan dosya yolunu alır ve motoru çalıştırır.
- `tests/AlertEngine.Tests` -> motorun testleri. Motorun doğru çalıştığını test eder.

Motor ile konsol bilinçli olarak ayrıldı. Bütün mantık library'de konsol yalnızca dosya okuma ve motoru çalıştırma işini yapar. 
Bu sayede motor başka uygulamalarda da kullanılabilir.

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download) veya üzeri

Proje platformdan bağımsızdır; aynı komutlar Windows, macOS ve Linux'ta çalışır.

## Proje Nasıl Derlenir?

- Projeyi klonlayın veya indirin.
- Terminal veya komut satırında projenin kök dizinine gidin (`.sln` dosyasının bulunduğu klasör).
- Aşağıdaki komutu çalıştırarak projeyi derleyin:

```bash
dotnet build
```

## Proje Nasıl Çalıştırılır?

Program iki argüman alır: önce fiyat dosyasının yolu, sonra kural dosyasının yolu.
Komutu deponun kök dizininden çalıştırın.

**macOS / Linux:**
```bash
dotnet run --project src/AlertEngine.Console -- data/data_prices.json data/rules_rules.json
```

**Windows:**
```powershell
dotnet run --project src/AlertEngine.Console -- data\data_prices.json data\rules_rules.json
```

> Not: `dotnet` ileri slash'ı (`/`) her platformda kabul eder, dolayısıyla macOS
> komutu Windows'ta da çalışır. Yol ayıracı dışında komutlar özdeştir.


## Proje Nasıl Test Edilir ?

Kural mantığının doğruluğunu kontrol eden birim testleri çalıştırmak için:
```bash
dotnet test
```
Tüm testler geçerse `Passed!` özetini ve sıfır başarısız test görürsünüz.

## Belirsiz Durumlar İçin Verilen Kararlar

Ödevde bilerek tanımsız bırakılan bazı durumlar vardı. Aşağıda her biri için
verdiğim kararı ve gerekçesini açıklıyorum.

### `change` kuralı - yüzde değişim hesabı

**Negatif önceki fiyat:** Yüzde değişim `(yeni − eski) / eski` formülüyle hesaplanır ve
bu formül önceki fiyata böler. Veride önceki fiyatın negatif olduğu bir durum var
(13 Ağustos 14:00'de −50). Negatif bir sayıya bölünce sonucun işareti ters döner: örneğin
−50'den 2481.83'e geçiş, fiyat aslında yükseldiği hâlde formülde negatif bir yüzde
(−%5063) verir. İşaretli karşılaştırma yapılsaydı bu devasa hareket eşiği geçemez ve
kaçırılırdı. Bunu önlemek için hesabı mutlak değerle (`abs`) yapıyorum: işaret yok
sayılır, yalnızca orana bakılır

**Sıfır önceki fiyat:** Önceki fiyat 0 ise yüzde değişim matematiksel olarak tanımsızdır.
Oran tanımsız olduğu için kuralı eşleştirmiyorum.

**İlk saat:** Serinin ilk saatinde önceki fiyat yoktur, dolayısıyla hareket tanımlanamaz.
Bu durumda kural eşleşmez.

**"Önceki saat" ne demek:** "Önceki", dizideki bir önceki kayıt değil, zaman olarak tam
bir saat öncesidir. İki kayıt arasındaki fark tam 1 saat değilse, değişimi tanımsız kabul
edip eşleştirmiyorum. Bunun sebebi verideki bir saatlik boşluktur (12 Ağustos 03:00 kaydı
eksik). `change` "saatlik" bir kıyas olduğu için, arada boşluk varsa bu kıyas anlamını
yitirir.

### `streak` kuralı — üst üste hareket sayımı

`streak`, fiyatın üst üste N saat aynı yönde hareket etmesini arar. Birkaç noktası
tanımsızdı:

**"N saat" kaç hareket demek:** `hours: 3` değerini "3 hareket" olarak yorumluyorum,
"3 fiyat noktası" olarak değil. Üç yukarı hareketi görmek için dört fiyat noktası gerekir
(nokta sayısı = hareket sayısı + 1). Bu yorum, kural mesajıyla ("üç saat üst üste arttı")
uyumlu: üç kez "arttı" olayı, üç harekettir.

**Zaman boşluğu:** `change` kuralındaki gibi, "ardışık saat" derken zaman olarak tam bir
saat farkını kastediyorum. Seri içindeki herhangi iki nokta arasında tam 1 saat yoksa
(12 Ağustos'taki boşluk gibi), seri kırılır.

**Yalnızca son pencereye bakar:** `streak` her saatte, yalnızca o ana kadarki son N
hareketi kontrol eder — daha eski hareketler önemli değildir. Kural hiçbir sayaç/durum
tutmaz; her saat geçmişteki son N+1 noktaya bakıp kararını yeniden verir. Bu, kuralı saf
(state'siz) tutar.

### Geçersiz veya bilinmeyen girdiler

Program, bozuk girdiyi sessizce görmezden gelmek yerine erkenden ve anlamlı bir hatayla
durur (fail-fast). Somut kararlar:

- **Bilinmeyen kural tipi:** Kural dosyasında tanımadığım bir `type` (örneğin "banana")
  varsa, o kuralı atlamıyorum; anlamlı bir hata fırlatıp duruyorum. Gerekçem: tanınmayan
  bir tip büyük olasılıkla bir yazım hatası ya da eksik bir tanımdır; bunu sessizce
  atlamak, kullanıcının o kuralın neden çalışmadığını fark etmemesine yol açar.

- **Eksik zorunlu alanlar:** Her kural tipi kendi zorunlu alanlarını doğrular (örneğin
  `threshold` için `operator` ve `value`, `cooldown` için `hours` ve iç `rule`). Eksikse
  hata verilir.

- **Boş dosya:** Fiyat veya kural dosyası boşsa (hiç fiyat / hiç kural içermiyorsa)
  program hata verir, çünkü değerlendirilecek veri yoktur.

### Çıktı formatı — ondalık ayıracı

Fiyatları, çalışılan makinenin bölgesel ayarlarından bağımsız olarak, ondalık ayıracı
nokta olacak şekilde basıyorum (`4200.00`, `4200,00` değil). Bunun sebebi, Türkçe gibi
bazı sistemlerde varsayılan ondalık ayıracının virgül olması ve çıktının ödevdeki
formata uymamasıdır. Çıktının hangi makinede çalışırsa çalışsın aynı ve deterministik
olması için formatı `InvariantCulture` ile sabitledim.

### Bir gözlem: `outside-comfort-zone` kuralı

Bu kural `not( range[1200, 3200] )` olarak tanımlı. `range` "band dışında" ise eşleştiği
için, `not` bunu tersine çevirir ve kural fiyat band **içindeyken** eşleşir. Yani ismi
("comfort zone dışında") davranışıyla ters görünüyor — fiyat 1200–3200 aralığındayken
(çoğu saat) alarm basıyor. Bu, verilen kural dosyasındaki tanımın doğal sonucudur; ben
tanıma sadık kaldım, kuralın ismine göre bir "düzeltme" yapmadım. Çıktıda bu kuralın sık
tetiklenmesinin sebebi budur.

## Daha Fazla Vaktim Olsa

Aşağıdakiler, farkında olduğum ama bu ödevin kapsamı ve süresi (4–6 saat) gereği bilerek
eklemediğim iyileştirmeler:

- **Geçersiz band doğrulaması:** `range` kuralında `min > max` gibi anlamsız bir band
  şu an kontrol edilmiyor (böyle bir band her fiyatı eşleştirir). Kural kurulurken bunu
  doğrulayıp anlamlı bir hata verirdim.

- **Performans (büyük veri için):** Motor şu an her saatte, o ana kadarki tüm geçmişi
  yeniden kopyalıyor. Bir haftalık veri (168 nokta) için bu önemsiz, ama veri büyüdükçe
  verimsizleşir. Bu durumu düzeltmeye çalışırdım.

## Bölüm 4

Bu opsiyonel bölümü, ana uygulamaya ve testlere odaklanmak için bu teslimde bıraktım.
Daha fazla zamanla, ölçeklenebilirlik ve konfigürasyonla genişletme konularını
araştırıp anlayıp cevaplandırabilirdim.
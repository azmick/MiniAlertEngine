using AlertEngine;
using AlertEngine.Json;

// İki dosya yolu argüman olarak bekleniyor: fiyatlar ve kurallar.
if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: AlertEngine.Console <prices.json> <rules.json>");
    return 1;   // hata koduyla çık
}

string pricesPath = args[0];
string rulesPath = args[1];

try
{
    // Dosyaları oku (fail-fast: bozuksa okuyucular anlamlı hata fırlatır).
    var prices = PriceFileReader.ReadFromFile(pricesPath);
    var rules = RuleFileReader.ReadFromFile(rulesPath);

    // Motoru çalıştır: saat saat gez, eşleşen alarmları topla.
    var engine = new Engine(rules);
    var matches = engine.Run(prices);

    // Sonuçları PDF'in tarif ettiği formatta konsola bas.
    foreach (var m in matches)
    {
        Console.WriteLine(
            $"[{m.Timestamp:yyyy-MM-ddTHH:mm:sszzz}] {m.RuleId}: {m.Message} (price: {m.Price:0.00})");
    }

    return 0;   // başarı
}
catch (Exception ex)
{
    // Herhangi bir hata (dosya yok, bozuk JSON, geçersiz kural) → anlamlı mesaj, hata kodu.
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
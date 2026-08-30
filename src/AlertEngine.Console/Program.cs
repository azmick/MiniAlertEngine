using AlertEngine;
using AlertEngine.Json;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: AlertEngine.Console <prices.json> <rules.json>");
    return 1;
}

string pricesPath = args[0];
string rulesPath = args[1];

try
{
    var prices = PriceFileReader.ReadFromFile(pricesPath);
    var rules = RuleFileReader.ReadFromFile(rulesPath);

    var engine = new Engine(rules);
    var matches = engine.Run(prices);

    foreach (var m in matches)
    {
        Console.WriteLine(
            string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            "[{0:yyyy-MM-ddTHH:mm:sszzz}] {1}: {2} (price: {3:0.00})",
            m.Timestamp, m.RuleId, m.Message, m.Price));
    }

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
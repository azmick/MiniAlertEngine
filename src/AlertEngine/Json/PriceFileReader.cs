using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AlertEngine.Json
{
    public static class PriceFileReader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static IReadOnlyList<PricePoint> ReadFromFile(string path)
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            return ReadFromFile(json);
        }

        public static IReadOnlyList<PricePoint> Read(string json)
        {
            var dto = JsonSerializer.Deserialize<PriceFileDto>(json, Options);

            if (dto is null || dto.Prices is null || dto.Prices.Count == 0)
            {
                throw new InvalidDataException("Price file is empty or invalid.");
            }

            return dto.Prices.Select(p => new PricePoint(p.Timestamp, p.Price)).ToList();
        }
    }
}

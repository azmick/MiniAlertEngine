using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace AlertEngine.Json
{
    internal sealed class PriceFileDto
    {
        [JsonPropertyName("prices")]
        public List<PricePointDto> Prices { get; set; } = new();
    }

    internal sealed class PricePointDto
    {
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}

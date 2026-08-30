using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AlertEngine.Json
{
    internal sealed class RuleDto
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        // threshold
        [JsonPropertyName("operator")]
        public string? Operator { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        // range
        [JsonPropertyName("min")]
        public decimal? Min { get; set; }

        [JsonPropertyName("max")]
        public decimal? Max { get; set; }

        // change
        [JsonPropertyName("percent")]
        public decimal? Percent { get; set; }

        // streak
        [JsonPropertyName("direction")]
        public string? Direction { get; set; }

        [JsonPropertyName("hours")]
        public int? Hours { get; set; }   // cooldown da bunu kullanır

        // and / or — birden fazla iç kural
        [JsonPropertyName("rules")]
        public List<RuleDto>? Rules { get; set; }

        // not / cooldown — tek iç kural
        [JsonPropertyName("rule")]
        public RuleDto? Rule { get; set; }

        // Sadece dıştaki kuralın id ve message'ı olur (iç kuralların olmaz).
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}

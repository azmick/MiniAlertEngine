using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AlertEngine.Json
{
    internal sealed class RuleFileDto
    {
        [JsonPropertyName("rules")]
        public List<RuleDto>? Rules { get; set; }
    }

}

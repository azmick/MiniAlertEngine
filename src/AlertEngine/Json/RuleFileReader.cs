using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AlertEngine.Json
{
    public sealed record AlertRule(string Id, string Message, IRule Condition);
    public static class RuleFileReader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static IReadOnlyList<AlertRule> ReadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return Read(json);
        }

        public static IReadOnlyList<AlertRule> Read(string json)
        {
            var dto = JsonSerializer.Deserialize<RuleFileDto>(json, Options);

            if (dto is null || dto.Rules is null || dto.Rules.Count == 0)
                throw new InvalidDataException("Rule file is empty or invalid.");

            var result = new List<AlertRule>();
            foreach (var ruleDto in dto.Rules)
            {
                // En dıştaki kural id ve message taşımalı.
                if (ruleDto.Id is null)
                    throw new InvalidDataException("Top-level rule requires an 'id'.");
                if (ruleDto.Message is null)
                    throw new InvalidDataException("Top-level rule requires a 'message'.");

                var condition = RuleFactory.Build(ruleDto);   // fabrika iç kuralları da kurar
                result.Add(new AlertRule(ruleDto.Id, ruleDto.Message, condition));
            }

            return result;
        }
    }
}

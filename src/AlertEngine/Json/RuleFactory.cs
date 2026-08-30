using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine.Json
{
    internal static class RuleFactory
    {
        public static IRule Build(RuleDto dto)
        {
            if (dto.Type is null)
                throw new InvalidDataException("Rule is missing 'type' field.");

            return dto.Type switch
            {
                "threshold" => BuildThreshold(dto),
                "range" => BuildRange(dto),
                "change" => BuildChange(dto),
                "and" => BuildAnd(dto),
                "or" => BuildOr(dto),
                "not" => BuildNot(dto),
                "streak" => BuildStreak(dto),
                "cooldown" => BuildCooldown(dto),
                _ => throw new InvalidDataException($"Unknown rule type: {dto.Type}")
            };
        }

        private static IRule BuildThreshold(RuleDto dto)
        {
            if (dto.Operator is null)
                throw new InvalidDataException("threshold rule requires 'operator'.");
            if (dto.Value is null)
                throw new InvalidDataException("threshold rule requires 'value'.");

            var op = ParseOperator(dto.Operator);
            return new ThresholdRule(op, dto.Value.Value);
        }
        private static IRule BuildRange(RuleDto dto)
        {
            if (dto.Min is null)
                throw new InvalidDataException("range rule requires 'min'.");
            if (dto.Max is null)
                throw new InvalidDataException("range rule requires 'max'.");

            return new RangeRule(dto.Min.Value, dto.Max.Value);
        }

        private static IRule BuildChange(RuleDto dto)
        {
            if (dto.Percent is null)
                throw new InvalidDataException("change rule requires 'percent'.");

            return new ChangeRule(dto.Percent.Value);
        }

        private static IRule BuildAnd(RuleDto dto)
        {
            var innerRules = BuildInnerList(dto, "and");
            return new AndRule(innerRules);
        }

        private static IRule BuildOr(RuleDto dto)
        {
            var innerRules = BuildInnerList(dto, "or");
            return new OrRule(innerRules);
        }

        private static IRule BuildNot(RuleDto dto)
        {
            if (dto.Rule is null)
                throw new InvalidDataException("not rule requires a single 'rule'.");

            var innerRule = Build(dto.Rule);
            return new NotRule(innerRule);
        }

        private static IRule BuildStreak(RuleDto dto)
        {
            if (dto.Direction is null)
                throw new InvalidDataException("streak rule requires 'direction'.");
            if (dto.Hours is null)
                throw new InvalidDataException("streak rule requires 'hours'.");

            var direction = ParseDirection(dto.Direction);
            return new StreakRule(direction, dto.Hours.Value);
        }

        private static IRule BuildCooldown(RuleDto dto)
        {

            if (dto.Hours is null)
                throw new InvalidDataException("cooldown rule requires 'hours'.");
            if (dto.Rule is null)
                throw new InvalidDataException("cooldown rule requires a single 'rule'.");

            var innerRule = Build(dto.Rule);  
            return new CooldownRule(innerRule, dto.Hours.Value);
        }

        //helper

        private static ComparisonOperator ParseOperator(string op)
        {
            return op switch
            {
                "gt" => ComparisonOperator.GreaterThan,
                "lt" => ComparisonOperator.LessThan,
                _ => throw new InvalidDataException($"Unknown operator: '{op}'")
            };
        }

        private static IReadOnlyList<IRule> BuildInnerList(RuleDto dto, string typeName)
        {
            if (dto.Rules is null || dto.Rules.Count == 0)
                throw new InvalidDataException($"{typeName} rule requires a non-empty 'rules' list.");

            var result = new List<IRule>();
            foreach (var childDto in dto.Rules)
                result.Add(Build(childDto));

            return result;
        }

        private static StreakDirection ParseDirection(string direction)
        {
            return direction switch
            {
                "up" => StreakDirection.Up,
                "down" => StreakDirection.Down,
                _ => throw new InvalidDataException($"Unknown streak direction: '{direction}'")
            };
        }
    }
}

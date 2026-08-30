using AlertEngine.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine;

public sealed class Engine
{
    private readonly IReadOnlyList<AlertRule> _rules;

    public Engine(IReadOnlyList<AlertRule> rules)
    {
        _rules = rules;
    }

    public IReadOnlyList<AlertMatch> Run(IReadOnlyList<PricePoint> prices)
    {
        var matches = new List<AlertMatch>();

        for (int i = 0; i < prices.Count; i++)
        {
            var history = prices.Take(i + 1).ToList();
            var context = new EvaluationContext(history);

            foreach (var rule in _rules)
            {
                if (rule.Condition.Evaluate(context))
                {
                    matches.Add(new AlertMatch(
                        context.Current.Timestamp,
                        rule.Id,
                        rule.Message,
                        context.Current.Price));
                }
            }
        }

        return matches;
    }
}

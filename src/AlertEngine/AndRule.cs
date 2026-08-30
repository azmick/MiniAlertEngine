using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class AndRule : IRule
    {
        private readonly IReadOnlyList<IRule> _rules;

        public AndRule(IReadOnlyList<IRule> rules)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }
        public bool Evaluate(EvaluationContext context)
        {
            foreach (var rule in _rules)
            {
                if (!rule.Evaluate(context))
                    return false;
            }
            return true;
        }
    }
}

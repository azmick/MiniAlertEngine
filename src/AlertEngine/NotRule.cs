using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class NotRule : IRule
    {
        private readonly IRule _rule;
        public NotRule(IRule rule)
        {
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }
        public bool Evaluate(EvaluationContext context)
        {
            return !_rule.Evaluate(context);
        }
    }
}

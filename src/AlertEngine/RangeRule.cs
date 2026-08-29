using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class RangeRule : IRule
    {
        private readonly decimal _min;
        private readonly decimal _max;

        public RangeRule(decimal min, decimal max)
        {
            _min = min;
            _max = max;
        }

        public bool Evaluate(EvaluationContext context)
        {
            decimal price = context.Current.Price;

            return price < _min || price > _max;
        }
    }
}

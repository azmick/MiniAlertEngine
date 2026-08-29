using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class ThresholdRule : IRule
    {
        private readonly ComparisonOperator _operator;
        private readonly decimal _value;

        public ThresholdRule(ComparisonOperator comparisonOperator, decimal value)
        {
            _operator = comparisonOperator;
            _value = value;
        }


        public bool Evaluate(EvaluationContext context)
        {
            decimal price = context.Current.Price;

            return _operator switch
            {
                ComparisonOperator.GreaterThan => price > _value,
                ComparisonOperator.LessThan => price < _value,
                _ => throw new InvalidOperationException($"Unsupported comparison operator: {_operator}")
            };
        }
    }
}

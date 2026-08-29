using System;
using System.Collections.Generic;
using System.Text;
using AlertEngine;

namespace AlertEngine.Tests
{
    public class ThresholdRuleTests
    {
        private static EvaluationContext ContextWithPrice(decimal price)
        {
            var point = new PricePoint(DateTimeOffset.Parse("2026-08-15T18:00:00+03:00"), price);
            return new EvaluationContext(new[] { point });
        }

        [Fact]
        public void GreaterThan_Matches_When_Price_Is_Above_Value()
        {
            var rule = new ThresholdRule(ComparisonOperator.GreaterThan, 3000m);
            var context = ContextWithPrice(4200m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void GreaterThan_Does_Not_Match_When_Price_Is_Below_Value()
        {
            var rule = new ThresholdRule(ComparisonOperator.GreaterThan, 3000m);
            var context = ContextWithPrice(2347.60m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void GreaterThan_Does_Not_Match_When_Price_Equals_Value()
        {
            var rule = new ThresholdRule(ComparisonOperator.GreaterThan, 3000m);
            var context = ContextWithPrice(3000m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void LessThan_Matches_When_Price_Is_Below_Value()
        {
            var rule = new ThresholdRule(ComparisonOperator.LessThan, 100m);
            var context = ContextWithPrice(-50m);

            Assert.True(rule.Evaluate(context));
        }
    }
}

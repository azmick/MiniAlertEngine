using System;
using System.Collections.Generic;
using System.Text;
using AlertEngine;

namespace AlertEngine.Tests
{
    public class RangeRuleTests
    {
        private static EvaluationContext ContextWithPrice(decimal price)
        {
            var point = new PricePoint(DateTimeOffset.Parse("2026-08-15T18:00:00+03:00"), price);
            return new EvaluationContext(new[] { point });
        }

        [Fact]
        public void Matches_When_Price_Is_Above_Max()
        {
            var rule = new RangeRule(0m, 3500m);
            var context = ContextWithPrice(4200m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Matches_When_Price_Is_Below_Min()
        {
            var rule = new RangeRule(0m, 3500m);
            var context = ContextWithPrice(-50m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Does_Not_Match_When_Price_Is_Inside_Band()
        {
            var rule = new RangeRule(0m, 3500m);
            var context = ContextWithPrice(2000m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Does_Not_Match_When_Price_Equals_Max()
        {
            var rule = new RangeRule(0m, 3500m);
            var context = ContextWithPrice(3500m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Does_Not_Match_When_Price_Equals_Min()
        {
            var rule = new RangeRule(0m, 3500m);
            var context = ContextWithPrice(0m);

            Assert.False(rule.Evaluate(context));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine.Tests
{
    public class ChangeRuleTests
    {
        private static EvaluationContext ContextFromHourlyPrices(params decimal[] prices)
        {
            var start = DateTimeOffset.Parse("2026-08-15T00:00:00+03:00");
            var points = new List<PricePoint>();
            for (int i = 0; i < prices.Length; i++)
                points.Add(new PricePoint(start.AddHours(i), prices[i]));
            return new EvaluationContext(points);
        }

        [Fact]
        public void Matches_When_Jump_Exceeds_Threshold()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(2000m, 2500m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Matches_On_Drop_As_Well_As_Rise()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(2500m, 2000m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Does_Not_Match_When_Move_Is_Small()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(2000m, 2100m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Matches_Exactly_At_Threshold()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(2000m, 2400m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Returns_False_On_First_Hour_When_No_Previous_Exists()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(2000m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Matches_With_Negative_Previous_Price()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(-50m, 2481.83m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Returns_False_When_Previous_Price_Is_Zero()
        {
            var rule = new ChangeRule(20m);
            var context = ContextFromHourlyPrices(0m, 10m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Returns_False_When_Previous_Point_Is_Not_One_Hour_Before()
        {
            var start = DateTimeOffset.Parse("2026-08-12T02:00:00+03:00");
            var points = new List<PricePoint>
        {
            new PricePoint(start, 1313.13m),
            new PricePoint(start.AddHours(2), 1371.45m)
        };
            var context = new EvaluationContext(points);
            var rule = new ChangeRule(20m);

            Assert.False(rule.Evaluate(context));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using AlertEngine;

namespace AlertEngine.Tests
{
    public class StreakRuleTests
    {
        private static EvaluationContext ContextFromHourlyPrices(params decimal[] prices)
        {
            var start = DateTimeOffset.Parse("2026-08-10T00:00:00+03:00");
            var points = new List<PricePoint>();
            for (int i = 0; i < prices.Length; i++)
                points.Add(new PricePoint(start.AddHours(i), prices[i]));
            return new EvaluationContext(points);
        }

        [Fact]
        public void Up_Matches_On_Three_Consecutive_Rises()
        {
            var rule = new StreakRule(StreakDirection.Up, 3);
            var context = ContextFromHourlyPrices(1408.83m, 1474.65m, 1682.10m, 1759.13m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Up_Does_Not_Match_When_One_Move_Is_Down()
        {
            var rule = new StreakRule(StreakDirection.Up, 3);
            var context = ContextFromHourlyPrices(1400m, 1500m, 1600m, 1550m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Up_Does_Not_Match_When_A_Move_Is_Flat()
        {
            var rule = new StreakRule(StreakDirection.Up, 3);
            var context = ContextFromHourlyPrices(1400m, 1500m, 1500m, 1600m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Down_Matches_On_Three_Consecutive_Falls()
        {
            var rule = new StreakRule(StreakDirection.Down, 3);
            var context = ContextFromHourlyPrices(2000m, 1900m, 1800m, 1700m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Returns_False_When_Not_Enough_Points()
        {
            var rule = new StreakRule(StreakDirection.Up, 3);
            var context = ContextFromHourlyPrices(1400m, 1500m, 1600m);

            Assert.False(rule.Evaluate(context));
        }

        [Fact]
        public void Only_Considers_The_Most_Recent_Window()
        {
            var rule = new StreakRule(StreakDirection.Up, 3);
            var context = ContextFromHourlyPrices(2000m, 1000m, 1400m, 1500m, 1600m);

            Assert.True(rule.Evaluate(context));
        }

        [Fact]
        public void Returns_False_When_There_Is_A_Time_Gap_In_The_Window()
        {
            var start = DateTimeOffset.Parse("2026-08-12T02:00:00+03:00");
            var points = new List<PricePoint>
        {
            new PricePoint(start,               1313.13m),  
            new PricePoint(start.AddHours(2),   1371.45m),  
            new PricePoint(start.AddHours(3),   1569.47m),  
            new PricePoint(start.AddHours(4),   1630.53m)   
        };
            var context = new EvaluationContext(points);
            var rule = new StreakRule(StreakDirection.Up, 3);

            Assert.False(rule.Evaluate(context));
        }
    }
}

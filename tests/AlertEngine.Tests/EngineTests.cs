using AlertEngine;
using AlertEngine.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine.Tests;

public class EngineTests
{
    private static IReadOnlyList<PricePoint> HourlyPrices(DateTimeOffset start, params decimal[] prices)
    {
        var points = new List<PricePoint>();
        for (int i = 0; i < prices.Length; i++)
            points.Add(new PricePoint(start.AddHours(i), prices[i]));
        return points;
    }

    private static AlertRule ThresholdAlert(string id, string message, ComparisonOperator op, decimal value)
    {
        return new AlertRule(id, message, new ThresholdRule(op, value));
    }

    [Fact]
    public void Fires_When_A_Rule_Matches_At_A_Given_Hour()
    {
        var prices = HourlyPrices(
            DateTimeOffset.Parse("2026-08-15T17:00:00+03:00"),
            2347.60m, 4200.00m);

        var rules = new[]
        {
            ThresholdAlert("price-above-3000", "Price exceeded 3000 TRY/MWh.",
                ComparisonOperator.GreaterThan, 3000m)
        };

        var engine = new Engine(rules);
        var matches = engine.Run(prices);

        Assert.Single(matches);
        Assert.Equal("price-above-3000", matches[0].RuleId);
        Assert.Equal(4200.00m, matches[0].Price);
        Assert.Equal(DateTimeOffset.Parse("2026-08-15T18:00:00+03:00"), matches[0].Timestamp);
    }

    [Fact]
    public void Does_Not_Fire_When_No_Rule_Matches()
    {
        var prices = HourlyPrices(
            DateTimeOffset.Parse("2026-08-15T17:00:00+03:00"),
            2347.60m, 2400.00m);  

        var rules = new[]
        {
            ThresholdAlert("price-above-3000", "high",
                ComparisonOperator.GreaterThan, 3000m)
        };

        var matches = new Engine(rules).Run(prices);

        Assert.Empty(matches);
    }

    [Fact]
    public void Evaluates_Every_Rule_At_Every_Hour()
    {
        var prices = HourlyPrices(
            DateTimeOffset.Parse("2026-08-15T00:00:00+03:00"),
            50m, 2000m, 4200m);

        var rules = new[]
        {
            ThresholdAlert("above-3000", "high", ComparisonOperator.GreaterThan, 3000m),
            ThresholdAlert("below-100", "low", ComparisonOperator.LessThan, 100m)
        };

        var matches = new Engine(rules).Run(prices);

        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, m => m.RuleId == "below-100" && m.Price == 50m);
        Assert.Contains(matches, m => m.RuleId == "above-3000" && m.Price == 4200m);
    }

    [Fact]
    public void Preserves_Chronological_Order_Of_Matches()
    {
        var prices = HourlyPrices(
            DateTimeOffset.Parse("2026-08-15T00:00:00+03:00"),
            4200m, 2000m, 5000m); 

        var rules = new[]
        {
            ThresholdAlert("above-3000", "high", ComparisonOperator.GreaterThan, 3000m)
        };

        var matches = new Engine(rules).Run(prices);

        Assert.Equal(2, matches.Count);
        Assert.Equal(DateTimeOffset.Parse("2026-08-15T00:00:00+03:00"), matches[0].Timestamp);
        Assert.Equal(DateTimeOffset.Parse("2026-08-15T02:00:00+03:00"), matches[1].Timestamp);
    }
}

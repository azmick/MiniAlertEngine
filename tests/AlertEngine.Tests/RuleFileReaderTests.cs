using AlertEngine;
using AlertEngine.Json;

namespace AlertEngine.Tests;

public class RuleFileReaderTests
{
    private static EvaluationContext ContextWithPrice(decimal price)
    {
        var point = new PricePoint(DateTimeOffset.Parse("2026-08-15T18:00:00+03:00"), price);
        return new EvaluationContext(new[] { point });
    }

    [Fact]
    public void Parses_A_Simple_Threshold_Rule()
    {
        string json = """
        { "rules": [
            { "id": "price-above-3000", "type": "threshold",
              "operator": "gt", "value": 3000, "message": "high" }
        ]}
        """;

        var rules = RuleFileReader.Read(json);

        Assert.Single(rules);
        Assert.Equal("price-above-3000", rules[0].Id);
        Assert.Equal("high", rules[0].Message);
        Assert.True(rules[0].Condition.Evaluate(ContextWithPrice(4200m)));
        Assert.False(rules[0].Condition.Evaluate(ContextWithPrice(2000m)));
    }

    [Fact]
    public void Parses_A_Nested_And_Rule()
    {
        string json = """
        { "rules": [
            { "id": "expensive-and-volatile", "type": "and", "message": "x",
              "rules": [
                { "type": "threshold", "operator": "gt", "value": 2500 },
                { "type": "change", "percent": 10 }
              ]
            }
        ]}
        """;

        var rules = RuleFileReader.Read(json);

        Assert.Single(rules);
        Assert.Equal("expensive-and-volatile", rules[0].Id);
        Assert.IsType<AndRule>(rules[0].Condition);
    }

    [Fact]
    public void Parses_A_Not_Rule_With_Inner_Range()
    {
        string json = """
        { "rules": [
            { "id": "outside-comfort-zone", "type": "not", "message": "x",
              "rule": { "type": "range", "min": 1200, "max": 3200 }
            }
        ]}
        """;

        var rules = RuleFileReader.Read(json);

        Assert.IsType<NotRule>(rules[0].Condition);
        Assert.True(rules[0].Condition.Evaluate(ContextWithPrice(2000m)));
        Assert.False(rules[0].Condition.Evaluate(ContextWithPrice(5000m)));
    }

    [Fact]
    public void Parses_A_Cooldown_Wrapping_A_Threshold()
    {
        string json = """
        { "rules": [
            { "id": "spike-alarm-with-cooldown", "type": "cooldown", "hours": 6,
              "message": "x",
              "rule": { "type": "threshold", "operator": "gt", "value": 2800 }
            }
        ]}
        """;

        var rules = RuleFileReader.Read(json);

        Assert.IsType<CooldownRule>(rules[0].Condition);
    }

    [Fact]
    public void Throws_On_Unknown_Rule_Type()
    {
        string json = """
        { "rules": [
            { "id": "x", "type": "banana", "message": "y" }
        ]}
        """;

        Assert.Throws<InvalidDataException>(() => RuleFileReader.Read(json));
    }

    [Fact]
    public void Throws_When_Top_Level_Rule_Has_No_Id()
    {
        string json = """
        { "rules": [
            { "type": "threshold", "operator": "gt", "value": 3000, "message": "x" }
        ]}
        """;

        Assert.Throws<InvalidDataException>(() => RuleFileReader.Read(json));
    }
}
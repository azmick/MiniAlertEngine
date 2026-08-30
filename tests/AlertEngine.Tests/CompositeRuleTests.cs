using System;
using System.Collections.Generic;
using System.Text;
using AlertEngine;

namespace AlertEngine.Tests;

public class CompositeRuleTests
{
    private sealed class StubRule : IRule
    {
        private readonly bool _result;
        public StubRule(bool result) => _result = result;
        public bool Evaluate(EvaluationContext context) => _result;
    }

    private static readonly IRule True = new StubRule(true);
    private static readonly IRule False = new StubRule(false);

    private static EvaluationContext AnyContext()
    {
        var point = new PricePoint(DateTimeOffset.Parse("2026-08-15T18:00:00+03:00"), 2000m);
        return new EvaluationContext(new[] { point });
    }


    [Fact]
    public void And_Matches_When_All_Children_Match()
    {
        var rule = new AndRule(new[] { True, True, True });
        Assert.True(rule.Evaluate(AnyContext()));
    }

    [Fact]
    public void And_Does_Not_Match_When_One_Child_Fails()
    {
        var rule = new AndRule(new[] { True, False, True });
        Assert.False(rule.Evaluate(AnyContext()));
    }


    [Fact]
    public void Or_Matches_When_At_Least_One_Child_Matches()
    {
        var rule = new OrRule(new[] { False, False, True });
        Assert.True(rule.Evaluate(AnyContext()));
    }

    [Fact]
    public void Or_Does_Not_Match_When_All_Children_Fail()
    {
        var rule = new OrRule(new[] { False, False, False });
        Assert.False(rule.Evaluate(AnyContext()));
    }


    [Fact]
    public void Not_Inverts_A_Matching_Child()
    {
        var rule = new NotRule(True);
        Assert.False(rule.Evaluate(AnyContext()));
    }

    [Fact]
    public void Not_Inverts_A_Failing_Child()
    {
        var rule = new NotRule(False);
        Assert.True(rule.Evaluate(AnyContext()));
    }

    [Fact]
    public void Supports_Arbitrary_Nesting()
    {
        var nested = new AndRule(new IRule[]
        {
            new OrRule(new[] { False, True }),
            new NotRule(False)
        });

        Assert.True(nested.Evaluate(AnyContext()));
    }

    [Fact]
    public void Nesting_Propagates_Failure_Correctly()
    {
        var nested = new AndRule(new IRule[]
        {
            new OrRule(new[] { False, False }),
            new NotRule(False)
        });

        Assert.False(nested.Evaluate(AnyContext()));
    }
}
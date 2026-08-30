using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine.Tests
{
    public class CooldownRuleTests
    {
        private sealed class AlwaysTrue : IRule
        {
            public bool Evaluate(EvaluationContext context) => true;
        }
        private static EvaluationContext ContextAt(DateTimeOffset timestamp)
        {
            return new EvaluationContext(new[] { new PricePoint(timestamp, 9999m) });
        }

        [Fact]
        public void Fires_On_First_Match()
        {
            var rule = new CooldownRule(new AlwaysTrue(), hours: 6);
            var t0 = DateTimeOffset.Parse("2026-08-15T10:00:00+03:00");

            Assert.True(rule.Evaluate(ContextAt(t0)));
        }

        [Fact]
        public void Suppresses_Within_Cooldown_Window()
        {
            var rule = new CooldownRule(new AlwaysTrue(), hours: 6);
            var start = DateTimeOffset.Parse("2026-08-15T10:00:00+03:00");

            Assert.True(rule.Evaluate(ContextAt(start)));              
            Assert.False(rule.Evaluate(ContextAt(start.AddHours(1)))); 
            Assert.False(rule.Evaluate(ContextAt(start.AddHours(2))));
            Assert.False(rule.Evaluate(ContextAt(start.AddHours(5))));
        }

        [Fact]
        public void Fires_Again_After_Cooldown_Elapses()
        {
            var rule = new CooldownRule(new AlwaysTrue(), hours: 6);
            var start = DateTimeOffset.Parse("2026-08-15T10:00:00+03:00");

            Assert.True(rule.Evaluate(ContextAt(start))); 
            Assert.False(rule.Evaluate(ContextAt(start.AddHours(3)))); 
            Assert.True(rule.Evaluate(ContextAt(start.AddHours(6)))); 
        }

        [Fact]
        public void Resets_Window_From_Last_Fire_Not_Last_Match()
        {
            var rule = new CooldownRule(new AlwaysTrue(), hours: 6);
            var start = DateTimeOffset.Parse("2026-08-15T10:00:00+03:00");

            Assert.True(rule.Evaluate(ContextAt(start)));               
            Assert.True(rule.Evaluate(ContextAt(start.AddHours(6))));   
            Assert.False(rule.Evaluate(ContextAt(start.AddHours(7))));  
            Assert.True(rule.Evaluate(ContextAt(start.AddHours(12))));  
        }

        [Fact]
        public void Does_Not_Fire_When_Inner_Rule_Does_Not_Match()
        {
            var alwaysFalse = new StubFalse();
            var rule = new CooldownRule(alwaysFalse, hours: 6);
            var t0 = DateTimeOffset.Parse("2026-08-15T10:00:00+03:00");

            Assert.False(rule.Evaluate(ContextAt(t0)));
        }

        private sealed class StubFalse : IRule
        {
            public bool Evaluate(EvaluationContext context) => false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class CooldownRule : IRule
    {
        private readonly IRule _inner;
        private readonly int _hours;

        private DateTimeOffset? _lastFired = null;

        public CooldownRule(IRule inner, int hours)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _hours = hours;
        }
        public bool Evaluate(EvaluationContext context)
        {
            if(!_inner.Evaluate(context))
            {
                return false;
            }
            
            var now = context.Current.Timestamp;

            if (_lastFired == null)
            {
                _lastFired = now;
                return true;
            }

            var elapsed = now - _lastFired.Value;
            if(elapsed >= TimeSpan.FromHours(_hours))
            {
                _lastFired = now;
                return true;
            }
            return false;
        }
    }
}

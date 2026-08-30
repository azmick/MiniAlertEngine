using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class StreakRule : IRule
    {
        private readonly StreakDirection _direction;
        private readonly int _hours;


        public StreakRule(StreakDirection direction, int hours)
        {
            _direction = direction;
            _hours = hours;
        }

        public bool Evaluate(EvaluationContext context)
        {
            var history = context.History;
            int pointsNeeded = _hours + 1;

            if (history.Count < pointsNeeded)
            {
                return false;
            }

            for (int i = history.Count - _hours; i < history.Count; i++)
            {
                var previos = history[i - 1];
                var current = history[i];

                if (current.Timestamp - previos.Timestamp != TimeSpan.FromHours(1))
                    return false;

                bool moveMatches = _direction switch
                {
                    StreakDirection.Up => current.Price > previos.Price,
                    StreakDirection.Down => current.Price < previos.Price,
                    _ => throw new InvalidOperationException($"Unknown streak direction: {_direction}")
                };

                if (!moveMatches)
                    return false;
            }

            return true;

        }
    }
}

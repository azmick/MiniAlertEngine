using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class EvaluationContext
    {
        public PricePoint Current { get; }

        public IReadOnlyList<PricePoint> History { get; }

        public EvaluationContext(IReadOnlyList<PricePoint> history)
        {
            if(history == null || history.Count == 0)
            {
                throw new ArgumentException("History cannot be null or empty.", nameof(history));
            }
            History = history ?? throw new ArgumentNullException(nameof(history));
            Current = history[^1]; // the last element in the history 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed class ChangeRule : IRule
    {
        private readonly decimal _percent;

        public ChangeRule(decimal percent)
        {
            _percent = percent;
        }
        public bool Evaluate(EvaluationContext context)
        {
            var history = context.History;

            if (history.Count < 2)
                return false;
           
            var current = history[^1];
            var previous = history[^2];

            //gerçekten 1 saatlik değişim mi yoksa 1 saatten fazla mı geçmiş onu kontrol et
            if (current.Timestamp - previous.Timestamp != TimeSpan.FromHours(1))
                return false;

            decimal oldPrice = previous.Price;
            decimal newPrice = current.Price;

            if (oldPrice == 0m)
                return false;

            decimal percentChange = Math.Abs((newPrice - oldPrice) / oldPrice * 100m);

            return percentChange >= _percent;
        }
    }
}

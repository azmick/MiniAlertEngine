using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public interface IRule
    {
        bool Evaluate(EvaluationContext context);
    }
}

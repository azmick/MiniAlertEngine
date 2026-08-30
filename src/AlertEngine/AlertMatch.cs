using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public sealed record AlertMatch(
        DateTimeOffset Timestamp,
        string RuleId,
        string Message,
        decimal Price);
}

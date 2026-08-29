using System;
using System.Collections.Generic;
using System.Text;

namespace AlertEngine
{
    public record PricePoint(DateTimeOffset Timestamp, decimal Price);
    
}

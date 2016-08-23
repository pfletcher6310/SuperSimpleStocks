using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Model
{
    public interface IStock
    {
        String Symbol { get; set; }
        StockType StockType { get; set; }
        Decimal MarketPrice { get; set; }
        Decimal LastDividendPrice { get; }
        Decimal? FixedDividend { get; set; }
        Decimal ParValue { get; set; }
    }
}

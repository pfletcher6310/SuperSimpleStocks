using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Model
{
    public interface ITrade
    {
        DateTime TimeStamp { get; set; }
        TradeIndicator Indicator { get; set; }
        IStock StockInformation { get; set; }
        Decimal Price { get; set; }
        Int32 Quantity { get; set; }

        Boolean Validate();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Model
{
    public class Stock : IStock
    { 
        public override string ToString()
        {
            return String.Format("{0} - Type: {1} | Market Price: {2:0.00}", this.Symbol, this.StockType, this.MarketPrice);
        }

        public Decimal LastDividendPrice
        {
            get; set;
        }

        public Decimal MarketPrice
        {
            get; set;
        }

        public Decimal ParValue
        {
            get; set;
        }

        public StockType StockType
        {
            get; set;
        }

        public string Symbol
        {
            get; set;
        }

        public Decimal? FixedDividend { get; set; }        
    }
}

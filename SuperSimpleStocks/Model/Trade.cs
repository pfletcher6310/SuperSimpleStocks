using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Model
{
    public class Trade : ITrade
    {
        public override string ToString()
        {
            return String.Format("{0} - Stock: {1} | Price: {2:0.00} | Quantity: {3} | Timestamp: {4:yyyy-MM-dd HH:mm:ss}"
                , this.Indicator
                , this.StockInformation.Symbol
                , this.Price
                , this.Quantity
                , this.TimeStamp);
        }

        public DateTime TimeStamp { get; set; }
        public TradeIndicator Indicator { get; set; }
        public IStock StockInformation { get; set; }
        public Decimal Price { get; set; }
        public Int32 Quantity { get; set; }

        public Boolean Validate()
        {
            if (this.StockInformation == null)
            {
                throw new Exception("Stock Information on trade not a valid object");
            }

            if (String.IsNullOrWhiteSpace(this.StockInformation.Symbol))
            {
                throw new Exception("Symbol not a valid string on trade");
            }

            if (this.Price <= Decimal.Zero)
            {
                throw new Exception("Price not above zero on trade");
            }

            if (this.Quantity <= 0)
            {
                throw new Exception("Quantity not above zero on trade");
            }

            return true;
        }
    }
}
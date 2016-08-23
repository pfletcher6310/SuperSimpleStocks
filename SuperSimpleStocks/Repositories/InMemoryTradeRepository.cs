using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleStocks.Model;

namespace SuperSimpleStocks.Repositories
{
    /// <summary>
    /// An in memory implementation of the Trade repository, this would usually be some sort of persistent storage
    /// but for the purposes of our demo app this will be suitable.
    /// </summary>
    public class InMemoryTradeRepository : ITradeRepository
    {
        private List<ITrade> Trades { get; set; }

        /// <summary>
        /// Create a new instances of the <c>InMemoryTradeRepository</c> with an initial set of starting trades.
        /// </summary>
        /// <param name="startingTrades">The list of starting trades for our in memory repository.</param>
        public InMemoryTradeRepository(List<ITrade> startingTrades)
        {
            if (startingTrades == null)
            {
                startingTrades = new List<ITrade>();
            }

            this.Trades = startingTrades;
        }

        public IEnumerable<ITrade> GetRecentTradesBySymbol(String stockSymbol, Int32 rangeInMinutes)
        {
            if(stockSymbol == null)
            {
                throw new ArgumentNullException("stockSymbol", "You must provide a valid stockSymbol string");
            }

            // Calculate when our range starts in terms of an actual date time.
            DateTime timeStampInThePast = DateTime.UtcNow.AddMinutes(rangeInMinutes * -1);

            return this.Trades.Where(trade => trade.StockInformation.Symbol == stockSymbol && trade.TimeStamp >= timeStampInThePast);
        }

        public void Save(ITrade trade)
        {
            this.Trades.Add(trade);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleStocks.Model;
using Microsoft.Extensions.Logging;

namespace SuperSimpleStocks.Repositories
{
    /// <summary>
    /// An in memory implementation of the Stock repository, this would usually be some sort of persistent storage
    /// but for the purposes of our demo app this will be suitable.
    /// </summary>
    public class InMemoryStockRepository : IStockRepository
    {
        ILogger Logger { get; } = Logging.CreateLogger<InMemoryStockRepository>();

        private List<IStock> Stocks { get; set; }

        /// <summary>
        /// Create a new instances of the <c>InMemoryStockRepository</c> with an initial set of starting stocks.
        /// </summary>
        /// <param name="startingStocks">The list of starting stocks for our in memory repository.</param>
        public InMemoryStockRepository(List<IStock> startingStocks)
        {
            if (startingStocks == null)
            {
                startingStocks = new List<IStock>();
            }

            this.Stocks = startingStocks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStock> GetAllStocks()
        {
            return this.Stocks;
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSimpleStocks.BusinessLogicServices;
using SuperSimpleStocks.Model;
using SuperSimpleStocks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Tests
{
    /// <summary>
    /// This class demonstrates the ability to meet the basic requirements of the Super Simple Stocks application.
    /// Each component has more in depth testing in its specific test class.
    /// </summary>
    [TestClass]
    public class DemoTests
    {
        public List<IStock> DemoData = new List<IStock>() {
                    new Stock() { Symbol = "TEA", StockType = StockType.Common, LastDividendPrice = 0, FixedDividend = null, ParValue = 100 }
                    , new Stock() { Symbol = "POP", StockType = StockType.Common, LastDividendPrice = 8, FixedDividend = null, ParValue = 100 }
                    , new Stock() { Symbol = "ALE", StockType = StockType.Common, LastDividendPrice = 23, FixedDividend = null, ParValue = 60 }
                    , new Stock() { Symbol = "GIN", StockType = StockType.Preferred, LastDividendPrice = 8, FixedDividend = 0.02M, ParValue = 100 }
                    , new Stock() { Symbol = "JOE", StockType = StockType.Common, LastDividendPrice = 13, FixedDividend = null, ParValue = 250 }
                };

        [TestMethod]
        public void RunThroughRequirements()
        {
            // Setup the sample data as per word document
            List<IStock> sampleData = DemoData;

            // Setup our stock repo with the sample data as standard and the trade with no data.
            IStockRepository stockRepo = new InMemoryStockRepository(sampleData);
            ITradeRepository tradeRepo = new InMemoryTradeRepository(null);

            ITradeService tradeService = new TradeService(tradeRepo);
            IStockManagementService stockManagementService = new StockManagementService(stockRepo, tradeService);

            // 1a i
            // Set the price, calculate the dividend yield and assert for COMMON
            stockManagementService.UpdateMarketPrice("POP", 100M);
            Decimal pop_dividendYield = stockManagementService.GetDividendYield("POP");
            Assert.AreEqual(0.08M, pop_dividendYield);

            // Set the price, calculate the dividend yield and assert for PREFERRED
            stockManagementService.UpdateMarketPrice("GIN", 102M);
            Decimal gin_dividendYield = stockManagementService.GetDividendYield("GIN");
            Assert.AreEqual(0.02M, gin_dividendYield);

            // 1a ii
            // Set the price, calculate the P/E ratio and assert
            stockManagementService.UpdateMarketPrice("ALE", 175M);
            Decimal ale_PERatio = stockManagementService.GetPERatio("ALE");
            Assert.AreEqual(7.6M, ale_PERatio);

            // 1a iii
            // Record trades at multiple prices
            stockManagementService.BuyAtCurrentPrice("POP", 5);
            stockManagementService.BuyAtCurrentPrice("ALE", 6);
            stockManagementService.BuyAtCurrentPrice("GIN", 1);
            // Price Change
            stockManagementService.UpdateMarketPrice("POP", 101M);
            stockManagementService.UpdateMarketPrice("ALE", 101M);
            stockManagementService.UpdateMarketPrice("GIN", 87M);
            stockManagementService.UpdateMarketPrice("TEA", 104M);
            // More trades
            stockManagementService.BuyAtCurrentPrice("TEA", 1);
            stockManagementService.BuyAtCurrentPrice("POP", 5);
            stockManagementService.BuyAtCurrentPrice("ALE", 5);
            stockManagementService.BuyAtCurrentPrice("ALE", 2);
            stockManagementService.SellAtCurrentPrice("GIN", 1);

            // 1a iv
            // Assert a number of Volume Weighted Stock Prices now trades have been recorded.
            Decimal pop_vwsp = tradeService.GetVolumeWeightedStockPrice("POP", 15); // ((100*5)+(101*5))/10 = 100.5
            Assert.AreEqual(100.5M, pop_vwsp);

            Decimal ale_vwsp = tradeService.GetVolumeWeightedStockPrice("ALE", 15); // ((175*6)+(101*5)+(101*2))/13 = 135.15
            Assert.AreEqual(135.15M, ale_vwsp);

            Decimal gin_vwsp = tradeService.GetVolumeWeightedStockPrice("GIN", 15); // ((102*1)+(87*1))/2 = 94.5
            Assert.AreEqual(94.5M, gin_vwsp);

            // 1b
            // Calculate the current price of the GBCE All Share Index
            Decimal indexPrice = stockManagementService.GetIndexPrice();
            Assert.AreEqual(98.02M, indexPrice);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSimpleStocks.BusinessLogicServices;
using SuperSimpleStocks.Model;
using SuperSimpleStocks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Tests.BusinessLogicServices
{
    [TestClass]
    public class TradeServiceTests
    {
        [TestMethod]
        public void ValidConstructor_Test()
        {
            Moq.Mock<ITradeRepository> mockTradeRepo = new Moq.Mock<ITradeRepository>();
            ITradeService tradeService = new TradeService(mockTradeRepo.Object);
            Assert.IsNotNull(tradeService);
        }

        [TestMethod]
        public void InvalidConstructor_Test()
        {
            try
            {
                ITradeService tradeService = new TradeService(null);

                Assert.Fail("TradeService should fail to construct with null");
            }
            catch(ArgumentNullException ex)
            {
                Assert.AreEqual("tradeRepo", ex.ParamName);
            }
        }

        [TestMethod]
        public void Buy_Test()
        {
            // The action we want to run on the tradeService.
            Action<ITradeService, IStock, Decimal, Int32, DateTime> action =
                (tradeService, stock, expectedPrice, expectedQuantity, expectedTimestamp)
                    => tradeService.Buy(stock, expectedPrice, expectedQuantity, expectedTimestamp);

            Trade_Suite_Test(action, TradeIndicator.Buy);
        }

        [TestMethod]
        public void Sell_Test()
        {
            // The action we want to run on the tradeService.
            Action<ITradeService, IStock, Decimal, Int32, DateTime> action =
                (tradeService, stock, expectedPrice, expectedQuantity, expectedTimestamp)
                    => tradeService.Sell(stock, expectedPrice, expectedQuantity, expectedTimestamp);

            Trade_Suite_Test(action, TradeIndicator.Sell);
        }

        [TestMethod]
        public void GetVolumeWeightedStockPrice_Valid_Test()
        {
            // Sample valid data which would be returned by ITradeRepository
            List<ITrade> listOfTrades = new List<ITrade>();
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, Price = 55M, Quantity = 10, TimeStamp = DateTime.UtcNow });
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, Price = 55M, Quantity = 10, TimeStamp = DateTime.UtcNow.AddMinutes(-8) });
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, Price = 45M, Quantity = 20, TimeStamp = DateTime.UtcNow.AddMinutes(-10) });
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, Price = 55M, Quantity = 10, TimeStamp = DateTime.UtcNow.AddMinutes(-12) });

            Moq.Mock<ITradeRepository> mockTradeRepo = new Moq.Mock<ITradeRepository>();
            mockTradeRepo.Setup(mock => mock.GetRecentTradesBySymbol("ABC", 15)).Returns(() => listOfTrades);

            // Run the action.
            ITradeService tradeService = new TradeService(mockTradeRepo.Object);

            Decimal actualResult = tradeService.GetVolumeWeightedStockPrice("ABC", 15);

            Assert.AreEqual(51, actualResult);
        }

        
        // This is a generic method for testing a range of buy/sell trades.
        private void Trade_Suite_Test(Action<ITradeService, IStock, Decimal, Int32, DateTime> action, TradeIndicator indicator)
        {
            Trade_Test(action, indicator, 100M, 10); // NORMAL
            Trade_Test(action, indicator, Decimal.MaxValue, 10); // MAX PRICE
            Trade_Test(action, indicator, 0.01M, 10); // MIN PRICE
            Trade_Test(action, indicator, Decimal.MaxValue, Int32.MaxValue); // MAX QUANTITY
            Trade_Test(action, indicator, 0.01M, Int32.MaxValue); // MIN QUANTITY

            try
            {
                Trade_Test(action, indicator, -1M, 10);
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Failure recording trade", ex.Message);
                Assert.AreEqual("Price not above zero on trade", ex.InnerException.Message);
            }

            try
            {
                Trade_Test(action, indicator, 1M, 0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure recording trade", ex.Message);
                Assert.AreEqual("Quantity not above zero on trade", ex.InnerException.Message);
            }
        }

        // This is a generic method for testing buy/sell.
        private void Trade_Test(Action<ITradeService, IStock, Decimal, Int32, DateTime> action, TradeIndicator indicator, Decimal expectedPrice, Int32 expectedQuantity)
        {
            Moq.Mock<ITradeRepository> mockTradeRepo = new Moq.Mock<ITradeRepository>();
            Moq.Mock<IStock> mockStock = new Moq.Mock<IStock>();
            mockStock.Setup(mock => mock.Symbol).Returns("ABC");

            ITrade actualTrade = null;
            IStock expectedStock = mockStock.Object;
            DateTime expectedTimestamp = DateTime.UtcNow;

            // Setup our mock dependency to listen for what we expect to happen and capture the trade created
            // so we can verify it.
            mockTradeRepo
                .Setup(mock => mock.Save(Moq.It.IsAny<ITrade>()))
                .Callback<ITrade>(trade =>
                {
                    actualTrade = trade; // Copy to unit test scope.
                })
                .Verifiable();

            // Run the action.
            ITradeService tradeService = new TradeService(mockTradeRepo.Object);
            action(tradeService, expectedStock, expectedPrice, expectedQuantity, expectedTimestamp);

            // Verify
            mockTradeRepo.Verify(x => x.Save(Moq.It.IsAny<ITrade>()), Moq.Times.Once);
            Assert.IsNotNull(actualTrade);
            Assert.AreEqual(expectedPrice, actualTrade.Price);
            Assert.AreEqual(expectedQuantity, actualTrade.Quantity);
            Assert.AreEqual(expectedTimestamp, actualTrade.TimeStamp);
            Assert.AreEqual(indicator, actualTrade.Indicator);
            Assert.ReferenceEquals(expectedStock, actualTrade.StockInformation);
        }
    }
}

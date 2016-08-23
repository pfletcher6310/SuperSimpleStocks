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
    public class StockManagementServiceTests
    {
        private IStockManagementService stockManagementService;

        [TestInitialize]
        public void Setup()
        {
            Moq.Mock<IStockRepository> mockStockRepo = new Moq.Mock<IStockRepository>();
            Moq.Mock<ITradeRepository> mockTradeRepo = new Moq.Mock<ITradeRepository>();
            
            mockStockRepo.Setup(m => m.GetAllStocks()).Returns(new DemoTests().DemoData);
            mockTradeRepo.Setup(m => m.Save(Moq.It.IsAny<ITrade>())).Callback<ITrade>(VerifyTrade);

            ITradeService tradeService = new TradeService(mockTradeRepo.Object);

            // Set private field on test so all tests can access.
            this.stockManagementService = new StockManagementService(mockStockRepo.Object, tradeService);
        }

        [TestMethod]
        public void ValidConstructor_Test()
        {
            Moq.Mock<ITradeService> mockTradeService = new Moq.Mock<ITradeService>();
            Moq.Mock<IStockRepository> mockStockRepo = new Moq.Mock<IStockRepository>();
            IStockManagementService stockManagementService = new StockManagementService(mockStockRepo.Object, mockTradeService.Object);
            Assert.IsNotNull(stockManagementService);
        }

        [TestMethod]
        public void InvalidConstructor_Test()
        {
            // First parameter null
            try
            {
                IStockManagementService stockManagementService = new StockManagementService(null, null);

                Assert.Fail("StockManagementService should fail to construct with null");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("stockRepo", ex.ParamName);
            }

            // First parameter not null.
            try
            {
                Moq.Mock<IStockRepository> mockStockRepo = new Moq.Mock<IStockRepository>();
                IStockManagementService stockManagementService = new StockManagementService(mockStockRepo.Object, null);

                Assert.Fail("StockManagementService should fail to construct with null");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("tradeService", ex.ParamName);
            }
        }

        [TestMethod]
        public void DividendTest()
        {
            TestYield("TEA", 100M, 0M);
            TestYield("POP", 100M, 0.08M);
            TestYield("ALE", 100M, 0.23M);
            TestYield("GIN", 100M, 0.02M);
            TestYield("JOE", 100M, 0.13M);

            try
            {
                TestYield("TEA", 0M, 0M);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure updating market price", ex.Message);
            }
        }

        [TestMethod]
        public void PERatioTest()
        {
            TestPERatio("POP", 100M, 12.5M);
            TestPERatio("ALE", 100M, 4.3M);
            TestPERatio("GIN", 100M, 12.5M);
            TestPERatio("JOE", 100M, 7.7M);

            try
            {
                TestPERatio("TEA", 100M, 0M);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure calculating the P/E ratio", ex.Message);
                Assert.AreEqual("Cannot calculate a P/E ratio when the last dividend price was zero", ex.InnerException.Message);
            }
        }

        [TestMethod]
        public void Buy_Valid_Test()
        {
            stockManagementService.UpdateMarketPrice("TEA", 4M);
            // Mock callback on next line will invoke VerifyTrade below for Asserts
            stockManagementService.BuyAtCurrentPrice("TEA", 1);
        }

        [TestMethod]
        public void Sell_Valid_Test()
        {
            stockManagementService.UpdateMarketPrice("JOE", 9M);
            // Mock callback on next line will invoke VerifyTrade below for Asserts
            stockManagementService.SellAtCurrentPrice("JOE", 2);
        }

        private void VerifyTrade(ITrade trade)
        {
            if (trade.Indicator == TradeIndicator.Buy)
            {
                Assert.AreEqual("TEA", trade.StockInformation.Symbol);
                Assert.AreEqual(4M, trade.Price);
                Assert.AreEqual(1, trade.Quantity);
            }

            if(trade.Indicator == TradeIndicator.Sell)
            {
                Assert.AreEqual("JOE", trade.StockInformation.Symbol);
                Assert.AreEqual(9M, trade.Price);
                Assert.AreEqual(2, trade.Quantity);
            }
        }

        [TestMethod]
        public void Buy_Invalid_Test()
        {
            stockManagementService.UpdateMarketPrice("TEA", 4M);

            try
            {
                stockManagementService.BuyAtCurrentPrice("TEA", 0);
            }
            catch(Exception ex)
            {
                Assert.AreEqual("Failure buying at current market price", ex.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                stockManagementService.BuyAtCurrentPrice(null, 2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure buying at current market price", ex.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentNullException));
            }

            try
            {
                stockManagementService.BuyAtCurrentPrice("", 2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure buying at current market price", ex.Message);
            }
        }

        [TestMethod]
        public void Sell_Invalid_Test()
        {
            stockManagementService.UpdateMarketPrice("TEA", 4M);

            try
            {
                stockManagementService.SellAtCurrentPrice("TEA", 0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure selling at current market price", ex.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                stockManagementService.SellAtCurrentPrice(null, 2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure selling at current market price", ex.Message);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentNullException));
            }

            try
            {
                stockManagementService.SellAtCurrentPrice("", 2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Failure selling at current market price", ex.Message);
            }
        }

        private void TestYield(String symbol, Decimal marketPrice, Decimal expected)
        {
            stockManagementService.UpdateMarketPrice(symbol, marketPrice);
            Decimal yield = stockManagementService.GetDividendYield(symbol);

            Assert.AreEqual(expected, yield, String.Format("Failed to assert dividend yield for {0}", symbol));
        }

        private void TestPERatio(String symbol, Decimal marketPrice, Decimal expected)
        {
            stockManagementService.UpdateMarketPrice(symbol, marketPrice);
            Decimal yield = stockManagementService.GetPERatio(symbol);

            Assert.AreEqual(expected, yield, String.Format("Failed to assert P/E ratio for {0}", symbol));
        }
    }
}

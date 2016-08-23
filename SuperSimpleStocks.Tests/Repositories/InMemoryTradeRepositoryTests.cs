using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSimpleStocks.Model;
using SuperSimpleStocks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Tests.Repositories
{
    [TestClass]
    public class InMemoryTradeRepositoryTests
    {
        [TestMethod]
        public void NullConstructor_Test()
        {
            InMemoryTradeRepository repo = new InMemoryTradeRepository(null);
            IEnumerable<ITrade> result = repo.GetRecentTradesBySymbol("ABC", 15);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetRecentTradesBySymbol_NullParameter_Test()
        {
            InMemoryTradeRepository repo = new InMemoryTradeRepository(null); // Doesn't matter what we pass in for this test.

            try
            {
                // Pass in null to simulate error.
                IEnumerable<ITrade> result = repo.GetRecentTradesBySymbol(null, 15);

                Assert.Fail("There should be an exception on this test");
            }
            catch(ArgumentNullException ex)
            {
                Assert.AreEqual("stockSymbol", ex.ParamName);
            }
        }

        [TestMethod]
        public void GetRecentTradesBySymbol_Valid_WithResults_Test()
        {
            // Setup with two valid.
            List<ITrade> listOfTrades = new List<ITrade>();
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow }); // VALID
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // VALID
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "DEF" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // INVALID NAME
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-16) }); // INVALID TIME

            // Start
            InMemoryTradeRepository repo = new InMemoryTradeRepository(listOfTrades);

            IEnumerable<ITrade> result = repo.GetRecentTradesBySymbol("ABC", 15);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetRecentTradesBySymbol_Valid_WithNoResults_Test()
        {
            // Setup with none valid.
            List<ITrade> listOfTrades = new List<ITrade>();
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // INVALID TIME
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "DEF" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // INVALID NAME
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-16) }); // INVALID TIME

            // Start
            InMemoryTradeRepository repo = new InMemoryTradeRepository(listOfTrades);

            IEnumerable<ITrade> result = repo.GetRecentTradesBySymbol("ABC", 5);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Save_Test()
        {
            // Setup with none valid.
            List<ITrade> listOfTrades = new List<ITrade>();
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // INVALID TIME
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "DEF" }, TimeStamp = DateTime.UtcNow.AddMinutes(-10) }); // INVALID NAME
            listOfTrades.Add(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow.AddMinutes(-16) }); // INVALID TIME

            // Start
            InMemoryTradeRepository repo = new InMemoryTradeRepository(listOfTrades);

            // Step one - check no results from initial data
            IEnumerable<ITrade> resultPreSave = repo.GetRecentTradesBySymbol("ABC", 5);
            Assert.AreEqual(0, resultPreSave.Count());

            // Step two - save new trade then check again
            repo.Save(new Trade() { StockInformation = new Stock() { Symbol = "ABC" }, TimeStamp = DateTime.UtcNow }); // VALID
            IEnumerable<ITrade> resultPostSave = repo.GetRecentTradesBySymbol("ABC", 5);
            Assert.AreEqual(1, resultPostSave.Count());
        }
    }
}

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
    public class InMemoryStockRepositoryTests
    {
        [TestMethod]
        public void NullConstructor_Test()
        {
            InMemoryStockRepository repo = new InMemoryStockRepository(null);
            IEnumerable<IStock> result = repo.GetAllStocks();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void ValidConstructor_Test()
        {
            List<IStock> listOfStocks = new List<IStock>();
            listOfStocks.Add(new Stock());
            listOfStocks.Add(new Stock());
            listOfStocks.Add(new Stock());
            listOfStocks.Add(new Stock());

            InMemoryStockRepository repo = new InMemoryStockRepository(listOfStocks);
            IEnumerable<IStock> result = repo.GetAllStocks();

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
            Assert.ReferenceEquals(result, listOfStocks);
        }
    }
}

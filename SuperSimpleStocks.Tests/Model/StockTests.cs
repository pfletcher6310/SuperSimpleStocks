using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperSimpleStocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Tests.Model
{
    [TestClass]
    public class StockTests
    {
        [TestMethod]
        public void ValidConstructor_Test()
        {
            Stock model = new Stock();
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void ToString_Test()
        {
            Stock model = new Stock();
            model.Symbol = "ABC";
            model.StockType = StockType.Common;
            model.MarketPrice = 34.78M;
            Assert.AreEqual("ABC - Type: Common | Market Price: 34.78", model.ToString());

            Stock modelPreferred = new Stock();
            modelPreferred.Symbol = "ZYX";
            modelPreferred.StockType = StockType.Preferred;
            modelPreferred.MarketPrice = 7M;
            Assert.AreEqual("ZYX - Type: Preferred | Market Price: 7.00", modelPreferred.ToString());
        }
    }
}

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
    public class TradeTests
    {
        [TestMethod]
        public void ValidConstructor_Test()
        {
            Trade model = new Trade();
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void ToString_Test()
        {
            Trade model = new Trade();
            model.StockInformation = new Stock();
            model.StockInformation.Symbol = "ABC";
            model.Price = 34.89M;
            model.Quantity = 2;
            model.TimeStamp = DateTime.ParseExact("2016-08-23 18:23:43", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("Buy - Stock: ABC | Price: 34.89 | Quantity: 2 | Timestamp: 2016-08-23 18:23:43", model.ToString());

            Trade modelSell = new Trade();
            modelSell.StockInformation = new Stock();
            modelSell.StockInformation.Symbol = "ZYX";
            modelSell.Price = 5M;
            modelSell.Quantity = 33;
            modelSell.Indicator = TradeIndicator.Sell;
            modelSell.TimeStamp = DateTime.ParseExact("2016-08-23 18:24:43", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("Sell - Stock: ZYX | Price: 5.00 | Quantity: 33 | Timestamp: 2016-08-23 18:24:43", modelSell.ToString());
        }

        [TestMethod]
        public void Validate_Test()
        {
            Trade model = new Trade();
            model.StockInformation = new Stock();
            model.StockInformation.Symbol = "ABC";
            model.Price = 34.89M;
            model.Quantity = 2;
            model.TimeStamp = DateTime.ParseExact("2016-08-23 18:23:43", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            Assert.IsTrue(model.Validate(), "Validation unit test failed on assumed valid trade");
        }

        [TestMethod]
        public void Validate_Stock_Test()
        {
            Trade model = new Trade();

            // Nothing so invalid.
            Validate_Stock_FailureMessage_Test(model, "Stock Information on trade not a valid object");

            model.StockInformation = new Stock(); // Add stock but no symbol inside.

            // No symbol set on property of stock.
            Validate_Stock_FailureMessage_Test(model, "Symbol not a valid string on trade");

            model.StockInformation.Symbol = "ABC"; // Set the symbol.

            // Price is zero so therefore invalid.
            Validate_Stock_FailureMessage_Test(model, "Price not above zero on trade");

            model.Price = 34.89M; // Set the price

            // Quantity is zero therefore invalid.
            Validate_Stock_FailureMessage_Test(model, "Quantity not above zero on trade");

            model.Quantity = 2; // Set the quantity

            // Now valid.
            Assert.IsTrue(model.Validate(), "Validation unit test failed on assumed valid trade");
        }

        private void Validate_Stock_FailureMessage_Test(ITrade trade, String expectedErrorMessage)
        {
            try
            {
                trade.Validate();

                Assert.Fail("Validation passed on invalid trade");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(expectedErrorMessage, ex.Message);
            }
        }
    }
}

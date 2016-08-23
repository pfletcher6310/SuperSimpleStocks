using Microsoft.Extensions.Logging;
using SuperSimpleStocks.Model;
using SuperSimpleStocks.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.BusinessLogicServices
{
    public class StockManagementService : IStockManagementService
    {
        ILogger Logger { get; } = Logging.CreateLogger<StockManagementService>();

        // Dependencies
        public IStockRepository stockRepo;
        public ITradeService tradeService;

        // Management list.
        private Dictionary<String, IStock> Stocks { get; set; }

        public StockManagementService(IStockRepository stockRepo, ITradeService tradeService)
        {
            if (stockRepo == null)
            {
                throw new ArgumentNullException("stockRepo");
            }

            if (tradeService == null)
            {
                throw new ArgumentNullException("tradeService");
            }

            this.stockRepo = stockRepo;
            this.tradeService = tradeService;

            this.Stocks = new Dictionary<String, IStock>();

            // Populate all the stocks into our dictionary
            foreach(IStock stock in stockRepo.GetAllStocks())
            {
                this.Stocks.Add(stock.Symbol, stock);
            }
        }

        private IStock GetStock(String symbol)
        {
            if (this.Stocks.ContainsKey(symbol))
            {
                return this.Stocks[symbol];
            }
            else // Doesn't contain key. Therefore we don't have this symbol.
            {
                String errorMessage = String.Format("Symbol '{0}' not supported", symbol);

                Logger.LogWarning(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        public Boolean BuyAtCurrentPrice(String symbol, Int32 quantity)
        {
            // Call generic method.
            return TradeAtCurrentPrice(symbol, quantity, tradeService.Buy, "Failure buying at current market price");
        }

        public Boolean SellAtCurrentPrice(String symbol, Int32 quantity)
        {
            // Call generic method.
            return TradeAtCurrentPrice(symbol, quantity, tradeService.Sell, "Failure selling at current market price");
        }

        /// <summary>
        /// Trade stocks, generic method to avoid the same checks on Buy/Sell.
        /// </summary>
        /// <param name="symbol">The stock symbol to trade</param>
        /// <param name="quantity">The quantity of stocks to trade</param>
        /// <param name="tradeMethod">The specific trade action to invoke</param>
        /// <returns></returns>
        private Boolean TradeAtCurrentPrice(String symbol, Int32 quantity, Action<IStock, Decimal, Int32, DateTime> tradeAction, String failureMessage)
        {
            try
            {
                if (symbol == null)
                {
                    throw new ArgumentNullException("symbol", "Provide a valid symbol string");
                }

                if (quantity <= 0)
                {
                    throw new ArgumentOutOfRangeException("quantity", "Quantity cannot be zero or lower.");
                }

                IStock stock = GetStock(symbol);
                tradeAction(stock, stock.MarketPrice, quantity, DateTime.UtcNow);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(Logging.Events.TRADE_AT_CURRENT_PRICE, ex, failureMessage);
                throw new Exception(failureMessage, ex);
            }
        }

        public void UpdateMarketPrice(String symbol, Decimal price)
        {
            try
            {
                if(price <= Decimal.Zero)
                {
                    throw new ArgumentOutOfRangeException("price", "Price cannot be zero or lower");
                }

                IStock stock = GetStock(symbol);
                stock.MarketPrice = price;
            }
            catch(Exception ex)
            {
                Logger.LogError(Logging.Events.UPDATE_MARKET_PRICE, ex, "Failure updating market price");
                throw new Exception("Failure updating market price", ex);
            }
        }

        public Decimal GetDividendYield(String symbol)
        {
            try
            {
                IStock stock = GetStock(symbol);

                Decimal yield;

                switch (stock.StockType)
                {
                    case StockType.Common:
                        yield = stock.LastDividendPrice / stock.MarketPrice;
                        break;
                    case StockType.Preferred:
                        yield = (stock.FixedDividend.Value * stock.ParValue) / stock.MarketPrice;
                        break;
                    default:
                        String errorMessage = String.Format("There is no implementation for the dividend yeild calculation of stock type '{0}'", stock.StockType);
                        throw new NotImplementedException(errorMessage);
                }

                return Math.Round(yield, 2);
            }
            catch(Exception ex)
            {
                Logger.LogError(Logging.Events.GET_DIVIDEND_YIELD, ex, "Failure calculating the dividend yield");
                throw new Exception("Failure calculating the dividend yield", ex);
            }
        }

        public Decimal GetPERatio(String symbol)
        {
            try
            {
                IStock stock = GetStock(symbol);

                if(stock.LastDividendPrice == 0)
                {
                    throw new Exception("Cannot calculate a P/E ratio when the last dividend price was zero");
                }

                return Math.Round(stock.MarketPrice / stock.LastDividendPrice, 1);
            }
            catch (Exception ex)
            {
                Logger.LogError(Logging.Events.GET_PERATIO_YIELD, ex, "Failure calculating the P/E ratio");
                throw new Exception("Failure calculating the P/E ratio", ex);
            }
        }

        public Decimal GetIndexPrice()
        {
            try
            {
                Decimal latestPriceMultiplied = Decimal.One;
                Int32 stockCount = 0;

                foreach (var stock in this.Stocks)
                {
                    if(stock.Value.MarketPrice <= Decimal.Zero)
                    {
                        continue; // We can't use this as part of the geometric mean.
                    }

                    latestPriceMultiplied *= stock.Value.MarketPrice;
                    stockCount++;
                }

                Double geometricMean = Math.Pow((Double)latestPriceMultiplied, 1d / stockCount);
                geometricMean = Math.Round(geometricMean, 2);

                return new Decimal(geometricMean);
            }
            catch (Exception ex)
            {
                Logger.LogError(Logging.Events.GET_INDEX_PRICE, ex, "Failure calculating the index price");
                throw new Exception("Failure calculating the index price", ex);
            }
        }
    }
}

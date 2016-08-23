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
    public class TradeService : ITradeService
    {
        ILogger Logger { get; } = Logging.CreateLogger<TradeService>();

        ITradeRepository tradeRepo;

        public TradeService(ITradeRepository tradeRepo)
        {
            if(tradeRepo == null)
            {
                throw new ArgumentNullException("tradeRepo");
            }

            this.tradeRepo = tradeRepo;
        }

        public void Buy(IStock stock, Decimal price, Int32 quantity, DateTime timeStamp)
        {
            Trade buyTrade = new Trade();
            buyTrade.StockInformation = stock;
            buyTrade.Indicator = TradeIndicator.Buy;
            buyTrade.Price = price;
            buyTrade.Quantity = quantity;
            buyTrade.TimeStamp = timeStamp;

            Record(buyTrade);
        }

        public void Sell(IStock stock, Decimal price, Int32 quantity, DateTime timeStamp)
        {
            Trade sellTrade = new Trade();
            sellTrade.StockInformation = stock;
            sellTrade.Indicator = TradeIndicator.Sell;
            sellTrade.Price = price;
            sellTrade.Quantity = quantity;
            sellTrade.TimeStamp = timeStamp;

            Record(sellTrade);
        }

        private void Record(ITrade trade)
        {
            try
            {
                if(trade == null)
                    throw new ArgumentNullException("trade");

                if (trade.Validate())
                {
                    this.tradeRepo.Save(trade);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(Logging.Events.RECORD, ex, "Failure recording trade");
                throw new Exception("Failure recording trade", ex);
            }
        }

        public Decimal GetVolumeWeightedStockPrice(String stockSymbol, Int32 rangeInMinutes)
        {
            try
            {
                var trades = tradeRepo.GetRecentTradesBySymbol(stockSymbol, rangeInMinutes);

                Decimal totalPrice = 0;
                Int32 totalQuantity = 0;

                foreach (var trade in trades)
                {
                    totalPrice += (trade.Price * trade.Quantity);
                    totalQuantity += trade.Quantity;
                }

                Decimal volumeWeightedStockPrice = totalPrice / totalQuantity;

                return Math.Round(volumeWeightedStockPrice, 2);
            }
            catch(Exception ex)
            {
                Logger.LogError(Logging.Events.GET_VOLUME_WEIGHTED_STOCK_PRICE, ex, "Failure calculating volume weighted stock price");
                throw new Exception("Failure calculating volume weighted stock price", ex);
            }
        }
    }
}
using SuperSimpleStocks.Model;
using System;

namespace SuperSimpleStocks.BusinessLogicServices
{
    public interface ITradeService
    {
        void Buy(IStock stock, Decimal price, Int32 quantity, DateTime timeStamp);
        decimal GetVolumeWeightedStockPrice(String stockSymbol, Int32 rangeInMinutes);
        void Sell(IStock stock, decimal price, int quantity, DateTime timeStamp);
    }
}
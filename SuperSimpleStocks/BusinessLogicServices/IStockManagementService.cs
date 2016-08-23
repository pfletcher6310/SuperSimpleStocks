using System;

namespace SuperSimpleStocks.BusinessLogicServices
{
    public interface IStockManagementService
    {
        Decimal GetDividendYield(String symbol);
        Decimal GetIndexPrice();
        Decimal GetPERatio(String symbol);
        void UpdateMarketPrice(String symbol, Decimal price);
        Boolean BuyAtCurrentPrice(String symbol, Int32 quantity);
        Boolean SellAtCurrentPrice(String symbol, Int32 quantity);
    }
}
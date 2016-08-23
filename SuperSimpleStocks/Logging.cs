using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks
{
    public static class Logging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddDebug(); // Debug output for this demo.

        public static ILogger CreateLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        public class Events
        {
            // StockManagementService
            public const Int32 TRADE_AT_CURRENT_PRICE = 1000;
            public const Int32 UPDATE_MARKET_PRICE = 1001;
            public const Int32 GET_DIVIDEND_YIELD = 1002;
            public const Int32 GET_PERATIO_YIELD = 1003;
            public const Int32 GET_INDEX_PRICE = 1004;

            // TradeService
            public const Int32 RECORD = 2000;
            public const Int32 GET_VOLUME_WEIGHTED_STOCK_PRICE = 2001;
        }
    }
}

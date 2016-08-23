using SuperSimpleStocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Repositories
{
    public interface ITradeRepository
    {
        void Save(ITrade trade);

        IEnumerable<ITrade> GetRecentTradesBySymbol(String stockSymbol, Int32 rangeInMinutes);
    }
}

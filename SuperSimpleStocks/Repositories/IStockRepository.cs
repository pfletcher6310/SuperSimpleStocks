using SuperSimpleStocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleStocks.Repositories
{
    public interface IStockRepository
    {
        IEnumerable<IStock> GetAllStocks();
    }
}

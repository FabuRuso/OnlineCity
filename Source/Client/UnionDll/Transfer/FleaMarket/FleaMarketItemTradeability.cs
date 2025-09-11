using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCUnion.Transfer.FleaMarket
{
    [Serializable]
    public enum FleaMarketItemTradeability
    {
        None,
        Sellable,
        Buyable,
        All
    }
}

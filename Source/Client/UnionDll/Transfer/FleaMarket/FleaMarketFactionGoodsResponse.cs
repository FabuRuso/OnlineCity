using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCUnion.Transfer.FleaMarket
{
    [Serializable]
    public class FleaMarketFactionGoodsResponse
    {
        public List<FleaMarketItem> Items { get; set; }
    }
}

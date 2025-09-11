using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCUnion.Transfer.FleaMarket
{
    [Serializable]
    public class FleaMarketBuyItemRequest
    {
        public int Id { get; set; }

        public int Count { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCUnion.Transfer.FleaMarket
{
    [Serializable]
    public class FleaMarketBuyItemResponse
    {
        public bool Result { get; set; }

        public string Message { get; set; }

        public FleaMarketItem Item { get; set; }

        public FleaMarketBuyItemResponse()
        {

        }

        public FleaMarketBuyItemResponse(bool result, string message, FleaMarketItem item)
        {
            this.Result = result;
            this.Message = message;
            this.Item = item;
        }
    }
}

using OCUnion.Transfer.FleaMarket;
using Server.Model.FleaMarket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Common
{
    public static class FleaMarketUtility
    {
        public static FleaMarketItem ToFleaMarketItem(this ServiceFleaMarketItem serviceFleaMarketItem)
        {
            return new FleaMarketItem()
            {
                Id = serviceFleaMarketItem.Id,
                FactionDefName = serviceFleaMarketItem.FactionDefName,
                ItemDefName = serviceFleaMarketItem.ItemDefName,
                Price = serviceFleaMarketItem.Price,
                Count = serviceFleaMarketItem.Count,
                Discount = serviceFleaMarketItem.Discount,
                CurrencyDefName = serviceFleaMarketItem.CurrencyDefName,
                Tradeability = serviceFleaMarketItem.Tradeability,
                CustomData = serviceFleaMarketItem.CustomData,
                RepToUnlock = serviceFleaMarketItem.RepToUnlock
            };
        }
    }
}

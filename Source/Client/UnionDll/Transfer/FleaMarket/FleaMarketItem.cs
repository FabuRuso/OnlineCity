using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCUnion.Transfer.FleaMarket
{
    [Serializable]
    public class FleaMarketItem
    {
        public int Id;

        public string FactionDefName;

        public string ItemDefName;

        public int Price;

        public int Count;

        public float Discount;

        public string CurrencyDefName;

        public FleaMarketItemTradeability Tradeability;

        public List<FleaMarketItemCustomData> CustomData = new List<FleaMarketItemCustomData>();

        public int? RepToUnlock;

        public override string ToString()
        {
            return $"FactionDefName: {FactionDefName} | ItemDefName: {ItemDefName} | Price: {Price} | Count: {Count} | Discount: {Discount} | CurrencyDefName: {CurrencyDefName} | Tradeability: {Tradeability} | RepToUnlock: {RepToUnlock}. CustomData: {string.Join("|", CustomData.Select(x => x.ToString()).ToArray())}";
        }
    }
}

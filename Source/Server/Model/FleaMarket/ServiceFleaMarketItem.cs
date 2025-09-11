using Newtonsoft.Json;
using OCUnion.Transfer.FleaMarket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model.FleaMarket
{
    [Table("fleamarket.faction_good")]
    public class ServiceFleaMarketItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("faction_def")]
        public string FactionDefName { get; set; }

        [Column("item_def")]
        public string ItemDefName { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("discount")]
        public float Discount { get; set; }

        [Column("currency_def")]
        public string CurrencyDefName { get; set; }

        [Column("tradeability")]
        public string TradeabilityStr { get; set; }

        [NotMapped]
        public FleaMarketItemTradeability Tradeability => (FleaMarketItemTradeability)Enum.Parse(typeof(FleaMarketItemTradeability), TradeabilityStr);

        [Column("custom_json_data")]
        public string CustomDataJson { get; set; }

        [Column("rep_to_unlock")]
        public int? RepToUnlock { get; set; }

        [NotMapped]
        public List<FleaMarketItemCustomData> CustomData
        {
            get
            {
                if (customData == null && !string.IsNullOrEmpty(CustomDataJson))
                {
                    customData = JsonConvert.DeserializeObject<List<FleaMarketItemCustomData>>(CustomDataJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
                }

                return customData;
            }
        }
        private List<FleaMarketItemCustomData> customData;

        [Column("added_time")]
        public DateTime AddedTime { get; set; }

    }
}

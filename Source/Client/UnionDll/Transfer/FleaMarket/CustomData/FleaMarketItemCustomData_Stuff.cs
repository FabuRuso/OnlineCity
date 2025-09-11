using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OCUnion.Transfer.FleaMarket.CustomData
{
    [Serializable]
    public class FleaMarketItemCustomData_Stuff : FleaMarketItemCustomData
    {
        public string StuffDefName { get; set; }

        public override void ToThing(FleaMarketItem item, object thing)
        {
            (thing as Thing).SetStuffDirect(DefDatabase<ThingDef>.GetNamed(StuffDefName));
        }

        public override string ToString()
        {
            return $"Stuff: {StuffDefName}";
        }
    }
}

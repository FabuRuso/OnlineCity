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
    public class FleaMarketItemCustomData_Quality : FleaMarketItemCustomData
    {
        public int Quality { get; set; }

        public override void ToThing(FleaMarketItem item, object thing)
        {
            (thing as Thing).TryGetComp<CompQuality>()?.SetQuality((QualityCategory)Quality, ArtGenerationContext.Colony);
        }

        public override string ToString()
        {
            return $"Quality: {Quality}";
        }
    }
}

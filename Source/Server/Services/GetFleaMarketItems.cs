using OCUnion.Transfer.FleaMarket;
using OCUnion.Transfer.Model;
using Server;
using Server.Common;
using ServerCore.Model;
using ServerOnlineCity.Model;
using System;
using System.Diagnostics;
using System.Linq;
using Transfer;

namespace ServerOnlineCity.Services
{

    internal sealed class GetFleaMarketItems : IGenerateResponseContainer
    {
        public int RequestTypePackage => (int)PackageType.Request57GetFactionGoods;

        public int ResponseTypePackage => (int)PackageType.Response57GetFactionGoods;

        public ModelContainer GenerateModelContainer(ModelContainer request, ServiceContext context)
        {
            if (context.Player == null) return null;

            var result = new ModelContainer() { TypePacket = ResponseTypePackage };
            result.Packet = getFactionGoods((FleaMarketFactionGoodsRequest)request.Packet, context);

            return result;
        }

        private FleaMarketFactionGoodsResponse getFactionGoods(FleaMarketFactionGoodsRequest packet, ServiceContext context)
        {
            lock (context.Player)
            {
                var data = Repository.GetData;

                using (NpgsqlDbContext dbContext = new NpgsqlDbContext())
                {
                    var items = dbContext.Items.Where(x => x.FactionDefName == packet.FactionDefName).ToList();

                    return new FleaMarketFactionGoodsResponse() { Items = items.Select(x => x.ToFleaMarketItem()).ToList() };
                }
            }
        }
    }
}

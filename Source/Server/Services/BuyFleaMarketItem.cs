using OCUnion.Transfer.FleaMarket;
using OCUnion.Transfer.Model;
using Server;
using Server.Common;
using Server.Model.FleaMarket;
using ServerCore.Model;
using ServerOnlineCity.Model;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Transfer;

namespace ServerOnlineCity.Services
{

    internal sealed class BuyFleaMarketItem : IGenerateResponseContainer
    {
        //private static Random random = new Random();

        //private const double MinDelay = 0.2d;

        //private const double MaxDelay = 1d;

        public const int TryingCount = 10;

        public int RequestTypePackage => (int)PackageType.Request58BuyFactionGood;

        public int ResponseTypePackage => (int)PackageType.Response58BuyFactionGood;

        public ModelContainer GenerateModelContainer(ModelContainer request, ServiceContext context)
        {
            if (context.Player == null) return null;
            var result = new ModelContainer() { TypePacket = ResponseTypePackage };
            result.Packet = getFactionGoods((FleaMarketBuyItemRequest)request.Packet, context);

            return result;
        }

        private FleaMarketBuyItemResponse getFactionGoods(FleaMarketBuyItemRequest packet, ServiceContext context)
        {
            lock (context.Player)
            {
                using (NpgsqlDbContext dbContext = new NpgsqlDbContext())
                {
                    //Делаем несколько попыток на покупку. Поможет, если несколько человек решили одновременно купить один предмет.
                    for (int i = 0; i < TryingCount; i++)
                    {
                        var transaction = dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
                        try
                        {
                            ServiceFleaMarketItem item = dbContext.Items.SingleOrDefault(it => it.Id == packet.Id);
                            if (item == null)
                            {
                                return new FleaMarketBuyItemResponse(false, "Предмет отсутствует.", null);
                            }
                            if (packet.Count > 0 && item.Count < packet.Count)
                            {
                                return new FleaMarketBuyItemResponse(false, $"Не удалось купить {packet.Count} предмета, доступно только  {item.Count}.", null);
                            }
                            if(packet.Count > 0 && item.Tradeability == FleaMarketItemTradeability.Sellable)
                            {
                                return new FleaMarketBuyItemResponse(false, $"Попытка купить предмет, но предмет только на продажу.", null);
                            }
                            if(packet.Count < 0 && item.Tradeability == FleaMarketItemTradeability.Buyable)
                            {
                                return new FleaMarketBuyItemResponse(false, $"Попытка продать предмет, но предмет только покупается.", null);
                            }

                            if (packet.Count > 0)
                            {
                                if (item.Count - packet.Count <= 0)
                                {
                                    dbContext.Items.Remove(item);
                                }
                                else
                                {
                                    item.Count -= packet.Count;
                                }
                            }

                            dbContext.SaveChanges();

                            transaction.Commit();

                            return new FleaMarketBuyItemResponse(true, "Успех", item.ToFleaMarketItem());
                        }
                        catch
                        {
                            //Ролбек
                            transaction.Rollback();

                            //Увы, код кал, поэтому локать весь поток мы не будем
                            //await Task.Delay((int)((random.NextDouble() * (MaxDelay - MinDelay) + MinDelay) * 1000d));
                        }
                    }

                    return new FleaMarketBuyItemResponse(false, $"Не удалось купить предмет. Попробуйте снова.", null);
                }
            }
        }
    }
}

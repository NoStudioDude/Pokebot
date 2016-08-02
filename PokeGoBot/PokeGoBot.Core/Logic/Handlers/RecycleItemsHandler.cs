using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.CrossCutting.Classes;
using PokeGoBot.Core.CrossCutting.Extenders;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokemonGo.RocketAPI;
using POGOProtos.Inventory.Item;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IRecycleItemsHandler
    {
        Task RecycleItems(Client client);
    }

    public class RecycleItemsHandler : IRecycleItemsHandler
    {
        private readonly ISettingsHandler _settings;
        private readonly ILogger _logger;

        Dictionary<ItemId ,int> ItemRecycleFilter { get; set; }

        public RecycleItemsHandler(ISettingsHandler settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;

            ItemRecycleFilter = new Dictionary<ItemId, int>()
            {
                { ItemId.ItemPokeBall, _settings.Settings.MaxPokeballs },
                { ItemId.ItemGreatBall, _settings.Settings.MaxGreatballs },
                { ItemId.ItemUltraBall, _settings.Settings.MaxUltraballs },
                { ItemId.ItemMasterBall, _settings.Settings.MaxMasterballs },
                { ItemId.ItemRevive, _settings.Settings.MaxRevives },
                { ItemId.ItemMaxRevive, _settings.Settings.MaxTopRevives },
                { ItemId.ItemPotion, _settings.Settings.MaxPotions },
                { ItemId.ItemSuperPotion, _settings.Settings.MaxSuperPotions },
                { ItemId.ItemHyperPotion, _settings.Settings.MaxHyperPotions },
                { ItemId.ItemMaxPotion, _settings.Settings.MaxTopPotions },
                { ItemId.ItemRazzBerry, _settings.Settings.MaxBerrys }
            };
        }

        public async Task RecycleItems(Client client)
        {
            if (ItemRecycleFilter != null)
            {
                var items = await GetItemsToRecycle(client);
                foreach (var item in items)
                {
                    await client.Inventory.RecycleItem(item.Item, item.Count);

                    _logger.Write($"Recycled {item.Count}x {item.Item}", LogLevel.INFO);
                    await Task.Delay(500);
                }
            }
            else
                _logger.Write("No Item recycle filter found", LogLevel.WARN);
        }

        private async Task<IEnumerable<ItemPerCount>> GetItemsToRecycle(Client client)
        {
            var myItems = await InventoryExtender.GetItems(client.Inventory);

            return myItems
                .Where(x => ItemRecycleFilter.Any(f => f.Key == x.ItemId && x.Count > f.Value))
                .Select(x =>
                new ItemPerCount
                {
                    Item = x.ItemId,
                    Count = x.Count - ItemRecycleFilter.Single(f => f.Key == x.ItemId).Value,
                    Unseen = x.Unseen
                });
        }
    }
}

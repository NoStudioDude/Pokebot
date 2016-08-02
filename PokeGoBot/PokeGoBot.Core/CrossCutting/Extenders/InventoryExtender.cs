using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Inventory.Item;

namespace PokeGoBot.Core.CrossCutting.Extenders
{
    public class InventoryExtender
    {
        public static async Task<IEnumerable<ItemData>> GetItems(Inventory inventory)
        {
            var inventoryResponse = await inventory.GetInventory();
            return
                inventoryResponse.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item)
                    .Where(p => p != null);
        }

        public static async Task<int> GetItemAmountByType(ItemId type, Inventory inventory)
        {
            var pokeballs = await GetItems(inventory);
            return pokeballs.FirstOrDefault(i => i.ItemId == type)?.Count ?? 0;
        }
    }
}

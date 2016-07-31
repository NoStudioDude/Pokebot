using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Settings.Master;

namespace PokeGoBot.WPF.Bot.Helpers
{
    public interface IPokemonHelper
    {
        Task<IEnumerable<PokemonData>> GetPokemons(Client client);
        Task<IEnumerable<PokemonSettings>> GetPokemonSettings(Client client);
        Task<IEnumerable<PokemonFamily>> GetPokemonFamilies(Client client);
        Task<PlayerData> GetPlayerData(Client client);
    }

    public class PokemonHelper : IPokemonHelper
    {
        public async Task<IEnumerable<PokemonData>> GetPokemons(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p?.PokemonId > 0);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings(Client client)
        {
            var templates = await client.Download.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonFamily>> GetPokemonFamilies(Client client)
        {
            var inventory = await client.Inventory.GetInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<PlayerData> GetPlayerData(Client client)
        {
            var playerData = await client.Player.GetPlayer();

            return playerData.PlayerData;
        }
    }
}

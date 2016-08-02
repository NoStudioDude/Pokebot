using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.CrossCutting.Extenders;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IPokemonItems
    {
        Task<ItemId> GetBestBall(WildPokemon pokemon, Inventory inventory);
        Task UseBerry(ulong encounterId, string spawnPointId, Client client);
    }

    public class PokemonItems : IPokemonItems
    {
        private const int HIGH_POKEMON_CP = 1000;
        private const int MIDIUM_POKEMON_CP = 600;
        private const int LOWER_POKEMON_CP = 350;

        public async Task<ItemId> GetBestBall(WildPokemon pokemon, Inventory inventory)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;
            
            var pokeBallsCount = await InventoryExtender.GetItemAmountByType(ItemId.ItemPokeBall, inventory);
            var greatBallsCount = await InventoryExtender.GetItemAmountByType(ItemId.ItemGreatBall, inventory);
            var ultraBallsCount = await InventoryExtender.GetItemAmountByType(ItemId.ItemUltraBall, inventory);
            var masterBallsCount = await InventoryExtender.GetItemAmountByType(ItemId.ItemMasterBall, inventory);

            if (masterBallsCount > 0 && pokemonCp >= HIGH_POKEMON_CP)
                return ItemId.ItemMasterBall;
            if (ultraBallsCount > 0 && pokemonCp >= HIGH_POKEMON_CP)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= HIGH_POKEMON_CP)
                return ItemId.ItemGreatBall;

            if (ultraBallsCount > 0 && pokemonCp >= MIDIUM_POKEMON_CP)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= MIDIUM_POKEMON_CP)
                return ItemId.ItemGreatBall;

            if (greatBallsCount > 0 && pokemonCp >= LOWER_POKEMON_CP)
                return ItemId.ItemGreatBall;

            if (pokeBallsCount > 0)
                return ItemId.ItemPokeBall;
            if (greatBallsCount > 0)
                return ItemId.ItemGreatBall;
            if (ultraBallsCount > 0)
                return ItemId.ItemUltraBall;
            if (masterBallsCount > 0)
                return ItemId.ItemMasterBall;

            return ItemId.ItemPokeBall;
        }

        public async Task UseBerry(ulong encounterId, string spawnPointId, Client client)
        {
            var inventoryBalls = await InventoryExtender.GetItems(client.Inventory);
            var berries = inventoryBalls.Where(p => p.ItemId == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            await client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            await Task.Delay(3000);
        }
    }
}

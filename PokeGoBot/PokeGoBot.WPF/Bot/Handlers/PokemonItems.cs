using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;

namespace PokeGoBot.WPF.Bot.Handlers
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

            var pokeBallsCount = await inventory.GetItemAmountByType(ItemId.ItemPokeBall);
            var greatBallsCount = await inventory.GetItemAmountByType(ItemId.ItemGreatBall);
            var ultraBallsCount = await inventory.GetItemAmountByType(ItemId.ItemUltraBall);
            var masterBallsCount = await inventory.GetItemAmountByType(ItemId.ItemMasterBall);

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
            var inventoryBalls = await client.Inventory.GetItems();
            var berries = inventoryBalls.Where(p => p.ItemId == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            await client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            await Task.Delay(3000);
        }
    }
}

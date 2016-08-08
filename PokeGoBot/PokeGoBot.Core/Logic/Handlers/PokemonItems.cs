using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.CrossCutting.Extenders;
using PokeGoBot.Core.Logging;
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
        Task UseLuckyEgg(Client client);
        Task UseIncense(Client client);
    }

    public class PokemonItems : IPokemonItems
    {
        private const int HIGH_POKEMON_CP = 1000;
        private const int MIDIUM_POKEMON_CP = 600;
        private const int LOWER_POKEMON_CP = 350;
        private readonly ILogger _logger;

        public PokemonItems(ILogger _logger)
        {
            this._logger = _logger;
        }

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
            {
                _logger.Write("No berries to use", LogLevel.WARN);
                return;
            }


            await client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            await Task.Delay(3000);
        }

        public async Task UseLuckyEgg(Client client)
        {
            var inventory = await InventoryExtender.GetItems(client.Inventory);
            var luckyEggs = inventory.Where(p => p.ItemId == ItemId.ItemLuckyEgg).ToList();
            var luckyEgg = luckyEggs.FirstOrDefault();

            if (luckyEgg == null)
            {
                _logger.Write("No lucky eggs to use", LogLevel.WARN);
                return;
            }

            await client.Inventory.UseItemXpBoost();

            _logger.Write($"Successfull used lucky egg. Lucky eggs left {luckyEggs.Count() - 1}", LogLevel.SUCC);
            await Task.Delay(3000);
        }

        public async Task UseIncense(Client client)
        {
            var inventory = await InventoryExtender.GetItems(client.Inventory);
            var incenses =
                inventory.Where(
                    p =>
                        p.ItemId == ItemId.ItemIncenseCool || p.ItemId == ItemId.ItemIncenseFloral ||
                        p.ItemId == ItemId.ItemIncenseOrdinary || p.ItemId == ItemId.ItemIncenseSpicy).ToList();
            var incense = incenses.FirstOrDefault();

            if (incense == null)
            {
                _logger.Write("No incenses to use", LogLevel.WARN);
                return;
            }

            await client.Inventory.UseIncense(incense.ItemId);

            _logger.Write($"Successfull used {incense.ItemId}. {incense.ItemId} left {incenses.Count() - 1}", LogLevel.SUCC);
            await Task.Delay(3000);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.WPF.Bot
{
    public class GoBot
    {
        private ISettings _settings;
        private Client _client;
        private Inventory _inventory;

        public void Initialize(ISettings settings)
        {
            _settings = settings;
            _client = new Client(_settings);
            _inventory = new Inventory(_client);
        }

        public async Task Execute()
        {
            while (true)
            {
                try
                {
                    if (_settings.AuthType == AuthType.Ptc)
                        await _client.Login.DoPtcLogin(_settings.PtcUsername, _settings.PtcPassword);
                    else if (_settings.AuthType == AuthType.Google)
                        await
                            _client.Login.DoGoogleLogin(_settings.GoogleUsername, _settings.GooglePassword);

                    await PostLoginExecute();
                }
                catch (AccessTokenExpiredException)
                {
                }
                await Task.Delay(10000);
            }
        }

        public async Task PostLoginExecute()
        {
            while (true)
            {
                try
                {
                    var inventory = _client.Inventory;

                    //await EvolveAllPokemonWithEnoughCandy();
                    //await TransferDuplicatePokemon();
                    //await RecycleItems();
                    if(_settings.CatchPokemons)
                        await ExecuteFarmingPokestopsAndPokemons();
                }
                catch (AccessTokenExpiredException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                }

                await Task.Delay(10000);
            }
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (var i = 0; i < repeat; i++)
                await action();
        }

        private async Task ExecuteFarmingPokestopsAndPokemons()
        {
            var mapObjects = await _client.Map.GetMapObjects();

            var pokeStops =
                mapObjects.MapCells.SelectMany(i => i.Forts)
                    .Where(
                        i =>
                            i.Type.Equals(FortType.Checkpoint) &&
                            i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            foreach (var pokeStop in pokeStops)
            {
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude,
                    pokeStop.Latitude, pokeStop.Longitude);
                var update =
                    await
                        _client.Player.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude,
                            _settings.DefaultAltitude);
                var fortInfo = await _client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await _client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                await Task.Delay(1000);
                //await RecycleItems();
                await ExecuteCatchAllNearbyPokemons();
                //await TransferDuplicatePokemon();
            }
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var mapObjects = await _client.Map.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);

            foreach (var pokemon in pokemons)
            {
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude,
                    pokemon.Latitude, pokemon.Longitude);
                if (distance > 100)
                    await Task.Delay(15000);
                else
                    await Task.Delay(500);

                await
                    _client.Player.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude,
                        _settings.DefaultAltitude);

                var encounter = await _client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                await CatchEncounter(encounter, pokemon);
            }
            await Task.Delay(15000);
        }

        private async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon)
        {
            CatchPokemonResponse caughtPokemonResponse;
            do
            {
                if (encounter?.CaptureProbability.CaptureProbability_.First() < 0.35)
                {
                    await UseBerry(pokemon.EncounterId, pokemon.SpawnPointId);
                }

                var pokeball = await GetBestBall(encounter?.WildPokemon);
                Navigation.DistanceBetween2Coordinates(_client.CurrentLatitude, _client.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);
                caughtPokemonResponse = await _client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);

                await Task.Delay(2000);
            } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                     caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);
        }

        //private async Task EvolveAllPokemonWithEnoughCandy()
        //{
        //    var pokemonToEvolve = await _inventory.GetPokemonToEvolve();
        //    foreach (var pokemon in pokemonToEvolve)
        //    {
        //        var evolvePokemonOutProto = await _client.EvolvePokemon((ulong)pokemon.Id);

        //        if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
        //            Logger.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info);
        //        else
        //            Logger.Write($"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", LogLevel.Info);


        //        await Task.Delay(3000);
        //    }
        //}

        //private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        //{
        //    var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve);

        //    foreach (var duplicatePokemon in duplicatePokemons)
        //    {
        //        var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
        //        Logger.Write($"Transfer {duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} CP", LogLevel.Info);
        //        await Task.Delay(500);
        //    }
        //}

        //private async Task RecycleItems()
        //{
        //    var items = await _inventory.GetItemsToRecycle(_settings);

        //    foreach (var item in items)
        //    {
        //        var transfer = await _client.RecycleItem((AllEnum.ItemId)item.Item_, item.Count);
        //        Logger.Write($"Recycled {item.Count}x {(AllEnum.ItemId)item.Item_}", LogLevel.Info);
        //        await Task.Delay(500);
        //    }
        //}

        private async Task<ItemId> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var pokeBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemPokeBall);
            var greatBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemGreatBall);
            var ultraBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemUltraBall);
            var masterBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemMasterBall);

            if (masterBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemMasterBall;
            else if (ultraBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemUltraBall;
            else if (greatBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemGreatBall;

            if (ultraBallsCount > 0 && pokemonCp >= 600)
                return ItemId.ItemUltraBall;
            else if (greatBallsCount > 0 && pokemonCp >= 600)
                return ItemId.ItemGreatBall;

            if (greatBallsCount > 0 && pokemonCp >= 350)
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

        public async Task UseBerry(ulong encounterId, string spawnPointId)
        {
            var inventoryBalls = await _inventory.GetItems();
            var berries = inventoryBalls.Where(p => p.ItemId == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            await _client.Encounter.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            await Task.Delay(3000);
        }

        //private static string GetSummedFriendlyNameOfItemAwardList(IEnumerable<FortSearchResponse.Types.ItemAward> items)
        //{
        //    var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

        //    if (!enumerable.Any())
        //        return string.Empty;

        //    return
        //        enumerable.GroupBy(i => i.ItemId)
        //                  .Select(kvp => new { ItemName = kvp.Key.ToString(), Amount = kvp.Sum(x => x.ItemCount) })
        //                  .Select(y => $"{y.Amount} x {y.ItemName}")
        //                  .Aggregate((a, b) => $"{a}, {b}");
        //}
    }
}

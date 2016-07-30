using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.WPF.Bot.Helpers;
using PokeGoBot.WPF.Handlers;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Data;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;

namespace PokeGoBot.WPF.Bot.Handlers
{
    public interface IPokemonItems
    {
        Task<ItemId> GetBestBall(WildPokemon pokemon, Inventory inventory);
        Task UseBerry(ulong encounterId, string spawnPointId, Client client);
        Task EvolveAllPokemonWithEnoughCandy(Client client);
        Task RecycleItems(Client client);
    }

    public class PokemonItems : IPokemonItems
    {
        private readonly ISettingsHandler _settings;
        private readonly IPokemonHelper _pokemonHelper;

        public PokemonItems(ISettingsHandler settings, 
                            IPokemonHelper pokemonHelper)
        {
            _settings = settings;
            _pokemonHelper = pokemonHelper;
        }

        public async Task<ItemId> GetBestBall(WildPokemon pokemon, Inventory inventory)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var pokeBallsCount = await inventory.GetItemAmountByType(ItemId.ItemPokeBall);
            var greatBallsCount = await inventory.GetItemAmountByType(ItemId.ItemGreatBall);
            var ultraBallsCount = await inventory.GetItemAmountByType(ItemId.ItemUltraBall);
            var masterBallsCount = await inventory.GetItemAmountByType(ItemId.ItemMasterBall);

            if (masterBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemMasterBall;
            if (ultraBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemGreatBall;

            if (ultraBallsCount > 0 && pokemonCp >= 600)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= 600)
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

        public async Task EvolveAllPokemonWithEnoughCandy(Client client)
        {
            var pokemonToEvolve = await GetPokemonToEvolve(client);
            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = await client.Inventory.EvolvePokemon(pokemon.Id);

                //TODO:Add logger
                //if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                    //Logger.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp",LogLevel.Info);
                //else
                    //Logger.Write( $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}",LogLevel.Info);


                await Task.Delay(3000);
            }
        }

        private async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(Client client)
        {
            var myPokemons = await _pokemonHelper.GetPokemons(client);
            var pokemons = myPokemons.Where(p => p.DeployedFortId == "0").ToList(); //Don't evolve pokemon in gyms

            var myPokemonSettings = await _pokemonHelper.GetPokemonSettings(client);
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies = await _pokemonHelper.GetPokemonFamilies(client);
            var pokemonFamilies = myPokemonFamilies.ToArray();

            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(p => pokemonSettings
                    .Single(x => x.PokemonId == p.PokemonId)
                    .FamilyId == settings.FamilyId) * settings.CandyToEvolve;

                if (familyCandy.Candy - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }

            return pokemonToEvolve;
        }

        public async Task RecycleItems(Client client)
        {
            var items = await GetItemsToRecycle(client);

            foreach (var item in items)
            {
                await client.Inventory.RecycleItem(item.Item, item.Count);

                //Logger.Write($"Recycled {item.Count}x {(AllEnum.ItemId)item.Item_}", LogLevel.Info);
                await Task.Delay(500);
            }
        }

        private async Task<IEnumerable<MiscEnums.ItemPerCount>> GetItemsToRecycle(Client client)
        {
            var myItems = await client.Inventory.GetItems();

            return myItems
                .Where(x => _settings.Settings.ItemRecycleFilter.Any(f => f.Key == x.ItemId && x.Count > f.Value))
                .Select(x => 
                new MiscEnums.ItemPerCount {
                    Item = x.ItemId,
                    Count = x.Count - _settings.Settings.ItemRecycleFilter.Single(f => f.Key == x.ItemId).Value,
                    Unseen = x.Unseen
                });
        }
    }
}

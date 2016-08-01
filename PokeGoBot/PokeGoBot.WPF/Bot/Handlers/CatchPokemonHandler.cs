using System;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.WPF.Bot.Helpers;
using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Logging;
using PokemonGo.RocketAPI;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.WPF.Bot.Handlers
{
    public interface ICatchPokemonHandler
    {
        Task CatchAllNearbyPokemons(Client client);
    }

    public class CatchPokemonHandler : ICatchPokemonHandler
    {
        private readonly ILogger _logger;
        private readonly IPokemonItems _pokemonItems;
        private readonly ISettingsHandler _settings;
        private readonly ITransferPokemonHandler _transferPokemonHandler;
        private readonly IPokemonHelper _pokemonHelper;

        public CatchPokemonHandler(ISettingsHandler settings,
                                   ITransferPokemonHandler transferPokemonHandler,
                                   IPokemonHelper pokemonHelper,
                                   IPokemonItems pokemonItems,
                                   ILogger logger)
        {
            _settings = settings;
            _transferPokemonHandler = transferPokemonHandler;
            _pokemonHelper = pokemonHelper;
            _pokemonItems = pokemonItems;
            _logger = logger;
        }

        public async Task CatchAllNearbyPokemons(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);
            _logger.Write($"Found {pokemons.Count()} nearby pokemons", LogLevel.INFO);

            foreach (var pokemon in pokemons)
            {
                var distance = Navigation.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude,
                    pokemon.Latitude, pokemon.Longitude);

                if (distance <= _settings.Settings.PlayerMaxTravelInMeters)
                {
                    if (_settings.Settings.UpdateLocation)
                    {
                        _logger.Write($"Traveling to location [LAT: {pokemon.Latitude} | LON: {pokemon.Longitude}]", LogLevel.INFO);
                        await Task.Delay(5000);
                    }

                    var encounter = await client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                    _logger.Write(
                        $"Starting encounter with pokemon: {pokemon.PokemonId}. Porbability: {encounter.CaptureProbability.CaptureProbability_.First()}.",
                        LogLevel.INFO);

                    await Task.Delay(2000);
                    await CatchEncounter(encounter, pokemon, client);
                }
            }

            await Task.Delay(_settings.Settings.DelayBetweenActions);
        }

        private async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon, Client client)
        {
            int trace = 0;
            try
            {
                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (encounter?.CaptureProbability.CaptureProbability_.First() < 0.40)
                    {
                        trace = 1;
                        _logger.Write("Using berry", LogLevel.INFO);
                        await _pokemonItems.UseBerry(pokemon.EncounterId, pokemon.SpawnPointId, client);
                    }

                    trace = 2;
                    var pokeball = await _pokemonItems.GetBestBall(encounter?.WildPokemon, client.Inventory);

                    trace = 3;
                    await Task.Delay(1000);
                    caughtPokemonResponse =
                        await client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);

                    trace = 4;
                    await Task.Delay(500);
                } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                         caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                trace = 5;
                _logger.Write($"Caught status: {caughtPokemonResponse.Status}", LogLevel.INFO);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    var wildPokemon = encounter?.WildPokemon.PokemonData;
                    if (wildPokemon != null)
                    {
                        var iv = wildPokemon.IndividualAttack + wildPokemon.IndividualDefense +
                                 wildPokemon.IndividualStamina;

                        _logger.Write($"Pokemon: {pokemon.PokemonId}[{encounter.CaptureProbability}%] - CP: {wildPokemon.Cp}, " +
                        $"IV {iv}", LogLevel.INFO);

                        if (_settings.Settings.QuickTransfer)
                        {
                            if (_pokemonHelper.ShouldTranferPokemon(wildPokemon, _settings.Settings.IvPercentageDiscart))
                                await _transferPokemonHandler.TransferPokemon(client, wildPokemon);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Write($"Exception at trace:{trace}. Ex: {e.Message}", LogLevel.DEBUG);
            }
        }
    }
}

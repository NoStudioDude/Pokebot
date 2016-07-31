using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IWalkingHandler _walkingHandler;
        private readonly ISettingsHandler _settings;

        public CatchPokemonHandler(ISettingsHandler settings,
                                   IPokemonItems pokemonItems,
                                   IWalkingHandler walkingHandler,
                                   ILogger logger)
        {
            _settings = settings;
            _pokemonItems = pokemonItems;
            _walkingHandler = walkingHandler;
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
                _logger.Write($"Caught status: {caughtPokemonResponse.Status}",
                    caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess
                        ? LogLevel.SUCC
                        : LogLevel.INFO);
            }
            catch (Exception e)
            {
                _logger.Write($"Exception at trace:{trace}. Ex: {e.Message}", LogLevel.DEBUG);
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI;
using POGOProtos.Data;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface ICatchPokemonHandler
    {
        Task CatchAllNearbyPokemon(Client client);

        event Action<PokemonData> OnCatch;
        event Action<int> OnExperienceAwarded;
    }

    public class CatchPokemonHandler : ICatchPokemonHandler
    {
        public event Action<PokemonData> OnCatch;
        public event Action<int> OnExperienceAwarded;

        private readonly ILogger _logger;
        private readonly IPokemonHelper _pokemonHelper;
        private readonly IPokemonItems _pokemonItems;
        private readonly ISettingsHandler _settings;
        private readonly ITransferPokemonHandler _transferPokemonHandler;

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

        public async Task CatchAllNearbyPokemon(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokemonList = mapObjects.Item1.MapCells.SelectMany(i => i.CatchablePokemons).ToList();
            _logger.Write($"Found {pokemonList.Count()} nearby pokemon", LogLevel.INFO);

            foreach (var pokemon in pokemonList)
            {
                var distance = Navigation.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude,
                    pokemon.Latitude, pokemon.Longitude);

                if (distance <= _settings.Settings.PlayerMaxTravelInMeters)
                {
                    if (_settings.Settings.UpdateLocation)
                    {
                        _logger.Write($"Walking to location [LAT: {pokemon.Latitude} | LON: {pokemon.Longitude}]",
                            LogLevel.INFO);
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
            CatchPokemonResponse caughtPokemonResponse;
            do
            {
                if (encounter?.CaptureProbability.CaptureProbability_.First() < 0.40)
                {
                    _logger.Write("Using berry", LogLevel.INFO);
                    await _pokemonItems.UseBerry(pokemon.EncounterId, pokemon.SpawnPointId, client);
                }

                var pokeball = await _pokemonItems.GetBestBall(encounter?.WildPokemon, client.Inventory);

                await Task.Delay(1000);
                caughtPokemonResponse =
                    await client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);

                await Task.Delay(500);
            } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                     caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

            _logger.Write($"Caught status: {caughtPokemonResponse.Status}", LogLevel.INFO);

            if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
            {
                var wildPokemon = encounter?.WildPokemon.PokemonData;
                if (wildPokemon != null)
                {
                    OnCatch?.Invoke(wildPokemon);
                    OnExperienceAwarded?.Invoke(caughtPokemonResponse.CaptureAward.Xp.Sum(x => x));

                    var iv = wildPokemon.IndividualAttack + wildPokemon.IndividualDefense +
                             wildPokemon.IndividualStamina;

                    _logger.Write($"Pokemon: {pokemon.PokemonId} - CP: {wildPokemon.Cp}, " +
                                  $"IV {iv}", LogLevel.SUCC);

                    if (_settings.Settings.QuickTransfer)
                    {
                        if (_pokemonHelper.ShouldTranferPokemon(wildPokemon, _settings.Settings.IvPercentageDiscart, 
                            _settings.Settings.KeepMinCp))
                            await _transferPokemonHandler.TransferPokemon(client, wildPokemon, true, true);
                    }
                }
            }
        }
    }
}

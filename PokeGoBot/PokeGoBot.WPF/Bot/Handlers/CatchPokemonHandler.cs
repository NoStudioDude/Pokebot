using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.WPF.Handlers;
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
        private readonly ISettingsHandler _settings;
        private readonly IPokemonItems _pokemonItems;

        public CatchPokemonHandler(ISettingsHandler settings, 
                                   IPokemonItems pokemonItems)
        {
            _settings = settings;
            _pokemonItems = pokemonItems;
        }

        public async Task CatchAllNearbyPokemons(Client client)
        {
            var mapObjects = await client.Map.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);

            foreach (var pokemon in pokemons)
            {
                var distance = Navigation.DistanceBetween2Coordinates(client.CurrentLatitude, client.CurrentLongitude,
                    pokemon.Latitude, pokemon.Longitude);
                if (distance > 100)
                    await Task.Delay(15000);
                else
                    await Task.Delay(500);

                await
                    client.Player.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude,
                        _settings.Settings.DefaultAltitude);

                var encounter = await client.Encounter.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                await CatchEncounter(encounter, pokemon, client);
            }
            await Task.Delay(15000);
        }

        private async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon, Client client)
        {
            CatchPokemonResponse caughtPokemonResponse;
            do
            {
                if (encounter?.CaptureProbability.CaptureProbability_.First() < 0.35)
                {
                    await _pokemonItems.UseBerry(pokemon.EncounterId, pokemon.SpawnPointId, client);
                }

                var pokeball = await _pokemonItems.GetBestBall(encounter?.WildPokemon, client.Inventory);
                Navigation.DistanceBetween2Coordinates(client.CurrentLatitude, client.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);
                caughtPokemonResponse = await client.Encounter.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokeball);

                await Task.Delay(2000);
            } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed ||
                     caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);
        }
    }
}

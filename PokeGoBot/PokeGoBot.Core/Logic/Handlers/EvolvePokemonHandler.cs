using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI;
using POGOProtos.Data;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IEvolvePokemonHandler
    {
        Task EvolveAllPokemonWithEnoughCandy(Client client);
        Task<EvolvePokemonResponse> EvolvePokemon(Client client, PokemonData pokemonData);
    }

    public class EvolvePokemonHandler : IEvolvePokemonHandler
    {
        private readonly ILogger _logger;
        private readonly IPokemonHelper _pokemonHelper;

        public EvolvePokemonHandler(IPokemonHelper pokemonHelper, ILogger logger)
        {
            _pokemonHelper = pokemonHelper;
            _logger = logger;
        }

        public async Task EvolveAllPokemonWithEnoughCandy(Client client)
        {
            var pokemonToEvolve = await GetPokemonToEvolve(client);
            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = await EvolvePokemon(client, pokemon);
                
                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                    _logger.Write(
                        $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp",
                        LogLevel.INFO);
                else
                    _logger.Write(
                        $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}",
                        LogLevel.INFO);

                await Task.Delay(3000);
            }
        }

        public async Task<EvolvePokemonResponse> EvolvePokemon(Client client, PokemonData pokemonData)
        {
            var evolvePokemonOutProto = await client.Inventory.EvolvePokemon(pokemonData.Id);

            return evolvePokemonOutProto;
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

                if (familyCandy.Candy_ - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }

            return pokemonToEvolve;
        }
    }
}

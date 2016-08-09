using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface IEvolvePokemonHandler
    {
        Task EvolveAllPokemonWithEnoughCandy(Client client);
        Task<EvolvePokemonResponse> EvolvePokemon(Client client, PokemonData pokemonData);

        event Action<PokemonData, EvolvePokemonResponse> OnEvolve;
        event Action<int> OnExperienceAwarded;
    }

    public class EvolvePokemonHandler : IEvolvePokemonHandler
    {
        private readonly ILogger _logger;
        private readonly IPokemonHelper _pokemonHelper;
        private readonly ISettingsHandler _settingsHandler;

        public EvolvePokemonHandler(IPokemonHelper pokemonHelper,
            ILogger logger,
            ISettingsHandler settingsHandler)
        {
            _pokemonHelper = pokemonHelper;
            _logger = logger;
            _settingsHandler = settingsHandler;
        }

        public event Action<PokemonData, EvolvePokemonResponse> OnEvolve;
        public event Action<int> OnExperienceAwarded;

        public async Task EvolveAllPokemonWithEnoughCandy(Client client)
        {
            var notEnoughCandyToEvolvePokemon = new Dictionary<PokemonId, bool>();

            var pokemonToEvolve = await GetPokemonToEvolve(client);
            foreach (var pokemon in pokemonToEvolve)
            {
                if (notEnoughCandyToEvolvePokemon.ContainsKey(pokemon.PokemonId))
                    continue;

                var evolvePokemonOutProto = await EvolvePokemon(client, pokemon);

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    OnExperienceAwarded?.Invoke(evolvePokemonOutProto.ExperienceAwarded);

                    _logger.Write(
                        $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp",
                        LogLevel.INFO);

                    await Task.Delay(_settingsHandler.Settings.DelayBetweenActions);
                }
                else
                {
                    _logger.Write(
                        $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}",
                        LogLevel.INFO);

                    if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.FailedInsufficientResources)
                    {
                        notEnoughCandyToEvolvePokemon.Add(pokemon.PokemonId, false);
                    }
                }
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
            var pokemons =
                myPokemons.Where(p => string.IsNullOrEmpty(p.DeployedFortId) | p.DeployedFortId == "0").ToList();

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

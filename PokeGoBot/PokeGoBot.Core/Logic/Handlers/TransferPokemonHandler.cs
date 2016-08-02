using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Helpers;
using PokemonGo.RocketAPI;
using POGOProtos.Data;

namespace PokeGoBot.Core.Logic.Handlers
{
    public interface ITransferPokemonHandler
    {
        Task TransferDuplicatePokemon(Client client, bool keepPokemonsThatCanEvolve);
        Task TransferPokemon(Client client, PokemonData pokemon, bool justCaught = false);
    }

    public class TransferPokemonHandler : ITransferPokemonHandler
    {
        private readonly IPokemonHelper _pokemonHelper;
        private readonly ISettingsHandler _settings;
        private readonly ILogger _logger;

        public TransferPokemonHandler(IPokemonHelper pokemonHelper,
                                      ISettingsHandler settings,
                                      ILogger logger)
        {
            _pokemonHelper = pokemonHelper;
            _settings = settings;
            _logger = logger;
        }

        public async Task TransferPokemon(Client client, PokemonData pokemon, bool justCaught = false)
        {
            var message = $"Tranfering {pokemon.PokemonId}";
            if (justCaught)
                message += " just caught";

            _logger.Write(message, LogLevel.INFO);
            await Task.Delay(1000);
            var transfer = await client.Inventory.TransferPokemon(pokemon.Id);

            _logger.Write($"Reward: {transfer.CandyAwarded} candy", LogLevel.INFO);
        }

        public async Task TransferDuplicatePokemon(Client client, bool keepPokemonsThatCanEvolve)
        {
            var duplicatePokemons = await GetDuplicatePokemonToTransfer(client, keepPokemonsThatCanEvolve);

            foreach (var duplicatePokemon in duplicatePokemons)
            {
                if (duplicatePokemon.Cp < _settings.Settings.KeepMinCp)
                {
                    if (_pokemonHelper.ShouldTranferPokemon(duplicatePokemon, _settings.Settings.IvPercentageDiscart))
                    {
                        await TransferPokemon(client, duplicatePokemon);

                        await Task.Delay(500);
                    }
                }
            }
        }

        private async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(Client client,
            bool keepPokemonsThatCanEvolve)
        {
            var myPokemon = await _pokemonHelper.GetPokemons(client);

            var pokemonList = myPokemon.Where(p => p.DeployedFortId == "0").ToList(); //Don't evolve pokemon in gyms
            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 2).ToList();

                var myPokemonSettings = await _pokemonHelper.GetPokemonSettings(client);
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await _pokemonHelper.GetPokemonFamilies(client);
                var pokemonFamilies = myPokemonFamilies.ToArray();

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                    if (settings.CandyToEvolve == 0)
                        continue;

                    var amountToSkip = (familyCandy.Candy_ + settings.CandyToEvolve - 1) / settings.CandyToEvolve + 2;

                    results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && x.Favorite == 0)
                        .OrderByDescending(x => x.Cp)
                        .ThenBy(n => n.StaminaMax)
                        .Skip(amountToSkip)
                        .ToList());
                }

                return results;
            }

            return pokemonList
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() > 1)
                .SelectMany( p => p.Where(x => x.Favorite == 0)
                            .OrderByDescending(x => x.Cp)
                            .ThenBy(n => n.StaminaMax)
                            .Skip(1)
                            .ToList());
        }
    }
}

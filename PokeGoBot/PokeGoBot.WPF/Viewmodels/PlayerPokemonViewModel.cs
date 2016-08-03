using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic;
using PokeGoBot.Core.Logic.Handlers;
using POGOProtos.Data;
using POGOProtos.Networking.Responses;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IPlayerPokemonViewModel
    {
    }

    public class PlayerPokemonViewModel : BindableBase, IPlayerPokemonViewModel
    {
        private readonly IGoBot _goBot;
        private readonly ITransferPokemonHandler _transferPokemonHandler;
        private readonly IEvolvePokemonHandler _evolvePokemonHandler;
        private readonly ILogger _logger;

        public ObservableCollection<PlayerPokemon> PokemonCollection { get; set; } =
            new ObservableCollection<PlayerPokemon>();

        public PlayerPokemonViewModel(IGoBot goBot,
                                      ITransferPokemonHandler transferPokemonHandler,
                                      IEvolvePokemonHandler evolvePokemonHandler,
                                      ILogger logger)
        {
            _goBot = goBot;
            _transferPokemonHandler = transferPokemonHandler;
            _evolvePokemonHandler = evolvePokemonHandler;
            _logger = logger;

            _goBot.OnLogin += GetPlayerPokemons;
        }

        private async void GetPlayerPokemons()
        {
            var playerInventory = await _goBot.GetInventoryData();
            var playerPokemons = 
                playerInventory.InventoryDelta.InventoryItems.Select(p => p.InventoryItemData.PokemonData);

            foreach (var pokemon in playerPokemons)
            {
                if (pokemon != null)
                {
                    if (pokemon.IsEgg)
                        continue;

                    var playerPokemon = new PlayerPokemon()
                    {
                        PokemonData = pokemon,
                        Count = (int) pokemon.PokemonId,
                        Pokemon = pokemon.PokemonId.ToString(),
                        Cp = pokemon.Cp,
                        Attack = pokemon.IndividualAttack,
                        Defense = pokemon.IndividualDefense,
                        Stamina = pokemon.IndividualStamina,
                        Iv = (pokemon.IndividualAttack + pokemon.IndividualDefense + pokemon.IndividualStamina) / 45,
                        TransferCommand = DelegateCommand<PlayerPokemon>.FromAsyncHandler(TransferPokemon),
                        EvolveCommand = DelegateCommand<PlayerPokemon>.FromAsyncHandler(EvolvePokemon)
                    };

                    var count = PokemonCollection.FirstOrDefault(d => d.PokemonData.Equals(pokemon));
                    if (count == null)
                        PokemonCollection.Add(playerPokemon);
                }
            }
        }

        private async Task TransferPokemon(PlayerPokemon playerPokemon)
        {
            await _transferPokemonHandler.TransferPokemon(_goBot.Client, playerPokemon.PokemonData);
            PokemonCollection.Remove(playerPokemon);
        }

        private async Task EvolvePokemon(PlayerPokemon playerPokemon)
        {
            var evolveResponse = await _evolvePokemonHandler.EvolvePokemon(_goBot.Client, playerPokemon.PokemonData);
            if (evolveResponse.Result == EvolvePokemonResponse.Types.Result.Success)
            {
                _logger.Write(
                    $"Evolved {playerPokemon.Pokemon.ToString()} successfully for {evolveResponse.ExperienceAwarded}xp",
                    LogLevel.INFO);

                PokemonCollection.Remove(playerPokemon);
                GetPlayerPokemons();
            }
            else
                _logger.Write(
                       $"Failed to evolve {playerPokemon.Pokemon.ToString()}. Reason: {evolveResponse.Result}",
                       LogLevel.WARN);
        }
    }

    public class PlayerPokemon
    {
        public PokemonData PokemonData { get; set; }

        public int Count { get; set; }
        public string Pokemon { get; set; }
        public int Cp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Stamina { get; set; }
        public int Iv { get; set; }

        public ICommand TransferCommand { get; set; }
        public ICommand EvolveCommand { get; set; }
        public ICommand PowerUpCommand { get; set; }

        public PlayerPokemon Entity => this;
    }
}

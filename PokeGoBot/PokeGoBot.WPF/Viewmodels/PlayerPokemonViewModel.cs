using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        private readonly ILogger _logger;

        public ObservableCollection<PlayerPokemon> PokemonCollection { get; set; } =
            new ObservableCollection<PlayerPokemon>();

        public PlayerPokemonViewModel(IGoBot goBot,
                                      ITransferPokemonHandler transferPokemonHandler,
                                      IEvolvePokemonHandler evolvePokemonHandler,
                                      ICatchPokemonHandler catchPokemonHandler,
                                      ILogger logger)
        {
            _goBot = goBot;
            _transferPokemonHandler = transferPokemonHandler;
            _evolvePokemonHandler = evolvePokemonHandler;
            _catchPokemonHandler = catchPokemonHandler;
            _logger = logger;

            _goBot.OnLogin += GetPlayerPokemons;
            _transferPokemonHandler.OnTranfer += TransferedPokemon;
            _catchPokemonHandler.OnCatch += CatchedPokemon;
            _evolvePokemonHandler.OnEvolve += EvolvedPokemon;
        }

        private void EvolvedPokemon(PokemonData pokemonData, EvolvePokemonResponse evolvePokemonResponse)
        {
            var playerPokemon = PokemonCollection.FirstOrDefault(d => d.PokemonData.Equals(pokemonData));
            if (playerPokemon != null)
                PokemonCollection.Remove(playerPokemon);

            AddPokemon(evolvePokemonResponse.EvolvedPokemonData);
        }

        private void TransferedPokemon(PokemonData pokemon)
        {
            var playerPokemon = PokemonCollection.FirstOrDefault(d => d.PokemonData.Equals(pokemon));
            if(playerPokemon != null)
                PokemonCollection.Remove(playerPokemon);
        }

        private void CatchedPokemon(PokemonData pokemon)
        {
            AddPokemon(pokemon);
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
                        Iv = Math.Round((double)(pokemon.IndividualAttack + pokemon.IndividualDefense + pokemon.IndividualStamina) / 45, 2, 
                        MidpointRounding.AwayFromZero) * 100,
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
            if (MessageBox.Show($"Are you sure you want to tranfer \b{playerPokemon.Pokemon}", 
                "You are about to tranfer a pokemon", 
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                await _transferPokemonHandler.TransferPokemon(_goBot.Client, playerPokemon.PokemonData);
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

        private void AddPokemon(PokemonData pokemon)
        {
            if (pokemon != null)
            {
                if (pokemon.IsEgg)
                    return;

                PokemonCollection.Add(new PlayerPokemon()
                {
                    PokemonData = pokemon,
                    Count = (int)pokemon.PokemonId,
                    Pokemon = pokemon.PokemonId.ToString(),
                    Cp = pokemon.Cp,
                    Attack = pokemon.IndividualAttack,
                    Defense = pokemon.IndividualDefense,
                    Stamina = pokemon.IndividualStamina,
                    Iv = Math.Round((double)(pokemon.IndividualAttack + pokemon.IndividualDefense + pokemon.IndividualStamina) / 45, 2,
                    MidpointRounding.AwayFromZero) * 100,
                    TransferCommand = DelegateCommand<PlayerPokemon>.FromAsyncHandler(TransferPokemon),
                    EvolveCommand = DelegateCommand<PlayerPokemon>.FromAsyncHandler(EvolvePokemon)
                });
            }
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
        public double Iv { get; set; }

        public ICommand TransferCommand { get; set; }
        public ICommand EvolveCommand { get; set; }
        public ICommand PowerUpCommand { get; set; }

        public PlayerPokemon Entity => this;
    }
}

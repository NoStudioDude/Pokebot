using System;
using System.Threading.Tasks;
using PokeGoBot.WPF.Bot.Handlers;
using PokeGoBot.WPF.Handlers;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;

namespace PokeGoBot.WPF.Bot
{
    public interface IGoBot
    {
        Task ExecuteLoginAndBot();
        Task RepeatAction(int repeat, Func<Task> action);
    }

    public class GoBot : IGoBot
    {
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        private readonly IPokestopsHandler _pokestopsHandler;
        private readonly ITransferPokemonHandler _transferPokemonHandler;
        private readonly IPokemonItems _pokemonItems;
        private readonly Client _client;
        private readonly ISettingsHandler _settings;

        private bool _isActive;

        public GoBot(ISettingsHandler settings, 
                     ICatchPokemonHandler catchPokemonHandler,
                     IPokestopsHandler pokestopsHandler,
                     ITransferPokemonHandler transferPokemonHandler,
                     IPokemonItems pokemonItems)
        {
            _settings = settings;
            _catchPokemonHandler = catchPokemonHandler;
            _pokestopsHandler = pokestopsHandler;
            _transferPokemonHandler = transferPokemonHandler;
            _pokemonItems = pokemonItems;

            _client = new Client(_settings.Settings);
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (var i = 0; i < repeat; i++)
                await action();
        }

        public async Task ExecuteLoginAndBot()
        {
            _isActive = true;

            while (_isActive)
            {
                try
                {
                    await DoLogin();
                    await ExecuteBot();
                }
                catch (AccessTokenExpiredException)
                {
                }
                catch (GoogleException g)
                {
                    _isActive = false;
                }
                catch (Exception)
                {
                    _isActive = false;
                }
                await Task.Delay(10000);
            }
        }

        private async Task ExecuteBot()
        {
            while (_isActive)
            {
                try
                {
                    //await _pokemonItems.RecycleItems(_client);
                    //await _pokestopsHandler.FarmPokestops(_client);
                    //await _catchPokemonHandler.CatchAllNearbyPokemons(_client);
                    //await _transferPokemonHandler.TransferDuplicatePokemon(_client, true);
                    //await _pokemonItems.EvolveAllPokemonWithEnoughCandy(_client);
                }
                catch (AccessTokenExpiredException)
                {
                    _isActive = false;
                }
                catch (Exception ex)
                {
                    _isActive = false;
                }

                await Task.Delay(10000);
            }
        }

        private async Task DoLogin()
        {
            var auth = _settings.Settings.AuthType;
            var username = _settings.Settings.Username;
            var password = _settings.Settings.Password;

            if (auth == AuthType.Google)
                await _client.Login.DoGoogleLogin(username, password);
            else
                await _client.Login.DoPtcLogin(username, password);
        }
    }
}

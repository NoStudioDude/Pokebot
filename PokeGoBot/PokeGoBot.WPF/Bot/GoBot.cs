using System;
using System.Threading.Tasks;
using PokeGoBot.WPF.Bot.Handlers;
using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Logging;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;

namespace PokeGoBot.WPF.Bot
{
    public interface IGoBot
    {
        bool IsActive { get; set; }
        Task ExecuteLoginAndBot();
        Task RepeatAction(int repeat, Func<Task> action);
    }

    public class GoBot : IGoBot
    {
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        private readonly Client _client;
        private readonly ILogger _logger;
        private readonly IPokemonItems _pokemonItems;
        private readonly IPokestopsHandler _pokestopsHandler;
        private readonly ISettingsHandler _settings;
        private readonly ITransferPokemonHandler _transferPokemonHandler;

        public bool IsActive { get; set; }

        public GoBot(ISettingsHandler settings,
            ICatchPokemonHandler catchPokemonHandler,
            IPokestopsHandler pokestopsHandler,
            ITransferPokemonHandler transferPokemonHandler,
            IPokemonItems pokemonItems,
            ILogger logger)
        {
            _settings = settings;
            _catchPokemonHandler = catchPokemonHandler;
            _pokestopsHandler = pokestopsHandler;
            _transferPokemonHandler = transferPokemonHandler;
            _pokemonItems = pokemonItems;
            _logger = logger;

            _client = new Client(_settings.Settings);
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (var i = 0; i < repeat; i++)
                await action();
        }

        public async Task ExecuteLoginAndBot()
        {
            IsActive = true;

            while (IsActive)
            {
                try
                {
                    await DoLogin();
                    await ExecuteBot();
                }
                catch (AccessTokenExpiredException)
                {
                    _logger.Write(
                        $"Login access token expired, attempting to loggin again in {_settings.Settings.DelayBetweenActions / 1000} seconds.",
                        LogLevel.WARN);
                }
                catch (GoogleException g)
                {
                    _logger.Write($"Failed to login to google. [{g.Message}]", LogLevel.ERROR);

                    IsActive = false;
                }
                catch (Exception ex)
                {
                    IsActive = false;
                    _logger.Write($"ExecuteLoginAndBot: {ex.Message}", LogLevel.DEBUG);
                }
                await Task.Delay(_settings.Settings.DelayBetweenActions);
            }

            _logger.Write("Bot stopped", LogLevel.INFO);
        }

        private async Task ExecuteBot()
        {
            _logger.Write("Bot is now running", LogLevel.INFO);

            while (IsActive)
            {
                try
                {
                    if(_settings.Settings.ReciclyItems)
                        await _pokemonItems.RecycleItems(_client);

                    if (_settings.Settings.CatchPokemons)
                        await _catchPokemonHandler.CatchAllNearbyPokemons(_client);

                    if (_settings.Settings.FarmPokestops)
                        await _pokestopsHandler.FarmPokestops(_client);

                    if(_settings.Settings.TransferDuplicates)
                        await _transferPokemonHandler.TransferDuplicatePokemon(_client, true);

                    if(_settings.Settings.EvolvePokemon)
                        await _pokemonItems.EvolveAllPokemonWithEnoughCandy(_client);

                }
                catch (AccessTokenExpiredException)
                {
                    IsActive = false;
                    _logger.Write("AccessTokenExpiredException", LogLevel.DEBUG);
                }
                catch (Exception ex)
                {
                    _logger.Write($"ExecuteBot: {ex.Message}", LogLevel.DEBUG);
                    _logger.Write("Something went wrong attempting to run again, if you keep seen this message restart the bot.", 
                        LogLevel.ERROR);
                }

                await Task.Delay(_settings.Settings.DelayBetweenActions);
            }
        }

        private async Task DoLogin()
        {
            _logger.Write("Loggin in..", LogLevel.INFO);

            var auth = _settings.Settings.AuthType;
            var username = _settings.Settings.Username;
            var password = _settings.Settings.Password;

            if (auth == AuthType.Google)
                await _client.Login.DoGoogleLogin(username, password);
            else
                await _client.Login.DoPtcLogin(username, password);

            _logger.Write("Successfull logged in", LogLevel.SUCC);
        }
    }
}

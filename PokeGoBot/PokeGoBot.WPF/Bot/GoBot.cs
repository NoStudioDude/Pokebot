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
        Client Client { get; }
        bool IsActive { get; set; }
        Task ExecuteLoginAndBot();
        Task ExecuteTasks();
        Task RepeatAction(int repeat, Func<Task> action);
    }

    public class GoBot : IGoBot
    {
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        public Client Client { get; private set; }
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

            Client = new Client(_settings.Settings);
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (var i = 0; i < repeat; i++)
                await action();
        }

        public async Task ExecuteLoginAndBot()
        {
            IsActive = true;
            try
            {
                await DoLogin();
            }
            catch (GoogleException g)
            {
                _logger.Write($"Failed to login to google. [{g.Message}]", LogLevel.ERROR);

                IsActive = false;
            }
            
            await Task.Delay(_settings.Settings.DelayBetweenActions);
        }

        public async Task ExecuteTasks()
        {
            while (IsActive)
            {
                await ExecuteBot();
            }
        }

        private async Task ExecuteBot()
        {
            _logger.Write("Bot is now running", LogLevel.INFO);

            while (IsActive)
            {
                try
                {
                    if(_settings.Settings.ReciclyItems)
                        await _pokemonItems.RecycleItems(Client);

                    if (_settings.Settings.FarmPokestops)
                        await _pokestopsHandler.FarmPokestops(Client);

                    if(_settings.Settings.TransferDuplicates)
                        await _transferPokemonHandler.TransferDuplicatePokemon(Client, true);

                    if(_settings.Settings.EvolvePokemon)
                        await _pokemonItems.EvolveAllPokemonWithEnoughCandy(Client);

                }
                catch (AccessTokenExpiredException)
                {
                    _logger.Write(
                        $"Login access token expired, attempting to loggin again.",
                        LogLevel.WARN);

                    await DoLogin();
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
                await Client.Login.DoGoogleLogin(username, password);
            else
                await Client.Login.DoPtcLogin(username, password);

            _logger.Write("Successfull logged in", LogLevel.SUCC);
        }
    }
}

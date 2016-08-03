using System;
using System.Threading.Tasks;

using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Handlers;

using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;

namespace PokeGoBot.Core.Logic
{
    public interface IGoBot
    {
        Client Client { get; }
        bool IsLoggedIn { get; set; }
        void InitializeClient();
        Task DoLogin();
        Task ExecuteTasks();
        Task RepeatAction(int repeat, Func<Task> action);
    }

    public class GoBot : IGoBot
    {
        private readonly IApiFailureStrategy _apiStrategyHandler;
        private readonly IEvolvePokemonHandler _evolvePokemonHandler;
        private readonly ILogger _logger;
        private readonly IPokestopsHandler _pokestopsHandler;
        private readonly IRecycleItemsHandler _recycleItemsHandler;
        private readonly ISettingsHandler _settings;
        private readonly ITransferPokemonHandler _transferPokemonHandler;

        public Client Client { get; private set; }

        public bool IsLoggedIn { get; set; }

        public GoBot(ISettingsHandler settings,
                     IPokestopsHandler pokestopsHandler,
                     ITransferPokemonHandler transferPokemonHandler,
                     IRecycleItemsHandler recycleItemsHandler,
                     IEvolvePokemonHandler evolvePokemonHandler,
                     IApiFailureStrategy apiStrategyHandler,
                     ILogger logger)
        {
            _settings = settings;
            _pokestopsHandler = pokestopsHandler;
            _transferPokemonHandler = transferPokemonHandler;
            _recycleItemsHandler = recycleItemsHandler;
            _evolvePokemonHandler = evolvePokemonHandler;
            _apiStrategyHandler = apiStrategyHandler;
            _logger = logger;

            InitializeClient();
        }

        public void InitializeClient()
        {
            Client = new Client(_settings.Settings.RocketSettings, _apiStrategyHandler);
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for(var i = 0;i < repeat;i++)
                await action();
        }

        public async Task DoLogin()
        {
            _logger.Write("Loggin in..", LogLevel.INFO);

            try
            {
                await Client.Login.DoLogin();
                IsLoggedIn = true;

                _logger.Write("Successfull logged in", LogLevel.SUCC);
            }
            catch(Exception e)
            {
                _logger.Write($"Not logged in. [Exception] - {e.Message}", LogLevel.ERROR);
                IsLoggedIn = false;
            }
        }

        public async Task ExecuteTasks()
        {
            while(IsLoggedIn)
            {
                await ExecuteBot();
            }
        }

        private async Task ExecuteBot()
        {
            _logger.Write("Bot is now running", LogLevel.INFO);

            try
            {
                if(_settings.Settings.ReciclyItems)
                    await _recycleItemsHandler.RecycleItems(Client);

                if(_settings.Settings.FarmPokestops)
                    await _pokestopsHandler.FarmPokestops(Client);

                if(_settings.Settings.TransferDuplicates)
                    await _transferPokemonHandler.TransferDuplicatePokemon(Client, true);

                if(_settings.Settings.EvolvePokemon)
                    await _evolvePokemonHandler.EvolveAllPokemonWithEnoughCandy(Client);
            }
            catch(AccessTokenExpiredException)
            {
                _logger.Write(
                    $"Login access token expired, attempting to loggin again.",
                    LogLevel.WARN);

                await DoLogin();
            }
            catch(Exception ex)
            {
                _logger.Write($"ExecuteBot: {ex.Message}", LogLevel.DEBUG);
                _logger.Write(
                    "Something went wrong attempting to run again, if you keep seen this message restart the bot.",
                    LogLevel.ERROR);
            }

            await Task.Delay(_settings.Settings.DelayBetweenActions);
        }
    }
}
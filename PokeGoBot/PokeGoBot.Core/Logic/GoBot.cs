using System;
using System.Threading.Tasks;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic.Handlers;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using AuthType = PokemonGo.RocketAPI.Enums.AuthType;

namespace PokeGoBot.Core.Logic
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
        public Client Client { get; private set; }
        private readonly ILogger _logger;
        private readonly IPokestopsHandler _pokestopsHandler;
        private readonly ISettingsHandler _settings;
        private readonly ITransferPokemonHandler _transferPokemonHandler;
        private readonly IRecycleItemsHandler _recycleItemsHandler;
        private readonly IEvolvePokemonHandler _evolvePokemonHandler;

        private ApiStrategyHandler _apiStrategyHandler;

        public bool IsActive { get; set; }

        public GoBot(ISettingsHandler settings,
            IPokestopsHandler pokestopsHandler,
            ITransferPokemonHandler transferPokemonHandler,
            IRecycleItemsHandler recycleItemsHandler,
            IEvolvePokemonHandler evolvePokemonHandler,
            ILogger logger)
        {
            _settings = settings;
            _pokestopsHandler = pokestopsHandler;
            _transferPokemonHandler = transferPokemonHandler;
            _recycleItemsHandler = recycleItemsHandler;
            _evolvePokemonHandler = evolvePokemonHandler;
            _logger = logger;

            _apiStrategyHandler = new ApiStrategyHandler();

            Client = new Client(_settings.Settings.RocketSettings, _apiStrategyHandler);
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
                        await _recycleItemsHandler.RecycleItems(Client);

                    if (_settings.Settings.FarmPokestops)
                        await _pokestopsHandler.FarmPokestops(Client);

                    if(_settings.Settings.TransferDuplicates)
                        await _transferPokemonHandler.TransferDuplicatePokemon(Client, true);

                    if(_settings.Settings.EvolvePokemon)
                        await _evolvePokemonHandler.EvolveAllPokemonWithEnoughCandy(Client);

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

            await Client.Login.DoLogin();

            _logger.Write("Successfull logged in", LogLevel.SUCC);
        }
    }
}

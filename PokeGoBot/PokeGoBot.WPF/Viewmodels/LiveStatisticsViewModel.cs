using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PokeGoBot.Core.Logic;
using PokeGoBot.Core.Logic.Handlers;

using POGOProtos.Inventory.Item;

using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface ILiveStatisticsViewModel
    {
        void StartTimer();
        void StopTimer();
    }

    public class LiveStatisticsViewModel : BindableBase, ILiveStatisticsViewModel
    {
        private readonly DispatcherTimer _dispatcher;
        private readonly IGoBot _goBot;
        private readonly IPokestopsHandler _pokestopHandler;
        private readonly ICatchPokemonHandler _catchPokemonHandler;
        private readonly IEvolvePokemonHandler _evolvePokemonHandler;

        public string Runtime
        {
            get { return _runtime; }
            set { SetProperty(ref _runtime, value); }
        }

        public string Level
        {
            get { return _level; }
            set { SetProperty(ref _level, value); }
        }

        public string Stardust
        {
            get { return _startdust; }
            set { SetProperty(ref _startdust, value); }
        }

        public string Pokemons
        {
            get { return _pokemons; }
            set { SetProperty(ref _pokemons, value); }
        }

        public string Pokestops
        {
            get { return _pokestops; }
            set { SetProperty(ref _pokestops, value); }
        }

        public string Experience
        {
            get { return _experience;}
            set { SetProperty(ref _experience, value); }
        }

        private DateTime _botStartTime;
        private string _runtime;
        private string _level;
        private string _startdust;
        private string _pokemons;
        private string _pokestops;
        private string _experience;

        private int _pokestopCount;
        private int _currentPokestop;
        private int _currentLevel;
        private int _currentExp;
        private int _nextLevelExp;

        public LiveStatisticsViewModel(IGoBot goBot,
                                       IPokestopsHandler pokestopHandler,
                                       ICatchPokemonHandler catchPokemonHandler,
                                       IEvolvePokemonHandler evolvePokemonHandler)
        {
            _goBot = goBot;
            _pokestopHandler = pokestopHandler;
            _catchPokemonHandler = catchPokemonHandler;
            _evolvePokemonHandler = evolvePokemonHandler;

            _goBot.OnLogin += OnLogin;
            _pokestopHandler.OnPokestopFound += OnPokestopFound;
            _pokestopHandler.OnPokestopVisited += OnPokestopVisited;
            _pokestopHandler.OnExperienceAwarded += OnExperienceAwarded;

            _catchPokemonHandler.OnExperienceAwarded += OncaughtExperience;

            Runtime = "00:00:00";
            Level = "#";
            Stardust = "#";
            Pokemons = "#";
            Pokestops = "0/0";
            Experience = "#";

            _dispatcher = new DispatcherTimer();
            _dispatcher.Tick += RunTimeDispatcher;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);
        }

        private void OncaughtExperience(int exp)
        {
            AddExperience(exp);
        }

        private void OnExperienceAwarded(int exp)
        {
            AddExperience(exp);
        }

        private async void AddExperience(int exp)
        {
            _currentExp += exp;
            if (_currentExp >= _nextLevelExp)
            {
                _currentLevel++;
                var inventoryData = await _goBot.GetInventoryData();
                var stat = inventoryData.InventoryDelta.InventoryItems.Select(z => z.InventoryItemData.PlayerStats).FirstOrDefault(p => p != null);
                _nextLevelExp = (int)stat.NextLevelXp;
            }

            Level = $"{_currentLevel}";
            Experience = $"{_currentExp}/{_nextLevelExp}";
        }

        private async void OnLogin()
        {
            var inventoryData = await _goBot.GetInventoryData();
            var stat = inventoryData.InventoryDelta.InventoryItems.Select(z => z.InventoryItemData.PlayerStats).FirstOrDefault(p => p != null);
            _currentLevel = stat.Level;
            _currentExp = (int)stat.Experience;
            _nextLevelExp = (int)stat.NextLevelXp;

            Level = $"{_currentLevel}";
            Experience = $"{_currentExp}/{_nextLevelExp}";

            var player = await _goBot.GetPlayer();
            var stardustAmount = player.PlayerData.Currencies.FirstOrDefault(c => c.Name.ToLower() == "stardust")?.Amount;
            Stardust = $"{stardustAmount}";

            var pokemon = inventoryData.InventoryDelta.InventoryItems.Select(p => p.InventoryItemData.PokemonData).Count();
            Pokemons = $"{pokemon}/ ?";
        }

        private void OnPokestopVisited()
        {
            _currentPokestop++;
            Pokestops = $"{_currentPokestop} / {_pokestopCount}";
        }

        private void OnPokestopFound(int count)
        {
            _currentPokestop = 0;
            _pokestopCount = count;

            Pokestops = $"{_currentPokestop} / {count}";
        }

        private void RunTimeDispatcher(object sender, EventArgs eventArgs)
        {
            var diff = DateTime.Now - _botStartTime;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    Runtime = $"{diff.Hours.ToString("00")}:{diff.Minutes.ToString("00")}:{diff.Seconds.ToString("00")}";
                });
        }

        public void StartTimer()
        {
            _botStartTime = DateTime.Now;
            _dispatcher.Start();
        }

        public void StopTimer()
        {
            _dispatcher.Stop();
            Runtime = "00:00:00";
        }
    }
}

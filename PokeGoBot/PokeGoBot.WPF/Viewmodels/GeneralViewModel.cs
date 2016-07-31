using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PokeGoBot.WPF.Bot;
using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Xceed.Wpf.Toolkit.Primitives;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IGeneralViewModel
    {
    }

    public class GeneralViewModel : BindableBase, IGeneralViewModel
    {
        private readonly ISettingsHandler _settingsHandler;
        private readonly IGoBot _goBot;
        private readonly DispatcherTimer _dispatcher;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public ILogger Logger { get; set; }

        public string Runtime
        {
            get { return _runtime; }
            set { SetProperty(ref _runtime, value); }
        }

        public string PlayerName
        {
            get { return _playerName; }
            set { SetProperty(ref _playerName, value); }
        }

        public string Level
        {
            get { return _level; }
            set { SetProperty(ref _level, value); }
        }

        public string CurrentExp
        {
            get { return _currentExp; }
            set { SetProperty(ref _currentExp, value); }
        }

        public string Stardust
        {
            get { return _startdust; }
            set { SetProperty(ref _startdust, value); }
        }

        public string NumberOfPokemons
        {
            get { return _numberOfPokemons;}
            set { SetProperty(ref _numberOfPokemons, value); }
        }

        public string PokemonsTranfered
        {
            get { return _pokemonsTranfered; }
            set { SetProperty(ref _pokemonsTranfered, value); }
        }

        public bool IsBotRunning
        {
            get { return _isBotRunning; }
            set
            {
                SetProperty(ref _isBotRunning, value);
                StartCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
            }
        }

        private string _runtime;
        private string _playerName;
        private string _level;
        private string _currentExp;
        private string _startdust;
        private string _numberOfPokemons;
        private string _pokemonsTranfered;
        private bool _isBotRunning;
        private DateTime _botStartTime;

        public GeneralViewModel(ISettingsHandler settingsHandler, 
                                IGoBot goBot,
                                ILogger logger)
        {
            _settingsHandler = settingsHandler;
            _goBot = goBot;
            Logger = logger;

            Runtime = "00:00:00";
            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);
            StopCommand = new DelegateCommand(StopBot, CanStopBot);
            StartCommand.RaiseCanExecuteChanged();

            _dispatcher = new DispatcherTimer();
            _dispatcher.Tick += DispatcherOnTick;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);
            
            Logger.Write("App initialized", LogLevel.INFO);
        }

        private void DispatcherOnTick(object sender, EventArgs eventArgs)
        {
            var diff = DateTime.Now - _botStartTime;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Runtime = $"{diff.Hours.ToString("00")}:{diff.Minutes.ToString("00")}:{diff.Seconds.ToString("00")}";
            }));
        }

        private void StopBot()
        {
            Logger.Write("Stopping bot.. Waiting for all actions to be done", LogLevel.INFO);
            _goBot.IsActive = false;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _dispatcher.Stop();
            Runtime = "00:00:00";
        }

        private bool CanStopBot()
        {
            return _isBotRunning;
        }

        public bool CanStartBot()
        {
            return !_isBotRunning;
        }

        public async Task StartBot()
        {
            _botStartTime = DateTime.Now;
            _dispatcher.Start();

            IsBotRunning = true;
            await _goBot.ExecuteLoginAndBot();

            IsBotRunning = false;
            InitializeTimer();
        }
    }
}

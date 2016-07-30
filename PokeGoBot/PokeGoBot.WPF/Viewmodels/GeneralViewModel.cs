using System.Threading.Tasks;
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

        public DelegateCommand StartCommand { get; set; }
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

        private string _runtime;
        private string _playerName;
        private string _level;
        private string _currentExp;
        private string _startdust;
        private string _numberOfPokemons;
        private string _pokemonsTranfered;


        public GeneralViewModel(ISettingsHandler settingsHandler, 
                                IGoBot goBot,
                                ILogger logger)
        {
            _settingsHandler = settingsHandler;
            _goBot = goBot;
            Logger = logger;

            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);
            StartCommand.RaiseCanExecuteChanged();

            Logger.Write("App initialized", LogLevel.INFO);
        }

        public bool CanStartBot()
        {
            return true;
        }

        public async Task StartBot()
        {
            await _goBot.ExecuteLoginAndBot();
        }
    }
}

using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Logging;
using PokemonGo.RocketAPI.Enums;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IConfigurationViewModel
    {
    }

    public class ConfigurationViewModel : BindableBase, IConfigurationViewModel
    {
        private readonly ISettingsHandler _settingsHandler;
        private readonly ILogger _logger;

        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        public bool UseGoogle
        {
            get { return _useGoogle; }
            set
            {
                SetProperty(ref _useGoogle, value);
            }
        }

        public double Latitude
        {
            get { return _latitude;}
            set { SetProperty(ref _latitude, value); }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        public double Altitude
        {
            get { return _altitude; }
            set { SetProperty(ref _altitude, value); }
        }

        public bool TransferDuplicates
        {
            get { return _transferDuplicates;}
            set { SetProperty(ref _transferDuplicates, value); }
        }

        public double KeepMinCp
        {
            get { return _keepMinCp; }
            set { SetProperty(ref _keepMinCp, value); }
        }

        public bool EvolvePokemon
        {
            get { return _evolvePokemon; }
            set { SetProperty(ref _evolvePokemon, value); }
        }

        public bool UseLuckyEgg
        {
            get { return _useLuckyEgg; }
            set { SetProperty(ref _useLuckyEgg, value); }
        }

        public bool UseIncense
        {
            get { return _useIncense; }
            set { SetProperty(ref _useIncense, value); }
        }

        private string _userName;
        private string _password;
        private bool _useGoogle;
        private double _latitude;
        private double _longitude;
        private double _altitude;
        private bool _transferDuplicates;
        private double _keepMinCp;
        private bool _evolvePokemon;
        private bool _useLuckyEgg;
        private bool _useIncense;

        public DelegateCommand SaveCommand { get; set; }

        public ConfigurationViewModel(ISettingsHandler settingsHandler, 
                                      ILogger logger)
        {
            _settingsHandler = settingsHandler;
            _logger = logger;
            LoadSettings();
            
            UseGoogle = _settingsHandler.Settings.AuthType == AuthType.Google;
            SaveCommand = new DelegateCommand(Save);
        }

        public void LoadSettings()
        {
            UseGoogle = _settingsHandler.Settings.AuthType == AuthType.Google;
            UserName = _settingsHandler.Settings.Username;
            Password = _settingsHandler.Settings.Password;

            Latitude = _settingsHandler.Settings.DefaultLatitude;
            Longitude = _settingsHandler.Settings.DefaultLongitude;
            Altitude = _settingsHandler.Settings.DefaultAltitude;

            TransferDuplicates = _settingsHandler.Settings.TransferDuplicates;
            KeepMinCp = _settingsHandler.Settings.KeepMinCp;
            EvolvePokemon = _settingsHandler.Settings.EvolvePokemon;
            UseLuckyEgg = _settingsHandler.Settings.UseLuckyEgg;
            UseIncense = _settingsHandler.Settings.UseIncense;

            _logger.Write("Settings loaded", LogLevel.DEBUG);
        }

        public void Save()
        {
            _settingsHandler.Settings.AuthType = UseGoogle ? AuthType.Google : AuthType.Ptc;
            _settingsHandler.Settings.Username = UserName;
            _settingsHandler.Settings.Password = Password;

            _settingsHandler.Settings.DefaultLatitude = Latitude;
            _settingsHandler.Settings.DefaultLongitude = Longitude;
            _settingsHandler.Settings.DefaultAltitude = Altitude;

            _settingsHandler.Settings.TransferDuplicates = TransferDuplicates;
            _settingsHandler.Settings.KeepMinCp = KeepMinCp;
            _settingsHandler.Settings.EvolvePokemon = EvolvePokemon;
            _settingsHandler.Settings.UseLuckyEgg = UseLuckyEgg;
            _settingsHandler.Settings.UseIncense = UseIncense;

            _settingsHandler.SaveSettings();

            _logger.Write("Settings saved", LogLevel.DEBUG);
        }
    }
}

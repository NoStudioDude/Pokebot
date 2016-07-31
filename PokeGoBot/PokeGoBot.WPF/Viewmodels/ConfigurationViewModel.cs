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

        public double PlayerWalkingSpeed
        {
            get { return _playerWalkingSpeed;}
            set { SetProperty(ref _playerWalkingSpeed, value); }
        }

        public double DelayBetweenActions
        {
            get { return _delayBetweenActions; }
            set { SetProperty(ref _delayBetweenActions, value); }
        }

        public double PlayerMaxTravel
        {
            get { return _playerMaxTravel; }
            set { SetProperty(ref _playerMaxTravel, value); }
        }

        public bool FarmPokestops
        {
            get { return _farmPokestops; }
            set { SetProperty(ref _farmPokestops, value); }
        }

        public bool CatchPokemons
        {
            get { return _catchPokemons; }
            set { SetProperty(ref _catchPokemons, value); }
        }

        public bool ReciclyItems
        {
            get { return _reciclyItems; }
            set { SetProperty(ref _reciclyItems, value); }
        }

        public bool UpdateLocation
        {
            get { return _updateLocation; }
            set { SetProperty(ref _updateLocation, value); }
        }

        public int IvPercentageDiscart
        {
            get { return _ivPercentageDiscart; }
            set { SetProperty(ref _ivPercentageDiscart, value); }
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
        private double _playerWalkingSpeed;
        private double _delayBetweenActions;
        private double _playerMaxTravel;
        private bool _farmPokestops;
        private bool _catchPokemons;
        private bool _reciclyItems;
        private bool _updateLocation;
        private int _ivPercentageDiscart;

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand MadridCommand { get; set; }
        public DelegateCommand NyCentralParkCommand { get; set; }
        public DelegateCommand HollywoodCommand { get; set; }

        public ConfigurationViewModel(ISettingsHandler settingsHandler, 
                                      ILogger logger)
        {
            _settingsHandler = settingsHandler;
            _logger = logger;
            LoadSettings();
            
            UseGoogle = _settingsHandler.Settings.AuthType == AuthType.Google;
            SaveCommand = new DelegateCommand(Save);
            MadridCommand = new DelegateCommand(MadridCoordinates);
            NyCentralParkCommand = new DelegateCommand(NyCoordinates);
            HollywoodCommand = new DelegateCommand(HollywoodCoordinates);
        }

        private void MadridCoordinates()
        {
            Latitude = 40.417426;
            Longitude = -3.68323;
            Altitude = 0;
        }

        private void NyCoordinates()
        {
            Latitude = 40.773858;
            Longitude = -73.971739;
            Altitude = 0;
        }

        private void HollywoodCoordinates()
        {
            Latitude = 34.120527;
            Longitude = -118.300462;
            Altitude = 0;
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

            UpdateLocation = _settingsHandler.Settings.UpdateLocation;
            PlayerWalkingSpeed = _settingsHandler.Settings.PlayerWalkingSpeed;
            DelayBetweenActions = _settingsHandler.Settings.DelayBetweenActions;
            PlayerMaxTravel = _settingsHandler.Settings.PlayerMaxTravelInMeters;
            FarmPokestops = _settingsHandler.Settings.FarmPokestops;
            CatchPokemons = _settingsHandler.Settings.CatchPokemons;
            ReciclyItems = _settingsHandler.Settings.ReciclyItems;
            IvPercentageDiscart = _settingsHandler.Settings.IvPercentageDiscart;


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

            _settingsHandler.Settings.UpdateLocation = UpdateLocation;
            _settingsHandler.Settings.PlayerWalkingSpeed = PlayerWalkingSpeed;
            _settingsHandler.Settings.DelayBetweenActions = (int)DelayBetweenActions;
            _settingsHandler.Settings.PlayerMaxTravelInMeters = PlayerMaxTravel;
            _settingsHandler.Settings.FarmPokestops = FarmPokestops;
            _settingsHandler.Settings.CatchPokemons = CatchPokemons;
            _settingsHandler.Settings.ReciclyItems = ReciclyItems;
            _settingsHandler.Settings.IvPercentageDiscart = IvPercentageDiscart;

            _settingsHandler.SaveSettings();

            _logger.Write("Settings saved", LogLevel.DEBUG);
        }
    }
}

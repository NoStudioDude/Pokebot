using System;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IConfigurationViewModel
    {
        event Action OnSave;
    }

    public class ConfigurationViewModel : BindableBase, IConfigurationViewModel
    {
        public event Action OnSave;

        #region Readonly

        private readonly ISettingsHandler _settingsHandler;
        private readonly ILogger _logger;

        #endregion

        #region Public properties
        
        public double Latitude
        {
            get { return _latitude; }
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
            get { return _transferDuplicates; }
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
            get { return _playerWalkingSpeed; }
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

        public bool QuickTransfer
        {
            get { return _quickTransfer; }
            set { SetProperty(ref _quickTransfer, value); }
        }

        public int MaxPokeballs
        {
            get { return _maxPokeballs; }
            set { SetProperty(ref _maxPokeballs, value); }
        }

        public int MaxGreatballs
        {
            get { return _maxGreatballs; }
            set { SetProperty(ref _maxGreatballs, value); }
        }

        public int MaxUltraballs
        {
            get { return _maxUltraballs; }
            set { SetProperty(ref _maxUltraballs, value); }
        }

        public int MaxMasterballs
        {
            get { return _maxMasterballs; }
            set { SetProperty(ref _maxMasterballs, value); }
        }

        public int MaxRevives
        {
            get { return _maxRevives; }
            set { SetProperty(ref _maxRevives, value); }
        }

        public int MaxTopRevives
        {
            get { return _maxTopRevives; }
            set { SetProperty(ref _maxTopRevives, value); }
        }

        public int MaxPotions
        {
            get { return _maxPotions; }
            set { SetProperty(ref _maxPotions, value); }
        }

        public int MaxSuperPotions
        {
            get { return _maxSuperPotions; }
            set { SetProperty(ref _maxSuperPotions, value); }
        }

        public int MaxHyperPotions
        {
            get { return _maxHyperPotions; }
            set { SetProperty(ref _maxHyperPotions, value); }
        }

        public int MaxTopPotions
        {
            get { return _maxTopPotions; }
            set { SetProperty(ref _maxTopPotions, value); }
        }

        public int MaxBerrys
        {
            get { return _maxBerrys; }
            set { SetProperty(ref _maxBerrys, value); }
        }

        public bool KeepPokemonsThatCanEvolve
        {
            get { return _keepPokemonsThatCanEvolve; }
            set { SetProperty(ref _keepPokemonsThatCanEvolve, value); }
        }
        
        #endregion

        #region Private properties

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
        private bool _quickTransfer;

        private int _maxPokeballs;
        private int _maxGreatballs;
        private int _maxUltraballs;
        private int _maxMasterballs;
        private int _maxRevives;
        private int _maxTopRevives;
        private int _maxPotions;
        private int _maxSuperPotions;
        private int _maxHyperPotions;
        private int _maxTopPotions;
        private int _maxBerrys;
        private bool _keepPokemonsThatCanEvolve;

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand MagikarpNest { get; set; }
        public DelegateCommand NyCentralParkCommand { get; set; }
        public DelegateCommand MachopNestCommand { get; set; }

        #endregion
       
        public ConfigurationViewModel(ISettingsHandler settingsHandler, 
                                      ILogger logger)
        {
            _settingsHandler = settingsHandler;
            _logger = logger;
            LoadSettings();
            
            
            SaveCommand = new DelegateCommand(Save);
            MagikarpNest = new DelegateCommand(MagikarpNestCoordinates);
            NyCentralParkCommand = new DelegateCommand(NyCoordinates);
            MachopNestCommand = new DelegateCommand(MachopNestCoordinates);
        }

        private void MagikarpNestCoordinates()
        {
            Latitude = 52.373124;
            Longitude = 4.895865;
            Altitude = 0;
        }

        private void NyCoordinates()
        {
            Latitude = 40.773858;
            Longitude = -73.971739;
            Altitude = 0;
        }

        private void MachopNestCoordinates()
        {
            Latitude = 36.210981;
            Longitude = -95.908203;
            Altitude = 0;
        }

        public void LoadSettings()
        {
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
            QuickTransfer = _settingsHandler.Settings.QuickTransfer;

            MaxPokeballs = _settingsHandler.Settings.MaxPokeballs;
            MaxGreatballs = _settingsHandler.Settings.MaxGreatballs;
            MaxUltraballs = _settingsHandler.Settings.MaxUltraballs;
            MaxMasterballs = _settingsHandler.Settings.MaxMasterballs;
            MaxRevives = _settingsHandler.Settings.MaxRevives;
            MaxTopRevives = _settingsHandler.Settings.MaxTopRevives;
            MaxPotions = _settingsHandler.Settings.MaxPotions;
            MaxSuperPotions = _settingsHandler.Settings.MaxSuperPotions;
            MaxHyperPotions = _settingsHandler.Settings.MaxHyperPotions;
            MaxTopPotions = _settingsHandler.Settings.MaxTopPotions;
            MaxBerrys = _settingsHandler.Settings.MaxBerrys;
            KeepPokemonsThatCanEvolve = _settingsHandler.Settings.KeepPokemonsThatCanEvolve;
        }

        public void Save()
        {
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
            _settingsHandler.Settings.QuickTransfer = QuickTransfer;

            _settingsHandler.Settings.MaxPokeballs = MaxPokeballs;
            _settingsHandler.Settings.MaxGreatballs = MaxGreatballs;
            _settingsHandler.Settings.MaxUltraballs = MaxUltraballs;
            _settingsHandler.Settings.MaxMasterballs = MaxMasterballs;
            _settingsHandler.Settings.MaxRevives = MaxRevives;
            _settingsHandler.Settings.MaxTopRevives = MaxTopRevives;
            _settingsHandler.Settings.MaxPotions = MaxPotions;
            _settingsHandler.Settings.MaxSuperPotions = MaxSuperPotions;
            _settingsHandler.Settings.MaxHyperPotions = MaxHyperPotions;
            _settingsHandler.Settings.MaxTopPotions = MaxTopPotions;
            _settingsHandler.Settings.MaxBerrys = MaxBerrys;
            _settingsHandler.Settings.KeepPokemonsThatCanEvolve = KeepPokemonsThatCanEvolve;

            _settingsHandler.Settings.SetRocketSettings();
            _settingsHandler.SaveSettings();

            OnSave?.Invoke();

            _logger.Write("Settings saved", LogLevel.INFO);
        }
    }
}

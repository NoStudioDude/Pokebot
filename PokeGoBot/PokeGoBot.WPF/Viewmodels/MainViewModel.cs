using System.Threading.Tasks;
using PokeGoBot.WPF.Bot;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public class MainViewModel : BindableBase
    {
        public string PtcUser
        {
            get { return _ptcUser; }
            set
            {
                SetProperty(ref _ptcUser, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public string PtcPassword
        {
            get { return _ptcPassword; }
            set
            {
                SetProperty(ref _ptcPassword, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public string GoogleUser
        {
            get { return _googleUser;}
            set
            {
                SetProperty(ref _googleUser, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public string GooglePassword
        {
            get { return _googlePassword; }
            set
            {
                SetProperty(ref _googlePassword, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

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

        public bool CatchPokemons
        {
            get { return _catchPokemons;}
            set { SetProperty(ref _catchPokemons, value); }
        }

        public bool UsePtc
        {
            get { return _usePtc; }
            set
            {
                SetProperty(ref _usePtc, value);
                if (value)
                    UseGoogle = false;
            }
        }

        public bool UseGoogle
        {
            get { return _useGoogle; }
            set
            {
                SetProperty(ref _useGoogle, value);
                if (value)
                    UsePtc = false;
            }
        }

        public DelegateCommand StartCommand { get; set; }

        private string _ptcUser;
        private string _ptcPassword;
        private string _googleUser;
        private string _googlePassword;
        private readonly ISettings _settings;
        private readonly GoBot _goBot;

        private bool _usePtc;
        private bool _useGoogle;
        private double _latitude;
        private double _longitude;
        private bool _catchPokemons;
        

        public MainViewModel()
        {
            _settings = new Settings();
            _goBot = new GoBot();
            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);

            StartCommand.RaiseCanExecuteChanged();
        }

        public bool CanStartBot()
        {
            return (!string.IsNullOrEmpty(PtcUser) & !string.IsNullOrEmpty(PtcPassword))
                   || (!string.IsNullOrEmpty(GoogleUser) & !string.IsNullOrEmpty(GooglePassword));
        }

        public async Task StartBot()
        {
            UpdateSettings();
            _goBot.Initialize(_settings);

            await _goBot.Execute();
        }

        public void UpdateSettings()
        {
            _settings.AuthType = UsePtc ? AuthType.Ptc : AuthType.Google;
            _settings.GoogleUsername = GoogleUser;
            _settings.GooglePassword = GooglePassword;
            _settings.PtcUsername = PtcUser;
            _settings.PtcPassword = PtcPassword;

            _settings.GoogleRefreshToken = "";
            _settings.DefaultLatitude = Latitude;
            _settings.DefaultLongitude = Longitude;

            _settings.CatchPokemons = CatchPokemons;
        }
    }
}

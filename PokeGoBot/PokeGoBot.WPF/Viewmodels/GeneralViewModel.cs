using System.Threading.Tasks;
using PokeGoBot.WPF.Bot;
using PokeGoBot.WPF.Handlers;
using PokemonGo.RocketAPI.Enums;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IGeneralViewModel
    {
    }

    public class GeneralViewModel : BindableBase, IGeneralViewModel
    {
        private readonly ISettingsHandler _settingsHandler;
        private readonly IGoBot _goBot;

        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                StartCommand.RaiseCanExecuteChanged();
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

        private string _userName;
        private string _password;
        private bool _useGoogle;
        private bool _saveCredentials;

        public DelegateCommand StartCommand { get; set; }

        public GeneralViewModel(ISettingsHandler settingsHandler, IGoBot goBot)
        {
            _settingsHandler = settingsHandler;
            _goBot = goBot;

            UseGoogle = _settingsHandler.Settings.AuthType == AuthType.Google;

            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);
            StartCommand.RaiseCanExecuteChanged();
        }

        public bool CanStartBot()
        {
            return !string.IsNullOrEmpty(UserName) & !string.IsNullOrEmpty(Password);
        }

        public async Task StartBot()
        {
            _settingsHandler.Settings.AuthType = UseGoogle ? AuthType.Google : AuthType.Ptc;
            _settingsHandler.Settings.Username = UserName;
            _settingsHandler.Settings.Password = Password;

            _settingsHandler.SaveSettings();

            await _goBot.ExecuteLoginAndBot();
        }
    }
}

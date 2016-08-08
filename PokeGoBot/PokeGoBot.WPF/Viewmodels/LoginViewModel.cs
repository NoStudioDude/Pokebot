using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PokeGoBot.Core;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logic;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface ILoginViewModel
    {
        string UserName { get; set; }

        event Action OnLogin;
        event Action GetPokemon;
    }

    public class LoginViewModel : BindableBase, ILoginViewModel
    {
        public struct LoginType
        {
            public LoginAuth LoginAuth;
            public string Type  => LoginAuth.ToString();
        }

        public event Action OnLogin;
        public event Action GetPokemon;

        private readonly ISettingsHandler _settingsHandler;
        private readonly IPlayerPokemonViewModel _playerPokemonViewModel;
        private readonly IGoBot _goBot;

        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        
        public ObservableCollection<LoginType> LoginTypes
        {
            get { return _loginTypes;}
            set { SetProperty(ref _loginTypes, value); }
        }

        public LoginType SelectedLoginType
        {
            get { return _selectedLoginType; }
            set { SetProperty(ref _selectedLoginType, value); }
        }

        private string _userName;
        private string _password;
        private ObservableCollection<LoginType> _loginTypes;
        private LoginType _selectedLoginType;

        public DelegateCommand LoginCommand { get; set; }

        public LoginViewModel(ISettingsHandler settingsHandler,
                              IGoBot goBot)
        {
            _settingsHandler = settingsHandler;
            
            _goBot = goBot;
            LoginCommand = DelegateCommand.FromAsyncHandler(Login, CanLogin);
            LoginTypes = new ObservableCollection<LoginType>(new List<LoginType>()
            {
                new LoginType() { LoginAuth = LoginAuth.Google },
                new LoginType() { LoginAuth = LoginAuth.PCT  }
            });

            SelectedLoginType = LoginTypes.FirstOrDefault(t => t.LoginAuth == _settingsHandler.Settings.LoginAuth);
            
            UserName = _settingsHandler.Settings.Username;
            Password = _settingsHandler.Settings.Password;

            LoginCommand.RaiseCanExecuteChanged();
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }

        public async Task Login()
        {
            _settingsHandler.Settings.LoginAuth = SelectedLoginType.LoginAuth;
            _settingsHandler.Settings.Username = UserName;
            _settingsHandler.Settings.Password = Password;

            _settingsHandler.Settings.SetRocketSettings();
            _settingsHandler.SaveSettings();

            _goBot.InitializeClient();
            await _goBot.DoLogin();

            OnLogin?.Invoke();
            GetPokemon?.Invoke();
        }
    }
}

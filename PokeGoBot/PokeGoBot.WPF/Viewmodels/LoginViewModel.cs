using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

        private readonly DispatcherTimer _dispatcher;
        private readonly ISettingsHandler _settingsHandler;
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
        private DateTime _lastLoginTime;

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

            _dispatcher = new DispatcherTimer();
            _dispatcher.Tick += LoginDispatcher;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);
        }

        private void LoginDispatcher(object sender, EventArgs eventArgs)
        {
            var diff = DateTime.Now - _lastLoginTime;
            if(diff.TotalMinutes >= 25)
            {
                Application.Current.Dispatcher.Invoke(
                async () =>
                {
                    _lastLoginTime = DateTime.Now;
                    await _goBot.DoLogin();

                    LoginCommand.RaiseCanExecuteChanged();
                });
            }
        }

        private bool CanLogin()
        {
            return (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) || !_goBot.IsLoggedIn);
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

            if(_goBot.IsLoggedIn)
            {
                _lastLoginTime = DateTime.Now;
                _dispatcher.Start();

                OnLogin?.Invoke();
                GetPokemon?.Invoke();
            }
            LoginCommand.RaiseCanExecuteChanged();
        }
    }
}

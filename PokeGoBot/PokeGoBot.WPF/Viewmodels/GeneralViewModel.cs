using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PokeGoBot.Core.Logging;
using PokeGoBot.Core.Logic;
using Prism.Commands;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface IGeneralViewModel
    {
    }

    public class GeneralViewModel : BindableBase, IGeneralViewModel
    {
        private readonly DispatcherTimer _dispatcher;
        private readonly IGoBot _goBot;
        private readonly ILogger _logger;

        public string Runtime
        {
            get { return _runtime; }
            set { SetProperty(ref _runtime, value); }
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

        private DateTime _botStartTime;
        private bool _isBotRunning;
        private string _runtime;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        public GeneralViewModel(IGoBot goBot,
            ILogger logger)
        {
            _goBot = goBot;
            _logger = logger;

            Runtime = "00:00:00";
            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);
            StopCommand = new DelegateCommand(StopBot, CanStopBot);
            StartCommand.RaiseCanExecuteChanged();

            _dispatcher = new DispatcherTimer();
            _dispatcher.Tick += RunTimeDispatcher;
            _dispatcher.Interval = new TimeSpan(0, 0, 1);
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

        private void StopBot()
        {
            _logger.Write("Stopping bot.. Waiting for all actions to be done", LogLevel.INFO);
            _goBot.IsLoggedIn = false;
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
            if (_goBot.IsLoggedIn)
            {
                _botStartTime = DateTime.Now;
                _dispatcher.Start();

                IsBotRunning = true;
                await _goBot.ExecuteTasks();

                IsBotRunning = false;
                InitializeTimer();
            }
            else
                _logger.Write("You must first log in", LogLevel.WARN);
        }
    }
}

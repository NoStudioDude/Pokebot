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
        
        private readonly IGoBot _goBot;
        public ILiveStatisticsViewModel LiveStatisticsViewModel { get; set; }
        public IPlayerPokemonViewModel PlayerPokemonViewModel { get; set; }
        public ILogger Logger { get; set; }

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

        private bool _isBotRunning;

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        public GeneralViewModel(IGoBot goBot,
                                IPlayerPokemonViewModel playerPokemonViewModel,
                                ILiveStatisticsViewModel liveStatisticsViewModel,
                                ILogger logger)
        {
            _goBot = goBot;
            LiveStatisticsViewModel = liveStatisticsViewModel;
            PlayerPokemonViewModel = playerPokemonViewModel;
            Logger = logger;
            
            StartCommand = DelegateCommand.FromAsyncHandler(StartBot, CanStartBot);
            StopCommand = new DelegateCommand(StopBot, CanStopBot);
            StartCommand.RaiseCanExecuteChanged();
        }

        private void StopBot()
        {
            Logger.Write("Stopping bot.. Waiting for all actions to be done", LogLevel.INFO);
            _goBot.IsLoggedIn = false;
            LiveStatisticsViewModel.StopTimer();
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
                LiveStatisticsViewModel.StartTimer();

                IsBotRunning = true;
                await _goBot.ExecuteTasks();
                IsBotRunning = false;

                LiveStatisticsViewModel.StopTimer();
            }
            else
                Logger.Write("You must first log in", LogLevel.WARN);
        }
    }
}

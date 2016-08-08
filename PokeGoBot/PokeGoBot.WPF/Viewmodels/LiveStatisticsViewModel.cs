using System;
using System.Windows;
using System.Windows.Threading;
using PokeGoBot.Core.Logic;
using Prism.Mvvm;

namespace PokeGoBot.WPF.Viewmodels
{
    public interface ILiveStatisticsViewModel
    {
        void StartTimer();
        void StopTimer();
    }

    public class LiveStatisticsViewModel : BindableBase, ILiveStatisticsViewModel
    {
        private readonly DispatcherTimer _dispatcher;
        private readonly IGoBot _goBot;

        public string Runtime
        {
            get { return _runtime; }
            set { SetProperty(ref _runtime, value); }
        }

        public string Level
        {
            get { return _level; }
            set { SetProperty(ref _level, value); }
        }

        public string Stardust
        {
            get { return _startdust; }
            set { SetProperty(ref _startdust, value); }
        }

        public string Pokemons
        {
            get { return _pokemons; }
            set { SetProperty(ref _pokemons, value); }
        }

        private DateTime _botStartTime;
        private string _runtime;
        private string _level;
        private string _startdust;
        private string _pokemons;

        public LiveStatisticsViewModel(IGoBot goBot)
        {
            _goBot = goBot;

            Runtime = "00:00:00";
            Level = "#";
            Stardust = "#";
            Pokemons = "#";

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

        public void StartTimer()
        {
            _botStartTime = DateTime.Now;
            _dispatcher.Start();
        }

        public void StopTimer()
        {
            _dispatcher.Stop();
            Runtime = "00:00:00";
        }
    }
}

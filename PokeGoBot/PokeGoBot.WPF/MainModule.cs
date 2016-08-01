using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.Unity;
using PokeGoBot.WPF.Bootstrapping;
using PokeGoBot.WPF.Handlers;
using PokeGoBot.WPF.Viewmodels;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;

namespace PokeGoBot.WPF
{
    public class MainModule
    {
        private static MainWindow _mainWindow;
        public static Application _app { get; set; }
        public static UnityContainer UnityContainer { get; set; }

        [STAThread]
        private static void Main()
        {
            UnityContainer = new UnityContainer();
            UnityContainer.AddNewExtension<Bootstrapper>();
            
            _app = new Application();
            _app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (IsInstanceRunning())
                return;

            InitializeConfig();

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Check you internet connection.", "No Internet?", MessageBoxButton.OKCancel);
                return;
            }

            _mainWindow = new MainWindow();
            var mainViewModel = UnityContainer.Resolve<MainViewModel>();

            _mainWindow.ccMain.Content = mainViewModel;
            _mainWindow.BringIntoView();
            _mainWindow.Show();

            _app.Run();
        }

        private static bool IsInstanceRunning()
        {
            return
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location))
                    .Length > 1;
        }

        private static void InitializeConfig()
        {
            if (!File.Exists(Configuration.ConfigFilePath()))
            {
                File.Create(Configuration.ConfigFilePath()).Close();

                var s = UnityContainer.Resolve<SettingsHandler>();
                s.Settings = new Settings()
                {
                    AuthType = AuthType.Google,
                    DefaultLatitude = 0,
                    DefaultLongitude = 0,
                    CatchPokemons = true,
                    EvolvePokemon = true,
                    KeepMinCp = 500,
                    TransferDuplicates = true,
                    PlayerWalkingSpeed = 5,
                    DelayBetweenActions = 10000,
                    PlayerMaxTravelInMeters = 5,
                    FarmPokestops = true,
                    ReciclyItems = false,
                    IvPercentageDiscart = 80,
                    QuickTransfer = false,
                    MaxPokeballs = 50,
                    MaxGreatballs= 50,
                    MaxUltraballs= 50,
                    MaxMasterballs= 50,
                    MaxRevives= 50,
                    MaxTopRevives= 50,
                    MaxPotions= 50,
                    MaxSuperPotions= 50,
                    MaxHyperPotions= 50,
                    MaxTopPotions= 50,
                    MaxBerrys = 50
                };
                s.SaveSettings();
            }

            
        }
    }
}

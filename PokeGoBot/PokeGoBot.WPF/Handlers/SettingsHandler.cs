using System;
using System.Configuration;
using PokemonGo.RocketAPI;

namespace PokeGoBot.WPF.Handlers
{
    public interface ISettingsHandler
    {
        ISettings Settings { get; set; }
        void SaveSettings();
    }

    public class SettingsHandler : ISettingsHandler
    {
        public SettingsHandler()
        {
            Settings = LoadSettings();
        }

        public ISettings Settings { get; set; }

        public void SaveSettings()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            Properties.Settings.Default.DefaultLatitude = Settings.DefaultLatitude;
            Properties.Settings.Default.DefaultLongitude = Settings.DefaultLongitude;
            Properties.Settings.Default.DefaultAltitude = Settings.DefaultAltitude;
            Properties.Settings.Default.GoogleRefreshToken = Settings.GoogleRefreshToken;
            Properties.Settings.Default.Username = Settings.Username;
            Properties.Settings.Default.Password = Settings.Password;
            Properties.Settings.Default.CatchPokemons = Convert.ToByte(Settings.CatchPokemons);

            config.Save(ConfigurationSaveMode.Modified);
        }

        private static ISettings LoadSettings()
        {
            return new Settings
            {
                DefaultLatitude = Properties.Settings.Default.DefaultLatitude,
                DefaultLongitude = Properties.Settings.Default.DefaultLongitude,
                DefaultAltitude = Properties.Settings.Default.DefaultAltitude,
                GoogleRefreshToken = Properties.Settings.Default.GoogleRefreshToken,
                Username = Properties.Settings.Default.Username,
                Password = Properties.Settings.Default.Password,
                CatchPokemons = Convert.ToBoolean(Properties.Settings.Default.CatchPokemons)
            };
        }
    }
}

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
            JsonSerialization.WriteToJsonFile((Settings)Settings);
        }

        private static ISettings LoadSettings()
        {
            return JsonSerialization.ReadFromJsonFile<Settings>();
        }
    }
}

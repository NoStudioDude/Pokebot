namespace PokeGoBot.Core.Data
{
    public interface ISettingsHandler
    {
        IAppSettings Settings { get; set; }
        void SaveSettings();
    }

    public class SettingsHandler : ISettingsHandler
    {
        public SettingsHandler()
        {
            Settings = LoadSettings();
        }

        public IAppSettings Settings { get; set; }

        public void SaveSettings()
        {
            JsonSerialization.WriteToJsonFile((AppSettings)Settings);

            Settings = LoadSettings();
        }

        private static IAppSettings LoadSettings()
        {
            return JsonSerialization.ReadFromJsonFile<AppSettings>();
        }
    }
}

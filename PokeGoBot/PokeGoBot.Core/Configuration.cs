using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PokeGoBot.Core
{
    public class Configuration
    {
        public static string Version()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        public static string DirectoryCurrent()
        {
            return Directory.GetCurrentDirectory();
        }

        public static string ConfigFilePath()
        {
            return Path.Combine(DirectoryCurrent(), "config.json");
        }
    }
}

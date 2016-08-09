using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PokeGoBot.Core
{
    public class Configuration
    {
        public static string DownloadAppLink()
        {
            return "https://github.com/NoStudioDude/POGO.Easy-Bot";
        }

        public static string Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
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

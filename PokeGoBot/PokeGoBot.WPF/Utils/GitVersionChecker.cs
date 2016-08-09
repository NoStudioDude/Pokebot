using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PokeGoBot.Core;
using PokeGoBot.Core.Logging;

namespace PokeGoBot.WPF.Utils
{
    public interface IGitVersionChecker
    {
        void CheckVersion();
    }

    public class GitVersionChecker : IGitVersionChecker
    {
        private readonly ILogger _logger;

        public GitVersionChecker(ILogger logger)
        {
            _logger = logger;
        }

        public void CheckVersion()
        {
            var currentVersion = Configuration.Version();
            var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

            if (!match.Success) return;
            var gitVersion =
                new Version(
                    $"{match.Groups[1]}.{match.Groups[2]}.{match.Groups[3]}.{match.Groups[4]}");

            if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version)
            {
                _logger.Write(
                    "You're running the lastest version! " +
                    Assembly.GetExecutingAssembly().GetName().Version, LogLevel.INFO);
            }
            else
            {
                _logger.Write($"There is a new Version available: {Configuration.DownloadAppLink()}", LogLevel.INFO);
                _logger.Write($"GitHub Version: {gitVersion} | Local Version: {currentVersion}", LogLevel.WARN);
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/NoStudioDude/POGO.Easy-Bot/master/version.txt");
        }
    }
}

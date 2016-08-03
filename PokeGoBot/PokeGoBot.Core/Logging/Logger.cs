using System;
using System.Collections.ObjectModel;
using PokeGoBot.Core.Data;
using PokeGoBot.Core.Logging.Helpers;

namespace PokeGoBot.Core.Logging
{
    public interface ILogger
    {
        void Write(string message, LogLevel level);
    }

    public class Logger : ILogger
    {
        private readonly ISettingsHandler _settingsHandler;
        private bool _logDebug;
        
        public ObservableCollection<LogMessage> LogCollection { get; set; } = new ObservableCollection<LogMessage>();

        public Logger(ISettingsHandler settingsHandler)
        {
            _settingsHandler = settingsHandler;
        }

        public void Write(string message, LogLevel level)
        {
#if DEBUG
            _logDebug = true;
#endif
            if (level == LogLevel.DEBUG && !_logDebug)
                return;

            if (LogCollection.Count >= _settingsHandler.Settings.LogMessagesCount)
                LogCollection.RemoveAt(LogCollection.Count - 1);

            LogCollection.Add(new LogMessage
            {
                Date = DateTime.Now,
                Message = message,
                Level = level
            });

            LogCollection.SortByDesc(d => d.Date);
        }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        SUCC,
        WARN,
        ERROR
    }

    public class LogMessage
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }
}

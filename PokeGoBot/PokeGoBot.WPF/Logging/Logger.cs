using System;
using System.Collections.ObjectModel;

namespace PokeGoBot.WPF.Logging
{
    public interface ILogger
    {
        void Write(string message, LogLevel level);
    }

    public class Logger : ILogger
    {
        private bool _logDebug;

        public ObservableCollection<LogMessage> LogCollection { get; set; } = new ObservableCollection<LogMessage>();
        
        public void Write(string message, LogLevel level)
        {
#if DEBUG
            _logDebug = true;
#endif
            if(level == LogLevel.DEBUG && !_logDebug)
                return;

            if (LogCollection.Count >= 50)
                LogCollection.RemoveAt(LogCollection.Count -1);

            LogCollection.Add(new LogMessage()
            {
                Message = message,
                Level = level
            });
        }
    }

    public enum LogLevel { DEBUG, INFO, WARN, ERROR }

    public class LogMessage
    {
        public string FormattedMessage => $"{DateTime.Now} [{Level}] | \t {Message}";

        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }
}

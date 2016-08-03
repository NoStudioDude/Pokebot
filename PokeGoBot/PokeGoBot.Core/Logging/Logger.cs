using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PokeGoBot.Core.Logging
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
                LogCollection.RemoveAt(0);

            LogCollection.Add(new LogMessage()
            {
                Date = DateTime.Now,
                Message = message,
                Level = level
            });
        }
    }

    public enum LogLevel { DEBUG, INFO, SUCC, WARN, ERROR }

    public class LogMessage
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }
}

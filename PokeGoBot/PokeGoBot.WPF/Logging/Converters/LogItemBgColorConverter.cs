using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokeGoBot.WPF.Logging.Converters
{
    public class LogItemBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (LogLevel) value;
            var result = Brushes.White;

            if (level == LogLevel.WARN)
                result = Brushes.Yellow;
            else if(level == LogLevel.ERROR)
                result = Brushes.Tomato;
            else if (level == LogLevel.SUCC)
                result = Brushes.ForestGreen;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

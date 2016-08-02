using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PokeGoBot.Core.Logging;

namespace PokeGoBot.WPF.Converters
{
    public class LogItemBgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (LogLevel) value;
            var result = Brushes.White;

            if (level == LogLevel.WARN)
                result = Brushes.Gold;
            else if(level == LogLevel.ERROR)
                result = Brushes.OrangeRed;
            else if (level == LogLevel.SUCC)
                result = Brushes.ForestGreen;
            else if (level == LogLevel.DEBUG)
                result = Brushes.Tomato;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using PokeGoBot.Core.Logging;

namespace PokeGoBot.WPF.Converters
{
    public class LogItemFgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (LogLevel) value;
            var result = Brushes.Black;

            if (level == LogLevel.ERROR && level == LogLevel.DEBUG)
            {
                result = Brushes.White;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

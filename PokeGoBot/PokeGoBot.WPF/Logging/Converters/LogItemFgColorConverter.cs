using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokeGoBot.WPF.Logging.Converters
{
    public class LogItemFgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (LogLevel) value;
            var result = Brushes.Black;

            if (level == LogLevel.ERROR)
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

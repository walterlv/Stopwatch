using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xblero.Frameworks.Converters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class Int32ToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int source = (int) value;
            return source > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility target = (Visibility)value;
            return target == Visibility.Visible ? 1 : 0;
        }
    }
}

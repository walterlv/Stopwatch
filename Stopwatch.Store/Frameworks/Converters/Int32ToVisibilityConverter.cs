using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Xblero.Frameworks.Converters
{
    public class Int32ToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int source = (int)value;
            return source > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility target = (Visibility)value;
            return target == Visibility.Visible ? 1 : 0;
        }
    }
}

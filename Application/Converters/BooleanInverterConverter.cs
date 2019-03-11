using System;
using System.Globalization;
using System.Windows.Data;

namespace AlexNoddings.Infinit3.Application.Converters
{
    internal class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean) return !boolean;

            throw new NotSupportedException($"Only accepts boolean values, not {targetType.Name}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
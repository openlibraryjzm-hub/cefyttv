using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ccc.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : MarkupExtension, IValueConverter
    {
        public string True { get; set; }
        public string False { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? True : False;
            }
            return False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == True;
        }
    }
}

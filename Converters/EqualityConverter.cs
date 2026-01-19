using System;
using System.Globalization;
using System.Windows.Data;

namespace ccc.Converters
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;
            
            var val1 = values[0];
            var val2 = values[1];

            if (val1 == null && val2 == null) return true;
            if (val1 == null || val2 == null) return false;

            // Handle string comparison specifically for case-insensitivity if needed
            if (val1 is string s1 && val2 is string s2)
            {
                return s1.Equals(s2, StringComparison.OrdinalIgnoreCase);
            }

            return val1.Equals(val2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

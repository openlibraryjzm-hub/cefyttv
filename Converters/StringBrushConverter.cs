using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using BrushConverter = System.Windows.Media.BrushConverter;

namespace ccc.Converters
{
    public class StringBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string resourceKey)
            {
                // Try to find the resource in app resources
                if (Application.Current.TryFindResource(resourceKey) is Brush brush)
                {
                    return brush;
                }
                // Fallback to brush from string color code
                try 
                {
                    return new BrushConverter().ConvertFromString(resourceKey);
                }
                catch { }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

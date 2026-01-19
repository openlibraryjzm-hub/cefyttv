using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ccc.Converters
{
    public class OrbSpillGeometryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Expected inputs:
            // [0] IsSpillEnabled (bool)
            // [1] SpillTopLeft (bool)
            // [2] SpillTopRight (bool)
            // [3] SpillBottomLeft (bool)
            // [4] SpillBottomRight (bool)

            if (values.Length < 5) 
                return new EllipseGeometry(new System.Windows.Point(77, 77), 77, 77);

            bool isEnabled = values[0] is bool b && b;
            
            // If Spillover is globally disabled, always return the strict circle.
            if (!isEnabled)
            {
                return new EllipseGeometry(new System.Windows.Point(77, 77), 77, 77);
            }

            bool tl = values[1] is bool b1 && b1;
            bool tr = values[2] is bool b2 && b2;
            bool bl = values[3] is bool b3 && b3;
            bool br = values[4] is bool b4 && b4;

            // GeometryGroup acts as a Union of its children
            var group = new GeometryGroup();
            group.FillRule = FillRule.Nonzero;

            // 1. The Base Circle (Always present to cover the core)
            group.Children.Add(new EllipseGeometry(new System.Windows.Point(77, 77), 77, 77));

            // 2. The Spillover Rectangles (Large enough to allow "Infinite" spill in that direction)
            // Center is 77,77. 
            // We use -500 to +500 to simulate "Infinity" relative to the 154px box.

            if (tl) group.Children.Add(new RectangleGeometry(new Rect(-500, -500, 577, 577))); // Top-Left to Center
            if (tr) group.Children.Add(new RectangleGeometry(new Rect(77, -500, 500, 577)));   // Center to Top-Right
            if (bl) group.Children.Add(new RectangleGeometry(new Rect(-500, 77, 577, 500)));   // Bottom-Left to Center
            if (br) group.Children.Add(new RectangleGeometry(new Rect(77, 77, 500, 500)));     // Center to Bottom-Right

            return group;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

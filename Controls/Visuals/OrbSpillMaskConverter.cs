using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ccc.Controls.Visuals
{
    public class OrbSpillMaskConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Values: TopLeft (bool), TopRight, BottomLeft, BottomRight
            bool tl = values.Length > 0 && values[0] is bool b0 && b0;
            bool tr = values.Length > 1 && values[1] is bool b1 && b1;
            bool bl = values.Length > 2 && values[2] is bool b2 && b2;
            bool br = values.Length > 3 && values[3] is bool b3 && b3;

            var geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.Nonzero; // Union
            
            // Container Size: 500x500
            // Center: 250, 250
            // Orb Radius: 75 (Diameter 150) - Same visual size as before
            
            // 1. Base Circle (Center of the 500x500 area)
            geometryGroup.Children.Add(new EllipseGeometry(new System.Windows.Point(250, 250), 75, 75));

            // 2. Add Corners if enabled - Extending to the edges of the 500x500 box
            // Top Left Quadrant: 0,0 to 250,250
            if (tl) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(0, 0, 250, 250)));
            
            // Top Right Quadrant: 250,0 to 500,250
            if (tr) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(250, 0, 250, 250)));
            
            // Bottom Left Quadrant: 0,250 to 250,500
            if (bl) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(0, 250, 250, 250)));
            
            // Bottom Right Quadrant: 250,250 to 500,500
            if (br) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(250, 250, 250, 250)));

            var drawing = new GeometryDrawing(System.Windows.Media.Brushes.Black, null, geometryGroup);
            return new DrawingBrush(drawing) { Stretch = Stretch.None, ViewportUnits = BrushMappingMode.Absolute, Viewport = new System.Windows.Rect(0,0,500,500) };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

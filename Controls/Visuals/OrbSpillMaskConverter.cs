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
            // Values: IsSpillEnabled, TopLeft, TopRight, BottomLeft, BottomRight
            bool isSpillEnabled = values.Length > 0 && values[0] is bool b0 && b0;
            bool tl = values.Length > 1 && values[1] is bool b1 && b1;
            bool tr = values.Length > 2 && values[2] is bool b2 && b2;
            bool bl = values.Length > 3 && values[3] is bool b3 && b3;
            bool br = values.Length > 4 && values[4] is bool b4 && b4;

            // Normalized Coordinates (0 to 1)
            // Center: 0.5, 0.5
            // Radius: 0.5 (Diameter 1.0)
            
            var geometryGroup = new GeometryGroup();
            geometryGroup.FillRule = FillRule.Nonzero; // Union behavior

            // 1. Base Circle
            geometryGroup.Children.Add(new EllipseGeometry(new System.Windows.Point(0.5, 0.5), 0.5, 0.5));

            // If Master Toggle is OFF, return just the circle ( clipped )
            if (!isSpillEnabled)
            {
                 var circleDrawing = new GeometryDrawing(System.Windows.Media.Brushes.Black, null, geometryGroup);
                 return new DrawingBrush(circleDrawing) 
                 { 
                    Stretch = Stretch.Fill, 
                    ViewportUnits = BrushMappingMode.RelativeToBoundingBox, 
                    Viewport = new System.Windows.Rect(0, 0, 1, 1) 
                 };
            }

            // 2. Add Spill Quadrants (Extending way beyond 0-1)
            // Note: In RelativeToBoundingBox, 0,0 is top-left, 1,1 is bottom-right.
            // We use large negative/positive values to simulate "infinite" spill.
            
            // Top Left (-10 to 0.5, -10 to 0.5)
            if (tl) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(-10, -10, 10.5, 10.5)));
            
            // Top Right (0.5 to 11, -10 to 0.5)
            if (tr) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(0.5, -10, 10.5, 10.5)));
            
            // Bottom Left (-10 to 0.5, 0.5 to 11)
            if (bl) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(-10, 0.5, 10.5, 10.5)));
            
            // Bottom Right (0.5 to 11, 0.5 to 11)
            if (br) geometryGroup.Children.Add(new RectangleGeometry(new System.Windows.Rect(0.5, 0.5, 10.5, 10.5)));

            var drawing = new GeometryDrawing(System.Windows.Media.Brushes.Black, null, geometryGroup);
            
            // Key: Use RelativeToBoundingBox to make it size-agnostic
            return new DrawingBrush(drawing) 
            { 
                Stretch = Stretch.Fill, 
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox, 
                Viewport = new System.Windows.Rect(0, 0, 1, 1) 
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

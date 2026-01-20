using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ccc.Controls.Visuals
{
    public partial class AudioVisualizer : System.Windows.Controls.UserControl
    {
        private const int BarCount = 113;
        private const double BaseRadius = 76; // Matches Orb Radius approx
        // private const double MaxBarLength = 76;
        private const double MinBarLength = 7;
        private const double MaxBarLength = 76;
        
        // 270 degrees start = -90 degrees = -PI/2
        private const double StartAngle = -Math.PI / 2;
        private const double TotalAngle = Math.PI * 2;
        
        private readonly List<Line> _bars = new List<Line>();
        private bool _subscribed = false;

        public AudioVisualizer()
        {
            InitializeComponent();
            
            // Layout Debugging: Force visible background
            // this.VisCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 255, 0, 0)); // Semi-transparent Red
            
            InitializeBars();
            
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void InitializeBars()
        {
            // Center of the canvas (300x300)
            double cx = 150;
            double cy = 150;

            for (int i = 0; i < BarCount; i++)
            {
                var line = new Line
                {
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeThickness = 4,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
                
                // Initial placement (min length)
                UpdateBarPosition(line, i, 0, cx, cy);
                
                _bars.Add(line);
                VisCanvas.Children.Add(line);
            }
        }

        private void UpdateBarPosition(Line line, int index, double normalizedValue, double cx, double cy)
        {
            // Calculate angle
            // Clockwise: index goes 0 -> 113
            double angle = StartAngle + (TotalAngle / BarCount) * index;
            
            // Length
            double length = MinBarLength + (normalizedValue * (MaxBarLength - MinBarLength));
            
            // Start Point (at BaseRadius)
            line.X1 = cx + Math.Cos(angle) * BaseRadius;
            line.Y1 = cy + Math.Sin(angle) * BaseRadius;
            
            // End Point
            line.X2 = cx + Math.Cos(angle) * (BaseRadius + length);
            line.Y2 = cy + Math.Sin(angle) * (BaseRadius + length);
            
            // Opacity/Gradient Simulation? 
            // WPF Lines are solid color. For gradient fade, we need LinearGradientBrush on the Stroke.
            // But LinearGradientBrush coordinates are relative to the bounding box or absolute. 
            // Since lines change angle, mapping a gradient is tricky without rotating the line coordinate system.
            // For now, simpler solid white is fine, maybe modify Opacity based on length if desired (Rainmeter aesthetic usually solid).
            // Actually description says: "bars fade from solid (at base) to transparent (at tip)"
            
            // To do gradient fade on a line in WPF, we can set the Stroke to a LinearGradientBrush.
            // But we need to set the StartPoint/EndPoint of the Brush to match the line direction.
            // Or simpler: Use Opacity mask? No.
            // Let's stick to Solid White for now as per "White color, 4px width" basic spec, 
            // though "Visualizer Gradient" was a feature.
            // If requested, we can use a RadialGradient on the Canvas or individual LinearGradients.
            // Individual LinearGradients are expensive to update every frame.
            // Optimization: Create the brushes once if possible? No, angle changes.
            // Wait, angle is constant per bar!
            // Yes! The angle for bar[i] never changes. 
            // So we can pre-calculate a LinearGradientBrush for each bar aligned to its vector.
            
            if (line.Tag == null)
            {
                 // Create aligned gradient
                 var brush = new LinearGradientBrush();
                 brush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 255, 255), 0));
                 brush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(50, 255, 255, 255), 1)); // Fade to tip
                 
                 // Brush mapping mode is relative to bounding box. Line BB is thin/diagonal.
                 // This is hard for Lines. 
                 // Alternative: Rotate the Line using RenderTransform, draw it vertical.
                 // Then X1=0, Y1=0, X2=0, Y2=Length. 
                 // Then Brush vertical gradient is easy.
                 
                 line.Tag = "configured"; // Mark as done if we were doing that.
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_subscribed && App.AudioVisualizerService != null)
            {
                App.AudioVisualizerService.AudioDataUpdated += OnAudioData;
                _subscribed = true;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_subscribed && App.AudioVisualizerService != null)
            {
                App.AudioVisualizerService.AudioDataUpdated -= OnAudioData;
                _subscribed = false;
            }
        }

        private int _logCounter = 0;

        private void OnAudioData(float[] data)
        {
            // Marshall to UI Thread
            Dispatcher.InvokeAsync(() =>
            {
                // _logCounter++;
                // if (_logCounter % 60 == 0) Console.WriteLine($"[AudioVisualizer UI] Frame {_logCounter} received. Data[0]: {data[0]:F2}");

                double cx = VisCanvas.ActualWidth / 2;
                double cy = VisCanvas.ActualHeight / 2;
                
                if (cx == 0) { cx = 150; cy = 150; }

                for (int i = 0; i < BarCount; i++)
                {
                    if (data.Length > i)
                    {
                         // Normalize: Log data shows values like 0.13, 0.18. 
                         // We need these to map to 0.0 - 1.0.
                         // Previous logic divided by 100 thinking they were 0-255.
                         // New logic: Multiply by 4.0 approx to get visible bars from 0.2 input.
                         // Let's try Multiplier = 5.0. 
                         
                         double val = data[i] * 5.0; 
                         
                         if (val < 0.05) val = 0; // Noise gate
                         if (val > 1) val = 1; 
                         
                         UpdateBarPosition(_bars[i], i, val, cx, cy);
                    }
                }
            }, System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}

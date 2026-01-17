using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Visuals
{
    public partial class UnifiedBannerBackground : UserControl
    {
        public UnifiedBannerBackground()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UnifiedBannerBackground),
                new PropertyMetadata(null, OnImageSourceChanged));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(UnifiedBannerBackground),
                new PropertyMetadata(0.0, OnLayoutParamChanged));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty BannerTotalHeightProperty =
            DependencyProperty.Register("BannerTotalHeight", typeof(double), typeof(UnifiedBannerBackground),
                new PropertyMetadata(280.0, OnLayoutParamChanged));

        public double BannerTotalHeight
        {
            get { return (double)GetValue(BannerTotalHeightProperty); }
            set { SetValue(BannerTotalHeightProperty, value); }
        }

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (UnifiedBannerBackground)d;
            ctrl.Image1.Source = (ImageSource)e.NewValue;
            ctrl.Image2.Source = (ImageSource)e.NewValue;
            
            // Re-setup animation when image changes/loads
            ctrl.SetupAnimation();
        }

        private static void OnLayoutParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as UnifiedBannerBackground)?.UpdateLayoutPositions();
        }

        private void UpdateLayoutPositions()
        {
            if (ImageSource == null) return;
            
            // Fixed Height Logic for Stitching consistency
            double h = BannerTotalHeight;
            if (h <= 0) h = ActualHeight > 0 ? ActualHeight : 220;
            
            Image1.Height = h;
            Image2.Height = h;
            
            // Apply Y Offset (Stitching)
            // We move the images UP by VerticalOffset
            Canvas.SetTop(Image1, -VerticalOffset);
            Canvas.SetTop(Image2, -VerticalOffset);
        }

        private void SetupAnimation()
        {
             Image1.SizeChanged -= Image1_SizeChanged;
             Image1.SizeChanged += Image1_SizeChanged;
        }

        private void Image1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            if (width <= 0) return;

            // Position Image 2 next to Image 1
            Canvas.SetLeft(Image1, 0);
            Canvas.SetLeft(Image2, width - 0.5); // Slight overlap

            // Start Animation
            StartScrolling(width);
        }

        private void StartScrolling(double width)
        {
            // Stop existing
            ScrollTransform.BeginAnimation(TranslateTransform.XProperty, null);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = -width,
                Duration = TimeSpan.FromSeconds(60), // Configurable?
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Using pure Animation on Transform
            ScrollTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}

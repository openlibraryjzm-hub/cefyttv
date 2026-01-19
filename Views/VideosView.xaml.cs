using System;
using System.Windows;
using System.Windows.Controls;

namespace ccc.Views
{
    public partial class VideosView : System.Windows.Controls.UserControl
    {
        public VideosView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Initial position: The toolbar should be exactly below the banner.
            // As we scroll down, the banner moves up (off screen), dragging the toolbar up with it.
            // UNTIL the toolbar hits the top of the container (Margin.Top = 0), at which point it sticks.
            
            if (PageBannerControl == null || StickyToolbar == null) return;

            // Get current banner height (or default if not loaded yet)
            double bannerHeight = PageBannerControl.ActualHeight;

            // Calculate the new Top margin.
            // Formula: StartTop (BannerHeight) - ScrollOffset. 
            // Clamp at 0 so it never goes above the top edge.
            
            double newTop = Math.Max(0, (bannerHeight - 21) - e.VerticalOffset);

            // Left/Right margin 0, relied on HorizontalAlignment="Center" in XAML
            StickyToolbar.Margin = new Thickness(0, newTop, 18, 0);
        }
    }
}

using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Views
{
    public partial class PlaylistsView : System.Windows.Controls.UserControl
    {
        public PlaylistsView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (PageBannerControl == null || StickyToolbar == null) return;

            // Get current banner height
            double bannerHeight = PageBannerControl.ActualHeight;

            // Compute new top margin. 
            // Margin clamps at 0 so it sticks to the top.
            double newTop = System.Math.Max(0, bannerHeight - e.VerticalOffset);

            // Left/Right margin 0, relied on HorizontalAlignment="Center" in XAML
            StickyToolbar.Margin = new System.Windows.Thickness(0, newTop, 0, 0);
        }
    }
}

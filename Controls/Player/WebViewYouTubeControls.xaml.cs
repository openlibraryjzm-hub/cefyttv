using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace ccc.Controls.Player
{
    public partial class WebViewYouTubeControls : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty VideoIdProperty =
            DependencyProperty.Register("VideoId", typeof(string), typeof(WebViewYouTubeControls), new PropertyMetadata(null, OnVideoIdChanged));

        public string VideoId
        {
            get { return (string)GetValue(VideoIdProperty); }
            set { SetValue(VideoIdProperty, value); }
        }

        public WebViewYouTubeControls()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // Ensure Runtime is ready
            await PlayerWebView.EnsureCoreWebView2Async();
            PlayerWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        }

        private static void OnVideoIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebViewYouTubeControls control && e.NewValue is string newId)
            {
                control.LoadVideo(newId);
            }
        }

        public void LoadVideo(string videoId)
        {
            if (string.IsNullOrEmpty(videoId)) return;

            // Hide overlay
            OverlayText.Visibility = Visibility.Collapsed;

            if (PlayerWebView.CoreWebView2 != null)
            {
                // Simple Embed URL for now. 
                // Later we can move to a local HTML wrapper for better control (looping, events)
                var url = $"https://www.youtube.com/embed/{videoId}?autoplay=1&rel=0&modestbranding=1";
                PlayerWebView.Source = new Uri(url);
            }
        }
    }
}

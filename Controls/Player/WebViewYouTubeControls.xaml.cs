using System;
using System.IO;
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

        private bool _isInitialized = false;
        private string? _pendingVideoId;

        public WebViewYouTubeControls()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // Ensure Runtime is ready
            await PlayerWebView.EnsureCoreWebView2Async();

            // Set User Agent to spoof a real browser to avoid "embedded" blocks
            PlayerWebView.CoreWebView2.Settings.UserAgent = 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

            PlayerWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            // Map the "assets" folder to a virtual host "app.local"
            // This allows us to serve player.html from https://app.local/player.html
            // avoiding file:// protocol restrictions and CORS issues.
            var assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
            if (Directory.Exists(assetsPath))
            {
                PlayerWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "app.local", 
                    assetsPath, 
                    CoreWebView2HostResourceAccessKind.Allow);
            }

            _isInitialized = true;

            // Process any pending video load that happened before initialization
            if (!string.IsNullOrEmpty(_pendingVideoId))
            {
                LoadVideo(_pendingVideoId);
            }
        }

        private static void OnVideoIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebViewYouTubeControls control && e.NewValue is string newId)
            {
                control.LoadVideo(newId);
            }
        }

        private bool _hasNavigatedToPlayer = false;

        public async void LoadVideo(string videoId)
        {
            _pendingVideoId = videoId;

            if (string.IsNullOrEmpty(videoId)) return;

            // Hide overlay
            OverlayText.Visibility = Visibility.Collapsed;

            // Only act if initialized
            if (_isInitialized && PlayerWebView.CoreWebView2 != null)
            {
                if (_hasNavigatedToPlayer)
                {
                    // FAST PATH: Player is already loaded, just swap video via JS
                    // This avoids the expensive full page reload
                    await PlayerWebView.CoreWebView2.ExecuteScriptAsync($"loadAppVideo('{videoId}')");
                }
                else
                {
                    // SLOW PATH: First load, navigate to the local player wrapper
                    // We pass the ID so it starts playing immediately upon load
                    var url = $"https://app.local/player.html?v={videoId}";
                    PlayerWebView.Source = new Uri(url);
                    _hasNavigatedToPlayer = true;
                }
            }
        }
    }
}

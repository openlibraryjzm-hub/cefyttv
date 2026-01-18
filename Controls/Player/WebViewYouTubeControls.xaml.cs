using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
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

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(double), typeof(WebViewYouTubeControls), new PropertyMetadata(0.0));

        public double StartTime
        {
            get { return (double)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty VideoEndedCommandProperty =
            DependencyProperty.Register("VideoEndedCommand", typeof(ICommand), typeof(WebViewYouTubeControls), new PropertyMetadata(null));

        public ICommand VideoEndedCommand
        {
            get { return (ICommand)GetValue(VideoEndedCommandProperty); }
            set { SetValue(VideoEndedCommandProperty, value); }
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

            // Subscribe to messages from JS
            PlayerWebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // Set User Agent to spoof a real browser to avoid "embedded" blocks
            PlayerWebView.CoreWebView2.Settings.UserAgent = 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

            PlayerWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            // Map the "assets" folder to a virtual host "app.local"
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

        private async void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(message)) return;

            if (message == "VIDEO_ENDED")
            {
                // Reset progress on end
                if (!string.IsNullOrEmpty(VideoId))
                {
                    await App.SqliteService.UpdateVideoProgressAsync(VideoId, "", 0, 0); 
                }

                if (VideoEndedCommand != null && VideoEndedCommand.CanExecute(null))
                {
                    VideoEndedCommand.Execute(null);
                }
            }
            else if (message.StartsWith("PROGRESS:"))
            {
                // Format: PROGRESS:currentTime|duration
                var parts = message.Substring(9).Split('|');
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double current) && 
                    double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double duration))
                {
                    if (!string.IsNullOrEmpty(VideoId))
                    {
                         // Fire and forget update
                         // TODO: Maybe debounce this? But SqliteService is fast enough for 3s interval.
                         await App.SqliteService.UpdateVideoProgressAsync(VideoId, "", duration, current);
                    }
                }
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
                    // Use InvariantCulture for the StartTime to ensure dot decimal separator
                    string script = $"loadAppVideo('{videoId}', {StartTime.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                    await PlayerWebView.CoreWebView2.ExecuteScriptAsync(script);
                }
                else
                {
                    // SLOW PATH: First load, navigate to the local player wrapper
                    // Pass t=StartTime if > 0
                    var url = $"https://app.local/player.html?v={videoId}";
                    if (StartTime > 0)
                    {
                        url += $"&t={StartTime}";
                    }
                    PlayerWebView.Source = new Uri(url);
                    _hasNavigatedToPlayer = true;
                }
            }
        }
    }
}

using System.Windows;

namespace ccc;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeWebView();
    }

    private async void InitializeWebView()
    {
        try
        {
            await PlayerWebView.EnsureCoreWebView2Async();
            
            // Map "app.local" to the physical "assets" folder
            // giving the iframe a valid Origin header (https://app.local)
            // We now copy assets to output, so it's just BaseDirectory/assets
            string assetsPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "assets");
            
            PlayerWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.local", 
                assetsPath, 
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            // Navigate to our local player with the video ID
            PlayerWebView.CoreWebView2.Navigate("https://app.local/player.html?v=QiemgC39tA0");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"WebView Error: {ex.Message}");
        }
    }
}
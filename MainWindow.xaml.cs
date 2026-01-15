using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ccc;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Hook up the initial Tab's browser if defined in XAML
        // Note: For a robust app, it's often better to create the first tab dynamically too,
        // but if we keep XAML, we must find it.
        // For now, let's just make sure new tabs work perfectly.
    }

    private void NewTab_Click(object sender, RoutedEventArgs e)
    {
        OpenNewTab("https://www.bing.com");
    }

    // The centralized method for creating "Optimized" browsers
    public void OpenNewTab(string url)
    {
        var browser = new CefSharp.Wpf.ChromiumWebBrowser();
        browser.Address = url;

        // 1. Versatility: Handle Popups as new Tabs
        browser.LifeSpanHandler = new Handlers.CustomLifeSpanHandler((targetUrl) => 
        {
            OpenNewTab(targetUrl);
        });

        // 2. Extensions: Inject AdBlock/Logic
        browser.RequestHandler = new Handlers.CustomRequestHandler();

        var tab = new TabItem
        {
            Header = "New Tab", // In a real app, bind this to browser.Title
            Content = browser
        };

        BrowserTabs.Items.Add(tab);
        BrowserTabs.SelectedItem = tab;
    }

    // ---------------------------------------------------------
    // Layout Logic (WebView2 vs CefSharp)
    // ---------------------------------------------------------

    // 1. WebView Takes Entire App
    private void Layout_WvFull_Click(object sender, RoutedEventArgs e)
    {
        ResetLayout();
        // Collapse Right Column
        Col1.Width = new GridLength(0);
    }

    // 2. Split (Default)
    private void Layout_Split_Click(object sender, RoutedEventArgs e)
    {
        ResetLayout();
        // Just the reset is enough as it restores 50/50
    }

    // 3. WebView Persists on Left Half, Cef Disappears
    private void Layout_WvHalf_Click(object sender, RoutedEventArgs e)
    {
        ResetLayout();
        // Right Pane hidden, but column width stays to keep VW on left half
        RightPane.Visibility = Visibility.Collapsed;
    }

    // 4. Cef Takes Entire App
    private void Layout_CefFull_Click(object sender, RoutedEventArgs e)
    {
        ResetLayout();
        // Collapse Left Column entirely (buttons are now safe in bottom row)
        Col0.Width = new GridLength(0);
        YoutubeView.Visibility = Visibility.Collapsed;
    }

    private void ResetLayout()
    {
        // Restore standard 50/50 split
        Col0.Width = new GridLength(1, GridUnitType.Star);
        Col1.Width = new GridLength(1, GridUnitType.Star);

        // Restore content visibility
        RightPane.Visibility = Visibility.Visible;
        YoutubeView.Visibility = Visibility.Visible;
        MpvView.Visibility = Visibility.Collapsed;
    }

    private void TestMpv_Click(object sender, RoutedEventArgs e)
    {
        // Simple Toggle for Testing
        if (MpvView.Visibility == Visibility.Visible)
        {
            // Switch back to YouTube
            MpvView.Stop();
            MpvView.Visibility = Visibility.Collapsed;
            YoutubeView.Visibility = Visibility.Visible;
        }
        else
        {
            // Switch to MPV
            YoutubeView.Visibility = Visibility.Collapsed;
            MpvView.Visibility = Visibility.Visible;
            
            // Just init, don't play anything yet
        }
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.mkv;*.avi;*.mov;*.webm|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            // Force Mode Switch: Show MPV, Hide YouTube
            YoutubeView.Visibility = Visibility.Collapsed;
            MpvView.Visibility = Visibility.Visible;

            // Play the File
            MpvView.Play(dialog.FileName);
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace ccc.Views
{
    public partial class BrowserView : System.Windows.Controls.UserControl
    {
        public BrowserView()
        {
            InitializeComponent();
            this.Loaded += BrowserView_Loaded;
        }

        private async void BrowserView_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView(MainWebView);
            MainWebView.Source = new System.Uri("https://www.google.com");
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            OpenNewTab("https://www.bing.com");
        }

        public async void OpenNewTab(string url)
        {
            var webView = new Microsoft.Web.WebView2.Wpf.WebView2();
            // Do not set Source here, init first
            
            await InitializeWebView(webView);
            webView.Source = new System.Uri(url);

            var tab = new TabItem
            {
                Header = "New Tab", 
                Content = webView
            };

            BrowserTabs.Items.Add(tab);
            BrowserTabs.SelectedItem = tab;
        }

        // Flag to prevent redundant extension loading calls
        private static bool _extensionsLoaded = false;

        private async System.Threading.Tasks.Task InitializeWebView(Microsoft.Web.WebView2.Wpf.WebView2 webView)
        {
            try 
            {
                // Ensure Global Env is ready
                await App.EnsureWebViewEnvironmentAsync();
                
                // Init Core with shared env
                await webView.EnsureCoreWebView2Async(App.WebEnv);

                // 2. Load Custom Extensions (Only once per session)
                if (!_extensionsLoaded)
                {
                    // We check multiple locations to be safe across Dev/Release builds
                    string[] possiblePaths = new string[] 
                    {
                        System.IO.Path.Combine(System.Environment.CurrentDirectory, "Extensions"),
                        System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Extensions"),
                        System.IO.Path.Combine(System.IO.Path.GetFullPath("."), "Extensions")
                    };

                    string? validPath = possiblePaths.FirstOrDefault(p => System.IO.Directory.Exists(p));

                    if (validPath != null)
                    {
                        foreach (var dir in System.IO.Directory.GetDirectories(validPath))
                        {
                            try 
                            {
                                await webView.CoreWebView2.Profile.AddBrowserExtensionAsync(dir);
                            }
                            catch { /* Ignore duplicate loading errors */ }
                        }
                    }
                    _extensionsLoaded = true;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView Init Error: {ex.Message}");
            }
            
            // 3. Popup Handling (Keep in Tab logic)
            webView.CoreWebView2.NewWindowRequested += (s, e) =>
            {
                e.Handled = true; 
                webView.Source = new System.Uri(e.Uri);
            };

            // 4. Download Handling
            webView.CoreWebView2.DownloadStarting += (s, e) =>
            {
                e.Handled = true; // Suppress default dialog
                
                string fileName = System.IO.Path.GetFileName(e.ResultFilePath);
                
                // Fallback only if totally empty
                if (string.IsNullOrWhiteSpace(fileName)) 
                {
                    fileName = "download_" + System.DateTime.Now.Ticks + ".tmp";
                }

                var downloadPath = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), 
                    "Downloads", 
                    fileName);

                e.ResultFilePath = downloadPath;
            };
        }


    }
}

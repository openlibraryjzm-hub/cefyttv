using System.Windows;
using System.Windows.Controls;
using System.Linq;
using ccc.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace ccc.Views
{
    public partial class BrowserView : System.Windows.Controls.UserControl
    {
        public BrowserView()
        {
            InitializeComponent();
        }

        // Flag to prevent redundant extension loading calls (Static across all instances)
        private static bool _extensionsLoaded = false;

        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            var webView = sender as Microsoft.Web.WebView2.Wpf.WebView2;
            if (webView == null) return;

            // Wait for DataContext
            if (webView.DataContext is not BrowserTabViewModel tabVm) return;

            try
            {
                // 1. Ensure Global Env is ready
                await App.EnsureWebViewEnvironmentAsync();

                // 2. Init Core with shared env
                await webView.EnsureCoreWebView2Async(App.WebEnv);

                // 3. Load Extensions (Once per session profile)
                if (!_extensionsLoaded)
                {
                    await LoadExtensions(webView);
                    _extensionsLoaded = true;
                }

                // 4. Initial Navigation
                if (!string.IsNullOrEmpty(tabVm.Url))
                {
                    webView.CoreWebView2.Navigate(tabVm.Url);
                }

                // 5. Setup Event Bindings (Two-Way)
                
                // VM -> View (Navigation request)
                System.ComponentModel.PropertyChangedEventHandler vmHandler = (s, args) =>
                {
                    if (args.PropertyName == nameof(BrowserTabViewModel.Url))
                    {
                        if (webView.CoreWebView2 != null && webView.Source.ToString() != tabVm.Url)
                        {
                            try 
                            { 
                                webView.CoreWebView2.Navigate(tabVm.Url); 
                            } 
                            catch { /* Ignore nav errors */ }
                        }
                    }
                };
                tabVm.PropertyChanged += vmHandler;

                // View -> VM (Update Address Bar & Title)
                webView.CoreWebView2.SourceChanged += (s, args) =>
                {
                    // Update VM without triggering the listener loop 
                    // (The VM setter typically triggers NotifyPropertyChanged, 
                    // so we rely on the check `webView.Source != tabVm.Url` above to break loop)
                    tabVm.Url = webView.Source.ToString();
                    
                    // Also update main address bar if this is the selected tab (Handled by BrowserViewModel)
                };

                webView.CoreWebView2.DocumentTitleChanged += (s, args) =>
                {
                    tabVm.Title = webView.CoreWebView2.DocumentTitle;
                };

                // 6. Cleanup on Unload
                webView.Unloaded += (s, args) =>
                {
                    tabVm.PropertyChanged -= vmHandler;
                    webView.Dispose(); // Important for resource release
                };

                // 7. Popup Handling
                webView.CoreWebView2.NewWindowRequested += (s, args) =>
                {
                    args.Handled = true;
                    // Logic to open in new tab?
                    // Accessing the parent VM is hard here. 
                    // For now, navigate in place or try to find parent.
                    webView.CoreWebView2.Navigate(args.Uri);
                };

                // 8. Download Handling
                webView.CoreWebView2.DownloadStarting += (s, args) =>
                {
                    args.Handled = true; // Suppress default dialog
                    
                    // 1. Determine Domain Folder
                    string domain = "Misc";
                    string rawSource = "";

                    try
                    {
                        // Prefer the Page Source (Context of where the user is)
                        rawSource = webView.CoreWebView2.Source;
                        
                        // Fallback: If page source is about:blank or empty, use the DL link host
                        if (string.IsNullOrEmpty(rawSource) || rawSource == "about:blank")
                        {
                            rawSource = args.DownloadOperation.Uri;
                        }

                        if (Uri.TryCreate(rawSource, UriKind.Absolute, out var uri))
                        {
                            domain = uri.Host.ToLower();
                            if (domain.StartsWith("www.")) domain = domain.Substring(4);
                        }
                    }
                    catch { /* Keep default "Misc" */ }

                    // Sanitize Domain for Filesystem
                    try
                    {
                        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                        {
                            domain = domain.Replace(c, '_');
                        }
                        // Explicitly remove common unsafe chars just in case
                        domain = domain.Trim();
                        if (string.IsNullOrEmpty(domain)) domain = "Misc";
                    }
                    catch { domain = "Misc"; }


                    // 2. Prepare Directory
                    var baseDownloads = System.IO.Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), 
                        "Downloads");
                    
                    var targetFolder = System.IO.Path.Combine(baseDownloads, domain);

                    try 
                    {
                        if (!System.IO.Directory.Exists(targetFolder))
                            System.IO.Directory.CreateDirectory(targetFolder);
                    }
                    catch 
                    {
                        // Fallback to root if folder creation fails (e.g. permissions)
                        targetFolder = baseDownloads;
                    }

                    // 3. Prepare Filename
                    string fileName = System.IO.Path.GetFileName(args.ResultFilePath);
                    
                    if (string.IsNullOrWhiteSpace(fileName)) 
                        fileName = $"file_{System.DateTime.Now.Ticks}.tmp";

                    // 4. Unique Filename Logic
                    var fullPath = System.IO.Path.Combine(targetFolder, fileName);
                    
                    try
                    {
                        if (System.IO.File.Exists(fullPath))
                        {
                            string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            string ext = System.IO.Path.GetExtension(fileName);
                            int count = 1;
                            
                            while(System.IO.File.Exists(fullPath) && count < 100)
                            {
                                fullPath = System.IO.Path.Combine(targetFolder, $"{nameWithoutExt} ({count}){ext}");
                                count++;
                            }
                        }
                    }
                    catch 
                    {
                        // Fallback unique name if logic fails
                         fullPath = System.IO.Path.Combine(targetFolder, $"{System.Guid.NewGuid()}_{fileName}");
                    }

                    args.ResultFilePath = fullPath;
                };

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView Init Error: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadExtensions(Microsoft.Web.WebView2.Wpf.WebView2 webView)
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
        }
    }
}

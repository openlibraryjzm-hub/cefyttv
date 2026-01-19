using System.Windows;
using System.Windows.Controls;

namespace ccc.Views
{
    public partial class BrowserView : System.Windows.Controls.UserControl
    {
        public BrowserView()
        {
            InitializeComponent();
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            OpenNewTab("https://www.bing.com");
        }

        public void OpenNewTab(string url)
        {
            var webView = new Microsoft.Web.WebView2.Wpf.WebView2();
            webView.Source = new System.Uri(url);

            InitializeWebView(webView);

            var tab = new TabItem
            {
                Header = "New Tab", 
                Content = webView
            };

            BrowserTabs.Items.Add(tab);
            BrowserTabs.SelectedItem = tab;
        }

        private async void InitializeWebView(Microsoft.Web.WebView2.Wpf.WebView2 webView)
        {
            await webView.EnsureCoreWebView2Async();
            
            // 1. Popup Handling (Keep in Tab)
            webView.CoreWebView2.NewWindowRequested += (s, e) =>
            {
                e.Handled = true; 
                webView.Source = new System.Uri(e.Uri);
            };

            // 2. Download Handling
            webView.CoreWebView2.DownloadStarting += (s, e) =>
            {
                // Auto-Download to Downloads folder without prompt
                e.Handled = true; // Suppress default dialog
                
                var downloadPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                    "Downloads", 
                    System.IO.Path.GetFileName(e.ResultFilePath));

                e.ResultFilePath = downloadPath;
            };

            // 3. Custom Context Menu "Quick Download"
            // Note: This API requires newer WebView2, checking availability strictly isn't always possible in hot-reload, 
            // but standard runtime supports it.
            // 3. Custom Context Menu "Quick Download"
            webView.CoreWebView2.ContextMenuRequested += (s, e) => 
            {
                // Only act if it is a link
                if (!string.IsNullOrEmpty(e.ContextMenuTarget.LinkUri))
                {
                     // Create the item using the Environment from the sender
                     var menuItem = webView.CoreWebView2.Environment.CreateContextMenuItem(
                        "⚡ Quick Download Link", null, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuItemKind.Command);
                
                    menuItem.CustomItemSelected += (send, args) =>
                    {
                        string linkUrl = e.ContextMenuTarget.LinkUri;
                        // Trigger navigation to force download
                        webView.CoreWebView2.ExecuteScriptAsync($"window.location.href = '{linkUrl}'"); 
                    };

                    // Insert at the VERY TOP of the menu
                    e.MenuItems.Insert(0, menuItem);
                }
            };
        }
    }
}

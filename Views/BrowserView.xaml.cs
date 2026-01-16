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
            var browser = new CefSharp.Wpf.ChromiumWebBrowser();
            browser.Address = url;

            browser.LifeSpanHandler = new Handlers.CustomLifeSpanHandler((targetUrl) => 
            {
                OpenNewTab(targetUrl);
            });

            browser.RequestHandler = new Handlers.CustomRequestHandler();

            var tab = new TabItem
            {
                Header = "New Tab", 
                Content = browser
            };

            BrowserTabs.Items.Add(tab);
            BrowserTabs.SelectedItem = tab;
        }
    }
}

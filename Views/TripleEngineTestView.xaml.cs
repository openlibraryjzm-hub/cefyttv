using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ccc.Views
{
    public partial class TripleEngineTestView : System.Windows.Controls.UserControl
    {
        public TripleEngineTestView()
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

        private void Layout_WvFull_Click(object sender, RoutedEventArgs e)
        {
            ResetLayout();
            Col1.Width = new GridLength(0);
        }

        private void Layout_Split_Click(object sender, RoutedEventArgs e)
        {
            ResetLayout();
        }

        private void Layout_WvHalf_Click(object sender, RoutedEventArgs e)
        {
            ResetLayout();
            RightPane.Visibility = Visibility.Collapsed;
        }

        private void Layout_CefFull_Click(object sender, RoutedEventArgs e)
        {
            ResetLayout();
            Col0.Width = new GridLength(0);
            YoutubeView.Visibility = Visibility.Collapsed;
        }

        private void ResetLayout()
        {
            Col0.Width = new GridLength(1, GridUnitType.Star);
            Col1.Width = new GridLength(1, GridUnitType.Star);

            RightPane.Visibility = Visibility.Visible;
            YoutubeView.Visibility = Visibility.Visible;
            MpvView.Visibility = Visibility.Collapsed;
        }

        private void TestMpv_Click(object sender, RoutedEventArgs e)
        {
            if (MpvView.Visibility == Visibility.Visible)
            {
                MpvView.Stop();
                MpvView.Visibility = Visibility.Collapsed;
                YoutubeView.Visibility = Visibility.Visible;
            }
            else
            {
                YoutubeView.Visibility = Visibility.Collapsed;
                MpvView.Visibility = Visibility.Visible;
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
                YoutubeView.Visibility = Visibility.Collapsed;
                MpvView.Visibility = Visibility.Visible;
                MpvView.Play(dialog.FileName);
            }
        }
    }
}

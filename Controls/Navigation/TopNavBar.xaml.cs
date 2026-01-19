using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Navigation
{
    public partial class TopNavBar : UserControl
    {
        public TopNavBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NavigateCommandProperty =
            DependencyProperty.Register("NavigateCommand", typeof(ICommand), typeof(TopNavBar), new PropertyMetadata(null));

        public ICommand NavigateCommand
        {
            get { return (ICommand)GetValue(NavigateCommandProperty); }
            set { SetValue(NavigateCommandProperty, value); }
        }

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(TopNavBar), new PropertyMetadata(null));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(TopNavBar), new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public static readonly DependencyProperty ActivePageProperty =
            DependencyProperty.Register("ActivePage", typeof(string), typeof(TopNavBar), new PropertyMetadata("playlists"));

        public string ActivePage
        {
            get { return (string)GetValue(ActivePageProperty); }
            set { SetValue(ActivePageProperty, value); }
        }
    }
}

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Navigation
{
    public partial class SideScrollButtons : UserControl
    {
        public SideScrollButtons()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ScrollUpCommandProperty =
            DependencyProperty.Register("ScrollUpCommand", typeof(ICommand), typeof(SideScrollButtons), new PropertyMetadata(null));

        public ICommand ScrollUpCommand
        {
            get { return (ICommand)GetValue(ScrollUpCommandProperty); }
            set { SetValue(ScrollUpCommandProperty, value); }
        }

        public static readonly DependencyProperty ScrollDownCommandProperty =
            DependencyProperty.Register("ScrollDownCommand", typeof(ICommand), typeof(SideScrollButtons), new PropertyMetadata(null));

        public ICommand ScrollDownCommand
        {
            get { return (ICommand)GetValue(ScrollDownCommandProperty); }
            set { SetValue(ScrollDownCommandProperty, value); }
        }

        public static readonly DependencyProperty ScrollToActiveCommandProperty =
            DependencyProperty.Register("ScrollToActiveCommand", typeof(ICommand), typeof(SideScrollButtons), new PropertyMetadata(null));

        public ICommand ScrollToActiveCommand
        {
            get { return (ICommand)GetValue(ScrollToActiveCommandProperty); }
            set { SetValue(ScrollToActiveCommandProperty, value); }
        }
    }
}

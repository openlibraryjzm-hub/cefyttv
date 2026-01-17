using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Buttons
{
    public partial class SidebarToggle : UserControl
    {
        public SidebarToggle()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SidebarToggle), new PropertyMetadata(false));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty ToggleCommandProperty =
            DependencyProperty.Register("ToggleCommand", typeof(ICommand), typeof(SidebarToggle), new PropertyMetadata(null));

        public ICommand ToggleCommand
        {
            get { return (ICommand)GetValue(ToggleCommandProperty); }
            set { SetValue(ToggleCommandProperty, value); }
        }
    }
}

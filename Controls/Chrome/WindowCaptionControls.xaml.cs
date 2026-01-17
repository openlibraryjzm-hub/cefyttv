using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Chrome
{
    public partial class WindowCaptionControls : UserControl
    {
        public WindowCaptionControls()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MinimizeCommandProperty =
            DependencyProperty.Register("MinimizeCommand", typeof(ICommand), typeof(WindowCaptionControls), new PropertyMetadata(null));

        public ICommand MinimizeCommand
        {
            get { return (ICommand)GetValue(MinimizeCommandProperty); }
            set { SetValue(MinimizeCommandProperty, value); }
        }

        public static readonly DependencyProperty MaximizeCommandProperty =
            DependencyProperty.Register("MaximizeCommand", typeof(ICommand), typeof(WindowCaptionControls), new PropertyMetadata(null));

        public ICommand MaximizeCommand
        {
            get { return (ICommand)GetValue(MaximizeCommandProperty); }
            set { SetValue(MaximizeCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(WindowCaptionControls), new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Cards
{
    public partial class FolderCard : UserControl
    {
        public FolderCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FolderTitleProperty = DependencyProperty.Register("FolderTitle", typeof(string), typeof(FolderCard));
        public string FolderTitle { get => (string)GetValue(FolderTitleProperty); set => SetValue(FolderTitleProperty, value); }

        public static readonly DependencyProperty VideoCountTextProperty = DependencyProperty.Register("VideoCountText", typeof(string), typeof(FolderCard));
        public string VideoCountText { get => (string)GetValue(VideoCountTextProperty); set => SetValue(VideoCountTextProperty, value); }

        public static readonly DependencyProperty FolderBrushProperty = DependencyProperty.Register("FolderBrush", typeof(System.Windows.Media.Brush), typeof(FolderCard));
        public System.Windows.Media.Brush FolderBrush { get => (System.Windows.Media.Brush)GetValue(FolderBrushProperty); set => SetValue(FolderBrushProperty, value); }
    }
}

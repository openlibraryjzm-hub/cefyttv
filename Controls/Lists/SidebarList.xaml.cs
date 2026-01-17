using System.Collections;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Lists
{
    public partial class SidebarList : UserControl
    {
        public SidebarList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlaylistsProperty =
            DependencyProperty.Register("Playlists", typeof(IEnumerable), typeof(SidebarList), new PropertyMetadata(null));

        public IEnumerable Playlists
        {
            get { return (IEnumerable)GetValue(PlaylistsProperty); }
            set { SetValue(PlaylistsProperty, value); }
        }
    }
}

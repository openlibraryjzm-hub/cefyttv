using System.Collections;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Lists
{
    public partial class StickyCarousel : UserControl
    {
        public StickyCarousel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty VideosProperty =
            DependencyProperty.Register("Videos", typeof(IEnumerable), typeof(StickyCarousel), new PropertyMetadata(null));

        public IEnumerable Videos
        {
            get { return (IEnumerable)GetValue(VideosProperty); }
            set { SetValue(VideosProperty, value); }
        }
    }
}

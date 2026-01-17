using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Visuals
{
    public partial class PageBanner : UserControl
    {
        public PageBanner()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PageBanner));
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(PageBanner));
        public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

        public static readonly DependencyProperty BannerImageProperty = DependencyProperty.Register("BannerImage", typeof(ImageSource), typeof(PageBanner));
        public ImageSource BannerImage { get => (ImageSource)GetValue(BannerImageProperty); set => SetValue(BannerImageProperty, value); }

        public static readonly DependencyProperty AvatarAsciiProperty = DependencyProperty.Register("AvatarAscii", typeof(string), typeof(PageBanner));
        public string AvatarAscii { get => (string)GetValue(AvatarAsciiProperty); set => SetValue(AvatarAsciiProperty, value); }

        public static readonly DependencyProperty VideoCountTextProperty = DependencyProperty.Register("VideoCountText", typeof(string), typeof(PageBanner));
        public string VideoCountText { get => (string)GetValue(VideoCountTextProperty); set => SetValue(VideoCountTextProperty, value); }

        public static readonly DependencyProperty AuthorNameProperty = DependencyProperty.Register("AuthorName", typeof(string), typeof(PageBanner));
        public string AuthorName { get => (string)GetValue(AuthorNameProperty); set => SetValue(AuthorNameProperty, value); }

        public static readonly DependencyProperty ContinueWatchingThumbnailProperty = DependencyProperty.Register("ContinueWatchingThumbnail", typeof(ImageSource), typeof(PageBanner));
        public ImageSource ContinueWatchingThumbnail { get => (ImageSource)GetValue(ContinueWatchingThumbnailProperty); set => SetValue(ContinueWatchingThumbnailProperty, value); }

        public static readonly DependencyProperty HasContinueWatchingProperty = DependencyProperty.Register("HasContinueWatching", typeof(bool), typeof(PageBanner));
        public bool HasContinueWatching { get => (bool)GetValue(HasContinueWatchingProperty); set => SetValue(HasContinueWatchingProperty, value); }

        public static readonly DependencyProperty CanEditProperty = DependencyProperty.Register("CanEdit", typeof(bool), typeof(PageBanner));
        public bool CanEdit { get => (bool)GetValue(CanEditProperty); set => SetValue(CanEditProperty, value); }
    }
}

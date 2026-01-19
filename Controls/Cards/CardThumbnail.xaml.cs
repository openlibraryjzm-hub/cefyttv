using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Cards
{
    public partial class CardThumbnail : UserControl
    {
        public CardThumbnail()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(CardThumbnail), new PropertyMetadata(null));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CardThumbnail), new PropertyMetadata(new CornerRadius(0)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(CardThumbnail), new PropertyMetadata(0.0));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        
        public static readonly DependencyProperty HasProgressProperty =
            DependencyProperty.Register("HasProgress", typeof(bool), typeof(CardThumbnail), new PropertyMetadata(false));

        public bool HasProgress
        {
            get { return (bool)GetValue(HasProgressProperty); }
            set { SetValue(HasProgressProperty, value); }
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(CardThumbnail), new PropertyMetadata(false));

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        public static readonly DependencyProperty IsWatchedProperty =
            DependencyProperty.Register("IsWatched", typeof(bool), typeof(CardThumbnail), new PropertyMetadata(false));

        public bool IsWatched
        {
            get { return (bool)GetValue(IsWatchedProperty); }
            set { SetValue(IsWatchedProperty, value); }
        }
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(CardThumbnail), new PropertyMetadata(Stretch.UniformToFill));

        public Stretch ImageStretch
        {
            get { return (Stretch)GetValue(ImageStretchProperty); }
            set { SetValue(ImageStretchProperty, value); }
        }
    }
}

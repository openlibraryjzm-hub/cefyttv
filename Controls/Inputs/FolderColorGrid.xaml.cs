using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using ccc.Constants;
using UserControl = System.Windows.Controls.UserControl;

namespace ccc.Controls.Inputs
{
    public partial class FolderColorGrid : UserControl
    {
        public ObservableCollection<ColorItem> ColorItems { get; private set; }

        public FolderColorGrid()
        {
            InitializeComponent();
            
            ColorItems = new ObservableCollection<ColorItem>();
            foreach (var key in ThemeConstants.OrderedFolderKeys)
            {
                if (ThemeConstants.FolderColors.TryGetValue(key, out var hex))
                {
                    try 
                    {
                        var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
                        ColorItems.Add(new ColorItem { Name = key, Brush = new SolidColorBrush(color) });
                    }
                    catch { }
                }
            }
            
            ColorItemsControl.ItemsSource = ColorItems;
        }

        // Define Command Dependency Property if needed for binding
    }

    public class ColorItem
    {
        public string Name { get; set; } = string.Empty;
        public SolidColorBrush? Brush { get; set; }
    }
}

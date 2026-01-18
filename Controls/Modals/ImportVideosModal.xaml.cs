using System.Windows.Controls;
using System.Windows.Input;

namespace ccc.Controls.Modals
{
    public partial class ImportVideosModal : System.Windows.Controls.UserControl
    {
        public ImportVideosModal()
        {
            InitializeComponent();
        }

        private void Backdrop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Close logic handled by ViewModel command ideally, but we can trigger it or just let VM handle explicit close button.
            // For now, let's assume clicking outside doesn't close it to prevent accidental data loss, 
            // OR finding the proper command in DataContext if we wanted to support click-outside-to-close.
        }
    }
}

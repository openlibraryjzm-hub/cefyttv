using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ccc.Services;
using ccc.Views;

namespace ccc.ViewModels
{
    public class PlaylistDisplayItem
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "/Resources/Images/placeholder.jpg"; // Path needs to exist or be handled
        public string VideoCountText { get; set; } = "0 Videos";
    }

    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "Playlists";

        [ObservableProperty]
        private object? _currentView;

        // Visibility State
        [ObservableProperty]
        private bool _isBrowserVisible;

        public bool IsLibraryVisible => !IsBrowserVisible;

        // Data for Views
        [ObservableProperty]
        private ObservableCollection<PlaylistDisplayItem> _playlists = new();

        public MainViewModel()
        {
            // Seed Dummy Data for Visual Verification
            PopulateDummyData();

            // Set Initial View
            // CurrentView = new PlaylistsView(); // This would require reference to Views. 
            // Better to genericize or use a NavigationLogic.
            // For this step, we will instantiate it directly if we have the reference, 
            // or let MainWindow set it. 
            // We'll expose the command to set it.
            
            // Actually, we can just new it up here if we add `using ccc.Views;`
            CurrentView = new PlaylistsView();
        }

        private void PopulateDummyData()
        {
            for (int i = 1; i <= 24; i++)
            {
                Playlists.Add(new PlaylistDisplayItem
                {
                    Name = $"Playlist {i}",
                    Description = $"A collection of amazing videos for topic {i}.",
                    VideoCountText = $"{i * 12} Videos",
                    // Use a valid image placeholder or generic web URL if specific images aren't ready
                    ThumbnailUrl = "https://picsum.photos/300/200" 
                });
            }
        }

        [RelayCommand]
        public void Navigate(string destination)
        {
            // Placeholder Navigation Logic
            switch (destination.ToLower())
            {
                case "playlists":
                    CurrentView = new PlaylistsView();
                    PageTitle = "Playlists";
                    break;
                case "videos":
                    // CurrentView = new VideosView();
                    PageTitle = "Videos";
                    break;
                case "settings":
                    // CurrentView = new SettingsView();
                    PageTitle = "Settings";
                    break;
                default:
                    // MessageBox.Show($"Navigating to {destination}");
                    PageTitle = destination;
                    break;
            }
        }

        [RelayCommand]
        public void GoBack()
        {
            // Placeholder
        }

        [RelayCommand]
        public void CloseSidebar()
        {
            // Placeholder
        }

        [RelayCommand]
        public void NavigateToBrowser()
        {
            PageTitle = "Web Browser";
            IsBrowserVisible = true;
            OnPropertyChanged(nameof(IsLibraryVisible));
        }
    }
}

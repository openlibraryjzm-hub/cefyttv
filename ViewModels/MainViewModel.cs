using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ccc.Services;

namespace ccc.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Singleton Access (optional, but convenient for x:Static binding if needed)
        // ideally passed via Constructor Injection in a bigger app.
        // For this refactor, we can genericize it.
        
        [ObservableProperty]
        private string _pageTitle = "Atlas 2.0";

        [ObservableProperty]
        private object? _currentView;

        // View Cache
        private object? _playlistsView;
        private object? _videosView;
        private object? _engineTestView;

        public MainViewModel()
        {
            // Initial View
            NavigateToPlaylists();
        }

        [RelayCommand]
        public void NavigateToPlaylists()
        {
            PageTitle = "Playlists";
            _playlistsView ??= new Views.PlaylistsView();
            CurrentView = _playlistsView;
        }

        [RelayCommand]
        public void NavigateToVideos()
        {
            PageTitle = "Videos";
            _videosView ??= new Views.VideosView();
            CurrentView = _videosView;
        }

        [RelayCommand]
        public void NavigateToEngineTest()
        {
            PageTitle = "Triple Engine Test";
            _engineTestView ??= new Views.TripleEngineTestView();
            CurrentView = _engineTestView;
        }
    }
}

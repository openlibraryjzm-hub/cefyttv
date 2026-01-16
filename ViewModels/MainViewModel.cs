using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ccc.Services;
using ccc.Views;

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

        // Visibility State
        [ObservableProperty]
        private bool _isBrowserVisible;

        public bool IsLibraryVisible => !IsBrowserVisible;

        // View Cache
        private object? _playlistsView;
        private object? _videosView;
        private object? _historyView;
        private object? _pinsView;
        private object? _settingsView;
        private object? _supportView;

        public MainViewModel()
        {
            NavigateToPlaylists();
        }

        private void SwitchToLibrary(string title, ref object? viewCache, Func<object> createView)
        {
            PageTitle = title;
            viewCache ??= createView();
            CurrentView = viewCache;
            IsBrowserVisible = false;
            OnPropertyChanged(nameof(IsLibraryVisible));
        }

        [RelayCommand]
        public void NavigateToPlaylists() => SwitchToLibrary("Playlists", ref _playlistsView, () => new ccc.Views.PlaylistsView());

        [RelayCommand]
        public void NavigateToVideos() => SwitchToLibrary("Videos", ref _videosView, () => new ccc.Views.VideosView());

        [RelayCommand]
        public void NavigateToHistory() => SwitchToLibrary("History", ref _historyView, () => new ccc.Views.HistoryView());

        [RelayCommand]
        public void NavigateToPins() => SwitchToLibrary("Pins", ref _pinsView, () => new ccc.Views.PinsView());

        [RelayCommand]
        public void NavigateToSettings() => SwitchToLibrary("Settings", ref _settingsView, () => new ccc.Views.SettingsView());

        [RelayCommand]
        public void NavigateToSupport() => SwitchToLibrary("Support", ref _supportView, () => new ccc.Views.SupportView());

        [RelayCommand]
        public void NavigateToBrowser()
        {
            PageTitle = "Web Browser";
            IsBrowserVisible = true;
            OnPropertyChanged(nameof(IsLibraryVisible));
        }
    }
}

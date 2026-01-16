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
        public MainViewModel()
        {
            // Revert to Triple Architecture default: Browser on Right, Player on Left.
            NavigateToBrowser();
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

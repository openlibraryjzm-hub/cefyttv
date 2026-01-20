using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace ccc.ViewModels
{
    public partial class BrowserTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "New Tab";

        [ObservableProperty]
        private string _url = "https://www.google.com";

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private bool _canGoBack;

        [ObservableProperty]
        private bool _canGoForward;
        
        [ObservableProperty]
        private bool _isLoading;

        // Command to request close
        public IRelayCommand CloseCommand { get; set; }

        // We can't hold the UI Control here in pure MVVM, but for practical bridging 
        // we might expose an event or just let the View bind to these properties.
        // The View will handle the actual WebView2 control mapping.
    }

    public partial class BrowserViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BrowserTabViewModel> _tabs = new();

        [ObservableProperty]
        private BrowserTabViewModel? _selectedTab;

        [ObservableProperty]
        private string _addressBarText = "";

        [ObservableProperty]
        private bool _isFullScreen;

        [RelayCommand]
        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
        }

        public BrowserViewModel()
        {
            // Initial Tab
            AddNewTab();
        }

        [RelayCommand]
        public void AddNewTab()
        {
            var newTab = new BrowserTabViewModel
            {
                Title = "New Tab",
                Url = "https://www.google.com",
                IsSelected = true
            };
            
            // Setup Close Command
            newTab.CloseCommand = new RelayCommand(() => CloseTab(newTab));

            Tabs.Add(newTab);
            SelectedTab = newTab;
        }

        [RelayCommand]
        public void CloseTab(BrowserTabViewModel tab)
        {
            if (tab == null) return;

            Tabs.Remove(tab);

            if (Tabs.Count == 0)
            {
                // If last tab closed, maybe create a new empty one or leave empty?
                // Most browsers close the app, but here we should probably keep one or create new.
                AddNewTab();
            }
            else if (SelectedTab == tab || SelectedTab == null)
            {
                // Select the last one
                SelectedTab = Tabs.Last();
            }
        }

        [RelayCommand]
        public void SelectTab(BrowserTabViewModel tab)
        {
            if (tab != null)
            {
                SelectedTab = tab;
            }
        }

        partial void OnSelectedTabChanged(BrowserTabViewModel? oldValue, BrowserTabViewModel? newValue)
        {
            if (oldValue != null)
            {
                 oldValue.IsSelected = false;
                 oldValue.PropertyChanged -= SelectedTab_PropertyChanged;
            }
            if (newValue != null) 
            {
                newValue.IsSelected = true;
                AddressBarText = newValue.Url;
                newValue.PropertyChanged += SelectedTab_PropertyChanged;
            }
        }

        private void SelectedTab_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BrowserTabViewModel.Url))
            {
                 if (SelectedTab != null) AddressBarText = SelectedTab.Url;
            }
        }

        [RelayCommand]
        public void Navigate()
        {
            if (SelectedTab == null) return;
            
            var url = AddressBarText;
            if (string.IsNullOrWhiteSpace(url)) return;

            // Simple search detection
            if (!url.StartsWith("http") && !url.Contains("."))
            {
                url = $"https://www.google.com/search?q={System.Web.HttpUtility.UrlEncode(url)}";
            }
            else if (!url.StartsWith("http"))
            {
                url = "https://" + url;
            }

            SelectedTab.Url = url; // View will pick this up and trigger navigation
        }
    }
}

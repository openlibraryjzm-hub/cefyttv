using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ccc.Services;
using ccc.Views;
using System.Threading.Tasks;
using System;

namespace ccc.ViewModels
{
    public class PlaylistDisplayItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string VideoCountText { get; set; } = "0 Videos";
    }

    public class VideoDisplayItem
    {
        public string Title { get; set; } = "";
        public string VideoId { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public int ProgressPercentage { get; set; }
        public bool HasProgress => ProgressPercentage > 0;
        public bool IsPlaying { get; set; }
        public bool IsWatched { get; set; }
    }

    public class HistoryDisplayItem
    {
        public string Title { get; set; } = "";
        public string VideoId { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string RelativeTime { get; set; } = "";
        public int ProgressPercentage { get; set; }
    }

    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _pageTitle = "Playlists";

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private bool _isBrowserVisible;

        public bool IsLibraryVisible => !IsBrowserVisible;

        // Player State
        [ObservableProperty]
        private string? _currentVideoId;

        [ObservableProperty]
        private PlaylistDisplayItem? _selectedPlaylist;

        [ObservableProperty]
        private VideoDisplayItem? _selectedVideo;

        // Tabs Config
        [ObservableProperty]
        private ObservableCollection<ccc.Models.Config.TabDefinition> _activeTabs = new();

        [ObservableProperty]
        private ObservableCollection<ccc.Models.Config.TabPreset> _tabPresets = new();

        [ObservableProperty]
        private ccc.Models.Config.TabPreset? _selectedPreset;

        // Collections
        [ObservableProperty]
        private ObservableCollection<PlaylistDisplayItem> _playlists = new();

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _videos = new(); // Current page videos

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _likedVideos = new(); // Re-use VideoDisplayItem for likes

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _pinnedVideos = new();

        [ObservableProperty]
        private ObservableCollection<HistoryDisplayItem> _historyItems = new();

        private List<VideoDisplayItem> _allVideosCache = new(); // Flattened playlist items for pagination

        public MainViewModel()
        {
            // Load Real Data
            Task.Run(LoadDataAsync);

            CurrentView = new PlaylistsView();
        }

        private async Task LoadDataAsync()
        {
            // 1. Playlists (Real Data)
            try
            {
                var playlists = await App.PlaylistService.GetAllPlaylistsAsync();
                
                System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                {
                    _allPlaylistsCache.Clear();
                    foreach (var p in playlists)
                    {
                        _allPlaylistsCache.Add(new PlaylistDisplayItem
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description ?? "",
                            VideoCountText = $"{p.Items.Count} Videos",
                            ThumbnailUrl = p.CustomThumbnailUrl ?? "https://picsum.photos/300/200" // Fallback
                        });
                    }

                    // Reset Page
                    CurrentPlaylistPage = 1;
                    TotalPlaylistPages = (int)Math.Ceiling(_allPlaylistsCache.Count / (double)ItemsPerPage);
                    if (TotalPlaylistPages < 1) TotalPlaylistPages = 1;
                    
                    UpdateDisplayedPlaylists();


                    // Auto-play Logic: Open the first playlist if available
                    if (playlists.Any())
                    {
                        await OpenPlaylist(playlists.First().Id);
                        
                        // Wait for OpenPlaylist to populate Videos, then play first
                        if (_allVideosCache.Any())
                        {
                            PlayVideo(_allVideosCache[0].VideoId);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlists: {ex.Message}");
            }

            // 2. Dummy Data for other views (until wired up)
            System.Windows.Application.Current.Dispatcher.Invoke(PopulateDummyDetailData);

            await LoadTabsAsync();
            await LoadHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                var history = await App.SqliteService.GetWatchHistoryAsync(100);
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    HistoryItems.Clear();
                    foreach (var h in history)
                    {
                        HistoryItems.Add(new HistoryDisplayItem
                        {
                            Title = h.Title ?? "Unknown Video",
                            VideoId = h.VideoId,
                            ThumbnailUrl = h.ThumbnailUrl ?? "https://picsum.photos/300/169",
                            RelativeTime = GetRelativeTime(h.WatchedAt),
                            ProgressPercentage = 0 // Needs to join with VideoProgress later
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        private string GetRelativeTime(string isoString)
        {
            if (DateTime.TryParse(isoString, out var date))
            {
                var span = DateTime.UtcNow - date;
                if (span.TotalMinutes < 1) return "Just now";
                if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} minutes ago";
                if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
                if (span.TotalDays < 7) return $"{(int)span.TotalDays} days ago";
                return date.ToShortDateString();
            }
            return "";
        }

        private void PopulateDummyDetailData()
        {
             // Videos (General)
            // ... (Detail data loading if needed for other views)
            // For now leaving dummy data population for other views, but Playlists/Videos are real.
            
            // Liked Videos
            for (int i = 1; i <= 15; i++)
            {
                LikedVideos.Add(new VideoDisplayItem
                {
                    Title = $"Liked Video {i}",
                    VideoId = $"LKD-{i:000}",
                    ThumbnailUrl = $"https://picsum.photos/300/169?random={200+i}",
                    ProgressPercentage = 0,
                    IsWatched = true
                });
            }

            // Pinned Videos
            for (int i = 1; i <= 8; i++)
            {
                PinnedVideos.Add(new VideoDisplayItem
                {
                    Title = $"Priority Content {i}",
                    VideoId = $"PIN-{i:000}",
                    ThumbnailUrl = $"https://picsum.photos/300/169?random={300+i}",
                    ProgressPercentage = 10,
                    IsWatched = false
                });
            }

             // History
             // Loaded from DB now
        }


        [ObservableProperty]
        private string? _selectedFolderColor;

        [RelayCommand]
        public async Task FilterByFolder(string color)
        {
            if (SelectedFolderColor == color)
            {
                SelectedFolderColor = null; // Toggle off
            }
            else
            {
                SelectedFolderColor = color;
            }

            if (SelectedPlaylist != null)
            {
                await LoadPlaylistVideos(SelectedPlaylist.Id);
            }
        }

        [RelayCommand]
        public async Task OpenPlaylist(long playlistId)
        {
            // Reset folder filter when opening a new playlist? 
            // Usually yes, or keep it if it's global preference. 
            // For now, let's reset it to avoid confusion.
            SelectedFolderColor = null;

            try
            {
                var playlist = _allPlaylistsCache.FirstOrDefault(p => p.Id == playlistId);
                
                // Fallback fetch if not in cache
                if (playlist == null)
                {
                    var p = App.PlaylistService.GetAllPlaylistsAsync().Result.FirstOrDefault(x => x.Id == playlistId);
                    if (p != null)
                    {
                        playlist = new PlaylistDisplayItem 
                        { 
                            Id = p.Id, 
                            Name = p.Name, 
                            Description = p.Description ?? "", 
                            // VideoCount will be updated after load
                            ThumbnailUrl = p.CustomThumbnailUrl ?? "https://picsum.photos/300/200" 
                        };
                    }
                }

                if (playlist != null)
                {
                    SelectedPlaylist = playlist;
                    PageTitle = playlist.Name;
                }

                await LoadPlaylistVideos(playlistId);

                // 3. Navigate
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                     Navigate("Videos");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening playlist: {ex.Message}");
            }
        }

        private async Task LoadPlaylistVideos(long playlistId)
        {
            try
            {
                // 1. Load data in Service (with Filter)
                await App.PlaylistService.LoadPlaylistAsync(playlistId, SelectedFolderColor);

                // 2. Map to ViewModel Collection
                var items = App.PlaylistService.CurrentPlaylistItems;

                // Switch to UI Thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _allVideosCache.Clear();
                    foreach (var item in items)
                    {
                        _allVideosCache.Add(new VideoDisplayItem
                        {
                            Title = item.Title ?? "Unknown Title",
                            VideoId = item.VideoId,
                            VideoUrl = item.VideoUrl,
                            ThumbnailUrl = item.ThumbnailUrl ?? "/Resources/Images/placeholder.jpg",
                            // Progress/Watched will come from joined data later
                            ProgressPercentage = 0,
                            IsWatched = false
                        });
                    }

                    // Update Video Count text if we loaded the playlist explicitly
                    if (SelectedPlaylist != null)
                    {
                        SelectedPlaylist.VideoCountText = $"{items.Count} Videos";
                    }

                    // Reset Page
                    CurrentVideoPage = 1;
                    TotalVideoPages = (int)Math.Ceiling(_allVideosCache.Count / (double)ItemsPerPage);
                    if (TotalVideoPages < 1) TotalVideoPages = 1;

                    UpdateDisplayedVideos(); // This populates 'Videos'
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlist videos: {ex.Message}");
            }
        }

        [RelayCommand]
        public void PlayVideo(string videoId)
        {
            if (!string.IsNullOrEmpty(videoId))
            {
                CurrentVideoId = videoId;
                // Ideally also update 'IsPlaying' state in the list
                foreach (var v in Videos)
                {
                    v.IsPlaying = (v.VideoId == videoId);
                    if (v.IsPlaying)
                    {
                        SelectedVideo = v;
                    }
                }

                // Record History
                if (SelectedVideo != null)
                {
                    Task.Run(async () => 
                    {
                        await App.SqliteService.AddToWatchHistoryAsync(
                            SelectedVideo.VideoUrl,
                            SelectedVideo.VideoId,
                            SelectedVideo.Title,
                            SelectedVideo.ThumbnailUrl
                        );
                    });
                }
            }
        }

        [ObservableProperty]
        private bool _isFullScreen; // Default false

        // ... existing properties ...

        [RelayCommand]
        public void Navigate(string destination)
        {
            // Always exit fullscreen when navigating explicitly
            IsFullScreen = false;

            PageTitle = destination; // Fallback title
            switch (destination.ToLower())
            {
                case "playlists":
                    if (CurrentView is not PlaylistsView) 
                        CurrentView = new PlaylistsView();
                    PageTitle = "Playlists";
                    break;
                case "videos":
                    if (CurrentView is not VideosView)
                        CurrentView = new VideosView();
                    // PageTitle = "Videos"; // Let caller set specific title if needed
                    break;
                case "history":
                    CurrentView = new HistoryView();
                    PageTitle = "History";
                    // Refresh History Data
                    Task.Run(LoadHistoryAsync);
                    break;
                case "likes":
                    CurrentView = new LikesView();
                    PageTitle = "Liked Videos";
                    Task.Run(LoadLikesAsync);
                    break;
                case "pins":
                    CurrentView = new PinsView();
                    PageTitle = "Pins";
                    break;
                case "settings":
                    CurrentView = new SettingsView();
                    PageTitle = "Settings";
                    break;
                case "support":
                    CurrentView = new SupportView();
                    PageTitle = "Support";
                    break;
                case "browser":
                    CurrentView = new BrowserView();
                    PageTitle = "Web Browser";
                    IsBrowserVisible = true;
                    OnPropertyChanged(nameof(IsLibraryVisible));
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        public void GoBack() 
        {
            // Simple back logic for now - just go to playlists if nowhere else
            Navigate("playlists");
        }

        [RelayCommand]
        public void CloseSidebar() 
        {
            // Enter Full Screen Mode
            IsFullScreen = true;
        }

        [RelayCommand]
        public void NavigateToBrowser()
        {
            PageTitle = "Web Browser";
            IsBrowserVisible = true;
            IsFullScreen = false; // Ensure UI is normal
            OnPropertyChanged(nameof(IsLibraryVisible));
        }

        // Pagination State
        private const int ItemsPerPage = 50;
        private List<PlaylistDisplayItem> _allPlaylistsCache = new();

        [ObservableProperty]
        private int _currentPlaylistPage = 1;

        [ObservableProperty]
        private int _totalPlaylistPages = 1;
        
        [ObservableProperty]
        private string _playlistPageText = "Page 1 of 1";

        [ObservableProperty]
        private int _currentVideoPage = 1;

        [ObservableProperty]
        private int _totalVideoPages = 1;

        [ObservableProperty]
        private string _videoPageText = "Page 1 of 1";


        [RelayCommand]
        public void NextPlaylistPage()
        {
            if (CurrentPlaylistPage < TotalPlaylistPages)
            {
                CurrentPlaylistPage++;
                UpdateDisplayedPlaylists();
            }
        }

        [RelayCommand]
        public void PrevPlaylistPage()
        {
            if (CurrentPlaylistPage > 1)
            {
                CurrentPlaylistPage--;
                UpdateDisplayedPlaylists();
            }
        }

        [RelayCommand]
        public void NextVideoPage()
        {
            if (CurrentVideoPage < TotalVideoPages)
            {
                CurrentVideoPage++;
                UpdateDisplayedVideos();
            }
        }

        [RelayCommand]
        public void PrevVideoPage()
        {
            if (CurrentVideoPage > 1)
            {
                CurrentVideoPage--;
                UpdateDisplayedVideos();
            }
        }

        private void UpdateDisplayedPlaylists()
        {
            var pagedItems = _allPlaylistsCache
                .Skip((CurrentPlaylistPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Playlists.Clear();
                foreach (var item in pagedItems) Playlists.Add(item);
                PlaylistPageText = $"Page {CurrentPlaylistPage} of {TotalPlaylistPages}";
            });
        }

        private void UpdateDisplayedVideos()
        {
            var pagedItems = _allVideosCache
                .Skip((CurrentVideoPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Videos.Clear();
                foreach (var item in pagedItems) Videos.Add(item);
                VideoPageText = $"Page {CurrentVideoPage} of {TotalVideoPages}";
            });
        }

        [RelayCommand]
        public async Task NextPlaylist()
        {
            if (SelectedPlaylist == null || !_allPlaylistsCache.Any()) return;

            var currentIndex = _allPlaylistsCache.FindIndex(p => p.Id == SelectedPlaylist.Id);
            if (currentIndex == -1) return;

            var nextIndex = (currentIndex + 1) % _allPlaylistsCache.Count; // Cycle
            
            await OpenPlaylist(_allPlaylistsCache[nextIndex].Id);
            
            // Auto-play
            if (_allVideosCache.Any())
            {
                PlayVideo(_allVideosCache[0].VideoId);
            }
        }

        [RelayCommand]
        public async Task PrevPlaylist()
        {
            if (SelectedPlaylist == null || !_allPlaylistsCache.Any()) return;

            var currentIndex = _allPlaylistsCache.FindIndex(p => p.Id == SelectedPlaylist.Id);
            if (currentIndex == -1) return;

            var prevIndex = (currentIndex - 1 + _allPlaylistsCache.Count) % _allPlaylistsCache.Count; // Cycle
            
            await OpenPlaylist(_allPlaylistsCache[prevIndex].Id);

            // Auto-play
            if (_allVideosCache.Any())
            {
                PlayVideo(_allVideosCache[0].VideoId);
            }
        }

        [RelayCommand]
        public void NextVideo()
        {
            var next = App.PlaylistService.NextVideo();
            if (next != null)
            {
                PlayVideo(next.VideoId);
            }
        }

        [RelayCommand]
        public void PrevVideo()
        {
            var prev = App.PlaylistService.PreviousVideo();
            if (prev != null)
            {
                PlayVideo(prev.VideoId);
            }
        }
        [RelayCommand]
        public void ChangeTabPreset(ccc.Models.Config.TabPreset preset)
        {
            if (preset == null) return;
            
            SelectedPreset = preset;
            App.TabService.SaveConfigAsync(new ccc.Models.Config.TabConfig
            {
                ActivePresetId = preset.Id,
                // Preserve other data manually since we don't have full state in one object here (simplified)
                // In a real app we'd keep the whole Config object alive
            });

            // Update displayed tabs
            ActiveTabs.Clear();
            var tabs = App.TabService.GetTabsForPreset(preset.Id);
            foreach (var t in tabs) ActiveTabs.Add(t);
        }

        [RelayCommand]
        public void SelectTab(string tabId)
        {
             //Logic to filter playlists by tabId would go here
             //For now, just visual selection
        }

        [RelayCommand]
        public async Task ToggleLike()
        {
            if (SelectedVideo == null) return;

            await App.SqliteService.ToggleLikeAsync(
                SelectedVideo.VideoId,
                SelectedVideo.VideoUrl,
                SelectedVideo.Title,
                SelectedVideo.ThumbnailUrl
            );
            
            // If we are on the likes page, refresh immediately
            if (CurrentView is Views.LikesView)
            {
                await LoadLikesAsync();
            }
        }

        private async Task LoadLikesAsync()
        {
            try
            {
                var likes = await App.SqliteService.GetLikedVideosAsync(1000);
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    LikedVideos.Clear();
                    foreach (var l in likes)
                    {
                        LikedVideos.Add(new VideoDisplayItem
                        {
                            Title = l.Title ?? "Unknown Video",
                            VideoId = l.VideoId,
                            VideoUrl = l.VideoUrl,
                            ThumbnailUrl = l.ThumbnailUrl ?? "/Resources/Images/placeholder.jpg",
                            ProgressPercentage = 0,
                            IsWatched = false
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading likes: {ex.Message}");
            }
        }

        private async Task LoadTabsAsync()
        {
             var config = await App.TabService.LoadConfigAsync();
             
             // Load Presets
             TabPresets.Clear();
             foreach(var p in config.Presets) TabPresets.Add(p);

             // Load Active Preset
             SelectedPreset = TabPresets.FirstOrDefault(p => p.Id == config.ActivePresetId) ?? TabPresets.FirstOrDefault();

             // Load Tabs
             if (SelectedPreset != null)
             {
                 ChangeTabPreset(SelectedPreset);
             }
        }
    }
}

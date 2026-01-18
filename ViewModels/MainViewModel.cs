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

        public string Author { get; set; } = "";
        public string PublishYear { get; set; } = "";
        public string ViewCount { get; set; } = "";
        public string MetadataLine { get; set; } = "";
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
        
        // Modal State
        [ObservableProperty]
        private bool _isImportModalOpen;

        [ObservableProperty]
        private string _importUrlInput = "";

        // User selected playlist in Modal
        [ObservableProperty]
        private PlaylistDisplayItem? _importTargetPlaylist;

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
             // Loaded from DB now

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
                            IsWatched = false,
                            Author = item.Author ?? "Unknown",
                            ViewCount = FormatViewCount(item.ViewCount),
                            PublishYear = !string.IsNullOrEmpty(item.PublishedAt) && DateTime.TryParse(item.PublishedAt, out var d) ? d.Year.ToString() : "",
                            MetadataLine = $"{FormatViewCount(item.ViewCount)} | {(!string.IsNullOrEmpty(item.PublishedAt) && DateTime.TryParse(item.PublishedAt, out var d2) ? d2.Year.ToString() : "")}"
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
                    Task.Run(LoadPinsAsync);
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
        public void ShuffleVideo()
        {
            if (!_allVideosCache.Any()) return;

            var random = new Random();
            var index = random.Next(_allVideosCache.Count);
            var video = _allVideosCache[index];
            
            PlayVideo(video.VideoId);
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

        [RelayCommand]
        public async Task TogglePin(VideoDisplayItem? video = null)
        {
            var target = video ?? SelectedVideo;
            if (target == null) return;

            await App.SqliteService.TogglePinAsync(
                target.VideoId,
                target.VideoUrl,
                target.Title,
                target.ThumbnailUrl
            );
            
            // If we are on the pins page, refresh immediately
            if (CurrentView is Views.PinsView)
            {
                await LoadPinsAsync();
            }
        }

        private async Task LoadPinsAsync()
        {
            try
            {
                var pins = await App.SqliteService.GetPinnedVideosAsync(1000);
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    PinnedVideos.Clear();
                    foreach (var p in pins)
                    {
                        PinnedVideos.Add(new VideoDisplayItem
                        {
                            Title = p.Title ?? "Unknown Video",
                            VideoId = p.VideoId,
                            VideoUrl = p.VideoUrl,
                            ThumbnailUrl = p.ThumbnailUrl ?? "/Resources/Images/placeholder.jpg",
                            ProgressPercentage = 0,
                            IsWatched = false
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading pins: {ex.Message}");
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
        private string FormatViewCount(string? rawCount)
        {
            if (string.IsNullOrEmpty(rawCount)) return "-";

            // Try to parse raw string (remove any existing non-digits if needed, though usually just digits)
            // Assuming database has raw numbers like "123456" OR already formatted strings?
            // "just replace that with actual data from @[playlists.db]", probably implies raw data needs formatting.
            if (long.TryParse(rawCount, out var count))
            {
                if (count >= 1_000_000_000) return $"{(count / 1_000_000_000.0):0.#}B";
                if (count >= 1_000_000) return $"{(count / 1_000_000)}M";
                if (count >= 1_000) return $"{(count / 1_000)}K";
                return count.ToString();
            }
            return rawCount; // Fallback if it's already text
        }

        public class ColorImportItem
        {
            public string ColorName { get; set; } = "";
            public string BrushKey { get; set; } = "";
            public string InputText { get; set; } = ""; // Bindable? No, needs INPC if we want updates.
            // Actually, we can just use a simple class but the UI needs to update the property. 
            // So let's make it a tiny ObservableObject or just manual binding.
            // Simpler: Just a class properties, if it's in an ObservableCollection the UI binds to properties. 
            // The TextBox updates the property. Standard binding works on POCO properties too.
        }

        [ObservableProperty]
        private ObservableCollection<ColorImportItem> _colorImports = new();

        [RelayCommand]
        public void OpenImportModal()
        {
            // Reset state
            IsImportModalOpen = true;
            ImportUrlInput = ""; 
            IsNewPlaylistMode = false;
            NewPlaylistName = "";

            // Initialize Color Imports if empty
            if (!ColorImports.Any())
            {
                var colors = new[] { "Red", "Orange", "Amber", "Yellow", "Lime", "Green", "Emerald", "Teal", "Cyan", "Sky", "Blue", "Indigo", "Violet", "Purple", "Fuchsia", "Pink" };
                foreach (var c in colors)
                {
                    ColorImports.Add(new ColorImportItem { ColorName = c, BrushKey = $"Folder{c}Brush", InputText = "" });
                }
            }
            else
            {
                // Clear inputs
                foreach(var c in ColorImports) c.InputText = "";
            }

            // Default Selection Logic
            ImportTargetPlaylist = null; // Clear previous
            
            // 1. If viewing a specific playlist (Videos View), select it
            if (CurrentView is VideosView && SelectedPlaylist != null)
            {
                 ImportTargetPlaylist = Playlists.FirstOrDefault(p => p.Id == SelectedPlaylist.Id) 
                                        ?? _allPlaylistsCache.FirstOrDefault(p => p.Id == SelectedPlaylist.Id);
            }
            // 2. If viewing Playlists View, select "Unsorted" (ID 1)
            else if (CurrentView is PlaylistsView)
            {
                // Assuming "Unsorted" is usually the first one or ID 1. 
                // Logic based on USER request: "if playlists page ... automatic option will be for the already existing 'unsorted' playlist"
                // Finding playlist named "Unsorted"
                ImportTargetPlaylist = Playlists.FirstOrDefault(p => p.Name.ToLower() == "unsorted") 
                                       ?? _allPlaylistsCache.FirstOrDefault(p => p.Name.ToLower() == "unsorted")
                                       ?? Playlists.FirstOrDefault(); // Fallback
            }
            
            // Fallback if still null
            if (ImportTargetPlaylist == null && Playlists.Any())
            {
                ImportTargetPlaylist = Playlists.First();
            }
        }

        [ObservableProperty]
        private bool _isNewPlaylistMode;

        [ObservableProperty]
        private string _newPlaylistName = "";

        [RelayCommand]
        public void ToggleNewPlaylistMode()
        {
            IsNewPlaylistMode = !IsNewPlaylistMode;
        }

        [RelayCommand]
        public void CloseImportModal()
        {
            IsImportModalOpen = false;
        }

        [RelayCommand]
        public async Task ImportVideos()
        {
            var hasMainInput = !string.IsNullOrWhiteSpace(ImportUrlInput);
            var hasColorInput = ColorImports.Any(c => !string.IsNullOrWhiteSpace(c.InputText));
            var isCreating = IsNewPlaylistMode && !string.IsNullOrWhiteSpace(NewPlaylistName);

            if ((!IsNewPlaylistMode && ImportTargetPlaylist == null) || (!hasMainInput && !hasColorInput)) return;

            IsImportModalOpen = false; 

            long targetId = 0;
            if (IsNewPlaylistMode)
            {
                 // Create new playlist
                 targetId = await App.PlaylistService.CreatePlaylistAsync(NewPlaylistName, "Created via Import");
                 // Refresh Playlists Cache
                 var newPlist = new PlaylistDisplayItem { Id = targetId, Name = NewPlaylistName, VideoCountText = "0 Videos", ThumbnailUrl = "https://picsum.photos/300/200" };
                 _allPlaylistsCache.Add(newPlist);
                 Playlists.Add(newPlist);
                 SelectedPlaylist = newPlist;
                 ImportTargetPlaylist = newPlist;
            }
            else
            {
                targetId = ImportTargetPlaylist.Id;
            }

            var input = ImportUrlInput;

            await Task.Run(async () =>
            {
                var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    // Check for Playlist Link first
                    var playlistId = ExtractPlaylistId(line);
                    if (!string.IsNullOrEmpty(playlistId))
                    {
                        try
                        {
                            var videos = await ccc.Services.YoutubeApiService.Instance.GetPlaylistVideos(playlistId);
                            foreach (var v in videos)
                            {
                                await App.PlaylistService.AddVideoToPlaylistAsync(
                                    targetId,
                                    $"https://www.youtube.com/watch?v={v.VideoId}",
                                    v.VideoId,
                                    v.Title,
                                    v.ThumbnailUrl,
                                    v.Author,
                                    v.ViewCount,
                                    v.PublishedAt
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to import playlist {playlistId}: {ex.Message}");
                        }
                        continue; // Done with this line
                    }

                    // Fallback to Video Link
                    var videoId = ExtractVideoId(line);
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        try 
                        {
                            var details = await ccc.Services.YoutubeApiService.Instance.GetVideoDetails(videoId);
                            await App.PlaylistService.AddVideoToPlaylistAsync(
                                targetId,
                                $"https://www.youtube.com/watch?v={videoId}",
                                videoId,
                                details.Title,
                                details.ThumbnailUrl,
                                details.Author,
                                details.ViewCount,
                                details.PublishedAt
                            );
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to import {videoId}: {ex.Message}");
                        }
                    }
                }
                // 2. Process Color Folder Inputs
                foreach (var colorItem in ColorImports)
                {
                    if (string.IsNullOrWhiteSpace(colorItem.InputText)) continue;

                    var colorInput = colorItem.InputText;
                    var colorName = colorItem.ColorName.ToLower(); // "red", "blue" etc to match db logic

                    var cLines = colorInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in cLines)
                    {
                         // Check for Playlist Link first
                        var playlistId = ExtractPlaylistId(line);
                        if (!string.IsNullOrEmpty(playlistId))
                        {
                            try
                            {
                                var videos = await ccc.Services.YoutubeApiService.Instance.GetPlaylistVideos(playlistId);
                                foreach (var v in videos)
                                {
                                    long itemId = await App.PlaylistService.AddVideoToPlaylistAsync(
                                        targetId,
                                        $"https://www.youtube.com/watch?v={v.VideoId}",
                                        v.VideoId,
                                        v.Title,
                                        v.ThumbnailUrl,
                                        v.Author,
                                        v.ViewCount,
                                        v.PublishedAt
                                    );
                                    // Assign to Folder
                                    await App.SqliteService.AssignVideoToFolderAsync(targetId, itemId, colorName);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to import color playlist {playlistId}: {ex.Message}");
                            }
                            continue;
                        }

                        // Fallback to Video Link
                        var videoId = ExtractVideoId(line);
                        if (!string.IsNullOrEmpty(videoId))
                        {
                            try 
                            {
                                var details = await ccc.Services.YoutubeApiService.Instance.GetVideoDetails(videoId);
                                var itemId = await App.PlaylistService.AddVideoToPlaylistAsync(
                                    targetId,
                                    $"https://www.youtube.com/watch?v={videoId}",
                                    videoId,
                                    details.Title,
                                    details.ThumbnailUrl,
                                    details.Author,
                                    details.ViewCount,
                                    details.PublishedAt
                                );
                                // Assign to Folder
                                await App.SqliteService.AssignVideoToFolderAsync(targetId, itemId, colorName);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to import color video {videoId}: {ex.Message}");
                            }
                        }
                    }
                }
            });

            // Refresh UI if we added to the currently viewed playlist
            if (SelectedPlaylist != null && SelectedPlaylist.Id == targetId)
            {
                 // Reload to refresh defaults and colors
                 await LoadPlaylistVideos(targetId);
            }
        }

        private string ExtractVideoId(string url)
        {
            // Simple robust regex for standard, short, and raw IDs
            // https://www.youtube.com/watch?v=VIDEO_ID
            // https://youtu.be/VIDEO_ID
            // VIDEO_ID
            var regex = new System.Text.RegularExpressions.Regex(@"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})");
            var match = regex.Match(url);
            if (match.Success) return match.Groups[1].Value;
            
            if (url.Trim().Length == 11) return url.Trim(); // Assume raw ID

            return "";
        }

        private string ExtractPlaylistId(string url)
        {
            // https://www.youtube.com/playlist?list=PLAYLIST_ID
            // https://www.youtube.com/watch?v=VIDEO_ID&list=PLAYLIST_ID
            var regex = new System.Text.RegularExpressions.Regex(@"[?&]list=([^#&?]+)");
            var match = regex.Match(url);
            if (match.Success) return match.Groups[1].Value;
            
            // Raw ID heuristics? usually starts with PL, UU, LL, RD and ~34 chars
            // Check if it's just a long string not starting with http
            var trimmed = url.Trim();
            if (!trimmed.StartsWith("http") && trimmed.Length > 12 && (trimmed.StartsWith("PL") || trimmed.StartsWith("UU") || trimmed.StartsWith("LL") || trimmed.StartsWith("RD"))) 
                return trimmed;

            return "";
        }

    }
}

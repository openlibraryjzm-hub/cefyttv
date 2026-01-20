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

    public class VideoDisplayItem : ObservableObject
    {
        public string Title { get; set; } = "";
        public string VideoId { get; set; } = "";
        public long ItemId { get; set; } // Database ID (PlaylistItem.Id)
        public string VideoUrl { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public int ProgressPercentage { get; set; }
        public bool HasProgress => ProgressPercentage > 0;

        private bool _isPlaying;
        public bool IsPlaying 
        { 
            get => _isPlaying; 
            set => SetProperty(ref _isPlaying, value); 
        }

        public bool IsWatched { get; set; }

        public string Author { get; set; } = "";
        public string PublishYear { get; set; } = "";
        public string ViewCount { get; set; } = "";
        public string MetadataLine { get; set; } = "";
        
        private bool _isQuickMenuOpen; // For Right-Click Star menu
        public bool IsQuickMenuOpen
        {
            get => _isQuickMenuOpen;
            set => SetProperty(ref _isQuickMenuOpen, value);
        }

        // Folder Assignments
        public ObservableCollection<string> FolderColors { get; set; } = new ObservableCollection<string>();

        // Indexer for Binding: IsChecked="{Binding [red]}"
        [System.Runtime.CompilerServices.IndexerName("Item")]
        public bool this[string color]
        {
            get => FolderColors.Contains(color);
            set
            {
                if (value)
                {
                    if (!FolderColors.Contains(color)) FolderColors.Add(color);
                }
                else
                {
                    if (FolderColors.Contains(color)) FolderColors.Remove(color);
                }
                OnPropertyChanged("Item[]");
            }
        }
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
        [NotifyPropertyChangedFor(nameof(IsLibraryVisible))]
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
        private string _defaultStarColor = "red";

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

        [ObservableProperty]
        private string _activeNavTab = "playlists";

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _orbImagePath;

        [ObservableProperty]
        private double _orbScale = 1.0;
        
        partial void OnOrbScaleChanged(double value) => App.ConfigService.OrbScale = value;

        [ObservableProperty]
        private double _orbOffsetX = 0;

        partial void OnOrbOffsetXChanged(double value) => App.ConfigService.OrbOffsetX = value;

        [ObservableProperty]
        private double _orbOffsetY = 0;

        partial void OnOrbOffsetYChanged(double value) => App.ConfigService.OrbOffsetY = value;

        [ObservableProperty]
        private bool _isSpillEnabled;
        partial void OnIsSpillEnabledChanged(bool value) => App.ConfigService.IsSpillEnabled = value;

        [ObservableProperty]
        private bool _spillTopLeft;
        partial void OnSpillTopLeftChanged(bool value) => App.ConfigService.SpillTopLeft = value;

        [ObservableProperty]
        private bool _spillTopRight;
        partial void OnSpillTopRightChanged(bool value) => App.ConfigService.SpillTopRight = value;

        [ObservableProperty]
        private bool _spillBottomLeft;
        partial void OnSpillBottomLeftChanged(bool value) => App.ConfigService.SpillBottomLeft = value;

        [ObservableProperty]
        private bool _spillBottomRight;
        partial void OnSpillBottomRightChanged(bool value) => App.ConfigService.SpillBottomRight = value;

         public ObservableCollection<int> Skeletons { get; } = new ObservableCollection<int>(System.Linq.Enumerable.Range(0, 15));

        private List<VideoDisplayItem> _allVideosCache = new(); // Flattened playlist items for pagination

        public BrowserViewModel BrowserVm { get; } = new BrowserViewModel();

        public MainViewModel()
        {
            // Init Defaults
            _defaultStarColor = App.ConfigService.DefaultAssignColor ?? "red";
            _orbImagePath = App.ConfigService.CustomOrbImage ?? "pack://siteoforigin:,,,/assets/orb.png";
            
            // Orb Config
            _orbScale = App.ConfigService.OrbScale;
            _orbOffsetX = App.ConfigService.OrbOffsetX;
            _orbOffsetY = App.ConfigService.OrbOffsetY;
            _isSpillEnabled = App.ConfigService.IsSpillEnabled;
            _spillTopLeft = App.ConfigService.SpillTopLeft;
            _spillTopRight = App.ConfigService.SpillTopRight;
            _spillBottomLeft = App.ConfigService.SpillBottomLeft;
            _spillBottomRight = App.ConfigService.SpillBottomRight;

            // Load Real Data
            Task.Run(LoadDataAsync);

            CurrentView = new PlaylistsView();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // 1. Playlists (Real Data)
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

                    // Auto-play Logic: Prefer Last Watched, then First Playlist
                    var lastWatched = await App.SqliteService.GetWatchHistoryAsync(1);
                    if (lastWatched.Any())
                    {
                        var last = lastWatched.First();
                        // Open the playlist containing this video? Or just play it?
                        // For now, let's try to OpenPlaylist for its playlist, then PlayVideo.
                        var playlistId = await App.SqliteService.GetPlaylistIdByVideoIdAsync(last.VideoId);
                        if (playlistId != null && playlistId > 0)
                        {
                            await OpenPlaylist(playlistId.Value);
                            await PlayVideo(last.VideoId);
                        }
                        else
                        {
                            // If orphaned, fallback
                             if (playlists.Any()) await OpenPlaylist(playlists.First().Id);
                        }
                    }
                    else if (playlists.Any())
                    {
                        await OpenPlaylist(playlists.First().Id);
                    }
                });

                // 2. Dummy Data for other views (until wired up)
                System.Windows.Application.Current.Dispatcher.Invoke(PopulateDummyDetailData);

                await LoadTabsAsync();
                // await LoadHistoryAsync(); // Moving out of Try block if needed, or keep inside
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Critical Error Loading Data: {ex.Message}\n\nStack: {ex.StackTrace}", "Startup Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            
            // Load History safely outside or keep inside? 
            // Better to keep independent.
            try 
            {
                 await LoadHistoryAsync();
            } 
            catch { /* Ignore history load fail */ }
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

        // --- Sorting ---
        public static PlaylistService.SortMode[] SortModes => (PlaylistService.SortMode[])Enum.GetValues(typeof(PlaylistService.SortMode));

        [ObservableProperty]
        private PlaylistService.SortMode _currentSortMode = PlaylistService.SortMode.Default;

        partial void OnCurrentSortModeChanged(PlaylistService.SortMode value)
        {
             if (SelectedPlaylist != null)
             {
                 // Trigger reload when selection changes
                 Task.Run(() => LoadPlaylistVideos(SelectedPlaylist.Id));
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

                // 2. Navigate Immediately (Skeleton Mode)
                IsLoading = true;
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                     Navigate("Videos");
                });

                // 3. Load Data
                await LoadPlaylistVideos(playlistId);
                
                // Done
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine($"Error opening playlist: {ex.Message}");
            }
        }

        private async Task LoadPlaylistVideos(long playlistId)
        {
            try
            {
                // 1. Load data in Service (with Filter and Sort)
                await App.PlaylistService.LoadPlaylistAsync(playlistId, SelectedFolderColor, CurrentSortMode);

                // 2. Map to ViewModel Collection
                var items = App.PlaylistService.CurrentPlaylistItems;

                // Switch to UI Thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _allVideosCache.Clear();
                    foreach (var item in items)
                    {
                        var videoItem = new VideoDisplayItem
                        {
                            Title = item.Title ?? "Unknown Title",
                            VideoId = item.VideoId,
                            ItemId = item.Id,
                            VideoUrl = item.VideoUrl,
                            ThumbnailUrl = item.ThumbnailUrl ?? "/Resources/Images/placeholder.jpg",
                            // Progress/Watched will come from joined data later
                            ProgressPercentage = 0,
                            IsWatched = false,
                            Author = item.Author ?? "Unknown",
                            ViewCount = FormatViewCount(item.ViewCount),
                            PublishYear = !string.IsNullOrEmpty(item.PublishedAt) && DateTime.TryParse(item.PublishedAt, out var d) ? d.Year.ToString() : "",
                            MetadataLine = $"{FormatViewCount(item.ViewCount)} | {(!string.IsNullOrEmpty(item.PublishedAt) && DateTime.TryParse(item.PublishedAt, out var d2) ? d2.Year.ToString() : "")}"
                        };

                        // Map Folder Colors
                        if (item.FolderAssignments != null)
                        {
                            foreach(var fa in item.FolderAssignments)
                            {
                                videoItem.FolderColors.Add(fa.FolderColor);
                            }
                        }

                        _allVideosCache.Add(videoItem);
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

        [ObservableProperty]
        private double _currentVideoStartTime;

        [RelayCommand]
        public async Task PlayVideo(string videoId)
        {
            if (string.IsNullOrEmpty(videoId)) return;

            // Fetch progress and set BEFORE setting VideoId (which triggers load)
            var progress = await App.SqliteService.GetVideoProgressAsync(videoId);
            CurrentVideoStartTime = progress?.LastProgress ?? 0;

            CurrentVideoId = videoId;
            
            // Check if video is in current list
            bool foundInCurrent = false;
            foreach (var v in Videos)
            {
                v.IsPlaying = (v.VideoId == videoId);
                if (v.IsPlaying)
                {
                    SelectedVideo = v;
                    foundInCurrent = true;
                    // Persist state
                    Task.Run(async () => await App.PlaylistService.UpdateLastWatched(videoId));
                }
            }

            // If not found in current context (e.g. played from History/Likes), 
            // we should try to sync the context to the video's playlist 
            // so nav menus and Next/Prev buttons work.
            if (!foundInCurrent)
            {
                Task.Run(async () =>
                {
                    // 1. Find Playlist ID
                    var playlistId = await App.SqliteService.GetPlaylistIdByVideoIdAsync(videoId);
                    
                    if (playlistId.HasValue)
                    {
                        // 2. Load Playlist Data (Background)
                        // Note: We don't want to fully navigate/render the VideosView if user is in "History" view?
                        // User Request: "top playlist and top video menus ... instead staying stuck"
                        // This implies updating the "SelectedPlaylist" and "PlaylistService" state is desired.
                        
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            // Update SelectedPlaylist Metadata
                            var p = _allPlaylistsCache.FirstOrDefault(x => x.Id == playlistId.Value);
                            if (p != null) SelectedPlaylist = p;
                            else 
                            {
                                // Fallback fetch
                                var dbP = await App.SqliteService.GetPlaylistAsync(playlistId.Value);
                                if (dbP != null)
                                {
                                    SelectedPlaylist = new PlaylistDisplayItem 
                                    { 
                                        Id = dbP.Id, Name = dbP.Name, Description = dbP.Description, 
                                        VideoCountText = $"{dbP.Items.Count} Videos",
                                        ThumbnailUrl = dbP.CustomThumbnailUrl ?? "https://picsum.photos/300/200"
                                    };
                                }
                            }
                            
                            // Load Service State (Critical for Next/Prev)
                            await App.PlaylistService.LoadPlaylistAsync(playlistId.Value, SelectedFolderColor);
                            // Persist this video as the current one for this playlist
                            await App.PlaylistService.UpdateLastWatched(videoId);
                            
                            // Manually find the video object in the *service* items to update SelectedVideo
                            var vidItem = App.PlaylistService.CurrentPlaylistItems.FirstOrDefault(x => x.VideoId == videoId);
                            if (vidItem != null)
                            {
                                // Create a display item for binding
                                var vDisplay = new VideoDisplayItem 
                                {
                                     Title = vidItem.Title, VideoId = vidItem.VideoId, ItemId = vidItem.Id,
                                     VideoUrl = vidItem.VideoUrl, ThumbnailUrl = vidItem.ThumbnailUrl,
                                     Author = vidItem.Author, ViewCount = FormatViewCount(vidItem.ViewCount), IsPlaying = true
                                     // Folder colors etc mapping if needed
                                };
                                SelectedVideo = vDisplay;
                            }
                            
                            // Force update logic? 
                            // If we switch context, we might confuse the user if they hit "Back".
                            // But for "Top Menu" sync, this is necessary.
                        });
                    }
                });
            }

            // Record History
            if (SelectedVideo != null) // Note: might be null initially if async load hasn't finished, handled safely inside AddToWatchHistoryAsync usually? No, let's wait or allow fire/forget
            {
                // We might capture an older SelectedVideo here if the async update hasn't happened.
                // Ideally passing videoId to AddToWatchHistoryAsync is safer if we allow partial data.
                // Or just use what we have. 
                 Task.Run(async () => 
                 {
                     // Retry fetching SelectedVideo if null/mismatched? 
                     // For now, assume if foundInCurrent it's fine. If not, the async PlayVideo handles context sync.
                     // IMPORTANT: History needs url/title/thumb. If not found in current, we might send nulls.
                     // Fallback: fetch from DB if needed inside service.
                     
                     // If we are strictly playing, SelectedVideo might be updated slightly later.
                     // Let's rely on the previous logic for history or defer it.
                     
                     if (SelectedVideo != null && SelectedVideo.VideoId == videoId)
                     {
                        await App.SqliteService.AddToWatchHistoryAsync(
                            SelectedVideo.VideoUrl,
                            SelectedVideo.VideoId,
                            SelectedVideo.Title,
                            SelectedVideo.ThumbnailUrl
                        );
                     }
                 });
            }
        }



        [RelayCommand]
        public void SetDefaultColor(string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                DefaultStarColor = color;
                App.ConfigService.DefaultAssignColor = color;
                
                // If in Quick Menu mode, close it
                if (_currentQuickMenuVideo != null)
                {
                    _currentQuickMenuVideo.IsQuickMenuOpen = false;
                    _currentQuickMenuVideo = null;
                    OnPropertyChanged(nameof(IsTaggingActive));
                }
            }
        }

        [RelayCommand]
        public async Task CancelBulkTags()
        {
            // Exit Modes
            IsBulkTagMode = false;
            if (_currentQuickMenuVideo != null)
            {
                _currentQuickMenuVideo.IsQuickMenuOpen = false;
                _currentQuickMenuVideo = null;
            }
            OnPropertyChanged(nameof(IsTaggingActive));
            
            // Revert Changes by reloading data
            if (SelectedPlaylist != null)
            {
                 await LoadPlaylistVideos(SelectedPlaylist.Id);
            }
        }
        
        public bool IsTaggingActive => IsBulkTagMode || _currentQuickMenuVideo != null;

        private VideoDisplayItem? _currentQuickMenuVideo;


        [RelayCommand]
        public void ToggleQuickMenu(VideoDisplayItem video)
        {
            if (video == null) return;

            if (_currentQuickMenuVideo != null && _currentQuickMenuVideo != video)
            {
                _currentQuickMenuVideo.IsQuickMenuOpen = false;
            }

            video.IsQuickMenuOpen = !video.IsQuickMenuOpen;
            
            if (video.IsQuickMenuOpen)
                _currentQuickMenuVideo = video;
            else
                _currentQuickMenuVideo = null;
                
            OnPropertyChanged(nameof(IsTaggingActive));
        }

        [RelayCommand]
        public async Task ToggleStar(VideoDisplayItem video)
        {
             var target = video ?? SelectedVideo;
             if (target == null) return;

             var color = DefaultStarColor;
             if (string.IsNullOrEmpty(color)) return;

             // Check if it has it
             if (target.FolderColors.Contains(color))
             {
                 // Remove
                 target.FolderColors.Remove(color);
                 if (SelectedPlaylist != null)
                 {
                    await App.SqliteService.RemoveVideoFolderAssignmentAsync(SelectedPlaylist.Id, target.ItemId, color);
                 }
             }
             else
             {
                 // Add
                 target.FolderColors.Add(color);
                 if (SelectedPlaylist != null)
                 {
                    await App.SqliteService.AssignVideoToFolderAsync(SelectedPlaylist.Id, target.ItemId, color);
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
            
            var destLower = destination.ToLower();
            ActiveNavTab = destLower;

            PageTitle = destination; // Fallback title

            // Handle Browser Visibility
            IsBrowserVisible = (destLower == "browser");

            switch (destLower)
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
                    // Browser logic is handled by IsBrowserVisible = true
                    // We simply hide the library view and show the persistent BrowserView
                    PageTitle = "Web Browser";
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

        [RelayCommand]
        public void CloseBrowser()
        {
            // close browser window entirely...
            IsBrowserVisible = false;
            
            // ...and having fullscreen youtube player take its place
            IsFullScreen = true; 
            
            // Also reset browser's internal fullscreen state so it starts normal next time
            if (BrowserVm != null) BrowserVm.IsFullScreen = false;
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
                UpdateDisplayedVideos(); // Simplified for brevity in this specific diff
            }
        }

        [RelayCommand]
        public void UploadOrbImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp",
                Title = "Select Orb Image"
            };

            if (dialog.ShowDialog() == true)
            {
                try 
                {
                    var sourcePath = dialog.FileName;
                    var appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ccc", "UserAssets");
                    if (!System.IO.Directory.Exists(appData))
                        System.IO.Directory.CreateDirectory(appData);

                    var ext = System.IO.Path.GetExtension(sourcePath);
                    var fileName = $"orb_custom_{DateTime.Now.Ticks}{ext}";
                    var destPath = System.IO.Path.Combine(appData, fileName);

                    System.IO.File.Copy(sourcePath, destPath, true);

                    // Update Config
                    App.ConfigService.CustomOrbImage = destPath;
                    
                    // Update UI
                    OrbImagePath = destPath;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error uploading orb image: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        public void ResetOrbConfig()
        {
            OrbScale = 1.0;
            OrbOffsetX = 0;
            OrbOffsetY = 0;
            SpillTopLeft = false;
            SpillTopRight = false;
            SpillBottomLeft = false;
            SpillBottomRight = false;
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
            // Auto-play from restored index
            if (_allVideosCache.Any())
            {
                var index = App.PlaylistService.CurrentVideoIndex;
                if (index >= 0 && index < _allVideosCache.Count)
                    await PlayVideo(_allVideosCache[index].VideoId);
                else
                    await PlayVideo(_allVideosCache[0].VideoId);
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
            // Auto-play from restored index
            if (_allVideosCache.Any())
            {
                var index = App.PlaylistService.CurrentVideoIndex;
                if (index >= 0 && index < _allVideosCache.Count)
                    await PlayVideo(_allVideosCache[index].VideoId);
                else
                    await PlayVideo(_allVideosCache[0].VideoId);
            }
        }

        [RelayCommand]
        public async Task NextVideo()
        {
            var next = App.PlaylistService.NextVideo();
            if (next != null)
            {
                await PlayVideo(next.VideoId);
            }
        }

        [RelayCommand]
        public async Task PrevVideo()
        {
            var prev = App.PlaylistService.PreviousVideo();
            if (prev != null)
            {
                await PlayVideo(prev.VideoId);
            }
        }

        [RelayCommand]
        public async Task ShuffleVideo()
        {
            if (!_allVideosCache.Any()) return;

            var random = new Random();
            var index = random.Next(_allVideosCache.Count);
            var video = _allVideosCache[index];
            
            await PlayVideo(video.VideoId);
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
             
             System.Windows.Application.Current.Dispatcher.Invoke(() =>
             {
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
             });
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

        [ObservableProperty]
        private bool _isBulkTagMode;

        [RelayCommand]
        public void ToggleBulkTagMode()
        {
            IsBulkTagMode = !IsBulkTagMode;
            if (_currentQuickMenuVideo != null)
            {
                _currentQuickMenuVideo.IsQuickMenuOpen = false; // Close individual if opening bulk
                _currentQuickMenuVideo = null;
            }
            OnPropertyChanged(nameof(IsTaggingActive));
        }

        [RelayCommand]
        public async Task SaveBulkTags()
        {
            if (SelectedPlaylist == null) return;
            var playlistId = SelectedPlaylist.Id;

            // Iterate all loaded videos (or all cached videos?)
            // We should probably save changes for ALL videos in cache, as user might have paged.
            // But 'FolderColors' is on VideoDisplayItem which is in _allVideosCache.
            
            foreach (var video in _allVideosCache)
            {
                // We need to compare with DB or just sync. 
                // Sync strategy: Get current DB assignments, diff, apply.
                // This might be heavy for 1000s of videos.
                // Optimization: Maybe track 'IsModified' on VideoDisplayItem?
                // For now, let's just do it for the current page or visible ones? 
                // "Bulk tag" usually means I'm working on a batch.
                // Let's assume we check all for correctness.
                
                // Get existing from DB to compare
                // To avoid N+1, maybe we trust the 'initial' state we loaded? 
                // Problem: We didn't keep a copy of 'OriginalFolderColors'.
                // Let's just process "Current Page" or ALL?
                // If we process ALL, it might take time.
                // Let's implement a 'Sync' in SqliteService? 
                // Better: Let's just add/remove based on current simple logic.
                // Since we don't have 'Originals', we can't diff easily without refetching.
                
                // Lazy approach: fetch current assignments for this video, compare, update.
                var currentDbColors = await App.SqliteService.GetVideoFolderAssignmentsAsync(playlistId, video.ItemId);
                
                var newColors = video.FolderColors.ToList();
                
                // To Add
                var toAdd = newColors.Except(currentDbColors).ToList();
                foreach (var c in toAdd)
                {
                    await App.SqliteService.AssignVideoToFolderAsync(playlistId, video.ItemId, c);
                }

                // To Remove
                var toRemove = currentDbColors.Except(newColors).ToList();
                foreach (var c in toRemove)
                {
                    await App.SqliteService.RemoveVideoFolderAssignmentAsync(playlistId, video.ItemId, c);
                }
            }
            
            IsBulkTagMode = false;
            OnPropertyChanged(nameof(IsTaggingActive));
            
            // Refresh view to ensure consistency
            if (SelectedPlaylist != null)
            {
                 await LoadPlaylistVideos(playlistId);
            }
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

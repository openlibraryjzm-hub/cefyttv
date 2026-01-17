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
        public string ThumbnailUrl { get; set; } = "";
        public int ProgressPercentage { get; set; }
        public bool HasProgress => ProgressPercentage > 0;
        public bool IsPlaying { get; set; }
        public bool IsWatched { get; set; }
    }

    public class HistoryDisplayItem
    {
        public string Title { get; set; } = "";
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

        // Collections
        [ObservableProperty]
        private ObservableCollection<PlaylistDisplayItem> _playlists = new();

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _videos = new();

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _likedVideos = new();

        [ObservableProperty]
        private ObservableCollection<VideoDisplayItem> _pinnedVideos = new();

        [ObservableProperty]
        private ObservableCollection<HistoryDisplayItem> _historyItems = new();

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
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Playlists.Clear();
                    foreach (var p in playlists)
                    {
                        Playlists.Add(new PlaylistDisplayItem
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description ?? "",
                            VideoCountText = $"{p.Items.Count} Videos",
                            ThumbnailUrl = p.CustomThumbnailUrl ?? "https://picsum.photos/300/200" // Fallback
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading playlists: {ex.Message}");
            }

            // 2. Dummy Data for other views (until wired up)
            System.Windows.Application.Current.Dispatcher.Invoke(PopulateDummyDetailData);
        }

        private void PopulateDummyDetailData()
        {
             // Videos (General)
            for (int i = 1; i <= 50; i++)
            {
                var progress = (i * 7) % 100;
                Videos.Add(new VideoDisplayItem
                {
                    Title = $"Video Title {i}: The Amazing Exploration",
                    VideoId = $"VID-{i:000}",
                    ThumbnailUrl = $"https://picsum.photos/300/169?random={100+i}",
                    ProgressPercentage = progress,
                    IsWatched = progress > 85,
                    IsPlaying = i == 2
                });
            }
            // ... (Rest of dummy data kept for UI stability)
            
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
            for (int i = 1; i <= 20; i++)
            {
                HistoryItems.Add(new HistoryDisplayItem
                {
                    Title = $"Recently Watched {i}",
                    ThumbnailUrl = $"https://picsum.photos/300/169?random={400+i}",
                    RelativeTime = $"{i * 10} minutes ago",
                    ProgressPercentage = 100 - (i * 2)
                });
            }
        }


        [RelayCommand]
        public async Task OpenPlaylist(long playlistId)
        {
            try
            {
                // 1. Load data in Service
                await App.PlaylistService.LoadPlaylistAsync(playlistId);

                // 2. Map to ViewModel Collection
                var items = App.PlaylistService.CurrentPlaylistItems;
                
                // Switch to UI Thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Videos.Clear();
                    foreach (var item in items)
                    {
                        Videos.Add(new VideoDisplayItem
                        {
                            Title = item.Title ?? "Unknown Title",
                            VideoId = item.VideoId,
                            ThumbnailUrl = item.ThumbnailUrl ?? "/Resources/Images/placeholder.jpg",
                            // Progress/Watched will come from joined data later
                            ProgressPercentage = 0, 
                            IsWatched = false
                        });
                    }
                    
                    // 3. Navigate
                    Navigate("Videos");
                    var playlist = App.PlaylistService.GetAllPlaylistsAsync().Result.FirstOrDefault(p => p.Id == playlistId);
                    if (playlist != null)
                    {
                        SelectedPlaylist = new PlaylistDisplayItem
                        {
                            Id = playlist.Id,
                            Name = playlist.Name,
                            Description = playlist.Description ?? "",
                            VideoCountText = $"{items.Count} Videos",
                            ThumbnailUrl = playlist.CustomThumbnailUrl ?? "https://picsum.photos/300/200"
                        };
                        PageTitle = playlist.Name;
                    }
                    else
                    {
                        PageTitle = "Playlist";
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening playlist: {ex.Message}");
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
            }
        }

        [RelayCommand]
        public void Navigate(string destination)
        {
            PageTitle = destination; // Fallback title
            switch (destination.ToLower())
            {
                case "playlists":
                    CurrentView = new PlaylistsView();
                    PageTitle = "Playlists";
                    break;
                case "videos":
                    CurrentView = new VideosView();
                    // PageTitle = "Videos"; // Let caller set specific title if needed
                    break;
                case "history":
                    CurrentView = new HistoryView();
                    PageTitle = "History";
                    break;
                case "likes":
                    CurrentView = new LikesView();
                    PageTitle = "Likes";
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
        public void GoBack() { /* TODO */ }

        [RelayCommand]
        public void CloseSidebar() { /* TODO */ }

        [RelayCommand]
        public void NavigateToBrowser()
        {
            PageTitle = "Web Browser";
            IsBrowserVisible = true;
            OnPropertyChanged(nameof(IsLibraryVisible));
        }

        [RelayCommand]
        public async Task NextPlaylist()
        {
            if (SelectedPlaylist == null || !Playlists.Any()) return;

            var currentIndex = Playlists.IndexOf(Playlists.FirstOrDefault(p => p.Id == SelectedPlaylist.Id));
            if (currentIndex == -1) return;

            var nextIndex = (currentIndex + 1) % Playlists.Count; // Cycle
            
            await OpenPlaylist(Playlists[nextIndex].Id);
            
            // Auto-play first video of the new playlist
            if (Videos.Any())
            {
                PlayVideo(Videos[0].VideoId);
            }
        }

        [RelayCommand]
        public async Task PrevPlaylist()
        {
            if (SelectedPlaylist == null || !Playlists.Any()) return;

            var currentIndex = Playlists.IndexOf(Playlists.FirstOrDefault(p => p.Id == SelectedPlaylist.Id));
            if (currentIndex == -1) return;

            var prevIndex = (currentIndex - 1 + Playlists.Count) % Playlists.Count; // Cycle
            
            await OpenPlaylist(Playlists[prevIndex].Id);

            // Auto-play first video of the new playlist
            if (Videos.Any())
            {
                PlayVideo(Videos[0].VideoId);
            }
        }

        [RelayCommand]
        public void NextVideo()
        {
            if (SelectedVideo == null || !Videos.Any()) return;

            var currentIndex = Videos.IndexOf(Videos.FirstOrDefault(v => v.VideoId == SelectedVideo.VideoId));
            if (currentIndex == -1) return;

            var nextIndex = (currentIndex + 1) % Videos.Count; // Cycle
            PlayVideo(Videos[nextIndex].VideoId);
        }

        [RelayCommand]
        public void PrevVideo()
        {
             if (SelectedVideo == null || !Videos.Any()) return;

            var currentIndex = Videos.IndexOf(Videos.FirstOrDefault(v => v.VideoId == SelectedVideo.VideoId));
             if (currentIndex == -1) return;

            var prevIndex = (currentIndex - 1 + Videos.Count) % Videos.Count; // Cycle
            PlayVideo(Videos[prevIndex].VideoId);
        }
    }
}

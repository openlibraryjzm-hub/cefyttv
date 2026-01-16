using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ccc.Services;
using ccc.Services.Database.Entities;
using ccc.ViewModels.Cards;

namespace ccc.ViewModels.Views
{
    public partial class VideosViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<VideoCardViewModel> _videos = new();

        public VideosViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            // For now, load ALL videos from ALL playlists
            // In real app, this would filter by the selected playlist
            var allPlaylists = PlaylistService.Instance.GetAllPlaylistsWithItems();
            var allVideos = allPlaylists.SelectMany(p => p.Items ?? new List<PlaylistItem>()).ToList();

            Videos = new ObservableCollection<VideoCardViewModel>(
                allVideos.Select(v => new VideoCardViewModel(v))
            );
        }
    }
}

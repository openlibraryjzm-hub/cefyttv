using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ccc.Services;
using ccc.ViewModels.Cards;

namespace ccc.ViewModels.Views
{
    public partial class PlaylistsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<PlaylistCardViewModel> _playlists = new();

        public PlaylistsViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = PlaylistService.Instance.GetAllPlaylistsWithItems();
            Playlists = new ObservableCollection<PlaylistCardViewModel>(
                data.Select(p => new PlaylistCardViewModel(p))
            );
        }
    }
}

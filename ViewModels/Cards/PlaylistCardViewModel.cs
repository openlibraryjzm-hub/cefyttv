using CommunityToolkit.Mvvm.ComponentModel;
using ccc.Services.Database.Entities;

namespace ccc.ViewModels.Cards
{
    public partial class PlaylistCardViewModel : ObservableObject
    {
        private readonly Playlist _playlist;

        public PlaylistCardViewModel(Playlist playlist)
        {
            _playlist = playlist;
        }

        public long Id => _playlist.Id;
        public string Name => _playlist.Name;
        public string Description => _playlist.Description ?? "";
        
        // Formatted Date
        public string CreatedAtString => DateTime.TryParse(_playlist.CreatedAt, out var date) 
            ? date.ToString("MMM dd, yyyy") 
            : _playlist.CreatedAt;

        // Video Count (Mock for now, or real if included)
        public int VideoCount => _playlist.Items?.Count ?? 0;
        public string VideoCountString => $"{VideoCount} videos";

        // Thumbnail (Mock or from first video)
        public string? ThumbnailUrl => _playlist.Items?.FirstOrDefault()?.ThumbnailUrl;
    }
}

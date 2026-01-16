using CommunityToolkit.Mvvm.ComponentModel;
using ccc.Services.Database.Entities;

namespace ccc.ViewModels.Cards
{
    public partial class VideoCardViewModel : ObservableObject
    {
        private readonly PlaylistItem _video;

        public VideoCardViewModel(PlaylistItem video)
        {
            _video = video;
        }

        public long Id => _video.Id;
        public string Title => _video.Title;
        public string Author => _video.Author ?? "Unknown Author";
        public string ThumbnailUrl => _video.ThumbnailUrl;
        
        // Duration (Mock for now, easy to add to DB later)
        public string DurationString => "10:00"; 
        
        // Progress Logic (Mock)
        public bool HasProgress => false;
    }
}

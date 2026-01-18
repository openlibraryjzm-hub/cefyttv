using ccc.Models.Entities;
using ccc.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccc.Services
{
    public class PlaylistService
    {
        private readonly SqliteService _sqliteService;

        // State corresponding to playlistStore.js
        public List<PlaylistItem> CurrentPlaylistItems { get; private set; } = new List<PlaylistItem>();
        public long? CurrentPlaylistId { get; private set; }
        public int CurrentVideoIndex { get; private set; } = 0;
        
        // Events
        public event EventHandler? PlaylistItemsChanged;
        public event EventHandler<int>? CurrentVideoIndexChanged;

        public PlaylistService(SqliteService sqliteService)
        {
            _sqliteService = sqliteService;
        }

        // --- Data Loading ---

        public async Task LoadPlaylistAsync(long playlistId, string? folderFilter = null)
        {
            CurrentPlaylistId = playlistId;
            if (string.IsNullOrEmpty(folderFilter) || folderFilter == "all")
            {
                CurrentPlaylistItems = await _sqliteService.GetPlaylistItemsAsync(playlistId);
            }
            else if (folderFilter == "unsorted")
            {
                CurrentPlaylistItems = await _sqliteService.GetUnsortedPlaylistItemsAsync(playlistId);
            }
            else
            {
                CurrentPlaylistItems = await _sqliteService.GetVideosInFolderAsync(playlistId, folderFilter);
            }

            CurrentVideoIndex = 0; // Reset or load from persistence?
            PlaylistItemsChanged?.Invoke(this, EventArgs.Empty);
            CurrentVideoIndexChanged?.Invoke(this, CurrentVideoIndex);
        }

        // --- Navigation Logic ---

        public void SetCurrentVideoIndex(int index)
        {
            if (index >= 0 && index < CurrentPlaylistItems.Count)
            {
                CurrentVideoIndex = index;
                CurrentVideoIndexChanged?.Invoke(this, CurrentVideoIndex);
            }
        }

        public PlaylistItem? GetCurrentVideo()
        {
            if (CurrentVideoIndex >= 0 && CurrentVideoIndex < CurrentPlaylistItems.Count)
            {
                return CurrentPlaylistItems[CurrentVideoIndex];
            }
            return null;
        }

        public PlaylistItem? NextVideo()
        {
            if (CurrentPlaylistItems.Count == 0) return null;
            int nextIndex = (CurrentVideoIndex + 1) % CurrentPlaylistItems.Count;
            SetCurrentVideoIndex(nextIndex);
            return GetCurrentVideo();
        }

        public PlaylistItem? PreviousVideo()
        {
            if (CurrentPlaylistItems.Count == 0) return null;
            int prevIndex = (CurrentVideoIndex - 1 + CurrentPlaylistItems.Count) % CurrentPlaylistItems.Count;
            SetCurrentVideoIndex(prevIndex);
            return GetCurrentVideo();
        }
        
        // --- Persistence / CRUD Wrappers ---
        
        public async Task<List<Playlist>> GetAllPlaylistsAsync() => await _sqliteService.GetAllPlaylistsAsync();
        
        public async Task<long> CreatePlaylistAsync(string name, string? description) => await _sqliteService.CreatePlaylistAsync(name, description);
        
        public async Task<long> AddVideoToPlaylistAsync(long playlistId, string url, string videoId, string title, string thumbnail, string? author = null, string? viewCount = null, string? publishedAt = null)
        {
             long itemId = await _sqliteService.AddVideoToPlaylistAsync(playlistId, url, videoId, title, thumbnail, author, viewCount, publishedAt);
             // If adding to current playlist, refresh?
             if (CurrentPlaylistId == playlistId)
             {
                 await LoadPlaylistAsync(playlistId); // Refresh
             }
             return itemId;
        }
    }
}

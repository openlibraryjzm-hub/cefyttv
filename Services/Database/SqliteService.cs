using ccc.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccc.Services.Database
{
    public class SqliteService
    {
        public SqliteService()
        {
        }

        public async Task InitializeAsync()
        {
            using var context = new AppDbContext();
            await context.Database.EnsureCreatedAsync();
        }

        // --- Playlist Operations ---

        public async Task<List<Playlist>> GetAllPlaylistsAsync()
        {
            using var context = new AppDbContext();
            return await context.Playlists
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<long> CreatePlaylistAsync(string name, string? description)
        {
            using var context = new AppDbContext();
            var playlist = new Playlist
            {
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                UpdatedAt = DateTime.UtcNow.ToString("o")
            };

            context.Playlists.Add(playlist);
            await context.SaveChangesAsync();
            return playlist.Id;
        }

        public async Task<Playlist?> GetPlaylistAsync(long id)
        {
            using var context = new AppDbContext();
            return await context.Playlists
                .Include(p => p.Items)
                .AsNoTracking() // Unless we need tracking
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdatePlaylistAsync(long id, string? name, string? description, string? customAscii)
        {
            using var context = new AppDbContext();
            var playlist = await context.Playlists.FindAsync(id);
            if (playlist == null) return false;

            if (name != null) playlist.Name = name;
            if (description != null) playlist.Description = description;
            if (customAscii != null) playlist.CustomAscii = customAscii;
            
            playlist.UpdatedAt = DateTime.UtcNow.ToString("o");
            
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlaylistAsync(long id)
        {
            using var context = new AppDbContext();
            var playlist = await context.Playlists.FindAsync(id);
            if (playlist == null) return false;

            context.Playlists.Remove(playlist);
            await context.SaveChangesAsync();
            return true;
        }

        // --- Playlist Item Operations ---

        public async Task<long> AddVideoToPlaylistAsync(long playlistId, string videoUrl, string videoId, string? title, string? thumbnailUrl)
        {
            using var context = new AppDbContext();
            var count = await context.PlaylistItems.CountAsync(i => i.PlaylistId == playlistId);
            
            // Get max pos if needed, but count is usually enough for append
             var maxPos = await context.PlaylistItems
                .Where(items => items.PlaylistId == playlistId)
                .OrderByDescending(items => items.Position)
                .Select(items => items.Position)
                .FirstOrDefaultAsync();

            long nextPos = count == 0 ? 0 : maxPos + 1;

            var item = new PlaylistItem
            {
                PlaylistId = playlistId,
                VideoUrl = videoUrl,
                VideoId = videoId,
                Title = title,
                ThumbnailUrl = thumbnailUrl,
                Position = nextPos,
                AddedAt = DateTime.UtcNow.ToString("o"),
                IsLocal = 0
            };

            context.PlaylistItems.Add(item);
            await context.SaveChangesAsync();
            return item.Id;
        }

        public async Task<List<PlaylistItem>> GetPlaylistItemsAsync(long playlistId)
        {
            using var context = new AppDbContext();
            return await context.PlaylistItems
                .Where(i => i.PlaylistId == playlistId)
                .OrderBy(i => i.Position)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> RemoveVideoFromPlaylistAsync(long playlistId, long itemId)
        {
            using var context = new AppDbContext();
            var item = await context.PlaylistItems.FindAsync(itemId);
            if (item == null || item.PlaylistId != playlistId) return false;

            context.PlaylistItems.Remove(item);
            await context.SaveChangesAsync();
            return true;
        }

        // --- Video Assignments ---

        public async Task<long> AssignVideoToFolderAsync(long playlistId, long itemId, string folderColor)
        {
            using var context = new AppDbContext();
            var exists = await context.VideoFolderAssignments
                .AnyAsync(a => a.PlaylistId == playlistId && a.ItemId == itemId && a.FolderColor == folderColor);
            
            if (exists) return -1;

            var assignment = new VideoFolderAssignment
            {
                PlaylistId = playlistId,
                ItemId = itemId,
                FolderColor = folderColor,
                CreatedAt = DateTime.UtcNow.ToString("o")
            };

            context.VideoFolderAssignments.Add(assignment);
            await context.SaveChangesAsync();
            return assignment.Id;
        }

        public async Task<List<string>> GetVideoFolderAssignmentsAsync(long playlistId, long itemId)
        {
            using var context = new AppDbContext();
            return await context.VideoFolderAssignments
                .Where(a => a.PlaylistId == playlistId && a.ItemId == itemId)
                .Select(a => a.FolderColor)
                .ToListAsync();
        }
        
        public async Task<List<PlaylistItem>> GetVideosInFolderAsync(long playlistId, string folderColor)
        {
             using var context = new AppDbContext();
             // Nested query can be complex in EF Core sometimes, forcing client evaluation if not careful
             // Join might be better
             return await context.PlaylistItems
                .Where(i => i.PlaylistId == playlistId && i.FolderAssignments.Any(a => a.FolderColor == folderColor))
                .OrderBy(i => i.Position)
                .AsNoTracking()
                .ToListAsync();
        }

         // --- History ---
         public async Task AddToWatchHistoryAsync(string videoUrl, string videoId, string? title, string? thumbnailUrl)
         {
             using var context = new AppDbContext();
             var record = new WatchHistory
             {
                 VideoUrl = videoUrl,
                 VideoId = videoId,
                 Title = title,
                 ThumbnailUrl = thumbnailUrl,
                 WatchedAt = DateTime.UtcNow.ToString("o")
             };
             context.WatchHistory.Add(record);
             await context.SaveChangesAsync();
         }

          public async Task<List<WatchHistory>> GetWatchHistoryAsync(int limit = 100)
         {
             using var context = new AppDbContext();
             return await context.WatchHistory
                 .OrderByDescending(h => h.WatchedAt)
                 .Take(limit)
                 .AsNoTracking()
                 .ToListAsync();
         }
         
         // --- Progress ---
         public async Task UpdateVideoProgressAsync(string videoId, string videoUrl, double? duration, double currentTime)
         {
             using var context = new AppDbContext();
             var progress = await context.VideoProgress.FirstOrDefaultAsync(p => p.VideoId == videoId);
             
             double percent = 0;
             if (duration.HasValue && duration.Value > 0)
             {
                  percent = (currentTime / duration.Value) * 100;
                  if (percent > 100) percent = 100;
             }

             if (progress == null)
             {
                 progress = new VideoProgress
                 {
                     VideoId = videoId,
                     VideoUrl = videoUrl,
                     Duration = duration,
                     LastProgress = currentTime,
                     ProgressPercentage = percent,
                     LastUpdated = DateTime.UtcNow.ToString("o"),
                     HasFullyWatched = (percent >= 85) ? 1 : 0
                 };
                 context.VideoProgress.Add(progress);
             }
             else
             {
                 progress.LastProgress = currentTime;
                 progress.ProgressPercentage = percent;
                 progress.LastUpdated = DateTime.UtcNow.ToString("o");
                 if (duration.HasValue) progress.Duration = duration;
                 if (percent >= 85) progress.HasFullyWatched = 1;
             }
             
             await context.SaveChangesAsync();
         }

         public async Task<VideoProgress?> GetVideoProgressAsync(string videoId)
         {
             using var context = new AppDbContext();
             return await context.VideoProgress
                .FirstOrDefaultAsync(p => p.VideoId == videoId);
         }
    }
}

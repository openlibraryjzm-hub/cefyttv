using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ccc.Services.Database;
using ccc.Services.Database.Entities;

namespace ccc.Services
{
    public class PlaylistService
    {
        // Singleton Instance
        private static PlaylistService? _instance;
        public static PlaylistService Instance => _instance ??= new PlaylistService();

        private PlaylistService() 
        { 
            // Private constructor for Singleton
        }

        /// <summary>
        /// Ensures the database file and tables exist.
        /// </summary>
        public void InitializeDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // This creates the database and all tables if they don't exist.
                    // It does NOT run migrations if the DB already exists but is out of date.
                    // For a local app refactor, this is safe for fresh starts.
                    context.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                // In a real app, log this using a logging service
                System.Diagnostics.Debug.WriteLine($"DB Init Error: {ex.Message}");
                throw; // Rethrow so the app knows it failed
            }
        }

        /// <summary>
        /// Gets all playlists ordered by creation date (or name, depending on pref).
        /// </summary>
        public List<Playlist> GetAllPlaylists()
        {
            using (var context = new AppDbContext())
            {
                return context.Playlists
                    .AsNoTracking() // Read-only for performance
                    .OrderBy(p => p.Id)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a detailed playlist view including Items and Folder Assignments.
        /// </summary>
        public Playlist? GetPlaylistWithItems(long playlistId)
        {
            using (var context = new AppDbContext())
            {
                return context.Playlists
                    .Include(p => p.Items)
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == playlistId);
            }
        }

        /// <summary>
        /// Creates a new playlist with the given name.
        /// </summary>
        public Playlist CreatePlaylist(string name, string? description = null)
        {
            using (var context = new AppDbContext())
            {
                var newPlaylist = new Playlist
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.UtcNow.ToString("o"),
                    UpdatedAt = DateTime.UtcNow.ToString("o")
                };

                context.Playlists.Add(newPlaylist);
                context.SaveChanges();
                return newPlaylist;
            }
        }

        /// <summary>
        /// Deletes a playlist by ID.
        /// </summary>
        public bool DeletePlaylist(long playlistId)
        {
            using (var context = new AppDbContext())
            {
                var playlist = context.Playlists.Find(playlistId);
                if (playlist == null) return false;

                context.Playlists.Remove(playlist);
                context.SaveChanges();
                return true;
            }
        }
        /// <summary>
        /// Gets all playlists with their tokens eagerly loaded.
        /// </summary>
        public List<Playlist> GetAllPlaylistsWithItems()
        {
            using (var context = new AppDbContext())
            {
                return context.Playlists
                    .Include(p => p.Items)
                    .AsNoTracking()
                    .OrderBy(p => p.Id)
                    .ToList();
            }
        }

        /// <summary>
        /// Seeds the database with debug data if empty.
        /// </summary>
        public void SeedDebugData()
        {
            using (var context = new AppDbContext())
            {
                if (context.Playlists.Any()) return; // Already data

                var p1 = new Playlist
                {
                    Name = "Music Mix",
                    Description = "Best songs for coding",
                    CreatedAt = DateTime.UtcNow.ToString("o"),
                    UpdatedAt = DateTime.UtcNow.ToString("o"),
                    Items = new List<PlaylistItem>
                    {
                        new PlaylistItem { 
                            VideoId="jfKfPfyJRdk", 
                            Title="lofi hip hop radio - beats to relax/study to", 
                            ThumbnailUrl="https://img.youtube.com/vi/jfKfPfyJRdk/maxresdefault.jpg",
                            Author="Lofi Girl",
                            VideoUrl="https://www.youtube.com/watch?v=jfKfPfyJRdk",
                            Position = 0
                        },
                         new PlaylistItem { 
                            VideoId="5qap5aO4i9A", 
                            Title="lofi hip hop radio - beats to sleep/chill to", 
                            ThumbnailUrl="https://img.youtube.com/vi/5qap5aO4i9A/maxresdefault.jpg",
                            Author="Lofi Girl",
                            VideoUrl="https://www.youtube.com/watch?v=5qap5aO4i9A",
                            Position = 1
                        }
                    }
                };

                var p2 = new Playlist
                {
                    Name = "Tech Talks",
                    Description = "Interesting conferences",
                    CreatedAt = DateTime.UtcNow.ToString("o"),
                    UpdatedAt = DateTime.UtcNow.ToString("o"),
                    Items = new List<PlaylistItem>
                    {
                        new PlaylistItem { 
                            VideoId="M7lc1UVf-VE", 
                            Title="YouTube Developers Live", 
                            ThumbnailUrl="https://img.youtube.com/vi/M7lc1UVf-VE/maxresdefault.jpg",
                            Author="Google Developers",
                            VideoUrl="https://www.youtube.com/watch?v=M7lc1UVf-VE",
                            Position = 0
                        }
                    }
                };

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a video to a playlist.
        /// </summary>
        public PlaylistItem AddVideoToPlaylist(long playlistId, string videoUrl, string videoId, string title, string thumbnail, string author, string? viewCount, string? publishedAt)
        {
            using (var context = new AppDbContext())
            {
                // Verify playlist exists
                var playlist = context.Playlists.Find(playlistId);
                if (playlist == null) throw new ArgumentException("Playlist not found");

                var newItem = new PlaylistItem
                {
                    PlaylistId = playlistId,
                    VideoUrl = videoUrl,
                    VideoId = videoId,
                    Title = title,
                    ThumbnailUrl = thumbnail,
                    Author = author,
                    ViewCount = viewCount,
                    PublishedAt = publishedAt ?? DateTime.UtcNow.ToString("o"),
                    AddedAt = DateTime.UtcNow.ToString("o"),
                    Position = context.PlaylistItems.Where(i => i.PlaylistId == playlistId).Count()
                };

                context.PlaylistItems.Add(newItem);
                context.SaveChanges();
                return newItem;
            }
        }
    }
}

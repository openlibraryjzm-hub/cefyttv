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
    }
}

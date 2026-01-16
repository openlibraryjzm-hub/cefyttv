using Microsoft.EntityFrameworkCore;
using ccc.Services.Database.Entities;
using System;
using System.IO;

namespace ccc.Services.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItem> PlaylistItems { get; set; }
        public DbSet<VideoProgress> VideoProgress { get; set; }
        public DbSet<WatchHistory> WatchHistory { get; set; }

        // The path to the SQLite database
        private string DbPath { get; }

        public AppDbContext()
        {
            // By default, match the Rust project's behavior of looking in the current directory first
            // or an AppData folder. For dev, we point it to the root 'playlists.db'
            var folder = Environment.CurrentDirectory;
            DbPath = System.IO.Path.Join(folder, "playlists.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys or unique indexes if needed to match Rust schema

            // UNIQUE(video_id) for VideoProgress
            modelBuilder.Entity<VideoProgress>()
                .HasIndex(v => v.VideoId)
                .IsUnique();
        }
    }
}

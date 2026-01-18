using Microsoft.EntityFrameworkCore;
using ccc.Models.Entities;
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
        public DbSet<LikedVideo> LikedVideos { get; set; }
        public DbSet<VideoFolderAssignment> VideoFolderAssignments { get; set; }
        public DbSet<StuckFolder> StuckFolders { get; set; }
        public DbSet<FolderMetadata> FolderMetadata { get; set; }
        public DbSet<PinnedVideo> PinnedVideos { get; set; }

        // The path to the SQLite database
        private string DbPath { get; }

        public AppDbContext()
        {
            // Match the Rust project's behavior of looking in the current directory first
            var folder = Environment.CurrentDirectory;
            DbPath = System.IO.Path.Join(folder, "playlists.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Playlist Items ---
            // Playlist deletion cascades to Items
            modelBuilder.Entity<PlaylistItem>()
                .HasOne(i => i.Playlist)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Video Folder Assignments ---
            // Playlist deletion cascades to Assignments
            modelBuilder.Entity<VideoFolderAssignment>()
                .HasOne(a => a.Playlist)
                .WithMany(p => p.FolderAssignments)
                .HasForeignKey(a => a.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Item deletion cascades to Assignments
            modelBuilder.Entity<VideoFolderAssignment>()
                .HasOne(a => a.PlaylistItem)
                .WithMany(i => i.FolderAssignments)
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Constraint: playlist_id, item_id, folder_color
            modelBuilder.Entity<VideoFolderAssignment>()
                .HasIndex(a => new { a.PlaylistId, a.ItemId, a.FolderColor })
                .IsUnique();

            // --- Stuck Folders ---
            // Playlist deletion cascades to Stuck Folders
            modelBuilder.Entity<StuckFolder>()
                .HasOne(s => s.Playlist)
                .WithMany(p => p.StuckFolders)
                .HasForeignKey(s => s.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Constraint: playlist_id, folder_color
            modelBuilder.Entity<StuckFolder>()
                .HasIndex(s => new { s.PlaylistId, s.FolderColor })
                .IsUnique();

            // --- Folder Metadata ---
            // Playlist deletion cascades to Folder Metadata
            modelBuilder.Entity<FolderMetadata>()
                .HasOne(m => m.Playlist)
                .WithMany(p => p.FolderMetadata)
                .HasForeignKey(m => m.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Constraint: playlist_id, folder_color
            modelBuilder.Entity<FolderMetadata>()
                .HasIndex(m => new { m.PlaylistId, m.FolderColor })
                .IsUnique();

            // --- Video Progress ---
            // Unique Constraint: video_id
            modelBuilder.Entity<VideoProgress>()
                .HasIndex(v => v.VideoId)
                .IsUnique();

            // --- Liked Videos ---
            // Unique Constraint: video_id
            modelBuilder.Entity<LikedVideo>()
                .HasIndex(v => v.VideoId)
                .IsUnique();

            // --- Pinned Videos ---
            // Unique Constraint: video_id
            modelBuilder.Entity<PinnedVideo>()
                .HasIndex(v => v.VideoId)
                .IsUnique();
        }
    }
}

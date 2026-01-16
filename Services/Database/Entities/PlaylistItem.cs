using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Services.Database.Entities
{
    [Table("playlist_items")]
    public class PlaylistItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("playlist_id")]
        public long PlaylistId { get; set; }

        [Required]
        [Column("video_url")]
        public string VideoUrl { get; set; } = string.Empty;

        [Required]
        [Column("video_id")]
        public string VideoId { get; set; } = string.Empty;

        [Column("title")]
        public string? Title { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Required]
        [Column("position")]
        public int Position { get; set; }

        [Required]
        [Column("added_at")]
        public string AddedAt { get; set; } = DateTime.UtcNow.ToString("o");

        [Required]
        [Column("is_local")]
        public bool IsLocal { get; set; }

        [Column("author")]
        public string? Author { get; set; }

        [Column("view_count")]
        public string? ViewCount { get; set; }

        [Column("published_at")]
        public string? PublishedAt { get; set; }

        // Navigation
        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Models.Entities
{
    [Table("watch_history")]
    public class WatchHistory
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

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
        [Column("watched_at")]
        public string WatchedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }
}

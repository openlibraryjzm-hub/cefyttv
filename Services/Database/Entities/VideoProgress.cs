using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Services.Database.Entities
{
    [Table("video_progress")]
    public class VideoProgress
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("video_id")]
        public string VideoId { get; set; } = string.Empty;

        [Required]
        [Column("video_url")]
        public string VideoUrl { get; set; } = string.Empty;

        [Column("duration")]
        public double? Duration { get; set; }

        [Required]
        [Column("last_progress")]
        public double LastProgress { get; set; } = 0.0;

        [Required]
        [Column("progress_percentage")]
        public double ProgressPercentage { get; set; } = 0.0;

        [Required]
        [Column("last_updated")]
        public string LastUpdated { get; set; } = DateTime.UtcNow.ToString("o");

        [Required]
        [Column("has_fully_watched")]
        public bool HasFullyWatched { get; set; } = false;
    }
}

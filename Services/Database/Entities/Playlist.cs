using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Services.Database.Entities
{
    [Table("playlists")]
    public class Playlist
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("custom_ascii")]
        public string? CustomAscii { get; set; }

        [Column("custom_thumbnail_url")]
        public string? CustomThumbnailUrl { get; set; }

        [Required]
        [Column("created_at")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        [Required]
        [Column("updated_at")]
        public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        // Navigation Property: One Playlist has many Items
        public virtual ICollection<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
    }
}

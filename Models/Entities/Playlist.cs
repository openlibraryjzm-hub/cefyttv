using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Models.Entities
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

        [Required]
        [Column("created_at")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        [Required]
        [Column("updated_at")]
        public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        [Column("custom_ascii")]
        public string? CustomAscii { get; set; }

        [Column("custom_thumbnail_url")]
        public string? CustomThumbnailUrl { get; set; }

        [Column("last_watched_video_id")]
        public string? LastWatchedVideoId { get; set; }

        // Navigation Properties
        public virtual ICollection<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
        public virtual ICollection<VideoFolderAssignment> FolderAssignments { get; set; } = new List<VideoFolderAssignment>();
        public virtual ICollection<StuckFolder> StuckFolders { get; set; } = new List<StuckFolder>();
        public virtual ICollection<FolderMetadata> FolderMetadata { get; set; } = new List<FolderMetadata>();
    }
}

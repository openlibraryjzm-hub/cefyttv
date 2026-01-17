using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Models.Entities
{
    [Table("folder_metadata")]
    public class FolderMetadata
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("playlist_id")]
        public long PlaylistId { get; set; }

        [Required]
        [Column("folder_color")]
        public string FolderColor { get; set; } = string.Empty;

        [Column("custom_name")]
        public string? CustomName { get; set; }

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

        // Navigation Properties
        [ForeignKey(nameof(PlaylistId))]
        public virtual Playlist? Playlist { get; set; }
    }
}

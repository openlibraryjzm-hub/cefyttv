using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Models.Entities
{
    [Table("stuck_folders")]
    public class StuckFolder
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

        [Required]
        [Column("created_at")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        // Navigation Properties
        [ForeignKey(nameof(PlaylistId))]
        public virtual Playlist? Playlist { get; set; }
    }
}

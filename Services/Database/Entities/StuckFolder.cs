using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Services.Database.Entities
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

        // Navigation
        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; } = null!;
    }
}

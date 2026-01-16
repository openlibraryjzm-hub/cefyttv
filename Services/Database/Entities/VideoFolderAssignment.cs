using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ccc.Services.Database.Entities
{
    [Table("video_folder_assignments")]
    public class VideoFolderAssignment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("playlist_id")]
        public long PlaylistId { get; set; }

        [Required]
        [Column("item_id")]
        public long ItemId { get; set; }

        [Required]
        [Column("folder_color")]
        public string FolderColor { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        // Navigations
        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; } = null!;

        [ForeignKey("ItemId")]
        public virtual PlaylistItem PlaylistItem { get; set; } = null!;
    }
}

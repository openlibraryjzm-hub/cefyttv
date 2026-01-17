using ccc.Services.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ccc.Services
{
    public class FolderService
    {
        private readonly SqliteService _sqliteService;
        private readonly ConfigService _configService;

        public event EventHandler<string?>? SelectedFolderChanged;
        public string? SelectedFolder { get; private set; }

        public FolderService(SqliteService sqliteService, ConfigService configService)
        {
            _sqliteService = sqliteService;
            _configService = configService;
        }

        public void SelectFolder(string? folderColor)
        {
            if (SelectedFolder != folderColor)
            {
                SelectedFolder = folderColor;
                SelectedFolderChanged?.Invoke(this, folderColor);
            }
        }

        public async Task AssignVideoToFolderAsync(long playlistId, long itemId, string folderColor)
        {
             await _sqliteService.AssignVideoToFolderAsync(playlistId, itemId, folderColor);
             // Trigger update event if needed?
        }
        
        // Additional methods for Bulk Tagging
    }
}

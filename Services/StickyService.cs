using ccc.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ccc.Services
{
    public class StickyService
    {
        // Format: $"{playlistId}::{videoId}::{folderKey}"
        private readonly HashSet<string> _stickiedVideos = new HashSet<string>();
        
        // Persistence should be handled (load/save to local storage/config)
        
        public event EventHandler? StickyChanged;

        public void ToggleSticky(long playlistId, string videoId, string folderId)
        {
            string key = $"{playlistId}::{videoId}::{folderId}";
            if (_stickiedVideos.Contains(key))
            {
                _stickiedVideos.Remove(key);
            }
            else
            {
                _stickiedVideos.Add(key);
            }
            StickyChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsStickied(long playlistId, string videoId, string folderId)
        {
             string key = $"{playlistId}::{videoId}::{folderId}";
             return _stickiedVideos.Contains(key);
        }
    }
}

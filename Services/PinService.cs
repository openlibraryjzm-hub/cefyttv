using System;
using System.Collections.Generic;
using System.Linq;

namespace ccc.Services
{
    public class PinService
    {
        // Session-only pins (expire in 24h logic handled here?)
        private readonly Dictionary<string, DateTime> _pinnedVideos = new Dictionary<string, DateTime>();
        private readonly HashSet<string> _priorityPinIds = new HashSet<string>();

        public event EventHandler? PinsChanged;

        public void TogglePin(string videoId)
        {
            if (_pinnedVideos.ContainsKey(videoId))
            {
                _pinnedVideos.Remove(videoId);
            }
            else
            {
                _pinnedVideos[videoId] = DateTime.UtcNow;
                // Ensure mutual exclusivity
                if (_priorityPinIds.Contains(videoId)) _priorityPinIds.Remove(videoId);
            }
            PinsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void TogglePriorityPin(string videoId)
        {
            if (_priorityPinIds.Contains(videoId))
            {
                _priorityPinIds.Remove(videoId);
            }
            else
            {
                _priorityPinIds.Add(videoId);
                 // Ensure mutual exclusivity
                 if (_pinnedVideos.ContainsKey(videoId)) _pinnedVideos.Remove(videoId);
            }
            PinsChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPinned(string videoId) => _pinnedVideos.ContainsKey(videoId);
        public bool IsPriorityPin(string videoId) => _priorityPinIds.Contains(videoId);
        
        public List<string> GetPinnedVideoIds() => _pinnedVideos.Keys.ToList();
    }
}

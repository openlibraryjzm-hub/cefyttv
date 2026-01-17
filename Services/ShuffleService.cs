using ccc.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ccc.Services
{
    public class ShuffleService
    {
        private Random _rng = new Random();

        // Maintains shuffled mapping or just shuffles list?
        // playlistStore.js: shuffled_order_{playlistId} persisted.

        public List<PlaylistItem> Shuffle(List<PlaylistItem> items)
        {
            var shuffled = new List<PlaylistItem>(items);
            int n = shuffled.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                var value = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = value;
            }
            return shuffled;
        }
    }
}

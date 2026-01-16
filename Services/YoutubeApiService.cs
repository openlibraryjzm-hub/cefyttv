using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ccc.Services
{
    public class YoutubeApiService
    {
        private static YoutubeApiService? _instance;
        public static YoutubeApiService Instance => _instance ??= new YoutubeApiService();
        
        // This should secure ideally, but user provided it for this session.
        public string ApiKey { get; set; } = "AIzaSyBYPwv0a-rRbTrvMA9nF4Wa1ryC0b6l7xw"; // Default from request
        
        private readonly HttpClient _httpClient;

        private YoutubeApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<YoutubePlaylistInfo> GetPlaylistDetails(string playlistId)
        {
            if (string.IsNullOrEmpty(ApiKey)) throw new Exception("API Key missing");

            var url = $"https://www.googleapis.com/youtube/v3/playlists?part=snippet&id={playlistId}&key={ApiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            if (data.RootElement.GetProperty("pageInfo").GetProperty("totalResults").GetInt32() == 0)
                throw new Exception("Playlist not found or private.");
            
            var item = data.RootElement.GetProperty("items")[0];
            var snippet = item.GetProperty("snippet");
            
            return new YoutubePlaylistInfo
            {
                Id = playlistId,
                Title = snippet.GetProperty("title").GetString() ?? "Unknown Playlist",
                Description = snippet.GetProperty("description").GetString() ?? "",
                ChannelTitle = snippet.GetProperty("channelTitle").GetString() ?? ""
            };
        }

        public async Task<List<YoutubeVideoInfo>> GetPlaylistVideos(string playlistId)
        {
            var videos = new List<YoutubeVideoInfo>();
            string? nextPageToken = null;

            do
            {
                var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={playlistId}&maxResults=50&key={ApiKey}";
                if (nextPageToken != null) url += $"&pageToken={nextPageToken}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) break; // Or throw

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var items = doc.RootElement.GetProperty("items");

                foreach (var item in items.EnumerateArray())
                {
                    var snippet = item.GetProperty("snippet");
                    var resourceId = snippet.GetProperty("resourceId");
                    
                    // Only videos
                    if (resourceId.GetProperty("kind").GetString() != "youtube#video") continue;

                    var videoId = resourceId.GetProperty("videoId").GetString()!;
                    var thumbnails = snippet.GetProperty("thumbnails");
                    
                    // Try to get best thumbnail
                    string thumbUrl = "";
                    if (thumbnails.TryGetProperty("maxres", out var maxres)) thumbUrl = maxres.GetProperty("url").GetString()!;
                    else if (thumbnails.TryGetProperty("high", out var high)) thumbUrl = high.GetProperty("url").GetString()!;
                    else if (thumbnails.TryGetProperty("medium", out var medium)) thumbUrl = medium.GetProperty("url").GetString()!;
                    else if (thumbnails.TryGetProperty("default", out var def)) thumbUrl = def.GetProperty("url").GetString()!;

                    videos.Add(new YoutubeVideoInfo
                    {
                        VideoId = videoId,
                        Title = snippet.GetProperty("title").GetString() ?? "Unknown Title",
                        ThumbnailUrl = thumbUrl,
                        Author = snippet.GetProperty("videoOwnerChannelTitle").GetString() ?? snippet.GetProperty("channelTitle").GetString() ?? "Unknown",
                        PublishedAt = snippet.GetProperty("publishedAt").GetString()
                    });
                }
                
                if (doc.RootElement.TryGetProperty("nextPageToken", out var token))
                    nextPageToken = token.GetString();
                else
                    nextPageToken = null;

            } while (nextPageToken != null);

            // Fetch statistics (view counts) in batches
            await EnrichVideoStatistics(videos);
            
            return videos;
        }

        public async Task<YoutubeVideoInfo> GetVideoDetails(string videoId)
        {
             var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&id={videoId}&key={ApiKey}";
             var response = await _httpClient.GetAsync(url);
             response.EnsureSuccessStatusCode();
             
             var json = await response.Content.ReadAsStringAsync();
             var doc = JsonDocument.Parse(json);
             
             if (doc.RootElement.GetProperty("pageInfo").GetProperty("totalResults").GetInt32() == 0)
                  throw new Exception("Video not found.");

             var item = doc.RootElement.GetProperty("items")[0];
             var snippet = item.GetProperty("snippet");
             var stats = item.GetProperty("statistics");

             var thumbnails = snippet.GetProperty("thumbnails");
             string thumbUrl = "";
             if (thumbnails.TryGetProperty("maxres", out var maxres)) thumbUrl = maxres.GetProperty("url").GetString()!;
             else if (thumbnails.TryGetProperty("medium", out var medium)) thumbUrl = medium.GetProperty("url").GetString()!;

             return new YoutubeVideoInfo
             {
                 VideoId = videoId,
                 Title = snippet.GetProperty("title").GetString() ?? "",
                 Author = snippet.GetProperty("channelTitle").GetString() ?? "",
                 ThumbnailUrl = thumbUrl,
                 PublishedAt = snippet.GetProperty("publishedAt").GetString(),
                 ViewCount = stats.TryGetProperty("viewCount", out var vc) ? vc.GetString() : "0"
             };
        }

        private async Task EnrichVideoStatistics(List<YoutubeVideoInfo> videos)
        {
            // Batches of 50
            for (int i = 0; i < videos.Count; i += 50)
            {
                var batch = videos.GetRange(i, Math.Min(50, videos.Count - i));
                var ids = string.Join(",", batch.Select(v => v.VideoId));
                
                var url = $"https://www.googleapis.com/youtube/v3/videos?part=statistics,contentDetails&id={ids}&key={ApiKey}";
                try 
                {
                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var doc = JsonDocument.Parse(json);
                        var items = doc.RootElement.GetProperty("items");

                        foreach (var item in items.EnumerateArray())
                        {
                            var id = item.GetProperty("id").GetString();
                            var stats = item.GetProperty("statistics");
                            var target = batch.FirstOrDefault(v => v.VideoId == id);
                            if (target != null)
                            {
                                if (stats.TryGetProperty("viewCount", out var vc))
                                    target.ViewCount = vc.GetString();
                                    
                                // Could also get Duration here if model supports it
                            }
                        }
                    }
                }
                catch { /* Ignore failures in stats enrichment */ }
            }
        }
    }

    public class YoutubePlaylistInfo
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ChannelTitle { get; set; } = "";
    }

    public class YoutubeVideoInfo
    {
        public string VideoId { get; set; } = "";
        public string Title { get; set; } = "";
        public string ThumbnailUrl { get; set; } = "";
        public string Author { get; set; } = "";
        public string? PublishedAt { get; set; }
        public string? ViewCount { get; set; }
    }
}

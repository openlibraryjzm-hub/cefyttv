using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ccc.Models.Config
{
    public class TabConfig
    {
        [JsonPropertyName("Tabs")]
        public List<TabDefinition> Tabs { get; set; } = new();

        [JsonPropertyName("Presets")]
        public List<TabPreset> Presets { get; set; } = new();

        [JsonPropertyName("ActiveTabId")]
        public string ActiveTabId { get; set; } = "all";

        [JsonPropertyName("ActivePresetId")]
        public string ActivePresetId { get; set; } = "all";
    }

    public class TabDefinition
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("PlaylistIds")]
        public List<string> PlaylistIds { get; set; } = new();
    }

    public class TabPreset
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("TabIds")]
        public List<string> TabIds { get; set; } = new();
    }
}

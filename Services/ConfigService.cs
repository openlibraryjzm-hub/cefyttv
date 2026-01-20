using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ccc.Services
{
    public class ConfigService
    {
        private const string ConfigFileName = "app_config.json";
        private readonly string _configPath;
        private AppConfig _config;

        public event EventHandler<string>? ThemeChanged;

        public ConfigService()
        {
            _configPath = Path.Combine(Environment.CurrentDirectory, ConfigFileName);
            _config = new AppConfig(); // Default
        }

        public async Task LoadAsync()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_configPath);
                    _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                catch (Exception)
                {
                    // Fallback to default
                    _config = new AppConfig();
                }
            }
        }

        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
             await File.WriteAllTextAsync(_configPath, json);
        }

        // --- Properties ---

        public string CurrentThemeId 
        { 
            get => _config.CurrentThemeId;
            set 
            {
                if (_config.CurrentThemeId != value)
                {
                    _config.CurrentThemeId = value;
                    _ = SaveAsync();
                    ThemeChanged?.Invoke(this, value);
                }
            }
        }

        public string UserName 
        { 
            get => _config.UserName;
            set { _config.UserName = value; _ = SaveAsync(); }
        }

        public string UserAvatar 
        { 
            get => _config.UserAvatar;
            set { _config.UserAvatar = value; _ = SaveAsync(); }
        }
        
        public string? DefaultAssignColor
        {
            get => _config.DefaultAssignColor;
            set { _config.DefaultAssignColor = value; _ = SaveAsync(); }
        }

        public string? CustomOrbImage
        {
            get => _config.CustomOrbImage;
            set 
            { 
                 _config.CustomOrbImage = value; 
                 _ = SaveAsync(); 
            }
        }

        public double OrbScale
        {
            get => _config.OrbScale;
            set { _config.OrbScale = value; _ = SaveAsync(); }
        }
        public double OrbOffsetX
        {
            get => _config.OrbOffsetX;
            set { _config.OrbOffsetX = value; _ = SaveAsync(); }
        }
        public double OrbOffsetY
        {
            get => _config.OrbOffsetY;
            set { _config.OrbOffsetY = value; _ = SaveAsync(); }
        }

        public bool IsSpillEnabled
        {
            get => _config.IsSpillEnabled;
            set { _config.IsSpillEnabled = value; _ = SaveAsync(); }
        }

        public bool SpillTopLeft
        {
            get => _config.SpillTopLeft;
            set { _config.SpillTopLeft = value; _ = SaveAsync(); }
        }
        public bool SpillTopRight
        {
            get => _config.SpillTopRight;
            set { _config.SpillTopRight = value; _ = SaveAsync(); }
        }
        public bool SpillBottomLeft
        {
            get => _config.SpillBottomLeft;
            set { _config.SpillBottomLeft = value; _ = SaveAsync(); }
        }
        public bool SpillBottomRight
        {
            get => _config.SpillBottomRight;
            set { _config.SpillBottomRight = value; _ = SaveAsync(); }
        }

        public bool IsVisualizerEnabled
        {
            get => _config.IsVisualizerEnabled;
            set { _config.IsVisualizerEnabled = value; _ = SaveAsync(); }
        }

        // Add other properties as needed from configStore
    }

    public class AppConfig
    {
        public string CurrentThemeId { get; set; } = "nebula"; // Default
        public string UserName { get; set; } = "Boss";
        public string UserAvatar { get; set; } = "( ͡° ͜ʖ ͡°)";
        public string? CustomOrbImage { get; set; }
        public string? CustomBannerImage { get; set; }
        public string? CustomPageBannerImage { get; set; }
        public bool IsSpillEnabled { get; set; } = true;
        public string? DefaultAssignColor { get; set; } = "red"; // Default to red
        
        // Orb Customization
        public double OrbScale { get; set; } = 1.0;
        public double OrbOffsetX { get; set; } = 0;
        public double OrbOffsetY { get; set; } = 0;
        
        public bool SpillTopLeft { get; set; } = false;
        public bool SpillTopRight { get; set; } = false;
        public bool SpillBottomLeft { get; set; } = false;
        public bool SpillBottomRight { get; set; } = false;
        
        public bool IsVisualizerEnabled { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ccc.Models.Config;

namespace ccc.Services
{
    public class TabService
    {
        private const string ConfigFileName = "tabs.json";
        private string _configPath;
        private TabConfig _currentConfig;

        public TabService()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            
            // Fallback to project root if not found in bin (for development)
            if (!File.Exists(_configPath))
            {
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                string devPath = Path.Combine(projectRoot, ConfigFileName);
                if (File.Exists(devPath))
                {
                    _configPath = devPath;
                }
            }
        }

        public async Task<TabConfig> LoadConfigAsync()
        {
            if (!File.Exists(_configPath))
            {
                // Return default empty config if missing
                _currentConfig = new TabConfig();
                return _currentConfig;
            }

            try
            {
                using var stream = File.OpenRead(_configPath);
                _currentConfig = await JsonSerializer.DeserializeAsync<TabConfig>(stream) ?? new TabConfig();
                return _currentConfig;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tabs.json: {ex.Message}");
                _currentConfig = new TabConfig();
                return _currentConfig;
            }
        }

        public async Task SaveConfigAsync(TabConfig config)
        {
            _currentConfig = config;
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                using var stream = File.Create(_configPath);
                await JsonSerializer.SerializeAsync(stream, config, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving tabs.json: {ex.Message}");
            }
        }

        public List<TabDefinition> GetTabsForPreset(string presetId)
        {
            if (_currentConfig == null) return new List<TabDefinition>();

            // "All" is a virtual preset often used to just show everything or default tabs
            if (presetId == "all") 
            {
                return _currentConfig.Tabs; // Simplified logic: All shows all defined tabs
            }

            var preset = _currentConfig.Presets.FirstOrDefault(p => p.Id == presetId);
            if (preset == null) return new List<TabDefinition>();

            return _currentConfig.Tabs
                .Where(t => preset.TabIds.Contains(t.Id))
                .ToList();
        }
    }
}

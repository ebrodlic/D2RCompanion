using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using D2RCompanion.UI.AppCore;

namespace D2RCompanion.UI.Services
{
    public class SettingsService
    {
        private readonly string _filePath;

        public AppSettings Settings { get; private set; }

        public SettingsService(AppPaths paths)
        {
            _filePath = Path.Combine(paths.Root, "settings.json");

            Settings = new AppSettings();
        }
        public void Initialize()
        {
            if (!File.Exists(_filePath))
            {
                Save();
            }
            else
            {
                Load();
            }
        }

        public void Load()
        {
            var json = File.ReadAllText(_filePath);
            Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }

    public class AppSettings
    {
        public bool AutoCheckForUpdates { get; set; } = true;
        public bool SaveImagesToDisk { get; set; } = true;
    }
}

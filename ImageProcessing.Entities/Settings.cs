using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private int theme;
        private int color;

        public static Settings CurrentSettings;
        public static string SettingsFilePath;

        public int Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public int Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Settings()
        {
            Theme = 1;
            Color = 1;
        }

        public async Task Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }

        public static async Task<Settings> GetSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                Settings newSettings = new Settings();
                await newSettings.Save();
                return newSettings;
            }

            JObject obj = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(SettingsFilePath));
            List<JProperty> removedProperties = obj.Properties().Where(x => typeof(Settings).GetProperty(x.Name) is null).ToList();

            if (removedProperties != null && removedProperties.Count > 0)
                removedProperties.ForEach(x => obj.Remove(x.Name));

            bool missingProperties = typeof(Settings).GetProperties().Any(x => obj.Property(x.Name) is null);
            Settings settings = obj.ToObject<Settings>();

            if (missingProperties)
                await settings.Save();

            return settings;
        }
    }
}

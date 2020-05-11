using ImageProcessing.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingsPath = $@"{appDataPath}\ImageProcessing";
            Settings.SettingsFilePath = $@"{settingsPath}\Settings.json";

            if (!Directory.Exists(settingsPath))
                Directory.CreateDirectory(settingsPath);

            Settings.CurrentSettings = Settings.GetSettings().Result;
            base.OnStartup(e);
        }
    }
}

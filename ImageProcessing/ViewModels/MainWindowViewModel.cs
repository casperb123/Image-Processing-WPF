using ControlzEx.Theming;
using ImageProcessing.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace ImageProcessing.ViewModels
{
    public class MainWindowViewModel
    {
        public bool SettingsChanged { get; set; }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            ProcessingUserControl processingUserControl = new ProcessingUserControl(mainWindow);
            mainWindow.userControlProcessing.Content = processingUserControl;

            mainWindow.comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            mainWindow.comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;

            string theme = mainWindow.comboBoxTheme.SelectedItem.ToString();
            string color = mainWindow.comboBoxColor.SelectedItem.ToString();
            ChangeTheme(theme, color, false);
        }

        public void ChangeTheme(string theme, string color, bool triggerChanges = true)
        {
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, theme);
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, color);
            if (triggerChanges)
                SettingsChanged = true;
        }
    }
}

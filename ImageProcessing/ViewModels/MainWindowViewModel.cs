using ControlzEx.Theming;
using ImageProcessing.UserControls;
using System.Windows;

namespace ImageProcessing.ViewModels
{
    public class MainWindowViewModel
    {
        public ProcessingUserControlViewModel ProcessingUserControlViewModel;

        public bool SettingsChanged { get; set; }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            ProcessingUserControl processingUserControl = new ProcessingUserControl(mainWindow);
            ProcessingUserControlViewModel = processingUserControl.ViewModel;
            mainWindow.userControlProcessing.Content = processingUserControl;

            mainWindow.comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            mainWindow.comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;

            string theme = mainWindow.comboBoxTheme.SelectedItem.ToString();
            string color = mainWindow.comboBoxColor.SelectedItem.ToString();
            ChangeTheme(theme, color, false);
        }

        public void ChangeTheme(string theme, string color, bool triggerChanges = true)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.{color}");
            if (triggerChanges)
                SettingsChanged = true;
        }
    }
}

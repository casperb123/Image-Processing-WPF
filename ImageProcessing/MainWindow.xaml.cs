using ImageProcessing.Entities;
using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel(this);
            DataContext = viewModel;
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            flyoutSettings.IsOpen = !flyoutSettings.IsOpen;
        }

        private async void FlyoutSettings_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel.SettingsChanged && !flyoutSettings.IsOpen)
            {
                await Settings.CurrentSettings.Save();
                viewModel.SettingsChanged = false;
            }
        }

        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            string theme = comboBoxTheme.SelectedItem.ToString();
            string color = comboBoxColor.SelectedItem.ToString();
            viewModel.ChangeTheme(theme, color);
        }

        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            await Settings.CurrentSettings.Save();
            if (viewModel.ProcessingUserControlViewModel.EffectsWindow != null)
            {
                if (viewModel.ProcessingUserControlViewModel.EffectsWindow.ViewModel.DragDropWindow != null)
                    viewModel.ProcessingUserControlViewModel.EffectsWindow.ViewModel.DragDropWindow.Close();

                viewModel.ProcessingUserControlViewModel.EffectsWindow.Close();
            }
        }

        private void MetroWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released &&
                viewModel.ProcessingUserControlViewModel != null &&
                viewModel.ProcessingUserControlViewModel.EffectsWindow != null)
                viewModel.ProcessingUserControlViewModel.EffectsWindow.ViewModel.CloseDragDropWindow();
        }
    }
}

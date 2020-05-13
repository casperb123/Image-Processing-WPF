using ImageProcessing.ViewModels;
using ImageProcessing.Windows;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageProcessing.UserControls
{
    /// <summary>
    /// Interaction logic for ProcessingUserControl.xaml
    /// </summary>
    public partial class ProcessingUserControl : UserControl
    {
        public ProcessingUserControlViewModel ViewModel;

        public ProcessingUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            ViewModel = new ProcessingUserControlViewModel(mainWindow, this);
            DataContext = ViewModel;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile();
        }

        private void ButtonResetImage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetImage();
        }

        private void SliderHue_ValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            ViewModel.SetBoxColors();
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ModifyImage();
        }

        private void ButtonSaveImage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveImage();
        }

        private void ButtonOtherEffects_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.OtherEffectsWindow is null)
            {
                ViewModel.OtherEffectsWindow = new OtherEffectsWindow(ViewModel, ViewModel.Pixelate, ViewModel.MedianFilter, ViewModel.PixelateSize, ViewModel.MedianSize);
                ViewModel.OtherEffectsWindow.Show();
            }
            else
                ViewModel.OtherEffectsWindow.Focus();
        }
    }
}

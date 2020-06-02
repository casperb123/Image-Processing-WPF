using ImageProcessing.ViewModels;
using ImageProcessing.Windows;
using MahApps.Metro.Controls;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void SliderHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        private void ButtonImageEffects_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.EffectsWindow is null)
                ViewModel.ShowEffectsWindow();
            else
                ViewModel.EffectsWindow.Close();
        }

        private void ButtonGrayColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerWindow colorPicker = new ColorPickerWindow(ViewModel.PixelColor.R, ViewModel.PixelColor.G, ViewModel.PixelColor.B, ViewModel.PixelColor.A);
            
            if (colorPicker.ShowDialog() == true)
                ViewModel.PixelColor = colorPicker.ViewModel.Color;
        }
    }
}

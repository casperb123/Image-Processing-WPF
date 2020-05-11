using ImageProcessing.ViewModels;
using ImageProcessing.Windows;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessing.UserControls
{
    /// <summary>
    /// Interaction logic for ProcessingUserControl.xaml
    /// </summary>
    public partial class ProcessingUserControl : UserControl
    {
        private ProcessingUserControlViewModel viewModel;

        public ProcessingUserControl(MainWindow mainWindow)
        {
            InitializeComponent();
            viewModel = new ProcessingUserControlViewModel(mainWindow, this);
            DataContext = viewModel;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenFile();
        }

        private void ButtonResetImage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ResetImage();
        }

        private void SliderHue_ValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            viewModel.SetBoxColors();
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ModifyImage();
        }

        private void ButtonSaveImage_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveImage();
        }

        private void ButtonPixelateSize_Click(object sender, RoutedEventArgs e)
        {
            OtherEffectsWindow otherEffects = new OtherEffectsWindow(viewModel.PixelateSize);

            if (otherEffects.ShowDialog() == true)
                viewModel.PixelateSize = otherEffects.ViewModel.PixelateSize;
        }
    }
}

using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System.Windows;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel(this);
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
    }
}

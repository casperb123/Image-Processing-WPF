using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

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

        private void ButtonToggleImages_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ToggleImages();
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

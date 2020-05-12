using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageProcessing.Windows
{
    /// <summary>
    /// Interaction logic for PixelateSizeWindow.xaml
    /// </summary>
    public partial class OtherEffectsWindow : MetroWindow
    {
        public OtherEffectsWindowViewModel ViewModel;

        public OtherEffectsWindow(bool pixelate, bool medianFilter, int pixelateSize, int medianSize)
        {
            InitializeComponent();
            ViewModel = new OtherEffectsWindowViewModel(pixelate, medianFilter, pixelateSize, medianSize);
            DataContext = ViewModel;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}

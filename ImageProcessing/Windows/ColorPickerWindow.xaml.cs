﻿using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace ImageProcessing.Windows
{
    /// <summary>
    /// Interaction logic for ColorIsolationWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : MetroWindow
    {
        public ColorPickerWindowViewModel ViewModel;

        public ColorPickerWindow(int red, int green, int blue, int alpha)
        {
            InitializeComponent();
            ViewModel = new ColorPickerWindowViewModel(red, green, blue, alpha);
            DataContext = ViewModel;
        }

        private void Color_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromArgb((byte)ViewModel.Alpha, (byte)ViewModel.Red, (byte)ViewModel.Green, (byte)ViewModel.Blue);
            rectangleColor.Fill = new SolidColorBrush(color);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
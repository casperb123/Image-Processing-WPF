using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageProcessing.Windows
{
    /// <summary>
    /// Interaction logic for DragDropWindow.xaml
    /// </summary>
    public partial class DragDropWindow : MetroWindow
    {
        public DragDropWindow(List<Button> buttons)
        {
            InitializeComponent();

            foreach (Button button in buttons)
            {
                Button newButton = new Button
                {
                    BorderThickness = new Thickness(2),
                    BorderBrush = button.BorderBrush,
                    Content = button.Content,
                    IsHitTestVisible = false,
                    Opacity = .7
                };

                stackPanelButtons.Children.Add(newButton);
            }
        }
    }
}

using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

using ImageProcessing.Entities;
using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static ImageProcessing.ViewModels.ProcessingUserControlViewModel;

namespace ImageProcessing.Windows
{
    /// <summary>
    /// Interaction logic for PixelateSizeWindow.xaml
    /// </summary>
    public partial class EffectsWindow : MetroWindow
    {
        public EffectsWindowViewModel ViewModel;

        public EffectsWindow(ProcessingUserControlViewModel processingUserControlViewModel, double top = -1, double left = -1)
        {
            InitializeComponent();
            ViewModel = new EffectsWindowViewModel(processingUserControlViewModel, this);
            DataContext = ViewModel;
            processingUserControlViewModel.UserControl.buttonImageEffects.Content = "Close";

            if (top > -1 && left > -1)
            {
                Top = top;
                Left = left;
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.Closing();
        }

        public void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SetStartPoint(e.GetPosition(null));
        }

        //public void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    ViewModel.EffectClicked(button);
        //}

        //public void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed && !ViewModel.Dragging)
        //    {
        //        Point position = e.GetPosition(null);
        //        Button button = sender as Button;
        //        ViewModel.TriggerDragDrop(position, button);
        //    }
        //}

        private void Effects_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
                e.Effects = DragDropEffects.Move;
        }

        private void Effects_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled == false && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                ObservableCollection<ImageEffect> rows = e.Data.GetData("Object") as ObservableCollection<ImageEffect>;
                ViewModel.DisableEffects(rows);
                e.Effects = DragDropEffects.Move;
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled && ViewModel.Dragging && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                int index = ViewModel.GetCurrentRowIndex(dataGridEnabledEffects, e.GetPosition);
                ObservableCollection<ImageEffect> rows = e.Data.GetData("Object") as ObservableCollection<ImageEffect>;
                ViewModel.EnableEffects(rows, index);
                e.Effects = DragDropEffects.Move;
            }
        }
    }
}

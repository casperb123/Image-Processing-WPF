using ImageProcessing.UserControls;
using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            ViewModel.ProcessingUserControlViewModel.UserControl.buttonImageEffects.Content = "Open";
            ViewModel.ProcessingUserControlViewModel.EffectsWindow = null;
        }

        private void Effects_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
                e.Effects = DragDropEffects.Move;
        }

        private void Effects_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled == false)
            {
                Button button = (Button)e.Data.GetData("Object");

                if (button != null)
                {
                    if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                    {
                        FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                        FilterType filter = ViewModel.ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == filterType);

                        if (filter != FilterType.Invalid)
                        {
                            ViewModel.ProcessingUserControlViewModel.Filters.Remove(filter);
                            stackPanelEnabledEffects.Children.Remove(button);
                            UpdateLayout();
                            stackPanelEffects.Children.Add(button);
                            e.Effects = DragDropEffects.Move;
                        }
                    }
                }
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled)
            {
                Button button = (Button)e.Data.GetData("Object");

                if (button != null)
                {
                    if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                    {
                        FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));

                        ViewModel.ProcessingUserControlViewModel.Filters.Add(filterType);
                        stackPanelEffects.Children.Remove(button);
                        UpdateLayout();
                        stackPanelEnabledEffects.Children.Add(button);
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("Object", sender);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            }
        }
    }
}

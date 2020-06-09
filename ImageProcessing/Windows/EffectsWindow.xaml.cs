using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
                foreach (var item in e.Data.GetData("Object") as Dictionary<Button, int>)
                {
                    FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), item.Key.Name.Substring(6));
                    FilterType filter = ViewModel.ProcessingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x == filterType);
                    int index = ViewModel.ProcessingUserControlViewModel.Filters.Keys.ToList().IndexOf(filterType);

                    if (ViewModel.ProcessingUserControlViewModel.EnabledFilters.Contains(filter))
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
                        stackPanelEnabledEffects.Children.Remove(item.Key);

                        UpdateLayout();

                        stackPanelEffects.Children.Insert(index, item.Key);
                        e.Effects = DragDropEffects.Move;
                    }
                }

                //Button button = (Button)e.Data.GetData("Object");

                //if (button != null && e.AllowedEffects.HasFlag(DragDropEffects.Move))
                //{
                //    FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                //    FilterType filter = ViewModel.ProcessingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x == filterType);

                //    if (filter != FilterType.Invalid)
                //    {
                //        int index = ViewModel.ProcessingUserControlViewModel.Filters.Keys.ToList().IndexOf(filterType);

                //        if (ViewModel.ProcessingUserControlViewModel.EnabledFilters.Contains(filter))
                //        {
                //            ViewModel.ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
                //            stackPanelEnabledEffects.Children.Remove(button);

                //            UpdateLayout();

                //            stackPanelEffects.Children.Insert(index, button);
                //            e.Effects = DragDropEffects.Move;
                //        }
                //    }

                //    ViewModel.Dragging = false;
                //}
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled && ViewModel.Dragging && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                int index = ViewModel.GetCurrentButtonIndex(stackPanelEnabledEffects, e.GetPosition);

                foreach (var item in e.Data.GetData("Object") as Dictionary<Button, int>)
                {
                    FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), item.Key.Name.Substring(6));

                    if (ViewModel.ProcessingUserControlViewModel.EnabledFilters.Contains(filter))
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
                        stackPanelEnabledEffects.Children.Remove(item.Key);
                    }
                    else
                        stackPanelEffects.Children.Remove(item.Key);

                    UpdateLayout();

                    if (index == -1)
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Add(filter);
                        stackPanelEnabledEffects.Children.Add(item.Key);
                        index = 0;
                    }
                    else
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Insert(index, filter);
                        stackPanelEnabledEffects.Children.Insert(index, item.Key);
                    }

                    e.Effects = DragDropEffects.Move;
                    index++;
                }

                ViewModel.ResetSelection();
                ViewModel.Dragging = false;

                //Button button = (Button)e.Data.GetData("Object");

                //if (button != null && e.AllowedEffects.HasFlag(DragDropEffects.Move))
                //{
                //    FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));

                //    if (filter != FilterType.Invalid)
                //    {
                //        int index = ViewModel.GetCurrentButtonIndex(stackPanelEnabledEffects, e.GetPosition);

                //        if (ViewModel.ProcessingUserControlViewModel.EnabledFilters.Contains(filter))
                //        {
                //            ViewModel.ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
                //            stackPanelEnabledEffects.Children.Remove(button);
                //        }
                //        else
                //            stackPanelEffects.Children.Remove(button);

                //        UpdateLayout();

                //        if (index == -1)
                //        {
                //            ViewModel.ProcessingUserControlViewModel.EnabledFilters.Add(filter);
                //            stackPanelEnabledEffects.Children.Add(button);
                //        }
                //        else
                //        {
                //            ViewModel.ProcessingUserControlViewModel.EnabledFilters.Insert(index, filter);
                //            stackPanelEnabledEffects.Children.Insert(index, button);
                //        }

                //        e.Effects = DragDropEffects.Move;
                //    }

                //    ViewModel.Dragging = false;
                //}
            }
        }
    }
}

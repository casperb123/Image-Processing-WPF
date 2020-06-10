using ImageProcessing.Entities;
using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
            ViewModel.ProcessingUserControlViewModel.UserControl.buttonImageEffects.Content = "Open";
            ViewModel.ProcessingUserControlViewModel.EffectsWindow = null;
        }

        public void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.StartPoint = e.GetPosition(null);
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
            int index = stackPanel.Children.IndexOf(button);

            if (ViewModel.ToMove.stackPanel is null)
                ViewModel.ToMove.stackPanel = stackPanel;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (ViewModel.Selected.startIndex > -1 && ViewModel.Selected.endIndex == -1)
                    ViewModel.ResetSelection();

                if (ViewModel.ToMove.stackPanel != stackPanel)
                {
                    ViewModel.ToMove.buttons.Clear();
                    ViewModel.ToMove.stackPanel = stackPanel;
                    ViewModel.ToMove.buttons.Add((button, index));
                }
                else
                {
                    if (ViewModel.ToMove.buttons.Any(x => x.Item1 == button))
                        ViewModel.ToMove.buttons.RemoveAt(index);
                    else
                        ViewModel.ToMove.buttons.Add((button, index));
                }

                ViewModel.StyleButtons();
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (ViewModel.Selected.startIndex == -1)
                {
                    ViewModel.ToMove.buttons.Clear();
                    ViewModel.Selected.startIndex = index;
                    ViewModel.ToMove.stackPanel = stackPanel;
                    ViewModel.ToMove.buttons.Add((button, index));
                }
                else if (ViewModel.Selected.startIndex > -1 && ViewModel.Selected.endIndex == -1)
                {
                    ViewModel.Selected.endIndex = index;

                    if (ViewModel.Selected.endIndex > ViewModel.Selected.startIndex)
                        for (int i = ViewModel.Selected.startIndex + 1; i <= ViewModel.Selected.endIndex; i++)
                            ViewModel.ToMove.buttons.Add((stackPanel.Children[i] as Button, i));
                    else
                    {
                        int newIndex = 0;

                        for (int i = ViewModel.Selected.endIndex; i < ViewModel.Selected.startIndex; i++)
                        {
                            ViewModel.ToMove.buttons.Insert(newIndex, (stackPanel.Children[i] as Button, newIndex));
                            newIndex++;
                        }
                    }

                    ViewModel.Selected.startIndex = -1;
                    ViewModel.Selected.endIndex = -1;
                }

                ViewModel.StyleButtons();
            }
            else
            {
                ViewModel.ResetSelection();

                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                ImageEffect imageEffect = ViewModel.ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x.Key == filterType).Value;

                if (imageEffect.MinimumValue == -1 && imageEffect.MaximumValue == -1)
                    flyoutEffectSettings.IsOpen = false;
                else
                {
                    ViewModel.CurrentImageEffect = imageEffect;
                    flyoutEffectSettings.IsOpen = true;
                }
            }
        }

        public void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !ViewModel.Dragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - ViewModel.StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - ViewModel.StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (ViewModel.Selected.startIndex > -1 && ViewModel.Selected.endIndex == -1)
                        ViewModel.ResetSelection();

                    Button button = sender as Button;
                    StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
                    int index = stackPanel.Children.IndexOf(button);

                    if (ViewModel.ToMove.stackPanel != stackPanel)
                    {
                        ViewModel.ResetSelection();
                        ViewModel.ToMove.stackPanel = stackPanel;
                    }

                    if (ViewModel.ToMove.buttons.Count == 0)
                        ViewModel.ToMove.buttons.Add((button, index));
                    else if (!ViewModel.ToMove.buttons.Any(x => x.Item1 == button))
                    {
                        ViewModel.ResetSelection();
                        ViewModel.ToMove.buttons.Add((button, index));
                    }

                    ViewModel.Dragging = true;

                    DataObject data = new DataObject();
                    data.SetData("Object", ViewModel.ToMove.buttons);

                    DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                }
            }
        }

        private void Effects_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
                e.Effects = DragDropEffects.Move;
        }

        private void Effects_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled == false && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                List<(Button, int)> buttons = e.Data.GetData("Object") as List<(Button, int)>;
                ViewModel.DisableEffects(buttons);
                e.Effects = DragDropEffects.Move;
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled && ViewModel.Dragging && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                int index = ViewModel.GetCurrentButtonIndex(stackPanelEnabledEffects, e.GetPosition);
                List<(Button, int)> buttons = e.Data.GetData("Object") as List<(Button, int)>;

                foreach (var button in buttons)
                {
                    FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));

                    if (ViewModel.ProcessingUserControlViewModel.EnabledFilters.Contains(filter))
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
                        stackPanelEnabledEffects.Children.Remove(button.Item1);
                    }
                    else
                        stackPanelEffects.Children.Remove(button.Item1);

                    UpdateLayout();

                    if (index == -1)
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Add(filter);
                        stackPanelEnabledEffects.Children.Add(button.Item1);
                        index = 0;
                    }
                    else
                    {
                        ViewModel.ProcessingUserControlViewModel.EnabledFilters.Insert(index, filter);
                        stackPanelEnabledEffects.Children.Insert(index, button.Item1);
                    }

                    e.Effects = DragDropEffects.Move;
                    index++;
                }

                ViewModel.ResetSelection();
                ViewModel.Dragging = false;
            }
        }
    }
}

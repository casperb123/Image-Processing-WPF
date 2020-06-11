﻿using ImageProcessing.Entities;
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
            ViewModel.Closing();
        }

        public void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.SetStartPoint(e.GetPosition(null));
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ViewModel.EffectClicked(button);
        }

        public void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                !ViewModel.Dragging &&
                !Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) &&
                !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.RightShift))
            {
                Point position = e.GetPosition(null);
                Button button = sender as Button;
                StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
                ViewModel.TriggerDragDrop(position, stackPanel, button);
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
                if (ViewModel.DragDropWindow != null)
                {
                    ViewModel.DragDropWindow.Close();
                    ViewModel.DragDropWindow = null;
                }
                e.Effects = DragDropEffects.Move;
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled && ViewModel.Dragging && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                int index = ViewModel.GetCurrentButtonIndex(stackPanelEnabledEffects, e.GetPosition);
                List<(Button, int)> buttons = e.Data.GetData("Object") as List<(Button, int)>;
                ViewModel.EnableEffects(buttons, index);
                if (ViewModel.DragDropWindow != null)
                {
                    ViewModel.DragDropWindow.Close();
                    ViewModel.DragDropWindow = null;
                }
                e.Effects = DragDropEffects.Move;
            }
        }

        private void StackPanelEffects_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            ViewModel.MoveDragDropWindow();
        }
    }
}

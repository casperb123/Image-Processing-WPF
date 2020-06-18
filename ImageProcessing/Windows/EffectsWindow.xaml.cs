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
                ValueTuple<StackPanel, List<Button>> toMove = (ValueTuple<StackPanel, List<Button>>)e.Data.GetData("Object");
                if (toMove.Item1 != stackPanelEffects)
                    ViewModel.DisableEffects(toMove.Item2);

                ViewModel.CloseDragDropWindow();
                e.Effects = DragDropEffects.Move;
            }
        }

        private void EnabledEffects_Drop(object sender, DragEventArgs e)
        {
            if (!e.Handled && ViewModel.Dragging && e.AllowedEffects.HasFlag(DragDropEffects.Move))
            {
                int index = ViewModel.GetCurrentButtonIndex(stackPanelEnabledEffects, e.GetPosition);
                ValueTuple<StackPanel, List<Button>> toMove = (ValueTuple<StackPanel, List<Button>>)e.Data.GetData("Object");
                ViewModel.EnableEffects(toMove.Item2, index);
                ViewModel.CloseDragDropWindow();
                e.Effects = DragDropEffects.Move;
            }
        }

        private void StackPanelEffects_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            ViewModel.MoveDragDropWindow();
        }

        private void MetroWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && ViewModel.DragDropWindow != null)
                ViewModel.CloseDragDropWindow();
        }

        private void Effects_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<Button> buttons = new List<Button>();

            foreach (Button disabledButton in stackPanelEffects.Children)
                buttons.Add(disabledButton);
            foreach (Button enabledButton in stackPanelEnabledEffects.Children)
                buttons.Add(enabledButton);

            bool isMouseOverButton = buttons.Any(x => x.IsMouseOver);

            if (!isMouseOverButton)
                ViewModel.ResetSelection();
        }

        private void FlyoutEffectSettings_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (!flyoutEffectSettings.IsOpen)
                ViewModel.ResetSelection();
        }
    }
}

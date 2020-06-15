using ImageProcessing.Entities;
using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ImageProcessing.Entities.ImageEffect;

namespace ImageProcessing.ViewModels
{
    public class EffectsWindowViewModel : INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int X;
            public int Y;
        }

        private ImageEffect currentImageEffect;
        private readonly EffectsWindow window;
        private Point startPoint;
        private (StackPanel stackPanel, List<(Button, int)> buttons) toMove;
        private (int startIndex, int endIndex) selected;
        private readonly ProcessingUserControlViewModel processingUserControlViewModel;

        public delegate Point GetPosition(IInputElement element);
        public DragDropWindow DragDropWindow;

        public ImageEffect CurrentImageEffect
        {
            get { return currentImageEffect; }
            set
            {
                currentImageEffect = value;
                OnPropertyChanged(nameof(CurrentImageEffect));
            }
        }

        public bool Dragging { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public EffectsWindowViewModel(ProcessingUserControlViewModel processingUserControlViewModel, EffectsWindow effectsWindow)
        {
            this.processingUserControlViewModel = processingUserControlViewModel;
            window = effectsWindow;
            CreateFilterButtons(processingUserControlViewModel.Filters, processingUserControlViewModel.EnabledFilters);
            selected.startIndex = -1;
            selected.endIndex = -1;
            toMove.buttons = new List<(Button, int)>();
        }

        public void CreateDragDropWindow(List<Button> buttons)
        {
            DragDropWindow = new DragDropWindow(buttons);
            DragDropWindow.Show();
            MoveDragDropWindow();
        }

        public void CloseDragDropWindow()
        {
            if (DragDropWindow != null)
            {
                DragDropWindow.Close();
                DragDropWindow = null;
                Dragging = false;
                ResetSelection();
            }
        }

        public void Closing()
        {
            processingUserControlViewModel.UserControl.buttonImageEffects.Content = "Open";
            processingUserControlViewModel.EffectsWindow = null;
        }

        private Button GetButton(StackPanel stackPanel, int index)
        {
            return stackPanel.Children[index] as Button;
        }

        public int GetCurrentButtonIndex(StackPanel stackPanel, GetPosition position)
        {
            int curIndex = -1;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                Button button = GetButton(stackPanel, i);
                if (GetMouseTarget(button, position))
                {
                    curIndex = i;
                    break;
                }
            }

            return curIndex;
        }

        private bool GetMouseTarget(Visual target, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(target);
            Point point = position((IInputElement)target);
            return rect.Contains(point);
        }

        private void CreateFilterButtons(List<ImageEffect> filters, List<ImageEffect> enabledFilters)
        {
            foreach (var filter in filters)
            {
                Button button = new Button
                {
                    Name = $"button{filter.Filter}",
                    Content = filter.EffectName,
                    Focusable = false,
                    BorderThickness = new Thickness(2)
                };

                button.PreviewMouseMove += window.Button_PreviewMouseMove;
                button.PreviewMouseLeftButtonDown += window.Button_PreviewMouseLeftButtonDown;
                button.Click += window.Button_Click;

                if (enabledFilters.Contains(filter))
                    window.stackPanelEnabledEffects.Children.Add(button);
                else
                    window.stackPanelEffects.Children.Add(button);
            }
        }

        public void MoveDragDropWindow()
        {
            Win32Point point = new Win32Point();
            GetCursorPos(ref point);

            DragDropWindow.Left = point.X + 15;
            DragDropWindow.Top = point.Y + 15;
        }

        public void StyleButtons()
        {
            List<Button> buttons = new List<Button>();

            foreach (Button disabledbutton in window.stackPanelEffects.Children)
                buttons.Add(disabledbutton);
            foreach (Button enabledButton in window.stackPanelEnabledEffects.Children)
                buttons.Add(enabledButton);

            foreach (Button button in buttons)
            {
                if (toMove.buttons.Any(x => x.Item1 == button))
                    button.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Accent");
                else
                    button.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Button.Border");
            }
        }

        public void ResetSelection()
        {
            selected.startIndex = -1;
            selected.endIndex = -1;
            toMove.buttons.Clear();
            StyleButtons();
        }

        public void SetStartPoint(Point point)
        {
            startPoint = point;
        }

        public void TriggerDragDrop(Point position, StackPanel stackPanel, Button button)
        {
            if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (selected.startIndex > -1 && selected.endIndex == -1)
                    ResetSelection();

                int index = stackPanel.Children.IndexOf(button);

                if (toMove.stackPanel != stackPanel)
                {
                    ResetSelection();
                    toMove.stackPanel = stackPanel;
                }

                if (toMove.buttons.Count == 0)
                    toMove.buttons.Add((button, index));
                else if (!toMove.buttons.Any(x => x.Item1 == button))
                {
                    ResetSelection();
                    toMove.buttons.Add((button, index));
                }

                StyleButtons();
                Dragging = true;

                DataObject data = new DataObject();
                data.SetData("Object", toMove);

                CreateDragDropWindow(new List<Button>(toMove.buttons.Select(x => x.Item1)));
                DragDrop.DoDragDrop(stackPanel, data, DragDropEffects.Move);
            }
        }

        public void EffectClicked(Button button)
        {
            StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
            int index = stackPanel.Children.IndexOf(button);

            if (toMove.stackPanel is null)
                toMove.stackPanel = stackPanel;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (selected.startIndex > -1 && selected.endIndex == -1 || toMove.stackPanel != stackPanel)
                    ResetSelection();

                if (toMove.stackPanel != stackPanel)
                {
                    toMove.buttons.Clear();
                    toMove.stackPanel = stackPanel;
                    toMove.buttons.Add((button, index));
                }
                else
                {
                    var selectedButton = toMove.buttons.FirstOrDefault(x => x.Item1 == button);

                    if (selectedButton.Item1 != null)
                        toMove.buttons.Remove(selectedButton);
                    else
                        toMove.buttons.Add((button, index));
                }

                StyleButtons();
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (toMove.stackPanel != stackPanel)
                    ResetSelection();

                if (selected.startIndex == -1)
                {
                    toMove.buttons.Clear();
                    selected.startIndex = index;
                    toMove.stackPanel = stackPanel;
                    toMove.buttons.Add((button, index));
                }
                else if (selected.startIndex > -1 && selected.endIndex == -1)
                {
                    selected.endIndex = index;

                    if (selected.endIndex > selected.startIndex)
                        for (int i = selected.startIndex + 1; i <= selected.endIndex; i++)
                            toMove.buttons.Add((stackPanel.Children[i] as Button, i));
                    else
                    {
                        int newIndex = 0;

                        for (int i = selected.endIndex; i < selected.startIndex; i++)
                        {
                            toMove.buttons.Insert(newIndex, (stackPanel.Children[i] as Button, newIndex));
                            newIndex++;
                        }
                    }

                    selected.startIndex = -1;
                    selected.endIndex = -1;
                }

                StyleButtons();
            }
            else
            {
                ResetSelection();
                selected.startIndex = index;
                selected.endIndex = -1;
                toMove.buttons.Add((button, index));
                StyleButtons();

                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                ImageEffect imageEffect = processingUserControlViewModel.Filters.FirstOrDefault(x => x.Filter == filterType);

                if (imageEffect.MinimumValue == -1 && imageEffect.MaximumValue == -1)
                    window.flyoutEffectSettings.IsOpen = false;
                else
                {
                    CurrentImageEffect = imageEffect;
                    window.flyoutEffectSettings.IsOpen = true;
                }
            }
        }

        public void DisableEffects(List<(Button, int)> buttons)
        {
            foreach (var button in buttons)
            {
                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));
                ImageEffect effect = processingUserControlViewModel.Filters.FirstOrDefault(x => x.Filter == filterType);
                ImageEffect enabledEffect = processingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x.Filter == filterType);
                int index = processingUserControlViewModel.Filters.IndexOf(effect);

                if (enabledEffect != null)
                    processingUserControlViewModel.EnabledFilters.Remove(enabledEffect);

                window.stackPanelEnabledEffects.Children.Remove(button.Item1);
                window.UpdateLayout();

                List<FilterType> filterTypes = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => (int)x > (int)filterType).ToList();
                foreach (Button existingButton in window.stackPanelEffects.Children)
                {
                    FilterType existingType = (FilterType)Enum.Parse(typeof(FilterType), existingButton.Name.Substring(6));

                    if (filterTypes.Contains(existingType))
                    {
                        index = window.stackPanelEffects.Children.IndexOf(existingButton);
                        break;
                    }
                }

                if (index > window.stackPanelEffects.Children.Count)
                    index = window.stackPanelEffects.Children.Count;

                window.stackPanelEffects.Children.Insert(index, button.Item1);
            }

            ResetSelection();
            Dragging = false;
        }

        public void EnableEffects(List<(Button, int)> buttons, int index)
        {
            foreach (var button in buttons)
            {
                FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));
                ImageEffect effect = processingUserControlViewModel.Filters.FirstOrDefault(x => x.Filter == filter);
                ImageEffect enabledEffect = processingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x.Filter == filter);

                if (enabledEffect != null)
                {
                    processingUserControlViewModel.EnabledFilters.Remove(effect);
                    window.stackPanelEnabledEffects.Children.Remove(button.Item1);
                }
                else
                    window.stackPanelEffects.Children.Remove(button.Item1);

                window.UpdateLayout();

                if (index == -1)
                {
                    processingUserControlViewModel.EnabledFilters.Add(effect);
                    window.stackPanelEnabledEffects.Children.Add(button.Item1);
                    index = 0;
                }
                else
                {
                    processingUserControlViewModel.EnabledFilters.Insert(index, effect);
                    window.stackPanelEnabledEffects.Children.Insert(index, button.Item1);
                }

                index++;
            }

            ResetSelection();
            Dragging = false;
        }
    }
}

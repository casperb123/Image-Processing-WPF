using ImageProcessing.Entities;
using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ImageProcessing.ViewModels.ProcessingUserControlViewModel;

namespace ImageProcessing.ViewModels
{
    public class EffectsWindowViewModel : INotifyPropertyChanged
    {
        private ImageEffect currentImageEffect;
        private readonly EffectsWindow window;
        private Point startPoint;
        private (StackPanel stackPanel, List<(Button, int)> buttons) toMove;
        private (int startIndex, int endIndex) selected;
        private readonly ProcessingUserControlViewModel processingUserControlViewModel;

        public delegate Point GetPosition(IInputElement element);

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

        private void CreateFilterButtons(Dictionary<FilterType, ImageEffect> filters, List<FilterType> enabledFilters)
        {
            foreach (var filter in filters)
            {
                Button button = new Button
                {
                    Name = $"button{filter.Key}",
                    Content = filter.Value.EffectName,
                    Focusable = false
                };

                button.PreviewMouseMove += window.Button_PreviewMouseMove;
                button.PreviewMouseLeftButtonDown += window.Button_PreviewMouseLeftButtonDown;
                button.Click += window.Button_Click;

                if (enabledFilters.Contains(filter.Key))
                    window.stackPanelEnabledEffects.Children.Add(button);
                else
                    window.stackPanelEffects.Children.Add(button);
            }
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
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 200, 0));
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

        public void TriggerDragDrop(Point position, Button button)
        {
            if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (selected.startIndex > -1 && selected.endIndex == -1)
                    ResetSelection();

                StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
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

                Dragging = true;

                DataObject data = new DataObject();
                data.SetData("Object", toMove.buttons);

                DragDrop.DoDragDrop(window, data, DragDropEffects.Move);
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
                if (selected.startIndex > -1 && selected.endIndex == -1)
                    ResetSelection();

                if (toMove.stackPanel != stackPanel)
                {
                    toMove.buttons.Clear();
                    toMove.stackPanel = stackPanel;
                    toMove.buttons.Add((button, index));
                }
                else
                {
                    if (toMove.buttons.Any(x => x.Item1 == button))
                        toMove.buttons.RemoveAt(index);
                    else
                        toMove.buttons.Add((button, index));
                }

                StyleButtons();
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
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

                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                ImageEffect imageEffect = processingUserControlViewModel.Filters.FirstOrDefault(x => x.Key == filterType).Value;

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
                FilterType filter = processingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x == filterType);
                int index = processingUserControlViewModel.Filters.Keys.ToList().IndexOf(filterType);

                if (filter != FilterType.Invalid)
                {
                    processingUserControlViewModel.EnabledFilters.Remove(filter);
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
            }

            ResetSelection();
            Dragging = false;
        }

        public void EnableEffects(List<(Button, int)> buttons, int index)
        {
            foreach (var button in buttons)
            {
                FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));

                if (processingUserControlViewModel.EnabledFilters.Contains(filter))
                {
                    processingUserControlViewModel.EnabledFilters.Remove(filter);
                    window.stackPanelEnabledEffects.Children.Remove(button.Item1);
                }
                else
                    window.stackPanelEffects.Children.Remove(button.Item1);

                window.UpdateLayout();

                if (index == -1)
                {
                    processingUserControlViewModel.EnabledFilters.Add(filter);
                    window.stackPanelEnabledEffects.Children.Add(button.Item1);
                    index = 0;
                }
                else
                {
                    processingUserControlViewModel.EnabledFilters.Insert(index, filter);
                    window.stackPanelEnabledEffects.Children.Insert(index, button.Item1);
                }

                index++;
            }

            ResetSelection();
            Dragging = false;
        }
    }
}

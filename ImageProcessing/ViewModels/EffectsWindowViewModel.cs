using ImageProcessing.Entities;
using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
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
        private Point startPoint;
        private EffectsWindow window;
        private (StackPanel stackPanel, Dictionary<Button, int> buttons) toMove;
        private (int startIndex, int endIndex) selected;

        public ProcessingUserControlViewModel ProcessingUserControlViewModel;
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
            ProcessingUserControlViewModel = processingUserControlViewModel;
            window = effectsWindow;
            CreateFilterButtons(processingUserControlViewModel.Filters, processingUserControlViewModel.EnabledFilters);
            selected.startIndex = -1;
            selected.endIndex = -1;
            toMove.buttons = new Dictionary<Button, int>();
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

                button.PreviewMouseMove += Button_PreviewMouseMove;
                button.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                button.Click += Button_Click;

                if (enabledFilters.Contains(filter.Key))
                    window.stackPanelEnabledEffects.Children.Add(button);
                else
                    window.stackPanelEffects.Children.Add(button);
            }
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
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
                    toMove.buttons.Add(button, index);
                }
                else
                {
                    if (toMove.buttons.Keys.Contains(button))
                        toMove.buttons.Remove(button);
                    else
                        toMove.buttons.Add(button, index);
                }

                StyleButtons();
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (selected.startIndex == -1)
                {
                    toMove.buttons.Clear();
                    selected.startIndex = index;
                    toMove.buttons.Add(button, index);
                }
                else if (selected.startIndex > -1 && selected.endIndex == -1)
                {
                    selected.endIndex = index;

                    if (selected.endIndex < selected.startIndex)
                        for (int i = selected.startIndex - 1; i >= selected.endIndex; i--)
                            toMove.buttons.Add(stackPanel.Children[i] as Button, i);
                    else
                        for (int i = selected.startIndex + 1; i <= selected.endIndex; i++)
                            toMove.buttons.Add(stackPanel.Children[i] as Button, i);

                    selected.startIndex = -1;
                    selected.endIndex = -1;
                }

                StyleButtons();
            }
            else
            {
                ResetSelection();

                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Name.Substring(6));
                ImageEffect imageEffect = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x.Key == filterType).Value;

                if (imageEffect.MinimumValue == -1 && imageEffect.MaximumValue == -1)
                    window.flyoutEffectSettings.IsOpen = false;
                else
                {
                    CurrentImageEffect = imageEffect;
                    window.flyoutEffectSettings.IsOpen = true;
                }
            }
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !Dragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (selected.startIndex > -1 && selected.endIndex == -1)
                        ResetSelection();

                    Button button = sender as Button;
                    StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
                    int index = stackPanel.Children.IndexOf(button);

                    if (toMove.stackPanel != stackPanel)
                    {
                        ResetSelection();
                        toMove.stackPanel = stackPanel;
                    }

                    if (toMove.buttons.Count == 0)
                        toMove.buttons.Add(button, index);
                    else if (!toMove.buttons.ContainsKey(button))
                    {
                        ResetSelection();
                        toMove.buttons.Add(button, index);
                    }

                    Dragging = true;

                    DataObject data = new DataObject();
                    data.SetData("Object", toMove.buttons);

                    DragDrop.DoDragDrop(window, data, DragDropEffects.Move);
                }
            }
        }

        private void StyleButtons()
        {
            List<Button> buttons = new List<Button>();

            foreach (Button disabledbutton in window.stackPanelEffects.Children)
                buttons.Add(disabledbutton);
            foreach (Button enabledButton in window.stackPanelEnabledEffects.Children)
                buttons.Add(enabledButton);

            foreach (Button button in buttons)
            {
                if (toMove.buttons.ContainsKey(button))
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
    }
}

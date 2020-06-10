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
        private EffectsWindow window;

        public ProcessingUserControlViewModel ProcessingUserControlViewModel;
        public delegate Point GetPosition(IInputElement element);
        public (StackPanel stackPanel, List<(Button, int)> buttons) ToMove;
        public Point StartPoint;
        public (int startIndex, int endIndex) Selected;

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
            Selected.startIndex = -1;
            Selected.endIndex = -1;
            ToMove.buttons = new List<(Button, int)>();
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
                if (ToMove.buttons.Any(x => x.Item1 == button))
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 200, 0));
                else
                    button.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Button.Border");
            }
        }

        public void ResetSelection()
        {
            Selected.startIndex = -1;
            Selected.endIndex = -1;
            ToMove.buttons.Clear();
            StyleButtons();
        }

        public void DisableEffects(List<(Button, int)> buttons)
        {
            foreach (var button in buttons)
            {
                FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));
                FilterType filter = ProcessingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x == filterType);
                int index = ProcessingUserControlViewModel.Filters.Keys.ToList().IndexOf(filterType);

                if (filter != FilterType.Invalid)
                {
                    ProcessingUserControlViewModel.EnabledFilters.Remove(filter);
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
    }
}

using ImageProcessing.Entities;
using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static ImageProcessing.Entities.ImageEffect;

namespace ImageProcessing.ViewModels
{
    public class EffectsWindowViewModel : INotifyPropertyChanged
    {
        private ImageEffect currentImageEffect;
        private readonly EffectsWindow window;
        private Point startPoint;
        private (int startIndex, int endIndex) selected;
        private readonly ProcessingUserControlViewModel processingUserControlViewModel;
        private ObservableCollection<ImageEffect> filters;
        private ObservableCollection<ImageEffect> enabledFilters;
        private ObservableCollection<ImageEffect> selectedRows;

        public delegate Point GetPosition(IInputElement element);

        public ObservableCollection<ImageEffect> Filters
        {
            get { return filters; }
            set
            {
                filters = value;
                OnPropertyChanged(nameof(Filters));
            }
        }

        public ObservableCollection<ImageEffect> EnabledFilters
        {
            get { return enabledFilters; }
            set
            {
                enabledFilters = value;
                OnPropertyChanged(nameof(EnabledFilters));
            }
        }

        public ObservableCollection<ImageEffect> SelectedRows
        {
            get { return selectedRows; }
            set
            {
                selectedRows = value;
                OnPropertyChanged(nameof(SelectedRows));
            }
        }

        public readonly List<ImageEffect> DefaultFilters;
        public DataGrid SelectedDataGrid;

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
            DefaultFilters = new List<ImageEffect>
            {
                { new ImageEffect(FilterType.Invert, "Invert Image") },
                { new ImageEffect(FilterType.SepiaTone, "Sepia Tone") },
                { new ImageEffect(FilterType.Emboss, "Emboss") },
                { new ImageEffect(FilterType.Pixelate, "Pixelate", 1, 100, 1) },
                { new ImageEffect(FilterType.Median, "Median", 3, 19, 2) },
                { new ImageEffect(FilterType.BoxBlur, "Box Blur", 3, 19, 2) },
                { new ImageEffect(FilterType.GaussianBlur, "Gaussian Blur", 1, 100, 1) },
                { new ImageEffect(FilterType.EdgeDetection, "Edge Detection") },
                { new ImageEffect(FilterType.EdgeDetection45Degree, "Edge Detection 45 Degree") },
                { new ImageEffect(FilterType.EdgeDetectionHorizontal, "Edge Detection Horizontal") },
                { new ImageEffect(FilterType.EdgeDetectionVertical, "Edge Detection Vertical") },
                { new ImageEffect(FilterType.EdgeDetectionTopLeft, "Edge Detection Top Left") }
            };
            filters = new ObservableCollection<ImageEffect>(DefaultFilters);
            //CreateFilterButtons(processingUserControlViewModel.Filters, processingUserControlViewModel.EnabledFilters);
            selectedRows = new ObservableCollection<ImageEffect>();
            selected.startIndex = -1;
            selected.endIndex = -1;
        }

        public void Closing()
        {
            processingUserControlViewModel.UserControl.buttonImageEffects.Content = "Open";
            processingUserControlViewModel.EffectsWindow = null;
        }

        private bool GetMouseTargetRow(Visual target, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(target);
            Point point = position((IInputElement)target);
            return rect.Contains(point);
        }

        private DataGridRow GetRowItem(DataGrid dataGrid, int index)
        {
            if (dataGrid.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        public int GetCurrentRowIndex(DataGrid dataGrid, GetPosition position)
        {
            int curIndex = -1;
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                DataGridRow row = GetRowItem(dataGrid, i);
                if (GetMouseTargetRow(row, position))
                {
                    curIndex = i;
                    break;
                }
            }

            return curIndex;
        }

        //private void CreateFilterButtons(ObservableCollection<(FilterType, ImageEffect)> filters,
        //                                 ObservableCollection<(FilterType, ImageEffect)> enabledFilters)
        //{
        //    foreach (var filter in filters)
        //    {
        //        Button button = new Button
        //        {
        //            Name = $"button{filter.Item1}",
        //            Content = filter.Item2.EffectName,
        //            Focusable = false,
        //            BorderThickness = new Thickness(2)
        //        };

        //        button.PreviewMouseMove += window.Button_PreviewMouseMove;
        //        button.PreviewMouseLeftButtonDown += window.Button_PreviewMouseLeftButtonDown;
        //        button.Click += window.Button_Click;

        //        //if (enabledFilters.Any(x => x.Item1 == filter.Item1))
        //        //    window.stackPanelEnabledEffects.Children.Add(button);
        //        //else
        //        //    window.dataGridEffects.Children.Add(button);
        //    }
        //}

        //public void StyleButtons()
        //{
        //    List<Button> buttons = new List<Button>();

        //    foreach (Button disabledbutton in window.stackPanelEffects.Children)
        //        buttons.Add(disabledbutton);
        //    foreach (Button enabledButton in window.stackPanelEnabledEffects.Children)
        //        buttons.Add(enabledButton);

        //    foreach (Button button in buttons)
        //    {
        //        if (toMove.buttons.Any(x => x.Item1 == button))
        //            button.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Accent");
        //        else
        //            button.SetResourceReference(Control.BorderBrushProperty, "MahApps.Brushes.Button.Border");
        //    }
        //}

        public void ResetSelection()
        {
            selected.startIndex = -1;
            selected.endIndex = -1;
            SelectedRows.Clear();
            //StyleButtons();
        }

        public void SetStartPoint(Point point)
        {
            startPoint = point;
        }

        public void TriggerDragDrop(Point position, DataGrid dataGrid, (FilterType, ImageEffect) effect)
        {
            if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (selected.startIndex > -1 && selected.endIndex == -1)
                    ResetSelection();

                //StackPanel stackPanel = VisualTreeHelper.GetParent(button) as StackPanel;
                //int index = stackPanel.Children.IndexOf(button);
                //int index = dataGrid.Items.IndexOf(gridRow);

                if (SelectedDataGrid != dataGrid)
                {
                    ResetSelection();
                    SelectedDataGrid = dataGrid;
                }

                //if (SelectedRows.Count == 0)
                //    SelectedRows.Add(effect);
                //else if (!SelectedRows.Any(x => x.Item1 == gridRow))
                //{
                //    ResetSelection();
                //    toMove.rows.Add((gridRow, index));
                //}

                Dragging = true;

                DataObject data = new DataObject();
                data.SetData("Object", SelectedRows);

                DragDrop.DoDragDrop(window, data, DragDropEffects.Move);
            }
        }

        //public void EffectClicked(DataGridRow gridRow)
        //{
        //    DataGrid dataGrid = VisualTreeHelper.GetParent(gridRow) as DataGrid;
        //    int index = dataGrid.Items.IndexOf(gridRow);

        //    if (toMove.dataGrid is null)
        //        toMove.dataGrid = dataGrid;

        //    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //    {
        //        if (selected.startIndex > -1 && selected.endIndex == -1)
        //            ResetSelection();

        //        if (toMove.dataGrid != dataGrid)
        //        {
        //            toMove.rows.Clear();
        //            toMove.dataGrid = dataGrid;
        //            toMove.rows.Add((gridRow, index));
        //        }
        //        else
        //        {
        //            if (toMove.rows.Any(x => x.Item1 == gridRow))
        //                toMove.rows.RemoveAt(index);
        //            else
        //                toMove.rows.Add((gridRow, index));
        //        }

        //        //StyleButtons();
        //    }
        //    else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
        //    {
        //        if (selected.startIndex == -1)
        //        {
        //            toMove.rows.Clear();
        //            selected.startIndex = index;
        //            toMove.dataGrid = dataGrid;
        //            toMove.rows.Add((gridRow, index));
        //        }
        //        else if (selected.startIndex > -1 && selected.endIndex == -1)
        //        {
        //            selected.endIndex = index;

        //            if (selected.endIndex > selected.startIndex)
        //                for (int i = selected.startIndex + 1; i <= selected.endIndex; i++)
        //                    toMove.rows.Add((dataGrid.Items[i] as DataGridRow, i));
        //            else
        //            {
        //                int newIndex = 0;

        //                for (int i = selected.endIndex; i < selected.startIndex; i++)
        //                {
        //                    toMove.rows.Insert(newIndex, (dataGrid.Items[i] as DataGridRow, newIndex));
        //                    newIndex++;
        //                }
        //            }

        //            selected.startIndex = -1;
        //            selected.endIndex = -1;
        //        }

        //        //StyleButtons();
        //    }
        //    else
        //    {
        //        ResetSelection();

        //        FilterType filterType = (FilterType)Enum.Parse(typeof(FilterType), gridRow.Name.Substring(6));
        //        ImageEffect imageEffect = processingUserControlViewModel.Filters.FirstOrDefault(x => x.Item1 == filterType).Item2;

        //        if (imageEffect.MinimumValue == -1 && imageEffect.MaximumValue == -1)
        //            window.flyoutEffectSettings.IsOpen = false;
        //        else
        //        {
        //            CurrentImageEffect = imageEffect;
        //            window.flyoutEffectSettings.IsOpen = true;
        //        }
        //    }
        //}

        public void DisableEffects(ObservableCollection<ImageEffect> rows)
        {
            foreach (ImageEffect row in rows)
            {
                if (!EnabledFilters.Contains(row))
                    continue;

                EnabledFilters.Remove(row);
                int index = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList().IndexOf(row.Filter);

                List<FilterType> filterTypes = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => (int)x > (int)row.Filter).ToList();
                foreach (var existingRow in Filters)
                {
                    if (filterTypes.Contains(existingRow.Filter))
                    {
                        index = Filters.IndexOf(existingRow);
                        break;
                    }
                }

                if (index > Filters.Count)
                    index = Filters.Count;

                Filters.Insert(index, row);
            }

            ResetSelection();
            Dragging = false;

            //foreach (var row in SelectedRows)
            //{
            //    FilterType filter = processingUserControlViewModel.EnabledFilters.FirstOrDefault(x => x.Item1 == row.Item1).Item1;
            //    int index = DefaultFilters.IndexOf(DefaultFilters.FirstOrDefault(x => x.Item1 == row.Item1));

            //    if (filter != FilterType.Invalid)
            //    {
            //        processingUserControlViewModel.EnabledFilters.Remove(row);
            //        //window.stackPanelEnabledEffects.Children.Remove(button.Item1);

            //        //window.UpdateLayout();

            //        List<FilterType> filterTypes = Enum.GetValues(typeof(FilterType)).Cast<FilterType>().Where(x => (int)x > (int)row.Item1).ToList();
            //        foreach ((FilterType, ImageEffect) existingRow in window.dataGridEffects.Items)
            //        {
            //            //FilterType existingType = (FilterType)Enum.Parse(typeof(FilterType), existingButton.Name.Substring(6));

            //            if (filterTypes.Contains(existingRow.Item1))
            //            {
            //                index = window.dataGridEffects.Items.IndexOf(existingRow);
            //                break;
            //            }
            //        }

            //        if (index > window.dataGridEffects.Items.Count)
            //            index = window.dataGridEffects.Items.Count;

            //        window.stackPanelEffects.Children.Insert(index, button.Item1);
            //    }
            //}

            ResetSelection();
            Dragging = false;
        }

        public void EnableEffects(ObservableCollection<ImageEffect> rows, int index)
        {
            foreach (ImageEffect row in rows)
            {
                if (EnabledFilters.Contains(row))
                    EnabledFilters.Remove(row);
                else
                    Filters.Remove(row);

                if (index == -1)
                {
                    EnabledFilters.Add(row);
                    index = 0;
                }
                else
                    EnabledFilters.Insert(index, row);

                index++;
            }

            //foreach (var button in buttons)
            //{
            //    FilterType filter = (FilterType)Enum.Parse(typeof(FilterType), button.Item1.Name.Substring(6));

            //    if (processingUserControlViewModel.EnabledFilters.Contains(filter))
            //    {
            //        processingUserControlViewModel.EnabledFilters.Remove(filter);
            //        window.stackPanelEnabledEffects.Children.Remove(button.Item1);
            //    }
            //    else
            //        window.stackPanelEffects.Children.Remove(button.Item1);

            //    window.UpdateLayout();

            //    if (index == -1)
            //    {
            //        processingUserControlViewModel.EnabledFilters.Add(filter);
            //        window.stackPanelEnabledEffects.Children.Add(button.Item1);
            //        index = 0;
            //    }
            //    else
            //    {
            //        processingUserControlViewModel.EnabledFilters.Insert(index, filter);
            //        window.stackPanelEnabledEffects.Children.Insert(index, button.Item1);
            //    }

            //    index++;
            //}

            ResetSelection();
            Dragging = false;
        }
    }
}

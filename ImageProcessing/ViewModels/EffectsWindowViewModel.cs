using ImageProcessing.UserControls;
using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ImageProcessing.ViewModels.ProcessingUserControlViewModel;

namespace ImageProcessing.ViewModels
{
    public class EffectsWindowViewModel : INotifyPropertyChanged
    {
        private bool invert;
        private bool sepiaTone;
        private bool emboss;
        private int pixelateSize;
        private int medianSize;
        private bool pixelate;
        private bool median;
        private bool gaussianBlur;
        private int gaussianBlurAmount;
        private bool boxBlur;
        private int boxBlurAmount;
        private bool edgeDetection;
        private bool edgeDetection45Degree;
        private bool edgeDetectionHorizontal;
        private bool edgeDetectionVertical;
        private bool edgeDetectionTopLeft;

        private EffectsWindow window;
        public ProcessingUserControlViewModel ProcessingUserControlViewModel;
        public delegate Point GetPosition(IInputElement element);

        public bool BoxBlur
        {
            get { return boxBlur; }
            set
            {
                boxBlur = value;
                OnPropertyChanged(nameof(boxBlur));

                if (value)
                {
                    int gaussianIndex = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.GaussianBlur);
                    if (gaussianIndex == -1)
                        ProcessingUserControlViewModel.Filters.Add(FilterType.BoxBlur);
                    else
                    {
                        GaussianBlur = false;
                        ProcessingUserControlViewModel.Filters.Insert(gaussianIndex, FilterType.BoxBlur);
                    }
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.BoxBlur);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public int BoxBlurAmount
        {
            get { return boxBlurAmount; }
            set
            {
                boxBlurAmount = value;
                OnPropertyChanged(nameof(BoxBlurAmount));

                ProcessingUserControlViewModel.BoxBlurAmount = value;
            }
        }

        public int GaussianBlurAmount
        {
            get { return gaussianBlurAmount; }
            set
            {
                gaussianBlurAmount = value;
                OnPropertyChanged(nameof(GaussianBlurAmount));

                ProcessingUserControlViewModel.GaussianBlurAmount = value;
            }
        }

        public bool GaussianBlur
        {
            get { return gaussianBlur; }
            set
            {
                gaussianBlur = value;
                OnPropertyChanged(nameof(GaussianBlur));

                if (value)
                {
                    int boxIndex = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.BoxBlur);
                    if (boxIndex == -1)
                        ProcessingUserControlViewModel.Filters.Add(FilterType.GaussianBlur);
                    else
                    {
                        BoxBlur = false;
                        ProcessingUserControlViewModel.Filters.Insert(boxIndex, FilterType.GaussianBlur);
                    }
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.GaussianBlur);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool Median
        {
            get { return median; }
            set
            {
                median = value;
                OnPropertyChanged(nameof(Median));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Median);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Median);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool Pixelate
        {
            get { return pixelate; }
            set
            {
                pixelate = value;
                OnPropertyChanged(nameof(Pixelate));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Pixelate);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Pixelate);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public int MedianSize
        {
            get { return medianSize; }
            set
            {
                medianSize = value;
                OnPropertyChanged(nameof(MedianSize));

                ProcessingUserControlViewModel.MedianSize = value;
            }
        }

        public int PixelateSize
        {
            get { return pixelateSize; }
            set
            {
                pixelateSize = value;
                OnPropertyChanged(nameof(PixelateSize));

                ProcessingUserControlViewModel.PixelateSize = value;
            }
        }

        public bool Emboss
        {
            get { return emboss; }
            set
            {
                emboss = value;
                OnPropertyChanged(nameof(Emboss));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Emboss);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Emboss);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool SepiaTone
        {
            get { return sepiaTone; }
            set
            {
                sepiaTone = value;
                OnPropertyChanged(nameof(SepiaTone));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.SepiaTone);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.SepiaTone);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool Invert
        {
            get { return invert; }
            set
            {
                invert = value;
                OnPropertyChanged(nameof(Invert));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Invert);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Invert);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool EdgeDetection
        {
            get { return edgeDetection; }
            set
            {
                edgeDetection = value;
                OnPropertyChanged(nameof(EdgeDetection));

                if (value)
                {
                    int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection45Degree);
                    int index2 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionHorizontal);
                    int index3 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionVertical);
                    int index4 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionTopLeft);

                    if (index > -1)
                    {
                        EdgeDetection45Degree = false;
                        ProcessingUserControlViewModel.Filters.Insert(index, FilterType.EdgeDetection);
                    }
                    else if (index2 > -1)
                    {
                        EdgeDetectionHorizontal = false;
                        ProcessingUserControlViewModel.Filters.Insert(index2, FilterType.EdgeDetection);
                    }
                    else if (index3 > -1)
                    {
                        EdgeDetectionVertical = false;
                        ProcessingUserControlViewModel.Filters.Insert(index3, FilterType.EdgeDetection);
                    }
                    else if (index4 > -1)
                    {
                        EdgeDetectionTopLeft = false;
                        ProcessingUserControlViewModel.Filters.Insert(index4, FilterType.EdgeDetection);
                    }
                    else
                        ProcessingUserControlViewModel.Filters.Add(FilterType.EdgeDetection);
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.EdgeDetection);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool EdgeDetection45Degree
        {
            get { return edgeDetection45Degree; }
            set
            {
                edgeDetection45Degree = value;
                OnPropertyChanged(nameof(EdgeDetection45Degree));

                if (value)
                {
                    int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection);
                    int index2 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionHorizontal);
                    int index3 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionVertical);
                    int index4 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionTopLeft);

                    if (index > -1)
                    {
                        EdgeDetection = false;
                        ProcessingUserControlViewModel.Filters.Insert(index, FilterType.EdgeDetection45Degree);
                    }
                    else if (index2 > -1)
                    {
                        EdgeDetectionHorizontal = false;
                        ProcessingUserControlViewModel.Filters.Insert(index2, FilterType.EdgeDetection45Degree);
                    }
                    else if (index3 > -1)
                    {
                        EdgeDetectionVertical = false;
                        ProcessingUserControlViewModel.Filters.Insert(index3, FilterType.EdgeDetection45Degree);
                    }
                    else if (index4 > -1)
                    {
                        EdgeDetectionTopLeft = false;
                        ProcessingUserControlViewModel.Filters.Insert(index4, FilterType.EdgeDetection45Degree);
                    }
                    else
                        ProcessingUserControlViewModel.Filters.Add(FilterType.EdgeDetection45Degree);
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.EdgeDetection45Degree);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool EdgeDetectionHorizontal
        {
            get { return edgeDetectionHorizontal; }
            set
            {
                edgeDetectionHorizontal = value;
                OnPropertyChanged(nameof(EdgeDetectionHorizontal));

                if (value)
                {
                    int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection);
                    int index2 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection45Degree);
                    int index3 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionVertical);
                    int index4 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionTopLeft);

                    if (index > -1)
                    {
                        EdgeDetection = false;
                        ProcessingUserControlViewModel.Filters.Insert(index, FilterType.EdgeDetectionHorizontal);
                    }
                    else if (index2 > -1)
                    {
                        EdgeDetection45Degree = false;
                        ProcessingUserControlViewModel.Filters.Insert(index2, FilterType.EdgeDetectionHorizontal);
                    }
                    else if (index3 > -1)
                    {
                        EdgeDetectionVertical = false;
                        ProcessingUserControlViewModel.Filters.Insert(index3, FilterType.EdgeDetectionHorizontal);
                    }
                    else if (index4 > -1)
                    {
                        EdgeDetectionTopLeft = false;
                        ProcessingUserControlViewModel.Filters.Insert(index4, FilterType.EdgeDetectionHorizontal);
                    }
                    else
                        ProcessingUserControlViewModel.Filters.Add(FilterType.EdgeDetectionHorizontal);
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.EdgeDetectionHorizontal);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool EdgeDetectionVertical
        {
            get { return edgeDetectionVertical; }
            set
            {
                edgeDetectionVertical = value;
                OnPropertyChanged(nameof(EdgeDetectionVertical));

                if (value)
                {
                    int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection);
                    int index2 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection45Degree);
                    int index3 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionHorizontal);
                    int index4 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionTopLeft);

                    if (index > -1)
                    {
                        EdgeDetection = false;
                        ProcessingUserControlViewModel.Filters.Insert(index, FilterType.EdgeDetectionVertical);
                    }
                    else if (index2 > -1)
                    {
                        EdgeDetection45Degree = false;
                        ProcessingUserControlViewModel.Filters.Insert(index2, FilterType.EdgeDetectionVertical);
                    }
                    else if (index3 > -1)
                    {
                        EdgeDetectionHorizontal = false;
                        ProcessingUserControlViewModel.Filters.Insert(index3, FilterType.EdgeDetectionVertical);
                    }
                    else if (index4 > -1)
                    {
                        EdgeDetectionTopLeft = false;
                        ProcessingUserControlViewModel.Filters.Insert(index4, FilterType.EdgeDetectionVertical);
                    }
                    else
                        ProcessingUserControlViewModel.Filters.Add(FilterType.EdgeDetectionVertical);
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.EdgeDetectionVertical);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

        public bool EdgeDetectionTopLeft
        {
            get { return edgeDetectionTopLeft; }
            set
            {
                edgeDetectionTopLeft = value;
                OnPropertyChanged(nameof(EdgeDetectionTopLeft));

                if (value)
                {
                    int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection);
                    int index2 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetection45Degree);
                    int index3 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionHorizontal);
                    int index4 = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.EdgeDetectionVertical);

                    if (index > -1)
                    {
                        EdgeDetection = false;
                        ProcessingUserControlViewModel.Filters.Insert(index, FilterType.EdgeDetectionTopLeft);
                    }
                    else if (index2 > -1)
                    {
                        EdgeDetection45Degree = false;
                        ProcessingUserControlViewModel.Filters.Insert(index2, FilterType.EdgeDetectionTopLeft);
                    }
                    else if (index3 > -1)
                    {
                        EdgeDetectionHorizontal = false;
                        ProcessingUserControlViewModel.Filters.Insert(index3, FilterType.EdgeDetectionTopLeft);
                    }
                    else if (index4 > -1)
                    {
                        EdgeDetectionVertical = false;
                        ProcessingUserControlViewModel.Filters.Insert(index4, FilterType.EdgeDetectionTopLeft);
                    }
                    else
                        ProcessingUserControlViewModel.Filters.Add(FilterType.EdgeDetectionTopLeft);
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.EdgeDetectionTopLeft);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                UpdateHeaders();
            }
        }

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

            foreach (FilterType filter in ProcessingUserControlViewModel.Filters)
            {
                switch (filter)
                {
                    case FilterType.Invert:
                        invert = true;
                        break;
                    case FilterType.SepiaTone:
                        sepiaTone = true;
                        break;
                    case FilterType.Emboss:
                        emboss = true;
                        break;
                    case FilterType.Pixelate:
                        pixelate = true;
                        break;
                    case FilterType.Median:
                        median = true;
                        break;
                    case FilterType.BoxBlur:
                        boxBlur = true;
                        break;
                    case FilterType.GaussianBlur:
                        gaussianBlur = true;
                        break;
                    case FilterType.EdgeDetection:
                        edgeDetection = true;
                        break;
                    case FilterType.EdgeDetection45Degree:
                        edgeDetection45Degree = true;
                        break;
                    case FilterType.EdgeDetectionHorizontal:
                        edgeDetectionHorizontal = true;
                        break;
                    case FilterType.EdgeDetectionVertical:
                        edgeDetectionVertical = true;
                        break;
                    case FilterType.EdgeDetectionTopLeft:
                        edgeDetectionTopLeft = true;
                        break;
                    default:
                        break;
                }
            }

            //Thread thread = new Thread(() =>
            //{
            //    window.Dispatcher.Invoke(() =>
            //    {
            //        while (!window.IsLoaded) { }
            //        UpdateHeaders();
            //    });
            //});
            //thread.Start();

            PixelateSize = ProcessingUserControlViewModel.PixelateSize;
            MedianSize = ProcessingUserControlViewModel.MedianSize;
            GaussianBlurAmount = ProcessingUserControlViewModel.GaussianBlurAmount;
            BoxBlurAmount = ProcessingUserControlViewModel.BoxBlurAmount;
        }

        private void UpdateHeaders()
        {
            IEnumerable<FilterType> enumValues = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
            foreach (FilterType filterType in enumValues)
            {
                if (filterType == FilterType.Invalid)
                    continue;

                FilterType filter = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == filterType);
                GroupBox groupBox = window.FindName($"groupBox{filterType}") as GroupBox;
                string filterName = ProcessingUserControlViewModel.FilterNames.FirstOrDefault(x => x.Key == filterType).Value;

                int index = ProcessingUserControlViewModel.Filters.IndexOf(filter);
                if (index == -1)
                    groupBox.Header = $"{filterName} - Not applied";
                else
                    groupBox.Header = $"{filterName} - {index + 1}";
            }
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
    }
}

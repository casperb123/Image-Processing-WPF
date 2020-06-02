using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
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
        private bool blurFilters;

        private EffectsWindow window;
        public ProcessingUserControlViewModel ProcessingUserControlViewModel;

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

            foreach (FilterType filter in ProcessingUserControlViewModel.Filters.ToList())
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
                    default:
                        break;
                }
            }

            Thread thread = new Thread(() => WaitForLoadedUpdateHeaders());
            thread.Start();

            PixelateSize = ProcessingUserControlViewModel.PixelateSize;
            MedianSize = ProcessingUserControlViewModel.MedianSize;
            GaussianBlurAmount = ProcessingUserControlViewModel.GaussianBlurAmount;
            BoxBlurAmount = ProcessingUserControlViewModel.BoxBlurAmount;
        }

        private void UpdateHeaders()
        {
            if (!window.IsLoaded)
                return;

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

        private void WaitForLoadedUpdateHeaders()
        {
            window.Dispatcher.Invoke(() =>
            {
                while (!window.IsLoaded) { }
                UpdateHeaders();
            });
        }
    }
}

using ImageProcessing.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static ImageProcessing.ViewModels.ProcessingUserControlViewModel;

namespace ImageProcessing.ViewModels
{
    public class EffectsWindowViewModel : INotifyPropertyChanged
    {
        private bool invertFilter;
        private bool sepiaToneFilter;
        private bool embossFilter;
        private int pixelateSize;
        private int medianSize;
        private bool pixelate;
        private bool medianFilter;
        private bool gaussianBlurFilter;
        private int gaussianBlurAmount;
        private bool boxBlurFilter;
        private int boxBlurAmount;
        private bool blurFilters;

        private EffectsWindow window;
        public ProcessingUserControlViewModel ProcessingUserControlViewModel;

        public bool BlurFilters
        {
            get { return blurFilters; }
            set
            {
                blurFilters = value;
                OnPropertyChanged(nameof(BlurFilters));

                if (!value)
                {
                    BoxBlurFilter = false;
                    GaussianBlurFilter = false;
                }

                ProcessingUserControlViewModel.BlurFilters = value;
            }
        }

        public bool BoxBlurFilter
        {
            get { return boxBlurFilter; }
            set
            {
                boxBlurFilter = value;
                OnPropertyChanged(nameof(boxBlurFilter));

                if (value)
                {
                    int gaussianIndex = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.GaussianBlur);
                    if (gaussianIndex == -1)
                        ProcessingUserControlViewModel.Filters.Add(FilterType.BoxBlur);
                    else
                    {
                        GaussianBlurFilter = false;
                        ProcessingUserControlViewModel.Filters.Insert(gaussianIndex, FilterType.BoxBlur);
                    }
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.BoxBlur);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.BoxBlur);
                if (index != -1)
                    window.groupBoxBoxBlurFilter.Header = "Box Blur Filter";
                else
                    window.groupBoxBoxBlurFilter.Header = $"Box Blur Filter - {index + 1}";
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

        public bool GaussianBlurFilter
        {
            get { return gaussianBlurFilter; }
            set
            {
                gaussianBlurFilter = value;
                OnPropertyChanged(nameof(GaussianBlurFilter));

                if (value)
                {
                    int boxIndex = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.BoxBlur);
                    if (boxIndex == -1)
                        ProcessingUserControlViewModel.Filters.Add(FilterType.GaussianBlur);
                    else
                    {
                        BoxBlurFilter = false;
                        ProcessingUserControlViewModel.Filters.Insert(boxIndex, FilterType.GaussianBlur);
                    }
                }
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.GaussianBlur);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.GaussianBlur);
                if (index == -1)
                    window.groupBoxGaussianBlurFilter.Header = "Gaussian Blur Filter";
                else
                    window.groupBoxGaussianBlurFilter.Header = $"Gaussian Blur Filter - {index + 1}";
            }
        }

        public bool MedianFilter
        {
            get { return medianFilter; }
            set
            {
                medianFilter = value;
                OnPropertyChanged(nameof(medianFilter));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Median);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Median);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.Median);
                if (index == -1)
                    window.groupBoxMedianFilter.Header = "Median Filter";
                else
                    window.groupBoxMedianFilter.Header = $"Median Filter - {index + 1}";
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

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.Pixelate);
                if (index == -1)
                    window.groupBoxPixelate.Header = "Pixelate";
                else
                    window.groupBoxPixelate.Header = $"Pixelate - {index + 1}";
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

        public bool EmbossFilter
        {
            get { return embossFilter; }
            set
            {
                embossFilter = value;
                OnPropertyChanged(nameof(EmbossFilter));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Emboss);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Emboss);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.Emboss);
                if (index == -1)
                    window.toggleSwitchEmboss.Header = null;
                else
                    window.toggleSwitchEmboss.Header = (index + 1).ToString();
            }
        }

        public bool SepiaToneFilter
        {
            get { return sepiaToneFilter; }
            set
            {
                sepiaToneFilter = value;
                OnPropertyChanged(nameof(SepiaToneFilter));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Sepia);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Sepia);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.Sepia);
                if (index == -1)
                    window.toggleSwitchSepiaTone.Header = null;
                else
                    window.toggleSwitchSepiaTone.Header = (index + 1).ToString();
            }
        }

        public bool InvertFilter
        {
            get { return invertFilter; }
            set
            {
                invertFilter = value;
                OnPropertyChanged(nameof(InvertFilter));

                if (value)
                    ProcessingUserControlViewModel.Filters.Add(FilterType.Invert);
                else
                {
                    FilterType filterType = ProcessingUserControlViewModel.Filters.FirstOrDefault(x => x == FilterType.Invert);
                    if (filterType != FilterType.Invalid)
                        ProcessingUserControlViewModel.Filters.Remove(filterType);
                }

                int index = ProcessingUserControlViewModel.Filters.IndexOf(FilterType.Invert);
                if (index == -1)
                    window.toggleSwitchInvert.Header = null;
                else
                    window.toggleSwitchInvert.Header = (index + 1).ToString();
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
                        InvertFilter = true;
                        break;
                    case FilterType.Sepia:
                        SepiaToneFilter = true;
                        break;
                    case FilterType.Emboss:
                        EmbossFilter = true;
                        break;
                    case FilterType.Pixelate:
                        Pixelate = true;
                        break;
                    case FilterType.Median:
                        MedianFilter = true;
                        break;
                    case FilterType.BoxBlur:
                        BoxBlurFilter = true;
                        break;
                    case FilterType.GaussianBlur:
                        GaussianBlurFilter = true;
                        break;
                    default:
                        break;
                }
            }

            PixelateSize = ProcessingUserControlViewModel.PixelateSize;
            MedianSize = ProcessingUserControlViewModel.MedianSize;
            BlurFilters = ProcessingUserControlViewModel.BlurFilters;
            GaussianBlurAmount = ProcessingUserControlViewModel.GaussianBlurAmount;
            BoxBlurAmount = ProcessingUserControlViewModel.BoxBlurAmount;
        }
    }
}

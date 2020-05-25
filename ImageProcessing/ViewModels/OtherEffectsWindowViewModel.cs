﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ImageProcessing.ViewModels
{
    public class OtherEffectsWindowViewModel : INotifyPropertyChanged
    {
        private int pixelateSize;
        private int medianSize;
        private bool pixelate;
        private bool medianFilter;
        private bool gaussianBlurFilter;
        private int gaussianBlurAmount;
        private bool meanBlurFilter;
        private int meanBlurAmount;
        private bool blurFilters;

        public ProcessingUserControlViewModel ProcessingUserControlViewModel;

        public bool BlurFilters
        {
            get { return blurFilters; }
            set
            {
                blurFilters = value;
                OnPropertyChanged(nameof(BlurFilters));

                ProcessingUserControlViewModel.BlurFilters = value;
            }
        }

        public bool MeanBlurFilter
        {
            get { return meanBlurFilter; }
            set
            {
                meanBlurFilter = value;
                OnPropertyChanged(nameof(meanBlurFilter));

                ProcessingUserControlViewModel.MeanBlurFilter = value;
            }
        }

        public int MeanBlurAmount
        {
            get { return meanBlurAmount; }
            set
            {
                meanBlurAmount = value;
                OnPropertyChanged(nameof(MeanBlurAmount));

                ProcessingUserControlViewModel.MeanBlurAmount = value;
            }
        }

        public int GaussianBlurAmount
        {
            get { return gaussianBlurAmount; }
            set
            {
                gaussianBlurAmount = value;
                OnPropertyChanged(nameof(GaussianBlurAmount));

                ProcessingUserControlViewModel.GaussianBlurAmount = gaussianBlurAmount;
            }
        }

        public bool GaussianBlurFilter
        {
            get { return gaussianBlurFilter; }
            set
            {
                gaussianBlurFilter = value;
                OnPropertyChanged(nameof(GaussianBlurFilter));

                ProcessingUserControlViewModel.GaussianBlurFilter = value;
            }
        }

        public bool MedianFilter
        {
            get { return medianFilter; }
            set
            {
                medianFilter = value;
                OnPropertyChanged(nameof(medianFilter));

                ProcessingUserControlViewModel.MedianFilter = value;
            }
        }

        public bool Pixelate
        {
            get { return pixelate; }
            set
            {
                pixelate = value;
                OnPropertyChanged(nameof(Pixelate));

                ProcessingUserControlViewModel.Pixelate = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public OtherEffectsWindowViewModel(ProcessingUserControlViewModel processingUserControlViewModel)
        {
            ProcessingUserControlViewModel = processingUserControlViewModel;
            Pixelate = ProcessingUserControlViewModel.Pixelate;
            MedianFilter = ProcessingUserControlViewModel.MedianFilter;
            PixelateSize = ProcessingUserControlViewModel.PixelateSize;
            MedianSize = ProcessingUserControlViewModel.MedianSize;
            GaussianBlurFilter = ProcessingUserControlViewModel.GaussianBlurFilter;
            GaussianBlurAmount = ProcessingUserControlViewModel.GaussianBlurAmount;
            MeanBlurFilter = ProcessingUserControlViewModel.MeanBlurFilter;
            MeanBlurAmount = ProcessingUserControlViewModel.MeanBlurAmount;
        }
    }
}

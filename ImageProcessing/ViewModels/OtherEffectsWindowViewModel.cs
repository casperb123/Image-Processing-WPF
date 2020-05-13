using System;
using System.Collections.Generic;
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

        public ProcessingUserControlViewModel ProcessingUserControlViewModel;

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

        public OtherEffectsWindowViewModel(ProcessingUserControlViewModel processingUserControlViewModel, bool pixelate, bool medianFilter, int pixelateSize, int medianSize)
        {
            this.ProcessingUserControlViewModel = processingUserControlViewModel;
            Pixelate = pixelate;
            MedianFilter = medianFilter;
            PixelateSize = pixelateSize;
            MedianSize = medianSize;
        }
    }
}

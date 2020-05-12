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

        public bool MedianFilter
        {
            get { return medianFilter; }
            set
            {
                medianFilter = value;
                OnPropertyChanged(nameof(medianFilter));
            }
        }

        public bool Pixelate
        {
            get { return pixelate; }
            set
            {
                pixelate = value;
                OnPropertyChanged(nameof(Pixelate));
            }
        }

        public int MedianSize
        {
            get { return medianSize; }
            set
            {
                medianSize = value;
                OnPropertyChanged(nameof(MedianSize));
            }
        }

        public int PixelateSize
        {
            get { return pixelateSize; }
            set
            {
                pixelateSize = value;
                OnPropertyChanged(nameof(PixelateSize));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public OtherEffectsWindowViewModel(bool pixelate, bool medianFilter, int pixelateSize, int medianSize)
        {
            Pixelate = pixelate;
            MedianFilter = medianFilter;
            PixelateSize = pixelateSize;
            MedianSize = medianSize;
        }
    }
}

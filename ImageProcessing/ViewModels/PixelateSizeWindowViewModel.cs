using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ImageProcessing.ViewModels
{
    public class PixelateSizeWindowViewModel : INotifyPropertyChanged
    {
        private int pixelateSize;

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

        public PixelateSizeWindowViewModel(int pixelateSize)
        {
            PixelateSize = pixelateSize;
        }
    }
}

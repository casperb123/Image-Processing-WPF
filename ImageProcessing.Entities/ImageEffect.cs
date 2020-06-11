using System.ComponentModel;

namespace ImageProcessing.Entities
{
    public class ImageEffect : INotifyPropertyChanged
    {
        public enum FilterType
        {
            Invert,
            SepiaTone,
            Emboss,
            Pixelate,
            Median,
            BoxBlur,
            GaussianBlur,
            EdgeDetection,
            EdgeDetection45Degree,
            EdgeDetectionHorizontal,
            EdgeDetectionVertical,
            EdgeDetectionTopLeft
        }

        private FilterType filter;
        private string effectName;
        private int minimumValue;
        private int maximumValue;
        private int interval;
        private int currentValue;

        public int CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }

        public int MaximumValue
        {
            get { return maximumValue; }
            set
            {
                maximumValue = value;
                OnPropertyChanged(nameof(MaximumValue));
            }
        }

        public int MinimumValue
        {
            get { return minimumValue; }
            set
            {
                minimumValue = value;
                OnPropertyChanged(nameof(MinimumValue));
            }
        }

        public string EffectName
        {
            get { return effectName; }
            set
            {
                effectName = value;
                OnPropertyChanged(nameof(EffectName));
            }
        }

        public FilterType Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public ImageEffect(FilterType filterType, string effectName)
        {
            Filter = filterType;
            EffectName = effectName;
            MinimumValue = -1;
            MaximumValue = -1;
            Interval = -1;
            CurrentValue = -1;
        }

        public ImageEffect(FilterType filterType, string effectName, int minimumValue, int maximumValue, int interval)
            : this(filterType, effectName)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
            Interval = interval;
            CurrentValue = MinimumValue;
        }
    }
}

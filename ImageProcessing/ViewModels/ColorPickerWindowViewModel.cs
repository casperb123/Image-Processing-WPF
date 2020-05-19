using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace ImageProcessing.ViewModels
{
    public class ColorPickerWindowViewModel : INotifyPropertyChanged
    {
		private int red;
		private int green;
		private int blue;
		private int alpha;

		public int Alpha
		{
			get { return alpha; }
			set
			{
				alpha = value;
				OnPropertyChanged(nameof(Alpha));

				Color = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
			}
		}

		public int Blue
		{
			get { return blue; }
			set
			{
				blue = value;
				OnPropertyChanged(nameof(Blue));

				Color = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
			}
		}

		public int Green
		{
			get { return green; }
			set
			{
				green = value;
				OnPropertyChanged(nameof(Green));

				Color = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
			}
		}

		public int Red
		{
			get { return red; }
			set
			{
				red = value;
				OnPropertyChanged(nameof(Red));

				Color = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
			}
		}

		public Color Color { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string prop)
		{
			if (!string.IsNullOrWhiteSpace(prop))
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		public ColorPickerWindowViewModel(int red, int green, int blue, int alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}
	}
}

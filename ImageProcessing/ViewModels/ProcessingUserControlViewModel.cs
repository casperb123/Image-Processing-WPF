using ImageProcessing.UserControls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace ImageProcessing.ViewModels
{
    public class ProcessingUserControlViewModel : INotifyPropertyChanged
    {
        private float minimumHue;
        private float maximumHue;
        private float brightness;
        private float contrast;
        private float gamma;
        private bool showChanges;
        private bool invert;
        private bool sepiaTone;
        private bool grayScale;
        private bool invertedGrayScale;
        private int pixelateSize;
        private int medianSize;
        private bool pixelate;
        private bool medianFilter;

        private MainWindow mainWindow;
        private ProcessingUserControl userControl;
        private Manipulation manipulation;
        private FileOperation fileOperation;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowChanges
        {
            get { return showChanges; }
            set
            {
                showChanges = value;
                OnPropertyChanged(nameof(ShowChanges));
                ToggleImages();
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
                    userControl.buttonPixelateSize.Background = new SolidColorBrush(Color.FromRgb(0, 150, 0));
                else
                    userControl.buttonPixelateSize.Background = new SolidColorBrush(Color.FromRgb(150, 0, 0));
            }
        }

        public bool MedianFilter
        {
            get { return medianFilter; }
            set
            {
                medianFilter = value;
                OnPropertyChanged(nameof(MedianFilter));

                if (value)
                    userControl.buttonMedianSize.Background = new SolidColorBrush(Color.FromRgb(0, 150, 0));
                else
                    userControl.buttonMedianSize.Background = new SolidColorBrush(Color.FromRgb(150, 0, 0));
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

        public bool InvertedGrayScale
        {
            get { return invertedGrayScale; }
            private set
            {
                invertedGrayScale = value;
                OnPropertyChanged(nameof(InvertedGrayScale));
            }
        }

        public bool GrayScale
        {
            get { return grayScale; }
            set
            {
                grayScale = value;
                OnPropertyChanged(nameof(GrayScale));
                InvertedGrayScale = !value;
            }
        }

        public bool SepiaTone
        {
            get { return sepiaTone; }
            set
            {
                sepiaTone = value;
                OnPropertyChanged(nameof(SepiaTone));
            }
        }

        public bool Invert
        {
            get { return invert; }
            set
            {
                invert = value;
                OnPropertyChanged(nameof(Invert));
            }
        }

        public Bitmap ModifiedImage { get; set; }

        public Bitmap OriginalImage { get; set; }

        public float Gamma
        {
            get { return gamma; }
            set
            {
                gamma = value;
                OnPropertyChanged(nameof(Gamma));
            }
        }

        public float Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                OnPropertyChanged(nameof(Contrast));
            }
        }

        public float Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnPropertyChanged(nameof(Brightness));
            }
        }

        public float MaximumHue
        {
            get { return maximumHue; }
            set
            {
                maximumHue = value;
                OnPropertyChanged(nameof(MaximumHue));
            }
        }

        public float MinimumHue
        {
            get { return minimumHue; }
            set
            {
                minimumHue = value;
                OnPropertyChanged(nameof(MinimumHue));
            }
        }

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ProcessingUserControlViewModel(MainWindow mainWindow, ProcessingUserControl userControl)
        {
            MaximumHue = 360;
            Contrast = 1;
            Gamma = 1;
            showChanges = true;
            invertedGrayScale = true;
            PixelateSize = 1;
            MedianSize = 3;

            this.mainWindow = mainWindow;
            this.userControl = userControl;
            manipulation = new Manipulation();
            fileOperation = new FileOperation();

            manipulation.ImageFinished += OnImageFinished;

            userControl.buttonPixelateSize.Background = new SolidColorBrush(Color.FromRgb(150, 0, 0));
            userControl.buttonMedianSize.Background = new SolidColorBrush(Color.FromRgb(150, 0, 0));
        }

        private void OnImageFinished(object sender, Manipulation.ImageEventArgs e)
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                ModifiedImage = e.Bitmap;
                DisplayImage(e.Bitmap, 2);
            });
        }

        public void DisplayImage(Bitmap bitmap, int window)
        {
            ImageSource source = fileOperation.BitmapToBitmapImage(bitmap);

            if (window == 1)
                userControl.imageOriginal.Source = source;
            else if (window == 2)
                userControl.imageManipulated.Source = source;
            else if (window == 3)
            {
                userControl.imageOriginal.Source = source;
                userControl.imageManipulated.Source = source;
            }
        }

        public void OpenFile()
        {
            Bitmap bitmap = fileOperation.OpenFile();
            if (bitmap is null)
                return;

            OriginalImage = bitmap;
            DisplayImage(OriginalImage, 3);
        }

        public async void ResetImage()
        {
            MessageDialogResult result = await mainWindow.ShowMessageAsync("Reset Image", "Are you sure that you want to reset the image?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative)
                return;

            MinimumHue = 0;
            MaximumHue = 360;
            Brightness = 0;
            Contrast = 1;
            Gamma = 1;
            Invert = false;
            SepiaTone = false;
            PixelateSize = 1;
            MedianSize = 3;
            Pixelate = false;
            MedianFilter = false;

            ModifiedImage = new Bitmap(OriginalImage);
            DisplayImage(ModifiedImage, 2);
        }

        public void ToggleImages()
        {
            if (userControl.borderManipulated.Visibility == Visibility.Visible)
            {
                userControl.borderManipulated.Visibility = Visibility.Hidden;
                userControl.borderOriginal.Visibility = Visibility.Visible;
            }
            else
            {
                userControl.borderOriginal.Visibility = Visibility.Hidden;
                userControl.borderManipulated.Visibility = Visibility.Visible;
            }
        }

        public void SetBoxColors()
        {
            System.Drawing.Color colorMin = manipulation.Hue(MinimumHue);
            System.Drawing.Color colorMax = manipulation.Hue(MaximumHue);
            System.Windows.Media.Color brushColorMin = System.Windows.Media.Color.FromArgb(colorMin.A, colorMin.R, colorMin.G, colorMin.B);
            System.Windows.Media.Color brushColorMax = System.Windows.Media.Color.FromArgb(colorMax.A, colorMax.R, colorMax.G, colorMax.B);
            Brush brushMin = new SolidColorBrush(brushColorMin);
            Brush brushMax = new SolidColorBrush(brushColorMax);

            userControl.rectangleColorMin.Fill = brushMin;
            userControl.rectangleColorMax.Fill = brushMax;
        }

        public void ModifyImage()
        {
            Thread thread = new Thread(() => manipulation.Modify(OriginalImage, MinimumHue, MaximumHue, Brightness, Contrast, Gamma, GrayScale, Invert, SepiaTone, pixelate, medianFilter, PixelateSize, MedianSize));
            thread.Start();
        }

        public async void SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)userControl.imageManipulated.Source));

                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                await mainWindow.ShowMessageAsync("Image Saved", "The image has been successfully saved!");
            }
        }
    }
}

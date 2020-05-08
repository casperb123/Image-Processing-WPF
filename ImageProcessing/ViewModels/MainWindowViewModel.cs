using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

namespace ImageProcessing.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double minimumHue;
        private double maximumHue;
        private double brightness;
        private double contrast;
        private double gamma;
        private Bitmap originalImage;
        private Bitmap modifiedImage;
        private bool showChanges;
        private bool invert;
        private bool sepiaTone;
        private bool grayScale;
        private bool invertedGrayScale;

        private MainWindow mainWindow;
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

        public bool InvertedGrayScale
        {
            get { return invertedGrayScale; }
            private set
            {
                invertedGrayScale = value;
                OnPropertyChanged(nameof(InvertedGrayScale));
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

        public Bitmap ModifiedImage
        {
            get { return modifiedImage; }
            set { modifiedImage = value; }
        }

        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set { originalImage = value; }
        }

        public double Gamma
        {
            get { return gamma; }
            set
            {
                gamma = value;
                OnPropertyChanged(nameof(Gamma));
            }
        }

        public double Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                OnPropertyChanged(nameof(Contrast));
            }
        }

        public double Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnPropertyChanged(nameof(Brightness));
            }
        }

        public double MaximumHue
        {
            get { return maximumHue; }
            set
            {
                maximumHue = value;
                OnPropertyChanged(nameof(MaximumHue));
            }
        }

        public double MinimumHue
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

        public MainWindowViewModel(MainWindow mainWindow)
        {
            MaximumHue = 360;
            Contrast = 1;
            Gamma = 1;
            showChanges = true;
            invertedGrayScale = true;

            this.mainWindow = mainWindow;
            manipulation = new Manipulation();
            fileOperation = new FileOperation();

            manipulation.ImageFinished += OnImageFinished;
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
                mainWindow.imageOriginal.Source = source;
            else if (window == 2)
                mainWindow.imageManipulated.Source = source;
            else if (window == 3)
            {
                mainWindow.imageOriginal.Source = source;
                mainWindow.imageManipulated.Source = source;
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

            ModifiedImage = new Bitmap(OriginalImage);
            DisplayImage(ModifiedImage, 2);
        }

        public void ToggleImages()
        {
            if (mainWindow.borderManipulated.Visibility == Visibility.Visible)
            {
                mainWindow.borderManipulated.Visibility = Visibility.Hidden;
                mainWindow.borderOriginal.Visibility = Visibility.Visible;
            }
            else
            {
                mainWindow.borderOriginal.Visibility = Visibility.Hidden;
                mainWindow.borderManipulated.Visibility = Visibility.Visible;
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

            mainWindow.rectangleColorMin.Fill = brushMin;
            mainWindow.rectangleColorMax.Fill = brushMax;
        }

        public void ModifyImage()
        {
            Thread thread = new Thread(() => manipulation.Modify(originalImage, MinimumHue, MaximumHue, (float)Brightness, (float)Contrast, (float)Gamma, GrayScale, Invert, SepiaTone));
            thread.Start();
        }

        public async void SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)mainWindow.imageManipulated.Source));

                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                await mainWindow.ShowMessageAsync("Image Saved", "The image has been successfully saved!");
            }
        }
    }
}
